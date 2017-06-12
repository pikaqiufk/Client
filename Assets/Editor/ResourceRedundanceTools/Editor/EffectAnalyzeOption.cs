using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;



public class EffectAnalyzeOption
{
    private string mEffectPath = "";
    private List<string> mEffectPathNameList = new List<string>();

    private const string mEffectPerformanceObjName = "EffectPerformanceObj";
    private Vector2 mListPosition = new Vector2(0.0f, 0.0f);

    private EffectPerformanceCollection.EffectPerformanceData mData = null;

    private enum ViewCategory
    {
        VC_EFFECT,
        VC_SUB_EFFECT
    }

    ViewCategory mViewCategory = ViewCategory.VC_EFFECT;
    private string[] mViewToolbarStrings = { "Effect Performance Data", "SubEffects Info Of Selected Effect" };

    private void GetFiles(DirectoryInfo dir)
    {
        FileInfo[] allFiles = dir.GetFiles();
        {
            var __array1 = allFiles;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var fi = (FileInfo)__array1[__i1];
                {
                    string extName = Path.GetExtension(fi.Name).ToLower();

                    if (extName != ".prefab")
                    {
                        continue;
                    }

                    string fullName = fi.DirectoryName + "/" + fi.Name;
                    fullName = fullName.Replace('\\', '/');
                    fullName = fullName.Replace(Application.dataPath, "Assets");

                    mEffectPathNameList.Add(fullName);
                }
            }
        }
        DirectoryInfo[] allDirs = dir.GetDirectories();
        {
            var __array2 = allDirs;
            var __arrayLength2 = __array2.Length;
            for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
            {
                var d = (DirectoryInfo)__array2[__i2];
                {
                    GetFiles(d);
                }
            }
        }
    }

    public void CollectEffectPerformanceData()
    {
        GameObject effectPerformanceObj;
        if (null != GameObject.Find(mEffectPerformanceObjName))
        {
            effectPerformanceObj = GameObject.Find(mEffectPerformanceObjName);
        }
        else
        {
            effectPerformanceObj = new GameObject(mEffectPerformanceObjName);
        }

        EffectPerformanceCollection component = effectPerformanceObj.GetComponent<EffectPerformanceCollection>();
        if (null == component)
        {
            component = effectPerformanceObj.AddComponent<EffectPerformanceCollection>();
        }

        if ("" != mEffectPath)
        {
            mEffectPathNameList.Clear();
            DirectoryInfo dir = new DirectoryInfo(mEffectPath);
            GetFiles(dir);

            component.mEffectPathNameArray = null;
            component.mEffectPathNameArray = new string[mEffectPathNameList.Count];
            component.mEffectPathNameArray = mEffectPathNameList.ToArray();
        }

    }

    private void ShowEffectPerformanceHeadInfo()
    {
        EffectPerformanceCollection component = null;
        GameObject effectPerformanceObj = GameObject.Find(mEffectPerformanceObjName);
        if (null != effectPerformanceObj)
        {
            component = effectPerformanceObj.GetComponent<EffectPerformanceCollection>();
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Effect File\nName", GUILayout.Width(300)))
        {

        }
        if (GUILayout.Button("Effect Path\nName", GUILayout.Width(400)))
        {

        }
        if (GUILayout.Button("Average Particle\nNumber", GUILayout.Width(120)) && null != component)
        {
            component.mEffectPerformanceDataList.Sort(EffectPerformanceCollection.EffectPerformanceData.SortByAverageParticleNum);

        }
        if (GUILayout.Button("Average Particle\nSize", GUILayout.Width(120)) && null != component)
        {
            component.mEffectPerformanceDataList.Sort(EffectPerformanceCollection.EffectPerformanceData.SortByAverageParticleSize);

        }
        if (GUILayout.Button("Average\nFPS", GUILayout.Width(120)) && null != component)
        {
            component.mEffectPerformanceDataList.Sort(EffectPerformanceCollection.EffectPerformanceData.SortByAverageFPS);
        }

        GUILayout.EndHorizontal();
    }

    private void ShowSubEffectPerformanceHeadInfo()
    {
        if (null == mData)
        {
            return;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("SubEffect\nName", GUILayout.Width(200)))
        {

        }
        if (GUILayout.Button("Average Particle\nNumber", GUILayout.Width(120)))
        {

        }
        if (GUILayout.Button("Average Particle\nSize", GUILayout.Width(120)))
        {

        }

        GUILayout.EndHorizontal();
    }


    private void DrawRow(EffectPerformanceCollection.EffectPerformanceData data)
    {
        GUI.color = (mData == data) ? new Color(0f, 0.5f, 1f) : Color.white;

        GUILayout.BeginHorizontal();

        string fileName = Path.GetFileName(data.mPathFileName);
        string pathName = Path.GetDirectoryName(data.mPathFileName);

        if (GUILayout.Button(fileName, EditorStyles.label, GUILayout.Width(300f)))
        {
            mData = data;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(data.mPathFileName, typeof(Object));
        }

        if (GUILayout.Button(pathName, EditorStyles.label, GUILayout.Width(400f)))
        {
            mData = data;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(data.mPathFileName, typeof(Object));
        }

        if (GUILayout.Button(data.mAverageParticleNum.ToString(), EditorStyles.label, GUILayout.Width(120f)))
        {
            mData = data;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(data.mPathFileName, typeof(Object));
        }

        if (GUILayout.Button(data.mAverageParticleSize.ToString(), EditorStyles.label, GUILayout.Width(120f)))
        {
            mData = data;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(data.mPathFileName, typeof(Object));
        }

        if (GUILayout.Button(data.mAverageFPS.ToString(), EditorStyles.label, GUILayout.Width(120f)))
        {
            mData = data;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(data.mPathFileName, typeof(Object));
        }

        GUILayout.EndHorizontal();
    }


    private void ShowEffectPerformanceList()
    {
        GameObject effectPerformanceObj = GameObject.Find(mEffectPerformanceObjName);

        if (null != effectPerformanceObj)
        {
            EffectPerformanceCollection component = effectPerformanceObj.GetComponent<EffectPerformanceCollection>();
            if (null != component && component.GetFinish())
            {
                {
                    var __list3 = component.mEffectPerformanceDataList;
                    var __listCount3 = __list3.Count;
                    for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                    {
                        var data = (EffectPerformanceCollection.EffectPerformanceData)__list3[__i3];
                        {
                            DrawRow(data);
                        }
                    }
                }
            }
        }
    }

    private void ShowSubEffectPerformanceList()
    {
        if (null == mData)
        {
            return;
        }
        {
            var __list4 = mData.mSubEffectInfoList;
            var __listCount4 = __list4.Count;
            for (int __i4 = 0; __i4 < __listCount4; ++__i4)
            {
                var info = (SubEffectInfo)__list4[__i4];
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Button(info.mName, EditorStyles.label, GUILayout.Width(200f));
                    GUILayout.Button(info.mNum.ToString(), EditorStyles.label, GUILayout.Width(120f));
                    GUILayout.Button(info.mSize.ToString(), EditorStyles.label, GUILayout.Width(120f));
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }


    public void OnGUI()
    {
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();

        GUI.backgroundColor = Color.green;
        bool choose = false;
        choose = GUILayout.Button("Choose Effect folder", GUILayout.Width(200f));

        if (choose)
        {
            mEffectPath = EditorUtility.SaveFolderPanel("Choose Effect folder", "", "");
        }

        GUI.backgroundColor = Color.white;
        mEffectPath = GUILayout.TextField(mEffectPath);

        GUILayout.EndHorizontal();


        if (GUILayout.Button("Collect Effect Performance Data"))
        {
            CollectEffectPerformanceData();
        }

        EditorGUILayout.Space();
        mViewCategory = (ViewCategory)GUILayout.Toolbar((int)mViewCategory, mViewToolbarStrings);

        switch (mViewCategory)
        {
            case ViewCategory.VC_EFFECT:
                EditorGUILayout.Space();
                ShowEffectPerformanceHeadInfo();

                EditorGUILayout.Space();
                mListPosition = GUILayout.BeginScrollView(mListPosition);
                ShowEffectPerformanceList();
                GUILayout.EndScrollView();
                break;

            case ViewCategory.VC_SUB_EFFECT:
                EditorGUILayout.Space();
                ShowSubEffectPerformanceHeadInfo();

                EditorGUILayout.Space();
                mListPosition = GUILayout.BeginScrollView(mListPosition);
                ShowSubEffectPerformanceList();
                GUILayout.EndScrollView();
                break;
        }

    }





}
