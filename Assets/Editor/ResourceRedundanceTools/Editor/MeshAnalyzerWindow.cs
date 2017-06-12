using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class MeshAnalyzerOption
{
    List<ModelInfo> modelinfoList = new List<ModelInfo>();
    string currentSelection = null;
    private int mAssetPathStartIndex = Application.dataPath.IndexOf("Assets");

    Vector2 meshScrollPosition;
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Collect All Meshes", GUILayout.Width(200)))
        {
            modelinfoList.Clear();
            collectModels();
            currentClip = new int[modelinfoList.Count];
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        showMeshTableHeadLine();

        meshScrollPosition = GUILayout.BeginScrollView(meshScrollPosition);
        showModelInfos();
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width - 150);
        if (GUILayout.Button("exportInfo"))
        {
            ExportModelInfo();
        }
        GUILayout.EndHorizontal();
    }

    private bool ExportModelInfo()
    {
        string path = EditorUtility.SaveFilePanel("Save Model Info", "", "ModelInfo.txt", "txt");
        if (path.Length == 0)
        {
            Debug.LogWarning("Save Mesh File Failed!");
            return false;
        }
        string meshInfoBufferStr = "model Name\tmeshFilter Count\tskinnedMesh Count\tvertex Count\ttriangle Count\t animationClip Count\t animationClip\t related Bones Count";
        {
            var __list1 = modelinfoList;
            var __listCount1 = __list1.Count;
            for (int __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var modelinfo = (ModelInfo)__list1[__i1];
                {
                    {
                        var __list8 = modelinfo.clipsList;
                        var __listCount8 = __list8.Count;
                        for (int __i8 = 0; __i8 < __listCount8; ++__i8)
                        {
                            var clip = (AnimationClipInfo)__list8[__i8];
                            {
                                meshInfoBufferStr += "\n";

                                meshInfoBufferStr += modelinfo.name;
                                meshInfoBufferStr += "\t";

                                meshInfoBufferStr += modelinfo.meshFilterCount;
                                meshInfoBufferStr += "\t";

                                meshInfoBufferStr += modelinfo.skinnedMeshCount;
                                meshInfoBufferStr += "\t";

                                meshInfoBufferStr += modelinfo.vertexCount;
                                meshInfoBufferStr += "\t";

                                meshInfoBufferStr += modelinfo.triangleCount;
                                meshInfoBufferStr += "\t";

                                meshInfoBufferStr += modelinfo.animationClipCount;
                                meshInfoBufferStr += "\t";

                                meshInfoBufferStr += ("\"" + clip.name + "\" " + clip.length + "s, " + clip.frameRate + "FPS, " + clip.wrapMode + ",\t" + clip.relatedBoneCount);
                                meshInfoBufferStr += "\n";
                            }
                        }
                    }
                }
            }
        }
        byte[] meshInfoBytes = System.Text.Encoding.UTF8.GetBytes(meshInfoBufferStr);
        try
        {
            File.WriteAllBytes(path, meshInfoBytes);
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
            return false;
        }
        Debug.LogWarning("Save Mesh File Success!");
        return true;
    }
    private void showMeshTableHeadLine()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("model\nName", GUILayout.Width(200)))
        {
            modelinfoList.Sort(ModelInfo.SortByName);
        }
        GUILayout.Button("meshFilter\nCount", GUILayout.Width(100));
        if (GUILayout.Button("skinnedMesh\nCount", GUILayout.Width(100)))
        {
            modelinfoList.Sort(ModelInfo.SortBySkinnedMeshCount);
        }
        if (GUILayout.Button("vertex\nCount", GUILayout.Width(100)))
        {
            modelinfoList.Sort(ModelInfo.SortByVertexCount);
        }
        if (GUILayout.Button("triangle\nCount", GUILayout.Width(100)))
        {
            modelinfoList.Sort(ModelInfo.SortByTriangleCount);
        }

        if (GUILayout.Button("animationClip\nCount", GUILayout.Width(100)))
        {
            modelinfoList.Sort(ModelInfo.SortByAnimationClipCount);
        }
        GUILayout.Button("animationClip \nInfo", GUILayout.Width(300));
        GUILayout.EndHorizontal();
    }

    int[] currentClip;
    private void showModelInfos()
    {
        int i = 0;
        {
            var __list2 = modelinfoList;
            var __listCount2 = __list2.Count;
            for (int __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var modelinfo = (ModelInfo)__list2[__i2];
                {
                    GUI.color = currentSelection == (modelinfo.assetPath) ? new Color(1, 0.5f, 1) : Color.white;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(modelinfo.name, GUILayout.Width(200)))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(modelinfo.assetPath, typeof(Object));
                        currentSelection = modelinfo.assetPath;
                    }

                    GUILayout.Button("  " + modelinfo.meshFilterCount.ToString(), EditorStyles.label, GUILayout.Width(100));

                    GUILayout.Button("  " + modelinfo.skinnedMeshCount.ToString(), EditorStyles.label, GUILayout.Width(100));

                    GUILayout.Button("  " + modelinfo.vertexCount.ToString(), EditorStyles.label, GUILayout.Width(100));

                    GUILayout.Button("  " + modelinfo.triangleCount.ToString(), EditorStyles.label, GUILayout.Width(100));

                    showAnimationInfo(modelinfo, i);
                    i++;

                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    private void showAnimationInfo(ModelInfo modelinfo, int i)
    {
        GUILayout.Button("  " + modelinfo.animationClipCount.ToString(), EditorStyles.label, GUILayout.Width(100));
        string[] clipnameOption = modelinfo.getClipNames();
        int[] clipIndexOption = ModelUtility.getIntArray(modelinfo.animationClipCount);
        if (clipnameOption.Length > 0)
        {
            currentClip[i] = EditorGUILayout.IntPopup(currentClip[i], clipnameOption, clipIndexOption, GUILayout.Width(100));
            AnimationClipInfo clipInfo = modelinfo.clipsList[currentClip[i]];
            GUILayout.Label(clipInfo.length.ToString("00.00") + "s  " + clipInfo.frameRate + "FPS  " + clipInfo.wrapMode + " " + clipInfo.relatedBoneCount + " Bones", EditorStyles.label, GUILayout.Width(200));
        }
    }
    private void collectModels()
    {
        DirectoryInfo rootDir = new DirectoryInfo(Application.dataPath);
        FileInfo[] allFiles = rootDir.GetFiles("*.fbx", SearchOption.AllDirectories);
        {
            var __array3 = allFiles;
            var __arrayLength3 = __array3.Length;
            for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
            {
                var fi = (FileInfo)__array3[__i3];
                {
                    string fullName = fi.FullName.Replace('\\', '/');
                    fullName = fullName.Remove(0, mAssetPathStartIndex);

                    Object obj = AssetDatabase.LoadAssetAtPath(fullName, typeof(Object));
                    GameObject go = obj as GameObject;
                    if (go == null)
                    {
                        continue;
                    }
                    modelinfoList.Add(new ModelInfo(go, fullName));
                }
            }
        }
        //string [ ] assets = AssetDatabase.FindAssets( "t:Model", null );
        //foreach ( string ob in assets )
        //{
        //    string assetpath = AssetDatabase.GUIDToAssetPath( ob );
        //    Object obj = AssetDatabase.LoadAssetAtPath( assetpath, typeof( Object ) );
        //    GameObject go = obj as GameObject;
        //    if ( go == null )
        //    {
        //        continue;
        //    }
        //    modelinfoList.Add( new ModelInfo( go, assetpath ) );
        //}
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}

public class ModelUtility
{
    public static List<SkinnedMeshRenderer> findAllSkinnedMesh(GameObject modelObject)
    {
        List<SkinnedMeshRenderer> skinnedMeshes = new List<SkinnedMeshRenderer>();
        SkinnedMeshRenderer skin = null;

        skin = modelObject.GetComponent<SkinnedMeshRenderer>();
        if (skin != null)
        {
            skinnedMeshes.Add(skin);
        }
        for (int i = 0; i < modelObject.transform.childCount; i++)
        {
            skin = modelObject.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            if (skin != null)
            {
                skinnedMeshes.Add(skin);
            }
        }
        return skinnedMeshes;
    }

    public static List<MeshFilter> findAllMeshFilter(GameObject modelObject)
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        MeshFilter filter = null;

        filter = modelObject.GetComponent<MeshFilter>();
        if (filter != null)
        {
            meshFilters.Add(filter);
        }
        for (int i = 0; i < modelObject.transform.childCount; i++)
        {
            filter = modelObject.transform.GetChild(i).GetComponent<MeshFilter>();
            if (filter != null)
            {
                meshFilters.Add(filter);
            }
        }
        return meshFilters;
    }

    public static int[] getIntArray(int num)
    {
        int[] array = new int[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = i;
        }
        return array;
    }
}

public class AnimationClipInfo
{
    public string name;
    public float length; //动画片段的长度，以秒为单位
    public WrapMode wrapMode;
    public float frameRate;
    public int relatedBoneCount;
    public AnimationClipInfo(AnimationClip aclip)
    {
        name = aclip.name;
        length = aclip.length;
        wrapMode = aclip.wrapMode;
        frameRate = aclip.frameRate;
        AnimationClipCurveData[] accds = AnimationUtility.GetAllCurves(aclip, false);
        string curveDataStr = name + "\n";
        List<string> relatedPathList = new List<string>();
        {
            var __array4 = accds;
            var __arrayLength4 = __array4.Length;
            for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
            {
                var accd = (AnimationClipCurveData)__array4[__i4];
                {
                    if (relatedPathList.Contains(accd.path))
                    {
                        continue;
                    }
                    relatedPathList.Add(accd.path);
                }
            }
        }
        relatedBoneCount = relatedPathList.Count;
    }
}
public class ModelInfo
{
    public string name;
    public string assetPath;
    public int skinnedMeshCount;
    public int meshFilterCount;
    public int animationClipCount;
    public int vertexCount;
    public int triangleCount;
    public List<AnimationClipInfo> clipsList = new List<AnimationClipInfo>();
    public ModelInfo(GameObject obj, string assetpath)
    {
        GameObject modelObject = obj;
        name = obj.name;
        assetPath = assetpath;
        collectAnimationInfo(obj);
        collectMeshInfo(obj);
        modelObject = null;
    }

    public string[] getClipNames()
    {
        string[] names = new string[clipsList.Count];
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = clipsList[i].name;
        }
        return names;
    }
    private void collectAnimationInfo(GameObject modelObject)
    {
        Animation legacyAnimation = null;
        legacyAnimation = modelObject.GetComponent<Animation>();
        if (legacyAnimation == null)
        {
            return;
        }
        animationClipCount = legacyAnimation.GetClipCount();
        {
            // foreach(var state in legacyAnimation)
            var __enumerator5 = (legacyAnimation).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var state = (AnimationState)__enumerator5.Current;
                {
                    AnimationClipInfo clip = new AnimationClipInfo(state.clip);
                    clipsList.Add(clip);
                }
            }
        }
    }
    private void collectMeshInfo(GameObject modelObject)
    {

        List<SkinnedMeshRenderer> _skinnedMeshes = ModelUtility.findAllSkinnedMesh(modelObject);
        List<MeshFilter> _meshFilters = ModelUtility.findAllMeshFilter(modelObject);
        {
            var __list6 = _skinnedMeshes;
            var __listCount6 = __list6.Count;
            for (int __i6 = 0; __i6 < __listCount6; ++__i6)
            {
                var _skin = (SkinnedMeshRenderer)__list6[__i6];
                {
                    vertexCount += _skin.sharedMesh.vertexCount;
                    triangleCount += _skin.sharedMesh.triangles.Length / 3;
                    //string bonsStr ="";
                    //for(int i = 0; i < _skin.bones.Length; i++){
                    //    bonsStr+=_skin.bones[i].name+" ";
                    //}
                    //Debug.Log(_skin.bones.Length + " " + bonsStr);
                }
            }
        }
        {
            var __list7 = _meshFilters;
            var __listCount7 = __list7.Count;
            for (int __i7 = 0; __i7 < __listCount7; ++__i7)
            {
                var _filter = (MeshFilter)__list7[__i7];
                {
                    vertexCount += _filter.sharedMesh.vertexCount;
                    triangleCount += _filter.sharedMesh.triangles.Length / 3;
                }
            }
        }
        skinnedMeshCount = _skinnedMeshes.Count;
        meshFilterCount = _meshFilters.Count;
    }
    public static int SortByName(ModelInfo a, ModelInfo b)
    {
        return string.Compare(a.name, b.name);
    }
    public static int SortByVertexCount(ModelInfo a, ModelInfo b)
    {
        if (a.vertexCount > b.vertexCount)
        {
            return -1;
        }
        else if (a.vertexCount < b.vertexCount)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public static int SortByTriangleCount(ModelInfo a, ModelInfo b)
    {
        if (a.triangleCount > b.triangleCount)
        {
            return -1;
        }
        else if (a.triangleCount < b.triangleCount)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public static int SortBySkinnedMeshCount(ModelInfo a, ModelInfo b)
    {
        if (a.skinnedMeshCount > b.skinnedMeshCount)
        {
            return -1;
        }
        else if (a.skinnedMeshCount < b.skinnedMeshCount)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public static int SortByAnimationClipCount(ModelInfo a, ModelInfo b)
    {
        if (a.animationClipCount > b.animationClipCount)
        {
            return -1;
        }
        else if (a.animationClipCount < b.animationClipCount)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
