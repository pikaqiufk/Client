using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public class BindCodeGenerator : ScriptableWizard
{
    public string ClassName;

    [MenuItem("Tools/BindCodeGenerator")]
    static void CodeGenerator()
    {
        DisplayWizard<BindCodeGenerator>("BindCodeGenerator", "Generator", "cancel");
    }

    void OnWizardUpdate()
    {
        
        helpString = "enter class name  Generator Control and Logic.";

        if (string.IsNullOrEmpty(ClassName))
        {
            errorString = "Please enter Bind class name";
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
        GenerateCode();
    }

    void OnWizardOtherButton()
    {
       Close();
    }

    void GenerateCode()
    {
        string outDictory = Application.dataPath + "/Script/UI/Logic/" + ClassName;
        string controllerDictory = Application.dataPath + "/Script/UI/Controller/" + ClassName;
        if (!Directory.Exists(outDictory))
        {
            Directory.CreateDirectory(outDictory);
        }

        string controllerPath = controllerDictory + "Controller.cs";
        string logicPath = Path.Combine(outDictory, ClassName) + "Logic.cs";

        if (File.Exists(controllerPath))
        {
            EditorUtility.DisplayDialog("error", controllerPath + " already exists!", "OK");
            return;
        }
        else
        {
            GetDataFromTemplate(controllerPath, "Controller");
        }

        if (File.Exists(logicPath))
        {
            EditorUtility.DisplayDialog("error", logicPath + " already exists!", "OK");
            return;
        }
        else
        {
            GetDataFromTemplate(logicPath, "Logic");
        }

        AssetDatabase.Refresh();
        Debug.Log( ClassName +" Finished!");
    }

    void GetDataFromTemplate(string path,string TemplateName)
    {
		string directory = Path.Combine(Application.dataPath, "Editor/ScorpionToolsMenu/BindCodeGenerator");
        string LogicTempPath = Path.Combine(directory, "Logic.Template");
        string controllerTempPath = Path.Combine(directory, "Controller.Template");

        if (!File.Exists(LogicTempPath))
        {
            Debug.LogError(LogicTempPath + "not exists!");
        }


        if (!File.Exists(controllerTempPath))
        {
            Debug.LogError(controllerTempPath + "not exists!");
        }


        StreamReader stream = null;
        var classname = ClassName;
        if (TemplateName.Equals("Logic"))
        {
            stream = File.OpenText(LogicTempPath);
            classname += "Logic";

        }
        else if (TemplateName.Equals("Controller"))
        {
            stream = File.OpenText(controllerTempPath);
            classname += "Controller";
        }


        var code = stream.ReadToEnd();
        var outPut = code.Replace("##CLASSNAME##", classname);
        stream.Close();


        File.WriteAllText(path, outPut, Encoding.UTF8);
    }

}
