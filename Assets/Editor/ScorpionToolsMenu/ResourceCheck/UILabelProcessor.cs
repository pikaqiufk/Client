using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DataTable;
using System.Text.RegularExpressions;
using System.Text;
using GameUI;

//处理UILabel
public class UILabelProcessor
{
	private const int DictionaryIDBegin = 100000000;

	private static List<UILabel> FindUILabel()
	{
		var list = new List<UILabel>();

		var cs = EnumAssets.EnumComponentRecursiveInCurrentSelection<UILabel>();

		int i = 0;
		int processed = 0;
		int count = cs.Count();
		{
			// foreach(var go in gos)
			var __enumerator2 = (cs).GetEnumerator();
			while (__enumerator2.MoveNext())
			{
				var c = __enumerator2.Current;
				{
					EditorUtility.DisplayProgressBar("Find Unprocessed UILabel In Selection", c.gameObject.name, i*1.0f/count);
					i++;

					if (!NeedProcess(c))
					{
						continue;
					}

					list.Add(c);
				}
			}
		}


		EditorUtility.ClearProgressBar();

		return list;
	}

	private static bool NeedProcess(UILabel label)
	{
		if (-2 == label.DictionaryId)
		{
			return false;
		}

		if (label.DictionaryId>-1)
 		{
		//  			if (null == Table.GetDictionary(label.DictionaryId))
		//  			{
		//  				label.DictionaryId = -1;
		//  				return true;
		//  			}
 
 			return false;
 
 		}
		
		{
			if (null != label.GetComponent<PropertyDisplayA>())
			{
				return false;
			}
			if (null != label.GetComponent<PropertyDisplayB>())
			{
				return false;
			}
			if (null != label.GetComponent<PropertyDisplayC>())
			{
				return false;
			}
			if (null != label.GetComponent<PropertyDisplayD>())
			{
				return false;
			}
			if (null != label.GetComponent<PropertyDisplayE>())
			{
				return false;
			}
			if (null != label.GetComponent<PropertyDisplayF>())
			{
				return false;
			}

			var parent = label.transform.parent;
			if (null != parent)
			{
				if (null != parent.GetComponent<PropertyDisplayA>())
				{
					return false;
				}
				if (null != parent.GetComponent<PropertyDisplayB>())
				{
					return false;
				}
				if (null != parent.GetComponent<PropertyDisplayC>())
				{
					return false;
				}
				if (null != parent.GetComponent<PropertyDisplayD>())
				{
					return false;
				}
				if (null != parent.GetComponent<PropertyDisplayE>())
				{
					return false;
				}
				if (null != parent.GetComponent<PropertyDisplayF>())
				{
					return false;
				}
			}
		}

		if (string.IsNullOrEmpty(label.text))
		{
			return false;
		}

		var cbs = label.gameObject.GetComponents<UIClassBinding>();

		if (null != cbs && cbs.Count() > 0)
		{
			foreach (var cb in cbs)
			{
				foreach (var bd in cb.BindingDatas)
				{
					if (null != bd.Target)
					{
						var thislabel = bd.Target.target as UILabel;
						if (null != thislabel)
						{
							if (bd.Target.name == "text")
							{
								return false;
							}
						}
					}

				}

			}
		}

		if (!HasCHZN(label.text))
		{
			return false;
		}

		if (null != label.GetComponent<TimerLogic>())
		{
			return false;
		}

		return true;

	}

	[MenuItem("NGUI/Tools/Find Unprocessed UILabel In (Selection)", true)]
	private static bool CanFindUnprocessedUILabel()
	{
		return null != Selection.activeObject;
	}

	[MenuItem("NGUI/Tools/Find Unprocessed UILabel In (Selection)")]
	public static void FindUnprocessedUILabel()
	{
		string str = "";
		var list = FindUILabel();
		foreach (var item in list)
		{
			if (NeedProcess(item))
			{
				str += item.transform.FullPath() + "\t" + item.text + "\n";	
			}
		}

		if (!string.IsNullOrEmpty(str))
		{
			Debug.Log(str);
		}

		Debug.Log("UILabel Total=[" + list.Count.ToString() + "]---------------end");
	}

	private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");

	private static bool HasCHZN(string inputData)
	{
		Match m = RegCHZN.Match(inputData);
		return m.Success;
	}

	private static string ProcessString(string str)
	{
		var temp = Regex.Replace(str, @"\r", "");
		return Regex.Replace(temp, "\n", "\\n");
	}

	[MenuItem("NGUI/Tools/Process UILabel Dictionary")]
	public static bool ProcessUILabel()
	{
		Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();

		Table.ForeachDictionary((table) =>
		{
			var listStr = new List<string>();
			listStr.Add("");
			listStr.Add(table.Desc[0]);
			listStr.Add(table.Desc[1]);
			dict.Add(table.Id, listStr);
			return true;
		});

		Dictionary<int, List<string>> temp = new Dictionary<int, List<string>>();

		int id = DictionaryIDBegin;

		string str = "";
		string ignoreLog = "ignore list\n";

		var guids = AssetDatabase.FindAssets("t:GameObject", new string[] {"Assets/Res/UI"});

		int count = 0;
		foreach (var guid in guids)
		{
			var p = AssetDatabase.GUIDToAssetPath(guid);
			var go = AssetDatabase.LoadAssetAtPath(p, typeof (GameObject)) as GameObject;
			var list = go.GetComponentsInChildren<UILabel>(true);

			var ignoreList = new List<UILabel>();
			var monoList = go.GetComponentsInChildren<MonoBehaviour>(true);
			foreach (var mono in monoList)
			{
				if (null == mono)
				{
					continue;
				}
				var fields = mono.GetType().GetFields();
				foreach (var field in fields)
				{
					var label = field.GetValue(mono) as UILabel;
					if (null != label)
					{
						ignoreList.Add(label);
					}
				}
				
			}
			

			bool needSave = false;
			foreach (var item in list)
			{
				if (!NeedProcess(item))
				{
					continue;
				}

				if (ignoreList.Contains(item))
				{
					ignoreLog += item.transform.FullPath()+"\n";
					continue;
				}

				needSave = true;

				bool processed = false;

				var ret = ProcessString(item.text);
				foreach (var pair in dict)
				{
					var desc = pair.Value[1];

					if (desc.Equals(ret))
					{
						item.DictionaryId = pair.Key;
						break;
					}
				}

				if (-1 != item.DictionaryId)
				{
					continue;
				}

				List<string> listStr = null;
				while (dict.TryGetValue(id, out listStr))
				{
					id++;
				}

				listStr = new List<string>();
				listStr.Add(item.transform.FullPath());
				listStr.Add(ret);
				listStr.Add(ret);
				dict.Add(id, listStr);

				item.DictionaryId = id;

				temp.Add(id, listStr);

				count++;
			}

			if (needSave)
			{
				EditorUtility.SetDirty(go);
			}

		}


		foreach (var pair in temp)
		{
			str += pair.Key + "\t";
			str += pair.Value[0] + "\t";
			str += pair.Value[1] + "\t";
			str += pair.Value[2] + "\n";
		}

		if (temp.Count > 0)
		{
			Debug.Log(str);
		}
		Debug.Log(ignoreLog);

		var path = Application.dataPath + "/UIDictionary.txt";
		FileStream wfs = new FileStream(path, FileMode.Truncate);
		StreamWriter sw = new StreamWriter(wfs, Encoding.GetEncoding("GB2312"));
		sw.Write(str);
		sw.Close();
		wfs.Close();

		AssetDatabase.Refresh();
		AssetDatabase.SaveAssets();

		EditorUtility.DisplayDialog("SUCCESS", "path:" + path, "OK");

		return true;
	}

}
