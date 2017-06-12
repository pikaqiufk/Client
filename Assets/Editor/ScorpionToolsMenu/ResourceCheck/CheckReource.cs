using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataTable;
using UnityEditor;
using UnityEngine;

namespace Assets.ScorpionLib.Tools.Editor
{
    internal class CheckResource
    {
        const string ResDir = @"Assets/Res/";

        [MenuItem("Tools/Check Resource/Check Weapon resource correctness")]
        private static void CheckWeapon()
        {
            List<string> MYText = new List<string>();
            //从武器表中获取特效的预制体文件路径，判断文件是否存在，存在则说明特效表信息正确，反正则错误。
            Func<WeaponMountRecord, bool> a = (i) =>
            {
                if (i.Path == "")
                {
                    Debug.Log("特效ID" + i.Id + "的路径为空");
                    return true;
                }
                string data = ResDir + i.Path + @".prefab";

                FileInfo mFileInfo = new FileInfo(data);
                //判断文件是否存在
                if (!mFileInfo.Exists)
                {
                    Debug.Log("Find  [" + i.Path + "] failed!!!");
                    MYText.Add(i.Path + "\n");
                }
                else
                {
                    //如果存在则判断大小写是否匹配
                    string[] s = i.Path.Split(new char[] { '/' });
                    string ss = s[s.Length - 1] += ".prefab";
                    if (ss != mFileInfo.Name)
                    {
                        Debug.Log(mFileInfo.Name + "和" + ss + "不匹配");
                        MYText.Add(mFileInfo.Name + "和" + ss + "不匹配" + "\n");
                    }

                }


                return true;



            };

            DataTable.Table.ForeachWeaponMount(a);

            Debug.Log("Effects resource correctness checked end");
        }

        [MenuItem("Tools/Check Resource/Check Effect resource correctness")]
        private static void CheckEffects()
        {

            //从特效表中获取特效的预制体文件路径，判断文件是否存在，存在则说明特效表信息正确，反正则错误。
            Func<EffectRecord, bool> a = (i) =>
            {
                if (i.Path == "")
                {
                    Debug.Log("特效ID" + i.Id + "的路径为空");
                    return true;
                }
                string data = ResDir + i.Path + @".prefab";

                if (!File.Exists(data))
                {
                    Debug.Log("Find" + data + "failed!!!");

                }
                //                 FileInfo mFileInfo = new FileInfo(data );
                //                 Debug.Log(mFileInfo.Name);
                return true;

            };

            DataTable.Table.ForeachEffect(a);
            Debug.Log("Effects resource correctness checked end");
        }

        [MenuItem("Tools/Check Resource/Check Character resource correctness")]
        private static void CheckCharacter()
        {


            //从数据表中获取模型文件路径，判断文件是否存在，存在则说明数据表信息正确，反正则错误。
            Func<CharacterBaseRecord, bool> a = (i) =>
            {

                if (DataTable.Table.GetCharModel(i.CharModelID) != null)
                {
                    string Path = ResDir +
                                  Resource.GetModelPath(DataTable.Table.GetCharModel(i.CharModelID).ResPath);
                    if (!File.Exists(ResDir))
                    {
                        Debug.Log("Find" + ResDir + "failed!!!");
                    }
                    string[] AnimPath =
                    {
                        ResDir +
                        DataTable.Table.GetCharModel(i.CharModelID).AnimPath +"/"+
                        DataTable.Table.GetAnimation(OBJ.CHARACTER_ANI.ATTACK).AinmName + ".anim"
                        ,
                        ResDir +
                        DataTable.Table.GetCharModel(i.CharModelID).AnimPath +"/"+
                        DataTable.Table.GetAnimation(OBJ.CHARACTER_ANI.DIE).AinmName + ".anim"
                        ,
                        ResDir +
                        DataTable.Table.GetCharModel(i.CharModelID).AnimPath +"/"+
                        DataTable.Table.GetAnimation(OBJ.CHARACTER_ANI.HIT).AinmName + ".anim"
                        ,
                        ResDir +
                        DataTable.Table.GetCharModel(i.CharModelID).AnimPath +"/"+
                        DataTable.Table.GetAnimation(OBJ.CHARACTER_ANI.RUN).AinmName + ".anim"
                        ,
                        ResDir +
                        DataTable.Table.GetCharModel(i.CharModelID).AnimPath +"/"+
                        DataTable.Table.GetAnimation(OBJ.CHARACTER_ANI.STAND).AinmName + ".anim"
                    };
                    {
                        var __array1 = AnimPath;
                        var __arrayLength1 = __array1.Length;
                        for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                        {
                            var data = __array1[__i1];
                            {
                                if (!File.Exists(data))
                                {
                                    Debug.Log(DataTable.Table.GetCharModel(i.CharModelID).Name + "  miss " + data);
                                }
                            }
                        }
                    }


                }
                return true;

            };

            DataTable.Table.ForeachCharacterBase(a);
            Debug.Log("Character resource correctness checked end");
        }
    }
}
