/********************************************************************************
 *
 *	功能说明：编辑器服务器阻挡生成选项
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;

using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Text;
using DataTable;


public class ServerObstacle : Editor
{
    private const byte NObstacle = 0;        //阻挡标识
    private const byte NRun = 1;            //可行走区域标识（默认是跑的）
    private const byte NWalk = 2;            //走路区域标识
    private static string _szServerObstacleTag = "ServerObstacleTestPoint";    //为服务器行走面的物理体的Tag
    private static string _szWalkableTag = "WalkingArea";    //为服务器走路的Tag
    private static Vector3 _testRayOrigin = new Vector3(0, 100.0f, 0);         //进行射线检测的射线原点
    private static Vector3 _testRayDirection = new Vector3(0, -100.0f, 0);     //进行射线检测的射线方向

    //保存的数据结构
    private struct ObstacleInfo
    {
        public float _FX;
        public float _FZ;
        public byte Value;
    }

    [MenuItem("Tools/Scorpion/ServerObstacle/CreateNavMesh")]
    public static void CreateNavMesh()
    {
        var tri = NavMesh.CalculateTriangulation();
        Mesh m = new Mesh();
        m.vertices = tri.vertices;
        m.triangles = tri.indices;
        string curScene = EditorApplication.currentScene.Substring(EditorApplication.currentScene.LastIndexOf('/') + 1);
        curScene = curScene.Substring(0, curScene.IndexOf('.'));
        string filePath = Application.dataPath + "/../../Public/ServerObstacle/Meshes/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string obstacleFilePath = filePath + curScene;
        using (StreamWriter sw = new StreamWriter(obstacleFilePath + ".obj"))
        {
            sw.Write(MeshToString(m));
        }
    }

    private static string MeshToString(Mesh m)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append("navmesh").Append("\n");
        {
            var __array1 = m.vertices;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var lv = (Vector3)__array1[__i1];
                {
                    Vector3 wv = lv;
                    sb.Append(string.Format("v {0} {1} {2}\n", wv.x, wv.y, wv.z));
                }
            }
        }

        var triangles = m.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            sb.Append(string.Format("f {0} {1} {2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
        }

        return sb.ToString();
    }

    //////////////////////////////////////////////////////////////////////////
    // 生成当前场景服务器阻挡
    //////////////////////////////////////////////////////////////////////////
    [MenuItem("Tools/Scorpion/ServerObstacle/Create")]
    public static void CreateObstacle()
    {
        //根据当前场景名字获得场景名称
        string curScene = EditorApplication.currentScene.Substring(EditorApplication.currentScene.LastIndexOf('/') + 1);
        curScene = curScene.Substring(0, curScene.IndexOf('.'));
        //首先获取当前场景的SceneClass
        SceneRecord sceneDefine = null;

        Debug.Log("Remember to add mesh collider to all the meshes.");

        var name = "Table/Scene.txt";
        var path = "Assets/Res/" + name;
        Dictionary<int, IRecord> Scene = new Dictionary<int, IRecord>();
        var txt = Resources.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
        TableInit.Table_Init(txt.bytes, Scene, TableType.Scene);
        {
            // foreach(var record in Scene)
            var __enumerator1 = (Scene).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var record = __enumerator1.Current;
                {
                    if (((SceneRecord)record.Value).ResName == curScene)
                    {
                        sceneDefine = (SceneRecord)record.Value;
                        break;
                    }
                }
            }
        }

        if (null == sceneDefine)
        {
            Debug.LogError("Scene Table Not Find, SceneID:" + curScene);
            return;
        }

        int nLenth = sceneDefine.TerrainHeightMapLength;
        int nWidth = sceneDefine.TerrainHeightMapWidth;

        //初始化文件
        string obstacleFilePath = GetCurSceneObstacleFilePath();

        FileStream fileStream = new FileStream(obstacleFilePath, FileMode.Create, FileAccess.ReadWrite);
        int nSeek = 0;

        //先将地图的长和宽写入文件头
        fileStream.Seek(nSeek, SeekOrigin.Begin);
        byte[] byteLen = System.BitConverter.GetBytes(nLenth);
        fileStream.Write(byteLen, 0, byteLen.Length);
        nSeek += byteLen.Length;

        fileStream.Seek(nSeek, SeekOrigin.Begin);
        byte[] byteWid = System.BitConverter.GetBytes(nWidth);
        fileStream.Write(byteWid, 0, byteWid.Length);
        nSeek += byteWid.Length;

        bool ok = false;

        //对当前场景进行遍历，进行寻路检测，得到每一个点的可行走类型
        for (float fX = 0.0f; fX <= (float)nWidth; fX += 0.5f)
        {
            for (float fZ = 0.0f; fZ <= (float)nLenth; fZ += 0.5f)
            {
                ObstacleInfo nState = GetScenePosPathState(fX, fZ);
                byte[] write = Struct2Byte(nState);

				if (nState.Value == NRun || nState.Value == NWalk)
                {
                    ok = true;
                }

                fileStream.Seek(nSeek, SeekOrigin.Begin);
                fileStream.Write(write, 0, write.Length);
                nSeek += write.Length;
            }
        }
        fileStream.Close();

        if (!ok)
        {
            Debug.Log("1. Check tag has added to the objects.");
            Debug.Log("2. Check mesh collider has added to the objects.");
        }
        else
        {
            Debug.Log("Server Obstacle Create OK");
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // 查看当前场景服务器阻挡
    //////////////////////////////////////////////////////////////////////////
    [MenuItem("Tools/Scorpion/ServerObstacle/Show")]
    public static void ShowObstacle()
    {
        //这个选项由于用到了地表高度，所以要保证Editor在运行的时候才可以看到
        if (!Application.isPlaying)
        {
            Debug.LogError("This Function Must Run Your Unity Editor");
            return;
        }
        //初始化文件
        string obstaclFilePath = GetCurSceneObstacleFilePath();
        if (!File.Exists(obstaclFilePath))
        {
            return;
        }
        //打开文件，获得阻挡文件长和宽
        FileStream fileStream = new FileStream(obstaclFilePath, FileMode.Open, FileAccess.Read);
        int nSeek = 0;

        //先从文件头读取地图的长和宽
        byte[] byteLen = new byte[Marshal.SizeOf(typeof(int))];
        fileStream.Seek(nSeek, SeekOrigin.Begin);
        fileStream.Read(byteLen, 0, byteLen.Length);
        int nLen = System.BitConverter.ToInt32(byteLen, 0);
        nSeek += byteLen.Length;

        byte[] byteWid = new byte[Marshal.SizeOf(typeof(int))];
        fileStream.Seek(nSeek, SeekOrigin.Begin);
        fileStream.Read(byteWid, 0, byteWid.Length);
        int nWid = System.BitConverter.ToInt32(byteWid, 0);
        nSeek += byteWid.Length;

        //读取数据，显示到客户端
        int nReadLen = Marshal.SizeOf(typeof(ObstacleInfo));
        byte[] read = new byte[nReadLen];
        GameObject obstacleRoot = CreateObstacleRoot();
        if (null != obstacleRoot)
        {
            //创建路径点模型
            GameObject pathObj = AssetDatabase.LoadAssetAtPath("Assets/Editor/Path.prefab", typeof(GameObject)) as GameObject;
            if (null == pathObj)
            {
                return;
            }

            for (int i = 0; i < (nLen * 2 + 1) * (nWid * 2 + 1); ++i)
            {
                fileStream.Seek(nSeek, SeekOrigin.Begin);
                fileStream.Read(read, 0, nReadLen);
                nSeek += nReadLen;

                ObstacleInfo info = Byte2Struct(read);

                //如果该点为路径点，则放置一个叫“Path”的Prefab
                if (info.Value == NRun)
                {
                    GameObject pathInst = (GameObject)GameObject.Instantiate(pathObj);
                    Vector3 pos = new Vector3(info._FX, 20, info._FZ);
                    pathInst.transform.position = pos;
                    pathInst.transform.parent = obstacleRoot.transform;
                }
				else if (info.Value == NWalk)
				{
					GameObject pathInst = (GameObject)GameObject.Instantiate(pathObj);
					Vector3 pos = new Vector3(info._FX, 20, info._FZ);
					pathInst.transform.position = pos;
					pathInst.transform.parent = obstacleRoot.transform;
					pathInst.renderer.material.color = Color.blue;
				}
            }
        }

        fileStream.Close();
        Debug.Log("Server Obstacle Show OK");
    }

    //////////////////////////////////////////////////////////////////////////
    // 隐藏当前场景服务器阻挡
    //////////////////////////////////////////////////////////////////////////
    [MenuItem("Tools/Scorpion/ServerObstacle/Hide")]
    public static void HideObstacle()
    {
        //这个选项由于用到了地表高度，所以要保证Editor在运行的时候才可以看到
        if (!Application.isPlaying)
        {
            Debug.LogError("This Function Must Run Your Unity Editor");
            return;
        }

        CreateObstacleRoot();
        Debug.Log("Server Obstacle Hide OK");
    }

    //得到当前场景的阻挡文件全路径
    public static string GetCurSceneObstacleFilePath()
    {
        string curScene = EditorApplication.currentScene.Substring(EditorApplication.currentScene.LastIndexOf('/') + 1);
        curScene = curScene.Substring(0, curScene.IndexOf('.'));
        string filePath = Application.dataPath + "/../../Server/Scene/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string obstacleFileName = curScene + ".path";
        string obstacleFilePath = filePath + obstacleFileName;
        return obstacleFilePath;
    }

    //创建Obstacle阻挡显示根节点，如果有则清空，没有则创建
    private static GameObject CreateObstacleRoot()
    {
        GameObject obRoot = GameObject.Find("ServerObstacleRoot") ?? new GameObject("ServerObstacleRoot");
        obRoot.AddComponent<PathFinder>();
        GameUtils.CleanGrid(obRoot);
        return obRoot;
    }

    //关键函数，根据某个点获取当前点状态
    private static ObstacleInfo GetScenePosPathState(float fX, float fZ)
    {
        ObstacleInfo info = new ObstacleInfo();
        info._FX = _testRayOrigin.x = fX;
        info._FZ = _testRayOrigin.z = fZ;

        Ray ray = new Ray(_testRayOrigin, _testRayDirection);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //如果检测点是
            info.Value = hit.collider.gameObject.CompareTag(_szServerObstacleTag) ? NRun : NObstacle;
            //info.Value = hit.collider.gameObject.CompareTag(_szWalkableTag) ? NWalk : info.Value;
			if (hit.collider.gameObject.CompareTag(_szWalkableTag)) {
				info.Value = NWalk;
			}
        }
        return info;
    }

    //struct与byte[]相互转换函数
    private static ObstacleInfo Byte2Struct(byte[] arr)
    {
        int structSize = Marshal.SizeOf(typeof(ObstacleInfo));
        IntPtr ptemp = Marshal.AllocHGlobal(structSize);
        Marshal.Copy(arr, 0, ptemp, structSize);
        ObstacleInfo rs = (ObstacleInfo)Marshal.PtrToStructure(ptemp, typeof(ObstacleInfo));
        Marshal.FreeHGlobal(ptemp);
        return rs;
    }

    private static byte[] Struct2Byte(ObstacleInfo s)
    {
        int structSize = Marshal.SizeOf(typeof(ObstacleInfo));
        byte[] buffer = new byte[structSize];
        //分配结构体大小的内存空间 
        IntPtr structPtr = Marshal.AllocHGlobal(structSize);
        //将结构体拷到分配好的内存空间 
        Marshal.StructureToPtr(s, structPtr, false);
        //从内存空间拷到byte数组 
        Marshal.Copy(structPtr, buffer, 0, structSize);
        //释放内存空间 
        Marshal.FreeHGlobal(structPtr);
        return buffer;
    }
}