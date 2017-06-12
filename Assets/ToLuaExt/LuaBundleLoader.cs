using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuaInterface;

namespace ToLuaEx
{
	public class LuaBundleLoader : MonoBehaviour
	{

		public delegate void DelegateLoading(int idx, int total, string bundleName, string path);

		public delegate void DelegateLoadOver();

		//正在加载中回掉
		public DelegateLoading OnLoading;

		//加载完成回掉
		public DelegateLoadOver OnLoadOver;

		//总共要加载的bundle个数
		private int mTotalBundleCount = 0;

		//当前已加载的bundle个数
		private int mBundleCount = 0;

#if UNITY_5
	public void LoadBundle(string dir1,string dir2, string bundleName)
	{
		StartCoroutine(LoadBundles(dir1,dir2, bundleName));
	}
#else
		public void LoadBundle(string dir1, string dir2, List<string> bundleList)
		{
			StartCoroutine(LoadBundles(dir1, dir2, bundleList));
		}
#endif

		private IEnumerator CoLoadBundle(string name, string path)
		{
			using (WWW www = new WWW(path))
			{
				if (www == null)
				{
					Debugger.LogError(name + " bundle not exists");
					yield break;
				}

				yield return www;

				if (www.error != null)
				{
					Debugger.LogError(string.Format("Read {0} failed: {1}", path, www.error));
					yield break;
				}

				mBundleCount++;
				LuaFileUtils.Instance.AddSearchBundle(name, www.assetBundle);

				try
				{
					if (null != OnLoading)
					{
						OnLoading(mBundleCount, mTotalBundleCount, name, path);
					}
					Debug.Log("LuaBundleLoader[" + mBundleCount.ToString() + "][" + path + "]");
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}


				www.Dispose();
			}
		}


#if UNITY_5
	private IEnumerator LoadBundles(string dir1, string dir2,string bundleName)		        
#else
		public IEnumerator LoadBundles(string dir1, string dir2, List<string> bundleList)
#endif
		{

			List<string> list = new List<string>();

#if UNITY_5
		
		var bundlePath = dir1+"/"+bundleName;
		if (!File.Exists(bundlePath))
		{
			bundlePath = dir2 + "/" + bundleName;
		}
		else
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			bundlePath = "file:///" + bundlePath;
#endif
		}
#if UNITY_ANDROID && !UNITY_EDITOR
        
#else
		bundlePath = "file:///" + bundlePath;
#endif
		using (WWW www = new WWW(bundlePath))
		{
			yield return www;

			AssetBundleManifest manifest = (AssetBundleManifest)www.assetBundle.LoadAsset("AssetBundleManifest");
			list = new List<string>(manifest.GetAllAssetBundles());
			//www.assetBundle.Unload(true);
			www.Dispose();
		}
#else
			list = bundleList;
#endif
			mTotalBundleCount = list.Count;

			for (int i = 0; i < list.Count; i++)
			{
				string str = list[i];

				string path = dir1 + "/" + str;
				if (!File.Exists(path))
				{
					path = dir2 + "/" + str;
				}
				else
				{
#if UNITY_ANDROID && !UNITY_EDITOR
				path = "file:///" + path;
#endif
				}
#if UNITY_ANDROID && !UNITY_EDITOR
			
#else
				path = "file:///" + path;
#endif
				string name = Path.GetFileNameWithoutExtension(str);
				StartCoroutine(CoLoadBundle(name, path));
			}

			yield return StartCoroutine(CheckLoadFinish());
		}

		private IEnumerator CheckLoadFinish()
		{
			while (mBundleCount < mTotalBundleCount)
			{
				yield return null;
			}

			if (null != OnLoadOver)
			{
				try
				{
					OnLoadOver();
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}

			}
		}

	}
}