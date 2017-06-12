using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ScorpionLib.Tools.Editor
{
    internal class SceneCheckupAndChangeRenderQueue
    {
        [MenuItem("Tools/Optimize Resource/Change Scene Render Queue In (Selection)", true)]
        private static bool NotGetFiltered()
        {
            return Selection.activeObject;
        }

        public static void ProcessRenderQueue(Renderer r, int q)
        {
            var mats = r.sharedMaterials;
            if (null == mats)
            {
                return;
            }
            {
                var __array1 = mats;
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var mat = __array1[__i1];
                    {
                        if (null == mat)
                        {
                            continue;
                        }
                        mat.renderQueue = q;
                    }
                }
            }
        }

		[MenuItem("Tools/Optimize Resource/Change Scene Render Queue In (Selection)")]
        private static void SceneCheckupAndChangeRenderQueueMethod()
        {
            int processed = 0;
            try
            {
                var cs = EnumAssets.EnumComponentRecursiveInCurrentSelection<Renderer>();
                EditorUtility.DisplayProgressBar("Change Scene Render Queue", "Collecting Component", 0);

                int count = cs.Count();
                var layer = LayerMask.NameToLayer("ShadowReceiver");
                var q = 2000 - 2; // Geometry - 2

                int idx = 0;
                {
                    // foreach(var c in cs)
                    var __enumerator2 = (cs).GetEnumerator();
                    while (__enumerator2.MoveNext())
                    {
                        var c = __enumerator2.Current;
                        {
                            idx++;
                            if (c.gameObject.layer != layer)
                            {
                                continue;
                            }
                            processed++;
                            EditorUtility.DisplayProgressBar("Change Scene Render Queue", c.gameObject.name, idx * 1.0f / count);
                            ProcessRenderQueue(c, q);
                        }
                    }
                }
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();

                EditorUtility.DisplayDialog("Change Scene Render Queue", "processed=" + processed.ToString(), "OK");
            }

        }

    }
}
