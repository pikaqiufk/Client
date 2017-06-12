using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MainButtonIcon : MonoBehaviour
{
	private Transform Lock;
	private Transform NoticeIcon;

	void Awake()
	{
#if !UNITY_EDITOR
try
{
#endif

		Lock = transform.Find("Lock");
		NoticeIcon = transform.Find("NoticeIcon");
		ShowLock(false);
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	public void ShowLock(bool flag)
	{
		if (null != Lock)
		{
			Lock.gameObject.SetActive(flag);				
		}
		if (null != NoticeIcon)
		{
			NoticeIcon.gameObject.SetActive(!flag);	
		}
		
	}

}
