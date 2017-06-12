using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KFUnitySDKCallBack:MonoBehaviour{

	private static KFUnitySDKCallBack _instance;

	private static object _lock = new object();

	private static GameObject sdkGameObject;

	public static KFUnitySDKCallBack getInstance(){
		lock (_lock)
		{
			if (_instance == null)
			{
				if (sdkGameObject == null)
				{
					sdkGameObject = new GameObject("sdkGameObject");
					UnityEngine.Object.DontDestroyOnLoad(_instance);
					_instance = sdkGameObject.AddComponent<KFUnitySDKCallBack>();
					
				}
				else
				{
					_instance = sdkGameObject.GetComponent<KFUnitySDKCallBack>();
				}
			}
			return _instance;
		}
	}


	public void KFPayCallBack(string jsonData)
	{

	}

	public void KFUserCallBack(string jsonData)
	{
		Debug.Log ("user related call back revoke");
	}

	
}