﻿using UnityEngine;
using LuaInterface;
using System.Collections.Generic;

namespace ToLuaEx
{
//Lua组件 
	[AddComponentMenu("Lua/LuaComponent")]
	public class LuaComponent : MonoBehaviour
	{
		//lua环境，需要在使用前给其赋值
		public static LuaState s_luaState;

		//函数名字定义
		protected static class FuncName
		{
			public static readonly string Awake = "Awake";
			public static readonly string OnEnable = "OnEnable";
			public static readonly string Start = "Start";
			public static readonly string Update = "Update";
			public static readonly string OnDisable = "OnDisable";
			public static readonly string OnDestroy = "OnDestroy";
		};

		//lua路径，不用填缀名，可以是bundle
		[Tooltip("script path")] public string LuaPath;

		//预存函数提高效率
		protected Dictionary<string, LuaFunction> mDictFunc = new Dictionary<string, LuaFunction>();

		//lua表，当gameObject销毁时要释放
		protected LuaTable mSelfTable = null;

		public LuaTable MyTable
		{
			get { return mSelfTable; }
			protected set { mSelfTable = value; }
		}

		//初始化函数，可以被重写，已添加其他
		protected virtual bool Init()
		{
			if (string.IsNullOrEmpty(LuaPath))
			{
				return false;
			}

			object[] luaRet = s_luaState.DoFile(LuaPath);
			if (luaRet == null || luaRet.Length < 1)
			{
				Debug.LogError("Lua must return a table " + LuaPath);
				return false;
			}

			mSelfTable = luaRet[0] as LuaTable;
			if (null == mSelfTable)
			{
				Debug.LogError("null == luaTable  " + LuaPath);
				return false;
			}

			AddFunc(FuncName.Awake);
			AddFunc(FuncName.OnEnable);
			AddFunc(FuncName.Start);
			AddFunc(FuncName.Update);
			AddFunc(FuncName.OnDisable);
			AddFunc(FuncName.OnDestroy);

			return true;
		}

		//保存函数
		protected bool AddFunc(string name)
		{
			var func = mSelfTable.GetLuaFunction(name);
			if (null == func)
			{
				return false;
			}
			mDictFunc.Add(name, func);
			return true;
		}

		//调用函数
		protected void CallLuaFunction(string name, params object[] args)
		{
			if (null == s_luaState)
			{
				return;
			}

			LuaFunction func = null;
			if (mDictFunc.TryGetValue(name, out func))
			{
				func.BeginPCall();
				foreach (var o in args)
				{
					func.Push(o);
				}
				func.PCall();
				func.EndPCall();
			}
		}

		private void Awake()
		{
			Init();
			CallLuaFunction(FuncName.Awake, mSelfTable, gameObject);
		}

		private void OnEnable()
		{
			CallLuaFunction(FuncName.OnEnable, mSelfTable, gameObject);
		}

		private void Start()
		{
			CallLuaFunction(FuncName.Start, mSelfTable, gameObject);
		}

		private void Update()
		{
			CallLuaFunction(FuncName.Update, mSelfTable, gameObject);
		}

		private void OnDisable()
		{
			CallLuaFunction(FuncName.OnDisable, mSelfTable, gameObject);
		}

		private void OnDestroy()
		{
			if (null == s_luaState)
			{
				return;
			}

			CallLuaFunction(FuncName.OnDestroy, mSelfTable, gameObject);

			//记得释放资源
			foreach (var pair in  mDictFunc)
			{
				pair.Value.Dispose();
			}
			mDictFunc.Clear();
			if (null != mSelfTable)
			{
				mSelfTable.Dispose();
				mSelfTable = null;
			}
		}

	}
}