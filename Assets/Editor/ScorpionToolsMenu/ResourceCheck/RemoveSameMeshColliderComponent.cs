using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;



internal class RemoveSameMeshColliderComponent : MonoBehaviour
{
    public static void RemoveSameMeshCollider(IEnumerable<GameObject> gos)
    {
        int count = gos.Count();
        int i = 0;
        int processed = 0;
        {
            // foreach(var go in gos)
            var __enumerator1 = (gos).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var go = __enumerator1.Current;
                {
                    i++;
                    EditorUtility.DisplayProgressBar("Draw Call Optimize", go.name, i * 1.0f / count);

                    var cs = go.GetComponents<MeshCollider>();
                    if (cs.Length > 1)
                    {
                        for (int idx = 1; idx < cs.Length; idx++)
                        {
                            GameObject.DestroyImmediate(cs[idx], true);
                        }
                        processed++;
                    }
                }
            }
        }

        Debug.Log("Remove Multiple MeshCollider In Selection Total[" + processed.ToString() + "]");

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

	[MenuItem("Tools/Optimize Resource/Remove Multiple MeshCollider In (Selection)", true)]
    private static bool NotGetFiltered()
    {
        return Selection.activeObject;
    }

	[MenuItem("Tools/Optimize Resource/Remove Multiple MeshCollider In (Selection)")]
    //去掉重复的mesh collider                                 
    public static void RemoveSameMeshColliderInSelection()
    {
        RemoveSameMeshCollider(EnumAssets.EnumGameObjectRecursiveInCurrentSelection());
    }
    public static void RemoveSameMeshColliderInEnumAssetAtPath(IEnumerable<UnityEngine.Object> gos)
    {
        int c = gos.Count();
        var modelArray = new string[c];
        int i = 0;
        {
            // foreach(var go in gos)
            var __enumerator2 = (gos).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var go = __enumerator2.Current;
                {
                    modelArray[i++] = AssetDatabase.GetAssetPath(go.GetInstanceID());

                }
            }
        }
        string log = "";
        var dep = AssetDatabase.GetDependencies(modelArray);
        var processed = 0;
        var count = dep.Length;
        for (i = 0; i < count; i++)
        {
            var path = dep[i];
            EditorUtility.DisplayProgressBar("Optimize Resource/Reset Model Properties", path, i * 1.0f / count);
            if (RemoveSameMeshColliderInIEnumer(path))
            {
                log += path + "\n";
                processed++;
            }
        }
        if (!string.IsNullOrEmpty(log))
        {
            Debug.Log(log);
        }
        Debug.Log("Optimize Resource/Reset Model Properties-----------------end-processed=[" + processed.ToString() + "]");

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

    }



    private static bool RemoveSameMeshColliderInIEnumer(string Path)
    {
        if (Path == null)
            return false;

        RemoveSameMeshCollider(EnumAssets.EunmAllGameObjectRecursiveAtPath(Path));
        return true;
    }
}
