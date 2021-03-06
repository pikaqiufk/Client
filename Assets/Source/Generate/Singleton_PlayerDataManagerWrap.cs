﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Singleton_PlayerDataManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Singleton<PlayerDataManager>), typeof(System.Object), "Singleton_PlayerDataManager");
		L.RegFunction("CreateInstance", CreateInstance);
		L.RegFunction("DestroyInstance", DestroyInstance);
		L.RegFunction("GetInstance", GetInstance);
		L.RegFunction("New", _CreateSingleton_PlayerDataManager);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateSingleton_PlayerDataManager(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				Singleton<PlayerDataManager> obj = new Singleton<PlayerDataManager>();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: Singleton<PlayerDataManager>.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateInstance(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Singleton<PlayerDataManager>.CreateInstance();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DestroyInstance(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Singleton<PlayerDataManager>.DestroyInstance();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetInstance(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			PlayerDataManager o = Singleton<PlayerDataManager>.GetInstance();
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.PushObject(L, Singleton<PlayerDataManager>.Instance);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

