using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FindAtlasReference : ScriptableWizard
{
    [Tooltip("Atlas Name")]
    public string AtlasName;

    // Use this for initialization
    void Start()
    {

    }

	[MenuItem("Assets/Find Atlas Reference")]
    public static void OpenDialog()
    {
        DisplayWizard<FindAtlasReference>("Find object using this atlas", "Find", "Cancel");
    }

    void OnWizardCreate()
    {
        Find1();
        Find2();
	    Find3();
    }
    void OnWizardOtherButton()
    {
        Close();
    }


    public void Find1()
    {
        if (string.IsNullOrEmpty(AtlasName))
        {
            return;
        }
        var temp = AtlasName.ToLower();

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
                    EditorUtility.DisplayProgressBar(AtlasName, c.gameObject.name, i * 1.0f / count);
                    var atlas = c.atlas;
                    if (null == atlas)
                    {
                        continue;
                    }
                    var atlasName = atlas.name.ToLower();
                    if (atlasName.Contains(temp))
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

    public void Find2()
    {
        if (string.IsNullOrEmpty(AtlasName))
        {
            return;
        }
        var temp = AtlasName.ToLower();

        var cs = EnumAssets.EnumComponentRecursiveInCurrentSelection<Particle2D>();
        EditorUtility.DisplayProgressBar("FindAtlasReference", "Collecting Component", 0);

        int count = cs.Count();
        int i = 0;
        int n = 0;
        string log = string.Empty;
        {
            // foreach(var c in cs)
            var __enumerator3 = (cs).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var c = __enumerator3.Current;
                {
                    i++;
                    EditorUtility.DisplayProgressBar(AtlasName, c.gameObject.name, i * 1.0f / count);
                    var atlas = c.atlas;
                    if (null == atlas)
                    {
                        continue;
                    }
                    var atlasName = atlas.name.ToLower();
                    if (atlasName.Contains(temp))
                    {
                        n++;
                        foreach (var str in c.sprites)
                        {
                            log += "(" + atlas.name + "/" + str + ")   " + c.transform.FullPath() + "\n";
                        }

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

	public void Find3()
	{
		if (string.IsNullOrEmpty(AtlasName))
		{
			return;
		}
		var temp = AtlasName.ToLower();

		var cs = EnumAssets.EnumComponentRecursiveInCurrentSelection<UILabel>();
		EditorUtility.DisplayProgressBar("FindAtlasReference", "Collecting Component", 0);

		int count = cs.Count();
		int i = 0;
		int n = 0;
		string log = string.Empty;
		{
			// foreach(var c in cs)
			var __enumerator3 = (cs).GetEnumerator();
			while (__enumerator3.MoveNext())
			{
				var c = __enumerator3.Current;
				{
					i++;
					EditorUtility.DisplayProgressBar(AtlasName, c.gameObject.name, i * 1.0f / count);
					var font = c.font;
					if (null == font)
					{
						continue;
					}
					var atlas = font.atlas;
					if (null == atlas)
					{
						continue;
					}
					var atlasName = font.atlas.name.ToLower();
					if (atlasName.Contains(temp))
					{
						n++;
						
						log += "(" + atlas.name  + ")   " + c.transform.FullPath() + "\n";
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
