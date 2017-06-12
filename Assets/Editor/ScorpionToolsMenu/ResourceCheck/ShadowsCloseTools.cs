
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;



internal class ShadowsCloseTools
{

    private static void getPath(string path)
    {
        List<string> mpathList = new List<string>();

        if (path != null)
        {
            string[] f1 = Directory.GetFiles(path, "*");
            ;
            string[] d1;
            {
                var __array1 = f1;
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var f11 = (string)__array1[__i1];
                    {

                        string[] s = f11.Split(new char[] { '\\' });
                        string sss = "";
                        for (int i = 4; i < s.Length; i++)
                        {
                            if (i != s.Length - 1)
                            {
                                sss += s[i];
                                sss += '/';
                            }
                            else
                            {
                                sss += s[i];
                            }
                        }
                        if (!sss.Contains("meta") && sss.Contains("prefab"))
                            mpathList.Add(sss);

                    }
                }
            }
            d1 = Directory.GetDirectories(path);
            {
                var __array2 = d1;
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var d11 = (string)__array2[__i2];
                    {

                        getPath(d11);

                    }
                }
            }
        }
        {
            var __list5 = mpathList;
            var __listCount5 = __list5.Count;
            for (int __i5 = 0; __i5 < __listCount5; ++__i5)
            {
                var data = __list5[__i5];
                {

                    int k = 0;
                    UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(data);

                    foreach (var VARIABLE in objects)
                    {
                        if (objects != null && VARIABLE != null)
                            EditorUtility.DisplayProgressBar("ShadowsCloseTools", VARIABLE.name, k * 1.0f / objects.Length);
                        if (objects != null && VARIABLE != null)
                        {

                            if (VARIABLE.GetType().ToString() == "UnityEngine.GameObject")
                            {

                                GameObject mpGameObject = VARIABLE as GameObject;
                                foreach (var aa in EnumAssets.EnumGameObjectRecursive(mpGameObject))
                                {
                                    if (aa.GetComponent<Renderer>() != null)
                                    {

                                        var MymeshRenderer = aa.GetComponent<Renderer>();
                                        MymeshRenderer.castShadows = false;
                                        MymeshRenderer.receiveShadows = false;
                                    }
                                }

                            }
                        }
                        k++;
                    }

                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

    }

	[MenuItem("Tools/Optimize Resource/Close Shadows In (Res and ArtAsset.Prefab)")]
    private static void ShadowsCloseTool()
    {
        getPath(Application.dataPath + "/Res");
        getPath(Application.dataPath + "/ArtAsset/Prefab");
        Debug.Log("ShadowsCloseTools successed！! ");
    }

	[MenuItem("Tools/Optimize Resource/Close Shadows In (Selection)", true)]
    private static bool CanUseCloseShadowsInSelection()
    {
        return null != Selection.activeObject;
    }

    public static void CloseShadows(IEnumerable<Renderer> cs)
    {
        EditorUtility.DisplayProgressBar("Close Shadows In Selection", "Collecting MeshRenderer Components", 0);

        int total = 0;
        {
            // foreach(var c in cs)
            var __enumerator6 = (cs).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var c = __enumerator6.Current;
                {
                    total++;
                }
            }
        }
        string str = "";

        int i = 0;
        int processed = 0;
        {
            // foreach(var c in cs)
            var __enumerator7 = (cs).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var c = __enumerator7.Current;
                {
                    EditorUtility.DisplayProgressBar("Close Shadows In Selection", c.gameObject.name, i * 1.0f / total);
                    if (false != c.castShadows || false != c.receiveShadows)
                    {
                        c.castShadows = false;
                        c.receiveShadows = false;
                        str += c.gameObject.name + "\n";
                        processed++;
                    }
                    i++;
                }
            }
        }

        EditorUtility.ClearProgressBar();

        Debug.Log("Close Shadows In Selection----------------------------begin");
        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }
        Debug.Log("Close Shadows In Selection----------------------------end-total=[" + processed.ToString() + "]");

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

	[MenuItem("Tools/Optimize Resource/Close Shadows In (Selection)")]
    public static void CloseShadowsInSelection()
    {
        CloseShadows(EnumAssets.EnumComponentRecursiveInCurrentSelection<Renderer>());
    }
}


