using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
public class NPCEditor : Editor 
{
    public GameObject EffectObj;

    [MenuItem("Tools/Scorpion/NPCEditor/ExportData")]
    public static  void ExportData()
    {
        GameObject curObj = Selection.activeGameObject;
        if (null == curObj)
        {
            Debug.LogError("pleas choose the npcEditor root");
            return;
        }
        NPCEditorRoot curRoot = curObj.GetComponent<NPCEditorRoot>();
        if (null == curRoot)
        {
            Debug.LogError("the object is not a npcEditor root");
            return;
        }

        string SaveFile = Application.dataPath + "/../../Public/数据编辑器/Config/SceneNpc.txt";
        curRoot.ExportData(SaveFile);
        AssetDatabase.Refresh();
        Debug.Log("保存完毕");

        /*
        string curPath = "";//= BundleManager.GetGUIDataName(target.name);
       
         * */
    }

    [MenuItem("Tools/Scorpion/NPCEditor/ImportData")]
    public static void ImportData()
    {
        GameObject curObj = Selection.activeGameObject;
        if (null == curObj)
        {
            Debug.LogError("pleas choose the npcEditor root");
            return;
        }
        NPCEditorRoot curRoot = curObj.GetComponent<NPCEditorRoot>();
        if (null == curRoot)
        {
            Debug.LogError("the object is not a npcEditor root");
            return;
        }
        string curPath = Application.dataPath + "/../../Public/数据编辑器/Config/SceneNpc.txt";
        if (!File.Exists(curPath))
        {
            EditorUtility.DisplayDialog("提示", "文件 " + curPath + " 不存在，无法加载", "确定");
        }
        else
        {
            curRoot.ImportData(curPath);
        }
    }
	
}
#endif
