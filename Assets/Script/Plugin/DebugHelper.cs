#region using

using EventSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using Console = DevConsole.Console;

#endregion

public class DebugHelper : MonoBehaviour
{
    public static GameObject helperInstance;
    public static bool m_bEnableTestAccount = false;
    public static bool m_bShowDamageBoard = true;
    public static bool m_bShowEffect = true;
    private float _heightValue = 80f;
    private float _widthValue = 300f;
    private readonly List<int> CullingMaskList = new List<int>();
    private bool DisplayAll = true;
    private Rect guiRect = new Rect(0, 100, 400, 600);
    private int HardwareScalerLevel;
    private bool m_bUseOtherFun;
    private string m_strSetNameHeight = "";
    private string mGMCommond = "GM Commond";
    private int mOriginHeight;
    private int mOriginWidth;
    private int scaleHeight;
    private int scaleWidth;
    private GUIStyle style = new GUIStyle();
    private GameObject uiRoot = null;
    private int urlIndex;

	private bool ShowDebugHelper = true;

    private readonly string[] urls =
    {
        "http://www.baidu.com",
        "192.168.0.209:3000/Index.html"
    };

    public static void CreateDebugHelper()
    {
        if (!PlatformHelper.IsEnableDebugMode())
        {
            return;
        }
        if (null == helperInstance)
        {
            ResourceManager.PrepareResource<GameObject>("UI/DebugHelper.prefab",
                res => { helperInstance = Instantiate(res) as GameObject; });
        }
    }

    private void OnGUI()
    {
	    if (!ShowDebugHelper)
	    {
		    return;
	    }

        if (GUILayout.Button("door", GUILayout.Height(_heightValue - 10), GUILayout.Width(_widthValue - 50)))
        {
            m_bUseOtherFun = !m_bUseOtherFun;
	        if (!UIManager.Instance.UIRoot.activeSelf)
	        {
				var e = new MainUiOperateEvent(0);
				EventDispatcher.Instance.DispatchEvent(e);
	        }
        }

	    GUI.depth = 10000;
        if (m_bUseOtherFun)
        {
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
			if (GUILayout.Button("HideUI", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
			{
				if (UIManager.Instance.UIRoot != null)
				{
					UIManager.Instance.UIRoot.SetActive(!UIManager.Instance.UIRoot.activeSelf);
				}
            }

            GUILayout.Space(30);
            if (GUILayout.Button("FPS unlimit ", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                Application.targetFrameRate = 9;
            }

            GUILayout.Space(30);
            if (GUILayout.Button("Clear Cache", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                ResourceManager.Instance.Reset();
                Resources.UnloadUnusedAssets();
                GC.Collect(0, GCCollectionMode.Forced);
            }

            // mGMCommond = GUI.TextField(new Rect(Screen.width - 200, _heightValue, 200, _heightValue), mGMCommond, 25);
            GUILayout.EndHorizontal();
            GUILayout.Space(30);


            GUILayout.BeginHorizontal();

            if (GUILayout.Button(string.Format("GM热键(`)"), GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                Console.Show();

//                 if (mGMCommond.Length > 0)
//                 {
//                     StartCoroutine(SendGmCOmmond());
//                 }
            }


            GUILayout.Space(30);
            if (GUILayout.Button(string.Format("开关特效"), GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                var bloom = GameLogic.Instance.MainCamera.GetComponent<DistortionAndBloom>();
                bloom.enabled = !bloom.enabled;

                //                 if (mGMCommond.Length > 0)
                //                 {
                //                     StartCoroutine(SendGmCOmmond());
                //                 }
            }

            GUILayout.Space(30);

            if (GUILayout.Button("LOW MEMORY", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                PlatformListener.Instance.OnLowMemory();
            }


            GUILayout.EndHorizontal();

            GUILayout.Space(30);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(string.Format("开关网络"), GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                if (NetManager.Instance.Connected)
                {
                    NetManager.Instance.Stop();
                }
                else
                {
                    NetManager.Instance.ReconnectToServer();
                }
            }

            GUILayout.Space(30);
            if (GUILayout.Button("Battery save test", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                if (DisplayAll)
                {
                    CullingMaskList.Clear();
                    for (var i = 0; i < Camera.allCameras.Length; ++i)
                    {
                        var cam = Camera.allCameras[i];
                        CullingMaskList.Add(cam.cullingMask);
                        cam.cullingMask = 0;
                    }
                }
                else
                {
                    for (var i = 0; i < Camera.allCameras.Length; ++i)
                    {
                        var cam = Camera.allCameras[i];
                        cam.cullingMask = CullingMaskList[i];
                    }
                }

                DisplayAll = !DisplayAll;
            }

            GUILayout.Space(30);

            if (GUILayout.Button("AvailMemory", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                var memory = PlatformHelper.GetAvailMemory();
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, "剩余内存为:" + memory + "MB", "");
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OpenUrl", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                var url = UpdateHelper.CheckUrl(urls[urlIndex%2]);
                Application.OpenURL(url);
                urlIndex++;
            }
            GUILayout.Space(30);

            if (GUILayout.Button("HDR", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                GameSetting.Instance.EnableHDR = !GameSetting.Instance.EnableHDR;
            }

            GUILayout.Space(30);

            if (GUILayout.Button("Hardware Scaler", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                if (mOriginWidth == 0)
                {
                    mOriginWidth = Screen.currentResolution.width;
                    mOriginHeight = Screen.currentResolution.height;

//                     int designWidth = 1280;
//                     int designHeight = 800;
// 
//                     float s1 = designWidth / (float)designHeight;
//                     float s2 = mOriginWidth / (float)mOriginHeight;
// 
//                     if (s1 < s2)
//                     {
//                         designWidth = Mathf.FloorToInt(designHeight * s2);
//                     }
//                     else if (s1 > s2)
//                     {
//                         designHeight = Mathf.FloorToInt(designWidth / s2);
//                     }
// 
//                     float contentScale = designWidth / (float)mOriginWidth;
//                     if (contentScale < 1f)
//                     {
//                         scaleWidth = designWidth;
//                         scaleHeight = designHeight;
//                         if (scaleWidth % 2 == 0)
//                         {
//                             scaleWidth += 1;
//                         }
//                         else
//                         {
//                             scaleWidth -= 1;
//                         }
//                     }
                }

                HardwareScalerLevel++;
                scaleWidth = Mathf.CeilToInt(mOriginWidth*(1 - 1.0f/(HardwareScalerLevel + 1)));
                scaleHeight = Mathf.CeilToInt(mOriginHeight*(1 - 1.0f/(HardwareScalerLevel + 1)));

                if (HardwareScalerLevel == 6)
                {
                    HardwareScalerLevel = 0;
                }

                if (HardwareScalerLevel != 0)
                {
                    Screen.SetResolution(scaleWidth, scaleHeight, true);
                    Logger.Debug("HandwareScaler enable!");
                }
                else
                {
                    Screen.SetResolution(mOriginWidth, mOriginHeight, true);
                    Logger.Debug("HandwareScaler disable!");
                }
            }



            GUILayout.EndHorizontal();

			GUILayout.Space(30);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("SPEED UP", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
			{
				var console = gameObject.GetComponent<DevConsole.Console>();
				console.PrintInput("!!SpeedSet,300");
			}

            GUILayout.Space(30);

            if (GUILayout.Button("RestartApp", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                PlatformHelper.RestartApp();
            }

            GUILayout.Space(30);

            if (GUILayout.Button("EnterStartup", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                Game.Instance.EnterStartup();
            }

			GUILayout.EndHorizontal();

            // 
            //             if (GUILayout.Button("PlayerSound", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
//             {
//                 bool bEnable = SoundManager.m_EnableBGM;
//                 bEnable = !bEnable;
//                 SoundManager.m_EnableBGM = bEnable;
//                 SoundManager.m_EnableSFX = bEnable;
//             }
//             
//             if (LoginData.m_bEnableTestAccount)
//             {
//                 if (GUILayout.Button("Disable TestAccount", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
//                 {
//                     LoginData.m_bEnableTestAccount = false;
//                 }
// 
//                 LoginData.m_strTestAccount = GUI.TextField(new Rect(Screen.width - 200, 0, 200, _heightValue), LoginData.m_strTestAccount, 15);
//             }
//             else
//             {
//                 if (GUILayout.Button("Enable TestAccount", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
//                 {
//                     LoginData.m_bEnableTestAccount = true;
//                 }
//             }
// 
//             if (GUILayout.Button("TerrainHeight", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
//             {
//                 Obj_MainPlayer objMain = Singleton<ObjManager>.GetInstance().MyPlayer;
//                 if (null != objMain)
//                 {
//                     if (null != GameManager.gameManager.ActiveScene &&
//                         null != GameManager.gameManager.ActiveScene.TerrainData)
//                     {
//                         float height = GameManager.gameManager.ActiveScene.TerrainData.GetTerrianHeight(objMain.Position);
//                         Logger.Debug("Terrain Heigt: " + height);
//                         return;
//                     }
//                 }
// 
//                 Logger.Debug("Get Terrain Height Error");
//             }

//             m_strSetNameHeight = GUI.TextField(new Rect(Screen.width - 200, _heightValue, 200, _heightValue), m_strSetNameHeight, 15);
//             if (GUILayout.Button("ChangeNameHeight", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
//             {               
//                 if (Singleton<ObjManager>.Instance.MyPlayer)
//                 {
//                     float fNewHeight;
//                     bool bResult = float.TryParse(m_strSetNameHeight, out fNewHeight);
//                     if (bResult)
//                     {
//                         Obj_Character target = Singleton<ObjManager>.Instance.MyPlayer.SelectTarget;
//                         if (null != target)
//                         {
//                             BillBoard billboard = target.HeadInfoBoard.GetComponent<BillBoard>();
//                             if (billboard != null)
//                             {
//                                 billboard.fDeltaHeight = fNewHeight;
//                             }
//                         }
// 
//                         //m_IsSetNameHeight = false;
//                     }
// 
//                 }
//             }
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        _widthValue = Screen.width*_widthValue/2000f;
        _heightValue = Screen.height*_heightValue/1000f;
#if !UNITY_EDITOR
		gameObject.active = false;
#endif

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

	public void Update()
	{
#if !UNITY_EDITOR
try
{
#endif

		if (Input.GetKeyUp(KeyCode.F12))
		{
			ShowDebugHelper = !ShowDebugHelper;
			if (UIManager.Instance.UIRoot != null)
			{
				UIManager.Instance.UIRoot.SetActive(ShowDebugHelper);
			}
			var fps = GameObject.FindObjectOfType<NcDrawFpsRect>();
			fps.enabled = ShowDebugHelper;
		}
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
}