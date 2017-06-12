using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using isotope;
using LuaInterface;
using Shared;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEditor.XCodeEditor;
using Debug = UnityEngine.Debug;
using FileMode = System.IO.FileMode;

/// <summary>
/// App包，打包工具
/// </summary>
public class AppBuildTool
{
    //公司名称
    private const string CN_COMPANY_NAME = "Uborm";
    //游戏名称
    private const string CN_GAME_NAME = @"玛雅传说";
    //应用程序名字
    private const string CN_APP_NAME = "Maya";
#if UNITY_IOS
        //应用程序bundle的唯一标记
	private const string CN_BUNDLE_IDENTIFIER = "com.Uborm.Maya";
#else
    //应用程序bundle的唯一标记
    private const string CN_BUNDLE_IDENTIFIER = "com.Uborm.Maya";
#endif

    //应用程序bundle的版本
    private static string CN_BUNDLE_VERSION ="1.0";
    //for android only
    private static int CN_BUNDLE_VERSION_CODE = 10000001;

    //宏定义
    private const string CN_COMMON_DEFINE_SYMBOLS = "LZ4s";

    private static string GameVersionPath = string.Empty;



 //bundle下载包
    private static bool s_IsThinPackage;

    public static HashSet<string> whiteListDirectorys = new HashSet<string>();
    public static HashSet<string> whiteListFils = new HashSet<string>();
    
    private static readonly string BuildChannelPath = Path.Combine(Application.dataPath, "../buildchannel.txt");
//做完资源后上传dll的update路径
    private static string DllBackupPath = string.Empty;
    //项目基本配置
    private static void SetupPlayerSettings(string bundleIdentifier, string bundleVersion, BuildTargetGroup group,
        string defineSymbols)
    {
        PlayerSettings.companyName = CN_COMPANY_NAME;
        PlayerSettings.productName = CN_GAME_NAME;
        PlayerSettings.bundleIdentifier = bundleIdentifier;
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defineSymbols);
        PlayerSettings.aotOptions = "nimt-trampolines=512"; //泛型嵌套,默认为128,太小了真机会崩溃
		PlayerSettings.strippingLevel = StrippingLevel.Disabled;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
    }

    [MenuItem("Build/Test")]
    private static void Quit()
    {
       AssetDatabase.Refresh();
       //MakeUpdatePackage("Android","Uborm","1.1.0", "1", "0");
    }


    private static void RefreshAsset()
    {
        AssetDatabase.Refresh();
    }

    private static void MakeUpdatePackageWithConfig()
    {
        var configPath = Path.Combine(Application.dataPath, "../build/makeconfig.txt");
        if (!File.Exists(configPath))
        {
            throw new Exception("makeconfig not found!");
        }

        var configs = File.ReadAllText(configPath).Split(',');
        if (configs.Length < 4)
        {
            throw new Exception("makeconfig error!");
        }

        File.Delete(configPath);

        for (int i = 0; i < configs.Length; i++)
        {
            configs[i] = configs[i].Trim();
        }

        MakeUpdatePackage(configs[0], configs[1], configs[2], configs[3], configs[4]);
    }
    

    [MenuItem("Build/Build IOS IPA Quickly")]
    private static void BuildToIosDeviceQuickly()
    {
        BuildToIosIpaImpl(false, "Uborm");
    }

    [MenuItem("Build/Build IOS IPA")]
    private static void BuildToIosIpa()
    {
        BuildToIosIpaImpl(true, "Uborm");
    }

    private static void BuildToIosStarJoys()
    {
        BuildToIosIpaImpl(true, "StarJoys");
    }

    private static void BuildToIosStarJoysQuickly()
    {
        BuildToIosIpaImpl(false, "StarJoys");
    }

    private static void BuildToIosIpaImpl(bool processResources, string channel)
    {
        BuildTarget buildTarget = BuildTarget.iPhone;
        string versionPath = string.Format("../../Public/Version/Cn/Ios/{0}/Game.ver", channel);
        GameVersionPath = Path.Combine(Application.dataPath, versionPath);
        ConfigVersionFile();

        SetupPlayerSettings(CN_BUNDLE_IDENTIFIER, CN_BUNDLE_VERSION, BuildTargetGroup.iPhone,
            CN_COMMON_DEFINE_SYMBOLS);
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);

        ProcessResourcesImpl(processResources);
        ProcessPlugins("ios", "TEST");
        string outPutDir = Application.dataPath.Replace("/Assets", "") + "/IOS_Test";
        if (Directory.Exists(outPutDir))
        {
            Directory.Delete(outPutDir, true);
            Directory.CreateDirectory(outPutDir);
        }
        else
        {
            Directory.CreateDirectory(outPutDir);
        }

        GenericBuild(FindEnabledEditorScenes(), outPutDir + "/" + CN_APP_NAME, buildTarget, BuildOptions.Il2CPP | BuildOptions.ConnectWithProfiler,
            false);

        ConfigXcodeProject(outPutDir);
    }

    private static void ConfigVersionFile()
    {
        string verInfo = File.ReadAllText(GameVersionPath).Trim();
        var config = verInfo.Split(',');

        CN_BUNDLE_VERSION = config[3];

        //安卓内部版本号
        var verInteger = int.Parse(CN_BUNDLE_VERSION.Replace(".", ""));
        CN_BUNDLE_VERSION_CODE = CN_BUNDLE_VERSION_CODE + verInteger;

        var svnVer = string.Format("../../Update/{0}/{1}/{2}/{3}/svn.ver", config[0], config[1], config[2], config[3]);
        var svnVerPath = Path.Combine(Application.dataPath, svnVer);
        var buildPath = Path.Combine(Application.dataPath, "../build/");
        RunLinuxShell(buildPath + "SaveSvnVersion.sh", Application.dataPath, svnVerPath);
    }

    private static void ConfigXcodeProject(string projectPath)
    {
        string projectname = projectPath + "/" + CN_APP_NAME + "/Unity-iPhone.xcodeproj";
        string plistPath = Path.GetFullPath(projectname);
        XCProject project = new XCProject(projectname);

        string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "Plugins/iOSProjectMods"), "*.projmods", SearchOption.AllDirectories);
        {
            var __array1 = files;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var file = (string)__array1[__i1];
                {
                    project.ApplyMod(file);
                }
            }
        }
        project.overwriteBuildSetting("GCC_ENABLE_CPP_EXCEPTIONS", "YES", "Release");
        project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", "YES", "Release");
        project.overwriteBuildSetting("GCC_ENABLE_CPP_RTTI", "YES", "Release");
        project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");

        project.overwriteBuildSetting("GCC_ENABLE_CPP_EXCEPTIONS", "YES", "Debug");
        project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", "YES", "Debug");
        project.overwriteBuildSetting("GCC_ENABLE_CPP_RTTI", "YES", "Debug");
        project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");

		//debug generate dSYM
		project.overwriteBuildSetting ("DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym", "Debug");

        plistPath = Path.Combine(projectPath + "/" + CN_APP_NAME, "info.plist");
        XCPlist list = new XCPlist(plistPath);

		list.ReplaceKey ("<string>en</string>", "<string>zh_CN</string>");

        string plistAdd = @"	
	<key>NSAppTransportSecurity</key>
	<dict>
		<key>NSAllowsArbitraryLoads</key>
		<true/>
	</dict>
	<key>LSApplicationQueriesSchemes</key>
	<array>
		<string>sinaweibo</string>
		<string>sinaweibohd</string>
		<string>sinaweibosso</string>
		<string>sinaweibohdsso</string>
		<string>weibosdk</string>
		<string>weibosdk2.5</string>
		<string>wechat</string>
		<string>weixin</string>
		<string>fbauth2</string>
	</array>
	<key>CFBundleURLTypes</key>
	<array>
		<dict>
			<key>CFBundleTypeRole</key>
			<string>Editor</string>
			<key>CFBundleURLSchemes</key>
			<array>
				<string>wb568898243</string>
			</array>
		</dict>
		<dict>
			<key>CFBundleTypeRole</key>
			<string>Editor</string>
			<key>CFBundleURLSchemes</key>
			<array>
				<string>wx4868b35061f87885</string>
			</array>
		</dict>
	</array>";


        list.AddKey(plistAdd);
        list.Save();
        project.Save();

        CopyLunchImage(projectPath);
    }

    private static void processOutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
    {
        Debug.Log(e.Data);
    }

    private static void BuildToAndroid()
    {
        BuildToAndroidImpl(BuildOptions.None, "Uborm");
    }

    private static void BuildToAndroidDevelopment()
    {
        BuildToAndroidImpl(BuildOptions.Development | BuildOptions.ConnectWithProfiler, "Uborm");
    }

	private static void BuildToAndroidDevelopmentWithoutProcessingResources()
	{
        BuildToAndroidImpl(BuildOptions.Development | BuildOptions.ConnectWithProfiler, "Uborm", false);
	}

    private static void BuildToAndroidBaidu()
    {
        BuildToAndroidImpl(BuildOptions.Development | BuildOptions.ConnectWithProfiler, "BaiDu");
    }

    private static void BuildToAndroidQh360()
    {
        BuildToAndroidImpl(BuildOptions.None, "Qh360");
    }

    //编译成Android程序
    [MenuItem("Build/Build Android")]
    private static void BuildToAndroidUborm()
    {
        BuildToAndroidImpl(BuildOptions.None, "Uborm");
    }

    //编译成Android程序
    [MenuItem("Build/Build Android Quickly")]
    private static void BuildToAndroidUbormQuickly()
    {
        BuildToAndroidImpl(BuildOptions.None, "Uborm", false);
    }

    [MenuItem("Build/Build Android Sjoys")]
    private static void BuildToAndroidStarJoys()
    {
        BuildToAndroidImpl(BuildOptions.None, "StarJoys");
    }

    [MenuItem("Build/Build Android Sjoys Quickly")]
    private static void BuildToAndroidStarJoysQuickly()
    {
        BuildToAndroidImpl(BuildOptions.None, "StarJoys", false); 
    }

    [MenuItem("Build/Build Android Sjoys Test")]
    private static void BuildToAndroidStarJoysTest()
    {
        BuildToAndroidImpl(BuildOptions.None, "StarJoysTest");
    }

    [MenuItem("Build/Build Android Sjoys Test Quickly")]
    private static void BuildToAndroidStarJoysTestQuickly()
    {
        BuildToAndroidImpl(BuildOptions.None, "StarJoysTest", false);
    }

    private static void ProcessUpdateResources()
    {
        ProcessUpdateResourcesImpl("StarJoysTest");
    }

    private static void ProcessUpdateResourcesImpl(string channel)
    {
        var versionPath = string.Format("../../Public/Version/Cn/Android/{0}/Game.ver", channel);
        GameVersionPath = Path.Combine(Application.dataPath, versionPath);
        string AssetBundlePath = Path.Combine(Application.dataPath, "BundleAsset");

        AssetBundleManagerEditor.RefreshDirectory();
        AssetBundleManagerEditor.RebuildAll();

    }

    private static void BUuildToAndroidmmy()
    {
        BuildToAndroidImpl(BuildOptions.Development | BuildOptions.ConnectWithProfiler, "mmy", false);
    }

    private static void BuildToAndroidkuaifa()
    {
        BuildToAndroidImpl(BuildOptions.None, "kuaifa");
    }

    private static void BuildToAndroidImpl(BuildOptions options,string channel, bool processResource = true)
    {
        BuildTarget buildTarget = BuildTarget.Android;
        PlayerSettings.Android.bundleVersionCode = CN_BUNDLE_VERSION_CODE;

        string versionPath = string.Format("../../Public/Version/Cn/Android/{0}/Game.ver", channel);

        GameVersionPath = Path.Combine(Application.dataPath, versionPath);

        ConfigVersionFile();

        var  bundle_identifier = CN_BUNDLE_IDENTIFIER;
        if (channel.Equals("StarJoys") || channel.Equals("StarJoysTest"))
        {
            bundle_identifier = "com.uborm.starjoys";
        }
        else
        {
            bundle_identifier = string.Format("{0}{1}{2}", bundle_identifier, ".", channel);
        }

        SetupPlayerSettings(bundle_identifier, CN_BUNDLE_VERSION, BuildTargetGroup.Android,
            CN_COMMON_DEFINE_SYMBOLS);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
        EditorUserBuildSettings.androidBuildSubtarget = AndroidBuildSubtarget.ETC2;
        ProcessPlugins("android", channel);
        ProcessResourcesImpl(processResource);
        string outPutDir = Application.dataPath.Replace("/Assets", "") + "/Android_Test";
        string outPutName = CN_APP_NAME + ".apk";
        if (Directory.Exists(outPutDir))
        {
            if (File.Exists(outPutName))
            {
                File.Delete(outPutName);
            }
        }
        else
        {
            Directory.CreateDirectory(outPutDir);
        }

        GenericBuild(FindEnabledEditorScenes(), outPutDir + "/" + outPutName, buildTarget, options, true);
    }

    //查找所有需要被编译的场景
    private static string[] FindEnabledEditorScenes()
    {
        Dictionary<string,int> scenesInPackage = new Dictionary<string, int>();
        scenesInPackage.Add("Assets/Startup.unity",1);
        scenesInPackage.Add("Assets/Loading.unity", 1);

        List<string> EditorScenes = new List<string>();
        {
            // foreach(var scene in EditorBuildSettings.scenes)
            var __enumerator2 = (EditorBuildSettings.scenes).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var scene = (EditorBuildSettingsScene)__enumerator2.Current;
                {
                    if (scenesInPackage.ContainsKey(scene.path))
                    {
                        EditorScenes.Add(scene.path);
                    }
                }
            }
        }
        return EditorScenes.ToArray();
    }

    

    //通用的编译方法
    private static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target,
        BuildOptions build_options, bool encrypted = true)
    {
        string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);

        if (res.Length > 0)
        {
            Logger.Error(res);
        }
        else 
        {
            if (build_target == BuildTarget.Android)
            {
                if (encrypted)
                {
                    EncryptedDll();
                    GenericBuild(scenes, target_dir, build_target, build_options, false);
                }
                else
                {
                    BackUpDll();
                    CleanAndroidPlugins();
                    Logger.Debug("GenericBuild Success path = {0}", target_dir);
                }
            }
            else
            {
                Logger.Debug("GenericBuild Success path = {0}", target_dir);
            }

        }

    }

    private static void BackUpDll()
    {
        var dll = Path.Combine(Application.dataPath,
            "Plugins/Android/assets/bin/Data/Managed/Assembly-CSharp.dll");
        var dll2 = Path.Combine(Application.dataPath,
            "Plugins/Android/assets/bin/Data/Managed/GameDataDefine.dll");

        if (!File.Exists(dll))
        {
            Logger.Error(" error dll file doesn't exist :" + dll);
            throw new Exception("error dll file doesn't exist :" + dll);
        }

        var backup = Path.Combine(DllBackupPath, "../dllbackup/a.bytes");
        var backup2 = Path.Combine(DllBackupPath, "../dllbackup/c.bytes");

        Logger.Debug("copy dll to backup path= " + backup);
        CheckTargetPath(backup);
        CheckTargetPath(backup2);

        File.Copy(dll, backup, true);
        File.Copy(dll2, backup2, true);
    }

    private static void CleanAndroidPlugins()
    {
        var destPath = Path.Combine(Application.dataPath, "Plugins/Android");

        if (Directory.Exists(destPath))
        {
            Directory.Delete(destPath, true);
        }

        Directory.CreateDirectory(destPath);
    }
    //处理插件
    private static void ProcessPlugins(string targetPlatform, string releaseName)
    {
        if (targetPlatform.Equals("ios"))
        {
            return;
        }

        if (targetPlatform.Equals("android"))
        {
            string pluginPath;
            var destPath = Path.Combine(Application.dataPath, "Plugins/Android");
            CleanAndroidPlugins();
            if (releaseName == "BaiDu")
            {
                pluginPath = Path.Combine(Application.dataPath, "../../PlatformPlugin/Android/baidu");
            }
            else if (releaseName == "Qh360")
            {
                pluginPath = Path.Combine(Application.dataPath, "../../PlatformPlugin/Android/360");
            }
            else if (releaseName == "Uborm")
            {
                pluginPath = Path.Combine(Application.dataPath, "../../PlatformPlugin/Android/Uborm");
            }
            else if (releaseName == "mmy")
            {
                pluginPath = Path.Combine(Application.dataPath, "../../PlatformPlugin/Android/mmy");

                //拷贝木蚂蚁渠道图标
                var iconPath = Path.Combine(Application.dataPath, "../../PlatformPlugin/AllSDK/ICONmmy.png");
                var targetName = Path.Combine(Application.dataPath, "ArtAsset/GameIcon/icon.png");
                if (File.Exists(iconPath))
                {
                    File.Copy(iconPath, targetName, true);
                }
            }
            else if (releaseName == "kuaifa")
            {
                pluginPath = Path.Combine(Application.dataPath, "../../PlatformPlugin/Android/kuaifa");
            }
            else if (releaseName == "StarJoys" || releaseName == "StarJoysTest")
            {
                pluginPath = Path.Combine(Application.dataPath, "../../PlatformPlugin/Android/StarJoys");
            }
            else
            {
                return;
            }

            var res1 = Path.Combine(pluginPath, "assets");
            var des1 = Path.Combine(destPath, "assets");
            DirectoryCopy(res1, des1, true);

            var res2 = Path.Combine(pluginPath, "libs");
            var des2 = Path.Combine(destPath, "libs");
            DirectoryCopy(res2, des2, true);

            var res3 = Path.Combine(pluginPath, "res");
            var des3 = Path.Combine(destPath, "res");
            DirectoryCopy(res3, des3, true);

            var res4 = Path.Combine(pluginPath, "outputjar/PlatfromPlugin.jar");
            var des4 = Path.Combine(des2, "PlatfromPlugin.jar");
            File.Copy(res4, des4, true);

            var res5 = Path.Combine(pluginPath, "AndroidManifest.xml");
            var des5 = Path.Combine(destPath, "AndroidManifest.xml");
            File.Copy(res5, des5, true);

            //------------------------------copy base begain
            var basedir = Path.Combine(Application.dataPath, "../../PlatformPlugin/Android/Uborm");
            var baseRes2 = Path.Combine(basedir, "libs");
            DirectoryCopy(baseRes2, des2, true);
            //---------------------------copy base finish

            var res6 = Path.Combine(destPath, "libs/classes.jar");
            if (File.Exists(res6))
            {
                File.Delete(res6);
            }
        }
    }

    private static void ProcessResourcesImpl(bool bRebuildBundle)
    {
        string AssetBundlePath = Path.Combine(Application.dataPath, "BundleAsset");
        if (bRebuildBundle)
        {
            if (Directory.Exists(AssetBundlePath))
            {
                Directory.Delete(AssetBundlePath, true);
                Directory.CreateDirectory(AssetBundlePath);
            }
            AssetBundleManagerEditor.RefreshDirectory();
            AssetBundleManagerEditor.RebuildAll();
        }
        else
        {
            var data = AssetBundleManager.GetManageData(false);
            var index = 0;
            {
                var __list3 = data.assetbundles;
                var __listCount3 = __list3.Count;
                for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var assetBundleData = __list3[__i3];
                    {
                        if (assetBundleData.File.Contains("Script"))
                        {
                            AssetBundleManagerEditor.CreateAssetBundleWithDependency(data, index, true);
                            break;
                        }
                        index++;
                    }
                }
            }
        }

        string streamAssetsPath = Application.streamingAssetsPath;
        if (Directory.Exists(streamAssetsPath))
        {
            Directory.Delete(streamAssetsPath, true);
        } 
        Directory.CreateDirectory(streamAssetsPath);

        //是不是要打小包
        string path = Path.Combine(Application.dataPath, "../../thinpackage.txt");
        if (File.Exists(path))
        {
            s_IsThinPackage = true;
            File.Delete(path);
        }
        else
        {
            s_IsThinPackage = false;
        }

        if (s_IsThinPackage)
        {
            ModifyVersionFile(GameVersionPath, 1);
            ProcessThinPackageResource();
            string destPath = Path.Combine(Application.streamingAssetsPath, "Game.ver");
            File.Copy(GameVersionPath, destPath, true);
            if (bRebuildBundle)
            {
                ModifyVersionFile(GameVersionPath, 1);
            }
        }
        else
        {
            DirectoryCopy(AssetBundlePath, streamAssetsPath, true);
            if (bRebuildBundle)
            {
                ModifyVersionFile(GameVersionPath, 1);
            }
            string destPath = Path.Combine(Application.streamingAssetsPath, "Game.ver");
            File.Copy(GameVersionPath, destPath, true);
        }

        string destPath3 = Path.Combine(Application.streamingAssetsPath, "logo.mp4");
        string mp4Path = Path.Combine(Application.dataPath, "../../PlatformPlugin/logo.mp4");
        if (File.Exists(mp4Path))
        {
            File.Copy(mp4Path, destPath3, true);
        }

        ToLuaMenu.BuildNotJitBundles();
        DirectoryCopy(Path.Combine(Application.streamingAssetsPath, LuaConst.osDir),
            Path.Combine(AssetBundlePath, LuaConst.osDir), true);

        //把需要更新文件备份
        string assetBundlePath = Path.Combine(Application.dataPath, "BundleAsset");
        string verInfo = File.ReadAllText(GameVersionPath).Trim();
        var config = verInfo.Split(',');
        var backupPathStr = string.Format("../../Update/{0}/{1}/{2}/{3}/BundleAsset", config[0], config[1], config[2], config[3]);
        var backupPath = Path.Combine(Application.dataPath, backupPathStr);
        CheckTargetPath(backupPath);

        if (Directory.Exists(backupPath))
        {
            Directory.Delete(backupPath, true);
        }
        DirectoryCopy(assetBundlePath, backupPath, true);
        DllBackupPath = backupPath;
    }


    static void EncryptedDll()
    {
        var dllPath = Path.Combine(Application.dataPath,
            "../Temp/StagingArea/assets/bin/Data/Managed/Assembly-CSharp.dll");
        var dllDestPath = Path.Combine(Application.dataPath,
            "Plugins/Android/assets/bin/Data/Managed/Assembly-CSharp.dll");

        EncrypteByPath(dllPath, dllDestPath);

        Logger.Debug("EncryptedDll Assembly-CSharp.dll");

        var dllPath2 = Path.Combine(Application.dataPath,
            "../Temp/StagingArea/assets/bin/Data/Managed/GameDataDefine.dll");
        var dllDestPath2 = Path.Combine(Application.dataPath,
            "Plugins/Android/assets/bin/Data/Managed/GameDataDefine.dll");

        EncrypteByPath(dllPath2, dllDestPath2);

        Logger.Debug("EncryptedDll GameDataDefine.dll");
    }

    static void GenerateAssemblyCSharpToRes(string verPath)
    {
        EasyBuild();
        EncryptedDll();

        var dllDestPath = Path.Combine(Application.dataPath,
            "Plugins/Android/assets/bin/Data/Managed/Assembly-CSharp.dll");

        var dllDestPath2 = Path.Combine(Application.dataPath,
            "Plugins/Android/assets/bin/Data/Managed/GameDataDefine.dll");
        var bundleDllPath1 = Path.Combine(Application.dataPath, "Res/dll/a.bytes");
        var bundleDllPath2 = Path.Combine(Application.dataPath, "Res/dll/c.bytes");
        CheckTargetPath(bundleDllPath1);

        File.Copy(dllDestPath, bundleDllPath1, true);
        File.Copy(dllDestPath2, bundleDllPath2, true);
        File.Delete(dllDestPath);
        File.Delete(dllDestPath2);

        Logger.Debug("copy dll to a.bytes");
        Logger.Debug("copy dll to c.bytes");

        var verInfo = File.ReadAllText(verPath).Trim();
        var config = verInfo.Split(',');
        var name1 = string.Format("../dllbackup/a{0}.bytes", config[4]);
        var name2 = string.Format("../dllbackup/c{0}.bytes", config[4]);
        var backup = Path.Combine(DllBackupPath, name1);
        var backup2 = Path.Combine(DllBackupPath, name2);

        Logger.Debug("copy dll to backup path= " + backup);
        CheckTargetPath(backup);
        File.Copy(bundleDllPath1, backup, true);
        File.Copy(bundleDllPath2, backup2, true);

        Logger.Debug("copy dll to backup:" + backup);
        Logger.Debug("copy dll to backup:" + backup);
    }

    static void EncrypteByPath(string dllPath, string dllDestPath)
    {
        UpdateHelper.CheckTargetPath(dllDestPath);
        File.Copy(dllPath, dllDestPath, true);
        var data = File.ReadAllBytes(dllDestPath);
        encrypte(data);
        File.WriteAllBytes(dllDestPath, data);
    }
    static void encrypte(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if ((i & 0xFFF00) != 0)
            {
                data[i] ^= 0xEA;
            }
        }
    }


    private static void LoadWhiteList()
    {
        whiteListDirectorys.Clear();
        whiteListFils.Clear();
        var pathDir = Path.Combine(Application.dataPath, "Res/Table/BundleWhiteList/Directorys.txt");
        var pathBundle = Path.Combine(Application.dataPath, "Res/Table/BundleWhiteList/BundleFiles.txt");
        var dirs = File.ReadAllLines(pathDir);


        //需要处理的文件夹
        foreach (var dir in dirs)
        {
            var str = dir.Trim();
            if (str.StartsWith("//") || string.IsNullOrEmpty(str))
            {
                continue;
            }

            whiteListDirectorys.Add(str);
        }

        //需要命中的文件
        var bundlePaths = File.ReadAllLines(pathBundle);
        foreach (var path in bundlePaths)
        {
            var str = path.Trim();
            if (str.StartsWith("//") || string.IsNullOrEmpty(str))
            {
                continue;
            }

            whiteListFils.Add(str);
        }
    }


    public static void ProcessThinPackageResource()
    {
        //读取白名单
        LoadWhiteList();

        string verInfo = File.ReadAllText(GameVersionPath).Trim();
        var config = verInfo.Split(',');
        string streamAssetsPath = Application.streamingAssetsPath;
        string assetBundlePath = Path.Combine(Application.dataPath, "BundleAsset");
        var rootPath = "Assets";
        var md5ListPathStr = string.Format("../../Update/{0}/{1}/{2}/{3}/Md5list", config[0], config[1], config[2], config[3]);
        var md5ListPath = Path.Combine(rootPath, md5ListPathStr);
        CheckTargetPath(md5ListPath);
        fileList.Clear();
        index = 1;
        var callBack = new Action<FileInfo>(GeneratorMd5);
        var bundlePath = Path.Combine(rootPath, "BundleAsset");
        ForeachFile(bundlePath, callBack);
        var resourceVersion = config[4];
        var md5ListFile = Path.Combine(md5ListPath, "md5list" + resourceVersion + ".txt");
        var textWriter = new StreamWriter(md5ListFile, false, Encoding.UTF8);

        if (Directory.Exists(streamAssetsPath))
        {
            Directory.Delete(streamAssetsPath, true);
        }
        Directory.CreateDirectory(streamAssetsPath);

        foreach (var file in fileList)
        {
            if (file == null)
            {
                continue;
            }
            if (file.Trim().IndexOf(" ", StringComparison.Ordinal) >= 0)
            {
                throw new Exception("文件名不能有空格!!!!" + file);
            }


            var strList = file.Split(',');
            var path = strList[1];
            var idx = path.IndexOf('/');
            var dir = idx == -1 ? string.Empty : path.Substring(0, idx);
            bool copyToPackage = true;

            if (!string.IsNullOrEmpty(dir) && whiteListDirectorys.Contains(dir))
            {
                copyToPackage = whiteListFils.Contains(path);
            }

            if (copyToPackage)
            {
                textWriter.WriteLine(file);
                CopyFileByPath(assetBundlePath, streamAssetsPath, strList[1]);
            }
        }
        textWriter.Close();
    }

    private static void CopyFileByPath(string assert, string stream, string filePath)
    {
        DirectoryInfo dir = new DirectoryInfo(assert);
        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the source directory does not exist, throw an exception.
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + assert);
        }

        var paths = filePath.Split('/');
        var fullpath = assert;
        var length = paths.Length;
        var fileName = paths[length - 1];
        var targetPath = stream;
        for (int i = 0; i < length - 1; i++)
        {
            fullpath = Path.Combine(fullpath, paths[i]);
            targetPath = Path.Combine(targetPath, paths[i]);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
        }
        fullpath = Path.Combine(fullpath, fileName);
        targetPath = Path.Combine(targetPath, fileName);
        var file = new FileInfo(fullpath);

        file.CopyTo(targetPath);
    }
     
    // ----------------Md5------------
    public const string separator = ",";
    static List<string> fileList = new List<string>();
    static int index = 1;
    public static void CheckTargetPath(string targetPath)
    {
        targetPath = targetPath.Replace('\\', '/');

        int dotPos = targetPath.LastIndexOf('.');
        int lastPathPos = targetPath.LastIndexOf('/');

        if (dotPos > 0 && lastPathPos < dotPos)
        {
            targetPath = targetPath.Substring(0, lastPathPos);
        }
        if (Directory.Exists(targetPath))
        {
            return;
        }

        string[] subPath = targetPath.Split('/');
        string curCheckPath = "";
        int subContentSize = subPath.Length;
        for (int i = 0; i < subContentSize; i++)
        {
            curCheckPath += subPath[i] + '/';
            if (!Directory.Exists(curCheckPath))
            {
                Directory.CreateDirectory(curCheckPath);
            }
        }
    }
    public static void GeneratorMd5(FileInfo info)
    {
        if (info.FullName.Contains(".meta"))
        {
            return;
        }
        var fileStream = info.OpenRead();
        var fileIndex = index++.ToString();
        string fileFullName = info.FullName.Replace('\\', '/');
        int extidx = fileFullName.LastIndexOf("BundleAsset/");
        var fileName = fileFullName.Substring(extidx + 12);
        var fileMd5 = GetMd5Hash(fileStream);
        var fileSize = info.Length.ToString();
        fileList.Add(fileIndex + separator + fileName + separator + fileMd5 + separator + fileSize);
        fileStream.Close();
    }
    private static void ForeachFile(string sourceDirName, System.Action<FileInfo> action)
    {
        var dirinfo = new DirectoryInfo(sourceDirName);
        var dirinfos = dirinfo.GetDirectories();
        if (!dirinfo.Exists)
        {
            throw new DirectoryNotFoundException("directory does not exist!!" + sourceDirName);
        }
        var files = dirinfo.GetFiles();
        foreach (var file in files)
        {
            action(file);
        }
        foreach (var info in dirinfos)
        {
            ForeachFile(info.FullName, action);
        }
    }
    private static string GetMd5Hash(FileStream fs)
    {
        var md5 = new MuUtility.MD5();
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        fs.Seek(0, SeekOrigin.Begin);
        md5.ValueAsByte = bytes;

        return md5.FingerPrint.ToLower();
    }

    private static bool showDialogMessage()
    {
        return EditorUtility.DisplayDialog("注意！", "将要切换平台花费很长时间是否开始？", "开始", "取消");
    }

    private static void DirectoryCopy(
        string sourceDirName, string destDirName, bool copySubDirs)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the source directory does not exist, throw an exception.
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        // If the destination directory does not exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }


        // Get the file contents of the directory to copy.
        FileInfo[] files = dir.GetFiles();
        {
            var __array4 = files;
            var __arrayLength4 = __array4.Length;
            for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
            {
                var file = (FileInfo)__array4[__i4];
                {
                    // Create the path to the new copy of the file.
                    string temppath = Path.Combine(destDirName, file.Name);
                    if (Path.GetExtension(temppath).Equals(".meta"))
                    {
                        continue;
                    }
                    // Copy the file.
                    file.CopyTo(temppath, true);
                }
            }
        }
        // If copySubDirs is true, copy the subdirectories.
        if (copySubDirs)
        {
            {
                var __array5 = dirs;
                var __arrayLength5 = __array5.Length;
                for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
                {
                    var subdir = (DirectoryInfo)__array5[__i5];
                    {
                        // Create the subdirectory.
                        string temppath = Path.Combine(destDirName, subdir.Name);

                        // Copy the subdirectories.
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
        }
    }

    //ios上的launchimage,unity自带的转换完了不够清晰，所以提前转换好生成工程后拷贝过去
    private static void CopyLunchImage(string projectPath)
    {
        var targetPath = Path.Combine(projectPath, CN_APP_NAME + "/Unity-iPhone/Images.xcassets/LaunchImage.launchimage/");
        var imagePath = Path.Combine(Application.dataPath, "../../Public/LaunchImage/output/");
        DirectoryCopy(imagePath, targetPath, false);
    }

    [MenuItem("Build/BuildControllerForMobile")]
    public static void BuildControllerForMobile()
    {
        var path = Path.Combine(Application.dataPath, "../build/BuildControllerForMobile.bat");
        var path2 = Path.Combine(Application.dataPath, "../build/CopyController.bat");
        RunWindowsBat(path);
        RunWindowsBat(path2);
    }

    public static void RunWindowsBat(string path)
    {
#if UNITY_EDITOR_WIN
        if (!File.Exists(path))
        {

            EditorUtility.DisplayDialog("注意！", "没有找到文件:" + path, "退出");
            return;
        }
        Process pro = new Process();
        FileInfo file = new FileInfo(path);
        pro.StartInfo.WorkingDirectory = file.Directory.FullName;
        pro.StartInfo.FileName = path;
        pro.StartInfo.CreateNoWindow = false;
        pro.Start();
        pro.WaitForExit();
#endif
    }

    [MenuItem("Build/PrintDebugLog")]
    private static void WriteDebugLog()
    {
        BundleLoader.Instance.PrintDebugLogToFile();
    }

    private static void ModifyVersionFile(string path, int change)
    {
        string verInfo = File.ReadAllText(path).Trim();
        var config = verInfo.Split(',');
        var resourceVersion = config[4];
        StringBuilder sb = new StringBuilder();
        sb.Append(config[0]);
        sb.Append(',');
        sb.Append(config[1]);
        sb.Append(',');
        sb.Append(config[2]);
        sb.Append(',');
        sb.Append(config[3]);
        sb.Append(',');
        int versionNum = 0;
        if (int.TryParse(resourceVersion, out versionNum))
        {
            versionNum += change;
        }
        else
        {
            throw new Exception("Game.ver is error!!");
        }
        sb.Append(versionNum);
        File.WriteAllText(path, sb.ToString());
    }


    #region 处理更新包相关

    public static void RunLinuxShell(string shellPath, params object[] args)
    {
#if UNITY_EDITOR_WIN
        return;
#endif
        if (!File.Exists(shellPath))
        {

            EditorUtility.DisplayDialog("注意！", "没有找到文件:" + shellPath, "退出");
            return;
        }
        Process pro = new Process();
        FileInfo file = new FileInfo(shellPath);
        pro.StartInfo.FileName = "bash";
        pro.StartInfo.WorkingDirectory = file.Directory.FullName;
        pro.StartInfo.CreateNoWindow = true;
        pro.StartInfo.UseShellExecute = false;
        pro.StartInfo.RedirectStandardOutput = true;
        pro.StartInfo.RedirectStandardError = true;
        if (args == null)
        {
            pro.StartInfo.Arguments = shellPath;
        }
        else
        {
            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.Append(" ");
                sb.Append(arg);
            }
            pro.StartInfo.Arguments = shellPath + sb;
        }

        pro.Start();
        var result = pro.StandardOutput.ReadToEnd();
        var error = pro.StandardError.ReadToEnd();
        Logger.Debug("result = {0}", result);
        if (!string.IsNullOrEmpty(error))
        {
            throw new Exception("运行脚本异常：" + error);
        }
        pro.WaitForExit();
    }

    private static void MakeUpdatePackage(string platform, string channel, string version, string cSharp, string script)
    {
        if (platform.Equals("Android"))
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
            EditorUserBuildSettings.androidBuildSubtarget = AndroidBuildSubtarget.ETC2;
        }

        //config读取
        var buildPath = Path.Combine(Application.dataPath, "../build/");
        var versionPath = string.Format("../../Public/Version/Cn/{0}/{1}/Game.ver", platform, channel);
        var versionFullPath = Path.Combine(Application.dataPath, versionPath);
        var verInfo = File.ReadAllText(versionFullPath).Trim();
        var config = verInfo.Split(',');
        var ver = config[3];
        if (!string.IsNullOrEmpty(version))
        {
            ver = version;
        }
        ver = ver.Trim().Replace(Environment.NewLine, "");


        //设置updatepath
        var backupPathStr = string.Format("../../Update/{0}/{1}/{2}/{3}/BundleAsset", config[0], config[1], config[2], ver);
        var backupPath = Path.Combine(Application.dataPath, backupPathStr);
        var assetBundlePath = Path.Combine(Application.dataPath, "BundleAsset");
        DllBackupPath = backupPath;



        var svnVer = string.Format("../../Update/{0}/{1}/{2}/{3}/svn.ver", config[0], config[1], config[2], ver);
        var svnVerPath = Path.Combine(Application.dataPath, svnVer);
        if (!File.Exists(svnVerPath))
        {
            throw new Exception("找不到上次svn版本号文件"+ svnVerPath);
        }

        ModifyVersionFile(versionFullPath, 1);

        if (cSharp.Equals("1"))
        {
            GenerateAssemblyCSharpToRes(versionFullPath);
        }

        var svnVersion = File.ReadAllText(svnVerPath);
        RunLinuxShell(buildPath + "Getsvndiff.sh", svnVersion, Application.dataPath);
        var diffPath = Path.Combine(buildPath, "changelist.txt");
        if (!File.Exists(diffPath))
        {
            throw new Exception("生成svndiff失败！！");
        }

        var difflist = GetDiffList(diffPath);

        AssetBundleManagerEditor.CreateAssetBundleWithDiffList(difflist, cSharp.Equals("1"), script.Equals("1"));

        Logger.Debug("CreateAssetBundleWithDiffList finish!");


        RunLinuxShell(buildPath + "SaveSvnVersion.sh", Application.dataPath, svnVerPath);


        DirectoryCopy(assetBundlePath, backupPath, true);

        CleanAndroidPlugins();
    }

    private static void EasyBuild()
    {
        string outPutDir = Application.dataPath.Replace("/Assets", "") + "/Android_Test/mayanouse.apk";

        if (File.Exists(outPutDir))
        {
            File.Delete(outPutDir);
        }

        //为的是先删除插件目录，否则会把插件目录内容拷贝到生成目录
        CleanAndroidPlugins();

        CheckTargetPath(outPutDir);
        var res = BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), outPutDir,  BuildTarget.Android, BuildOptions.None);

        if (res.Length > 0)
        {
            throw new Exception("编译代码错误！！！");
        }

        Logger.Debug("easybuild success!!!");
    }

    private static List<string> GetDiffList(string path)
    {
        var list = new List<string>();

        using (var fs = new FileStream(path, FileMode.Open))
        {
            using (var sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    var line = GetPathFromSvnDiffLine(sr.ReadLine());
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    line = line.Replace("\\", "/");
                    line = line.Substring(line.IndexOf("Assets"));
                    //var dataPath = Application.dataPath.Replace("\\", "/");
                    list.Add(line);
                }
            }
        }
        return list;
    }

    private static string GetPathFromSvnDiffLine(string line)
    {
        var ret = string.Empty;
        if (!string.IsNullOrEmpty(line))
        {
            line = line.Trim();
            if (line.StartsWith("M") || line.StartsWith("A"))
            {
                var charArray = line.ToCharArray();
                for (int i = 1; i < charArray.Length; i++)
                {
                    var onechar = charArray[i];
                    if (onechar != ' ')
                    {
                        ret = line.Substring(i, line.Length - i);
                        break;
                    }
                }
            }
        }

        return ret;
    }
    #endregion


    [MenuItem("Tools/CheckSceneResources")]
    private static void CheckSceneResource()
    {
        var dir1 = Application.dataPath + "/Res/Scene/";
        var dir2 = Application.dataPath + "/Res/Terrain/";
        var dir3 = Application.dataPath + "/Res/TerrainMeshTree/";

        var list1 = GetFileNameWithoutExtList(dir1);
        var list2 = GetFileNameWithoutExtList(dir2);
        var list3 = GetFileNameWithoutExtList(dir3);

        var diff = list1.Except(list2).ToList();
        var diff2 = list1.Except(list3).ToList();


        var sb = new StringBuilder();
        if (diff.Count > 0)
        {
            foreach (var str in diff)
            {
              sb.AppendLine("发现Terrain和Scene不匹配内容" + str); 
            }
        }

        if (diff2.Count > 0)
        {
            foreach (var str in diff2)
            {
                sb.AppendLine("发现TerrainMeshTree和Scene不匹配内容" + str);
            }
        }

        Logger.Debug(sb.Length == 0 ? "没有发现问题！" : sb.ToString());
    }

    private static List<string> GetFileNameWithoutExtList(string dirName)
    {
        var list = new List<string>();
        ForeachFile(dirName, info =>
        {
            if (info.Name.Contains(".meta"))
            {
                return;
            }

            if (info.Name.Contains("Login"))
            {
                return;
            }

            if (info.Name.Contains("SelectCharacter"))
            {
                return;
            }

            list.Add(Path.GetFileNameWithoutExtension(info.Name));
        });

        return list;
    }
}