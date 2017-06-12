using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ScorpionLib.Tools.Editor
{
    internal class RemoveModelAnim
    {
        private static void getPath(string path)
        {
            List<string> fbxList = new List<string>();
            List<string> PrefabList = new List<string>();
            if (path != null)
            {
                string[] f1 = Directory.GetFiles(path, "*");

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
                            sss += "Assets/";
                            for (int i = 1; i < s.Length; i++)
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

                            if (sss.Contains("prefab") && !sss.Contains("meta"))
                                PrefabList.Add(sss);
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
            int k = 0;
            {
                var __list5 = PrefabList;
                var __listCount5 = __list5.Count;
                for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                {
                    var data = __list5[__i5];
                    {
                        try
                        {
                            UnityEngine.Object objects = AssetDatabase.LoadAssetAtPath(data, typeof(UnityEngine.Object));
                            if (objects != null)
                                if (objects.GetType().ToString() == "UnityEngine.GameObject")
                                {
                                    if (objects != null && objects != null)
                                        EditorUtility.DisplayProgressBar("OptimizingRes", objects.name,
                                            k * 1.0f / PrefabList.Count);
                                    //   Debug.Log("正在优化" + data + "属性");
                                    GameObject mpGameObject = objects as GameObject;

                                    foreach (var aa in EnumAssets.EnumGameObjectRecursive(mpGameObject))
                                    {
                                        try
                                        {

                                            //修改animator
                                            if (aa.GetComponent<Animation>() != null)
                                            {
                                                Animation myAnimation = aa.GetComponent<Animation>();
                                                if (myAnimation.clip != null)
                                                {
                                                    Debug.Log(data + "去掉多余动画成功");
                                                    myAnimation.clip = null;

                                                }
                                                foreach (AnimationState state in myAnimation)
                                                {
                                                    Debug.Log(state.clip.name);
                                                    myAnimation.RemoveClip(state.name);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {

                                            throw;
                                        }
                                    }

                                    //  Debug.Log(data + "属性优化完毕！！"); 

                                }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        k++;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        //[MenuItem("Tools/Optimize Resource/Remove Model Anim")]
        //优化资源
        public static void OptimizingResmethod()
        {
            getPath((Application.dataPath + "/Res/Model"));
        }


    }
}
