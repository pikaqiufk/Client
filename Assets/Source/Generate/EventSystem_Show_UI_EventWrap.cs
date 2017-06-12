﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class EventSystem_Show_UI_EventWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(EventSystem.Show_UI_Event), typeof(EventSystem.EventBase));
		L.RegFunction("New", _CreateEventSystem_Show_UI_Event);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("EVENT_TYPE", get_EVENT_TYPE, set_EVENT_TYPE);
		L.RegVar("config", get_config, set_config);
		L.RegVar("Args", get_Args, set_Args);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEventSystem_Show_UI_Event(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				UIConfig arg0 = (UIConfig)ToLua.CheckObject(L, 1, typeof(UIConfig));
				UIInitArguments arg1 = (UIInitArguments)ToLua.CheckObject(L, 2, typeof(UIInitArguments));
				EventSystem.Show_UI_Event obj = new EventSystem.Show_UI_Event(arg0, arg1);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: EventSystem.Show_UI_Event.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EVENT_TYPE(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, EventSystem.Show_UI_Event.EVENT_TYPE);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_config(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EventSystem.Show_UI_Event obj = (EventSystem.Show_UI_Event)o;
			UIConfig ret = obj.config;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index config on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Args(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EventSystem.Show_UI_Event obj = (EventSystem.Show_UI_Event)o;
			UIInitArguments ret = obj.Args;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Args on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EVENT_TYPE(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			EventSystem.Show_UI_Event.EVENT_TYPE = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_config(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EventSystem.Show_UI_Event obj = (EventSystem.Show_UI_Event)o;
			UIConfig arg0 = (UIConfig)ToLua.CheckObject(L, 2, typeof(UIConfig));
			obj.config = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index config on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Args(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EventSystem.Show_UI_Event obj = (EventSystem.Show_UI_Event)o;
			UIInitArguments arg0 = (UIInitArguments)ToLua.CheckObject(L, 2, typeof(UIInitArguments));
			obj.Args = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Args on a nil value" : e.Message);
		}
	}
}

