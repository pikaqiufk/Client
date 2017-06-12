using UnityEngine;
using System.Collections;
using DataContract;
using EventSystem;
using System;

[RequireComponent(typeof(MainButton))]
public class NewFunctionGuide : MonoBehaviour {
	private MainButton mBtnController;
	public Transform PanelRoot;
	public float StayDelay = 1.0f;
	public float MoveTime = 0.4f;
	public Transform EffectlRoot;
	void Awake()
	{
#if !UNITY_EDITOR
try
{
#endif

		mBtnController = GetComponent<MainButton>();
		PanelRoot.gameObject.SetActive(false);
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	// Use this for initialization
	void Start () {
#if !UNITY_EDITOR
try
{
#endif

		EventDispatcher.Instance.AddEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, OnEvent_RunAnim);
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
	
	// Update is called once per frame
	void Update () {
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

	void OnDisable()
	{
#if !UNITY_EDITOR
try
{
#endif

		PanelRoot.gameObject.SetActive(false);
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
	void OnDestroy()
	{
#if !UNITY_EDITOR
try
{
#endif

		EventDispatcher.Instance.RemoveEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, OnEvent_RunAnim);
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	private void OnEvent_RunAnim(IEvent ievent)
	{
		if (!GameSetting.Instance.EnableNewFunctionTip)
		{
			return;
		}

		var e = ievent as UIEvent_PlayMainUIBtnAnimEvent;
		if (null == e)
		{
			return;
		}

		ShowNewFunction(e.BtnName,e.CallBack);
	}

	public string TestBtn = "BtnRank";

	[ContextMenu("TestBtn")]
	public void Test()
	{
		ShowNewFunction(TestBtn);
	}

	private void ShowNewFunction(string btnName,Action call = null)
	{
		Transform tm = null;
		UIButton parentBtn = null;
		foreach (var group in mBtnController.ToggleList)
		{
			for (int i=0; i<group.ToggleObj.transform.childCount; i++)
			{
				var btn = group.ToggleObj.transform.GetChild(i);
				if (btn.name.Equals(btnName))
				{
					parentBtn = group.Btn;
					tm = btn;
					break;
				}
			}

			if (null != tm && null != parentBtn)
			{
				break;
			}
		}

		if (null == tm || null == parentBtn)
		{
			return;
		}

		mBtnController.OnSubBtnClick(parentBtn,true);

		var spr = tm.GetComponent<UISprite>();
		var go = new GameObject();
		var spr1 = go.AddComponent<UISprite>();
		spr1.atlas = spr.atlas;
		spr1.spriteName = spr.spriteName;
		spr1.width = spr.width;
		spr1.height = spr.height;

		go.transform.parent = EffectlRoot;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;

		EffectlRoot.localPosition = Vector3.zero;

		var des = transform.root.InverseTransformPoint(tm.position)+new Vector3(0,-74,0);


		PanelRoot.root.GetComponent<UIRoot>().StartCoroutine(MoveToPos(StayDelay, MoveTime, go.transform, des, tm, call));
	}

	IEnumerator MoveToPos(float delay,float time, Transform tm,Vector3 pos, Transform unlock, Action call=null)
	{



		PanelRoot.gameObject.SetActive(true);
		yield return new WaitForSeconds(delay);
		Vector3 start = EffectlRoot.localPosition;
		float elapse = 0;
		while (elapse < time)
		{
			elapse += Time.deltaTime;

			EffectlRoot.localPosition = Vector3.Lerp(start, pos, elapse / time);
			yield return null;
		}
		//yield return new WaitForSeconds(delay);
		PanelRoot.gameObject.SetActive(false);
		GameObject.Destroy(tm.gameObject);

		var icon = unlock.GetComponent<MainButtonIcon>();
		if (null != icon)
		{
			icon.ShowLock(false);
		}
// 		if (null != parentBtn)
// 		{
// 			mBtnController.OnSubBtnClick(parentBtn);
// 		}

		if (null != call)
		{
			call();
		}
	}

}
