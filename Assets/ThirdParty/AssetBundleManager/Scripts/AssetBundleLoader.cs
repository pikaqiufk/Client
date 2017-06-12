//--------------------------------------------------------------------------------
/*
 *	@file		AppendFileLoader
 *	@brief		Assetbundle loader
 *	@ingroup	AssetBundleManager
 *	@version	1.05
 *	@date		2015.02.28
 *	Assetbundle loader
 *	
 */
//--------------------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.IO;
using Antlr.Runtime;
using Assets.Script.Utility;
using Object = UnityEngine.Object;

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// Assetbundle loader
	/// </summary>
	public class AssetBundleLoader
	{
		/// <summary>AssetBundle URL</summary>
		public string URL
		{
			get
			{
				string path = this._path;
#if !NOT_USE_PLATFORM
				if (this.PlatformFolder && !string.IsNullOrEmpty(path))
				{
					path = path.Replace(AssetBundleManager.PlatformFolder, AssetBundleManager.GetPlatformFolder(Application.platform));
				}
#endif
				if (this._atStreamingAssets)
					return GetStreamingAssetsURL(path);
				else
					return path;
			}
		}
		/// <summary>
		/// AssetBundle Path
		/// </summary>
		public string Path{get { return this._path; }}
		/// <summary>AssetBundle</summary>
		public AssetBundleContainer AssetBundle { get { return this._assetbundle; } set { this._assetbundle = value; } }
		/// <summary>AssetBundleName</summary>
		public string AssetBundleName { get { return this._assetbundleName; } set { this._assetbundleName = value; } }
		/// <summary>AssetName</summary>
		public string AssetName { get { return this._assetName; } set { this._assetName = value; } }

		/// <summary>If true, add each platform folder to path.</summary>
		public bool PlatformFolder { get { return this._platformFolder; } set { this._platformFolder = value; } }

		/// <summary>If true, instantiate asset after load.</summary>
		public bool InstantiateAfterLoad { get { return this._instantiateAfterLoad; } set { this._instantiateAfterLoad = value; } }
		/// <summary>Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</summary>
		public int Version { get { return this._version; } set { this._version = value; } }
		/// <summary>An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</summary>
		public uint CRC
		{
			get
			{
				uint crc;
				if( uint.TryParse(this._crc, out crc) )
					return crc;
				return 0;
			}
			set { this._crc = value.ToString(); }
		}

		/// <summary>Return true while load assetbundle.</summary>
		public bool IsLoading { get; protected set; }
		/// <summary>Return true if finished to load assetbundle.</summary>
		public bool IsLoaded { get; protected set; }
		/// <summary>Loaded asset</summary>
		public Object Asset { get; protected set; }

		/// <summary>Return true while load assetbundle.</summary>
		public bool IsError
		{
			get
			{
				if( this.AssetBundle != null)
					return this.AssetBundle.IsError;
				return false;
			}
		}
		/// <summary>Error message.</summary>
		public string ErrorMsg
		{
			get
			{
				if( this.AssetBundle != null )
					return this.AssetBundle.ErrorMsg;
				return null;
			}
		}


		/// <summary>
		/// Set assetbundle URL
		/// </summary>
		/// <param name="url">URL</param>
		public void SetURL(string url)
		{
			//Debug.Log("SetURL(" + url + ")");
			this._path = url;
			this._atStreamingAssets = false;
		}
		/// <summary>
		/// Set assetbundle path (belong StreamingAssets)
		/// </summary>
		/// <param name="path">path</param>
		public void SetStreamingAssetsPath(string path)
		{
			//Debug.Log("SetStreamingAssetsPath(" + path + ")");
			this._path = path.Replace('\\', '/');
			this._atStreamingAssets = true;
		}

		/// <summary>
		/// Load assetbundle
		/// </summary>
		public void Load()
		{
			if( this._path != null )
			{
				this.IsLoaded = false;
				this.IsLoading = true;
                ResourceManager.Instance.StartCoroutine(this.LoadAssetBundle(this.URL, -1, 0));
			}
		}

	    public void Loadsync()
	    {
            if (this._path != null)
            {
                this.IsLoaded = false;
                this.IsLoading = true;
                this.LoadAssetBundleSync(this.URL, -1, 0);
            }
	    }
		/// <summary>
		/// Loads an AssetBundle with the specified version number from the cache.
		/// refer to UnityEngine.WWW.LoadFromCacheOrDownload(url, version)
		/// </summary>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</param>
		/// <returns>Assetbundle</returns>
		public void LoadFromCacheOrDownload(int version)
		{
			this.LoadFromCacheOrDownload(version, 0);	// no check-crc 
		}
		/// <summary>
		/// Loads an AssetBundle with the specified version number from the cache.
		/// refer to UnityEngine.WWW.LoadFromCacheOrDownload(url, version, crc)
		/// </summary>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</param>
		/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
		/// <returns>Assetbundle</returns>
		public void LoadFromCacheOrDownload(int version, uint crc)
		{
			if( this._path != null )
			{
				this.IsLoading = true;
                ResourceManager.Instance.StartCoroutine(this.LoadAssetBundle(this.URL, version, crc));
			}
		}

		/// <summary>
		/// make compatible old version
		/// </summary>
		public void MakeCompatibleOldVersion()
		{
			// compatible old version
			if(this.PlatformFolder && !this.Path.Contains( AssetBundleManager.PlatformFolder ))
			{
				int idx = this.Path.LastIndexOf( '/' );
				var dir = this.Path.Substring( 0, idx + 1 );
				var file = this.Path.Substring( idx + 1 );
				this._path = dir + AssetBundleManager.PlatformFolder + "/" + file;
			}
		}

// 		void Awake()
// 		{
// 			this.MakeCompatibleOldVersion();
// 		}
        public AssetBundleLoader() { this.MakeCompatibleOldVersion();}

		public void Dispose()
		{
			//Debug.LogWarning("\t" + this.AssetBundle + "(" + (this.AssetBundle ? "="+this.AssetBundle.AssetBundle : "none") + ")" + ", " + this.useAssetbundle + ", " + (this.AssetBundle != null ? this.AssetBundle.Name : "null"));
			if( this.AssetBundle != null )
			{
#if UNITY_EDITOR
				if( this.useAssetbundle )
					AssetBundleManager.Instance.UnloadBundle(this.AssetBundle);
				else
				{
					if( this.AssetBundle.AssetBundle )
						this.AssetBundle.AssetBundle.Unload(false);

				}
#else
				AssetBundleManager.Instance.UnloadBundle( this.AssetBundle );
#endif
				this.AssetBundle = null;
#if UNITY_EDITOR
				foreach( var bundle in this.parentListOfStreamdSceneAssetBundleLoaded )
					AssetBundleManager.Instance.UnloadBundle(bundle);
#endif
			}
		}

		IEnumerator LoadAssetBundle(string url, int version, uint crc)
		{
			//Debug.Log("LoadAssetBundle " + url);
#if UNITY_EDITOR
			this.useAssetbundle = true;
#else
			bool useAssetbundle = true;
#endif

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				var data = AssetBundleManager.GetManageData(true);
				if (data)
				{
					if( data.notUseBundleOnEditor )
					{
						// load asset from AssetDatabase
						useAssetbundle = false;

						string sceneName = url.Substring(url.LastIndexOf('/') + 1);
                        var fullname = url.Substring(url.IndexOf("Assets/"));
                        var bundle = data.assetbundles.Find(ab => fullname.CompareTo(ab.Path) == 0);
					    string orginFilePath = null;
						if( bundle == null )
						{
							bundle = data.assetbundles.Find(ab => {
								if( ab.Separated )
								{
									foreach( var path in ab.GetAllContentsPath() )
									{
									    var unExtPath = System.IO.Path.GetDirectoryName(path) +"/" + System.IO.Path.GetFileNameWithoutExtension(path) + ".unity3d";
                                        unExtPath = unExtPath.Replace("Res", "BundleAsset");
                                        if (fullname.CompareTo(unExtPath) == 0)
                                        {
                                            orginFilePath = path;
											return true;
										}
									}
								}
								return false;
							});
						}
						if( bundle != null && orginFilePath!= null)
						{
							if( !bundle.ForStreamedScene )
							{
								// Normal AssetBundle
								if( !string.IsNullOrEmpty(this.AssetName) )
								{
								    yield return new WaitForEndOfFrame();
									this.Asset = Resources.LoadAssetAtPath(orginFilePath, typeof(UnityEngine.Object));
								}
							}
							else
							{
								// StreamedSceneAssetBundle
#if UNITY_EDITOR
#if !DLL
								// check if the scene is contained within "Scene in Builds" on BuildSettings.
								var scenes = UnityEditor.EditorBuildSettings.scenes;
								if( System.Array.Find(scenes, scene => scene.path == sceneName) == null )
								{
									// not be contained...
									useAssetbundle = true;
									// with follow way, scene is enable at next play.
									//System.Array.Resize(ref scenes, scenes.Length + 1);
									//scenes[scenes.Length - 1] = new UnityEditor.EditorBuildSettingsScene(s, true);
									//UnityEditor.EditorBuildSettings.scenes = scenes;
								}
#else
								useAssetbundle = true;
#endif
								if( useAssetbundle )
								{
									Debug.LogWarning(string.Format(@"AssetBundleManager:
		You can't LoadLevel the scene that is not contained within UnityEditor.EditorBuildSettings.scenes without assetbundle.
		So ""{0}"" is loaded from assetbundle.
		(ignored setting ""Not load on assetbundle on editor"")", System.IO.Path.GetFileNameWithoutExtension(sceneName)));
									// need to load parent assetbundle
									var bundlework = bundle;
									System.Collections.Generic.Stack<AssetBundleData> bundleStack = new System.Collections.Generic.Stack<AssetBundleData>();
									while( 0 <= bundlework.ParentNo )
									{
										bundlework = data.assetbundles[bundlework.ParentNo];
										bundleStack.Push(bundlework);
									}
									while( 0 < bundleStack.Count )
									{
										bundlework = bundleStack.Pop();
										Debug.Log("AssetBundleManager:\n\tLoad parent assetbundle(" + bundlework.Path + ") for loading " + url + ".");
										var loader = AssetBundleManager.Instance.LoadBundleAsync("file://" + Application.dataPath + "/../" + bundlework.Path);
										this.parentListOfStreamdSceneAssetBundleLoaded.Add(loader);
										while( !loader.IsReady )
											yield return 0;
									}
								}
#endif
							}
							if( !useAssetbundle )
							{
								// if not use assetbundle, create dammy AssetBundleContainer
								var prev = this.AssetBundle;
								this.AssetBundle = AssetBundleManager.Instance.LoadBundleAsync(null);	// dammy
								if( prev != null )
									AssetBundleManager.Instance.UnloadBundle(prev);
								this.AssetBundle.Initialize(url, null, null);
							}
						}
					}
				}
			}
#endif

			if( useAssetbundle )
			{
				// load from AssetBundle
				var prev = this.AssetBundle;
				this.AssetBundle = AssetBundleManager.Instance.LoadBundleFromCacheOrDownloadAsync(url, version, crc);
				if( prev != null )
					AssetBundleManager.Instance.UnloadBundle(prev);

				while( !this.AssetBundle.IsReady )
					yield return 0;

				if( !this.AssetBundle.IsError )
				{
					if( !string.IsNullOrEmpty(this.AssetName) )
					{
						// find asset type
						System.Type type = typeof(Object);
						// load async

						string assetname = this.AssetName;
						int extidx = assetname.LastIndexOf('.');
						if( 0 < extidx )
						{
                            type = GetType(assetname.Substring(extidx + 1));
							assetname = assetname.Substring(0, extidx);
						}
						if( UnityEngine.Application.HasProLicense() )
						{
							// pro license
#if UNITY_5
							var request = this.AssetBundle.AssetBundle.LoadAssetAsync(assetname, type);
#else
							var request = this.AssetBundle.AssetBundle.LoadAsync(assetname, type);
#endif
							while( !request.isDone )
								yield return 0;
							this.Asset = request.asset;	// If AssetName is .unity(Scene Asset), request takes no time and this.Asset is null. 


						    if (this.Asset is GameObject)
						    {
						        var obj = this.Asset as GameObject;
                                Logger.Debug("changeShader() url =" + url);
						        ResourceManager.ChangeShader(obj.transform);
						    }

						}
						else
						{
							// free license
#if UNITY_5
							this.Asset = this.AssetBundle.AssetBundle.LoadAsset(assetname, type);
#else
							this.Asset = this.AssetBundle.AssetBundle.Load(assetname, type);
#endif
						}
					}
				}
			}

			if( this.InstantiateAfterLoad )
			{
				if( this.Asset is GameObject )
				{
					// instantiate prefab
					var obj = Object.Instantiate(this.Asset) as GameObject;
					Vector3 wpos = obj.transform.localPosition;
					Vector3 wscale = obj.transform.localScale;
					Quaternion wq = obj.transform.localRotation;
					obj.transform.parent = ResourceManager.sGameObject.transform;
					obj.transform.localPosition = wpos;
					obj.transform.localScale = wscale;
					obj.transform.localRotation = wq;
				}
				else
				{
					int idx = this.AssetName.IndexOf(".unity");
					if( 0 <= idx )
					{
						// load scene
						Application.LoadLevel(this.AssetName.Substring(0, idx));
					}
				}
			}
			this.IsLoading = false;
			this.IsLoaded = true;
			yield break;
		}

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        void LoadAssetBundleSync(string url, int version, uint crc)
        {
            //Debug.Log("LoadAssetBundle " + url);
#if UNITY_EDITOR
            this.useAssetbundle = true;
#else
			bool useAssetbundle = true;
#endif

#if UNITY_EDITOR
            if (Application.isEditor)
            {
                var data = AssetBundleManager.GetManageData(true);
                if (data)
                {
                    if (data.notUseBundleOnEditor)
                    {
                        // load asset from AssetDatabase
                        useAssetbundle = false;

                        string sceneName = url.Substring(url.LastIndexOf('/') + 1);
                        var fullname = url.Substring(url.IndexOf("Assets/"));
                        var bundle = data.assetbundles.Find(ab => fullname.CompareTo(ab.Path) == 0);
                        string orginFilePath = null;
                        if (bundle == null)
                        {
                            bundle = data.assetbundles.Find(ab =>
                            {
                                if (ab.Separated)
                                {
                                    foreach (var path in ab.GetAllContentsPath())
                                    {
                                        var unExtPath = System.IO.Path.GetDirectoryName(path) + "/" + System.IO.Path.GetFileNameWithoutExtension(path) + ".unity3d";
                                        unExtPath = unExtPath.Replace("Res", "BundleAsset");
                                        if (fullname.CompareTo(unExtPath) == 0)
                                        {
                                            orginFilePath = path;
                                            return true;
                                        }
                                    }
                                }
                                return false;
                            });
                        }
                        if (bundle != null && orginFilePath != null)
                        {
                            if (!bundle.ForStreamedScene)
                            {
                                // Normal AssetBundle
                                if (!string.IsNullOrEmpty(this.AssetName))
                                {
                                    this.Asset = Resources.LoadAssetAtPath(orginFilePath, typeof(UnityEngine.Object));
                                }
                            }
                            else
                            {
                                // StreamedSceneAssetBundle
#if UNITY_EDITOR
#if !DLL
                                // check if the scene is contained within "Scene in Builds" on BuildSettings.
                                var scenes = UnityEditor.EditorBuildSettings.scenes;
                                if (System.Array.Find(scenes, scene => scene.path == sceneName) == null)
                                {
                                    // not be contained...
                                    useAssetbundle = true;
                                    // with follow way, scene is enable at next play.
                                    //System.Array.Resize(ref scenes, scenes.Length + 1);
                                    //scenes[scenes.Length - 1] = new UnityEditor.EditorBuildSettingsScene(s, true);
                                    //UnityEditor.EditorBuildSettings.scenes = scenes;
                                }
#else
								useAssetbundle = true;
#endif
                                if (useAssetbundle)
                                {
                                    Debug.LogWarning(string.Format(@"AssetBundleManager:
		You can't LoadLevel the scene that is not contained within UnityEditor.EditorBuildSettings.scenes without assetbundle.
		So ""{0}"" is loaded from assetbundle.
		(ignored setting ""Not load on assetbundle on editor"")", System.IO.Path.GetFileNameWithoutExtension(sceneName)));
                                    // need to load parent assetbundle
                                    var bundlework = bundle;
                                    System.Collections.Generic.Stack<AssetBundleData> bundleStack = new System.Collections.Generic.Stack<AssetBundleData>();
                                    while (0 <= bundlework.ParentNo)
                                    {
                                        bundlework = data.assetbundles[bundlework.ParentNo];
                                        bundleStack.Push(bundlework);
                                    }
                                    while (0 < bundleStack.Count)
                                    {
                                        bundlework = bundleStack.Pop();
                                        Debug.Log("AssetBundleManager:\n\tLoad parent assetbundle(" + bundlework.Path + ") for loading " + url + ".");
                                        var loader = AssetBundleManager.Instance.LoadBundleSync("file://" + Application.dataPath + "/../" + bundlework.Path);
                                        this.parentListOfStreamdSceneAssetBundleLoaded.Add(loader);
                                    }
                                }
#endif
                            }
                            if (!useAssetbundle)
                            {
                                // if not use assetbundle, create dammy AssetBundleContainer
                                var prev = this.AssetBundle;
                                this.AssetBundle = AssetBundleManager.Instance.LoadBundleSync(null);	// dammy
                                if (prev != null)
                                    AssetBundleManager.Instance.UnloadBundle(prev);
                                this.AssetBundle.Initialize(url, null, null);
                            }
                        }
                    }
                }
            }
#endif

            if (useAssetbundle)
            {
                // load from AssetBundle
                var prev = this.AssetBundle;
                this.AssetBundle = AssetBundleManager.Instance.LoadBundleFromCacheOrDownloadSync(url, version, crc);
                if (prev != null)
                    AssetBundleManager.Instance.UnloadBundle(prev);
                if (!this.AssetBundle.IsError)
                {
                    if (!string.IsNullOrEmpty(this.AssetName))
                    {
                        // find asset type
                        System.Type type = typeof(Object);
                        // load async
                        string assetname = this.AssetName;
                        int extidx = assetname.LastIndexOf('.');
                        if (0 < extidx)
                        {
                            type = GetType(assetname.Substring(extidx + 1));
                            assetname = assetname.Substring(0, extidx);
                        }
                        if (UnityEngine.Application.HasProLicense())
                        {
                            // pro license
#if UNITY_5
							var request = this.AssetBundle.AssetBundle.LoadAssetsync(assetname, type);
#else
                            var asset = this.AssetBundle.AssetBundle.Load(assetname, type);
                            if (this.Asset is GameObject)
                            {
                                var obj = this.Asset as GameObject;
                                ResourceManager.ChangeShader(obj.transform);
                            }
#endif
                            this.Asset = asset;	// If AssetName is .unity(Scene Asset), request takes no time and this.Asset is null. 
                        }
                        else
                        {
                            // free license
#if UNITY_5
							this.Asset = this.AssetBundle.AssetBundle.LoadAsset(assetname, type);
#else
                            this.Asset = this.AssetBundle.AssetBundle.Load(assetname, type);
#endif
                        }
                    }
                }
            }

            if (this.InstantiateAfterLoad)
            {
                if (this.Asset is GameObject)
                {
                    // instantiate prefab
                    var obj = Object.Instantiate(this.Asset) as GameObject;
                    Vector3 wpos = obj.transform.localPosition;
                    Vector3 wscale = obj.transform.localScale;
                    Quaternion wq = obj.transform.localRotation;
                    obj.transform.parent = ResourceManager.sGameObject.transform;
                    obj.transform.localPosition = wpos;
                    obj.transform.localScale = wscale;
                    obj.transform.localRotation = wq;
                }
                else
                {
                    int idx = this.AssetName.IndexOf(".unity");
                    if (0 <= idx)
                    {
                        // load scene
                        Application.LoadLevel(this.AssetName.Substring(0, idx));
                    }
                }
            }
            this.IsLoading = false;
            this.IsLoaded = true;
        }


		public static string GetStreamingAssetsURL(string path)
		{
#if UNITY_EDITOR
            return String.Format("file://{0}/{1}", Application.dataPath + "/BundleAsset", path);
#endif
			if( Application.platform == RuntimePlatform.Android )
			{
				// include "jar:file://" in Application.streamingAssetsPath
				return string.Format("{0}/{1}", Application.streamingAssetsPath, path);
			}
			return string.Format("file://{0}/{1}", Application.streamingAssetsPath, path);
		}

	    private static System.Type GetType(string ext)
	    {
	        switch (ext)
	        {
	            case "prefab":
	                return typeof (GameObject);
	            case "png":
	            case "jpg":
	            case "bmp":
	            case "tga":
	                return typeof (Texture);
	            case "mat":
	                return typeof (Material);
	            case "txt":
	                return typeof (TextAsset);
	            case "mp3":
	            case "wav":
	                return typeof (AudioClip);
	        }
	        return typeof (Object);
	    }

	    [SerializeField]
		string _path;
		[SerializeField]
		string _assetbundleName;
		[SerializeField]
		string _assetName;
		[SerializeField]
		bool _atStreamingAssets;
		[SerializeField]
		bool _platformFolder;
		[SerializeField]
		bool _instantiateAfterLoad;
		[SerializeField, UnityEngine.Range(0, int.MaxValue)]
		int _version = -1;
		[SerializeField]
		string _crc;

		AssetBundleContainer _assetbundle;

#if UNITY_EDITOR
		// list of parent assetbundle loaded for StreamedSceneAssetBundle.
		System.Collections.Generic.List<AssetBundleContainer> parentListOfStreamdSceneAssetBundleLoaded = new System.Collections.Generic.List<AssetBundleContainer>();
		// if load from assetbundle, true
		bool useAssetbundle;
#endif
	}
}
