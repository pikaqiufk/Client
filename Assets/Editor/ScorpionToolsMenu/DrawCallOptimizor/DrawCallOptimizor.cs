using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DrawCallOptimizor : ScriptableWizard
{
    public string Path = "Assets/Res/UI";

    private IEnumerable<GameObject> mGos;
    private IEnumerator<GameObject> mGosEnumerator;
    private GameObject mRoot;
    private GameObject mCurObj;
    private UIPanel mRootPanel;
    private int mCount;
    private int mIndex;
    private int mStep;

    [MenuItem("Tools/DrawCallOptimizor")]
    static void CreateView()
    {
        DisplayWizard<DrawCallOptimizor>("DrawCallOptimizor", "Close", "Go");
    }

    private DateTime mTimer;
    private const double Interval = 10.0;

    void Update()
    {
        if (mCurObj)
        {
            var now = DateTime.Now;
            if ((now - mTimer).TotalMilliseconds < Interval)
            {
                return;
            }
            mTimer = now;

            var p1 = mCurObj.GetComponent<UIPanel>();
            var root = p1 ?? mRootPanel;
            if (UIDrawCallViewer.AutoAdjustDepth(root, mStep++))
            {
                mStep = 0;

                EditorUtility.DisplayProgressBar("Draw Call Optimize", mCurObj.name, 1.0f * mIndex / mCount);

                var path = AssetDatabase.GetAssetPath(mGosEnumerator.Current);
                PrefabUtility.CreatePrefab(path, mCurObj);
                DestroyImmediate(mCurObj);
                GetNextObj();
            }
        }
    }

    void OnWizardUpdate()
    {
        if (string.IsNullOrEmpty(Path))
        {
            errorString = "Please enter Path!";
            isValid = false;
        }
        else
        {
            errorString = "";
            isValid = true;
        }
    }

    void OnWizardCreate()
    {
    }

    void OnWizardOtherButton()
    {
        if (mRoot == null)
        {
            {
                // foreach(var p in UIPanel.list.ToArray())
                var __enumerator1 = (UIPanel.list.ToArray()).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var p = (UIPanel)__enumerator1.Current;
                    {
                        p.gameObject.SetActive(false);
                    }
                }
            }

            mRoot = new GameObject("root");
            mRoot.AddComponent<UIPanel>();
            mRootPanel = mRoot.GetComponent<UIPanel>();
            mGos = EnumAssets.EnumGameObjectAtPath(Path);
            mCount = mGos.ToList().Count;
        }
        mIndex = 0;
        mGosEnumerator = mGos.GetEnumerator();
        GetNextObj();
        mTimer = DateTime.Now;
    }

    private void GetNextObj()
    {
        string path;
        do
        {
            if (mGosEnumerator.MoveNext())
            {
                ++mIndex;
                //if (mIndex > 3)
                //{
                //    End();
                //    return;
                //}
                path = AssetDatabase.GetAssetPath(mGosEnumerator.Current);
            }
            else
            {
                End();
                return;
            }
        } while (path.Contains("/Atlas/") || path.Contains("/Font/") || path.Contains("NameBoard.prefab"));

        mCurObj = (GameObject)Instantiate(mGosEnumerator.Current, Vector3.zero, Quaternion.identity);
        mCurObj.gameObject.SetActive(true);
        mCurObj.transform.parent = mRoot.transform;
    }

    private void End()
    {
        mCurObj = null;
        DestroyImmediate(mRoot);
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
}
