/* Copyright (c) 2012 MoPho' Games
 * All Rights Reserved
 * 
 * Please see the included 'LICENSE.TXT' for usage rights
 * If this asset was downloaded from the Unity Asset Store,
 * you may instead refer to the Unity Asset Store Customer EULA
 * If the asset was NOT purchased or downloaded from the Unity
 * Asset Store and no such 'LICENSE.TXT' is present, you may
 * assume that the software has been pirated.
 * */

using UnityEngine;
using System.Collections;

namespace MoPhoGames.USpeak.Core
{
	/// <summary>
	/// Container for encoded audio data
	/// Used to store encoded audio data, and convert to network-friendly compact byte array format
	/// </summary>
	public struct USpeakFrameContainer
	{
		public ushort Samples;
		public byte[] encodedData;

		public int LoadFrom( byte[] source, int offset )
		{
            int encLen = System.BitConverter.ToUInt16(source, offset);
            encodedData = new byte[encLen];
            System.Array.Copy(source, 2 + offset, encodedData, 0, encLen);
		    return encLen + 2;
		}

		/// <summary>
		/// Convert the USpeakFrameContainer to a network-friendly byte array
		/// </summary>
		public byte[] ToByteArray()
		{
			byte[] ret = new byte[ encodedData.Length + 2 ];
            byte[] len = System.BitConverter.GetBytes((ushort)encodedData.Length);
            len.CopyTo(ret, 0);
			System.Array.Copy( encodedData, 0, ret, 2, encodedData.Length);
			return ret;
		}
	}
}