﻿
#region license

/*
Copyright (c) 2013, Milosz Krajewski
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided 
that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions 
  and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
  and the following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR 
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE 
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN 
IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion

using System;
using System.Diagnostics;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace LZ4s
{
    /// <summary>Safe LZ4 codec.</summary>
    public static partial class LZ4Codec
    {

        #region configuration

        /// <summary>
        /// Memory usage formula : N->2^N Bytes (examples : 10 -> 1KB; 12 -> 4KB ; 16 -> 64KB; 20 -> 1MB; etc.)
        /// Increasing memory usage improves compression ratio
        /// Reduced memory usage can improve speed, due to cache effect
        /// Default value is 14, for 16KB, which nicely fits into Intel x86 L1 cache
        /// </summary>
        private const int MEMORY_USAGE = 14;

        /// <summary>
        /// Decreasing this value will make the algorithm skip faster data segments considered "incompressible"
        /// This may decrease compression ratio dramatically, but will be faster on incompressible data
        /// Increasing this value will make the algorithm search more before declaring a segment "incompressible"
        /// This could improve compression a bit, but will be slower on incompressible data
        /// The default value (6) is recommended
        /// </summary>
        private const int NOTCOMPRESSIBLE_DETECTIONLEVEL = 6;

#if LZ4s

        /// <summary>Buffer length when Buffer.BlockCopy becomes faster than straight loop.
        /// Please note that safe implementation REQUIRES it to be greater (not even equal) than 8.</summary>
        private const int BLOCK_COPY_LIMIT = 16;

#endif

        #endregion

        #region consts

        private const int MINMATCH = 4;
#pragma warning disable 162
        private const int SKIPSTRENGTH = NOTCOMPRESSIBLE_DETECTIONLEVEL > 2 ? NOTCOMPRESSIBLE_DETECTIONLEVEL : 2;
#pragma warning restore 162
        private const int COPYLENGTH = 8;
        private const int LASTLITERALS = 5;
        private const int MFLIMIT = COPYLENGTH + MINMATCH;
        private const int MINLENGTH = MFLIMIT + 1;
        private const int MAXD_LOG = 16;
        private const int MAXD = 1 << MAXD_LOG;
        private const int MAXD_MASK = MAXD - 1;
        private const int MAX_DISTANCE = (1 << MAXD_LOG) - 1;
        private const int ML_BITS = 4;
        private const int ML_MASK = (1 << ML_BITS) - 1;
        private const int RUN_BITS = 8 - ML_BITS;
        private const int RUN_MASK = (1 << RUN_BITS) - 1;
        private const int STEPSIZE_64 = 8;
        private const int STEPSIZE_32 = 4;

        private const int LZ4_64KLIMIT = (1 << 16) + (MFLIMIT - 1);

        private const int HASH_LOG = MEMORY_USAGE - 2;
        private const int HASH_TABLESIZE = 1 << HASH_LOG;
        private const int HASH_ADJUST = (MINMATCH*8) - HASH_LOG;

        private const int HASH64K_LOG = HASH_LOG + 1;
        private const int HASH64K_TABLESIZE = 1 << HASH64K_LOG;
        private const int HASH64K_ADJUST = (MINMATCH*8) - HASH64K_LOG;

        private const int HASHHC_LOG = MAXD_LOG - 1;
        private const int HASHHC_TABLESIZE = 1 << HASHHC_LOG;
        private const int HASHHC_ADJUST = (MINMATCH*8) - HASHHC_LOG;
        //private const int HASHHC_MASK = HASHHC_TABLESIZE - 1;

        private static readonly int[] DECODER_TABLE_32 = {0, 3, 2, 3, 0, 0, 0, 0};
        private static readonly int[] DECODER_TABLE_64 = {0, 0, 0, -1, 0, 1, 2, 3};

        private static readonly int[] DEBRUIJN_TABLE_32 =
        {
            0, 0, 3, 0, 3, 1, 3, 0, 3, 2, 2, 1, 3, 2, 0, 1,
            3, 3, 1, 2, 2, 2, 2, 0, 3, 1, 2, 0, 1, 0, 1, 1
        };

        private static readonly int[] DEBRUIJN_TABLE_64 =
        {
            0, 0, 0, 0, 0, 1, 1, 2, 0, 3, 1, 3, 1, 4, 2, 7,
            0, 2, 3, 6, 1, 5, 3, 5, 1, 3, 4, 4, 2, 5, 6, 7,
            7, 0, 1, 2, 3, 3, 4, 6, 2, 6, 5, 5, 3, 4, 5, 6,
            7, 1, 2, 4, 6, 4, 4, 5, 7, 2, 6, 5, 7, 6, 7, 7
        };

        private const int MAX_NB_ATTEMPTS = 256;
        private const int OPTIMAL_ML = (ML_MASK - 1) + MINMATCH;

        #endregion

        #region public interface (common)

        /// <summary>Gets maximum the length of the output.</summary>
        /// <param name="inputLength">Length of the input.</param>
        /// <returns>Maximum number of bytes needed for compressed buffer.</returns>
        public static int MaximumOutputLength(int inputLength)
        {
            return inputLength + (inputLength/255) + 16;
        }

        #endregion

        #region internal interface (common)

        internal static void CheckArguments(
            byte[] input, int inputOffset, ref int inputLength,
            byte[] output, int outputOffset, ref int outputLength)
        {
            if (inputLength < 0) inputLength = input.Length - inputOffset;
            if (inputLength == 0)
            {
                outputLength = 0;
                return;
            }

            if (input == null) throw new ArgumentNullException("input");
            if (inputOffset < 0 || inputOffset + inputLength > input.Length)
                throw new ArgumentException("inputOffset and inputLength are invalid for given input");

            if (outputLength < 0) outputLength = output.Length - outputOffset;
            if (output == null) throw new ArgumentNullException("output");
            if (outputOffset < 0 || outputOffset + outputLength > output.Length)
                throw new ArgumentException("outputOffset and outputLength are invalid for given output");
        }

        #endregion

        #region Helper

        // ReSharper disable UnusedParameter.Local

        [Conditional("DEBUG")]
        private static void Assert(bool condition, string errorMessage)
        {
            if (!condition) throw new ArgumentException(errorMessage);
            Debug.Assert(condition, errorMessage);
        }

        // ReSharper restore UnusedParameter.Local

        #endregion

        #region Byte manipulation

        // ReSharper disable RedundantCast

        internal static void Poke2(byte[] buffer, int offset, ushort value)
        {
            buffer[offset] = (byte) value;
            buffer[offset + 1] = (byte) (value >> 8);
        }

        internal static ushort Peek2(byte[] buffer, int offset)
        {
            // NOTE: It's faster than BitConverter.ToUInt16 (suprised? me too)
            return (ushort) (((uint) buffer[offset]) | ((uint) buffer[offset + 1] << 8));
        }

        internal static uint Peek4(byte[] buffer, int offset)
        {
            // NOTE: It's faster than BitConverter.ToUInt32 (suprised? me too)
            return
                ((uint) buffer[offset]) |
                ((uint) buffer[offset + 1] << 8) |
                ((uint) buffer[offset + 2] << 16) |
                ((uint) buffer[offset + 3] << 24);
        }

        private static uint Xor4(byte[] buffer, int offset1, int offset2)
        {
            // return Peek4(buffer, offset1) ^ Peek4(buffer, offset2);
            var value1 =
                ((uint) buffer[offset1]) |
                ((uint) buffer[offset1 + 1] << 8) |
                ((uint) buffer[offset1 + 2] << 16) |
                ((uint) buffer[offset1 + 3] << 24);
            var value2 =
                ((uint) buffer[offset2]) |
                ((uint) buffer[offset2 + 1] << 8) |
                ((uint) buffer[offset2 + 2] << 16) |
                ((uint) buffer[offset2 + 3] << 24);
            return value1 ^ value2;
        }

        private static ulong Xor8(byte[] buffer, int offset1, int offset2)
        {
            // return Peek8(buffer, offset1) ^ Peek8(buffer, offset2);
            var value1 =
                ((ulong) buffer[offset1]) |
                ((ulong) buffer[offset1 + 1] << 8) |
                ((ulong) buffer[offset1 + 2] << 16) |
                ((ulong) buffer[offset1 + 3] << 24) |
                ((ulong) buffer[offset1 + 4] << 32) |
                ((ulong) buffer[offset1 + 5] << 40) |
                ((ulong) buffer[offset1 + 6] << 48) |
                ((ulong) buffer[offset1 + 7] << 56);
            var value2 =
                ((ulong) buffer[offset2]) |
                ((ulong) buffer[offset2 + 1] << 8) |
                ((ulong) buffer[offset2 + 2] << 16) |
                ((ulong) buffer[offset2 + 3] << 24) |
                ((ulong) buffer[offset2 + 4] << 32) |
                ((ulong) buffer[offset2 + 5] << 40) |
                ((ulong) buffer[offset2 + 6] << 48) |
                ((ulong) buffer[offset2 + 7] << 56);
            return value1 ^ value2;
        }

        private static bool Equal2(byte[] buffer, int offset1, int offset2)
        {
            // return Peek2(buffer, offset1) == Peek2(buffer, offset2);
            if (buffer[offset1] != buffer[offset2]) return false;
            return buffer[offset1 + 1] == buffer[offset2 + 1];
        }

        private static bool Equal4(byte[] buffer, int offset1, int offset2)
        {
            // return Peek4(buffer, offset1) == Peek4(buffer, offset2);
            if (buffer[offset1] != buffer[offset2]) return false;
            if (buffer[offset1 + 1] != buffer[offset2 + 1]) return false;
            if (buffer[offset1 + 2] != buffer[offset2 + 2]) return false;
            return buffer[offset1 + 3] == buffer[offset2 + 3];
        }

        // ReSharper restore RedundantCast

        #endregion

        #region Byte block copy

        private static void Copy4(byte[] buf, int src, int dst)
        {
            Assert(dst > src, "Copying backwards is not implemented");
            buf[dst + 3] = buf[src + 3];
            buf[dst + 2] = buf[src + 2];
            buf[dst + 1] = buf[src + 1];
            buf[dst] = buf[src];
        }

        private static void Copy8(byte[] buf, int src, int dst)
        {
            Assert(dst > src, "Copying backwards is not implemented");
            buf[dst + 7] = buf[src + 7];
            buf[dst + 6] = buf[src + 6];
            buf[dst + 5] = buf[src + 5];
            buf[dst + 4] = buf[src + 4];
            buf[dst + 3] = buf[src + 3];
            buf[dst + 2] = buf[src + 2];
            buf[dst + 1] = buf[src + 1];
            buf[dst] = buf[src];
        }

        private static void BlockCopy(byte[] src, int src_0, byte[] dst, int dst_0, int len)
        {
            Assert(src != dst, "BlockCopy does not handle copying to the same buffer");

            if (len >= BLOCK_COPY_LIMIT)
            {
                Buffer.BlockCopy(src, src_0, dst, dst_0, len);
            }
            else
            {
                while (len >= 8)
                {
                    dst[dst_0] = src[src_0];
                    dst[dst_0 + 1] = src[src_0 + 1];
                    dst[dst_0 + 2] = src[src_0 + 2];
                    dst[dst_0 + 3] = src[src_0 + 3];
                    dst[dst_0 + 4] = src[src_0 + 4];
                    dst[dst_0 + 5] = src[src_0 + 5];
                    dst[dst_0 + 6] = src[src_0 + 6];
                    dst[dst_0 + 7] = src[src_0 + 7];
                    len -= 8;
                    src_0 += 8;
                    dst_0 += 8;
                }

                while (len >= 4)
                {
                    dst[dst_0] = src[src_0];
                    dst[dst_0 + 1] = src[src_0 + 1];
                    dst[dst_0 + 2] = src[src_0 + 2];
                    dst[dst_0 + 3] = src[src_0 + 3];
                    len -= 4;
                    src_0 += 4;
                    dst_0 += 4;
                }

                while (len-- > 0)
                {
                    dst[dst_0++] = src[src_0++];
                }
            }
        }

        private static int WildCopy(byte[] src, int src_0, byte[] dst, int dst_0, int dst_end)
        {
            var len = dst_end - dst_0;

            Assert(src != dst, "BlockCopy does not handle copying to the same buffer");
            Assert(len > 0, "Length have to be greater than 0");

            if (len >= BLOCK_COPY_LIMIT)
            {
                Buffer.BlockCopy(src, src_0, dst, dst_0, len);
            }
            else
            {
                // apparently (tested) this is an overkill
                // it seems to be faster without this 8-byte loop
                //while (len >= 8)
                //{
                //	dst[dst_0] = src[src_0];
                //	dst[dst_0 + 1] = src[src_0 + 1];
                //	dst[dst_0 + 2] = src[src_0 + 2];
                //	dst[dst_0 + 3] = src[src_0 + 3];
                //	dst[dst_0 + 4] = src[src_0 + 4];
                //	dst[dst_0 + 5] = src[src_0 + 5];
                //	dst[dst_0 + 6] = src[src_0 + 6];
                //	dst[dst_0 + 7] = src[src_0 + 7];
                //	len -= 8; src_0 += 8; dst_0 += 8;
                //}

                while (len >= 4)
                {
                    dst[dst_0] = src[src_0];
                    dst[dst_0 + 1] = src[src_0 + 1];
                    dst[dst_0 + 2] = src[src_0 + 2];
                    dst[dst_0 + 3] = src[src_0 + 3];
                    len -= 4;
                    src_0 += 4;
                    dst_0 += 4;
                }

                while (len-- > 0)
                {
                    dst[dst_0++] = src[src_0++];
                }
            }

            return len;
        }

        private static int SecureCopy(byte[] buffer, int src, int dst, int dst_end)
        {
            var diff = dst - src;
            var length = dst_end - dst;
            var len = length;

            Assert(diff >= 4, "Target must be at least 4 bytes further than source");
            Assert(BLOCK_COPY_LIMIT > 4, "This method requires BLOCK_COPY_LIMIT > 4");
            Assert(len > 0, "Length have to be greater than 0");

            if (diff >= BLOCK_COPY_LIMIT)
            {
                if (diff >= length)
                {
                    Buffer.BlockCopy(buffer, src, buffer, dst, length);
                    return length; // done
                }

                do
                {
                    Buffer.BlockCopy(buffer, src, buffer, dst, diff);
                    src += diff;
                    dst += diff;
                    len -= diff;
                } while (len >= diff);
            }

            // apparently (tested) this is an overkill
            // it seems to be faster without this 8-byte loop
            //while (len >= 8)
            //{
            //	buffer[dst] = buffer[src];
            //	buffer[dst + 1] = buffer[src + 1];
            //	buffer[dst + 2] = buffer[src + 2];
            //	buffer[dst + 3] = buffer[src + 3];
            //	buffer[dst + 4] = buffer[src + 4];
            //	buffer[dst + 5] = buffer[src + 5];
            //	buffer[dst + 6] = buffer[src + 6];
            //	buffer[dst + 7] = buffer[src + 7];
            //	dst += 8; src += 8; len -= 8;
            //}

            while (len >= 4)
            {
                buffer[dst] = buffer[src];
                buffer[dst + 1] = buffer[src + 1];
                buffer[dst + 2] = buffer[src + 2];
                buffer[dst + 3] = buffer[src + 3];
                dst += 4;
                src += 4;
                len -= 4;
            }

            while (len-- > 0)
            {
                buffer[dst++] = buffer[src++];
            }

            return length; // done
        }

        #endregion

        #region Encode32

        /// <summary>Encodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <param name="output">The output.</param>
        /// <param name="outputOffset">The output offset.</param>
        /// <param name="outputLength">Length of the output.</param>
        /// <returns>Number of bytes written.</returns>
        public static int Encode32(
            byte[] input,
            int inputOffset,
            int inputLength,
            byte[] output,
            int outputOffset,
            int outputLength)
        {
            CheckArguments(
                input, inputOffset, ref inputLength,
                output, outputOffset, ref outputLength);
            if (outputLength == 0) return 0;

            if (inputLength < LZ4_64KLIMIT)
            {
                var hashTable = new ushort[HASH64K_TABLESIZE];
                return LZ4_compress64kCtx_safe32(hashTable, input, output, inputOffset, outputOffset, inputLength,
                    outputLength);
            }
            else
            {
                var hashTable = new int[HASH_TABLESIZE];
                return LZ4_compressCtx_safe32(hashTable, input, output, inputOffset, outputOffset, inputLength,
                    outputLength);
            }
        }

        /// <summary>Encodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <returns>Compressed buffer.</returns>
        public static byte[] Encode32(byte[] input, int inputOffset, int inputLength)
        {
            if (inputLength < 0) inputLength = input.Length - inputOffset;

            if (input == null) throw new ArgumentNullException("input");
            if (inputOffset < 0 || inputOffset + inputLength > input.Length)
                throw new ArgumentException("inputOffset and inputLength are invalid for given input");

            var result = new byte[MaximumOutputLength(inputLength)];
            var length = Encode32(input, inputOffset, inputLength, result, 0, result.Length);

            if (length != result.Length)
            {
                if (length < 0)
                    throw new InvalidOperationException("Compression has been corrupted");
                var buffer = new byte[length];
                Buffer.BlockCopy(result, 0, buffer, 0, length);
                return buffer;
            }
            return result;
        }

        #endregion

        #region Decode32

        /// <summary>Decodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <param name="output">The output.</param>
        /// <param name="outputOffset">The output offset.</param>
        /// <param name="outputLength">Length of the output.</param>
        /// <param name="knownOutputLength">Set it to <c>true</c> if output length is known.</param>
        /// <returns>Number of bytes written.</returns>
        public static int Decode32(
            byte[] input,
            int inputOffset,
            int inputLength,
            byte[] output,
            int outputOffset,
            int outputLength,
            bool knownOutputLength)
        {
            CheckArguments(
                input, inputOffset, ref inputLength,
                output, outputOffset, ref outputLength);

            if (outputLength == 0) return 0;

            if (knownOutputLength)
            {
                var length = LZ4_uncompress_safe32(input, output, inputOffset, outputOffset, outputLength);
                if (length != inputLength)
                    throw new ArgumentException("LZ4 block is corrupted, or invalid length has been given.");
                return outputLength;
            }
            else
            {
                var length = LZ4_uncompress_unknownOutputSize_safe32(input, output, inputOffset, outputOffset,
                    inputLength, outputLength);
                if (length < 0)
                    throw new ArgumentException("LZ4 block is corrupted, or invalid length has been given.");
                return length;
            }
        }

        /// <summary>Decodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <param name="outputLength">Length of the output.</param>
        /// <returns>Decompressed buffer.</returns>
        public static byte[] Decode32(byte[] input, int inputOffset, int inputLength, int outputLength)
        {
            if (inputLength < 0) inputLength = input.Length - inputOffset;

            if (input == null) throw new ArgumentNullException("input");
            if (inputOffset < 0 || inputOffset + inputLength > input.Length)
                throw new ArgumentException("inputOffset and inputLength are invalid for given input");

            var result = new byte[outputLength];
            var length = Decode32(input, inputOffset, inputLength, result, 0, outputLength, true);
            if (length != outputLength)
                throw new ArgumentException("outputLength is not valid");
            return result;
        }

        #endregion

    }
}

// ReSharper restore InconsistentNaming
// ReSharper restore CheckNamespace
