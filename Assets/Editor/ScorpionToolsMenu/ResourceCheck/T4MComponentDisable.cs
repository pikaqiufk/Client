using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ScorpionLib.Tools.Editor
{
    internal class T4MComponentDisable
    {
		[MenuItem("Tools/Optimize Resource/T4MComponentDisable In (Selection)", true)]
        private static bool NotGetFiltered()
        {
            return Selection.activeObject;
        }

		[MenuItem("Tools/Optimize Resource/T4MComponentDisable In (Selection)")]
        //关闭T4m插件
        public static void T4MComponentDisablemethod()
        {
            {
                // foreach(var go in EnumAssets.EnumComponentRecursiveInCurrentSelection<T4MObjSC>())
                var __enumerator1 = (EnumAssets.EnumComponentRecursiveInCurrentSelection<T4MObjSC>()).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var go = __enumerator1.Current;
                    {
                        var g = go.gameObject;

                        Debug.Log(g.name + "T4MComponentDisable");
                        T4MObjSC mObjSc = g.GetComponent<T4MObjSC>();
                        mObjSc.enabled = false;
                    }
                }
            }
            AssetDatabase.Refresh();
        }
        public static void T4MComponentDisablemethod(IEnumerable<T4MObjSC> cs)
        {
            EditorUtility.DisplayProgressBar("T4MComponentDisable", "Collecting T4MObjSC Components", 0);

            int total = 0;
            {
                // foreach(var c in cs)
                var __enumerator2 = (cs).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var c = __enumerator2.Current;
                    {
                        total++;
                    }
                }
            }
            string str = "";

            int i = 0;
            int processed = 0;
            {
                // foreach(var go in cs)
                var __enumerator3 = (cs).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var go = __enumerator3.Current;
                    {
                        EditorUtility.DisplayProgressBar("T4MComponentDisable", go.gameObject.name, i * 1.0f / total);
                        var g = go.gameObject;

                        Debug.Log(g.name + "T4MComponentDisable");
                        T4MObjSC mObjSc = g.GetComponent<T4MObjSC>();
                        mObjSc.enabled = false;
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            Debug.Log("T4MComponentDisable----------------------------begin");
            if (!string.IsNullOrEmpty(str))
            {
                Debug.Log(str);
            }
            Debug.Log("T4MComponentDisable----------------------------end-total=[" + processed.ToString() + "]");

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
