using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

public class EventRegistChecker {

    [MenuItem("Tools/CheckEventRegister")]
    public static void CheckEventRegister()
    {
        var files = Directory.GetFiles(Application.dataPath + "/Script", "*.cs", SearchOption.AllDirectories);

        HashSet<string> set = new HashSet<string>();

        foreach (var file in files)
        {
            if (file.EndsWith("Controller.cs"))
                continue;

            set.Clear();
            string content = File.ReadAllText(file);
            var matches = Regex.Matches(content, @"EventDispatcher\.Instance\.AddEventListener\((.+),(.+)\)");
            foreach (Match match in matches)
            {
                //Logger.Info("{0}\t\t\t{1}", match.ToString(), match.Groups[1].Value);
                if (!set.Add(match.Groups[1].Value.Trim() + ":" + match.Groups[2].Value.Trim()))
                {
                    Logger.Error("{0} 被重复添加了，文件{1}", match.ToString(), file); 
                }
            }

            matches = Regex.Matches(content, @"EventDispatcher\.Instance\.RemoveEventListener\((.+),(.+)\)");
            foreach (Match match in matches)
            {
                //Logger.Info("{0}\t\t\t{1}", match.ToString(), match.Groups[1].Value);
                if (!set.Remove(match.Groups[1].Value.Trim() + ":" + match.Groups[2].Value.Trim()))
                {
                    Logger.Error("{0} 被错误删除了，文件{1}", match.ToString(), file);
                }
            }

            foreach (var str in set)
            {
                Logger.Error("{0} 只被注册了，没有被删除，文件 {1}", str, file);
            }
        }
    }

}
