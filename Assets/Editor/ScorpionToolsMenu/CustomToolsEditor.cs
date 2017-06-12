using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using DataTable;


public class CustomToolsEditor : Editor
{
    [MenuItem("Tools/T4M/Terrain/FixLayer")]
    public static void FixTerrainLayer()
    {
        Object selectObj = Selection.activeGameObject;
        if (selectObj == null || !(selectObj is GameObject))
        {
            Debug.LogError("Choose a GameObject first");
            return;
        }

        GameObject gameObj = selectObj as GameObject;
        gameObj.layer = 30;
    }

    [MenuItem("Tools/T4M/ConvertToT4m")]
    public static void ConvertToT4M()
    {
        Object objSelected = Selection.activeObject;
        if (null == objSelected || !(objSelected is GameObject))
        {
            Debug.LogError("choose a GameObject first");
            return;
        }

        var gameObject = objSelected as GameObject;
        gameObject.layer = 30;

        if (!Directory.Exists(Application.dataPath + "/T4MOBJ"))
        {
            Directory.CreateDirectory(Application.dataPath + "/T4MOBJ");
        }

        if (!Directory.Exists(Application.dataPath + "/T4MOBJ/Obj"))
        {
            Directory.CreateDirectory(Application.dataPath + "/T4MOBJ/Obj");
        }

        if (!Directory.Exists(Application.dataPath + "/T4MOBJ/Obj/Texture"))
        {
            Directory.CreateDirectory(Application.dataPath + "/T4MOBJ/Obj/Texture");
        }

        AssetDatabase.Refresh();

        Material material = null;
        if (!gameObject.renderer.material.name.Contains("T4M"))
        {
            material = new Material(Shader.Find("T4MShaders/ShaderModel2/Diffuse/T4M 4 Textures"));
            AssetDatabase.CreateAsset(material, "Assets/T4MOBJ/Obj/" + gameObject.name + ".mat");
            gameObject.renderer.material = material;
        }

        var t4m = gameObject.GetComponent<T4MObjSC>();
        if (t4m == null)
        {
            t4m = gameObject.AddComponent<T4MObjSC>();
            t4m.T4MMaterial = material;
            t4m.T4MMesh = gameObject.GetComponent<MeshFilter>();
        }

        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            var meshcol = gameObject.AddComponent<MeshCollider>();
            meshcol.mesh = t4m.T4MMesh.mesh;
        }

        Texture2D Control2 = new Texture2D(128, 128, TextureFormat.ARGB32, true);
        Color[] ColorBase = new Color[128 * 128];
        for (var t = 0; t < ColorBase.Length; t++)
        {
            ColorBase[t] = new Color(1, 0, 0, 0);
        }

        Control2.SetPixels(ColorBase);
        string path;
        if (gameObject.GetComponent<T4MObjSC>().T4MMaterial.GetTexture("_Control") != null)
        {
            path = Application.dataPath + "/T4MOBJ/Obj/Texture/" +
                   gameObject.GetComponent<T4MObjSC>().T4MMaterial.GetTexture("_Control").name + "C2.png";

        }
        else path = Application.dataPath + "/T4MOBJ/Obj/Texture/" + gameObject.name + "C2.png";
        byte[] data = Control2.EncodeToPNG();
        File.WriteAllBytes(path, data);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        AssetDatabase.Refresh();

        TextureImporter TextureI =
            AssetImporter.GetAtPath("Assets/T4MOBJ/Obj/Texture/" + gameObject.name + "C2.png") as TextureImporter;
        TextureI.textureFormat = TextureImporterFormat.ARGB32;
        TextureI.isReadable = true;
        TextureI.anisoLevel = 9;
        TextureI.mipmapEnabled = false;
        TextureI.wrapMode = TextureWrapMode.Clamp;

        AssetDatabase.ImportAsset("Assets/T4MOBJ/Obj/Texture/" + gameObject.name + "C2.png",
            ImportAssetOptions.ForceUpdate);

        Texture Contr2 =
            (Texture)
                Resources.LoadAssetAtPath("Assets/T4MOBJ/Obj/Texture/" + gameObject.name + "C2.png", typeof(Texture));
        t4m.T4MMaterial.SetTexture("_Control", Contr2);

        AssetDatabase.SaveAssets();

    }

    [MenuItem("Tools/Scorpion/Shadow/Remove")]
    public static void RemoveShadow()
    {
        Object selectObj = Selection.activeGameObject;
        if (selectObj == null || !(selectObj is GameObject))
        {
            Debug.LogError("Choose a GameObject first");
            return;
        }
        GameObject gameObj = (GameObject)selectObj;
        MeshRenderer[] meshs = gameObj.GetComponentsInChildren<MeshRenderer>();
        {
            var __array1 = meshs;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var renderer = __array1[__i1];
                {
                    renderer.castShadows = false;
                    renderer.receiveShadows = false;
                }
            }
        }
    }
    [MenuItem("Tools/Scorpion/Shadow/Add")]
    public static void AddShadow()
    {
        Object selectObj = Selection.activeGameObject;
        if (selectObj == null || !(selectObj is GameObject))
        {
            Debug.LogError("Choose a GameObject first");
            return;
        }
        GameObject gameObj = (GameObject)selectObj;
        MeshRenderer[] meshs = gameObj.GetComponentsInChildren<MeshRenderer>();
        {
            var __array2 = meshs;
            var __arrayLength2 = __array2.Length;
            for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
            {
                var renderer = __array2[__i2];
                {
                    renderer.castShadows = true;
                    renderer.receiveShadows = true;
                }
            }
        }
    }

    [MenuItem("Tools/Scorpion/MergeMeshesUsingSameMaterial")]
    public static void MergeMeshes()
    {
        var objs = Selection.gameObjects;
        if (objs.Length <= 0)
        {
            Logger.Error("Please select some mesh to merge.");
            return;
        }

        
        // check mesh
        foreach (var go in objs)
        {
            if (!go.GetComponent<MeshFilter>())
            {
                Logger.Error("GameObject {0} do not has mesh", go);
                return;
            }
        }

        // check material
        var mat = objs[0].GetComponent<MeshRenderer>().sharedMaterial;
        foreach (var go in objs)
        {
            if (go.GetComponent<MeshRenderer>().sharedMaterial != mat)
            {
                Logger.Error("Please select meshes using same material. {0} {1}", objs[0], go);
                return;
            }
        }

        List<CombineInstance> combine = new List<CombineInstance>();
        foreach (var go in objs)
        {
            var filter = go.GetComponent<MeshFilter>();
            CombineInstance c = new CombineInstance();
            c.mesh = filter.sharedMesh;
            c.transform = filter.transform.localToWorldMatrix;
            combine.Add(c);
        }

        Mesh m = new Mesh();
        m.CombineMeshes(combine.ToArray());
        var filePath = EditorUtility.SaveFilePanelInProject(
             "Save Mesh in Assets",
              "Mesh" + ".asset",
             "asset", "Please enter a file name to save the Mesh to ");

        if (filePath != "")
        {
            AssetDatabase.CreateAsset(m, filePath);
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Tools/Scorpion/Remove Animator")]
    public static void RemoveAnimator()
    {
        Object selectObj = Selection.activeGameObject;
        if (selectObj == null || !(selectObj is GameObject))
        {
            Debug.LogError("Choose a GameObject first");
            return;
        }
        GameObject gameObj = (GameObject)selectObj;
        Animator[] animators = gameObj.GetComponentsInChildren<Animator>();
        {
            var __array3 = animators;
            var __arrayLength3 = __array3.Length;
            for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
            {
                var animator = __array3[__i3];
                {
                    DestroyImmediate(animator);
                }
            }
        }
    }

    [MenuItem("Tools/Dictionary Reload")]
    public static void ReloadDictionaryTable()
    {
        var ta = Resources.LoadAssetAtPath("Assets/Res/Table/Dictionary.txt", typeof(TextAsset)) as TextAsset;
        TableInit.Table_Init(ta.bytes, Dictionary, TableType.Dictionary);
    }

    [MenuItem("Tools/Check Character")]
    public static void CheckCharacterIco()
    {
        GameObject res = Resources.LoadAssetAtPath("Assets/NotBuild/CharacterIco/CharacterIcos.prefab", typeof(GameObject)) as GameObject;
        GameObject obj = GameObject.Instantiate(res.gameObject) as GameObject;
        var tarns = obj.transform;
        obj.SetActive(true);
        var uiroot = GameObject.FindObjectOfType<UIRoot>();
        tarns.parent = uiroot.transform;
        tarns.localScale = Vector3.one;
        tarns.localPosition = Vector3.zero;

    }

    public static string GetDictionary(int dicId)
    {
        if (Dictionary.Count == 0)
        {
            ReloadDictionaryTable();
        }
        IRecord record;
        if (Dictionary.TryGetValue(dicId, out record))
        {
            return ((DictionaryRecord)record).Desc[0];
        }
        return "Error";
    }

    public static void ReloadSoundTable()
    {
        var res = Resources.LoadAssetAtPath<TextAsset>("Assets/Res/Table/Sound.txt");
        TableInit.Table_Init(res.bytes, SoundTable, TableType.Sound);
    }

    public static string GetSoundPath(int dicId)
    {
        if (SoundTable.Count == 0)
        {
            ReloadSoundTable();
        }

        IRecord record;
        if (SoundTable.TryGetValue(dicId, out record))
        {
            return ((SoundRecord) record).FullPathName;
        }
        return "null";
    }

    public static Dictionary<int, IRecord> SoundTable = new Dictionary<int, IRecord>(); 
    public static Dictionary<int, IRecord> Dictionary = new Dictionary<int, IRecord>();
}