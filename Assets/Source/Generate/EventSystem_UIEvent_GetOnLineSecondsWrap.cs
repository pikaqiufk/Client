﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class EventSystem_UIEvent_GetOnLineSecondsWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(EventSystem.UIEvent_GetOnLineSeconds), typeof(EventSystem.EventBase));
		L.RegFunction("New", _CreateEventSystem_UIEvent_GetOnLineSeconds);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("EVENT_TYPE", get_EVENT_TYPE, set_EVENT_TYPE);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEventSystem_UIEvent_GetOnLineSeconds(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				EventSystem.UIEvent_GetOnLineSeconds obj = new EventSystem.UIEvent_GetOnLineSeconds();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: EventSystem.UIEvent_GetOnLineSeconds.New");
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
			LuaDLL.lua_pushstring(L, EventSystem.UIEvent_GetOnLineSeconds.EVENT_TYPE);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EVENT_TYPE(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			EventSystem.UIEvent_GetOnLineSeconds.EVENT_TYPE = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

