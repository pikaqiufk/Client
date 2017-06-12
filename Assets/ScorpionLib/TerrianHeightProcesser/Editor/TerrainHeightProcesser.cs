using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

/*
 * 地形高度处理器
 */
public class TerrainHeightProcesser : ScriptableWizard
{
	[Tooltip("x axis length")]
	public int mapWdith;

	[Tooltip("z axis length")]
	public int mapHeight;

	[Tooltip("safe height")]
	public float SafeHeight = 0;

	public const float InaccurayHeight = 4.0f;

	[MenuItem("Tools/Save Terrain Height File")]
	public static void OpenDialog()
	{
		DisplayWizard<TerrainHeightProcesser>("Save Terrain Height", "Save", "Cancel");
	}

	public void SaveTerrainHeightFile()
	{

		//以每米为间隔从高空向下发射射线
		Vector3 pos = Vector3.zero;
		pos.y = 100;
		Ray ray = new Ray(pos, -Vector3.up);

		//创建一个对象
		TerrainHeightData th = ScriptableObject.CreateInstance<TerrainHeightData>();
		th.MapWidth = mapWdith;
		th.MapHeight = mapHeight;
		th.List = new List<float>();

		//纵向
		for (short i = 0; i < mapHeight; i++)
		{
			pos.z = i;
			//横向
			for (short j = 0; j < mapWdith; j++)
			{
				float height = SafeHeight;
				pos.x = j;
				ray.origin = pos;
				RaycastHit result;
				if (Physics.Raycast(ray, out result))
				{
					if (SafeHeight - result.point.y < InaccurayHeight)
					{
						height = result.point.y;
					}
				}

				th.List.Add(height);
			}
		}
		
		//计算file name
		string sceneName = UnityEditor.EditorApplication.currentScene;
		int start = sceneName.LastIndexOf('/') + 1;
		int count = sceneName.LastIndexOf('.')-start;
		sceneName = sceneName.Substring(start, count);
		string path = "Assets/ArtAsset/TerrainHeightData/" + sceneName + ".asset";

		//保存
		AssetDatabase.CreateAsset(th, path);
		AssetDatabase.Refresh();

		string str = string.Empty;
		str += "mapHeight=" + mapHeight.ToString() + "   ";
		str += "mapWdith=" + mapWdith.ToString();
		EditorUtility.DisplayDialog("SUCCESS", "file:" + path + "\n" + str, "OK");
	}

	void OnWizardCreate()
	{
		SaveTerrainHeightFile();
	}

	void OnWizardOtherButton()
	{
		Close();
	}

	
}
