using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ScorpionLib.Tools.Editor
{
    internal class RemoveSameAnim
    {
        private static void getPath(string path)
        {
            List<string> AinmList = new List<string>();

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
                            if (f11.Contains("anim") && !f11.Contains("meta"))
                                AinmList.Add(f11);
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
                var __list3 = AinmList;
                var __listCount3 = __list3.Count;
                for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var data = __list3[__i3];
                    {
                        EditorUtility.DisplayProgressBar("OptimizingRes", data,
                            k * 1.0f / AinmList.Count);
                        try
                        {
                            //                      FileInfo a = new FileInfo(data);
                            //                      Debug.Log(a.Name+" "+a.Length);
                            string[] s = data.Split(new char[] { '\\' });
                            string sss = "";
                            sss += Application.dataPath + "/Res/Animation/";
                            string aa = Application.dataPath + "/Res/Animation/";
                            for (int i = 1; i < s.Length; i++)
                            {
                                if (i == s.Length - 2)
                                    continue;
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
                            for (int i = 1; i < s.Length; i++)
                            {

                                if (i != s.Length - 1)
                                {
                                    aa += s[i];
                                    aa += '/';
                                }
                                else
                                {
                                    aa += s[i];
                                }
                            }
                            Debug.Log(aa + " " + sss);

                            if (File.Exists(sss))
                            {

                                FileInfo a = new FileInfo(sss);
                                FileInfo b = new FileInfo(aa);

                                if (GameUtils.GetMd5Hash(sss) == GameUtils.GetMd5Hash(aa))

                                {
                                    Debug.Log(data + " \n" + sss);
                                    File.Delete(data);
                                }

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
            k = 0;
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

		[MenuItem("Tools/Optimize Resource/Remove Same Animation In (Res.Animation)")]
        private static void RemoveSameAnimTool()
        {

            string log = string.Empty;

            var files = Directory.GetFiles(Application.dataPath + "/Res/Animation", "*.anim", SearchOption.AllDirectories);
            int i = 0;
            int count = files.Length;
            {
                var __array4 = files;
                var __arrayLength4 = __array4.Length;
                for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
                {
                    var file = __array4[__i4];
                    {
                        i++;
                        var temp = file.Replace("\\", "/");

                        EditorUtility.DisplayProgressBar("Remove Same Animation", temp, i * 1.0f / count);

                        int idx = temp.LastIndexOf("/");
                        var fileName = temp.Substring(idx + 1, temp.Length - 1 - idx);

                        var idx_ = temp.Substring(0, idx - 1).LastIndexOf("/");
                        var dir = temp.Substring(0, idx_);
                        if (dir.CompareTo(Application.dataPath + "/Res/Animation") == 0)
                        {
                            continue;
                        }

                        string compareFile = dir + "/" + fileName;
                        if (File.Exists(compareFile))
                        {
                            if (GameUtils.GetMd5Hash(compareFile) == GameUtils.GetMd5Hash(temp))
                            {
                                log += "[" + temp + "]=[" + compareFile + "]\n";
                            }
                        }

                    }
                }
            }
            EditorUtility.ClearProgressBar();

            if (!string.IsNullOrEmpty(log))
            {
                Debug.Log(log);
            }

        }
    }

}
