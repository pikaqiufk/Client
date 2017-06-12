
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
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Win32;

namespace LZ4s
{
	/// <summary>
	///     LZ4 codec selecting best implementation depending on platform.
	/// </summary>
	public static partial class LZ4Codec
	{
		#region Wrap

		private const int WRAP_OFFSET_0 = 0;
		private const int WRAP_OFFSET_4 = sizeof(int);
		private const int WRAP_OFFSET_8 = 2 * sizeof(int);
        private const int WRAP_LENGTH = WRAP_OFFSET_4;

		/// <summary>Sets uint32 value in byte buffer.</summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="value">The value.</param>
		private static void Poke4(byte[] buffer, int offset, uint value)
		{
			buffer[offset + 0] = (byte)value;
			buffer[offset + 1] = (byte)(value >> 8);
			buffer[offset + 2] = (byte)(value >> 16);
			buffer[offset + 3] = (byte)(value >> 24);
		}


		/// <summary>Compresses and wraps given input byte buffer.</summary>
		/// <param name="inputBuffer">The input buffer.</param>
		/// <param name="inputOffset">The input offset.</param>
		/// <param name="inputLength">Length of the input.</param>
		/// <param name="highCompression">if set to <c>true</c> uses high compression.</param>
		/// <returns>Compressed buffer.</returns>
		/// <exception cref="System.ArgumentException">inputBuffer size of inputLength is invalid</exception>
        private static byte[] Wrap(byte[] inputBuffer, int inputOffset, int inputLength, bool highCompression)
        {
            inputLength = Math.Min(inputBuffer.Length - inputOffset, inputLength);
            if (inputLength < 0)
                throw new ArgumentException("inputBuffer size of inputLength is invalid");
            if (inputLength == 0)
                return new byte[WRAP_LENGTH];

            var outputLength = inputLength; // MaximumOutputLength(inputLength);
            var outputBuffer = new byte[outputLength];

            outputLength = Encode32(inputBuffer, inputOffset, inputLength, outputBuffer, 0, outputLength);

            byte[] result;

            if (outputLength >= inputLength || outputLength <= 0)
            {
                result = new byte[inputLength + WRAP_LENGTH];
                Poke4(result, WRAP_OFFSET_0, (uint)inputLength);
                Buffer.BlockCopy(inputBuffer, inputOffset, result, WRAP_OFFSET_4, inputLength);
            }
            else
            {
                result = new byte[outputLength + WRAP_LENGTH];
                Poke4(result, WRAP_OFFSET_0, (uint)inputLength);
                Buffer.BlockCopy(outputBuffer, 0, result, WRAP_OFFSET_4, outputLength);
            }

            return result;
        }

		/// <summary>Compresses and wraps given input byte buffer.</summary>
		/// <param name="inputBuffer">The input buffer.</param>
		/// <param name="inputOffset">The input offset.</param>
		/// <param name="inputLength">Length of the input.</param>
		/// <returns>Compressed buffer.</returns>
		/// <exception cref="System.ArgumentException">inputBuffer size of inputLength is invalid</exception>
		public static byte[] Wrap(byte[] inputBuffer, int inputOffset = 0, int inputLength = int.MaxValue)
		{
			return Wrap(inputBuffer, inputOffset, inputLength, false);
		}

		/// <summary>Compresses (with high compression algorithm) and wraps given input byte buffer.</summary>
		/// <param name="inputBuffer">The input buffer.</param>
		/// <param name="inputOffset">The input offset.</param>
		/// <param name="inputLength">Length of the input.</param>
		/// <returns>Compressed buffer.</returns>
		/// <exception cref="System.ArgumentException">inputBuffer size of inputLength is invalid</exception>
		public static byte[] WrapHC(byte[] inputBuffer, int inputOffset = 0, int inputLength = int.MaxValue)
		{
			return Wrap(inputBuffer, inputOffset, inputLength, true);
		}


	    public static byte[] Unwrap(byte[] inputBuffer)
	    {
	        return Unwrap(inputBuffer, 0, inputBuffer.Length);
	    }

		/// <summary>Unwraps the specified compressed buffer.</summary>
		/// <param name="inputBuffer">The input buffer.</param>
		/// <param name="inputOffset">The input offset.</param>
		/// <returns>Uncompressed buffer.</returns>
		/// <exception cref="System.ArgumentException">
		///     inputBuffer size is invalid or inputBuffer size is invalid or has been corrupted
		/// </exception>
        public static byte[] Unwrap(byte[] inputBuffer, int inputOffset, int length)
        {
            var inputLength = length - inputOffset;
            if (inputLength < WRAP_LENGTH)
                throw new ArgumentException("inputBuffer size is invalid");

            var outputLength = (int)Peek4(inputBuffer, inputOffset + WRAP_OFFSET_0);
            inputLength -= WRAP_LENGTH;

            byte[] result;

            if (inputLength >= outputLength)
            {
                result = new byte[inputLength];
                Buffer.BlockCopy(inputBuffer, inputOffset + WRAP_OFFSET_4, result, 0, inputLength);
            }
            else
            {
                result = new byte[outputLength];
                Decode32(inputBuffer, inputOffset + WRAP_OFFSET_4, inputLength, result, 0, outputLength, true);
            }

            return result;
        }

		#endregion
	}
}