#region using

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ClientService;

#endregion

public static class Logger
{
    public enum LogLevelType
    {
        Debug = 0,
        Info,
        Warning,
        Error,
        Fatal,

        None
    }

#if UNITY_EDITOR
    public static LogLevelType LogLevel = LogLevelType.Debug;
#else
	public static LogLevelType LogLevel = LogLevelType.Info;
#endif
    private static string tempText = "";

    public static void Log2Bugly(string message, params object[] args)
    {
        var st = new StackTrace(true);
        var stackTraceBuilder = new StringBuilder("");
        var stFrameCount0 = st.FrameCount;
        for (var i = 1; i < stFrameCount0; i++)
        {
            var frame = st.GetFrame(i);
            stackTraceBuilder.AppendFormat("{0}.{1}", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name);
            var parameters = frame.GetMethod().GetParameters();
            if (parameters == null || parameters.Length == 0)
            {
                stackTraceBuilder.Append(" () ");
            }
            else
            {
                stackTraceBuilder.Append(" (");

                var pcount = parameters.Length;

                ParameterInfo param = null;
                for (var p = 0; p < pcount; p++)
                {
                    param = parameters[p];
                    stackTraceBuilder.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);

                    if (p != pcount - 1)
                    {
                        stackTraceBuilder.Append(", ");
                    }
                }
                param = null;
                stackTraceBuilder.Append(") ");
            }

            var fileName = frame.GetFileName();
            if (!string.IsNullOrEmpty(fileName) && !fileName.ToLower().Equals("unknown"))
            {
                fileName = fileName.Replace("\\", "/");

                var loc = fileName.ToLower().IndexOf("/assets/");
                if (loc < 0)
                {
                    loc = fileName.ToLower().IndexOf("assets/");
                }

                if (loc > 0)
                {
                    fileName = fileName.Substring(loc);
                }

                stackTraceBuilder.AppendFormat("(at {0}:{1})", fileName, frame.GetFileLineNumber());
            }
            stackTraceBuilder.AppendLine();
        }
        UnityEngine.Debug.LogError("[Log2Bugly] " + string.Format(message, args));
       // BuglyAgent.ReportException("Log2Bugly", string.Format(message, args), stackTraceBuilder.ToString());
    }


    public static void Debug(string message, params object[] args)
    {
        if (LogLevel <= LogLevelType.Debug)
        {
            var argsLength1 = args.Length;
            for (var i = 0; i < argsLength1; i++)
            {
                if (args[i] == null)
                {
                    UnityEngine.Debug.Log("[Debug] " + message + ", some of the args is null.");
                    return;
                }
            }
            UnityEngine.Debug.Log("[Debug] " + string.Format(message, args));
        }
    }

    public static void Info(string message, params object[] args)
    {
        if (LogLevel <= LogLevelType.Info)
        {
            var argsLength2 = args.Length;
            for (var i = 0; i < argsLength2; i++)
            {
                if (args[i] == null)
                {
                    UnityEngine.Debug.Log("[Info] " + message + ", some of the args is null.");
                    return;
                }
            }
            UnityEngine.Debug.Log("[Info] " + string.Format(message, args));
        }
    }

    public static void Warn(string message, params object[] args)
    {
        if (LogLevel <= LogLevelType.Warning)
        {
            var argsLength3 = args.Length;
            for (var i = 0; i < argsLength3; i++)
            {
                if (args[i] == null)
                {
                    tempText = message + ", some of the args is null.";
                    LogtoServer((int) LogLevelType.Warning, tempText);
                    UnityEngine.Debug.Log("[Warning] " + tempText);
                    return;
                }
            }
            tempText = string.Format(message, args);
            LogtoServer((int) LogLevelType.Warning, tempText);
            UnityEngine.Debug.LogWarning("[Warning] " + tempText);
        }
    }

    public static void Error(string message, params object[] args)
    {
        if (LogLevel <= LogLevelType.Error)
        {
            var argsLength4 = args.Length;
            for (var i = 0; i < argsLength4; i++)
            {
                if (args[i] == null)
                {
                    tempText = message + ", some of the args is null.";
                    LogtoServer((int) LogLevelType.Error, tempText);
                    UnityEngine.Debug.Log("[Error] " + tempText);
                    return;
                }
            }
            tempText = string.Format(message, args);
            LogtoServer((int) LogLevelType.Error, tempText);
            UnityEngine.Debug.LogError("[Error] " + tempText);
        }
    }

    public static void Fatal(string message, params object[] args)
    {
        if (LogLevel <= LogLevelType.Fatal)
        {
            var argsLength5 = args.Length;
            for (var i = 0; i < argsLength5; i++)
            {
                if (args[i] == null)
                {
                    tempText = message + ", some of the args is null.";
                    LogtoServer((int) LogLevelType.Fatal, tempText);
                    UnityEngine.Debug.Log("[Fatal] " + tempText);
                    return;
                }
            }
            tempText = string.Format(message, args);
            LogtoServer((int) LogLevelType.Error, tempText);
            UnityEngine.Debug.LogError("[Fatal] " + tempText);
        }
    }


    public static void LogtoServer(int type, string text)
    {
        if (NetManager.Instance != null)
        {
            NetManager.Instance.StartCoroutine(LogtoServerEnumerator(type, text));
        }
    }

    private static IEnumerator LogtoServerEnumerator(int type, string text)
    {
        var msg = NetManager.Instance.ClientErrorMessage(type, text);
        yield return msg.SendAndWaitUntilDone();
    }
}