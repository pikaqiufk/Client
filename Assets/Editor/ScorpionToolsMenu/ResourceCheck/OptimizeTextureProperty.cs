using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;

public class OptimizeTextureProperty
{
    public static void ResetTextureProperty(IEnumerable<UnityEngine.Object> gos)
    {
        EditorUtility.DisplayProgressBar("OptimizeTextureProperty", "Collecting Texture", 0);

        Debug.Log("OptimizeTextureProperty-----------------begin");

        int c = gos.Count();
        var modelArray = new string[c];
        int i = 0;
        {
            // foreach(var t in gos)
            var __enumerator1 = (gos).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var t = __enumerator1.Current;
                {
                    modelArray[i++] = AssetDatabase.GetAssetPath(t.GetInstanceID());
                }
            }
        }

        string log = "";

        var dep = AssetDatabase.GetDependencies(modelArray);
        var count = dep.Length;
        var processed = 0;
        for (i = 0; i < count; i++)
        {
            var path = dep[i];
            EditorUtility.DisplayProgressBar("OptimizeTextureProperty", path, i * 1.0f / count);
            if (ResetProperty(path))
            {
                log += path + "\n";
                processed++;
            }
        }

        if (!string.IsNullOrEmpty(log))
        {
            Debug.Log(log);
        }
        Debug.Log("OptimizeTextureProperty-----------------end-processed=[" + processed.ToString() + "]");

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

    }

    public static bool ResetProperty(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (null == importer)
        {
            return false;
        }

        importer.textureType = TextureImporterType.Advanced;
        importer.generateCubemap = TextureImporterGenerateCubemap.None;
        importer.isReadable = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;

        //importer.textureFormat = TextureImporterFormat.AutomaticCompressed;

        AssetDatabase.ImportAsset(path);

        return true;
    }

	[MenuItem("Tools/Optimize Resource/Optimize Textrue Propertie In (Selection)", true)]
	private static bool CanOptimizeTextruePropertie()
	{
		return null != Selection.activeObject;
		
	}

	[MenuItem("Tools/Optimize Resource/Optimize Textrue Propertie In (Selection)")]
	private static void OptimizeTextruePropertie()
	{
		var gos = EnumAssets.EnumInCurrentSelection<UnityEngine.Object>();
		ResetTextureProperty(gos);
	}
}
