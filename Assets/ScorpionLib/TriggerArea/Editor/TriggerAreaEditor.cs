#define ID_SORT

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;



public class TriggerAreaData
{
    public int Id;
    public int SceneId;
    public float X;
    public float Y;
    public float R;

    public string GetString()
    {
        string str = "";
        str += Id.ToString() + "\t";
        str += SceneId.ToString() + "\t";
        str += X.ToString() + "\t";
        str += Y.ToString() + "\t";
        str += R.ToString();

        return str;
    }
}

[CanEditMultipleObjects()]
[CustomEditor(typeof(TriggerAreaRoot), true)]
public class TriggerAreaRootEditor : Editor
{

    public const string TriggerAreaTableFilePath = "/../../Server/Tables/TriggerArea.txt";

    static Dictionary<int, List<TriggerAreaData>> mDic = new Dictionary<int, List<TriggerAreaData>>();
    SerializedProperty SceneId;

    static TriggerAreaRoot mRoot;
    public static TriggerAreaRoot GetObjectRoot()
    {

        mRoot = GameObject.FindObjectOfType<TriggerAreaRoot>();
        if (null == mRoot)
        {
            var obj = new GameObject(TriggerAreaRootEditorName);
            mRoot = obj.AddComponent<TriggerAreaRoot>();
        }
        return mRoot;


    }
    static string TriggerAreaRootEditorName = "Trigger Area Root Editor";

    [MenuItem("Tools/Scorpion/Open Trigger Area Editor")]
    public static void OpenTriggerAreaEditor()
    {
        UnityEditor.Selection.activeObject = GetObjectRoot();
    }

    void OnEnable()
    {
        this.SceneId = base.serializedObject.FindProperty("SceneId");

    }

    public override void OnInspectorGUI()
    {
        // 更新编辑器显示的序列化属性
        serializedObject.Update();

        //  没什么说的， 显示属性
        EditorGUILayout.PropertyField(SceneId);

        if (GUILayout.Button("Create Trigger Area", GUILayout.Width(200)))
        {
            CreateTriggerArea();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load", GUILayout.Width(60)))
        {
            Load(SceneId.intValue);
        }

        else if (GUILayout.Button("Save", GUILayout.Width(60)))
        {
            Save();
        }
        EditorGUILayout.EndHorizontal();


        // 接受序列化赋值
        serializedObject.ApplyModifiedProperties();
    }


    public TriggerArea CreateTriggerArea(int id = -1, float x = 0, float z = 0, float r = 1)
    {
        //var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var obj = new GameObject();
        obj.name = "TriggerArea";
        var area = obj.AddComponent<TriggerArea>();
        area.transform.parent = GetObjectRoot().transform;
        area.transform.position = new Vector3(x, 0, z);
        area.Id = id;
        area.SceneId = SceneId.intValue;
        area.Radius = r;

        return area;
    }

    static bool LoadTableToCache()
    {
        string path = Application.dataPath + TriggerAreaTableFilePath;
        if (!File.Exists(path))
        {
            EditorUtility.DisplayDialog("提示", "文件 " + path + " 不存在，无法加载", "确定");
            return false;
        }

        FileStream rfs = new FileStream(path, FileMode.Open);
        StreamReader sr = new StreamReader(rfs, Encoding.GetEncoding("GB2312"));

        mDic.Clear();

        sr.ReadLine();
        sr.ReadLine();//skip

        while (!sr.EndOfStream)
        {
            string curLine = sr.ReadLine();
            string[] values = curLine.Split('\t');

            TriggerAreaData data = new TriggerAreaData
            {
                Id = int.Parse(values[0]),
                SceneId = int.Parse(values[1]),
                X = float.Parse(values[2]),
                Y = float.Parse(values[3]),
                R = float.Parse(values[4])
            };

            if (!mDic.ContainsKey(data.SceneId))
            {
                mDic[data.SceneId] = new List<TriggerAreaData>();
            }

            mDic[data.SceneId].Add(data);

        }
        rfs.Close();
        sr.Close();


        return true;
    }

    public void Load(int sceneId)
    {
        if (!LoadTableToCache())
        {
            return;
        }

        if (!mDic.ContainsKey(sceneId))
        {
            mDic[sceneId] = new List<TriggerAreaData>();
        }

        var objs = GameObject.FindObjectsOfType<TriggerArea>();
        {
            var __array1 = objs;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var obj = __array1[__i1];
                {
                    GameObject.DestroyImmediate(obj.gameObject);
                }
            }
        }
        {
            var __list2 = mDic[sceneId];
            var __listCount2 = __list2.Count;
            for (int __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var data = __list2[__i2];
                {
                    CreateTriggerArea(data.Id, data.X, data.Y, data.R);
                }
            }
        }
        string path = Application.dataPath + TriggerAreaTableFilePath;
        Debug.Log("加载文件[" + path + "]成功");
    }

    public void Save()
    {
        if (!LoadTableToCache())
        {
            return;
        }

        if (!mDic.ContainsKey(SceneId.intValue))
        {
            mDic[SceneId.intValue] = new List<TriggerAreaData>();
        }
        mDic[SceneId.intValue].Clear();

        var objs = GameObject.FindObjectsOfType<TriggerArea>();
        {
            var __array3 = objs;
            var __arrayLength3 = __array3.Length;
            for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
            {
                var obj = __array3[__i3];
                {
                    TriggerAreaData data = new TriggerAreaData
                    {
                        Id = obj.Id,
                        SceneId = obj.SceneId,
                        X = obj.gameObject.transform.position.x,
                        Y = obj.gameObject.transform.position.z,
                        R = obj.Radius
                    };
                    mDic[SceneId.intValue].Add(data);
                }
            }
        }

        SortedDictionary<int, TriggerAreaData> dict = new SortedDictionary<int, TriggerAreaData>();
        {
            // foreach(var pair in mDic)
            var __enumerator4 = (mDic).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var pair = __enumerator4.Current;
                {
                    {
                        var __list6 = pair.Value;
                        var __listCount6 = __list6.Count;
                        for (int __i6 = 0; __i6 < __listCount6; ++__i6)
                        {
                            var area = __list6[__i6];
                            {
                                if (dict.ContainsKey(area.Id))
                                {
                                    string str = string.Format("Id{0}重复", area.Id);
                                    EditorUtility.DisplayDialog("提示", str, "确定");
                                    return;
                                }
                                dict.Add(area.Id, area);
                            }
                        }
                    }
                }
            }
        }

        //SortedDictionary<>;
        string path = Application.dataPath + TriggerAreaTableFilePath;

        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream wfs = new FileStream(path, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(wfs, Encoding.GetEncoding("GB2312"));
        sw.WriteLine("INT\tINT\tFLOAT\tFLOAT\tFLOAT");
        sw.WriteLine("索引ID\t场景ID\tX坐标\tZ坐标\t半径");
        {
            // foreach(var data in dict)
            var __enumerator5 = (dict).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var data = __enumerator5.Current;
                {
                    sw.WriteLine(data.Value.GetString());
                }
            }
        }

        sw.Close();
        wfs.Close();

        EditorUtility.DisplayDialog("提示", "保存文件成功[" + path + "]", "确定");
    }


}