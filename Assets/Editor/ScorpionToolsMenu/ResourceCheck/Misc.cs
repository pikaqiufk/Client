using UnityEngine;
using System.Collections;
using UnityEditor;

public static class ExportTransform {

	[MenuItem("Misc/ExportTransform")]
	private static void ExportTransformToLog()
	{
		var t = Selection.activeGameObject.transform;
		var p = t.position;
		var  a = t.rotation.eulerAngles;
		string log = string.Format("{0},{1},{2},{3},{4},{5}", p.x, p.y, p.z, a.x, a.y, a.z);
		Debug.Log(log);
	}

	[MenuItem("Misc/Print Game Object Path")]
	private static void ExportGameObjectPath()
	{
		var t = Selection.activeGameObject.transform;

		Debug.Log(FullPath(t));
	}

	public static string FullPath(Transform t)
	{
		string path = "";

		do
		{
			path = @"/" + t.name + path;
			t = t.parent;
		} while (null != t);

		return path;
	}

	[MenuItem("Misc/Print Sound File Length")]
	private static void PrintSoundFileLength()
	{
		string log = "";
		var gos = EnumAssets.EnumInCurrentSelection<UnityEngine.Object>();
		foreach (var go in gos)
		{
			var path = AssetDatabase.GetAssetPath(go.GetInstanceID());
			var audio = Resources.LoadAssetAtPath<AudioClip>(path);
			if (null != audio)
			{
				log += path + "\t" + audio.length+"\n";
			}
		}
		Debug.Log(log);
		
		
		
	}
}
