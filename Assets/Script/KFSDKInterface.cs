using UnityEngine;

public abstract class KFSDKInterface{
	
//	public delegate void LoginSucHandler(U8LoginResult data);
//	public delegate void LogoutHandler();
	
	private static KFSDKInterface _instance;
	
//	public LoginSucHandler OnLoginSuc;
//	public LogoutHandler OnLogout;
	
	
	public static KFSDKInterface Instance
	{
		get
		{
			if (_instance == null)
			{
				#if UNITY_EDITOR || UNITY_STANDLONE
				UnityEngine.Debug.Log("no implemention");
				//_instance = new KFSDKInterfaceAndroid();
				#elif UNITY_ANDROID
				_instance = new KFSDKInterfaceAndroid();
				#elif UNITY_IOS
				_instance = new SDKInterfaceIOS();
				#endif
			}
			
			return _instance;
		}
	}

	//init 
	public abstract void initSDK();

	//login
	public abstract void login();

	//logout
	public abstract void logout();

	//changeAccout
	public abstract void changeAccout();

	//exit
	public abstract void exit();

	//pay
	public abstract void pay(string payparam);

	//getOrderInfo
	public abstract void getOrderInfo();

	//statisticsLogin
	public abstract void statisticRecordLogin(string logiJson);

	//statisticEnterGame
	public abstract void statisticEnterGame(string enterJson);

	//statisticCreateRole
	public abstract void statisticCreateRole(string createRoleJson);

	//statisticRoleUp
	public abstract void statisticRoleUp(string roleUpJson);

	
}