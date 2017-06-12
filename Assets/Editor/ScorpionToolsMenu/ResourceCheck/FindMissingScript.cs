using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

//查找缺失脚本
public class FindMissingScript
{
    //移除AnimationClip
    [MenuItem("Tools/Find Missing Script")]
    public static bool FindMissingScriptInCurrentSelection()
    {
        string str = "";
        Debug.Log("Find Missing Script----------------begin");

        EditorUtility.DisplayProgressBar("Find Missing Script In Selection", "", 0);

        var gos = EnumAssets.EnumGameObjectRecursiveInCurrentSelection();

        int i = 0;
        int processed = 0;
        int count = gos.Count();
        {
            // foreach(var go in gos)
            var __enumerator2 = (gos).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var go = __enumerator2.Current;
                {
                    EditorUtility.DisplayProgressBar("Find Missing Script In Selection", go.name, i * 1.0f / count);
                    i++;

                    var cs = go.GetComponents<Component>();
                    foreach (var c in cs)
                    {
                        if (null != c)
                        {
                            continue;
                        }

                        str += AssetDatabase.GetAssetPath(go.GetInstanceID());
                        str += "   [" + go.gameObject.transform.FullPath() + "]\n";


                        processed++;
                    }
                }
            }
        }


        EditorUtility.ClearProgressBar();

        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }

        Debug.Log("Find Missing Script Total=[" + processed.ToString() + "]---------------end");
        return true;
    }
}
