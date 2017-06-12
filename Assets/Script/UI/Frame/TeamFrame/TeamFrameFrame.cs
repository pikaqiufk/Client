using System;
#region using

using System.Collections.Generic;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TeamFrameFrame : MonoBehaviour
	{
	    private readonly List<ObjFakeCharacter> Characters = new List<ObjFakeCharacter>();
	    private bool mRemoveBind = true;
	    public BindDataRoot TeamFrameBindData;
	    public List<TeamMemberCell> TeamMemberCellLogics;
	    //关闭按钮
	    private void ModelView(IEvent ievent)
	    {
	        var ee = ievent as UIEvent_TeamFrame_RefreshModel;
	        ModelView();
	    }
	
	    private void ModelView()
	    {
	        {
	            var __list3 = Characters;
	            var __listCount3 = __list3.Count;
	            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
	            {
	                var model = __list3[__i3];
	                {
	                    if (null != model)
	                    {
	                        model.Destroy();
	                    }
	                }
	            }
	        }
	        Characters.Clear();
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.TeamFrame);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        var teamData = controllerBase.GetDataModel("") as TeamDataModel;
	        if (teamData == null)
	        {
	            return;
	        }
	        for (var i = 0; i < 5; i++)
	        {
	            var one = teamData.TeamList[i];
	            if (one.Level == 0)
	            {
	                continue;
	            }
	            var i1 = i;
	            one.Equips[12] = -1;
	            Characters.Add(ObjFakeCharacter.Create(one.TypeId, one.Equips, character =>
	            {
	                var xform = character.gameObject.transform;
	
	                xform.parent = TeamMemberCellLogics[i1].ModelRoot.transform;
	                xform.localPosition = new Vector3(0f, -110f, 0f);
	                xform.localScale = new Vector3(138, 138, 138);
	                xform.forward = Vector3.back;
	                xform.gameObject.SetLayerRecursive(LayerMask.NameToLayer(GAMELAYER.UI));
	            }, LayerMask.NameToLayer(GAMELAYER.UI)));
	        }
	    }
	
	    //关闭界面
	    public void OnClick_Close()
	    {
	        var e2 = new Close_UI_Event(UIConfig.OperationList);
	        EventDispatcher.Instance.DispatchEvent(e2);
	
	        var e = new Close_UI_Event(UIConfig.TeamFrame);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //离开队伍
	    public void OnClick_Kick(int index)
	    {
	        var e = new UIEvent_TeamFrame_Kick(index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //离开队伍
	    public void OnClick_Leave()
	    {
	        var e = new UIEvent_TeamFrame_Leave();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //点击模型
	    public void OnClick_Model(int index)
	    {
	        var parent = UIManager.GetInstance().GetUIRoot(UIType.TYPE_TIP);
	        UIConfig.OperationList.Loction = parent.transform.worldToLocalMatrix*
	                                         TeamMemberCellLogics[index].ModelView.worldCenter;
	        UIConfig.OperationList.Loction.x += 64;
	        UIConfig.OperationList.Loction.y += 100;
	        UIConfig.OperationList.Loction.z = 0;
	        var e = new TeamMemberShowMenu(index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //tab：附近玩家
	    public void OnClick_NearPlayer()
	    {
	        var e2 = new Close_UI_Event(UIConfig.OperationList);
	        EventDispatcher.Instance.DispatchEvent(e2);
	        var e = new UIEvent_TeamFrame_NearPlayer();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //tab：附近队伍
	    public void OnClick_NearTeam()
	    {
	        var e2 = new Close_UI_Event(UIConfig.OperationList);
	        EventDispatcher.Instance.DispatchEvent(e2);
	        var e = new UIEvent_TeamFrame_NearTeam();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //tab：自己队伍
	    public void OnClick_SelfTeam()
	    {
	        var e2 = new Close_UI_Event(UIConfig.OperationList);
	        EventDispatcher.Instance.DispatchEvent(e2);
	    }
	
	    //变更：自动加入
	    public void OnClickAutoAccept()
	    {
	        var e = new UIEvent_TeamFrame_AutoAccept();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //变更：自动申请
	    public void OnClickAutoJion()
	    {
	        var e = new UIEvent_TeamFrame_AutoJion();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnCloseUiBindRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.TeamFrame)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            mRemoveBind = false;
	        }
	        else
	        {
	            if (mRemoveBind == false)
	            {
	                RemoveBindEvent();
	            }
	            mRemoveBind = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (mRemoveBind == false)
	        {
	            RemoveBindEvent();
	        }
	        mRemoveBind = true;
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
	        if (mRemoveBind)
	        {
	            RemoveBindEvent();
	        }
	
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
	        if (mRemoveBind)
	        {
	            EventDispatcher.Instance.AddEventListener(UIEvent_TeamFrame_RefreshModel.EVENT_TYPE, ModelView);
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	
	            var controllerBase = UIManager.Instance.GetController(UIConfig.TeamFrame);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            TeamFrameBindData.SetBindDataSource(controllerBase.GetDataModel(""));
	
	            ModelView();
	        }
	        mRemoveBind = true;
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void RemoveBindEvent()
	    {
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_TeamFrame_RefreshModel.EVENT_TYPE, ModelView);
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	        TeamFrameBindData.RemoveBinding();
	        {
	            var __list5 = Characters;
	            var __listCount5 = __list5.Count;
	            for (var __i5 = 0; __i5 < __listCount5; ++__i5)
	            {
	                var mychar = __list5[__i5];
	                {
	                    if (mychar)
	                    {
	                        mychar.Destroy();
	                    }
	                }
	            }
	        }
	        Characters.Clear();
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        for (var i = 0; i < 5; i++)
	        {
	            var cell = TeamMemberCellLogics[i];
	            var j = i;
	
	            if (i != 0)
	            {
	                cell.KickButton.onClick.Add(new EventDelegate(()
	                    => {
	                           OnClick_Kick(j);
	                    }));
	            }
	
	            var trigger = cell.ModelView.GetComponent<UIEventTrigger>();
	            trigger.onClick.Add(new EventDelegate(() => { OnClick_Model(j); }));
	        }
	
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void Update()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	}
}