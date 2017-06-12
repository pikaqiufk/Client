using System.IO;
using System.Reflection;
using UnityEngine;
using System;
using UnityEditor;
using System.Text;

//将CG带cg打头的函数导出到Warpper
public class ExportCGFunction
{
	[MenuItem("Tools/Export CG Function To Wrapper")]
	public static void ExportCGFunctionToWrapper()
	{
		PlayCG.Instance.Init();

		string logRegister = "";

		string log = "";
		log += "using System;";
		log += "\n";

		log += "using UnityEngine;";
		log += "\n";

		log += "using System.Collections;";
		log += "\n";

		log += "using NCalc;";
		log += "\n";

		log += "\n";
		log += "\n";
		log += "//Auto generate code.Don't edit manually.Use menu [Tools/Export CG Function To Wrapper]";
		log += "\n";
		log += "\n";
		log += "\n";

		log += "public static class CGFunctionWrapper";
		log += "\n";
		log += "{";
		

		Type t = typeof (PlayCG);
		var ms = t.GetMethods();
		for (int i=0; i<ms.Length;i++)
		{
			var m = ms[i];
			var name = m.Name.Substring(0, 2);
			if (name.Equals("cg"))
			{

				logRegister += string.Format("mExecutor.RegisterFunction(\"{0}\", CGFunctionWrapper.{1});\n", m.Name, m.Name);

				log += "public static object ";
				log += m.Name;
				log += "(Expression[] args)";
				log += "\n";
				log += "{";
				log += "\n";

				var pCount = m.GetParameters().Length;

				
				log += string.Format("if (args.Length < {0})\n", pCount);
				log += "{\n";
				log += string.Format("Logger.Error(\"{0}() args<[{1}]\");\n", m.Name, pCount);
				log += "return 0;\n";
				log += "}\n";
				

				log += "PlayCG.Instance.";
				log += m.Name;
				log += "(";
				log += "\n";
				
				for (int j = 0; j < pCount; j++)
				{
					var p = m.GetParameters()[j];
					log += "Convert.To";
					log += p.ParameterType.Name;
					log += "(";
					log += "args[" + j.ToString() + "].Evaluate()";
					log += ")";
					if (j != pCount-1)
					{
						log += ",";
					}
					log += "\n";
				}

				log += ");\n";

				log += "return 1;";
				

				log += "\n";
				log += "}";
			}
		}
		log += "\n";
		log += "}";
		log += "\n";



		var filePath = Application.dataPath + "/../../Client/Assets/Script/CG/CGFunctionWrapper.cs";

		FileStream wfs = new FileStream(filePath, FileMode.OpenOrCreate | FileMode.Truncate);
		StreamWriter sw = new StreamWriter(wfs, Encoding.GetEncoding("GB2312"));
		sw.Write(log);
		sw.Close();
		wfs.Close();

		AssetDatabase.Refresh();

		Debug.Log(logRegister);
		
		
	}
}
