﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class SoundManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(SoundManager), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("IsBGMPlaying", IsBGMPlaying);
		L.RegFunction("IsPlaying", IsPlaying);
		L.RegFunction("PlayBGMusic", PlayBGMusic);
		L.RegFunction("PlaySoundEffect", PlaySoundEffect);
		L.RegFunction("PlaySoundEffectAtPos", PlaySoundEffectAtPos);
		L.RegFunction("PlaySoundEffectAtPos2", PlaySoundEffectAtPos2);
		L.RegFunction("SetAreaSoundMute", SetAreaSoundMute);
		L.RegFunction("SetBgmPause", SetBgmPause);
		L.RegFunction("StopAllSoundEffect", StopAllSoundEffect);
		L.RegFunction("StopBGM", StopBGM);
		L.RegFunction("StopSoundEffect", StopSoundEffect);
		L.RegFunction("StopSoundEffectByTag", StopSoundEffectByTag);
		L.RegFunction("Init", Init);
		L.RegFunction("Reset", Reset);
		L.RegFunction("Tick", Tick);
		L.RegFunction("Destroy", Destroy);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("BGMPrefsKey", get_BGMPrefsKey, null);
		L.RegVar("SFXPrefsKey", get_SFXPrefsKey, null);
		L.RegVar("mEnableBGM", get_mEnableBGM, set_mEnableBGM);
		L.RegVar("mEnableSFX", get_mEnableSFX, set_mEnableSFX);
		L.RegVar("mSFXChannelsCount", get_mSFXChannelsCount, set_mSFXChannelsCount);
		L.RegVar("mBgmVolume", get_mBgmVolume, set_mBgmVolume);
		L.RegVar("mSfxVolume", get_mSfxVolume, set_mSfxVolume);
		L.RegVar("mSoundClipPools", get_mSoundClipPools, set_mSoundClipPools);
		L.RegVar("EnableBGM", get_EnableBGM, set_EnableBGM);
		L.RegVar("EnableSFX", get_EnableSFX, set_EnableSFX);
		L.RegVar("Instance", get_Instance, null);
		L.RegVar("NextTag", get_NextTag, null);
		L.RegVar("VoicePlaying", null, set_VoicePlaying);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsBGMPlaying(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			bool o = obj.IsBGMPlaying(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsPlaying(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			bool o = obj.IsPlaying(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayBGMusic(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 5);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 3);
			float arg2 = (float)LuaDLL.luaL_checknumber(L, 4);
			bool arg3 = LuaDLL.luaL_checkboolean(L, 5);
			obj.PlayBGMusic(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlaySoundEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 5);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 3);
			uint arg2 = (uint)LuaDLL.luaL_checknumber(L, 4);
			bool arg3 = LuaDLL.luaL_checkboolean(L, 5);
			obj.PlaySoundEffect(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlaySoundEffectAtPos(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 5);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.Vector3 arg1 = ToLua.ToVector3(L, 3);
			UnityEngine.Vector3 arg2 = ToLua.ToVector3(L, 4);
			uint arg3 = (uint)LuaDLL.luaL_checknumber(L, 5);
			obj.PlaySoundEffectAtPos(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlaySoundEffectAtPos2(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 6);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.Vector3 arg1 = ToLua.ToVector3(L, 3);
			UnityEngine.Vector3 arg2 = ToLua.ToVector3(L, 4);
			uint arg3 = (uint)LuaDLL.luaL_checknumber(L, 5);
			bool arg4 = LuaDLL.luaL_checkboolean(L, 6);
			obj.PlaySoundEffectAtPos2(arg0, arg1, arg2, arg3, arg4);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetAreaSoundMute(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.SetAreaSoundMute(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetBgmPause(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			System.Collections.IEnumerator o = obj.SetBgmPause(arg0);
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopAllSoundEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			obj.StopAllSoundEffect();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopBGM(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.StopBGM(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopSoundEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.StopSoundEffect(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopSoundEffectByTag(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			uint arg0 = (uint)LuaDLL.luaL_checknumber(L, 2);
			obj.StopSoundEffectByTag(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			System.Collections.IEnumerator o = obj.Init();
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reset(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			obj.Reset();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Tick(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.Tick(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Destroy(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			SoundManager obj = (SoundManager)ToLua.CheckObject(L, 1, typeof(SoundManager));
			obj.Destroy();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_BGMPrefsKey(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, SoundManager.BGMPrefsKey);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SFXPrefsKey(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, SoundManager.SFXPrefsKey);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mEnableBGM(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, SoundManager.mEnableBGM);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mEnableSFX(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, SoundManager.mEnableSFX);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mSFXChannelsCount(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, SoundManager.mSFXChannelsCount);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mBgmVolume(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			float ret = obj.mBgmVolume;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mBgmVolume on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mSfxVolume(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			float ret = obj.mSfxVolume;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mSfxVolume on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mSoundClipPools(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			SoundClipPools ret = obj.mSoundClipPools;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mSoundClipPools on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EnableBGM(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			bool ret = obj.EnableBGM;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index EnableBGM on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EnableSFX(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			bool ret = obj.EnableSFX;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index EnableSFX on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.Push(L, SoundManager.Instance);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NextTag(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, SoundManager.NextTag);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mEnableBGM(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			SoundManager.mEnableBGM = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mEnableSFX(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			SoundManager.mEnableSFX = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mSFXChannelsCount(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			SoundManager.mSFXChannelsCount = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mBgmVolume(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.mBgmVolume = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mBgmVolume on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mSfxVolume(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.mSfxVolume = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mSfxVolume on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mSoundClipPools(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			SoundClipPools arg0 = (SoundClipPools)ToLua.CheckObject(L, 2, typeof(SoundClipPools));
			obj.mSoundClipPools = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index mSoundClipPools on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EnableBGM(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.EnableBGM = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index EnableBGM on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EnableSFX(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.EnableSFX = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index EnableSFX on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_VoicePlaying(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			SoundManager obj = (SoundManager)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.VoicePlaying = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index VoicePlaying on a nil value" : e.Message);
		}
	}
}
