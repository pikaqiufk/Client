#region using

using System;
using System.Collections;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class LoginWindow : MonoBehaviour
{
    public static LoginWindow instance;
    public static LoginState State = LoginState.BeforeLogin;
    public static Action ThirdLoginAction;
    public UIInput Account = null;
    public UIInput IP = null;
    public Transform LoginFrame;
    private Coroutine mLoginCoroutine;
    public UIInput Password = null;
    public BindDataRoot root;
    public Transform ThirdLoginFrame;
	public UIPopupList PopList;
	public GameObject IpRoot;
    public UILabel Version;
    public enum LoginState
    {
        BeforeLogin,
        ThirdLogin,
        LoginSuccess,
        InGaming
    }

    private void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        instance = this;
        Init();
        Version.text = string.Format(GameUtils.GetDictionaryText(110000002), UpdateHelper.LocalGameVersion) + "." + UpdateHelper.Version;
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

	private void Start()
	{
#if !UNITY_EDITOR
try
{
#endif

#if !UNITY_EDITOR
	   var debug = PlayerPrefs.GetInt(GameSetting.LoginAssistantKey, 0);
        if(0 == debug)
        {
		    IpRoot.SetActive(false);
        }
#endif
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    public void Init()
    {
        LoginAssistant.CreateAssistant(transform.parent);

        switch (State)
        {
            case LoginState.BeforeLogin:
            {
                ShowLogin();
            }
                break;
            case LoginState.ThirdLogin:
            {
                LoginFrame.gameObject.SetActive(false);
                ThirdLoginFrame.gameObject.SetActive(false);
                if (null != ThirdLoginAction)
                {
                    ThirdLoginAction();
                }
            }
                break;
            case LoginState.LoginSuccess:
            {
                LoginFrame.gameObject.SetActive(false);
                ThirdLoginFrame.gameObject.SetActive(false);
            }
                break;
        }
    }

    public static void InvisibleLoginFrame()
    {
        if (null != instance)
        {
            instance.LoginFrame.gameObject.SetActive(false);
            instance.ThirdLoginFrame.gameObject.SetActive(false);
        }
    }

    public static IEnumerator LoginByThirdCoroutine(string platform, string channel, string uid, string accessToken)
    {
        ThirdLoginAction = null;
        using (new BlockingLayerHelper(0))
        {
            NetManager.Instance.Stop();
            NetManager.Instance.ServerAddress = GameUtils.GetServerAddress();
            var result = new AsyncResult<int>();
            var connectToGate = NetManager.Instance.ConnectToGate(result);
            yield return connectToGate;
            if (0 == result.Result)
            {
                // 连接失败!
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, 270111, "", PlatformHelper.UserLogout);
                yield break;
            }

            if (string.IsNullOrEmpty(uid) && string.IsNullOrEmpty(accessToken))
            {
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, "uid and accessToken empty!");
                yield break;
            }

            var loginMsg = NetManager.Instance.PlayerLoginByThirdKey(platform, channel, uid, accessToken);
            yield return loginMsg.SendAndWaitUntilDone();

            if (loginMsg.State == MessageState.Reply)
            {
                if ((int) ErrorCodes.OK == loginMsg.ErrorCode)
                {
                    NetManager.Instance.NeedReconnet = true;
                    PlayerDataManager.Instance.LastLoginServerId = loginMsg.Response.LastServerId;
                    if (channel.Equals("BaiDu"))
                    {
                        PlayerDataManager.Instance.UidForPay = uid;
                    }
                    else
                    {
                        PlayerDataManager.Instance.UidForPay = loginMsg.Response.Uid;
                    }
                    NetManager.Instance.StartCoroutine(LoginSuccess());
                }
                else if (loginMsg.ErrorCode == (int) ErrorCodes.Error_PLayerLoginWait)
                {
                    NetManager.Instance.NeedReconnet = false;
                    PlayerDataManager.Instance.AccountDataModel.LineUpShow = true;
                    var e = new Show_UI_Event(UIConfig.ServerListUI, null);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    NetManager.Instance.NeedReconnet = false;
                    UIManager.Instance.ShowNetError(loginMsg.ErrorCode);
                }
            }
            else
            {
                NetManager.Instance.NeedReconnet = false;
                Logger.Error("LoginByThirdCoroutine MessageState:{0}", loginMsg.State);
                GameUtils.ShowLoginTimeOutTip();
            }
        }
    }

    public static IEnumerator LoginCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            NetManager.Instance.Stop();
            var result = new AsyncResult<int>();
            var connectToGate = NetManager.Instance.ConnectToGate(result);
            yield return connectToGate;
            if (0 == result.Result)
            {
                // 连接失败!
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, 270111);
                yield break;
            }

            if (string.IsNullOrEmpty(PlayerDataManager.Instance.UserName))
            {
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, "user name empty!");
                yield break;
            }

            var loginMsg = NetManager.Instance.PlayerLoginByUserNamePassword(PlayerDataManager.Instance.UserName,
                PlayerDataManager.Instance.Password);
            yield return loginMsg.SendAndWaitUntilDone();

            if (loginMsg.State == MessageState.Reply)
            {
                if ((int) ErrorCodes.OK == loginMsg.ErrorCode)
                {
                    NetManager.Instance.NeedReconnet = true;
                    PlayerDataManager.Instance.LastLoginServerId = loginMsg.Response.LastServerId;
                    NetManager.Instance.StartCoroutine(LoginSuccess());
                }
                else if (loginMsg.ErrorCode == (int) ErrorCodes.Error_PLayerLoginWait)
                {
                    NetManager.Instance.NeedReconnet = false;
                    PlayerDataManager.Instance.AccountDataModel.LineUpShow = true;
                    var e = new Show_UI_Event(UIConfig.ServerListUI, null);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if ((int) ErrorCodes.PasswordIncorrect == loginMsg.ErrorCode)
                {
                    var errorCode = loginMsg.ErrorCode;
                    var dicId = errorCode + 200000000;
                    var tbDic = Table.GetDictionary(dicId);
                    var info = "";
                    if (tbDic == null)
                    {
                        info = GameUtils.GetDictionaryText(200000001) + errorCode;
                        Logger.Error(GameUtils.GetDictionaryText(200098), errorCode);
                    }
                    else
                    {
                        info = tbDic.Desc[GameUtils.LanguageIndex];
                    }
                    UIManager.Instance.ShowMessage(MessageBoxType.Ok, info);
                }
                else
                {
                    NetManager.Instance.NeedReconnet = false;
                    //Logger.Error("PlayerLoginByUserNamePassword ErrorCode" + loginMsg.ErrorCode);
                    UIManager.Instance.ShowNetError(loginMsg.ErrorCode);
                }
            }
            else
            {
                NetManager.Instance.NeedReconnet = false;
                Logger.Error("PlayerLoginByUserNamePassword MessageState:{0}", loginMsg.State);
                GameUtils.ShowLoginTimeOutTip();
            }
        }
    }

    public static IEnumerator LoginSuccess()
    {
        using (new BlockingLayerHelper(1))
        {
            const int placeHolder = 0;
            var serverListMsg = NetManager.Instance.GetServerList(placeHolder);
            yield return serverListMsg.SendAndWaitUntilDone();

            if (serverListMsg.State == MessageState.Reply)
            {
                if (serverListMsg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var serverListData = serverListMsg.Response;
                    //-----------------修改老数据导致 LastLoginServerId 数组越界的bug--------------------
                    if (PlayerDataManager.Instance.LastLoginServerId >= serverListData.Data.Count)
                    {
                        PlayerDataManager.Instance.LastLoginServerId = 0;
                    }
                    //---------------------------------------------------------------------------------
                    State = LoginState.LoginSuccess;
                    var e = new Show_UI_Event(UIConfig.ServerListUI,
                        new ServerListArguments {Data = serverListMsg.Response});
                    EventDispatcher.Instance.DispatchEvent(e);
                    Game.Instance.ServerInfoCached = true;
                }
                else
                {
                    State = LoginState.BeforeLogin;
                    Logger.Error("get server list error!");
                    GameUtils.ShowLoginTimeOutTip();
                }
            }
            else
            {
                State = LoginState.BeforeLogin;
                GameUtils.ShowLoginTimeOutTip();
                Logger.Error("GetServerList MessageState:{0}", serverListMsg.State);
            }
        }
    }

    public void MoveFoucsToAccount()
    {
        StartCoroutine(MoveFoucsToAccountCoroutine());
    }

    private IEnumerator MoveFoucsToAccountCoroutine()
    {
        yield return new WaitForEndOfFrame();
        IP.RemoveFocus();
        UIInput.selection = Account;
        Account.selectionEnd = Account.value.Length;
        Account.SaveValue();
    }

    public void MoveFoucsToPassWord()
    {
        Account.RemoveFocus();
        UIInput.selection = Password;
        Password.selectionEnd = Password.value.Length;
        Password.SaveValue();
    }

    public void OnAccountChange()
    {
        PlayerPrefs.SetString("Account", Account.value);
    }

    public void OnBtnThirdLogin()
    {
        PlatformHelper.UserLogin();
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        instance = null;
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    private void OnDisable()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        root.RemoveBinding();

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
        try
        {
#endif
		var var = PlayerPrefs.GetString("SelectServer", "uborm.com.cn:18001");
	    PopList.value = var;
        PlayerDataManager.Instance.AccountDataModel.Account = PlayerPrefs.GetString("Account",
            "Uborm" + Random.Range(1, 99999));
        PlayerDataManager.Instance.AccountDataModel.Pwd = PlayerPrefs.GetString("Password", "123");
        IP.value = PlayerPrefs.GetString("IP", "uborm.com.cn:18001");
        root.SetBindDataSource(PlayerDataManager.Instance.AccountDataModel);

        var soundId = int.Parse(Table.GetClientConfig(998).Value);
        if (!SoundManager.Instance.IsBGMPlaying(soundId))
        {
            SoundManager.Instance.PlayBGMusic(soundId, 1, 1);
        }
	    OnServerIPChanged();

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public void OnIPChange()
    {
		if (-1==PopList.items.IndexOf(IP.value))
		{
			PlayerPrefs.SetString("IP", IP.value);
			PlayerPrefs.Save();
		}
        
    }

    public void OnLoginBtnClick()
    {
        if (GameUtils.IsOurChannel())
        {
			NetManager.Instance.ServerAddress = IP.value;
            PlayerDataManager.Instance.UserName = Account.value;
            PlayerDataManager.Instance.Password = Password.value;
            if (mLoginCoroutine != null)
            {
                StopCoroutine(mLoginCoroutine);
            }
            mLoginCoroutine = StartCoroutine(LoginCoroutine());
        }
        else
        {
            PlatformHelper.UserLogin();
        }
    }

    public void OnPwdChange()
    {
        PlayerPrefs.SetString("Password", Password.value);
    }

    private void ShowLogin()
    {
        if (GameUtils.IsOurChannel())
        {
            LoginFrame.gameObject.SetActive(true);
            ThirdLoginFrame.gameObject.SetActive(false);
        }
        else
        {
            LoginFrame.gameObject.SetActive(false);
            ThirdLoginFrame.gameObject.SetActive(true);
            OnBtnThirdLogin();
        }
    }

	public void OnServerIPChanged()
	{
		var v = PopList.value;
	    var index = PopList.items.IndexOf(v);
		if (index >= 0 && index < PopList.items.Count)
		{
			if (PopList.items.Count - 1 == index)
			{
				IP.value = PlayerPrefs.GetString("IP", "uborm.com.cn:18001"); 
			}
			else
			{
				IP.value = PopList.items[index];
			}
		}
		else
		{
			IP.value = PopList.items[0];
		}
		PlayerPrefs.SetString("SelectServer", v);
		PlayerPrefs.Save();
	}

	private int TapCount = 0;
	public void ShowIpBtnClick()
	{
		TapCount++;
		if (0 == TapCount%10)
		{
			IpRoot.SetActive(true);
		}
		else
		{
			IpRoot.SetActive(false);
		}
	}

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif


#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }
}