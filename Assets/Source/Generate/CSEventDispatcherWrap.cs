﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class CSEventDispatcherWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("CSEventDispatcher");
		L.RegFunction("DispatchEvent", DispatchEvent);
		L.RegFunction("DispatchEvent_ParamInt", DispatchEvent_ParamInt);
		L.RegFunction("DispatchEvent_ParamFloat", DispatchEvent_ParamFloat);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DispatchEvent(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
			string arg0 = ToLua.CheckString(L, 1);
			object[] arg1 = ToLua.ToParamsObject(L, 2, count - 1);
			CSEventDispatcher.DispatchEvent(arg0, arg1);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DispatchEvent_ParamInt(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
			string arg0 = ToLua.CheckString(L, 1);
			int[] arg1 = ToLua.CheckParamsNumber<int>(L, 2, count - 1);
			CSEventDispatcher.DispatchEvent_ParamInt(arg0, arg1);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DispatchEvent_ParamFloat(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
			string arg0 = ToLua.CheckString(L, 1);
			float[] arg1 = ToLua.CheckParamsNumber<float>(L, 2, count - 1);
			CSEventDispatcher.DispatchEvent_ParamFloat(arg0, arg1);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

