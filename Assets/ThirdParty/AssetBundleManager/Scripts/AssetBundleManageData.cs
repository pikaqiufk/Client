//--------------------------------------------------------------------------------
/*
 *	@file		AssetBundleManagerData
 *	@brief		
 *	@ingroup	
 *	@version	1.00
 *	@date		2014
 *	AssetBUndleManager data
 *	
 */
//--------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

#if !DLL && UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
using Microsoft.Win32;
#endif

namespace isotope
{
    using DebugUtility;

    /// <summary>
    /// AssetBundle management data
    /// </summary>
    public class AssetBundleManageData : ScriptableObject
    {
#if UNITY_EDITOR
	    public AssetBundleManageData()
	    {
            RegistryKey key = Registry.CurrentUser;
            RegistryKey software;
            software = key.OpenSubKey("software\\Spartacus", true);
            if (software == null)
            {
                software = key.CreateSubKey("software\\Spartacus");
            }
            var path = software.GetValue("NotUseBundleOnEditor");

	        if (path == null)
	        {
	            notUseBundleOnEditor = false;
                software.SetValue("NotUseBundleOnEditor", false);
	        }
	        else
	        {
	            notUseBundleOnEditor = bool.Parse(path.ToString().ToLower());
	        }

            key.Close();
	    }

#endif

        /// <summary>AssetBundle List</summary>
        public List<AssetBundleData> assetbundles = new List<AssetBundleData>();

        /// <summary>If true, load asset from AssetDatabase</summary>

        private bool _notUseBundleOnEditor = false;

        public bool notUseBundleOnEditor
        {
            get { return _notUseBundleOnEditor; }
            set
            {
                if (_notUseBundleOnEditor != value)
                {
#if UNITY_EDITOR
                    RegistryKey key = Registry.CurrentUser;
                    RegistryKey software;
                    software = key.OpenSubKey("software\\Spartacus", true);
                    if (software == null)
                    {
                        software = key.CreateSubKey("software\\Spartacus");
                    }

                    software.SetValue("NotUseBundleOnEditor", value);
                    _notUseBundleOnEditor = value;

                    key.Close();
#endif
                    AssetBundleManager.ChangeSceneSetting(value);
                }
            }
        }

        /// <summary>MultiPlatform</summary>
        public bool multiPlatform;
#if !DLL && UNITY_EDITOR
		/// <summary>BuildTarget list</summary>
		public List<BuildTarget> targets = new List<BuildTarget>();
#endif
    }
    /// <summary>
    /// AssetBundle data
    /// </summary>
    [System.Serializable]
    public class AssetBundleData
    {
        /// <summary>Constructor</summary>
        /// <param name="dir">directory name of assetbundle</param>
        /// <param name="file">file name of assetbundle</param>
        public AssetBundleData(string dir, string file)
        {
            this._directory = dir.Replace('\\', '/');
            this._file = file;
        }
        /// <summary>Target directory</summary>
        public string Directory { get { return this._directory; } protected set { this._directory = value; } }
        /// <summary>File Name</summary>
        public string File { get { return this._file; } protected set { this._file = value; } }
        /// <summary>Target path</summary>
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(this.Directory))
                    return this.File;
                else
                    return this.Directory + '/' + this.File;
            }
        }
        /// <summary>Files make each assetbundle</summary>
        public bool Separated { get { return this._separated; } set { this._separated = value; } }
        /// <summary>Assetbundle for streamed scene</summary>
        public bool ForStreamedScene { get { return this._streamedScene; } set { this._streamedScene = value; } }
        /// <summary>Parent assetbundle. Use BuildPipeline.Push(Pop)AssetDependencies.</summary>
        public int ParentNo { get { return this._parent; } set { this._parent = value; } }
        /// <summary>Create each platform folder.</summary>
        public bool PlatformFolder { get { return this._platformFolder; } set { this._platformFolder = value; } }
#if false//!DLL && UNITY_EDITOR
		/// <summary>BuildAssetBundleOption</summary>
		public BuildAssetBundleOptions Options { get { return this._options; } set { this._options = value; } }
#else
        /// <summary>BuildAssetBundleOption</summary>
        public int Options { get { return this._options; } set { this._options = value; } }
#endif
        /// <summary>changed.</summary>
        public bool Changed { get { return this._changed; } set { this._changed = value; } }

        public int FileTypeOptions { get { return this._fileTypeOptions; } set { this._fileTypeOptions = value; } }

        public int DependTimes { get { return this._dependtimes; } set { this._dependtimes = value; } }

        public string AddFilter
        {
            get { return this._addFilter; }
            set { this._addFilter = value; }
        }

        /// <summary>
        /// SetPath
        /// </summary>
        /// <param name="path">AssetBundle Path</param>
        public void SetPath(string path)
        {
            path = path.Replace('\\', '/');
            int idx = path.LastIndexOf('/');
            if (0 < idx)
                this.Directory = path.Substring(0, idx);
            else
                this.Directory = "";
            this.File = path.Substring(idx + 1);
        }

        /// <summary>
        /// Get all contents.
        /// </summary>
        /// <returns>content information</returns>
        public List<iContent> GetAllContents()
        {
            return this._contents;
        }
        /// <summary>
        /// Get all contents path in assetbundle.
        /// </summary>
        /// <returns>content information</returns>
        public IEnumerable<string> GetAllContentsPath()
        {
            {
                var __list1 = this._contents;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var c = __list1[__i1];
                    {
                        {
                            // foreach(var cc in c.GetContents())
                            var __enumerator4 = (c.GetContents()).GetEnumerator();
                            while (__enumerator4.MoveNext())
                            {
                                var cc = __enumerator4.Current;
                                yield return cc;
                            }
                        }
                    }
                }
            }
            yield break;
        }
        /// <summary>
        /// Get content information in assetbundle.
        /// </summary>
        /// <param name="idx">index</param>
        /// <returns>content information</returns>
        public iContent GetContent(int idx)
        {
            if (idx < 0 || this._contents.Count <= idx)
                return null;
            return this._contents[idx];
        }
        /// <summary>
        /// Add file with assetbundle
        /// </summary>
        /// <param name="path">File path</param>
        public void AddFile(string path)
        {
            //Debug.Log("Add file:" + path);
            var c = new iContent();
            c.Initialize(path);
            this._contents.Add(c);
        }
        /// <summary>
        /// Add directory with assetbundle
        /// </summary>
        /// <param name="directory">Directory</param>
        /// <param name="pattern">Target file name pattern</param>
        public void AddDirectory(string directory)
        {
            //Debug.Log("Add directory:" + directory + pattern);

            string reletivePath = directory;
            if (directory.StartsWith(Application.dataPath))
            {
                reletivePath = directory.Replace(Application.dataPath, "Assets");
            }

            var c = new iContent();
            c.Initialize(reletivePath);
            c.Type = iContent.Types.DirectoryName;
            _contents.Add(c);
            AddFileWithDirectory(directory);
        }

        public void AddFileWithDirectory(string directory)
        {
            var dirInfo = new System.IO.DirectoryInfo(directory);
            if (!dirInfo.Exists)
            {
                Logger.Error("directory does not exists!!" + directory);
            }
            var dirInfos = dirInfo.GetDirectories();
            {
                var __array2 = dirInfos;
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var directoryInfo = __array2[__i2];
                    {
                        AddFileWithDirectory(directoryInfo.FullName);
                    }
                }
            }
            var files = dirInfo.GetFiles();
            {
                var __array3 = files;
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var file = __array3[__i3];
                    {
                        if (!file.FullName.EndsWith(".meta") && !file.FullName.Contains(".DS_Store"))
                        {
                            var path = file.FullName.Replace('\\', '/');
                            var path2 = path.Substring(path.LastIndexOf("Assets/"));
                            Remove(path2);
                            AddFile(path2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete content
        /// </summary>
        /// <param name="info">delete content information</param>
        public void Remove(string info)
        {
            var f = this._contents.Find(c => c.Name == info);
            if (f != null)
                this._contents.Remove(f);
        }

        [SerializeField]
        string _directory;
        [SerializeField]
        string _file;
        [SerializeField]
        bool _separated;
        [SerializeField]
        bool _streamedScene;
        [SerializeField]
        int _parent = -1;
        [SerializeField]
        bool _platformFolder;
#if false//!DLL && UNITY_EDITOR
		[SerializeField]
		BuildAssetBundleOptions _options;
#else
        [SerializeField]
        int _options;
#endif
        [SerializeField]
        List<iContent> _contents = new List<iContent>();
        [SerializeField]
        bool _changed = true;

        [SerializeField]
        int _fileTypeOptions;

        [SerializeField]
        int _dependtimes;
        [SerializeField]
        string _addFilter;
    }
}
