using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.ScorpionLib.Tools.Editor;
using UnityEngine;
using UnityEditor;

namespace Assets.ScorpionLib.Tools.Editor
{
    public class DeleteSpriteRendererTools
    {

		[UnityEditor.MenuItem("Tools/Optimize Resource/Delete Sprite Renderer In (Assets.Res.UI)")]
        public static void DeleteSpriteRenderer()
        {
            const string assetPath = "Assets/Res/UI";
            DeleteSpriteRenderer(
                EnumAssets.EnumAllComponentDependenciesRecursive<SpriteRenderer>(
                    EnumAssets.EnumAssetAtPath<UnityEngine.Object>(assetPath)));
        }

        public static void DeleteSpriteRenderer(IEnumerable<SpriteRenderer> cs)
        {
            EditorUtility.DisplayProgressBar("Delete SpriteRenderer", "Collecting SpriteRenderer Components", 0);

            var spriteRenderers = cs as IList<SpriteRenderer> ?? cs.ToList();
            var total = spriteRenderers.Count();

            string str = "";

            int i = 0;
            int processed = 0;
            {
                // foreach(var go in spriteRenderers)
                var __enumerator1 = (spriteRenderers).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var go = __enumerator1.Current;
                    {
                        EditorUtility.DisplayProgressBar("Delete SpriteRenderer", go.gameObject.name, i * 1.0f / total);
                        var g = go.gameObject;


                        str += g.name + "\n";
                        SpriteRenderer mObjSc = g.GetComponent<SpriteRenderer>();
                        GameObject.DestroyImmediate(mObjSc, true);
                        UnityEditor.EditorUtility.SetDirty(g);
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            Debug.Log("Delete SpriteRenderer----------------------------begin");
            if (!string.IsNullOrEmpty(str))
            {
                Debug.Log(str);
            }
            Debug.Log("Delete SpriteRenderer----------------------------end-total=[" + processed.ToString() + "]");

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }

}