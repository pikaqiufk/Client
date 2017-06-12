using System;
#region using

using UnityEngine;

#endregion

public class AndroidHelper : MonoBehaviour
{
    public static bool isDebug;
    private static AndroidHelper mInstance;


#if !UNITY_EDITOR && UNITY_ANDROID

    private const string SDK_JAVA_CLASS = "com.base.plugin.SDKService";
    private const string PLATFORM_CLASS = "com.base.plugin.PlatformHelper";

    private AndroidJavaObject SDKInstance;
    private AndroidJavaObject PlatformInstance;

    public static AndroidHelper Instance {
        get
        {
            return mInstance;
        }
    }


    void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mInstance = this;
        isDebug = false;
        AndroidJavaClass javaClass = new AndroidJavaClass(SDK_JAVA_CLASS);
        SDKInstance = javaClass.CallStatic<AndroidJavaObject>("getInstance");
        if (SDKInstance == null)
        {
            Logger.Error("SDKInstance = javaClass.CallStatic<AndroidJavaObject>(getInstance); return null!!");
        }
        AndroidJavaClass javaClass2 = new AndroidJavaClass(PLATFORM_CLASS);
        PlatformInstance = javaClass2.CallStatic<AndroidJavaObject>("getInstance");
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}


    public void doSdk(string functionName, string json)
    {
        Logger.Debug("SDKService_Debug_C#" + "funcname =" + functionName + ";json=" + json);
        onFuncCall("jniCall", functionName, json);
    }

    public void onFuncCall(string func, params object[] args)
    {
        Logger.Debug("onFuncCall funcname:" + func);
        SDKInstance.Call(func, args);
    }

    public void SpeechRecognize(short[] data, int length)
    {
        byte[] dBytes = new byte[length];
        Buffer.BlockCopy(data,0,dBytes,0,length);

        //注释是错误的示例,这些转换已经封装在AndroidJavaObject中了
//         IntPtr jAryPtr = AndroidJNIHelper.ConvertToJNIArray(dBytes);
//         jvalue array = new jvalue {l = jAryPtr};
// 
//         jvalue v2 = new jvalue();
//         v2.i = length;
        
        PlatformInstance.Call("SpeechRecognize",dBytes, length);

    }

    public static string platformHelper(string func, string json = "")
    {
        using (AndroidJavaClass cls = new AndroidJavaClass(PLATFORM_CLASS))
        {
            string ret = cls.CallStatic<string>("jniCall", func, json);
            return ret;
        }
    }

    void OnCallResult(string str)
    {
      //  Logger.Debug("androidHelper get message from java :" + str);
    }
#endif
}