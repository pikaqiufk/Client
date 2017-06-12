#region using

using System.Collections.Generic;
using DataTable;
using DevConsole;
using ObjCommand;
using UnityEngine;

#endregion

public class GMCommand
{
    public static Dictionary<string, eGmCommandType> mGms = new Dictionary<string, eGmCommandType>();
    //添加控制台快捷显示
    public static void addGmGMCommand(string command, eGmCommandType type, string helpInfo = "")
    {
        if (!mGms.ContainsKey(command))
        {
            mGms.Add(command, type);
            Console.AddCommand(command, Unuse, helpInfo);
        }
    }

    public static void MoveTo(string command)
    {
        command = command.TrimStart(',');
        var cmd = command.Split(',');
        if (null == ObjManager.Instance.MyPlayer)
        {
            return;
        }

        if (cmd.Length < 3)
        {
            Logger.Info("eg. @@Moveto,-1,10,30");
        }

        var sceneId = int.Parse(cmd[0]);
        var x = int.Parse(cmd[1]);
        var y = int.Parse(cmd[2]);

        GameControl.Executer.Stop();

        if (-1 == sceneId)
        {
            GameControl.Executer.PushCommand(new MoveCommand(ObjManager.Instance.MyPlayer, new Vector3(x, 0, y)));
        }
        else
        {
            GameControl.Executer.PushCommand(new GoToCommand(sceneId, new Vector3(x, 0, y)));
        }
    }

    public static void regeditGMCommand()
    {
        Table.ForeachGMCommand(table =>
        {
            addGmGMCommand("!!" + table.Command, (eGmCommandType) table.Type);
            return true;
        });
        Console.AddCommand("@@Moveto", MoveTo, "");
    }

    //已经在DevConsole里存在了，不需要再这里实现
    public static void Unuse()
    {
    }
}