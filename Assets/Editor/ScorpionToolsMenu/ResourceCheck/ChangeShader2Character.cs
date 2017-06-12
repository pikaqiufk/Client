using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;

public class ChangeShader2Character
{
    [MenuItem("Tools/Shader/Change Shader From CharacterThrough To Character")]
    //优化资源
    public static void ChangeNPCShader()
    {
		var shaderId = Shader.Find("Scorpion/CharacterThrough").GetInstanceID();
		var shader = Shader.Find("Scorpion/Character");

        var cs = EnumAssets.EnumComponentRecursiveInCurrentSelection<Renderer>();

        int count = cs.Count();
        int i = 0;
        {
            // foreach(var c in cs)
            var __enumerator2 = (cs).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var c = __enumerator2.Current;
                {
                    foreach (var m in c.sharedMaterials)
                    {
                        if (null != m)
                        {
                            if (m.shader.GetInstanceID() == shaderId)
                            {
                                m.shader = shader;
                            }
                        }
                    }
                    i++;
                    EditorUtility.DisplayProgressBar("Change Shader From CharacterThrough To Character", c.gameObject.name, i * 1.0f / count);
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

}
