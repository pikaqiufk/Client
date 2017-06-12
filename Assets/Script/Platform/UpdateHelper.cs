#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClientDataModel;
using ClientService;
using DataTable;
using ScorpionNetLib;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

#if UNITY_ANDROID && !UNITY_EDITOR
using ScorpionNetLib;
#endif


public class UpdateHelper
{
    public static string AnnoucementURL = "http://www.google.com";
    public static string DownloadRoot = Path.Combine(Application.persistentDataPath, "Maya"); //所有资源更新下来存放的根目录
    public static string GateAddress = string.Empty;
    //资源版本号
    private static int LocalVersion = -1;
    private static UpdateHelper mInstance;
    private static int RemoteVersion = -1;

    //
    public static HashSet<string> whiteListDirectorys = new HashSet<string>();
    public static HashSet<string> whiteListFils = new HashSet<string>();

    //是否需要重启
    public bool NeedRestart = false;
    private void LoadWhiteList()
    {
        whiteListDirectorys.Clear();
        whiteListFils.Clear();
        const string pathDir = "Table/BundleWhiteList/Directorys.txt";
        var asset = ResourceManager.PrepareResourceSync<TextAsset>(pathDir);
        if (null == asset)
        {
            Logger.Error("can't find Table/BundleWhiteList/Directorys.txt ");
            return;
        }


        var ms = new MemoryStream(asset.bytes, false);
        var sr = new StreamReader(ms);
        var content = sr.ReadLine();
        while (null != content)
        {
            content = content.Trim();
            if (!string.IsNullOrEmpty(content) || !content.StartsWith("//"))
            {
                whiteListDirectorys.Add(content);
            }
            content = sr.ReadLine();
        }
        sr.Close();
        ms.Close();

        const string pathBundle = "Table/BundleWhiteList/BundleFiles.txt";
        asset = ResourceManager.PrepareResourceSync<TextAsset>(pathBundle);
        if (null == asset)
        {
            Logger.Error("can't find Table/BundleWhiteList/BundleFiles.txt");
            return;
        }

        ms = new MemoryStream(asset.bytes, false);
        sr = new StreamReader(ms);
        content = sr.ReadLine();
        while (null != content)
        {
            content = content.Trim();
            if (!string.IsNullOrEmpty(content) || !content.StartsWith("//"))
            {
                whiteListFils.Add(content);
            }
            content = sr.ReadLine();
        }
        sr.Close();
        ms.Close();
    }
    public UpdateHelper()
    {
        DownLoadVersionPath = Path.Combine(DownloadRoot, "Resources.ver"); //下载的版本号目录
        DownLoadGameVersionPath = Path.Combine(DownloadRoot, "BigGame.ver"); // 游戏大版本号
        LocalMd5ListFilePath = Path.Combine(DownloadRoot, "tempMd5List.txt");
        InitDictionary();
        LoadWhiteList();

    }

    private int CurFileSize;
    public int CurrentCount;
    private readonly Dictionary<int, IRecord> Dictionary = new Dictionary<int, IRecord>();
    public string DownLoadGameVersionPath; // 游戏大版本号
    public string DownLoadVersionPath; //下载的版本号目录
    public string LocalMd5ListFilePath;
    //状态显示用
    private long mAlreadyDownloadSize;
    private Dictionary<string, string> mDownloadMd5Dictionary;
    //游戏版本号
    public static string LocalGameVersion = string.Empty;
    private bool mStop = false;
    private long mTotalSize = 1;
    private long mLateDownloadTotalSize = 1;
    public string RemoteUrlRoot;
    public int TotalCount;
    public string UpdateStatus;
    //当前下载文件www
    private WWW wwwCurFile;

    public enum CheckVersionResult
    {
        NONEEDUPDATE, // 不需要更新
        GAMENEEDUPDATE, //游戏需要大更新
        NEEDUPDATE, // 需要更新
        ERROR // 检查失败
    }

    public enum UpdateResult
    {
        NONE,
        Success, // 更新成功
        GetMd5ListFail, // 获取更新列表文件失败
        GetMd5ListSuccess, // 获取下载列表成功
        GetMd5ListSuccessAndLateUpdate //首次后台下载
    }

    public string DownloadedSize
    {
        get
        {
            var downloadSize = mAlreadyDownloadSize;
            if (wwwCurFile != null)
            {
                downloadSize += (int) (wwwCurFile.progress*CurFileSize);
            }
            return string.Format("{0} MB", (downloadSize/1048576.0f).ToString("F3"));
        }
    }

    public string TotalSize
    {
        get { return string.Format("{0} MB", (mTotalSize/1048576.0f).ToString("F3")); }
    }

    public float UpdatePrecent
    {
        get
        {
//             var downloadSize = mAlreadyDownloadSize;
//             if (wwwCurFile != null)
//             {
//                 downloadSize += wwwCurFile.bytesDownloaded;
//             }
            var progress = 0f;
            if (wwwCurFile != null)
            {
                progress = wwwCurFile.progress;
            }

            return progress;
        }
    }

    public static int Version
    {
        get { return LocalVersion; }
    }

    //检查游戏和资源版本号
    private IEnumerator CheckResourceVersion(DelegateCheckResVersion checkVersionDelFun, bool updateGate)
    {
        UpdateStatus = GetDictionaryText(3300000);
        string versionConfig;
        var gameVersionPath = Path.Combine(Application.streamingAssetsPath, "Game.ver");
        if (!GameUtils.GetStringFromPackage(gameVersionPath, out versionConfig))
        {
            Logger.Error("cant find Game.ver at{0}", gameVersionPath);
            if (null != checkVersionDelFun)
            {
                checkVersionDelFun(CheckVersionResult.ERROR, GetDictionaryText(3300008));
            }
            yield break;
        }

        var config = versionConfig.Split(',');
        var langue = config[0];
        var platform = config[1];
        var channel = config[2];
        var bigVersion = config[3];
        var resourceVersion = config[4];
        if (string.IsNullOrEmpty(langue) || string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(channel) ||
            string.IsNullOrEmpty(bigVersion) || string.IsNullOrEmpty(bigVersion))
        {
            Logger.Error("Game.ver error {0}", versionConfig);
        }

        LocalGameVersion = bigVersion;

        if (!Int32.TryParse(resourceVersion, out LocalVersion))
        {
            Logger.Error("Game.ver error {0}", LocalVersion);
            if (null != checkVersionDelFun)
            {
                checkVersionDelFun(CheckVersionResult.ERROR, GetDictionaryText(3300009));
            }
            yield break;
        }

        UpdateStatus = GetDictionaryText(3300001);
        //获取远端游戏版本信息
        var UpdateHelper = new GameObject("UpdateHelper");
        var network = UpdateHelper.AddComponent<DirectoryNetwork>();
        CheckVersionOutMessage msg;

        var index = 0;
        while (true)
        {
            if (index == GameSetting.Instance.DirectoryAddress.Count)
            {
                network.Stop();
                Object.Destroy(UpdateHelper);
                if (null != checkVersionDelFun)
                {
                    checkVersionDelFun(CheckVersionResult.ERROR, GetDictionaryText(3300010));
                }
                yield break;
            }

            network.ServerAddress = GameSetting.Instance.DirectoryAddress[index];
            yield return network.StartAndWaitConnect(TimeSpan.FromSeconds(3));
            if (network.Connected)
            {
                Logger.Debug("Connect to Directory [" + network.ServerAddress + "] succeed!");

                msg = network.CheckVersion(langue, platform, channel, bigVersion);
                yield return msg.SendAndWaitUntilDone();
                if (msg.State == MessageState.Reply)
                {
                   break;
                }
            }
            network.Stop();
            index++;
        }

        UpdateStatus = GetDictionaryText(3300002);

        if (msg.ErrorCode == (int) ErrorCodes.OK)
        {
            if (msg.Response.HasNewVersion == 1)
            {
                var url = CheckUrl(msg.Response.NewVersionURL);
                if (null != checkVersionDelFun)
                {
                    checkVersionDelFun(CheckVersionResult.GAMENEEDUPDATE, url);
                }
                network.Stop();
                Object.DestroyImmediate(UpdateHelper);
                yield break;
            }
            LocalGameVersion = bigVersion;
            RemoteVersion = msg.Response.SmallVersion;
            RemoteUrlRoot = msg.Response.ResourceURL;
            AnnoucementURL = CheckUrl(msg.Response.AnnoucementURL);
            if (updateGate && GateAddress != msg.Response.GateAddress)
            {
                GateAddress = msg.Response.GateAddress;
            }
            GameSetting.Instance.ReviewState = msg.Response.ReviewState;


            //数据收集，设备id，只发送一次
            const string key = "deviceid";
            var udid = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(udid))
            {
                udid = Guid.NewGuid().ToString();
                NetManager.Instance.ServerAddress = GateAddress;
                yield return NetManager.Instance.StartAndWaitConnect(TimeSpan.FromSeconds(3));
                if (NetManager.Instance.Connected)
                {
                    var msgUdid = NetManager.Instance.SendDeviceUdid(udid);
                    yield return msgUdid.SendAndWaitUntilDone();
                    PlayerPrefs.SetString(key, udid);
                    PlayerPrefs.Save();
                    NetManager.Instance.Stop();
                }
            }
        }
        else
        {
            network.Stop();
            Object.Destroy(UpdateHelper);
            if (null != checkVersionDelFun)
            {
                checkVersionDelFun(CheckVersionResult.ERROR, GetDictionaryText(3300011) + msg.ErrorCode);
            }
            yield break;
        }
        

        network.Stop();
        Object.DestroyImmediate(UpdateHelper);

        ClearLastGameVersionResource();

        //读取之前更新过的版本号
        if (File.Exists(DownLoadVersionPath))
        {
            if (!GameUtils.GetIntFromFile(DownLoadVersionPath, out LocalVersion))
            {
                Logger.Error("parse version error");
                if (null != checkVersionDelFun)
                {
                    checkVersionDelFun(CheckVersionResult.ERROR, GetDictionaryText(3300013));
                }
            }
            Logger.Debug("Last Update ResourceVersion from local =" + LocalVersion);
        }

        if (RemoteVersion > LocalVersion)
        {
            UpdateStatus = GetDictionaryText(3300003);
            Caching.CleanCache();
            PlayerPrefs.SetInt(GameSetting.ShowWaitingTipKey, 0);
            checkVersionDelFun(CheckVersionResult.NEEDUPDATE);
            Logger.Debug("-----CheckUpDate needupdate = true--");
        }
        else
        {
            UpdateStatus = "";
            checkVersionDelFun(CheckVersionResult.NONEEDUPDATE);
            Logger.Debug("-----CheckUpdate needupdate = false---");
        }
    }

    public static void CheckTargetPath(string targetPath)
    {
        targetPath = targetPath.Replace('\\', '/');

        var dotPos = targetPath.LastIndexOf('.');
        var lastPathPos = targetPath.LastIndexOf('/');

        if (dotPos > 0 && lastPathPos < dotPos)
        {
            targetPath = targetPath.Substring(0, lastPathPos);
        }
        if (Directory.Exists(targetPath))
        {
            return;
        }

        var subPath = targetPath.Split('/');
        var curCheckPath = "";
        var subContentSize = subPath.Length;
        for (var i = 0; i < subContentSize; i++)
        {
            curCheckPath += subPath[i] + '/';
            if (!Directory.Exists(curCheckPath))
            {
                Directory.CreateDirectory(curCheckPath);
            }
        }
    }

    public static string CheckUrl(string url)
    {
        var ret = url;
        if (!url.Contains("http://") && !url.Contains("https://"))
        {
            ret = "http://" + url;
        }
        return ret;
    }

    public void CheckVersion(DelegateCheckResVersion checkVersionDelFun, bool updateGate = false)
    {
        ResourceManager.Instance.StartCoroutine(CheckResourceVersion(checkVersionDelFun, updateGate));
    }

    //todo 
    public static bool CheckWiFi()
    {
        var state = (NetworkStatus)PlatformHelper.GetNetworkState();
        return state == NetworkStatus.ReachableViaWiFi;
    }

    private void ClearLastGameVersionResource()
    {
        if (File.Exists(DownLoadGameVersionPath))
        {
            var GameVersion = string.Empty;
            if (GameUtils.GetStringFromFile(DownLoadGameVersionPath, out GameVersion))
            {
                if (!GameVersion.Equals(LocalGameVersion))
                {
                    ClearLocalFile();
                }
            }
        }
        else
        {
            WriteStringToFile(DownLoadGameVersionPath, LocalGameVersion);
        }


        if (File.Exists(DownLoadVersionPath))
        {
            string ResVersion;
            if (GameUtils.GetStringFromFile(DownLoadVersionPath, out ResVersion))
            {
                int downloadversion;
                if (int.TryParse(ResVersion, out downloadversion))
                {
                    if (LocalVersion > downloadversion)
                    {
                        ClearLocalFile();
                    }
                }
            }
        }
        else
        {
            WriteStringToFile(DownLoadVersionPath, LocalVersion.ToString());
        }
    }

    private void ClearLocalFile()
    {
        Caching.CleanCache();
        PlayerPrefs.SetInt(GameSetting.ShowWaitingTipKey, 0);
        DeleteDir(DownloadRoot);
        WriteStringToFile(DownLoadGameVersionPath, LocalGameVersion);
        WriteStringToFile(DownLoadVersionPath, LocalVersion.ToString());
        Logger.Debug("ClearLastGameVersionResource");
    }

    //删除文件夹下所有文件,保留目录
    private void DeleteDir(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                var dirs = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);
                {
                    var __array1 = files;
                    var __arrayLength1 = __array1.Length;
                    for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                    {
                        var file = __array1[__i1];
                        {
                            File.Delete(file);
                        }
                    }
                }
                {
                    var __array2 = dirs;
                    var __arrayLength2 = __array2.Length;
                    for (var __i2 = 0; __i2 < __arrayLength2; ++__i2)
                    {
                        var dir = __array2[__i2];
                        {
                            Directory.Delete(dir, true);
                        }
                    }
                }
            }
            else
            {
                Logger.Error("Delete download cache path {0} not exist!!! ", path);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("DeleteDir throw exception {0}", ex);
        }
    }

    private string GetDictionaryText(int id)
    {
        IRecord tableRecord;
        if (!Dictionary.TryGetValue(id, out tableRecord))
        {
            Logger.Error("Dictionary{0} not find by PreDictionary UpdateHelper!", id);
            return "";
        }

        var tbdic = tableRecord as DictionaryRecord;

        return tbdic.Desc[GameUtils.LanguageIndex];
    }

    private static string GetMd5DiffListFileName()
    {
        return String.Format("/Md5Diff/md5diff{0}.{1}.txt", LocalVersion, RemoteVersion);
    }

    private void InitDictionary()
    {
        Dictionary.Clear();
        ResourceManager.PrepareResource<TextAsset>("Table/Dictionary.txt", asset =>
        {
            if (asset == null)
            {
                Logger.Error("PreInit Dictionary Table error!!!");
                return;
            }
            TableInit.Table_Init(asset.bytes, Dictionary, TableType.Dictionary);
        }, true, false, true, true, false);
    }

    public IEnumerator StartUpdateAll(DelegateDownloadFinish delFun)
    {
        UpdateStatus = GetDictionaryText(3300006);
        if (null == mDownloadMd5Dictionary)
        {
            if (delFun != null)
            {
                delFun(false);
            }
            yield break;
        }

        var remoteUrl = "";
        var localUrl = "";
        {
            TotalCount = mDownloadMd5Dictionary.Count;
            // foreach(var pair in mDownloadMd5Dictionary)
            var __enumerator4 = (mDownloadMd5Dictionary).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var pair = __enumerator4.Current;
                {
                    remoteUrl = CheckUrl(RemoteUrlRoot + "/Resources/" + pair.Key);
                    localUrl = DownloadRoot + "/" + pair.Key;
                    CurFileSize = int.Parse(pair.Value.Split(',')[1]);
                    wwwCurFile = new WWW(GameUtils.GetNoCacheUrl(remoteUrl));
                    yield return wwwCurFile;
                    CurFileSize = 0;
                    CurrentCount++;

                    if (!String.IsNullOrEmpty(wwwCurFile.error))
                    {
                        Logger.Error("download file fail,file name = {0}, error = {1}", remoteUrl, wwwCurFile.error);
                        if (null != delFun)
                        {
                            delFun(false, string.Format(GetDictionaryText(3300015), remoteUrl));
                        }
                        yield break;
                    }
                    try
                    {

                        CheckTargetPath(localUrl);
                        var fs = new FileStream(localUrl, FileMode.Create);
                        fs.Write(wwwCurFile.bytes, 0, wwwCurFile.bytesDownloaded);
                        fs.Close();

                        WriteDllToDisk(pair.Key, wwwCurFile);

                        var md5Hash = GameUtils.GetMd5Hash(localUrl);
                        var md5List = pair.Value.Split(',')[0];
                        if (!md5List.ToLower().Equals(md5Hash.ToLower()))
                        {
                            Logger.Error(
                                "Download file md5 not equal!!! md5 in md5list={0}, compute md5={1},file name = {2}",
                                pair.Value, md5Hash, pair.Key);
                            if (delFun != null)
                            {
                                delFun(false, string.Format(GetDictionaryText(3300015), remoteUrl));
                            }
                            yield break;
                        }

                        //下载完成后写入本地已下载文件列表
                        CheckTargetPath(LocalMd5ListFilePath);

                        var md5Tempfs = new FileStream(LocalMd5ListFilePath, FileMode.Append, FileAccess.Write);
                        var md5Tempsw = new StreamWriter(md5Tempfs);
                        var appendText = pair.Key + "," + md5Hash;
                        md5Tempsw.WriteLine(appendText);
                        md5Tempsw.Close();
                        md5Tempfs.Close();
                        mAlreadyDownloadSize += wwwCurFile.bytesDownloaded;

                        wwwCurFile.Dispose();
                        wwwCurFile = null;
                        Logger.Debug("Download file to " + localUrl);

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("download file and write to local error" + ex);
                        if (null != delFun)
                        {
                            delFun(false, string.Format(GetDictionaryText(3300016), localUrl));
                        }
                        yield break;
                    }
                }
            }
        }
        UpdateStatus = GetDictionaryText(3300007);
        //全部完成,删除本地临时md5list,升级本地资源版本号

        try
        {
            if (File.Exists(LocalMd5ListFilePath))
            {
                File.Delete(LocalMd5ListFilePath);
            }
            if (File.Exists(DownLoadVersionPath))
            {
                File.Delete(DownLoadVersionPath);
            }

            CheckTargetPath(DownLoadVersionPath);

            var versionfs = new FileStream(DownLoadVersionPath, FileMode.OpenOrCreate);
            var versionsr = new StreamWriter(versionfs, Encoding.UTF8);
            versionsr.WriteLine(RemoteVersion.ToString());
            versionsr.Close();
            versionfs.Close();
            LocalVersion = RemoteVersion;
            UpdateStatus = string.Empty;
            if (null != delFun)
            {
                if (NeedRestart)
                {
                    delFun(true, GetDictionaryText(200006012)); 
                }
                else
                {
                    delFun(true);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Updata finish ,but write version to local exception : ex{0}", ex);
            if (null != delFun)
            {
                delFun(false, GetDictionaryText(3300019));
            }
        }
    }

    private void WriteDllToDisk(string key, WWW www)
    {
        if (null == www)
        {
            return;
        }

        if (!key.Equals("dll/dll_a.unity3d") && !key.Equals("dll/dll_c.unity3d"))
        {
            return;
        }


        try
        {

            string localPath, filename;
            string localPath2 = string.Empty;

            if (key.Equals("dll/dll_a.unity3d"))
            {
                localPath = DownloadRoot + "/dll/a.bytes";
                filename = "a";
                NeedRestart = true;
            }
            else
            {
                localPath = DownloadRoot + "/dll/c.bytes";
                filename = "c";
                NeedRestart = true;
            }

            var assetBundle = AssetBundle.CreateFromMemoryImmediate(www.bytes);
            var asset = assetBundle.Load(filename, typeof(TextAsset)) as TextAsset;

            if (null == asset)
            {
                Logger.Error("read {0}.unity3d error! www.bytes {1}", filename, www.bytes);
                return;
            }

            Logger.Debug("WriteDllToDisk localPath ={0}", localPath);
            File.WriteAllBytes(localPath, asset.bytes);
            Logger.Debug("WriteDllToDisk localPath ={0} finish", localPath);

            //storage/emulated/0/Android/data/com.Uborm.Maya.Uborm/files/Maya/dll/a.bytes
            if (DownloadRoot.Contains("emulated"))
            {
                localPath2 = "/data" + localPath.Substring(localPath.IndexOf("/data/com."));
            }

            if (!string.IsNullOrEmpty(localPath2))
            {
                CheckTargetPath(localPath2);
                Logger.Debug("WriteDllToDisk localPath2 ={0}", localPath2);
                File.WriteAllBytes(localPath2, asset.bytes);
                Logger.Debug("WriteDllToDisk localPath2 ={0} finish!", localPath2);
            }

            assetBundle.Unload(false);
            Logger.Debug("write {0}, to disk {1}", key, localPath);
        }
        catch (Exception e)
        {
            Logger.Error("WriteDllToDisk Exception! ex ={0}" , e);
        }

    }

    //获取下载列表,去除已下载文件
    public IEnumerator UpdateMd5List(DelegateGetMd5List delFun)
    {
        UpdateStatus = GetDictionaryText(3300004);
        var md5ListUrl = CheckUrl(RemoteUrlRoot + GetMd5DiffListFileName());

        Logger.Debug("-----UpdateMd5List : " + md5ListUrl);

        var wwwMd5ListFile = new WWW(GameUtils.GetNoCacheUrl(md5ListUrl));
        yield return wwwMd5ListFile;

        if (!String.IsNullOrEmpty(wwwMd5ListFile.error))
        {
            delFun(UpdateResult.GetMd5ListFail, GetDictionaryText(3300014));
            yield break;
        }

        UpdateStatus = GetDictionaryText(3300005);
        //读取远端列表
        mDownloadMd5Dictionary = new Dictionary<string, string>();
        var stream = new MemoryStream(wwwMd5ListFile.bytes);
        var sr = new StreamReader(stream);
        var line = sr.ReadLine();
        var changed = false;
        string key;
        while (!String.IsNullOrEmpty(line))
        {
            var str = line.Split(',');
            if (!String.IsNullOrEmpty(str[1]))
            {
                key = str[1];
                if (!mDownloadMd5Dictionary.ContainsKey(key))
                {
                    var idx = key.IndexOf('/');
                    var dir = idx == -1 ? string.Empty : key.Substring(0, idx);
                    bool lateUpdate = false;
                    if (!string.IsNullOrEmpty(dir) && whiteListDirectorys.Contains(dir))
                    {
                        lateUpdate = !whiteListFils.Contains(key);
                    }

                    if (lateUpdate)
                    {
                        lock (BundleLoader.Instance.mWaitingDownloadBundles)
                        {
                            if (BundleLoader.Instance.mWaitingDownloadBundles.ContainsKey(key))
                            {
                                BundleLoader.Instance.mWaitingDownloadBundles[key] = RemoteUrlRoot;
                                BundleLoader.Instance.DownloadBundleKeyQueue.Enqueue(key);
                            }
                            else
                            {
                                BundleLoader.Instance.mWaitingDownloadBundles.Add(key, RemoteUrlRoot);
                                BundleLoader.Instance.DownloadBundleKeyQueue.Enqueue(key);
                            }
                        }
                        changed = true;
                        mLateDownloadTotalSize += int.Parse(str[3]);
                    }
                    else
                    {
                        mDownloadMd5Dictionary.Add(str[1], String.Format("{0},{1}", str[2], str[3]));
                        mTotalSize += int.Parse(str[3]);
                    }
                }
                else
                {
                    Logger.Error("md5list have repeated file:" + key);
                }
            }
            line = sr.ReadLine();
        }

        if (changed)
        {
            BundleLoader.Instance.SaveBundleDictionary(BundleLoader.Instance.mWaitingDownloadBundles);
        }

        Logger.Debug("-----RemoteMd5List have {0} files---", mDownloadMd5Dictionary.Count);

        //读取本地已下载列表
        if (File.Exists(LocalMd5ListFilePath))
        {
            var list = new List<string>();
            var localMd5sr = new StreamReader(LocalMd5ListFilePath);
            var localLine = localMd5sr.ReadLine();
            while (!String.IsNullOrEmpty(localLine))
            {
                list.Add(localLine);
                localLine = localMd5sr.ReadLine();
            }
            localMd5sr.Close();
            localMd5sr.Dispose();

            Logger.Debug("LocalMd5List have {0} files", list.Count);
            {
                var __list3 = list;
                var __listCount3 = __list3.Count;
                for (var __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var strline = __list3[__i3];
                    {
                        var str = strline.Split(',');
                        var fileName = str[0];
                        var fileMd5 = str[1];

                        if (mDownloadMd5Dictionary.ContainsKey(fileName))
                        {
                            var listMd5 = mDownloadMd5Dictionary[fileName].Split(',')[0];
                            if (listMd5.ToLower().Equals(fileMd5.ToLower()))
                            {
                                mTotalSize -= int.Parse(mDownloadMd5Dictionary[fileName].Split(',')[1]);
                                mDownloadMd5Dictionary.Remove(fileName);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Logger.Debug("-----already downloaded 0 files---");
        }
        stream.Close();
        stream.Dispose();
        sr.Close();
        sr.Dispose();
        wwwMd5ListFile.Dispose();

        if (null != delFun)
        {
            try
            {
                var size = mLateDownloadTotalSize/1048576.0f;
                if (size > 50)
                {
                    delFun(UpdateResult.GetMd5ListSuccessAndLateUpdate, size.ToString("F2")); 
                }
                else
                {
                    delFun(UpdateResult.GetMd5ListSuccess, (mTotalSize / 1048576.0f).ToString("F2"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
            }
        }
    }

    private void WriteStringToFile(string path, string centent)
    {
        CheckTargetPath(path);
        var versionfs = new FileStream(path, FileMode.OpenOrCreate);
        var versionsr = new StreamWriter(versionfs,new UTF8Encoding(false));
        versionsr.WriteLine(centent);
        versionsr.Close();
        versionfs.Close();
    }

    public delegate void DelegateCheckResVersion(CheckVersionResult result, string message = "");

    public delegate void DelegateGetMd5List(UpdateResult result, string message = "");

    public delegate void DelegateDownloadFinish(bool bSuccess, string message = "");
}