using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class AnalyzeClientTable : AnalyzeBase
{
    private string mTableFolderName = "TableFile";

    public AnalyzeClientTable(ResourceRedundanceManager mgr)
        : base(mgr)
    {
    }

    public override int GetAnalyzeType()
    {
        return (int)(AnalyzeType.AT_TXT);
    }

    public override void Analyze(ResourceUnit res)
    {
        int startIndex = res.GetPathName().IndexOf(mTableFolderName);

        string resourcesPath = res.GetPathName().Remove(startIndex);
        string tableSubPath = res.GetPathName().Remove(0, startIndex);
        string[] list = res.GetFileName().Split('.');
        string fileName = tableSubPath + "/" + list[0];

        int col = res.GetTableValidColmn();

        TextAsset textContent = Resources.Load(fileName, typeof(TextAsset)) as TextAsset;
        if (textContent == null)
        {
            Debug.LogError("Failed to open " + fileName);
            return;
        }

        string[] alldataline = textContent.text.Split('\n');
        for (int i = 2; i < alldataline.Length; i++)
        {
            string line = alldataline[i];
            if (String.IsNullOrEmpty(line))
                continue;

            if (line.IndexOf('#') == 0)
                continue;

            string[] strCols = line.Split('\t');

            if (strCols.Length > col)
            {
                string name = strCols[col-1];

                mResourceRedundanceMgr.ProcessResource((int)(AnalyzeType.AT_PREFAB), resourcesPath + res.GetTablePathSectorName() + name + ".prefab", res);
            }
        }


        

    }


}
