using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ScorpionLib.Tools.Editor
{
    public class OptimizingPropertiesOfModelResources
    {
        static void getPath(string path)
        {
            List<string> mpathList = new List<string>();

            if (path != null)
            {
                string[] f1 = Directory.GetFiles(path, "*"); ;
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
                            if (sss.Contains("FBX") && !sss.Contains("meta"))
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
            int k = 0;
            {
                var __list3 = mpathList;
                var __listCount3 = __list3.Count;
                for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var data = __list3[__i3];
                    {

                        Debug.Log("正在优化" + data + "属性");
                        if (ResetProperty(data))
                        {
                            k++;
                        }
                        Debug.Log(data + "属性优化完毕！！");
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static bool ResetProperty(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (null == importer)
            {
                return false;
            }
            bool dirty = false;
            if (0.01f != importer.globalScale)
            {
                importer.globalScale = 0.01f;
                dirty = true;
            }
            if (importer.meshCompression != ModelImporterMeshCompression.Off)
            {
                importer.meshCompression = ModelImporterMeshCompression.Off;
                dirty = true;
            }

            if (importer.isReadable != false)
            {
                importer.isReadable = false;
                dirty = true;
            }

            if (importer.optimizeMesh != true)
            {
                importer.optimizeMesh = true;
                dirty = true;
            }

            if (importer.importBlendShapes != false)
            {
                importer.importBlendShapes = false;
                dirty = true;
            }

            if (importer.addCollider != false)
            {
                importer.addCollider = false;
                dirty = true;
            }

            if (importer.swapUVChannels != false)
            {
                importer.swapUVChannels = false;
                dirty = true;
            }

            if (importer.generateSecondaryUV != false)
            {
                importer.generateSecondaryUV = false;
                dirty = true;
            }

            if (importer.normalImportMode != ModelImporterTangentSpaceMode.Import)
            {

                importer.normalImportMode = ModelImporterTangentSpaceMode.Import;
                dirty = true;
            }

            if (importer.tangentImportMode != ModelImporterTangentSpaceMode.None)
            {
                importer.tangentImportMode = ModelImporterTangentSpaceMode.None;
                dirty = true;
            }

            if (importer.animationType != ModelImporterAnimationType.Legacy)
            {
                importer.animationType = ModelImporterAnimationType.Legacy;
                dirty = true;
            }

            //SerializedObject modelImporterObj = new SerializedObject(importer);
            //modelImporterObj.ApplyModifiedProperties();
            if (dirty)
            {
                AssetDatabase.ImportAsset(path);
            }

            return true;
        }

		[MenuItem("Tools/Optimize Resource/Optimize Model Properties In (ArtAsset.Model)")]
        private static void MYOptimizingPropertiesOfModelResources()
        {

            getPath(Application.dataPath + "/ArtAsset/Model");
        }

        public static void ResetObjectModelProperties(IEnumerable<UnityEngine.Object> gos)
        {
            EditorUtility.DisplayProgressBar("Optimize Resource/Reset Model Properties", "Collecting FBX", 0);

            Debug.Log("Optimize Resource/Reset Model Properties-----------------begin");

            int c = gos.Count();
            var modelArray = new string[c];
            int i = 0;
            {
                // foreach(var t in gos)
                var __enumerator4 = (gos).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var t = __enumerator4.Current;
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
                EditorUtility.DisplayProgressBar("Optimize Resource/Reset Model Properties", path, i * 1.0f / count);
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
            Debug.Log("Optimize Resource/Reset Model Properties-----------------end-processed=[" + processed.ToString() + "]");

            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

        }

 		[MenuItem("Tools/Optimize Resource/Optimize Model Propertie In (Selection)", true)]
        private static bool CanResetModelProperties()
        {
            return null != Selection.activeObject;
        }


		[MenuItem("Tools/Optimize Resource/Optimize Model Propertie In (Selection)")]
        private static void ResetModelProperties()
        {
            EditorUtility.DisplayProgressBar("Optimize Resource/Reset Model Properties", "Collecting FBX", 0);

			var gos = EnumAssets.EnumInCurrentSelection<UnityEngine.Object>();
			ResetObjectModelProperties(gos);
			

			/*
            List<string> list = new List<string>();
            {
                // foreach(var go in gos)
                var __enumerator5 = (gos).GetEnumerator();
                while (__enumerator5.MoveNext())
                {
                    var go = __enumerator5.Current;
                    {
                        var path = AssetDatabase.GetAssetPath(go.GetInstanceID());
                        if ((path.Contains(".FBX") || path.Contains(".fbx")) && !path.Contains(".meta"))
                        {
                            list.Add(path);
                        }
                    }
                }
            }

            int i = 0;
            string log = "";
            {
                var __list6 = list;
                var __listCount6 = __list6.Count;
                for (int __i6 = 0; __i6 < __listCount6; ++__i6)
                {
                    var path = __list6[__i6];
                    {
                        EditorUtility.DisplayProgressBar("Optimize Resource/Reset Model Properties", path, i * 1.0f / list.Count);
                        log += path + "\n";
                        ResetProperty(path);
                        i++;
                    }
                }
            }
			*/ 
            AssetDatabase.Refresh();

//             if (!string.IsNullOrEmpty(log))
//             {
//                 Debug.Log(log);
//             }
            EditorUtility.ClearProgressBar();

            //EditorUtility.DisplayDialog("Optimize Resource/Reset Model Properties", "Done Total=" + list.Count.ToString(), "OK");

        }

    }
}
