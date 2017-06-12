using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ResourceRedundanceManager
{
    public Dictionary<int, Dictionary<string, ResourceUnit>> mTypeFileDictionary = new Dictionary<int, Dictionary<string, ResourceUnit>>();
    private int mAssetPathStartIndex = Application.dataPath.IndexOf("Assets");

    private Dictionary<int, AnalyzeBase> mAnalyzeDictionary = new Dictionary<int, AnalyzeBase>();
    private string mScenePath;

    private Queue<ResourceUnit> mResUnitQueue = new Queue<ResourceUnit>();




    public ResourceRedundanceManager()
    {
        RegisterAnalyzers();

    }

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

                    if (string.IsNullOrEmpty(extName) || ".meta" == extName)
                    {
                        continue;
                    }

                    int analyzeType = ResourceUnit.GetAnalyzeType(extName);

                    if (!mTypeFileDictionary.ContainsKey(analyzeType))
                    {
                        mTypeFileDictionary.Add(analyzeType, new Dictionary<string, ResourceUnit>());
                    }

                    Dictionary<string, ResourceUnit> fileDictionary = mTypeFileDictionary[analyzeType] as Dictionary<string, ResourceUnit>;

                    string dirName = fi.DirectoryName.Replace('\\', '/');
                    dirName = dirName.Remove(0, mAssetPathStartIndex);
                    ResourceUnit resUnit = new ResourceUnit(fi.Name, dirName);
                    string fullName = dirName + "/" + fi.Name;

                    fileDictionary.Add(fullName, resUnit);

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

    private void RegisterSingleAnalyzer(AnalyzeBase analyzer)
    {
        mAnalyzeDictionary.Add(analyzer.GetAnalyzeType(), analyzer);
    }

    private void RegisterAnalyzers()
    {
        RegisterSingleAnalyzer(new AnalyzeScene(this));
        RegisterSingleAnalyzer(new AnalyzePrefab(this));
        RegisterSingleAnalyzer(new AnalyzeMat(this));
        RegisterSingleAnalyzer(new AnalyzeClientTable(this));

    }

    AnalyzeBase GetAnalyzer(int type)
    {
        if (mAnalyzeDictionary.ContainsKey(type))
        {
            return mAnalyzeDictionary[type];
        }

        return null;
    }

    public void CollectAllFiles()
    {
        mTypeFileDictionary.Clear();
        DirectoryInfo rootDir = new DirectoryInfo(Application.dataPath);
        GetFiles(rootDir);

    }

    public void AnalyzeResourceFiles()
    {
        // Step 1: Scene
        mScenePath = "D:\\XKX\\Main\\RTM0_02\\Client\\Assets\\Scene";

        DirectoryInfo sceneDir = new DirectoryInfo(mScenePath);
        FileInfo[] fis = sceneDir.GetFiles("*.unity", SearchOption.TopDirectoryOnly);

        int sceneType = (int)(AnalyzeType.AT_SCENE);
        Dictionary<string, ResourceUnit> sceneFileDictionary = mTypeFileDictionary[sceneType];
        {
            var __array3 = fis;
            var __arrayLength3 = __array3.Length;
            for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
            {
                var fi = (FileInfo)__array3[__i3];
                {
                    string name = fi.DirectoryName.Replace('\\', '/');
                    name = name.Remove(0, mAssetPathStartIndex);
                    name = name + "/" + fi.Name;

                    if (sceneFileDictionary.ContainsKey(name))
                    {
                        sceneFileDictionary[name].SetResourceAnalyzeType(sceneType);
                        sceneFileDictionary[name].SetQueued();
                        mResUnitQueue.Enqueue(sceneFileDictionary[name]);
                    }
                }
            }
        }
        // Step 2: Table
        // NPC
        List<string> tableFileList = new List<string>
                                    { "Assets/Resources/TableFile/NpcAttr.txt", "4", "Prefab/NPC/",
                                      "Assets/Resources/TableFile/UIInterface.txt", "2", ""

                                    };

        int tableType = (int)(AnalyzeType.AT_TXT);
        Dictionary<string, ResourceUnit> tableFileDictionary = mTypeFileDictionary[tableType];

        for (int i = 0; i < tableFileList.Count; i += 3)
        {
            string name = tableFileList[i];
            int column = Convert.ToInt32(tableFileList[i + 1]);
            string secName = tableFileList[i + 2];
            if (tableFileDictionary.ContainsKey(name))
            {
                tableFileDictionary[name].SetResourceAnalyzeType(tableType);
                tableFileDictionary[name].SetQueued();
                tableFileDictionary[name].SetTableValidColmn(column);
                tableFileDictionary[name].SetTablePathSectorName(secName);
                mResUnitQueue.Enqueue(tableFileDictionary[name]);
            }

        }




        // Player
        string playerPathName_0 = "D:\\XKX\\Main\\RTM0_02\\Client\\Assets\\Resources\\Prefab\\Player\\Player_SL.prefab";
        string playerPathName_1 = "D:\\XKX\\Main\\RTM0_02\\Client\\Assets\\Resources\\Prefab\\Player\\Player_EM.prefab";

        int prefabType = (int)(AnalyzeType.AT_PREFAB);
        Dictionary<string, ResourceUnit> prefabFileDictionary = mTypeFileDictionary[prefabType];

        playerPathName_0 = playerPathName_0.Replace('\\', '/');
        playerPathName_0 = playerPathName_0.Remove(0, mAssetPathStartIndex);

        if (prefabFileDictionary.ContainsKey(playerPathName_0))
        {
            prefabFileDictionary[playerPathName_0].SetResourceAnalyzeType(prefabType);
            prefabFileDictionary[playerPathName_0].SetQueued();
            mResUnitQueue.Enqueue(prefabFileDictionary[playerPathName_0]);
        }

        playerPathName_1 = playerPathName_1.Replace('\\', '/');
        playerPathName_1 = playerPathName_1.Remove(0, mAssetPathStartIndex);

        if (prefabFileDictionary.ContainsKey(playerPathName_1))
        {
            prefabFileDictionary[playerPathName_1].SetResourceAnalyzeType(prefabType);
            prefabFileDictionary[playerPathName_1].SetQueued();
            mResUnitQueue.Enqueue(prefabFileDictionary[playerPathName_1]);
        }




        while (mResUnitQueue.Count > 0)
        {
            ResourceUnit resUnit = mResUnitQueue.Dequeue();

            AnalyzeBase analyzer = GetAnalyzer(resUnit.GetResourceAnalyzeType());
            if (null != analyzer)
            {
                analyzer.Analyze(resUnit);
            }
        }
    }

    public void ProcessResource(int type, string name, ResourceUnit parentRes)
    {
        if (mTypeFileDictionary.ContainsKey(type))
        {
            Dictionary<string, ResourceUnit> fileDictionary = mTypeFileDictionary[type];

            if (fileDictionary.ContainsKey(name))
            {
                ResourceUnit res = fileDictionary[name];

                res.SetResourceAnalyzeType(type);
                res.AddReferrence(parentRes);
                parentRes.AddInclude(res);

                AnalyzeBase analyzer = GetAnalyzer(type);
                if (null != analyzer)
                {
                    if (!res.IsQueued())
                    {
                        res.SetQueued();
                        mResUnitQueue.Enqueue(res);
                    }
                }
            }
        }
    }

    public void DeleteSingleResourceFile(ResourceUnit resUnit)
    {
        int type = ResourceUnit.GetAnalyzeType(Path.GetExtension(resUnit.GetFileName()).ToLower());
        if (mTypeFileDictionary.ContainsKey(type))
        {
            Dictionary<string, ResourceUnit> fileDictionary = mTypeFileDictionary[type];
            string fullName = resUnit.GetPathName() + "/" + resUnit.GetFileName();

            if (fileDictionary.ContainsKey(fullName))
            {
                fileDictionary.Remove(fullName);
                File.Delete(fullName);
            }
        }
    }








}
