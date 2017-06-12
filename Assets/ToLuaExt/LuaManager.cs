using System.Collections;
using UnityEngine;
using LuaInterface;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToLuaEx
{
	public class LuaManager : MonoBehaviour
	{

		public bool UseBundle = false;

		public LuaState Lua = null;

		public static LuaManager Instance = null;

		private void Awake()
		{
			Instance = this;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		UseBundle = true;
#endif
			LuaFileUtils.Instance.beZip = UseBundle;
		}

		private void Start()
		{


		}

		// Update is called once per frame
		private void Update()
		{
			// 		lua.CheckTop();
			// 		lua.Collect();
		}

		public IEnumerator LoadLuaRes()
		{
			if (!UseBundle)
			{
				yield break;
			}

			var luaBundleLoader = gameObject.GetComponent<LuaBundleLoader>();
			if (null == luaBundleLoader)
			{
				luaBundleLoader = gameObject.AddComponent<LuaBundleLoader>();
			}

			string FirstSearchPath = Application.temporaryCachePath.Replace('\\', '/') + "/MUres/" + LuaConst.osDir;
			string SecondSearchPath = Application.streamingAssetsPath.Replace('\\', '/') + "/" + LuaConst.osDir;


#if UNITY_5
		yield return StartCoroutine(luaBundleLoader.LoadBundles(FirstSearchPath,SecondSearchPath, LuaConst.osDir));
#else

			List<string> list = LuaBundleList;


			/*
		if (Directory.Exists(SecondSearchPath))
		{
			string[] dirs = Directory.GetFiles(SecondSearchPath, "*.unity3d");
			foreach (var dir in dirs)
			{
				list.Add(Path.GetFileName(dir));
			}
		}
		*/


			if (Directory.Exists(FirstSearchPath))
			{
				string[] dirs = Directory.GetFiles(FirstSearchPath, "*.unity3d");
				foreach (var dir in dirs)
				{
					var temp = Path.GetFileName(dir);
					if (!list.Contains(temp))
					{
						list.Add(dir);
					}
				}
			}

			yield return StartCoroutine(luaBundleLoader.LoadBundles(FirstSearchPath, SecondSearchPath, list));
#endif

		}

		public void InitLua()
		{
			Lua = new LuaState();
			OpenLibs(Lua);
			Lua.LuaSetTop(0);
			LuaBinder.Bind(Lua);
			LuaCoroutine.Register(Lua, this);
			Lua.Start();

			var luaLooper = gameObject.AddComponent<LuaLooper>();
			luaLooper.luaState = Lua;
		}

		protected void OpenLibs(LuaState luaState)
		{
			luaState.OpenLibs(LuaDLL.luaopen_pb);
			luaState.OpenLibs(LuaDLL.luaopen_struct);
			luaState.OpenLibs(LuaDLL.luaopen_lpeg);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        luaState.OpenLibs(LuaDLL.luaopen_bit);
#endif
			/*
			if (LuaConst.openLuaSocket)
			{
				OpenLuaSocket();
			}

			if (LuaConst.openZbsDebugger)
			{
				OpenZbsDebugger();
			}
			 * */
			luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
			luaState.OpenLibs(LuaDLL.luaopen_cjson);
			luaState.LuaSetField(-2, "cjson");

			luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
			luaState.LuaSetField(-2, "cjson.safe");    
		}

		private void OnDestroy()
		{
			if (null != Lua)
			{
				Lua.Dispose();
				Lua = null;
			}
			Instance = null;
		}

#if !UNITY_5
		public List<string> LuaBundleList;

#if UNITY_EDITOR
		[ContextMenu("Refresh Lua Bundle List")]
		private void RefreshLuaBundleList()
		{
			//ToLuaMenu.BuildNotJitBundles();
			string SecondSearchPath = Application.streamingAssetsPath.Replace('\\', '/') + "/" + LuaConst.osDir;
			LuaBundleList.Clear();
			if (Directory.Exists(SecondSearchPath))
			{
				string[] dirs = Directory.GetFiles(SecondSearchPath, "*.unity3d");
				foreach (var dir in dirs)
				{
					LuaBundleList.Add(Path.GetFileName(dir));
				}
			}
		}
#endif
#endif
	}
}