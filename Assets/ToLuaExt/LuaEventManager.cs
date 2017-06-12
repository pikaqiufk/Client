using UnityEngine;
using System.Collections;
using LuaInterface;
using System;
using EventSystem;

namespace ToLuaEx
{
	

public class LuaEventManager
{
	private static LuaEventManager s_Instance = null;

	public static LuaEventManager Instance
	{
		get
		{
			if (null == s_Instance)
			{
				s_Instance = new LuaEventManager();
			}
			return s_Instance;
		}
		
	}

	private LuaFunction mEventLuaFunc = null;

	private LuaFunction EventLuaFunc
	{
		get
		{
			if (null == mEventLuaFunc)
			{
				if (null != LuaManager.Instance.Lua)
				{
					mEventLuaFunc = LuaManager.Instance.Lua.GetFunction("EventManager.DispatchEvent");	
				}
			}
			return mEventLuaFunc;
		}
	}

	public void PushEvent(string eventName, params object[] args)
	{
		try
		{
			var func = EventLuaFunc;
			if (null != EventLuaFunc)
			{
				func.BeginPCall();
				func.Push(eventName);
				foreach (var o in args)
				{
					func.Push(o);
				}
				func.PCall();
				func.EndPCall();
			}
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
		
		
	}

}
}

public static class CSEventDispatcher
{

	public static void DispatchEvent(string eventName, params object[] param)
	{
		var type = Type.GetType(eventName);
		if (null == type)
		{
			Debug.LogError("null == Type.GetType(" + eventName + ")");
			return;
		}

		IEvent e = null;
		if (null == param || 0 == param.Length)
		{
			e = Activator.CreateInstance(type) as IEvent;
		}
		else if (1 == param.Length)
		{
			e = Activator.CreateInstance(type, param[0]) as IEvent;
		}
		else if (2 == param.Length)
		{
			e = Activator.CreateInstance(type, param[0], param[1]) as IEvent;
		}
		else if (3 == param.Length)
		{
			e = Activator.CreateInstance(type, param[0], param[1], param[2]) as IEvent;
		}
		else if (4 == param.Length)
		{
			e = Activator.CreateInstance(type, param[0], param[1], param[2], param[3]) as IEvent;
		}
		if (null == e)
		{
			Debug.LogError("null == Type.GetType(" + eventName + ")");
			return;
		}
		EventDispatcher.Instance.DispatchEvent(e);
	}

	//这个函数和上面那个实践是一样的，但是不能直接掉上面的函数，如果直接用上面的函数，那参数就被转成object[1]
	private static void DispatchEventTemplate<T>(string eventName, params T[] param)
	{
		var type = Type.GetType(eventName);
		if (null == type)
		{
			Debug.LogError("null == Type.GetType(" + eventName + ")");
			return;
		}

		IEvent e = null;
		if (null == param || 0 == param.Length)
		{
			e = Activator.CreateInstance(type) as IEvent;
		}
		else if (1 == param.Length)
		{
			e = Activator.CreateInstance(type, param[0]) as IEvent;
		}
		else if (2 == param.Length)
		{
			e = Activator.CreateInstance(type, param[0], param[1]) as IEvent;
		}
		else if (3 == param.Length)
		{
			e = Activator.CreateInstance(type, param[0], param[1], param[2]) as IEvent;
		}
		else if (4 == param.Length)
		{
			e = Activator.CreateInstance(type, param[0], param[1], param[2], param[3]) as IEvent;
		}
		if (null == e)
		{
			Debug.LogError("null == Type.GetType(" + eventName + ")");
			return;
		}
		EventDispatcher.Instance.DispatchEvent(e);
	}

	public static void DispatchEvent_ParamInt(string eventName, params int[] param)
	{
		DispatchEventTemplate<int>(eventName, param);
	}

	public static void DispatchEvent_ParamFloat(string eventName, params float[] param)
	{
		DispatchEventTemplate<float>(eventName, param);
	}
}