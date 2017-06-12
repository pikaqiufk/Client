#region using

using System;
using System.Collections.Generic;
using ClientDataModel;
using DataContract;
using DataTable;
using EventSystem;
using UnityEngine;
// ReSharper disable All

#endregion

namespace GameUI
{
	public class ChooseRoleFrame : MonoBehaviour
	{
	    public static bool s_IsModelLoaded;
	    //创建界面的
	    public UIButton CloseCreateRoleFrame;
	    private GameObject curModel;
	    public GameObject CreateModelRoot;
	    public UIButton CreateRoleButton;
	    public List<UIEventTrigger> CreateTypeList;
	    //private Transform[] RoleModels = new Transform[3];
	    private Vector3 distantPos = new Vector3(0, 100, 0);
	    public int LastSelectIndex;
	    private int LastChooseType = -1;
	    private int loadCount;
	    public List<TweenPosition> CreateTweener1 = new List<TweenPosition>();
	    public List<TweenAlpha> CreateTweener2 = new List<TweenAlpha>();
	
	    private ObjFakeCharacter choseRoleModel = null;
	    public UIInput NameInput;
	    public UIButton RandomNameButton;
	    public ulong SelectedRoleId;
	
	    //选择界面的
	    public List<UIEventTrigger> SelectedRoleList;
	    public List<UIToggle> SelectedToggles;
	    public GameObject SelectModelRoot;
	    public BindDataRoot SelectRoleBindData;
	    public UILabel ServerNameLabel;
	    public List<TweenPosition> SelectTweener1 = new List<TweenPosition>();
	    public List<TweenAlpha> SelectTweener2 = new List<TweenAlpha>();
	
	    //第一次进入时不播放ui动画
	    private bool isShowFirst = true;
	
	    public static int GetEquipDataModelFromDecimal(int intDecimal)
	    {
	        var s = Convert.ToString(intDecimal, 2);
	        var index = s.Length - 1 - s.IndexOf('1');
	        return index + 7;
	    }
	
	    public void OnClick_Back()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SelectRole_Back());
	    }
	
	    //返回人物选择界面
	    public void OnClick_CloseCreateRoleFrame()
	    {
	        isShowFirst = true;
	        var e = new UIEvent_ShowCreateRole(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	        LastChooseType = -1;
	        var ee = new UIEvent_SelectRole_Index(LastSelectIndex);
	        EventDispatcher.Instance.DispatchEvent(ee);
	    }
	
	    //创建角色按钮
	    public void OnClick_CreateRole()
	    {
	        var e = new UIEvent_CreateRole();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClick_Enter()
	    {
	        SoundManager.Instance.StopBGM(0.5f);
	        NetManager.Instance.CallEnterGame(SelectedRoleId);
	    }
	
	    //随机取名
	    public void OnClick_RandomName()
	    {
	        var e = new UIEvent_GetRandomName();
	        EventDispatcher.Instance.DispatchEvent(e);
	        EventDispatcher.Instance.DispatchEvent(new NameChange(false));
	    }
	
	    public void OnClick_Select(int nIndex)
	    {
	        RefreshSelectedToggle(nIndex);
	        if (nIndex == LastSelectIndex)
	        {
	            return;
	        }
	        LastSelectIndex = nIndex;
	        var e = new UIEvent_SelectRole_Index(nIndex);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClick_ShowCreateRoleFrame()
	    {
	        isShowFirst = true;
	        var e = new UIEvent_ShowCreateRole(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	
	        for (int i = 0; i < CreateTypeList.Count; i++)
	        {
	            var trigger = CreateTypeList[i];
	            var toggle = CreateTypeList[i].gameObject.GetComponent<UIToggle>();
	            if (toggle != null) toggle.Set(i == 0);
	        }
	
	    }
	
	
	
	    //选择职业
	    public void OnCreateTypeChange(int index)
	    {
	        if (LastChooseType == index)
	        {
	            return;
	        }
	        var ee = new UIEvent_CreateRoleType_Change(index);
	        EventDispatcher.Instance.DispatchEvent(ee);
	        LastChooseType = index;
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_SelectCharacter_ShowUIAnimation.EVENT_TYPE, OnEvent_ShowUIAnime);
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        SelectRoleBindData.RemoveBinding();
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        var controllerBase = UIManager.Instance.GetController(UIConfig.SelectRoleUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        var data = controllerBase.GetDataModel("");
	        SelectRoleBindData.SetBindDataSource(data);
	        SelectRoleBindData.SetBindDataSource(controllerBase.GetDataModel("selectRole"));
	        var ret = (int) controllerBase.CallFromOtherClass("GetCreateShow", null);
	        if (ret == 1)
	        {
	            OnCreateTypeChange(0);
	        }
	
	        var soundId = int.Parse(Table.GetClientConfig(998).Value);
	        if (!SoundManager.Instance.IsBGMPlaying(soundId))
	        {
	            SoundManager.Instance.PlayBGMusic(soundId, 0, 0);
	        }
	
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public void OnNameChange()
	    {
	    }
	
	    public void onSubmitName()
	    {
	        EventDispatcher.Instance.DispatchEvent(new NameChange(true));
	        if (GameUtils.ContainEmoji(NameInput.text))
	        {
	            NameInput.text = string.Empty;
	            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 725);
	        }
	    }
	
	    private void OnEvent_ShowUIAnime(IEvent ievent)
	    {
	        var e = ievent as UIEvent_SelectCharacter_ShowUIAnimation;
	        if (e != null) RunUIAnime(e.Direction, e.Index, e.StateType);
	    }
	
	    private void RunUIAnime(int direction, int index, SelectCharacterLogic.StateType type)
	    {
	
	
	        List<TweenPosition> Tweener1;
	        List<TweenAlpha> Tweener2;
	        if (type == SelectCharacterLogic.StateType.CreateNewRole)
	        {
	            Tweener1 = CreateTweener1;
	            Tweener2 = CreateTweener2;
	        }
	        else if (type == SelectCharacterLogic.StateType.SelectMyRole)
	        {
	            Tweener1 = SelectTweener1;
	            Tweener2 = SelectTweener2;
	        }
	        else
	        {
	            return;
	        }
	
	        if (isShowFirst)
	        {
	            isShowFirst = false;
	            foreach (var tweenPosition in Tweener1)
	            {
	                tweenPosition.PlayForward();
	                var widget = tweenPosition.gameObject.GetComponent<UIWidget>();
	                if (widget != null) widget.alpha = 0;
	            }
	            return;
	        }
	
	
	
	        switch (direction)
	        {
	            case 0:
	                for (var i = 0; i < Tweener1.Count; i++)
	                {
	                    var uiTweener = Tweener1[i];
	                    uiTweener.ResetForPlay();
	                    uiTweener.PlayForward();
	                }
	
	                for (var i = 0; i < Tweener2.Count; i++)
	                {
	                    var uiTweener = Tweener2[i];
	                    uiTweener.ResetForPlay();
	                    uiTweener.PlayForward();
	                }
	
	                break;
	            case 1:
	                for (var i = 0; i < Tweener1.Count; i++)
	                {
	                    var uiTweener = Tweener1[i];
	                   // uiTweener.ResetForPlay();
	                    uiTweener.PlayReverse();
	                }
	
	                for (var i = 0; i < Tweener2.Count; i++)
	                {
	                    var uiTweener = Tweener2[i];
	                   // uiTweener.ResetForPlay();
	                    uiTweener.PlayReverse();
	                }
	                break;
	        }
	    }
	
	    private void RefreshSelectedToggle(int index)
	    {
	        {
	            var __list5 = SelectedToggles;
	            var __listCount5 = __list5.Count;
	            for (var __i5 = 0; __i5 < __listCount5; ++__i5)
	            {
	                var selectedToggle = __list5[__i5];
	                {
	                    var idx = SelectedToggles.IndexOf(selectedToggle);
	                    selectedToggle.Set(index == idx);
	                }
	            }
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        CloseCreateRoleFrame.onClick.Add(new EventDelegate(OnClick_CloseCreateRoleFrame));
	        RandomNameButton.onClick.Add(new EventDelegate(OnClick_RandomName));
	        CreateRoleButton.onClick.Add(new EventDelegate(OnClick_CreateRole));
	        EventDispatcher.Instance.AddEventListener(UIEvent_SelectCharacter_ShowUIAnimation.EVENT_TYPE, OnEvent_ShowUIAnime);
	
	        NameInput.onSubmit.Add(new EventDelegate(onSubmitName));
	        NameInput.onChange.Add(new EventDelegate(OnNameChange));
	
	        //选择人物界面,选中已创建的角色
	        var i = 0;
	        {
	            var __list1 = SelectedRoleList;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var uiEventTrigger = __list1[__i1];
	                {
	                    var j = i;
	                    var dg = new EventDelegate(() => { OnClick_Select(j); });
	                    uiEventTrigger.onClick.Add(dg);
	                    i++;
	                }
	            }
	        }
	        //创建人物界面,选中创建角色类型
	        i = 0;
	        {
	            var __list2 = CreateTypeList;
	            var __listCount2 = __list2.Count;
	            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	            {
	                var uiEventTrigger = __list2[__i2];
	                {
	                    var j = i;
	                    var dg = new EventDelegate(() => { OnCreateTypeChange(j); });
	                    uiEventTrigger.onClick.Add(dg);
	                    i++;
	                }
	            }
	        }
	
	        OnClick_Select(LastSelectIndex);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	#if UNITY_EDITOR
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (Input.GetKeyDown(KeyCode.F2))
	        {
	            var controllerBase = UIManager.Instance.GetController(UIConfig.SelectRoleUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            var data = controllerBase.GetDataModel("") as LoginDataModel;
	            if (0 == data.CreateType)
	            {
					PlayCG.Instance.PlayCGFile("Video/jianshi.txt", null, true);
	            }
	            else if (1 == data.CreateType)
	            {
					PlayCG.Instance.PlayCGFile("Video/fashi.txt", null, true);
	            }
	            else if (2 == data.CreateType)
	            {
					PlayCG.Instance.PlayCGFile("Video/gongshou.txt", null, true);
	            }
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	#endif
	}
}