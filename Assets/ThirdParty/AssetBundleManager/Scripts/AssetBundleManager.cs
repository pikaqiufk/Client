//--------------------------------------------------------------------------------
/*
 *	@file		AppendFileManager
 *	@brief		AssetBundleManager
 *	@ingroup	AssetBundleManager
 *	@version	1.05
 *	@date		2015.02.28
 *	AssetBundleManager
 */
//--------------------------------------------------------------------------------

// if you don't need log message, comment-out follow line.
#define ASSETBUNDLE_LOG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if !DLL && UNITY_EDITOR
using UnityEditor;
#endif

namespace isotope
{
    using DebugUtility;
    /// <summary>
    /// Assetbundle Manager
    /// </summary>
    public class AssetBundleManager
    {

        public static void ChangeSceneSetting(bool bAdd)
        {
#if !DLL && UNITY_EDITOR
	        List<string> dirs = new List<string>();

            //从新生成scene配置
	        dirs.Add("Assets/Startup.unity");
	        dirs.Add("Assets/Loading.unity");
	        GetDirs(Path.Combine(Application.dataPath, "Res/Scene"), ref dirs);
	        EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[dirs.Count];
	        for (int i = 0; i < newSettings.Length; i++)
	        {
	            newSettings[i] = new EditorBuildSettingsScene(dirs[i], true);
	            if (!bAdd && i>=2)
	            {
	                newSettings[i].enabled = false;
	            }
	        }
	        EditorBuildSettings.scenes = newSettings;

	        Logger.Debug("ChangeSceneSetting");
#endif
        }

        private static void GetDirs(string dirPath, ref List<string> dirs)
        {
            var paths = Directory.GetFiles(dirPath);
            {
                var __array1 = paths;
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var path = (string)__array1[__i1];
                    {
                        if (System.IO.Path.GetExtension(path) == ".unity")
                        {
                            var newpath = path.Replace("\\", "/");
                            dirs.Add(newpath.Substring(newpath.IndexOf("Assets/")));
                        }
                    }
                }
            }
        }

        /// <summary>Platform folder</summary>
        public const string PlatformFolder = "$(Platform)";

        #region Singleton
        /// <summary>Singleton object</summary>
        public static AssetBundleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.LogError("Call CreateInstance() before use \"AssetBundleManager.I\".");
                    _instance = new AssetBundleManager();
                }
                return _instance;
            }

        }
        /// <summary>Destroy singleton object</summary>

        static AssetBundleManager _instance;
        #endregion

        /// <summary>
        /// Check if has loaded assetbundle on url.
        /// </summary>
        /// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
        /// <returns>return true if assetbundle has been loaded</returns>
        public bool HasLoaded(string url)
        {
            return this._assetBundleDic.ContainsKey(url);
        }
        /// <summary>
        /// Loads an AssetBundle with the specified version number from the cache.
        /// refer to UnityEngine.WWW.LoadFromCacheOrDownload(url, version)
        /// </summary>
        /// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
        /// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</param>
        /// <returns>Assetbundle</returns>
        public AssetBundleContainer LoadBundleFromCacheOrDownloadAsync(string url, int version)
        {
            return this.LoadBundleFromCacheOrDownloadAsync(url, version, 0);    // no check-crc 
        }
        /// <summary>
        /// Loads an AssetBundle with the specified version number from the cache.
        /// refer to UnityEngine.WWW.LoadFromCacheOrDownload(url, version, crc)
        /// </summary>
        /// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
        /// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</param>
        /// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
        /// <returns>Assetbundle</returns>
        public AssetBundleContainer LoadBundleFromCacheOrDownloadAsync(string url, int version, uint crc)
        {
            //Debug.Log("LoadBundleAsync " + url);
            if (url != null)
                url = url.Replace(AssetBundleManager.PlatformFolder, AssetBundleManager.GetPlatformFolder(Application.platform));
            Value data;
            if (string.IsNullOrEmpty(url) || !this._assetBundleDic.TryGetValue(url, out data))
            {
                // not find...
                data = new Value();
                data.AssetBundle = new AssetBundleContainer();
                if (!string.IsNullOrEmpty(url))
                {
                    Debug.Log("AssetBundleManager:\n\t" + "LoadBundle " + url);
                    this._assetBundleDic.Add(url, data);
                    ResourceManager.Instance.StartCoroutine(this.LoadAssetBundle(data.AssetBundle, url, version, crc));
                }
            }

            // add counter
            ++data.Counter;
            return data.AssetBundle;
        }

        public AssetBundleContainer LoadBundleFromCacheOrDownloadSync(string url, int version, uint crc)
        {
            //Debug.Log("LoadBundleAsync " + url);
            if (url != null)
                url = url.Replace(AssetBundleManager.PlatformFolder, AssetBundleManager.GetPlatformFolder(Application.platform));
            Value data;
            if (string.IsNullOrEmpty(url) || !this._assetBundleDic.TryGetValue(url, out data))
            {
                // not find...
                data = new Value();
                data.AssetBundle = new AssetBundleContainer();
                if (!string.IsNullOrEmpty(url))
                {
                    Debug.Log("AssetBundleManager:\n\t" + "LoadBundle " + url);
                    this._assetBundleDic.Add(url, data);
                    this.LoadAssetBundleSync(data.AssetBundle, url, version, crc);
                }
            }

            // add counter
            ++data.Counter;
            return data.AssetBundle;
        }

        /// <summary>
        /// Load async assetbundle.
        /// </summary>
        /// <param name="url">Assetbundle URL</param>
        /// <returns>Assetbundle</returns>
        public AssetBundleContainer LoadBundleAsync(string url)
        {
            return this.LoadBundleFromCacheOrDownloadAsync(url, -1, 0);
        }

        public AssetBundleContainer LoadBundleSync(string url)
        {
            return this.LoadBundleFromCacheOrDownloadSync(url, -1, 0);
        }
#if false
		/// <summary>
		/// Load assetbundle.
		/// </summary>
		/// <param name="filename">Assetbundle URL</param>
		/// <returns>Assetbundle</returns>
		public AssetBundleContainer LoadBundle(string filename)
		{
			//Debug.Log("LoadBundle " + filename);
			Value data;
			if (!this._assetBundleDic.TryGetValue(filename, out data))
			{
				Debug.Log("LoadBundle " + data);
				// not find...
				data = new Value();
				data.AssetBundle = base.gameObject.AddComponent<AssetBundleContainer>();
				var bundle = AssetBundle.CreateFromFile(?);
				this._assetBundleDic.Add(filename, data);
				var list = www.assetBundle.Load("list", typeof(BundleAssetList)) as BundleAssetList;
				data.AssetBundle.Initialize(filename, bundle, list.Assets);
			}

			// add counter
			++data.Counter;
			return data.AssetBundle;
		}
#endif

        /// <summary>
        /// Unload assetbundle
        /// </summary>
        /// <param name="assetbundle">Assetbundle</param>
        public void UnloadBundle(AssetBundleContainer assetbundle)
        {
            if (assetbundle != null)
            {
                if (!string.IsNullOrEmpty(assetbundle.Name))
                    this.UnloadBundle(assetbundle.Name);
                else
                {
                    // for debug
                    if (assetbundle.AssetBundle)
                        assetbundle.AssetBundle.Unload(false);
                }
            }
        }
        /// <summary>
        /// Unload assetbundle
        /// </summary>
        /// <param name="filename">Assetbundle URL</param>
        public void UnloadBundle(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && this._assetBundleDic.ContainsKey(filename))
            {
                // not find...
                Value data = this._assetBundleDic[filename];
                if (--data.Counter <= 0)
                {
                    Debug.Log("AssetBundleManager:\n\t" + "Unload " + filename);
                    if (data.AssetBundle != null)
                    {
                        if (data.AssetBundle.AssetBundle)
                            data.AssetBundle.AssetBundle.Unload(false);
                    }
                    this._assetBundleDic.Remove(filename);
                }
            }
        }

#if !NOT_USE_PLATFORM
        /// <summary>
        /// Get platform folder
        /// </summary>
        /// <param name="platform"></param>
        /// <returns>each platform folder</returns>
        public static string GetPlatformFolder(RuntimePlatform platform)
        {
            switch (platform)
            {
#if UNITY_EDITOR
#if !DLL
			case RuntimePlatform.OSXEditor:
				return GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget);
			case RuntimePlatform.WindowsEditor:
				return GetPlatformFolder( EditorUserBuildSettings.activeBuildTarget );
#else
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				{
					// use Reflection for UnityEditor
					var type = Types.GetType("isotope.GetPlatformClass", "Assembly-CSharp-Editor");
					var getPlatform = type.GetMethod("GetPlatform");
					return getPlatform.Invoke(null, null) as string;
				}
#endif
#endif
                case RuntimePlatform.OSXPlayer:
                    return "Standalone";
                case RuntimePlatform.WindowsPlayer:
                    return "Standalone";
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.OSXDashboardPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    return "WebPlayer";
                case RuntimePlatform.IPhonePlayer:
                    return "iPhone";
                case RuntimePlatform.PS3:
                    return "PS3";
                case RuntimePlatform.XBOX360:
                    return "XBOX360";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.LinuxPlayer:
                    return "Standalone";
#if !UNITY_5
                case RuntimePlatform.NaCl:
                    break;
                case RuntimePlatform.FlashPlayer:
                    return "Flash";
#endif
#if UNITY_5
			case RuntimePlatform.WSAPlayerX86:
			case RuntimePlatform.WSAPlayerX64:
			case RuntimePlatform.WSAPlayerARM:
#else
                case RuntimePlatform.MetroPlayerX86:
                case RuntimePlatform.MetroPlayerX64:
                case RuntimePlatform.MetroPlayerARM:
#endif
                case RuntimePlatform.WP8Player:
                    //case RuntimePlatform.BB10Player:
                    break;
#if UNITY_4_5
			case RuntimePlatform.BlackBerryPlayer:
			case RuntimePlatform.TizenPlayer:
			case RuntimePlatform.PSP2:
				break;
			case RuntimePlatform.PS4:
				return "PS4";
			case RuntimePlatform.PSMPlayer:
				return "PSM";
			case RuntimePlatform.XboxOne:
				return "XboxOne";
			case RuntimePlatform.SamsungTVPlayer:
				break;
#endif
            }
            return platform.ToString();
        }
#endif

#if UNITY_EDITOR && !DLL
		/// <summary>
		/// Get platform folder
		/// </summary>
		/// <param name="platform"></param>
		/// <returns>each platform folder</returns>
		public static string GetPlatformFolder(UnityEditor.BuildTarget platform)
		{
			switch (platform)
			{
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneOSXIntel:
				return "Standalone";
			case BuildTarget.StandaloneWindows:
				return "Standalone";
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
				return "WebPlayer";
#if UNITY_5
			case BuildTarget.iOS:
#else
			case BuildTarget.iPhone:
#endif
				return "iPhone";
			case BuildTarget.PS3:
				return "PS3";
			case BuildTarget.XBOX360:
				return "XBOX360";
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.StandaloneGLESEmu:
#if !UNITY_5
			case BuildTarget.NaCl:
#endif
				break;
			case BuildTarget.StandaloneLinux:
				return "Standalone";
#if !UNITY_5
			case BuildTarget.FlashPlayer:
				return "Flash";
#endif
			case BuildTarget.StandaloneWindows64:
				return "Standalone";
#if UNITY_5
			case BuildTarget.WSAPlayer:
#else
			case BuildTarget.MetroPlayer:
#endif
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				return "Standalone";
			case BuildTarget.WP8Player:
			case BuildTarget.StandaloneOSXIntel64:
				return "Standalone";
#if UNITY_4_5
			case BuildTarget.BlackBerry:
			//case BuildTarget.BB10:
			case BuildTarget.Tizen:
			case BuildTarget.PSP2:
			case BuildTarget.PS4:
				return "PS4";
			case BuildTarget.PSM:
				return "PSM";
			case BuildTarget.XboxOne:
				return "XboxOne";
			case BuildTarget.SamsungTV:
				break;
#endif
			}
			return platform.ToString();
		}
#endif
        /// <summary>
        /// Load manage data
        /// </summary>
        /// <param name="loadBackup">if exist, load backup file</param>
        /// <returns></returns>
        public static AssetBundleManageData GetManageData(bool loadBackup)
        {
            AssetBundleManageData abd = null;
#if UNITY_EDITOR
			string path = SettingDataPath;
			if (loadBackup)
			{
				var backupname = SettingBackupDataPath;
				if (File.Exists(Application.dataPath + "/" + backupname))
				{
					abd = Resources.LoadAssetAtPath("Assets/" + backupname, typeof(AssetBundleManageData)) as AssetBundleManageData;
				}
			}
			if (abd == null)
				abd = Resources.LoadAssetAtPath("Assets/" + path, typeof(AssetBundleManageData)) as AssetBundleManageData;
#endif
            return abd;
        }
#if UNITY_EDITOR
		/// <summary>設定データパス</summary>
		public static string SettingDataPath
		{
			get
			{
				var path = GetPrefsString(SettingDataKey);
				if (string.IsNullOrEmpty(path) || !File.Exists(Application.dataPath + "/" + path))
				{
					// find directory "AssetBundleManager"
					var find = FindDirectory(Application.dataPath, "AssetBundleManager");
					if (find == null)
						find = "Assets/";	// project root directory
					find = find.Replace('\\', '/');
					path = find.Substring(find.IndexOf("Assets/") + "Assets/".Length) + "/AssetBundleManagerSetting.asset";
					SetPrefsString(AssetBundleManager.SettingDataKey, path);
				}
				return path;
			}
		}
		/// <summary>設定バックアップデータパス</summary>
		public static string SettingBackupDataPath
		{
			get
			{
				var path = SettingDataPath;
				return path.Substring(0, path.LastIndexOf('.')) + "_backup.asset";
			}
		}

		static string GetPrefsString(string name)
		{
#if !DLL
			return EditorPrefs.GetString(SettingDataKey);
#else
			return UnityEngine.PlayerPrefs.GetString(SettingDataKey);
#endif
		}
		static void SetPrefsString(string name, string value)
		{
#if !DLL
			EditorPrefs.SetString(SettingDataKey, value);
#else
			UnityEngine.PlayerPrefs.SetString(SettingDataKey, value);
#endif
		}

		const string SettingDataKey = "AssetBundleManagerDataDirectory";
#endif

        void LoadAssetBundleSync(AssetBundleContainer bundle, string url, int version, uint crc)
        {

            var realpath = Path.GetDirectoryName(url) + "/" + url.Substring(AssetBundleLoader.GetStreamingAssetsURL("").Length).Replace('/', '_');

// #if UNITY_ANDROID && !UNITY_EDITOR
//             //GetNextEntry    assets/Controller/MainPlayer.unity3d
//             //url             jar:file:///data/app/com.base.mayatest-1.apk!/assets/Controller/MainPlayer.unity3d
// 
//             var apkPath = Application.dataPath;
//             var filepath = realpath.Substring(realpath.IndexOf("assets/"));
//             var filestream = new FileStream(apkPath, FileMode.Open, FileAccess.Read);
//             var zipfile = new ZipFile(filestream);
//             var item = zipfile.GetEntry(filepath);
//             if (null != item)
//             {
//                 var stream = zipfile.GetInputStream(item);
//                 byte[] buffer = new byte[stream.Length];
//                 stream.Read(buffer, 0, buffer.Length);
//                 AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(buffer);
//                 BundleAssetList list = null;
//                 list = assetBundle.Load("list", typeof(BundleAssetList)) as BundleAssetList;
//                 bundle.Initialize(realpath, assetBundle, list ? list.Assets : null);
//             }
//             else
//             {
//                 Logger.Error("get file from apk error !:" + realpath);
//             }
// #else
            // path = Application.dataPath + "/Raw";  IOS
            // path = Application.dataPath + "/StreamingAssets"; PC
            var path = realpath.Substring(realpath.IndexOf("file://") + 7);
            if (File.Exists(path))
            {
                var stream = File.OpenRead(path);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(buffer);
                BundleAssetList list = null;
                list = assetBundle.Load("list", typeof(BundleAssetList)) as BundleAssetList;
                bundle.Initialize(realpath, assetBundle, list ? list.Assets : null);
            }
            else
            {
                Logger.Error("get file error !:" + path);
            }
//#endif
        }
        IEnumerator LoadAssetBundle(AssetBundleContainer bundle, string url, int version, uint crc)
        {
            WWW www = null;

            var path = Path.Combine(Path.GetDirectoryName(url), url.Substring(AssetBundleLoader.GetStreamingAssetsURL("").Length).Replace('/', '_'));
            path = path.Replace("\\", "/");

            if (0 <= version)
                www = WWW.LoadFromCacheOrDownload(path, version, crc);
            else
                www = new WWW(path);
            yield return www;

            if (www.error != null)
            {
                Debug.Log(string.Format("AssetBundleManager:\n\tload \"{0}\" error:{1}", path, www.error));
                bundle.Initialize(path, null, null);
                bundle.SetError(www.error);
            }
            else
            {
                BundleAssetList list = null;
                if (Application.HasProLicense())
                {
#if UNITY_5
					var request = www.assetBundle.LoadAssetAsync<BundleAssetList>( "list" );
#else
                    var request = www.assetBundle.LoadAsync("list", typeof(BundleAssetList));
#endif
                    yield return request;
                    list = request.asset as BundleAssetList;
                }
                else
                {
#if UNITY_5
					list = www.assetBundle.LoadAsset<BundleAssetList>( "list" );
#else
                    list = www.assetBundle.Load("list", typeof(BundleAssetList)) as BundleAssetList;
#endif
                }
                if (list)
                    bundle.Initialize(path, www.assetBundle, list.Assets);
                else
                    //Debug.LogWarning( "There is no BundleAssetList in " + www.url );
                    bundle.Initialize(path, www.assetBundle, null);
            }
            if (www != null)
                www.Dispose();
        }

        // Value for dictionary
        class Value
        {
            public AssetBundleContainer AssetBundle { get; set; }
            public int Counter { get; set; }
        }
        Dictionary<string, Value> _assetBundleDic = new Dictionary<string, Value>();

#if UNITY_EDITOR
		// Find directory from path
		static string FindDirectory(string path, string find)
		{
			var directories = Directory.GetDirectories(path);
			foreach (var f in directories)
			{
				if (Path.GetFileName(f) == find)
					return f;
			}
			foreach (var f in directories)
			{
				var ret = FindDirectory(f, find);
				if (ret != null)
					return ret;
			}
			return null;
		}	
#endif
    }

    namespace DebugUtility
    {
        class Debug
        {
            //[System.Diagnostics.Conditional("ASSETBUNDLE_LOG")]
            public static void Log(string format, params object[] args)
            {
#if ASSETBUNDLE_LOG
                UnityEngine.Debug.Log(string.Format(format, args));
#endif
            }
            //[System.Diagnostics.Conditional("ASSETBUNDLE_LOG")]
            public static void LogError(string format, params object[] args)
            {
#if ASSETBUNDLE_LOG
                UnityEngine.Debug.LogError(string.Format(format, args));
#endif
            }
            //[System.Diagnostics.Conditional("ASSETBUNDLE_LOG")]
            public static void LogWarning(string format, params object[] args)
            {
#if ASSETBUNDLE_LOG
                UnityEngine.Debug.LogWarning(string.Format(format, args));
#endif
            }
        }
    }
}
