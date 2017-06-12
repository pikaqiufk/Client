using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;

//处理UILabel
public class EmptyUpdateCleaner
{

	[MenuItem("Tools/Log Empty Update")]
	public static void Test()
	{
		/*
		string log = "";
		var module = Mono.Cecil.ModuleDefinition.ReadModule(Application.dataPath+ "/../Temp/UnityVS_bin/Debug/Assembly-CSharp.dll");
		foreach (var type in module.Types)
		{
			if (null==type.BaseType)
			{
				continue;
			}

			if (!type.BaseType.Name.Contains("MonoBehaviour"))
			{
				continue;
			}

			foreach (var method in type.Methods)
			{
				if (method.Name.Equals("Update"))
				{
					if (method.Body.Instructions.Count <= 2)
					{
						log += type.Name + "."+method.Name + "\n";						
					}
				}
				else if (method.Name.Equals("LateUpdate"))
				{
					if (method.Body.Instructions.Count <= 2)
					{
						log += type.Name + "." + method.Name + "\n";
					}
				}
				else if (method.Name.Equals("FixedUpdate"))
				{
					if (method.Body.Instructions.Count <= 2)
					{
						log += type.Name + "." + method.Name + "\n";
					}
				}
			}
		}
		Debug.Log(log);
	
*/

	}
}
