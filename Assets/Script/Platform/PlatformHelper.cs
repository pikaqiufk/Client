

// / ************************
// / 手机设备相关都在这
// / ************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Umeng;
using UnityEngine;
using EventSystem;
using LitJson;
using System.Runtime.InteropServices;
using cn.sharesdk.unity3d;
using DataContract;


public class PlatformHelper
{
    public static string Token = string.Empty;
    public static string RegisterID = string.Empty;
    private static ShareSDK ssdk = null;
    public static bool IsEnableDebugMode()
    {
        return true;
    }

    public static void Initialize()
    {
        //防止休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //crash eye
        Crasheye.RegisterLogCallback();
        var channel = GameSetting.Channel + GetCchId();
        Crasheye.SetChannelID(channel);

      //  InitBuglySDK();
        InitUmengSDK();
        InitSDK();
        

        var Game = GameObject.Find("Game");
        if (null != Game)
        {
            ssdk = Game.GetComponent<ShareSDK>();
            ssdk.shareHandler = ShareResultHandler;
            ssdk.authHandler = AuthResultHandler;
        }



    }

    public static void InitBuglySDK()
    {
//         Logger.Debug("InitBuglySDK");
//         // enable debug log print
//         if (GameUtils.IsOurChannel())
//         {
//             BuglyAgent.ConfigDebugMode(true);
//         }
// 
//         BuglyAgent.ConfigAutoReportLogLevel(LogSeverity.LogError);
//         BuglyAgent.RegisterLogCallback(CallbackDelegate.Instance.OnApplicationLogCallbackHandler);
// 
// #if UNITY_IOS && !UNITY_EDITOR
// 		BuglyAgent.InitWithAppId (GameSetting.buglyAppIdiOS);
//         #elif UNITY_ANDROID && !UNITY_EDITOR
//         BuglyAgent.InitWithAppId(GameSetting.buglyAppIdAndroid);
//         #endif
    }

    public static void InitUmengSDK()
    {
        Logger.Debug("InitUmengSDK");

        var channel = GameSetting.Channel + GetCchId();

        #if UNITY_IOS && !UNITY_EDITOR
        Analytics.SetCrashReportEnabled(false);
        GA.StartWithAppKeyAndChannelId(GameSetting.UmengAppKeyiOS, channel);
        #elif UNITY_ANDROID && !UNITY_EDITOR
        GA.StartWithAppKeyAndChannelId(GameSetting.UmengAppKeyAndroid, channel);
        #endif

        GA.SetLogEnabled(false);
        //for debug
        Logger.Debug(" -------GetDeviceInfo ={0}", GA.GetDeviceInfo());
        // UmengTest();
    }

    public static void OnPlayerLevelUpGrade()
    {
        try
        {
            int level = PlayerDataManager.Instance.GetLevel();
            if (level > 0)
            {
                SetUserLevel(level);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }

    public static void UmengTest()
    {
//         Logger.Debug("UmengTest -- begain");
//         ProfileSignIn("10001");
//         SetUserLevel(100);
        //UMEvent("umengtest");
        //UMEvent("yindqi", "test");
//          UMEventBegain("testevent", "test");
//          UMEventBegain("testevent", "test");

//         var dict = new Dictionary<string,string>();
//         dict["type"] = "popular";
//         dict["artist"] = "JJLin";
//         GA.EventBeginWithPrimarykeyAndAttributes("music", "song1", dict);
// 
//         var dict2 = new Dictionary<string,string>();
//         dict2["type"] = "popular";
//         dict2["artist"] = "Jobs";
//         GA.EventBeginWithPrimarykeyAndAttributes("music", "song2", dict2);
// 
// 
//         GA.EventEndWithPrimarykey("music", "song1");
//         GA.EventEndWithPrimarykey("music", "song2");


//         UMPageBegain("testpage");
//         UMPageEnd("testpage");
//         Logger.Debug("UmengTest -- end");

// 
//         var dict = new Dictionary<string, string>();
//         dict["book"] = "3";
//         GA.Event("Mission", dict);
// 
//         UMEvent("Mission", "Accept", 111);
//         UMEvent("Mission", "AccCompleteept", 111);
    }


    public static void BuglySetUserId(string userid)
    {
       // BuglyAgent.SetUserId(userid);
    }

    public static void PlayLogoMovie()
    {
        Logger.Debug("PlayFullScreenMovie logo.mp4!");
#if UNITY_EDITOR
        return;
#elif UNITY_IOS
		PlayMovieAtPath("Data/Raw/logo.mp4");
#elif UNITY_ANDROID
        PlayMovieAtPath("logo.mp4");
#else
       // Handheld.PlayFullScreenMovie(path, Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFill);
#endif
    }

    #region 游戏统计方法

    //统计账号信息
    public static void ProfileSignIn(string playerid)
    {
        GA.ProfileSignIn(playerid, GameSetting.Channel);
    }

    //账号登出时使用
    public static void ProfileSignOff()
    {
        GA.ProfileSignOff();
    }

    //玩家等级统计
    public static void SetUserLevel(int level)
    {
        GA.SetUserLevel(level);
    }

    //事件数量统计
     public static void Event(string eventId)
     {
//         GA.Event(eventId);
     }
 
     //GA.Event("player_dead", "level1");
     public static void Event(string eventId, string label)
     {
//         GA.Event(eventId, label);
     } 
     public static void Event(string eventId, string name, int value)
     {
//          var dic = new Dictionary<string, string>(1);
//          dic.Add(name, value.ToString());
//          Event(eventId, dic);
     }

     public static void Event(string eventId, Dictionary<string, string> attributes)
     {
//         if (attributes.Count > 10)
//         {
//             Logger.Error("属性中的Key-Vaule Pair不能超过10个");
//             return;
//         }
//         GA.Event(eventId, attributes);
     }

    public static void UMEvent(string eventId)
    {
        GA.Event(eventId);
    }

    public static void UMEvent(string eventId, string label)
    {
        GA.Event(eventId, label);
    }

    public static void UMEvent(string eventId, string name, string value)
    {
        var dic = new Dictionary<string, string>(1);
        dic.Add(name, value);
        UMEvent(eventId, dic);
    }

    public static void UMEvent(string eventId, string name, int value)
    {
        var dic = new Dictionary<string, string>(1);
        dic.Add(name, value.ToString());
        UMEvent(eventId, dic);
    }

    public static void UMEvent(string eventId, Dictionary<string, string> attributes)
    {
        if (attributes.Count > 10)
        {
            Logger.Error("属性中的Key-Vaule Pair不能超过10个");
            return;
        }
        GA.Event(eventId, attributes);
    }

    //事件时长统计
    public static void EventBegain(string eventId, string label)
    {
        //GA.EventBegin(eventId, label);
//         GA.EventBegin(eventId);
    }

    public static void EventEnd(string eventId, string label)
    {
        //GA.EventEnd(eventId, label);
/*        GA.EventEnd(eventId);*/
    }

    //页面访问统计
    public static void PageBegain(string pageName)
    {
       GA.PageBegin(pageName);
    }

    public static void PageEnd(string pageName)
    {
        GA.PageEnd(pageName);
    }


    #endregion

#region 分享功能
    public static void ShareToPlatfrom(string text, string imageUrl, string Title)
    {
#if UNITY_EDITOR
        PlayerDataManager.Instance.SetFlag(559);
        var list = new Int32Array();
        list.Items.Add(559);
        PlayerDataManager.Instance.SetFlagNet(list);
        return;
#endif

        if (!File.Exists(imageUrl))
        {
            Logger.Error("share error! cant find screenshot!");
            return;
        }
        ShareContent content = new ShareContent();
        content.SetText(text);
        content.SetImageUrl(imageUrl);
        content.SetTitle(Title);
        content.SetShareType(ContentType.Image);
        ssdk.ShowPlatformList(null, content, 100, 100);
    }


    static void ShareResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Logger.Debug(MiniJSON.jsonEncode(result));
            PlayerDataManager.Instance.SetFlag(559);
            var list = new Int32Array();
            list.Items.Add(559);
            PlayerDataManager.Instance.SetFlagNet(list);
        }
        else if (state == ResponseState.Fail)
        {
            Logger.Debug("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
        }
        else if (state == ResponseState.Cancel) 
        {
            Logger.Debug("cancel share!");
        }
    }

    static void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Logger.Debug("authorize success !");
        }
        else if (state == ResponseState.Fail)
        {
           Logger.Debug("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
        }
        else if (state == ResponseState.Cancel)
        {
           Logger.Debug("cancel !");
        }
    }

#endregion



#region 平台共有方法

    public static void RestartApp()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
       // AndroidHelper.platformHelper("RestartApp");
       Application.Quit();
#endif

    }


    public static void UserCenter()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        DoUserCenter();
#endif
    }

    public static void CollectionEnterGameDataForKuaifa(string roleId, string roleName, int roleLevel, string serverId, string serverName, int viplevel, string partName, string createTime)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("type");
        writer.Write("enterGame");
        writer.WritePropertyName("roleId");
        writer.Write(roleId);
        writer.WritePropertyName("roleName");
        writer.Write(roleName);
        writer.WritePropertyName("serverId");
        writer.Write(serverId);
        writer.WritePropertyName("serverName");
        writer.Write(serverName);
        writer.WritePropertyName("roleLevel");
        writer.Write(roleLevel);
        writer.WritePropertyName("vipLevel");
        writer.Write(viplevel);
        writer.WritePropertyName("partName");
        writer.Write(partName);
        writer.WritePropertyName("createTime");
        writer.Write(createTime);
        writer.WriteObjectEnd();
        OnColloections(sb.ToString());
#endif
    }

    public static void CollectionCreateRoleDataForKuaifa(string roleId, string roleName, string serverId, string serverName, string createTime)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("type");
        writer.Write("createRole");
        writer.WritePropertyName("roleId");
        writer.Write(roleId);
        writer.WritePropertyName("roleName");
        writer.Write(roleName);
        writer.WritePropertyName("serverId");
        writer.Write(serverId);
        writer.WritePropertyName("serverName");
        writer.Write(serverName);
        writer.WritePropertyName("createTime");
        writer.Write(createTime);
        writer.WriteObjectEnd();
        OnColloections(sb.ToString());
#endif
    }

    public static void CollectionLevelUpDataForKuaifa(int roleLevel, string time)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("type");
        writer.Write("levelUp");
        writer.WritePropertyName("roleLevel");
        writer.Write(roleLevel);
        writer.WritePropertyName("time");
        writer.Write(time);
        writer.WriteObjectEnd();
        OnColloections(sb.ToString());
#endif
    }

    public static void CollectionPayDataForKuaifa(string amount, string orderNumber, string orderDesc)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("type");
        writer.Write("paySuccess");
        writer.WritePropertyName("amount");
        writer.Write(amount);
        writer.WritePropertyName("orderNumber");
        writer.Write(orderNumber);
        writer.WritePropertyName("orderDesc");
        writer.Write(orderDesc);
        writer.WriteObjectEnd();
        OnColloections(sb.ToString());
        #endif
    }




#endregion

#if UNITY_ANDROID && !UNITY_EDITOR
    public static void InitSDK()
    {
        AndroidHelper.Instance.doSdk("initSdk", "");
    }

    public static void ShowWebView(string url, float x, float y, float width, float height)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("URL");
        writer.Write(url);
        writer.WritePropertyName("X");
        writer.Write(x);
        writer.WritePropertyName("Y");
        writer.Write(y);
        writer.WritePropertyName("WIDTH");
        writer.Write(width);
        writer.WritePropertyName("HEIGHT");
        writer.Write(height);
        writer.WriteObjectEnd();

        AndroidHelper.platformHelper("ShowWebView",sb.ToString());
    }

    public static void CLoseWebView()
    {
        AndroidHelper.platformHelper("CloseWebView");
    }

    public static void UserLogin()
    {
        if (GetNetworkState()==-1)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 3300018);
            return;
        }
        AndroidHelper.Instance.doSdk("doLogin", "");
    }

    public static void UserLogout()
    {
        AndroidHelper.Instance.doSdk("doLogout", "");
    }

    public static void ChangeAccount()
    {
        AndroidHelper.Instance.doSdk("doChangeAccount", "");
    }

    public static void ShowToolBar()
    {
        AndroidHelper.Instance.doSdk("showToolBar", "");
    }

    public static void CloseToolBar()
    {
        AndroidHelper.Instance.doSdk("closeToolBar", "");
    }

    public static void Exit()
    {
        if (GameUtils.IsOurChannel())
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 300918, "", Application.Quit);
        }
        else
        {
            AndroidHelper.Instance.doSdk("exit", "");
        }
    }

    public static string GetCchId()
    {
        string ret = AndroidHelper.platformHelper("GetCchId");
        return ret;
    }


    public static void SendLocalOrder()
    {
        AndroidHelper.Instance.doSdk("sendLocalOrder", "");
    }

    public static void ClearOrder(string orderid)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("orderid");
        writer.Write(orderid);
        writer.WriteObjectEnd();
        AndroidHelper.Instance.doSdk("clearOrder", sb.ToString());
    }

    public static void MakePay(string uid, string roleID, string groupID, string oid, string token, string roleName)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("ROLEID");
        writer.Write(roleID);
        writer.WritePropertyName("UID");
        writer.Write(uid);
        writer.WritePropertyName("SERVERID");
        writer.Write(groupID);
        writer.WritePropertyName("OID");
        writer.Write(oid);
        writer.WritePropertyName("TOKEN");
        writer.Write(token);
        writer.WritePropertyName("ROLENAME");
        writer.Write(roleName);
        writer.WriteObjectEnd();

        AndroidHelper.Instance.doSdk("doOrder", sb.ToString());
    }

    public static void MakePayWithGoodInfo(string info)
    {
         AndroidHelper.Instance.doSdk("makePayWithGoodInfo", info.ToString());
    }

    public static void SetScreenBrightness(float brightness)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("brightness");
        writer.Write(brightness);
        writer.WriteObjectEnd();
        AndroidHelper.platformHelper("SetScreenBrightNess", sb.ToString());
    }
    
    public static float GetScreenBrightness()
    {
        string ret = AndroidHelper.platformHelper("GetScreenBrightNess");
        float bright = float.Parse(ret);
        return bright;
    }

	public static int GetNetworkState()
	{
		string ret = AndroidHelper.platformHelper("GetNetworkState");
		int state = int.Parse(ret);
		return state;
	}

    public static float GetBatteryLevel()
    {
        string ret = AndroidHelper.platformHelper("GetBatteryLevel");
		float state = float.Parse(ret);
		return state;
    }

    public static void SpeechRecognize(int voiceid, short[] data, int length)
    {
        AndroidHelper.Instance.SpeechRecognize(data,length);
    }

    
    public static void SetLocalNotification(string key, string message, double timeInterval)
    {
        if (timeInterval < 0)
        {
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("KEY");
        writer.Write(key);
        writer.WritePropertyName("MESSAGE");
        writer.Write(message);
        writer.WritePropertyName("TIMEINTERVAL");
        writer.Write(timeInterval);
        writer.WriteObjectEnd();
        AndroidHelper.platformHelper("SetLocalNotification", sb.ToString());
    }

    public static void DeleteLocalNotificationWithKey(string key)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("KEY");
        writer.Write(key);
        writer.WriteObjectEnd();
        AndroidHelper.platformHelper("DeleteLocalNotificationWithKey", sb.ToString());
    }

    public static void ClearAllLocalNotification()
    {
        AndroidHelper.platformHelper("ClearAllLocalNotification");
    }

    public static int GetAvailMemory()
    {
        string ret = AndroidHelper.platformHelper("GetAvailMemory");
		int size = int.Parse(ret);
		return size;
    }

    public static void PlayMovieAtPath(string path)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("Path");
        writer.Write(path);
        writer.WriteObjectEnd();
        AndroidHelper.platformHelper("PlayMovieAtPath", sb.ToString());
    }

    public static void DoUserCenter()
    {
        AndroidHelper.Instance.doSdk("userCenter", "");
    }

    public static void OnColloections(string json)
    {
        AndroidHelper.Instance.doSdk("onCollections", json);
    }


#elif UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern string GetChannelString();

    [DllImport("__Internal")]
    public static extern void ShowWebView(string url,float x, float y, float width, float height);

    [DllImport("__Internal")]
    public static extern void CLoseWebView();

    [DllImport("__Internal")]
    public static extern void SetScreenBrightness(float brightness);

    [DllImport("__Internal")]
    public static extern float GetScreenBrightness();

	[DllImport("__Internal")]
	public static extern int GetNetworkState();

	[DllImport("__Internal")]
	public static extern float GetBatteryLevel();

    [DllImport("__Internal")]
	public static extern void SpeechRecognize(int voiceid, short[] data, int length);

    [DllImport("__Internal")]
    public static extern void SetLocalNotification(string key, string message, double timeInterval);

    [DllImport("__Internal")]
    public static extern void DeleteLocalNotificationWithKey(string key);

    [DllImport("__Internal")]
    public static extern void ClearAllLocalNotification();

	[DllImport("__Internal")]
	public static extern void PlayMovieAtPath(string path);

    public static int GetAvailMemory()
    {
        return SystemInfo.systemMemorySize;
    }

    public static void ShowToolBar()
    {

    }

    public static void CloseToolBar()
    {
        
    }

    public static void InitSDK()
    {

    }
    public static void UserLogin()
    {

    }

    public static void MakePayWithGoodInfo(string info)
    {

    }

    public static void Exit()
    {
       
    }

    public static void ChangeAccount()
    {

    }

    public static void UserLogout()
    {

    }
    public static string GetCchId()
    {
        return "UNITY_EDITER";
    }

#else
    public static void InitSDK()
    {

    }

    public static void UserLogin()
    {

    }

    public static void UserLogout()
    {

    }

    public static void ChangeAccount()
    {

    }

    public static string GetCchId()
    {
        return "UNITY_EDITER";
    }

    public static void ShowWebView(string url,float x, float y, float width, float height)
    {
       Application.OpenURL(url);  
    }

    public static void CLoseWebView()
    {
        
    }

    public static void MakePay(string uid, string roleID, string groupID, string oid, string token, string roleName)
    {

    }

    public static void MakePayWithGoodInfo(string info)
    {

    }

    public static void SendLocalOrder()
    {

    }

    public static void ClearOrder(string orderid)
    {

    }

    public static void ShowToolBar()
    {

    }

    public static void CloseToolBar()
    {
        
    }

    public static void SetScreenBrightness(float brightness)
    {

    }

    public static float GetScreenBrightness()
    {
        return 1.0f;
    }

	public static int GetNetworkState()
	{
//		case NotReachable:
//		// 没有网络连接
//		{
//			return -1;
//		}
//		break;
//		case ReachableViaWWAN:
//		// 使用3G网络
//		{
//			return 0;
//		}
//		break;
//		case ReachableViaWiFi:
//		// 使用WiFi网络
//		{
//			return 1;
//		}

		return UnityEngine.Random.Range(0,2);
	}

    public static float GetBatteryLevel()
    {
        return 1f;
    }

    public static void SpeechRecognize(int EncodeSize, short[] pcm, int length)
    {
        ResourceManager.Instance.StartCoroutine(SpeechRecognizefinished(EncodeSize, pcm,length));
    }

    static IEnumerator SpeechRecognizefinished(int EncodeSize, short[] pcm,int length)
    {
        yield return new WaitForSeconds(1);
        string result = string.Format("EncodeData Size:{0}KB, DecodeData Size:{1}KB", EncodeSize/1024, length /1024);
        EventDispatcher.Instance.DispatchEvent(new ChatMainSpeechRecognized(result));
    }

    public static void SetLocalNotification(string key, string message, double timeInterval)
    {
        if (timeInterval < 0)
        {
            return;
        }
    }

    public static void DeleteLocalNotificationWithKey(string key)
    {
        
    }

    public static void ClearAllLocalNotification()
    {
        
    }

    public static int GetAvailMemory()
    {
        return SystemInfo.systemMemorySize;
    }

    public static void Exit()
    {
       UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 300918, "", Application.Quit);
    }
#endif
}

