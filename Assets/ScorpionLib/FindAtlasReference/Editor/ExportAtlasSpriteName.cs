using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ExportAtlasSpriteName : ScriptableWizard
{
	[Tooltip("if Atlas is empty means the path[Assets]")]
	public UIAtlas Atlas;

	// Use this for initialization
	void Start()
	{

	}

	[MenuItem("Tools/Atlas Tool/Export Atlas Sprite Name")]
	public static void OpenDialog()
	{
		DisplayWizard<ExportAtlasSpriteName>("Export Atlas Sprite Name", "OK", "Cancel");
	}

	void OnWizardCreate()
	{
		Find();
	}
	void OnWizardOtherButton()
	{
		Close();
	}


	public void Find()
	{
		string log = "";
		if (null != Atlas)
		{
			log = LogSpriteName(Atlas);
			Debug.Log(log);
			return;
		}

		EditorUtility.DisplayProgressBar("Export Atlas Sprite Name", "loading...", 0);

		var gos = EnumAssets.EnumAssetAtPath<GameObject>("Assets/Res");
		foreach (var go in gos)
		{
			EditorUtility.DisplayProgressBar("Export Atlas Sprite Name", go.name, 0);
			var atlas = go.GetComponent<UIAtlas>();
			if (null != atlas)
			{
				log+=LogSpriteName(atlas);
			}
		}
		EditorUtility.ClearProgressBar();
		Debug.Log(log);

	}

	string LogSpriteName(UIAtlas atlas)
	{
		if (null == atlas)
		{
			return "";
		}

		string log = string.Format("[{0}]------------------------------------------\n", atlas.name);

		var list = atlas.GetListOfSprites();
		foreach (var sprite in list)
		{
			log += sprite;
			log += "\n";
		}

		log += "\n";
		return log;
	}
}

