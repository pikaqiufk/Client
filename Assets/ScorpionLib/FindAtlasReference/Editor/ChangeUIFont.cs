using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ChangeUIFont : ScriptableWizard
{
    [Tooltip("Font")]
    public Object TargetFont;

    // Use this for initialization
    void Start()
    {

    }

    [MenuItem("Tools/Change UI Font In Current Select")]
    public static void OpenDialog()
    {
        DisplayWizard<ChangeUIFont>("Change UI Font", "Ok", "Cancel");
    }

    void OnWizardCreate()
    {
        Do();
    }
    void OnWizardOtherButton()
    {
        Close();
    }


    public void Do()
    {
        if (null == TargetFont)
        {
            return;
        }
        var font = TargetFont as Font;
        if (null == font)
        {
            Logger.Debug("TargetFont is not fount");
            return;
        }

        var cs = EnumAssets.EnumComponentRecursiveInCurrentSelection<UIFont>();
        EditorUtility.DisplayProgressBar("FindAtlasReference", "Collecting Component", 0);

        int count = cs.Count();
        int i = 0;
        string log = string.Empty;
        {
            // foreach(var c in cs)
            var __enumerator1 = (cs).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var c = __enumerator1.Current;
                {
                    if (null == c.dynamicFont)
                    {
                        continue;
                    }
                    c.dynamicFont = font;

                    i++;
                    EditorUtility.DisplayProgressBar(font.name, c.gameObject.name, i * 1.0f / count);
                }
            }
        }

        if (!string.IsNullOrEmpty(log))
        {
            Debug.Log(log);
        }

        Debug.Log("Using [" + font.name + "] Total=" + i.ToString() + "------------------------------------------end");

        if (i > 0)
        {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }


}
