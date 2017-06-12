#region using

using System;
using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class ReliveController : IControllerBase
{
    public ReliveController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(RelieveOperateEvent.EVENT_TYPE, OnRelieveOperate);
        EventDispatcher.Instance.AddEventListener(RefreshReliveInfoEvent.EVENT_TYPE, OnRefreshReliveInfo);
    }

	private int mFreeTime = 0;

    public ReliveDataModel DataModel;

    public void OnRefreshReliveInfo(IEvent ievent)
    {
        var e = ievent as RefreshReliveInfoEvent;
		var name = e.KillerName;
		float time = 0;
		
		if (null != GameLogic.Instance)
		{
            if (GameLogic.Instance.Scene != null)
		    {
			    time = GameLogic.Instance.Scene.TableScene.SafeReliveCD;
		    }
		}
		
		if (time > 0)
		{
			DataModel.FreeClick = false;
		}
		else
		{
			DataModel.FreeClick = true;
			time = 0.5f;
		}
		DataModel.FreeTime = Game.Instance.ServerTime.AddSeconds(time);
        DataModel.KillName = name;

        if (UIConfig.MainUI.Visible())
        {
            var ee = new Show_UI_Event(UIConfig.ReliveUI);
            EventDispatcher.Instance.DispatchEvent(ee);
        }
    }

	public void OnReliveCountdownEvent(IEvent ievent)
	{
		DataModel.FreeClick = true;
	}

    public void OnRelieveOperate(IEvent ievent)
    {
        var e = ievent as RelieveOperateEvent;
        switch (e.Type)
        {
            case 0:
            {
                ReleveStone();
            }
                break;
            case 1:
            {
                ReleveDiamond();
            }
                break;
            case 2:
            {
                ReleveFree();
            }
                break;
            default:
                break;
        }
    }

    public void ReleveDiamond()
    {
        var dia = Table.GetClientConfig(900).ToInt();
        if (PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Diamond < dia)
        {
			EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
			/*
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 1043, "",
                () => { EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RechargeFrame)); });
			 * */
			EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RechargeFrame));
            return;
        }
        NetManager.Instance.StartCoroutine(ReliveTypeCoroutine(1));
    }

    public IEnumerator ExitDungeonCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ExitDungeon(-10);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    var logic = GameLogic.Instance;
                    if (logic == null)
                    {
                        yield break;
                    }
                    var scene = logic.Scene;
                    if (scene == null)
                    {
                        yield break;
                    }
                    var tbScene = Table.GetScene(scene.SceneTypeId);
                    if (tbScene == null)
                    {
                        yield break;
                    }
                    PlatformHelper.UMEvent("Fuben", "Exit", tbScene.FubenId.ToString());
                }
                else
                {
                    Logger.Error(".....ExitDungeon.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....ExitDungeon.......{0}.", msg.State);
            }
        }
    }

    public void ReleveFree()
    {
        if (IsDescFuBen())
        {
            NetManager.Instance.StartCoroutine(ExitDungeonCoroutine());
        }
        else
        {
            NetManager.Instance.StartCoroutine(ReliveTypeCoroutine(2));
        }
    }

    public void ReleveStone()
    {
		if (PlayerDataManager.Instance.GetItemTotalCount(22019).Count <= 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(1042));
			GameUtils.GotoUiTab(79,3);
			//EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RechargeFrame));
            return;
        }
        NetManager.Instance.StartCoroutine(ReliveTypeCoroutine(0));
    }

    public IEnumerator ReliveTypeCoroutine(int t)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ReliveType(t);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var e = new Close_UI_Event(UIConfig.ReliveUI);
                    EventDispatcher.Instance.DispatchEvent(e);

                    var e1 = new Show_UI_Event(UIConfig.MainUI);
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNoDie)
                {
                    //TODO
                    var e = new Close_UI_Event(UIConfig.ReliveUI);
                    EventDispatcher.Instance.DispatchEvent(e);

                    var e1 = new Show_UI_Event(UIConfig.MainUI);
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("ReliveType Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("ReliveType Error!............State..." + msg.State);
            }
        }
    }

    public void CleanUp()
    {
        DataModel = new ReliveDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {

	    //DataModel.StoneCount = PlayerDataManager.Instance.GetItemTotalCount(22019);
		DataModel.StoneCount.Count = 1;
        if (DataModel.FreeClick)
        {
            DataModel.FreeTime = Game.Instance.ServerTime.AddSeconds(0.5f);
        }
        
//		float time = 0;
//         if (DataModel.FreeTime < Game.Instance.ServerTime)
//         {
// 			if (null != GameLogic.Instance)
// 			{
// 				time = GameLogic.Instance.Scene.TableScene.SafeReliveCD;
// 			}
// 			if (time > 0)
// 			{
// 				DataModel.FreeClick = false;
// 			}
// 			else
// 			{
// 				DataModel.FreeClick = true;
// 				time = 0.5f;
// 			}
// 			DataModel.FreeTime = Game.Instance.ServerTime.AddSeconds(time);
//         }


        DataModel.FuHuoDiamond = Table.GetClientConfig(900).ToInt();

        if (IsDescFuBen())
        {
            DataModel.IsShowFuHuoTime = false;
            DataModel.SafeFuHuoDesc = GameUtils.GetDictionaryText(100001188);
        }
        else
        {
            DataModel.IsShowFuHuoTime = true;
            DataModel.SafeFuHuoDesc = GameUtils.GetDictionaryText(100000690);
        }
    }

    private bool IsDescFuBen()
    {
        var logic = GameLogic.Instance;
        if (logic == null)
        {
            return false;
        }
        var scene = logic.Scene;
        if (scene == null)
        {
            return false;
        }
        var tbScene = Table.GetScene(scene.SceneTypeId);
        if (tbScene == null)
        {
            return false;
        }

        if (tbScene.SafeReliveCD != -1)
        {
            return false;
        }

        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return false;
        }

        return true;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
    }

    public void Tick()
    {
	    if (false==DataModel.FreeClick && 0 == Time.frameCount%15)
	    {
		    if ((Game.Instance.ServerTime-DataModel.FreeTime).TotalSeconds >= 0)
		    {
				DataModel.FreeClick = true;    
		    }
	    }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnShow()
    {
    }

    public FrameState State { get; set; }
}