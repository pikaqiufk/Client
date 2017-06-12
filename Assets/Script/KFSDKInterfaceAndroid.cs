using UnityEngine;
using System;

public class KFSDKInterfaceAndroid : KFSDKInterface
{
	
	private AndroidJavaObject javaObject;
	
	public KFSDKInterfaceAndroid()
	{
		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			javaObject = jc.GetStatic<AndroidJavaObject>("currentActivity");
		}
	}
	
	private T invokeJavaSDK<T>(string method, params object[] param)
	{
		try
		{
			return javaObject.Call<T>(method, param);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
		return default(T);
	}
	
	private void invokeJavaSDK(string method, params object[] param)
	{
		try
		{
			javaObject.Call(method, param);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	//这里Android中，在onCreate中直接调用了initSDK，所以这里就不用调用了
	public override void initSDK()
	{
		invokeJavaSDK("initSDK");
		
	}
	
	public override void login()
	{
		invokeJavaSDK("login");
	}

	public override void logout ()
	{
		invokeJavaSDK ("logout");
	}

	public override void exit ()
	{
		invokeJavaSDK ("exit");
	}

	public override void changeAccout ()
	{
		invokeJavaSDK ("changeAccout");
	}

	public override void pay (String payparam)
	{
		invokeJavaSDK ("pay",payparam);
	}
	
	public override void getOrderInfo ()
	{
		invokeJavaSDK ("getOrderInfo");
	}
	// 统计
	public override void statisticRecordLogin (string logiJson)
	{
		invokeJavaSDK ("statisticRecordLogin",logiJson);
	}

	public override void statisticEnterGame (string enterJson)
	{
		invokeJavaSDK ("statisticEnterGame",enterJson);
	}
	public override void statisticCreateRole (string createRoleJson)
	{
		invokeJavaSDK ("statisticCreateRole",createRoleJson);
	}
	public override void statisticRoleUp (string roleUpJson)
	{
		invokeJavaSDK ("statisticRoleUp",roleUpJson);
	}


}