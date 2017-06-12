

using System;
using System.Collections;
using ClientService;
using DataContract;
using EventSystem;
using UnityEngine;
using LitJson;
using Shared;


internal class PlatformListener : MonoBehaviour
{
    private static PlatformListener sInstance;

    public static PlatformListener Instance { get{return sInstance;}}

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif
        sInstance = this;
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void OnUserLogin(string param)
    {
        JsonData jsonData = JsonMapper.ToObject(param);
        string platfrom = (string)jsonData["platform"];
        string channel = (string)jsonData["channel"];
        string uid = string.Empty;
        var js = (IDictionary) jsonData;
        if (js.Contains("uid"))
        {
            uid = (string) jsonData["uid"];
        }
        string accesstoken = (string)jsonData["accesstoken"];

        if (string.IsNullOrEmpty(platfrom)
            && string.IsNullOrEmpty(channel)
            && string.IsNullOrEmpty(accesstoken))
        {
            Logger.Error("Third LoginData error! data = {0}", param);
        }

        LoginWindow.instance.StartCoroutine(LoginWindow.LoginByThirdCoroutine(platfrom, channel, uid, accesstoken));
    }

    public void OnUserLogout(string param)
    {

    }

    public void OnPayResult(string param)
    {

    }

	public void OnCallResult(string json)
	{
		JsonData jsonData = JsonMapper.ToObject (json);
		string funcName = (string)jsonData["FuncName"];

		if (funcName.Equals ("ReachabilityChanged")) 
        {
			ReachabilityChanged ();
		} 
        else if (funcName.Equals ("SpeechRecognized")) 
        {
			string content = (string)jsonData ["content"];
			SpeechRecognized (content);
		} 
        else if (funcName.Equals ("OnLowMemory")) 
		{
			OnLowMemory();
		}
        else if (funcName.Equals("onLogin"))
		{
		    string data =(string)jsonData["jsondata"];
            OnUserLogin(data);
		}
        else if (funcName.Equals("LoginCancel"))
		{
		    Game.Instance.ExitToLogin();
		}
        else if (funcName.Equals("QuitGame"))
		{
		    Application.Quit();
		}
        else if (funcName.Equals("logoutSuccess"))
		{
		   // Game.Instance.ChangeSceneToLogin();
        }
        else if (funcName.Equals("SwitchAccountSuccess"))
        {
            string data = (string)jsonData["jsondata"];
            ExitToLoginAndDoLogin(data);
        }
	}

    private void ExitToLoginAndDoLogin(string json)
    {
       Game.Instance.ChangeSceneToLoginAndAutoLogin(() =>
       {
           OnUserLogin(json);
       });
    }

    private void ReachabilityChanged()
    {
        EventDispatcher.Instance.DispatchEvent(new UIEvent_DeviceInfo_NetWorkStateChange());
    }

    private void SpeechRecognized(string content)
    {
        content = content.CheckSensitive();
        EventDispatcher.Instance.DispatchEvent(new ChatMainSpeechRecognized( content));
    }

	public void OnLowMemory()
	{
        Logger.Debug("----------OnLowMemory-------");

	    if (null != UIManager.Instance)
	    {
            UIManager.Instance.DestoryCloseUi();
	    }

	    ComplexObjectPool.DestroyUnusedObject();

	    if (null != ResourceManager.Instance)
	    {
            ResourceManager.Instance.ClearCache(true);
	    }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
	}

    private float interval = 0;

#if UNITY_ANDROID && !UNITY_EDITOR
    void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        interval += Time.deltaTime;
        if (interval > 1)
        {
            interval = 0;
             if (PlatformHelper.GetAvailMemory() < GameSetting.Instance.LowMemorySize)
            {
                OnLowMemory();
                interval = -10;
            }
        }
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
#endif
}

