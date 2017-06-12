using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class TerrainHeightData : ScriptableObject
{
	[SerializeField]
	public int MapWidth = 0;

	[SerializeField]
	public int MapHeight = 0;

	[SerializeField]
	public List<float> List;

	public float GetTerrainHeight(float x,float z)
	{
		int w = (int)Mathf.Floor(x);
		int h = (int)Mathf.Floor(z);
		if (w < 0 || w >= MapWidth || h < 0 || h >= MapHeight)
			return 0.0f;

		int idx = h*MapWidth+w;
		if (idx < 0 || idx >= MapWidth * MapHeight || idx >= List.Count)
			return 0.0f;

		return List[idx];
	}


}

