using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class NPCEditorRoot : MonoBehaviour
{

    public float UpdateDuration = 1.0f;
    private float m_updateTimer = 1.0f;
    public float m_terrainHeight = 20.0f;
    public int curSceneID = 1;
    public int curDataID = -1;
    public float curRadius = 1;
    public Color curColor = Color.white;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        m_updateTimer -= Time.deltaTime;
        if (m_updateTimer < 0)
        {
            m_updateTimer = UpdateDuration;
            {
                var __array1 = transform.GetComponentsInChildren<NPCPresent>(false);
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var child = (NPCPresent)__array1[__i1];
                    {
                        child.SetHeight(m_terrainHeight);
                        if (child.DataID == curDataID)
                        {

                            child.SetColor(curColor);
                            child.SetRadius(curRadius);
                        }
                    }
                }
            }
        }
    }

    public class NPCData
    {
        public int Id;
        public string name;
        public int sceneID;
        public int dataID;
        public float xPos;
        public float yPos;
        public float rad;
        public int visiableInMiniMap;
        public int createInScene;
        public int randCoordStartId;
        public int randCoordEndId;
        public int group;

        public string GetString()
        {
            return (Id + "\t" +
                name + "\t" +
                dataID.ToString() + "\t" +
                sceneID.ToString() + "\t" +
                xPos.ToString("0.##") + "\t" +
                yPos.ToString("0.##") + "\t" +
                rad.ToString("0.##") + "\t" +
                visiableInMiniMap.ToString() + "\t" +
                createInScene.ToString() + "\t" +
                randCoordStartId.ToString() + "\t" +
                randCoordEndId.ToString() + "\t" +
                group.ToString()
                );
        }
    };

    public static Quaternion DirServerToClient(float rad)
    {
        return Quaternion.Euler(0, 90.0f - rad * 180.0f / Mathf.PI, 0);
    }

    public static float DirClientToServer(Quaternion rotate)
    {
        return Mathf.PI * 0.5f - rotate.eulerAngles.y * Mathf.PI / 180.0f;
    }

    private Dictionary<int, List<NPCData>> m_dicNPCCache = new Dictionary<int, List<NPCData>>();
    public void ExportData(string filePath)
    {

        LoadFile(filePath);
        if (m_dicNPCCache.ContainsKey(curSceneID))
        {
            m_dicNPCCache[curSceneID] = new List<NPCData>();
        }
        else
        {
            m_dicNPCCache.Add(curSceneID, new List<NPCData>());
        }
        {
            var __array2 = transform.GetComponentsInChildren<NPCPresent>(false);
            var __arrayLength2 = __array2.Length;
            for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
            {
                var child = (NPCPresent)__array2[__i2];
                {
                    NPCData curNPC = new NPCData();
                    curNPC.Id = child.Id;
                    curNPC.name = child.NPCName;
                    curNPC.sceneID = curSceneID;
                    curNPC.dataID = child.DataID;
                    curNPC.xPos = child.transform.position.x;
                    curNPC.yPos = child.transform.position.z;
                    curNPC.rad = DirClientToServer(child.transform.rotation);
                    curNPC.visiableInMiniMap = child.ShowInMiniMap;
                    curNPC.createInScene = child.CreateInScene;
                    curNPC.randCoordStartId = child.RandCoordStartId;
                    curNPC.randCoordEndId = child.RandCoordEndId;
                    curNPC.group = child.Group;
                    m_dicNPCCache[curSceneID].Add(curNPC);
                }
            }
        }
        WriteFile(filePath);
        //ConvertFile(filePath);
    }

    public void ImportData(string path)
    {
        {
            // foreach(var curObj in transform)
            var __enumerator3 = (transform).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var curObj = (Transform)__enumerator3.Current;
                {
                    DestroyImmediate(curObj.gameObject);
                }
            }
        }

        List<GameObject> objList = new List<GameObject>();
        for (int i = 0, count = transform.childCount; i < count; i++)
        {
            objList.Add(transform.GetChild(i).gameObject);
        }
        {
            var __list4 = objList;
            var __listCount4 = __list4.Count;
            for (int __i4 = 0; __i4 < __listCount4; ++__i4)
            {
                var curObj = (GameObject)__list4[__i4];
                {
                    DestroyImmediate(curObj);
                }
            }
        }
        LoadFile(path);
        if (!m_dicNPCCache.ContainsKey(curSceneID))
        {
            return;
        }
        List<NPCData> curDataList = m_dicNPCCache[curSceneID];

        NPCPresent curPresent = Resources.LoadAssetAtPath("Assets/NotBuild/NpcEditor/NPCInst.prefab", typeof(NPCPresent)) as NPCPresent;

        int curID = 0;
        {
            var __list5 = curDataList;
            var __listCount5 = __list5.Count;
            for (int __i5 = 0; __i5 < __listCount5; ++__i5)
            {
                var curNPC = (NPCData)__list5[__i5];
                {
                    GameObject newRes = GameObject.Instantiate(curPresent.gameObject) as GameObject;
                    newRes.transform.parent = transform;
                    NPCPresent curNPCS = newRes.GetComponent<NPCPresent>();
                    curNPCS.transform.position = new Vector3(curNPC.xPos, 0, curNPC.yPos);
                    curNPCS.transform.rotation = DirServerToClient(curNPC.rad);
                    curNPCS.NPCName = curNPC.name;
                    curNPCS.DataID = curNPC.dataID;
                    curNPCS.Id = curNPC.Id;
                    curNPCS.ShowInMiniMap = curNPC.visiableInMiniMap;
                    curNPCS.CreateInScene = curNPC.createInScene;
                    curNPCS.RandCoordStartId = curNPC.randCoordStartId;
                    curNPCS.RandCoordEndId = curNPC.randCoordEndId;
                    curNPCS.Group = curNPC.group;
                    newRes.name = curID.ToString() + "_" + curNPC.name;
                    curID++;
                }
            }
        }
        return;
    }

    private void LoadFile(string filePath)
    {

        m_dicNPCCache.Clear();
        if (File.Exists(filePath))
        {
            FileStream rfs = new FileStream(filePath, FileMode.Open);
            StreamReader sr = new StreamReader(rfs, Encoding.GetEncoding("GB2312"));

            int i = 0;
            while (!sr.EndOfStream)
            {
                string curLine = sr.ReadLine();
                Debug.Log(curLine);
                if (i++ < 3) continue;

                string[] values = curLine.Split('\t');
                if (values.Length != 12)
                {
                    Debug.LogError(filePath + "表格应该有12列");
                    return;
                }

                NPCData curData = new NPCData();
                if (!int.TryParse(values[0], out curData.Id))
                {
                    Debug.LogError("第0列应该是int");
                    return;
                }
                curData.name = values[1];
                if (!int.TryParse(values[2], out curData.dataID))
                {
                    Debug.LogError("第2列应该是int");
                    return;
                }
                if (!int.TryParse(values[3], out curData.sceneID))
                {
                    Debug.LogError("第3列应该是int");
                    return;
                }
                if (!float.TryParse(values[4], out curData.xPos))
                {
                    Debug.LogError("第4列应该是float");
                    return;
                }
                if (!float.TryParse(values[5], out curData.yPos))
                {
                    Debug.LogError("第5列应该是float");
                    return;
                }
                if (!float.TryParse(values[6], out curData.rad))
                {
                    Debug.LogError("第6列应该是float");
                    return;
                }
                if (!int.TryParse(values[7], out curData.visiableInMiniMap))
                {
                    Debug.LogError("第7列应该是int");
                    return;
                }
                if (!int.TryParse(values[8], out curData.createInScene))
                {
                    Debug.LogError("第8列应该是int");
                    return;
                }
                if (!int.TryParse(values[9], out curData.randCoordStartId))
                {
                    Debug.LogError("第9列应该是int");
                    return;
                }
                if (!int.TryParse(values[10], out curData.randCoordEndId))
                {
                    Debug.LogError("第10列应该是int");
                    return;
                }
                if (!int.TryParse(values[11], out curData.group))
                {
                    Debug.LogError("第11列应该是int");
                    return;
                }

                if (!m_dicNPCCache.ContainsKey(curData.sceneID))
                {
                    m_dicNPCCache.Add(curData.sceneID, new List<NPCData>());
                }

                m_dicNPCCache[curData.sceneID].Add(curData);
                Debug.Log("get record");
            }

            rfs.Close();
            sr.Close();
        }
        else
        {
            Debug.LogError(filePath + " 文件不存在");
            return;
        }
    }

    void WriteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        FileStream wfs = new FileStream(filePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(wfs, Encoding.GetEncoding("GB2312"));
        sw.WriteLine("INT\tSTRING\tINT\tINT\tFLOAT\tFLOAT\tFLOAT\tINT\tINT\tINT\tINT");
        sw.WriteLine("索引ID\tNPC名称\tNPCID\t场景ID\tX坐标\tZ坐标\t朝向修正度数（默认正北）\t是否在小地图显示\t是否直接刷新在场景中\t随机坐标起始ID\t随机坐标结束ID");
        sw.WriteLine("#SceneNpc");
        {
            List<NPCData> orderById = new List<NPCData>();
            // foreach(var curList in m_dicNPCCache.Values)
            var __enumerator6 = (m_dicNPCCache.Values).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var curList = (List<NPCData>)__enumerator6.Current;
                {
                    {
                        var __list7 = curList;
                        var __listCount7 = __list7.Count;
                        for (int __i7 = 0; __i7 < __listCount7; ++__i7)
                        {
                            var curNPC = (NPCData)__list7[__i7];
                            {
                                //sw.WriteLine(curNPC.GetString());
                                orderById.Add(curNPC);
                            }
                        }
                    }
                }
            }

            orderById.Sort((l, r) =>
            {
                return l.Id - r.Id;
            });
            {
                var __list8 = orderById;
                var __listCount8 = __list8.Count;
                for (int __i8 = 0; __i8 < __listCount8; ++__i8)
                {
                    var item = __list8[__i8];
                    {
                        sw.WriteLine(item.GetString());
                    }
                }
            }
        }

        sw.Close();
        wfs.Close();

    }

    void ConvertFile(string filePath)
    {
#if UNITY_EDITOR
        FileStream rfs = new FileStream(filePath, FileMode.Open);
        StreamReader sr = new StreamReader(rfs);
        string curFile = sr.ReadToEnd();
        rfs.Close();
        sr.Close();

        FileStream wfs = new FileStream(filePath, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(wfs);
        Byte[] bytes = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(curFile));


        //Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(curFile.ToCharArray()));
        sw.Write(bytes);
        sw.Close();
        wfs.Close();
#endif
    }
}
