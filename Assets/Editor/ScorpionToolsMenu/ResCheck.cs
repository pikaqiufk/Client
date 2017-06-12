#region using

using System;
using System.Collections.Generic;
using System.IO;
using DataTable;
using UnityEngine;

#endregion

internal class ResCheck : MonoBehaviour
{
    public string Check(string Path, string Directory, string Suffix)
    {
        var MYText = "";
        if (Path == "")
        {
            Debug.Log("的路径为空");
            return null;
        }
        var data = Directory + Path + Suffix;

        var dir = System.IO.Path.GetDirectoryName(data);
        var files = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(data));

        var find = false;
        foreach (var file in files)
        {
            var filename = System.IO.Path.GetFileNameWithoutExtension(file);
            var s = Path.Split('/');
            var ss = s[s.Length - 1];

            if (filename == ss)
            {
                find = true;
                break;
            }
        }

        if (!find)
        {
            MYText += Path + " can not found.";
        }

        return MYText;
    }

    private void getPath(string path, HashSet<string> mHashSet)
    {
        var mpathList = new List<string>();

        if (path != null)
        {
            var f1 = Directory.GetFiles(path, "*");
            ;
            string[] d1;
            {
                var __array9 = f1;
                var __arrayLength9 = __array9.Length;
                for (var __i9 = 0; __i9 < __arrayLength9; ++__i9)
                {
                    var f11 = __array9[__i9];
                    {
                        var s = f11.Split('\\');
                        var sss = "";
                        for (var i = 1; i < s.Length; i++)
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
                        if (!sss.Contains("meta") && (sss.Contains("prefab") || sss.Contains("mp3")))
                        {
                            mpathList.Add(sss);
                        }
                    }
                }
            }
            d1 = Directory.GetDirectories(path);
            {
                var __array10 = d1;
                var __arrayLength10 = __array10.Length;
                for (var __i10 = 0; __i10 < __arrayLength10; ++__i10)
                {
                    var d11 = __array10[__i10];
                    {
                        getPath(d11, mHashSet);
                    }
                }
            }
        }
        {
            var __list11 = mpathList;
            var __listCount11 = __list11.Count;
            for (var __i11 = 0; __i11 < __listCount11; ++__i11)
            {
                var VARIABLE = __list11[__i11];
                {
                    mHashSet.Add("Assets/" + VARIABLE);
                }
            }
        }
    }

    //反向检查
    public void OnClick_ReverseChck()
    {
        var ResHashSet = new HashSet<string>();
        var pathHashSet = new HashSet<string>();
        getPath(Application.dataPath + "\\Res\\Model", pathHashSet);
        getPath(Application.dataPath + "\\Res\\Effect", pathHashSet);
        getPath(Application.dataPath + "\\Res\\Sound", pathHashSet);
        Table.ForeachWeaponMount(recoard =>
        {
            var data = @"Assets/Res/" + recoard.Path + @".prefab";
            ResHashSet.Add(data);
            return true;
        });
        Table.ForeachBuildingRes(recoard =>
        {
            var data = @"Assets/Res/" + recoard.Path + @".prefab";
            ResHashSet.Add(data);
            return true;
        });
        Table.ForeachDropModel(recoard =>
        {
            var data = @"Assets/Res/" + recoard.ModelPath + @"";
            ResHashSet.Add(data);
            return true;
        });
        Table.ForeachCharModel(recoard =>
        {
            var data = @"Assets/Res/Model/" + recoard.ResPath + @".prefab";
            ResHashSet.Add(data);
            return true;
        });
        Table.ForeachSound(recoard =>
        {
            var data = @"Assets/Res/" + recoard.FullPathName + @"";
            ResHashSet.Add(data);
            return true;
        });
        Table.ForeachEffect(recoard =>
        {
            var data = @"Assets/Res/" + recoard.Path + @".prefab";
            ResHashSet.Add(data);
            return true;
        }
            );
        Table.ForeachSceneEffect(recoard =>
        {
            var data = @"Assets/Res/" + recoard.Path + @".prefab";
            ResHashSet.Add(data);
            return true;
        }
            );
        var MYText = new List<string>();
        {
            // foreach(var data in pathHashSet)
            var __enumerator7 = (pathHashSet).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var data = __enumerator7.Current;
                {
                    if (!ResHashSet.Contains(data))
                    {
                        Debug.Log(data);
                        MYText.Add(data);
                    }
                }
            }
        }

        var pathout = "C:\\f反向检查Model，Effect，Sound目录.txt";
        var sw = new StreamWriter(pathout, false);
        {
            var __list8 = MYText;
            var __listCount8 = __list8.Count;
            for (var __i8 = 0; __i8 < __listCount8; ++__i8)
            {
                var VARIABLE = __list8[__i8];
                {
                    if (string.IsNullOrEmpty(VARIABLE))
                    {
                        continue;
                    }
                    sw.WriteLine(VARIABLE);
                }
            }
        }
        sw.Close();
        sw.Dispose();
        Debug.Log("Effects resource correctness checked end");
    }

    //检查武器挂载
    public void OnClick1()
    {
        var MYText = new List<string>();
        //从武器表中获取特效的预制体文件路径，判断文件是否存在，存在则说明特效表信息正确，反正则错误。
        Func<WeaponMountRecord, bool> a = i =>
        {
            //             if (i.Path == "")
            //             {
            //                 Debug.Log("特效ID" + i.Id + "的路径为空");
            //                 return true;
            //             }
            //             string data = @"Assets/Res/" + i.Path + ".prefab";
            // 
            //             FileInfo mFileInfo = new FileInfo(data);
            //             //判断文件是否存在
            //             if (!mFileInfo.Exists)
            //             {
            //                 Debug.Log("Find  [" + i.Path + "  ] failed!!!");
            //                 MYText.Add(i.Path + "\n");
            //             }
            //             else
            //             {
            //                 //如果存在则判断大小写是否匹配
            //                 string[] s = i.Path.Split(new char[] { '/' });
            //                 string ss = s[s.Length - 1] += ".prefab";
            //                 if (ss != mFileInfo.Name)
            //                 {
            //                     Debug.Log(mFileInfo.Name + "和" + ss + "不匹配");
            //                     MYText.Add(mFileInfo.Name + "和" + ss + "大小写不匹配" + "\n");
            //                 }
            // 
            //             }
            MYText.Add(Check(i.Path, @"Assets/Res/", ".prefab"));
            return true;
        };
        Table.ForeachWeaponMount(a);

        var pathout = "C:\\装备挂载表资源问题.txt";

        var sw = new StreamWriter(pathout, false);
        {
            var __list1 = MYText;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var VARIABLE = __list1[__i1];
                {
                    if (string.IsNullOrEmpty(VARIABLE))
                    {
                        continue;
                    }

                    sw.WriteLine(VARIABLE);
                }
            }
        }
        sw.Close();
        sw.Dispose();
        Debug.Log("Effects resource correctness checked end");
    }

    //检查家园建筑
    public void OnClick2()
    {
        var MYText = new List<string>();
        //从表中获取文件路径，判断文件是否存在，存在则说明特效表信息正确，反正则错误。
        Table.ForeachBuildingRes(i =>
        {
            MYText.Add(Check(i.Path, @"Assets/Res/", ".prefab"));
            return true;
        }
            );
        var pathout = "C:\\家园资源问题.txt";
        var sw = new StreamWriter(pathout, false);
        {
            var __list2 = MYText;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var VARIABLE = __list2[__i2];
                {
                    if (string.IsNullOrEmpty(VARIABLE))
                    {
                        continue;
                    }
                    sw.WriteLine(VARIABLE);
                }
            }
        }
        sw.Close();
        sw.Dispose();
        Debug.Log("Effects resource correctness checked end");
    }

    //检查掉落物品
    public void OnClick3()
    {
        var MYText = new List<string>();
        //从表中获取文件路径，判断文件是否存在，存在则说明特效表信息正确，反正则错误。
        Table.ForeachDropModel(i =>
        {
            MYText.Add(Check(i.ModelPath, @"Assets/Res/", ""));
            return true;
        }
            );
        var pathout = "C:\\掉落物品资源问题.txt";
        var sw = new StreamWriter(pathout, false);
        {
            var __list3 = MYText;
            var __listCount3 = __list3.Count;
            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
            {
                var VARIABLE = __list3[__i3];
                {
                    if (string.IsNullOrEmpty(VARIABLE))
                    {
                        continue;
                    }
                    sw.WriteLine(VARIABLE);
                }
            }
        }
        sw.Close();
        sw.Dispose();
        Debug.Log("Effects resource correctness checked end");
    }

    //检查模型
    public void OnClick4()
    {
        var MYText = new List<string>();
        //从表中获取文件路径，判断文件是否存在，存在则说明特效表信息正确，反正则错误。
        Table.ForeachCharModel(i =>
        {
            MYText.Add(Check(i.ResPath, @"Assets/Res/Model/", ".prefab"));
            return true;
        }
            );
        var pathout = "C:\\模型资源问题.txt";
        var sw = new StreamWriter(pathout, false);
        {
            var __list4 = MYText;
            var __listCount4 = __list4.Count;
            for (var __i4 = 0; __i4 < __listCount4; ++__i4)
            {
                var VARIABLE = __list4[__i4];
                {
                    if (string.IsNullOrEmpty(VARIABLE))
                    {
                        continue;
                    }
                    sw.WriteLine(VARIABLE);
                }
            }
        }
        sw.Close();
        sw.Dispose();
        Debug.Log("Effects resource correctness checked end");
    }

    //检查声音资源
    public void OnClick5()
    {
        var MYText = new List<string>();
        //从表中获取文件路径，判断文件是否存在，存在则说明特效表信息正确，反正则错误。
        Table.ForeachSound(i =>
        {
            MYText.Add(Check(i.FullPathName, @"Assets/Res/", ""));
            return true;
        }
            );
        var pathout = "C:\\声音问题.txt";
        var sw = new StreamWriter(pathout, false);
        {
            var __list5 = MYText;
            var __listCount5 = __list5.Count;
            for (var __i5 = 0; __i5 < __listCount5; ++__i5)
            {
                var VARIABLE = __list5[__i5];
                {
                    if (string.IsNullOrEmpty(VARIABLE))
                    {
                        continue;
                    }
                    sw.WriteLine(VARIABLE);
                }
            }
        }
        sw.Close();
        sw.Dispose();
        Debug.Log("Effects resource correctness checked end");
    }

    //检查特效资源
    public void OnClick6()
    {
        var MYText = new List<string>();
        //从表中获取文件路径，判断文件是否存在，存在则说明特效表信息正确，反正则错误。
        Table.ForeachEffect(i =>
        {
            MYText.Add(Check(i.Path, @"Assets/Res/", ".prefab"));
            return true;
        }
            );
        Table.ForeachSceneEffect(i =>
        {
            MYText.Add(Check(i.Path, @"Assets/Res/", ".prefab"));
            return true;
        }
            );
        var pathout = "C:\\特效问题.txt";
        var sw = new StreamWriter(pathout, false);
        {
            var __list6 = MYText;
            var __listCount6 = __list6.Count;
            for (var __i6 = 0; __i6 < __listCount6; ++__i6)
            {
                var VARIABLE = __list6[__i6];
                {
                    if (string.IsNullOrEmpty(VARIABLE))
                    {
                        continue;
                    }

                    sw.WriteLine(VARIABLE);
                }
            }
        }
        sw.Close();
        sw.Dispose();
        Debug.Log("Effects resource correctness checked end");
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        var helper = new LoadResourceHelper();

        var i = 0;
        var itr = (Table.GetTableNames()).GetEnumerator();
        while (itr.MoveNext())
        {
            var tableName = itr.Current;
            {
                helper.AddLoadInfo("Table/" + tableName + ".txt", tableName + ".txt", true, true, true);
            }
        }

        helper.BeginLoad(
            () => { Table.Init(); }
            );

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        BundleLoader.Instance.Tick(0);

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }
}