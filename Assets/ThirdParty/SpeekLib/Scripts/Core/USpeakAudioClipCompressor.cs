
using UnityEngine;
using System.Collections.Generic;
using System.IO;

using MoPhoGames.USpeak.Codec;

namespace MoPhoGames.USpeak.Core
{
	/// <summary>
	/// Helper class to aid in converting and compressing audio clips
	/// for sending over the network
	/// </summary>
	public class USpeakAudioClipCompressor : MonoBehaviour
	{
		#region Static Fields

		public static ICodec Codec = new SpeexCodec();

		#endregion

		#region Static Methods

		public static byte[] CompressAudioData( float[] samples, int channels, out int sample_count, BandMode mode, float gain = 1.0f )
		{
			data.Clear();
			sample_count = 0;
			short[] b = USpeakAudioClipConverter.AudioDataToShorts( samples, channels, gain );
			byte[] mlaw = Codec.Encode( b, mode );
			data.AddRange( mlaw );
		    return data.ToArray();
		    //return zip( data.ToArray() );
		}

// 		public static byte[] CompressAudioClip( AudioClip clip, out int samples, BandMode mode, float gain = 1.0f )
// 		{
// 			data.Clear();
// 			samples = 0;
// 			short[] b = USpeakAudioClipConverter.AudioClipToShorts( clip, gain );
// 			byte[] mlaw = Codec.Encode( b, mode );
// 			data.AddRange( mlaw );
// 			return zip( data.ToArray() );
// 		}

		public static AudioClip DecompressAudioClip( byte[] data, int samples, int channels, bool threeD, BandMode mode, float gain )
		{
// 			byte[] d;
// 			d = unzip( data );
			short[] pcm = Codec.Decode( data, mode );
			tmp.Clear();
			tmp.AddRange( pcm );
			return USpeakAudioClipConverter.ShortsToAudioClip( tmp.ToArray(), channels, WSpeak.getFrequency(mode), threeD, gain );
		}

		#endregion

		#region Private Fields

		private static List<byte> data = new List<byte>();

		private static List<short> tmp = new List<short>();

		#endregion

		#region Private Methods

// 		private static byte[] zip( byte[] data )
// 		{
// 			MemoryStream memstream = new MemoryStream( data );
// 			MemoryStream outstream = new MemoryStream();
// 
// 			using( GZipStream encoder = new GZipStream( outstream, CompressionMode.Compress ) )
// 			{
// 				memstream.WriteTo( encoder );
// 			}
// 
// 			return outstream.ToArray();
// 		}
// 
// 		private static byte[] unzip( byte[] data )
// 		{
// 			GZipStream decoder = new GZipStream( new MemoryStream( data ), CompressionMode.Decompress );
// 			MemoryStream outstream = new MemoryStream();
// 			CopyStream( decoder, outstream );
// 			return outstream.ToArray();
// 		}

		private static void CopyStream( Stream input, Stream output )
		{
			byte[] buffer = new byte[ 32768 ];
			//long TempPos = input.Position;
			while( true )
			{
				int read = input.Read( buffer, 0, buffer.Length );
				if( read <= 0 ) break;
				output.Write( buffer, 0, read );
			}
			//input.Position = TempPos;// or you make Position = 0 to set it at the start 
		}

		#endregion
	}
}