using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FindAtlasSpriteReference : ScriptableWizard
{
    public UIAtlas Atlas;
	public string SpriteName;
    // Use this for initialization
    void Start()
    {

    }

	[MenuItem("Assets/Find Sprite Reference")]
    public static void OpenDialog()
    {
		DisplayWizard<FindAtlasSpriteReference>("Find Sprite", "Find", "Cancel");
    }

    void OnWizardCreate()
    {
        Find1();
    }
    void OnWizardOtherButton()
    {
        Close();
    }


    public void Find1()
	{
		if (null==Atlas)
		{
			return;
		}
		if (string.IsNullOrEmpty(SpriteName))
        {
            return;
        }
		var temp = SpriteName.ToLower();

        var cs = EnumAssets.EnumComponentRecursiveInCurrentSelection<UISprite>();
        EditorUtility.DisplayProgressBar("FindAtlasReference", "Collecting Component", 0);

        int count = cs.Count();
        int i = 0;
        int n = 0;
        string log = string.Empty;
        {
            // foreach(var c in cs)
            var __enumerator1 = (cs).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var c = __enumerator1.Current;
                {
                    i++;
					EditorUtility.DisplayProgressBar(Atlas.name, c.gameObject.name, i * 1.0f / count);
                    var atlas = c.atlas;
					if (Atlas != atlas)
                    {
                        continue;
                    }
					var spriteName = c.spriteName.ToLower();
					if (spriteName.Contains(temp))
                    {
                        n++;
                        log += "(" + atlas.name + "/" + c.spriteName + ")   " + c.transform.FullPath() + "\n";
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(log))
        {
            Debug.Log(log);
        }

        //Debug.Log("Using [" + AtlasName + "] Total=" + n.ToString() + "------------------------------------------end");

    }

}
