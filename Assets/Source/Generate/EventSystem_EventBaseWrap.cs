﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class EventSystem_EventBaseWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(EventSystem.EventBase), typeof(System.Object));
		L.RegFunction("New", _CreateEventSystem_EventBase);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEventSystem_EventBase(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				EventSystem.EventBase obj = new EventSystem.EventBase(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: EventSystem.EventBase.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

