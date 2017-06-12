#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using BehaviourMachine;
using EventSystem;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;

#endregion

#if UNITY_ANDROID && !UNITY_EDITOR
using ICSharpCode.SharpZipLib.Zip;
#endif

public class ResourceHolder<T>
{
    private bool mCompleted;
    public T Resource { get; private set; }

    public void LoadCompleteCallback(T t)
    {
        Resource = t;
        mCompleted = true;
    }

    public Coroutine Wait()
    {
        return ResourceManager.Instance.StartCoroutine(WaitImpl());
    }

    private IEnumerator WaitImpl()
    {
        while (!mCompleted)
        {
            yield return null;
        }
    }
}

public class BundleLoader : Singleton<BundleLoader>
{
    private readonly LinkedList<Action> mQueue = new LinkedList<Action>();

    public Dictionary<string, string> mWaitingDownloadBundles = new Dictionary<string, string>();
    public Queue<string> DownloadBundleKeyQueue = new Queue<string>();

    //用来在 debugwindow 显示用
    public string DownLoadingFileName = string.Empty;
    public string ErrorMessage = "Nothing";
    public bool FirstPriorityDownLoading = false;
    private static int WritingBundle = 0;
    private readonly Thread saveThread;
    private readonly AutoResetEvent evt = new AutoResetEvent(false);
    private FileStream saveFileStream;
    private string bundleFilePath;
    private string bundlePathKey;
    private MemoryStream writeBundleStream = null;
    public bool DownLoadCanStart = false;

    //用来确保每次只有一个www实例
    public bool mQueueLocker;
    public bool mDownloadLocker;


#if UNITY_ANDROID && !UNITY_EDITOR
    private ZipFile mZipFile = null;

    public ZipFile ZipFile
    {
        get
        {
            return mZipFile;
        }
        set { mZipFile = value; }
    }
#endif


    enum eLoadType
    {
        Scene = 0,
        SyncLoad = 1,
        AsyncLoad = 2,
    }

    private HashSet<string> syncBundle = new HashSet<string>();
    private HashSet<string> asyncBundle = new HashSet<string>();
    private HashSet<string> sceneBundle = new HashSet<string>();
    private readonly string[] noSyncPath = 
    {
        "Animation",
        "Effect",
        "Model",
        "ScenePrefab",
        "Sound"
    };

    public BundleLoader()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var apkPath = Application.dataPath;
        var filestream = new FileStream(apkPath, FileMode.Open, FileAccess.Read);
        mZipFile = new ZipFile(filestream);
#endif
        UpdateHelper.CheckTargetPath(downloadDictionaryPath);
        LoadBundleDictionary();
        saveThread = new Thread(SaveBundleThread);
        saveThread.Start();
    }

    readonly string replace = "file://" + Application.dataPath.Replace("\\", "/") + "/BundleAsset/";

    private void BundleDebugLog(eLoadType type, string path, int size)
    {
#if BUNDLE_DEBUG_ENABLE
        var str = path.Replace(replace, "");
        // var str = string.Format("{0},{1},{2}", path, size, Environment.NewLine);
        str = str.Replace("\\", "/");
        str = str + Environment.NewLine;
        switch (type)
        {
            case eLoadType.Scene:
                sceneBundle.Add(str);
                break;
            case eLoadType.AsyncLoad:
                asyncBundle.Add(str);
                break;
            case eLoadType.SyncLoad:
                syncBundle.Add(str);
                break;
        }
#endif
    }

    public void PrintDebugLogToFile()
    {
#if BUNDLE_DEBUG_ENABLE
        WriteToFile(sceneBundle, Application.dataPath+"/../BundleLog/sceneBundle.txt");
        WriteToFile(asyncBundle, Application.dataPath+ "/../BundleLog/asyncBunle.txt");
        WriteToFile(syncBundle, Application.dataPath+ "/../BundleLog/syncBundle.txt");
#endif
    }

    public void WriteToFile(HashSet<string> list, string path)
    {
#if BUNDLE_DEBUG_ENABLE
        var dir = Path.GetDirectoryName(path);
        if (dir != null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        FileStream fs = new FileStream(path, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        foreach (var oneLine in list)
        {
            sw.Write(oneLine);
        }
        sw.Flush();
        sw.Close();
        fs.Close();
#endif
    }


    private const int MacLoadcount = 4;
    public void Tick(float delta)
    {
        for (var i = 0; i < MacLoadcount; i++)
        {
            if (mQueueLocker)
            {
                return;
            }

            if (mQueue.Count <= 0)
            {
                DownloadBundle();
            }

            var action = mQueue.First.Value;
            mQueue.RemoveFirst();
            action();
        }
    }

    // 通过ResourceHolder.Resource获取资源
    public ResourceHolder<T> PrepareResource<T>(string bundlePath,
                                                string assetName,
                                                bool clearBundle,
                                                bool cacheResource,
                                                bool firstPriority,
                                                bool fromCache) where T : Object
    {
        var resourceHolder = new ResourceHolder<T>();

        if (mQueue.Count == 0 || firstPriority || fromCache)
        {
            Object res = null;
            if (ResourceManager.Instance.TryGetResourceFromCache(bundlePath, out res))
            {
                resourceHolder.LoadCompleteCallback(res as T);
                return resourceHolder;
            }
        }

        Action func = () =>
        {
            Object res = null;
            if (ResourceManager.Instance.TryGetResourceFromCache(bundlePath, out res))
            {
                resourceHolder.LoadCompleteCallback(res as T);
                return;
            }
            mQueueLocker = true;
            ResourceManager.Instance.StartCoroutine(Instance.GetResourceWithHolder(bundlePath, assetName, resourceHolder,
                clearBundle, cacheResource));
        };

        if (firstPriority)
        {
            mQueue.AddFirst(func);
        }
        else
        {
            mQueue.AddLast(func);
        }

        return resourceHolder;
    }

    // 通过action获取资源
    public void GetBundleResource<T>(string path,
                                     string name,
                                     Action<T> callBack,
                                     bool clearBundle,
                                     bool cacheResource,
                                     bool firstPriority,
                                     bool fromCache)
        where T : Object
    {
        if (mQueue.Count == 0 || firstPriority || fromCache)
        {
            Object res = null;
            if (ResourceManager.Instance.TryGetResourceFromCache(path, out res))
            {
                if (callBack != null)
                {
                    try
                    {
                        callBack(res as T);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("GetBundleResource {0}, {1}, {2}", path, name, ex.ToString());
                    }
                }
                return;
            }
        }

        Action func = () =>
        {
            Object res = null;
            if (ResourceManager.Instance.TryGetResourceFromCache(path, out res))
            {
                if (callBack != null)
                {
                    try
                    {
                        callBack(res as T);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("GetBundleResource {0}, {1}, {2}", path, name, ex.ToString());
                    }
                }
                return;
            }
            mQueueLocker = true;
            ResourceManager.Instance.StartCoroutine(Instance.GetBundleResourceWithCallBack(path, name, callBack,
                clearBundle,
                cacheResource));
        };

        if (firstPriority)
        {
            mQueue.AddFirst(func);
        }
        else
        {
            mQueue.AddLast(func);
        }
    }



    //同步获取bundle资源方法
    public T GetResourceSync<T>(string bundlePath, string assetName, bool clearBundle, bool cacheResource)
        where T : Object
    {

#if BUNDLE_DEBUG_ENABLE
        foreach (var t in noSyncPath.Where(bundlePath.StartsWith))
        {
            Logger.Error("!!!!Load bundle Sync error!!!!! path :{0}, name :{1}", bundlePath, assetName);
        }
#endif
        var size = 0;
        Object res = null;
        if (ResourceManager.Instance.TryGetResourceFromCache(bundlePath, out res))
        {
            return res as T;
        }

        T resource;

        if (ResourceManager.Instance.UseAssetBundle)
        {
            resource = LoadResourceFromBundleSync<T>(bundlePath, assetName, clearBundle, out size);
        }
        else
        {
            resource = LoadResourceFromAsset<T>(bundlePath, assetName);
        }

        if (cacheResource)
        {
            ResourceManager.Instance.AddResourcesToCache(bundlePath, resource, size);
        }

        return resource;
    }

    private IEnumerator GetResourceWithHolder<T>(string bundlePath,
                                                 string assetName,
                                                 ResourceHolder<T> resourceHolder,
                                                 bool clearBundle,
                                                 bool cacheResource) where T : Object
    {
        T resource;
        var size = 0;
        if (ResourceManager.Instance.UseAssetBundle)
        {
            string path;
            string bundlefullname;
            var needDown = GetBundleRealPath(bundlePath, out path, out bundlefullname);
            WWW www = clearBundle ? new WWW(path) : WWW.LoadFromCacheOrDownload(path, UpdateHelper.Version);
            
            yield return www;

            if (www.error != null)
            {
                Logger.Error("{0}, {1}", www.error, bundlePath);
                www.Dispose();
                mQueueLocker = false;
                yield break;
            }

            if (clearBundle)
            {
                BundleDebugLog(eLoadType.AsyncLoad, path, www.bytes.Length);
            }

            size = 1;
            Object res = null;
            if (ResourceManager.Instance.TryGetResourceFromCache(bundlePath, out res))
            {
                resource = res as T;
            }
            else
            {
                resource = www.assetBundle.Load(Path.GetFileNameWithoutExtension(assetName), typeof (T)) as T;
            }

#if UNITY_EDITOR
            var obj = resource as GameObject;
            if (obj)
            {
                ResourceManager.ChangeShader(obj.transform);
            }
#endif

            //缓存到硬盘
            if (needDown)
            {
                while (Interlocked.CompareExchange(ref WritingBundle, 1, 1) == 1)
                {
                    yield return new WaitForEndOfFrame();
                }
                SaveBundleToDisk(www, bundlefullname);
            }


            //清理
            if (clearBundle)
            {
                www.assetBundle.Unload(false);
            }
            else
            {
                ResourceManager.Instance.mCommonBundle.Add(www.assetBundle);
            }
            www.Dispose();
        }
        else
        {
            resource = LoadResourceFromAsset<T>(bundlePath, assetName);
            yield return new WaitForEndOfFrame();
        }

        if (cacheResource)
        {
            ResourceManager.Instance.AddResourcesToCache(bundlePath, resource, size);
        }

        mQueueLocker = false;
        resourceHolder.LoadCompleteCallback(resource);
    }



    private void SaveBundleThread()
    {

        while (evt.WaitOne())
        {
            try
            {
                saveFileStream = new FileStream(bundleFilePath, FileMode.Create);
                saveFileStream.Write(writeBundleStream.GetBuffer(), 0, (int) writeBundleStream.Length);
                saveFileStream.Close();

                Dictionary<string, string> dict = null;
                lock (mWaitingDownloadBundles)
                {
                    mWaitingDownloadBundles.Remove(bundlePathKey);
                    if (++saveCount % 10 == 0 || mWaitingDownloadBundles.Count < 2)
                    {
                        dict = new Dictionary<string, string>(mWaitingDownloadBundles);
                    }

                    if (mWaitingDownloadBundles.Count == 0)
                    {
                        writeBundleStream.Dispose();
                    }
                }

                if (dict != null)
                {
                    SaveBundleDictionary(dict);
                }
            }
            catch
            {

            }
            finally
            {
                Interlocked.Exchange(ref WritingBundle, 0);
            }
        }
    }

    private void SaveBundleToDisk(WWW www,string path)
    {
        try
        {
            if (null == writeBundleStream)
            {
                writeBundleStream = new MemoryStream();
            }
            Interlocked.Exchange(ref WritingBundle, 1);
            var localUrl = Path.Combine(UpdateHelper.DownloadRoot, path);
            UpdateHelper.CheckTargetPath(localUrl);
            writeBundleStream.SetLength(0);
            writeBundleStream.Seek(0, SeekOrigin.Begin);
            writeBundleStream.Write(www.bytes, 0, www.bytesDownloaded);
            bundleFilePath = localUrl;
            bundlePathKey = path;
            //Profiler.BeginSample("FileStream Write ");
            evt.Set();
            // DownloadBundleKeyQueue.
            //Profiler.EndSample();
        }
        catch (Exception e)
        {

            Interlocked.Exchange(ref WritingBundle, 0);
            Logger.Error("SaveBundleToDisk fail. path {0}, error {1}", path, e);
        }
    }

    private static int saveCount = 0;

    private static void SaveBundleToCache()
    {
       
    }

    private IEnumerator GetBundleResourceWithCallBack<T>(string bundlePath,
                                                         string assetName,
                                                         Action<T> callBack,
                                                         bool clearBundle,
                                                         bool cacheResource) where T : Object
    {
        var resource = default(T);
        var size = 0;
        if (ResourceManager.Instance.UseAssetBundle)
        {
            string path;
            string bundlefullname;
            var needDown = GetBundleRealPath(bundlePath, out path, out bundlefullname);
            WWW www;
            if (clearBundle)
            {
                www = new WWW(path);
            }
            else
            {
                www = WWW.LoadFromCacheOrDownload(path, UpdateHelper.Version);
            }
            yield return www;

            if (www.error != null)
            {
                Logger.Error("{0}, {1}", www.error, bundlePath);
                www.Dispose();
                mQueueLocker = false;
                yield break;
            }

            if (clearBundle)
            {
                BundleDebugLog(eLoadType.AsyncLoad, path, www.bytes.Length);
            }

            size = 1;
            Object res = null;
            if (ResourceManager.Instance.TryGetResourceFromCache(bundlePath, out res))
            {
                resource = res as T;
            }
            else
            {
                resource = www.assetBundle.Load(Path.GetFileNameWithoutExtension(assetName), typeof (T)) as T;
            }

#if UNITY_EDITOR
            var obj = resource as GameObject;
            if (obj)
            {
                ResourceManager.ChangeShader(obj.transform);
            }
#endif

            if (needDown)
            {
                while (Interlocked.CompareExchange(ref WritingBundle, 1, 1) == 1)
                {
                    yield return new WaitForEndOfFrame();
                }
                SaveBundleToDisk(www, bundlefullname);
            }

            if (clearBundle)
            {
                www.assetBundle.Unload(false);
            }
            else
            {
                ResourceManager.Instance.mCommonBundle.Add(www.assetBundle);
            }
            www.Dispose();
        }
        else
        {
            resource = LoadResourceFromAsset<T>(bundlePath, assetName);
            yield return new WaitForEndOfFrame();
        }

        if (cacheResource)
        {
            ResourceManager.Instance.AddResourcesToCache(bundlePath, resource, size);
        }

        mQueueLocker = false;

        try
        {
            if (resource == null)
            {
                callBack(null);
            }
            else
            {
                callBack(resource);
            }
        }
        catch (Exception e)
        {
            Logger.Error("---------CallBackError!!------ Name = {0} , Exception = {1}", bundlePath, e);
        }
    }

    private void DownloadBundle()
    {
        if (!DownLoadCanStart || mDownloadLocker)
        {
            return;
        }

        lock (mWaitingDownloadBundles)
        {
            if (mWaitingDownloadBundles.Count == 0)
            {
                return;
            }
        }

        mDownloadLocker = true;
        try
        {
            ResourceManager.Instance.StartCoroutine(DownloadBundleOne());
        }
        catch (Exception e)
        {
            mDownloadLocker = false;
        }
    }

    private IEnumerator DownloadBundleOne()
    {

        //var enumerator = mWaitingDownloadBundles.GetEnumerator();
        if (DownloadBundleKeyQueue.Count == 0)
        {
            mDownloadLocker = false;
            yield break;
        }
        var key = DownloadBundleKeyQueue.Dequeue();

        string value = string.Empty;
        lock (mWaitingDownloadBundles)
        {
            if (!mWaitingDownloadBundles.ContainsKey(key))
            {
                mDownloadLocker = false;
                yield break;
            }

            value = mWaitingDownloadBundles[key];
        }

        var path = UpdateHelper.CheckUrl(value + "/Resources/" + key);
        DownLoadingFileName = key;
        FirstPriorityDownLoading = false;
        var www = new WWW(path);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            DownloadBundleKeyQueue.Enqueue(key);
            ErrorMessage = string.Format("download www bundle:{0} error:{1}", path, www.error);
            Logger.Error(ErrorMessage);
            mDownloadLocker = false;
            www.Dispose();
            yield break;
        }

        SaveBundleToCache();
        while (Interlocked.CompareExchange(ref WritingBundle, 1, 1) == 1)
        {
            yield return new WaitForEndOfFrame();
        }
        SaveBundleToDisk(www, key);
        www.Dispose();
        mDownloadLocker = false;
    }

    internal IEnumerator GetSceneResource(string bundlePath, string assetName, Action<WWW> callBack)
    {
        mQueueLocker = true;
      
        string path;
        string bundlefullname;
        var needDown = GetBundleRealPath(bundlePath, out path, out bundlefullname);
        if (needDown)
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ShowDownloadingSceneTipEvent());
        }

        WWW www = new WWW(path);
        yield return www;

        if (www.error != null)
        {
            if (null != callBack)
            {
                try
                {
                    callBack(www);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }

            mQueueLocker = false;
            www.Dispose();

            yield break;
        }

        if (needDown)
        {
            while (Interlocked.CompareExchange(ref WritingBundle, 1, 1) == 1)
            {
                yield return new WaitForEndOfFrame();
            }
            SaveBundleToDisk(www, bundlefullname);
        }

        BundleDebugLog(eLoadType.Scene, path, www.bytes.Length);

        if (!www.assetBundle)
        {
            yield break;
        }

        www.assetBundle.LoadAll();

        if (null != callBack)
        {
            try
            {
                callBack(www);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                mQueueLocker = false;
            }
        }
    }

    public static string GetStreamingAssetsUrl(string path)
    {
#if UNITY_EDITOR
        return String.Format("file://{0}/{1}", Application.dataPath + "/BundleAsset", path);
#endif
        if (Application.platform == RuntimePlatform.Android)
        {
            return string.Format("{0}/{1}", Application.streamingAssetsPath, path);
        }
        return string.Format("file://{0}/{1}", Application.streamingAssetsPath, path);
    }

    public static bool BundleExistInUpdatePath(string bundlePath, out string bundleUrl)
    {
#if UNITY_EDITOR
        if (!ResourceManager.Instance.UseAssetBundle)
        {
            bundleUrl = string.Empty;
            return false;
        }
#endif
        bundlePath = bundlePath.Replace("\\", "/");
        var fileName = bundlePath.Replace("/", "_");
        var tempPath = Path.Combine(
            Path.Combine(UpdateHelper.DownloadRoot, Path.GetDirectoryName(bundlePath)),
            Path.GetFileNameWithoutExtension(fileName)) + ".unity3d";
        bundleUrl = string.Format("file://{0}", tempPath);
#if UNITY_EDITOR_WIN
        bundleUrl = bundleUrl.Replace("\\", "/");
#endif
        return File.Exists(tempPath);
    }

    public bool GetBundleRealPath(string bundlePath, out string path, out string bundleFullName)
    {
        bundleFullName = string.Empty;
        bundlePath = bundlePath.Replace("\\", "/");
        var fileName = bundlePath.Replace("/", "_");
        fileName = Path.GetFileNameWithoutExtension(fileName) + ".unity3d";
        var nameKey = Path.Combine(Path.GetDirectoryName(bundlePath), fileName);
#if UNITY_EDITOR_WIN
        nameKey = nameKey.Replace("\\", "/");
#endif

        lock (mWaitingDownloadBundles)
        {
            if (mWaitingDownloadBundles.ContainsKey(nameKey))
            {
                bundleFullName = nameKey;
                path = UpdateHelper.CheckUrl(mWaitingDownloadBundles[nameKey] + "/Resources/" + nameKey);
                //  path = GameUtils.GetNoCacheUrl(path);
                DownLoadingFileName = nameKey;
                FirstPriorityDownLoading = true;

                return true;
            }
        }

        if (BundleExistInUpdatePath(bundlePath, out path))
        {
            return false;
        }

        var url = GetStreamingAssetsUrl(bundlePath);
        var p1 = url.Substring(0, url.LastIndexOf('/'));
        var p2 = url.Substring(GetStreamingAssetsUrl("").Length).Replace('/', '_');
        path = Path.Combine(p1, p2);
        return false;
    }

    //调试环境直接从源文件读取资源
    private static T LoadResourceFromAsset<T>(string bundlePath, string assetName) where T : Object
    {
        if (string.IsNullOrEmpty(Path.GetExtension(assetName)))
        {
            assetName = assetName + ".prefab";
        }
        var realPath = Path.Combine("Assets/Res", Path.GetDirectoryName(bundlePath));
        realPath = Path.Combine(realPath, Path.GetFileName(assetName));
        var asset = Resources.LoadAssetAtPath<T>(realPath);
        if (!asset)
        {
            Logger.Error("can not load resource :" + assetName);
        }
        return asset;
    }

    private T LoadResourceFromBundleSync<T>(string bundlePath, string assetName, bool clearBundle, out int size)
        where T : Object
    {
        var resource = default(T);
        size = 1;
        string realpath;
        string unuse;
        var needDown = GetBundleRealPath(bundlePath, out realpath, out unuse);
        if (needDown)
        {
            size = 0;
            Logger.Error("get file error !:" + bundlePath);
            return null;
        }

        string noUse;
        if (!BundleExistInUpdatePath(bundlePath, out noUse))
        {
            //从apk包中读取文件
#if UNITY_ANDROID && !UNITY_EDITOR
    //GetNextEntry    assets/Controller/MainPlayer.unity3d
    //url             jar:file:///data/app/com.base.maya.test-1.apk!/assets/Controller/MainPlayer.unity3d

        var filepath = realpath.Substring(realpath.IndexOf("assets/"));
        var item = Instance.ZipFile.GetEntry(filepath);
        if (null != item)
        {
            var stream = Instance.ZipFile.GetInputStream(item);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var assetBundle = AssetBundle.CreateFromMemoryImmediate(buffer);
            resource = assetBundle.Load(Path.GetFileNameWithoutExtension(assetName), typeof(T)) as T;
            stream.Close();
            if(clearBundle)
            {
                assetBundle.Unload(false);
            }
            else
            {
                ResourceManager.Instance.mCommonBundle.Add(assetBundle);
            }
        }
        else
        {
            Logger.Error("--LoadResourceFromBundleSync--,get file from apk error !:" + realpath);
        }

        return resource;
#endif
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        var path = realpath.Substring(realpath.IndexOf("file://") + 7);
#else
		var path = realpath.Substring(realpath.IndexOf("file:") +  5);
#endif
        if (File.Exists(path))
        {
            var stream = File.OpenRead(path);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var assetBundle = AssetBundle.CreateFromMemoryImmediate(buffer);
            resource = assetBundle.Load(Path.GetFileNameWithoutExtension(assetName), typeof (T)) as T;
            if (clearBundle)
            {
                assetBundle.Unload(false);
            }
            else
            {
                ResourceManager.Instance.mCommonBundle.Add(assetBundle);
            }

            Instance.BundleDebugLog(eLoadType.SyncLoad, path, buffer.Length);
            stream.Close();
        }
        else
        {
            Logger.Error("get file error !:" + path);
        }

        return resource;
    }

    private readonly string downloadDictionaryPath = Path.Combine(Application.persistentDataPath, "download.txt");

    public void SaveBundleDictionary(Dictionary<string, string> dict)
    {
        using (var fs = new FileStream(downloadDictionaryPath, FileMode.Create))
        {
            var bf = new BinaryFormatter();
            bf.Serialize(fs, dict);
            fs.Close();
        }
    }

    private void LoadBundleDictionary()
    {
        using (var fs = new FileStream(downloadDictionaryPath, FileMode.OpenOrCreate))
        {
            if (fs.Length != 0)
            {
                var bf = new BinaryFormatter();
                mWaitingDownloadBundles = (Dictionary<string, string>)bf.Deserialize(fs);
                var enumerator = mWaitingDownloadBundles.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DownloadBundleKeyQueue.Enqueue(enumerator.Current.Key);
                }
            }
            fs.Close();
        }
    }

    public float GetWwwProgress()
    {

//         if (mWaitingWww != null)
//         {
//             return mWaitingWww.progress;
//         }
        return 0;
    }

}