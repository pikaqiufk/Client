﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class EventSystem_UIEvent_CliamRewardWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(EventSystem.UIEvent_CliamReward), typeof(EventSystem.EventBase));
		L.RegFunction("New", _CreateEventSystem_UIEvent_CliamReward);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("EVENT_TYPE", get_EVENT_TYPE, set_EVENT_TYPE);
		L.RegVar("Idx", get_Idx, set_Idx);
		L.RegVar("RewardType", get_RewardType, set_RewardType);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEventSystem_UIEvent_CliamReward(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				EventSystem.UIEvent_CliamReward.Type arg0 = (EventSystem.UIEvent_CliamReward.Type)ToLua.CheckObject(L, 1, typeof(EventSystem.UIEvent_CliamReward.Type));
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
				EventSystem.UIEvent_CliamReward obj = new EventSystem.UIEvent_CliamReward(arg0, arg1);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: EventSystem.UIEvent_CliamReward.New");
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
			LuaDLL.lua_pushstring(L, EventSystem.UIEvent_CliamReward.EVENT_TYPE);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Idx(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EventSystem.UIEvent_CliamReward obj = (EventSystem.UIEvent_CliamReward)o;
			int ret = obj.Idx;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Idx on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RewardType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EventSystem.UIEvent_CliamReward obj = (EventSystem.UIEvent_CliamReward)o;
			EventSystem.UIEvent_CliamReward.Type ret = obj.RewardType;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index RewardType on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EVENT_TYPE(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			EventSystem.UIEvent_CliamReward.EVENT_TYPE = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Idx(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EventSystem.UIEvent_CliamReward obj = (EventSystem.UIEvent_CliamReward)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.Idx = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Idx on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_RewardType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EventSystem.UIEvent_CliamReward obj = (EventSystem.UIEvent_CliamReward)o;
			EventSystem.UIEvent_CliamReward.Type arg0 = (EventSystem.UIEvent_CliamReward.Type)ToLua.CheckObject(L, 2, typeof(EventSystem.UIEvent_CliamReward.Type));
			obj.RewardType = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index RewardType on a nil value" : e.Message);
		}
	}
}

