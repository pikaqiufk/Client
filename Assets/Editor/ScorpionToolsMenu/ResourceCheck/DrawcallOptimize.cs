
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ScorpionLib.Tools.Editor
{
    internal class DrawcallOptimize
    {
        public static void DrawcallOptimizeGameObject(GameObject go)
        {
            List<GameObject> staticList = new List<GameObject>();

            bool flag = false;
            do
            {
                if (null != go.GetComponent<Animation>())
                {
                    break;
                }

                if (null != go.GetComponent<SceneAnimationTrigger>())
                {
                    break;
                }

                if (null != go.GetComponent<ParticleSystem>())
                {
                    break;
                }

                if (null != go.GetComponent<ModelTextureAnimation>())
                {
                    break;
                }

                flag = true;

            } while (false);

            go.gameObject.isStatic = flag;

            if (go.GetComponent<SceneAnimationTrigger>() != null)
            {
                SceneAnimationTrigger ada = go.GetComponent<SceneAnimationTrigger>();
                {
                    // foreach(var d in ada.KeyFrames)
                    var __enumerator1 = (ada.KeyFrames).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var d = __enumerator1.Current;
                        {
                            staticList.Add(d.Target);
                        }
                    }
                }

            }
            {
                var __list2 = staticList;
                var __listCount2 = __list2.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var a = __list2[__i2];
                    {

                        a.isStatic = false;


                    }
                }
            }
        }

		[MenuItem("Tools/Optimize Resource/Draw Call Optimize In (Selection)", true)]
        private static bool NotGetFiltered()
        {
            return Selection.activeObject;
        }

		[MenuItem("Tools/Optimize Resource/Draw Call Optimize In (Selection)")]
        ///把除了带Animation的 带SceneAnimationTrigger 是SceneAnimationTrigger的Target 的perfab 都设成static

        public static void DrawcallOptimizemethod()
        {
            // GameObject[] gameObjects = EnumAssets.EnumGameObjectRecursiveInCurrentSelection() as GameObject[];
            List<GameObject> staticList = new List<GameObject>();
            int k = 0;
            {
                // foreach(var go in EnumAssets.EnumGameObjectRecursiveInCurrentSelection())
                var __enumerator4 = (EnumAssets.EnumGameObjectRecursiveInCurrentSelection()).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var go = __enumerator4.Current;
                    {
                        EditorUtility.DisplayProgressBar("Draw Call Optimize", go.name,
                            k * 1.0f / EnumAssets.EnumGameObjectRecursiveInCurrentSelection().Count());

                        bool flag = false;

                        do
                        {
                            if (null != go.GetComponent<Animation>())
                            {
                                break;
                            }

                            if (null != go.GetComponent<SceneAnimationTrigger>())
                            {
                                break;
                            }

                            if (null != go.GetComponent<ParticleSystem>())
                            {
                                break;
                            }

                            if (null != go.GetComponent<ModelTextureAnimation>())
                            {
                                break;
                            }

                            flag = true;

                        } while (false);

                        go.gameObject.isStatic = flag;

                        if (go.GetComponent<SceneAnimationTrigger>())
                        {
                            SceneAnimationTrigger ada = go.GetComponent<SceneAnimationTrigger>();
                            foreach (var d in ada.KeyFrames)
                            {

                                staticList.Add(d.Target);


                            }

                        }


                        k++;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            {
                var __list5 = staticList;
                var __listCount5 = __list5.Count;
                for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                {
                    var a = __list5[__i5];
                    {

                        a.isStatic = false;


                    }
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

        }
        public static bool DrawcallOptimizemethod(string ASSET_PATH)
        {
            EditorUtility.DisplayProgressBar("DrawcallOptimizemethod in Path", "Collecting Object ", 0);
            string str = "";
            Debug.Log("DrawcallOptimizemethod----------------begin");
            int i = 0;
            var gos =
                EnumAssets.EnumAllGameObjectDependenciesRecursive(
                    EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH));

            int total = 0;
            List<GameObject> staticList = new List<GameObject>();
            int k = 0;
            {
                // foreach(var c in gos)
                var __enumerator6 = (gos).GetEnumerator();
                while (__enumerator6.MoveNext())
                {
                    var c = __enumerator6.Current;
                    {
                        total++;
                    }
                }
            }
            {
                // foreach(var go in gos)
                var __enumerator8 = (gos).GetEnumerator();
                while (__enumerator8.MoveNext())
                {
                    var go = __enumerator8.Current;
                    {
                        // GameObject[] gameObjects = EnumAssets.EnumGameObjectRecursiveInCurrentSelection() as GameObject[];


                        EditorUtility.DisplayProgressBar("Draw Call Optimize", go.name,
                            k * 1.0f / total);

                        bool flag = false;

                        do
                        {
                            if (null != go.GetComponent<Animation>())
                            {
                                break;
                            }

                            if (null != go.GetComponent<SceneAnimationTrigger>())
                            {
                                break;
                            }

                            if (null != go.GetComponent<ParticleSystem>())
                            {
                                break;
                            }

                            if (null != go.GetComponent<ModelTextureAnimation>())
                            {
                                break;
                            }

                            flag = true;

                        } while (false);

                        go.gameObject.isStatic = flag;

                        if (go.GetComponent<SceneAnimationTrigger>())
                        {
                            SceneAnimationTrigger ada = go.GetComponent<SceneAnimationTrigger>();
                            foreach (var d in ada.KeyFrames)
                            {

                                staticList.Add(d.Target);

                                str += go.name + "\n";
                            }

                        }


                        k++;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            if (!string.IsNullOrEmpty(str))
            {
                Debug.Log(str);
            }
            {
                var __list9 = staticList;
                var __listCount9 = __list9.Count;
                for (int __i9 = 0; __i9 < __listCount9; ++__i9)
                {
                    var a = __list9[__i9];
                    {

                        a.isStatic = false;


                    }
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Debug.Log("DrawcallOptimizemethod-Total=[" + i.ToString() + "]---------------end");
            return true;
        }
    }
}
