﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GuideManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(GuideManager), typeof(Singleton<GuideManager>));
		L.RegFunction("FinishGuide", FinishGuide);
		L.RegFunction("GetCurrentGuideData", GetCurrentGuideData);
		L.RegFunction("GetCurrentGuidingStepId", GetCurrentGuidingStepId);
		L.RegFunction("IsGuiding", IsGuiding);
		L.RegFunction("NextStep", NextStep);
		L.RegFunction("Skip", Skip);
		L.RegFunction("StartGuide", StartGuide);
		L.RegFunction("StopGuiding", StopGuiding);
		L.RegFunction("New", _CreateGuideManager);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("DataList", get_DataList, set_DataList);
		L.RegVar("mCurrentGuideId", get_mCurrentGuideId, set_mCurrentGuideId);
		L.RegVar("mStep", get_mStep, set_mStep);
		L.RegVar("Enable", get_Enable, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateGuideManager(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				GuideManager obj = new GuideManager();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: GuideManager.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FinishGuide(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GuideManager obj = (GuideManager)ToLua.CheckObject(L, 1, typeof(GuideManager));
			obj.FinishGuide();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentGuideData(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GuideManager obj = (GuideManager)ToLua.CheckObject(L, 1, typeof(GuideManager));
			DataTable.StepByStepRecord o = obj.GetCurrentGuideData();
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentGuidingStepId(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GuideManager obj = (GuideManager)ToLua.CheckObject(L, 1, typeof(GuideManager));
			int o = obj.GetCurrentGuidingStepId();
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsGuiding(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GuideManager obj = (GuideManager)ToLua.CheckObject(L, 1, typeof(GuideManager));
			bool o = obj.IsGuiding();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NextStep(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GuideManager obj = (GuideManager)ToLua.CheckObject(L, 1, typeof(GuideManager));
			obj.NextStep();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Skip(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GuideManager obj = (GuideManager)ToLua.CheckObject(L, 1, typeof(GuideManager));
			obj.Skip();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StartGuide(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			GuideManager obj = (GuideManager)ToLua.CheckObject(L, 1, typeof(GuideManager));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			bool o = obj.StartGuide(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopGuiding(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GuideManager obj = (GuideManager)ToLua.CheckObject(L, 1, typeof(GuideManager));
			obj.StopGuiding();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DataList(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GuideManager obj = (GuideManager)o;
			System.Collections.Generic.List<DataTable.StepByStepRecord> ret = obj.DataList;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index DataList on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mCurrentGuideId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GuideManager obj = (GuideManager)o;
			int ret = obj.mCurrentGuideId;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mCurrentGuideId on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mStep(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GuideManager obj = (GuideManager)o;
			int ret = obj.mStep;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mStep on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Enable(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GuideManager obj = (GuideManager)o;
			bool ret = obj.Enable;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Enable on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_DataList(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GuideManager obj = (GuideManager)o;
			System.Collections.Generic.List<DataTable.StepByStepRecord> arg0 = (System.Collections.Generic.List<DataTable.StepByStepRecord>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<DataTable.StepByStepRecord>));
			obj.DataList = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index DataList on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mCurrentGuideId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GuideManager obj = (GuideManager)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.mCurrentGuideId = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mCurrentGuideId on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mStep(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GuideManager obj = (GuideManager)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.mStep = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mStep on a nil value" : e.Message);
		}
	}
}

