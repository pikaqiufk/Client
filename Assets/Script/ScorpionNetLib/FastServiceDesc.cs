using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace ScorpionNetLib
{
    public class SerializerUtility
    {

        public static int ReadShort(byte[] buffer, int offset = 0)
        {
            return ((buffer[offset] << 8) |
                    (buffer[offset + 1])
                );
        }

        public static void WriteShort(byte[] buffer, short number, int offset = 0)
        {
            buffer[offset] = (byte)((number >> 8) & 0xFF);
            buffer[offset + 1] = (byte)((number) & 0xFF);
        }

        public static int ReadInt(byte[] buffer, int offset = 0)
        {
            return ((buffer[offset] << 24) |
                    (buffer[offset + 1] << 16) |
                    (buffer[offset + 2] << 8) |
                    (buffer[offset + 3])
                );
        }

        public static void WriteInt(byte[] buffer, int number, int offset = 0)
        {
            buffer[offset] = (byte)((number >> 24) & 0xFF);
            buffer[offset + 1] = (byte)((number >> 16) & 0xFF);
            buffer[offset + 2] = (byte)((number >> 8) & 0xFF);
            buffer[offset + 3] = (byte)((number) & 0xFF);
        }

        public static long ReadLongOpt(Stream s, byte[] buffer)
        {
            s.Read(buffer, 0, 1);
            int opt = buffer[0];
            if (opt == 1)
            {
                s.Read(buffer, 1, 1);
                return buffer[1];
            }
            else if (opt == 2)
            {
                s.Read(buffer, 1, 2);
                return ReadShort(buffer, 1);
            }
            else if (opt == 3)
            {
                s.Read(buffer, 1, 4);
                return ReadInt(buffer, 1);
            }
            else
            {
                s.Read(buffer, 1, 8);
                return ReadLong(buffer, 1);
            }
        }

        public static void WriteLongOpt(Stream s, byte[] buffer, long value)
        {
            if ((ulong)value < 256)
            {
                buffer[0] = 1;
                buffer[1] = (byte)value;
                s.Write(buffer, 0, 2);
            }
            else if ((ulong)value < UInt16.MaxValue)
            {
                buffer[0] = 2;
                WriteShort(buffer, (short)value, 1);
                s.Write(buffer, 0, 3);
            }
            else if ((ulong)value < UInt32.MaxValue)
            {
                buffer[0] = 3;
                WriteInt(buffer, (int)value, 1);
                s.Write(buffer, 0, 5);
            }
            else
            {
                buffer[0] = 4;
                WriteLong(buffer, value, 1);
                s.Write(buffer, 0, 9);
            }
        }

        public static long ReadLong(byte[] buffer, int offset = 0)
        {
            long result = 0;
            result |= ((long)buffer[offset] << 56);
            result |= ((long)buffer[offset + 1] << 48);
            result |= ((long)buffer[offset + 2] << 40);
            result |= ((long)buffer[offset + 3] << 32);
            result |= ((long)buffer[offset + 4] << 24);
            result |= ((long)buffer[offset + 5] << 16);
            result |= ((long)buffer[offset + 6] << 8);
            result |= ((long)buffer[offset + 7]);
            return result;
        }

        public static void WriteLong(byte[] buffer, long number, int offset = 0)
        {
            buffer[offset] = (byte)((number >> 56) & 0xFF);
            buffer[offset + 1] = (byte)((number >> 48) & 0xFF);
            buffer[offset + 2] = (byte)((number >> 40) & 0xFF);
            buffer[offset + 3] = (byte)((number >> 32) & 0xFF);
            buffer[offset + 4] = (byte)((number >> 24) & 0xFF);
            buffer[offset + 5] = (byte)((number >> 16) & 0xFF);
            buffer[offset + 6] = (byte)((number >> 8) & 0xFF);
            buffer[offset + 7] = (byte)((number) & 0xFF);
        }

        public static void Serialize(Stream s, ServiceDesc desc)
        {
            ServiceDesc.Serialize(s, desc);
        }

        public static ServiceDesc Deserialize(Stream s)
        {
            return ServiceDesc.Deserialize(s);
        }
    }

    public class ServiceDesc
    {
        private int _Type;
        public int Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private int _ServiceType = default(int);
        public int ServiceType
        {
            get { return _ServiceType; }
            set { _ServiceType = value; }
        }

        private uint _FuncId = default(uint);
        public uint FuncId
        {
            get { return _FuncId; }
            set { _FuncId = value; }
        }

        private uint _PacketId = default(uint);
        public uint PacketId
        {
            get { return _PacketId; }
            set { _PacketId = value; }
        }

        private uint _Error = default(uint);
        public uint Error
        {
            get { return _Error; }
            set { _Error = value; }
        }

        private ulong _ClientId = default(ulong);
        public ulong ClientId
        {
            get { return _ClientId; }
            set { _ClientId = value; }
        }

        private ulong _CharacterId = default(ulong);
        public ulong CharacterId
        {
            get { return _CharacterId; }
            set { _CharacterId = value; }
        }

        private List<ulong> _Routing;
        public List<ulong> Routing
        {
            get
            {
                if (_Routing == null)
                    _Routing = new List<ulong>();
                return _Routing;
            }
        }

        private byte[] _Data = null;

        public byte[] Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        private static readonly byte[] sEmptyByteArray = new byte[0];
        public static void Serialize(Stream s, ServiceDesc desc)
        {
            if (desc == null)
                return;

            if (!s.CanWrite || !s.CanSeek)
                return;

            byte[] buffer = new byte[9];
            var pos = s.Position;
            s.WriteByte(0);
            byte mask = 0;
            buffer[0] = (byte)desc._Type;
            s.Write(buffer, 0, 1);
            if (desc._ServiceType != default(int))
            {
                mask |= 1;
                buffer[0] = (byte)desc._ServiceType;
                s.Write(buffer, 0, 1);
            }
            if (desc._FuncId != default(int))
            {
                mask |= 2;
                SerializerUtility.WriteInt(buffer, (int)desc._FuncId);
                s.Write(buffer, 0, 4);
            }
            if (desc._PacketId != default(int))
            {
                mask |= 4;
                SerializerUtility.WriteInt(buffer, (int)desc._PacketId);
                s.Write(buffer, 0, 4);
            }
            if (desc._Error != default(int))
            {
                mask |= 8;
                SerializerUtility.WriteShort(buffer, (short)desc._Error);
                s.Write(buffer, 0, 2);
            }
            if (desc._ClientId != default(ulong))
            {
                mask |= 16;
                SerializerUtility.WriteLongOpt(s, buffer, (long)desc._ClientId);
            }
            if (desc._CharacterId != default(ulong))
            {
                mask |= 32;
                SerializerUtility.WriteLongOpt(s, buffer, (long)desc._CharacterId);
            }
            if (desc._Routing != null && desc._Routing.Count != 0)
            {
                mask |= 64;
                SerializerUtility.WriteShort(buffer, (short)desc._Routing.Count);
                s.Write(buffer, 0, 2);
                {
                    var __list1 = desc._Routing;
                    var __listCount1 = __list1.Count;
                    for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                    {
                        var r = __list1[__i1];
                        {
                            SerializerUtility.WriteLongOpt(s, buffer, (long)r);
                        }
                    }
                }
            }
            if (desc.Data != null && desc.Data.Length != 0)
            {
                mask |= 128;
                SerializerUtility.WriteInt(buffer, desc.Data.Length);
                s.Write(buffer, 0, 4);
                s.Write(desc.Data, 0, desc.Data.Length);
            }

            buffer[0] = mask;
            var last = s.Position;
            s.Seek(pos, SeekOrigin.Begin);
            s.Write(buffer, 0, 1);
            s.Seek(last, SeekOrigin.Begin);
        }

        public static ServiceDesc Deserialize(Stream s)
        {
            ServiceDesc desc = new ServiceDesc();
            byte[] buffer = new byte[9];
            s.Read(buffer, 0, 1);
            byte mask = buffer[0];
            s.Read(buffer, 0, 1);
            desc._Type = buffer[0];
            if ((mask & 1) != 0)
            {
                s.Read(buffer, 0, 1);
                desc._ServiceType = buffer[0];
            }
            if ((mask & 2) != 0)
            {
                s.Read(buffer, 0, 4);
                desc._FuncId = (uint)SerializerUtility.ReadInt(buffer);
            }
            if ((mask & 4) != 0)
            {
                s.Read(buffer, 0, 4);
                desc._PacketId = (uint)SerializerUtility.ReadInt(buffer);
            }
            if ((mask & 8) != 0)
            {
                s.Read(buffer, 0, 2);
                desc._Error = (uint)SerializerUtility.ReadShort(buffer);
            }
            if ((mask & 16) != 0)
            {
                desc._ClientId = (ulong)SerializerUtility.ReadLongOpt(s, buffer);
            }
            if ((mask & 32) != 0)
            {
                desc._CharacterId = (ulong)SerializerUtility.ReadLongOpt(s, buffer);
            }
            if ((mask & 64) != 0)
            {
                s.Read(buffer, 0, 2);
                var length = SerializerUtility.ReadShort(buffer);
                ulong l;
                if (desc._Routing == null)
                    desc._Routing = new List<ulong>();
                for (int i = 0; i < length; i++)
                {
                    l = (ulong)SerializerUtility.ReadLongOpt(s, buffer);
                    desc._Routing.Add(l);
                }
            }
            if ((mask & 128) != 0)
            {
                s.Read(buffer, 0, 4);
                var length = SerializerUtility.ReadInt(buffer);
                desc.Data = new byte[length];
                s.Read(desc.Data, 0, length);
            }
            else
            {
                desc._Data = sEmptyByteArray;
            }

            return desc;
        }
    }
}
