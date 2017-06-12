using System;
using System.IO;
namespace Mono.CompilerServices.SymbolWriter
{
	public class SourceFileEntry
	{
		public readonly int Index;
		private int DataOffset;
		private MonoSymbolFile file;
		private string file_name;
		private byte[] guid;
		private byte[] hash;
		private bool creating;
		private bool auto_generated;
		public static int Size
		{
			get
			{
				return 8;
			}
		}
		public string FileName
		{
			get
			{
				return this.file_name;
			}
		}
		public bool AutoGenerated
		{
			get
			{
				return this.auto_generated;
			}
		}
		public SourceFileEntry(MonoSymbolFile file, string file_name)
		{
			this.file = file;
			this.file_name = file_name;
			this.Index = file.AddSource(this);
			this.creating = true;
		}
		public SourceFileEntry(MonoSymbolFile file, string file_name, byte[] guid, byte[] checksum) : this(file, file_name)
		{
			this.guid = guid;
			this.hash = checksum;
		}
        //internal void WriteData(MyBinaryWriter bw)
        //{
        //    this.DataOffset = (int)bw.BaseStream.Position;
        //    bw.Write(this.file_name);
        //    if (this.guid == null)
        //    {
        //        this.guid = Guid.NewGuid().ToByteArray();
        //        try
        //        {
        //            using (FileStream fs = new FileStream(this.file_name, FileMode.Open, FileAccess.Read))
        //            {
        //                MD5 md5 = MD5.Create();
        //                this.hash = md5.ComputeHash(fs);
        //            }
        //        }
        //        catch
        //        {
        //            this.hash = new byte[16];
        //        }
        //    }
        //    bw.Write(this.guid);
        //    bw.Write(this.hash);
        //    bw.Write(this.auto_generated ? 1 : 0);
        //}
        //internal void Write(BinaryWriter bw)
        //{
        //    bw.Write(this.Index);
        //    bw.Write(this.DataOffset);
        //}
		internal SourceFileEntry(MonoSymbolFile file, MyBinaryReader reader)
		{
			this.file = file;
			this.Index = reader.ReadInt32();
			this.DataOffset = reader.ReadInt32();
			int old_pos = (int)reader.BaseStream.Position;
			reader.BaseStream.Position = (long)this.DataOffset;
			this.file_name = reader.ReadString();
			this.guid = reader.ReadBytes(16);
			this.hash = reader.ReadBytes(16);
			this.auto_generated = (reader.ReadByte() == 1);
			reader.BaseStream.Position = (long)old_pos;
		}
		public void SetAutoGenerated()
		{
			if (!this.creating)
			{
				throw new InvalidOperationException();
			}
			this.auto_generated = true;
			this.file.OffsetTable.FileFlags |= OffsetTable.Flags.IsAspxSource;
		}
		public bool CheckChecksum()
		{
		    return true;
		}
		public override string ToString()
		{
			return string.Format("SourceFileEntry ({0}:{1})", this.Index, this.DataOffset);
		}
	}
}
