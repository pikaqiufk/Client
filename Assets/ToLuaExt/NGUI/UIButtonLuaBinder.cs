using UnityEngine;
using System.Collections;

public static class UIButtonLuaBinder 
{
	public delegate void OnClick(GameObject go);
	public static void BindClickEvent(UIButton btn,OnClick call)
	{
		btn.onClick.Clear();
		var del = new EventDelegate(() =>
		{
			call(btn.gameObject);
		});
		btn.onClick.Add(del);
		
	}

	public static void AddClickEvent(UIButton btn, OnClick call)
	{
		var del = new EventDelegate(() =>
		{
			call(btn.gameObject);
		});
		btn.onClick.Add(del);
	}
}
