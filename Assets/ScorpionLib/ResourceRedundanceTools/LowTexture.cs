#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class LowTexture{
	static bool run = false;
	static int beginIndex = Application.dataPath.Length-6;

	[MenuItem("Window/lowTexture/do")]
	public static void doLowTextrue()
	{
		run   = true;
		Debug.Log("lowTexture begin");
		Object[] SelectedAsset = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
		foreach(Object obj in SelectedAsset)
		{
			string sourcePath = AssetDatabase.GetAssetPath (obj);
			string fullPath = Application.dataPath+sourcePath.Substring(6);
			doLowTextrue(fullPath);
		}
		EditorUtility.ClearProgressBar();
		Debug.Log("lowTexture ok");
	}

	enum RetType
	{
		Error=-1,
		Ok = 0,
		Ignore=1,
	}

	public static void doLowTextrue(string path)
	{
		run   = true;
		Debug.Log("lowTexture begin:"+path);
		if(File.Exists(path))
		{
			FileInfo fi = new FileInfo(path);
			handleFile(fi);
		}
		else
		{
			DirectoryInfo dir=new DirectoryInfo(path);
			enumResource(dir);
		}
		EditorUtility.ClearProgressBar();
		Debug.Log("lowTexture ok");
	}

	static void enumResource(DirectoryInfo dir)
	{
		FileInfo[] fi = dir.GetFiles();
		DirectoryInfo[] di = dir.GetDirectories();
		for(int i=0; i<fi.Length&&run; i++)
		{
			handleFile(fi[i]);
		}
		for(int i=0; i<di.Length&&run; i++)
		{
			enumResource(di[i]);
		}
	}

	static void handleFile(FileInfo fi)
	{
		//fn should begin at Assets\,and has ext
		string fp = fi.FullName.Substring(beginIndex);
		run = !EditorUtility.DisplayCancelableProgressBar("lowTexture", fp, 0);
		RetType ret = RetType.Ok;
		switch(fi.Extension.ToLower())
		{
		case ".png":
		case ".jpg":
		case ".tga":
			ret = handleTexture(fp);
			break;
		case ".prefab":
			ret = handlePrefab(fp);
			break;
		};
		if(ret==RetType.Error)Debug.LogError("low quality failed:"+fp);
	}

	static RetType handleTexture(string  assetPath)
	{
		Texture2D t = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
		if(null == t)return RetType.Error;
		TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
		//已经缩放到0.5/
		if(textureImporter.userData == "low")return RetType.Ignore;
		//平台差异设置/
		int maxTextSize = 0;
		TextureImporterFormat tf = new TextureImporterFormat();
		bool ret = textureImporter.GetPlatformTextureSettings("Android",out maxTextSize, out tf);
		if(!ret)return RetType.Error;
		//图片大小缩小1/2/
		int max = t.width > t.height?t.width:t.height;
		if(max==0)return RetType.Error;
		if(max<=32 || maxTextSize<=32)return RetType.Ignore;
		int a = maxTextSize;
		while(a>=max)a=a>>1;
		textureImporter.SetPlatformTextureSettings("Android",a,tf);
		textureImporter.userData = "low";
		AssetDatabase.ImportAsset(assetPath,ImportAssetOptions.ForceUpdate);
		return RetType.Ok;
	}

	static RetType handlePrefab(string assetPath)
	{
		UIAtlas atlas = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UIAtlas)) as UIAtlas;
		if(null==atlas)return RetType.Ignore;
		if(null==atlas.spriteMaterial || null==atlas.spriteMaterial.mainTexture)return RetType.Error;
		string path = AssetDatabase.GetAssetPath(atlas.spriteMaterial.mainTexture);
		//缩放关联的texture/
		RetType ret = handleTexture(path);
		if(atlas.pixelSize != 1f)return RetType.Ignore;
		atlas.pixelSize = 0.5f;
		//缩放图集坐标/
		BetterList<string> sl = atlas.GetListOfSprites();
		if(sl==null)return RetType.Error;
		foreach(string sn in sl)
		{
			UISpriteData sd = atlas.GetSprite(sn);
			sd.x /= 2;
			sd.y /= 2;
			sd.width /= 2;
			sd.height /= 2;
			
			sd.borderBottom /= 2;
			sd.borderLeft /= 2;
			sd.borderRight /=2;
			sd.borderTop /= 2;

			sd.paddingTop /= 2;
			sd.paddingBottom /= 2;
			sd.paddingLeft /= 2;
			sd.paddingRight /= 2;
		}
		atlas.pixelSize*=2;
		atlas.MarkAsChanged();
		return ret;
	}
}


#endif