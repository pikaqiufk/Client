using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UnityEditor.XCodeEditor
{
    public partial class XCPlist
    {

        private string filePath;
        List<string> contents = new List<string>();
        public XCPlist(string Path)
        {
            filePath = Path;
            if (!System.IO.File.Exists(filePath))
            {
                Logger.Error(filePath + "路径下文件不存在");
                return;
            }

            FileInfo projectFileInfo = new FileInfo(filePath);
            StreamReader sr = projectFileInfo.OpenText();
            while (sr.Peek() >= 0)
            {
                contents.Add(sr.ReadLine());
            }
            sr.Close();
            this.plistPath = filePath;
        }
        public void AddKey(string key)
        {
            if (contents.Count < 2)
                return;
            contents.Insert(contents.Count - 2, key);

        }

        public void ReplaceKey(string key, string replace)
        {
            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i].IndexOf(key) != -1)
                {
                    contents[i] = contents[i].Replace(key, replace);
                }
            }
        }

        public void Save()
        {
            StreamWriter saveFile = File.CreateText(filePath);
            {
                var __list1 = contents;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var line = (string)__list1[__i1];
                    saveFile.WriteLine(line);
                }
            }
            saveFile.Close();
        }

    }
}