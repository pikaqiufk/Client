#region using

using System;
using System.Collections;
using System.Collections.Generic;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class SceneManager : Singleton<SceneManager>, IManager
{
    public int CurrentSceneTypeId = -1;
    public int EnterSceneCounter;
    private string mChangeSceneOverMessage = string.Empty;
    public bool mIsLoadSceneOver;
    private readonly Queue<Action<bool>> mLoadSceneOverActions = new Queue<Action<bool>>();
    public Dictionary<int, List<MapTransferRecord>> MapTrasferDictionary = new Dictionary<int, List<MapTransferRecord>>();

    public string ChangeSceneOverMessage
    {
        set
        {
            if (!value.Equals(mChangeSceneOverMessage))
            {
                mChangeSceneOverMessage = value;
            }
        }
    }

    /// <summary>
    ///     切换场景成功
    ///     请求服务器刷新新的场景的人，广播自己进入新场景的事件
    /// </summary>
    public Coroutine ChangeSceneOver()
    {
        //NetManager.Instance.StartCoroutine(GameLogic.Instance.AskSceneExtData);
        return NetManager.Instance.StartCoroutine(ChangeSceneOverCoroutine());
    }

    public IEnumerator ChangeSceneOverCoroutine()
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var data = PlayerDataManager.Instance.mInitBaseAttr;

            var msg = NetManager.Instance.ChangeSceneOver(data.SceneId, data.SceneGuid);

            yield return msg.SendAndWaitUntilDone();
            Logger.Debug("ChangeSceneOver-------" + msg.State);
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (ObjManager.Instance != null && ObjManager.Instance.MyPlayer != null)
                    {
                        ObjManager.Instance.MyPlayer.Position =
                            GameLogic.GetTerrainPosition(PlayerDataManager.Instance.mInitBaseAttr.X,
                                PlayerDataManager.Instance.mInitBaseAttr.Y);
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }

        //yield return new WaitForSeconds(1);
        ShowChangeSceneMessage();
    }

    private bool EnterScene(int sceneId)
    {
        PlatformHelper.UMEvent("SceneChange", sceneId.ToString());
        //如果目标场景一致，就不用再加载资源了
        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null &&
            GameLogic.Instance.Scene.SceneTypeId == sceneId)
        {
            var player = ObjManager.Instance.MyPlayer;
            if (null == player)
            {
                Logger.Error("EnterScene  null == player");
                return false;
            }

            Logger.Debug("Resume to this scene.");

            ObjManager.Instance.RemoveObjExceptPlayer();

            player.StopMove();
            player.RemoveAllBuff(); //服务端会重新同步buff
            player.LeaveAutoCombat();

            ChangeSceneOver();
            mIsLoadSceneOver = true;
            return true;
        }

        //判断目标场景资源和当前场景资源是否一样，如果一样就不Load直接把当前Obj删除掉，等服务端同步来新的Obj
        do
        {
            var player = ObjManager.Instance.MyPlayer;

            //判断必要条件
            if (null == player || null == GameLogic.Instance)
            {
                break;
            }

            //判断当前场景
            if (GameLogic.Instance.Scene == null)
            {
                break;
            }

            var table1 = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
            var table2 = Table.GetScene(sceneId);
            if (null == table1 || null == table2)
            {
                break;
            }

            //是同一个场景资源
            if (0 != table1.ResName.CompareTo(table2.ResName))
            {
                break;
            }

            var isPhaseDungeon = false;
            {
                var fuben1 = Table.GetFuben(table1.FubenId);
                if (null != fuben1)
                {
                    if (eDungeonAssistType.PhaseDungeon == (eDungeonAssistType) fuben1.AssistType)
                    {
                        isPhaseDungeon = true;
                    }
                }

                if (!isPhaseDungeon)
                {
                    var fuben2 = Table.GetFuben(table2.FubenId);
                    if (null != fuben2)
                    {
                        if (eDungeonAssistType.PhaseDungeon == (eDungeonAssistType) fuben2.AssistType)
                        {
                            isPhaseDungeon = true;
                        }
                    }
                }
            }

            //是否有其中一个是相位副本
            if (!isPhaseDungeon)
            {
                break;
            }

            ObjManager.Instance.RemoveObjExceptPlayer();

            CurrentSceneTypeId = sceneId;

            GameLogic.Instance.Scene.ClearLastScene(false);
            GameLogic.Instance.Scene.LoadTable();
            var data = PlayerDataManager.Instance.mInitBaseAttr;
            var pos = GameLogic.GetTerrainPosition(data.X, data.Y);
            player.StopMove();
            player.RemoveAllBuff(); //服务端会重新同步buff
            player.LeaveAutoCombat();

            //如果是相位副本，就自动战斗
            if (-1 != GameLogic.Instance.Scene.TableScene.FubenId)
            {
                var tableFuben = Table.GetFuben(GameLogic.Instance.Scene.TableScene.FubenId);
                if (null != tableFuben)
                {
                    if (eDungeonAssistType.PhaseDungeon == (eDungeonAssistType) tableFuben.AssistType)
                    {
                        player.EnterAutoCombat();
                    }
                }
            }
            else
            {
//如果是出副本，就自动交任务
                MissionManager.Instance.OnEnterNormalScene();
            }
            player.Position = pos;
            //通知服务端客户端已经切完了
            ChangeSceneOver();
            EventDispatcher.Instance.DispatchEvent(new RefresSceneMap(sceneId));
            EventDispatcher.Instance.DispatchEvent(new SceneTransition_Event());
            Logger.Debug("Resume to this scene.");
	        OnLoadSceneOver();
            return true;
        } while (false);

        Logger.Debug("Loading new scene.");

        var tbscene = Table.GetScene(sceneId);
        if (tbscene == null)
        {
            var log = string.Format("ERROR::!Table.Scene.ContainsKey({0})", sceneId);
            Logger.Info(log);
            return false;
        }

        //放在GameLogic的Destroy里了
        //ObjManager.Instance.RemoveAllObj();
        //UIManager.Instance.Destroy();
        //ComplexObjectPool.Destroy();

        if (GameLogic.Instance != null)
        {
            if (GameLogic.Instance.Scene != null)
            {
                GameLogic.Instance.Scene.ClearLastScene();
            }
        }

        CurrentSceneTypeId = sceneId;
        ++EnterSceneCounter;
        var sceneName = tbscene.ResName;
        //Logger.Info("---------------{" + sceneName + "}-------------");

        Application.LoadLevel("Loading");

        return true;
    }

    public IEnumerator EnterSceneCoroutinue(int sceneId)
    {
        while (!PlayerDataManager.Instance.CheckLoginApplyState())
        {
            yield return new WaitForEndOfFrame();
        }
        if (!PlayerAttr.Instance.InitOver)
        {
            PlayerAttr.Instance.InitAttributesAll();
        }
        EnterScene(sceneId);
    }

    public void OnLoadSceneOver()
    {
        mIsLoadSceneOver = true;
        while (mLoadSceneOverActions.Count > 0)
        {
            var act = mLoadSceneOverActions.Dequeue();
            try
            {
                act(true);
            }
            catch (Exception)
            {
                // do nothing.
            }
        }

	    if (null != GameLogic.Instance && null != GameLogic.Instance.Scene)
	    {
		    var table = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
		    if (null != table)
		    {
			    var fuben = Table.GetFuben(table.FubenId);
				if (null != fuben)
			    {
					if(-1!=fuben.BeforeStoryId)
					{
						PlayCG.PlayById(fuben.BeforeStoryId);
					}
			    }
				
		    }
	    }

        if (isInMieshiFuben())
        {
            var e1 = new Show_UI_Event(UIConfig.MieShiTapUI);
            EventDispatcher.Instance.DispatchEvent(e1);
        }

        var logic = GameLogic.Instance;
        if (logic != null)
        {
            var scene = logic.Scene;
            if (scene != null)
            {
                var tbScene = Table.GetScene(scene.SceneTypeId);
                if (tbScene != null && tbScene.FubenId == -1) // 不是副本
                {
                    PlayerDataManager.Instance.PlayerDataModel.DungeonState = (int)eDungeonState.Start;
                }
            }
        }

		 
    }



    public List<MapTransferRecord> GetMapTransferList(int sceneId)
    {
        if (MapTrasferDictionary.Count <= 0)
        {
            Table.ForeachMapTransfer(record =>
            {
                List<MapTransferRecord> mapTransList;
                if (!MapTrasferDictionary.TryGetValue(record.SceneID, out mapTransList))
                    MapTrasferDictionary[record.SceneID] = new List<MapTransferRecord>();
                MapTrasferDictionary[record.SceneID].Add(record);
                return true;
            });
        }

        List<MapTransferRecord> mapTransferList;
        if (!MapTrasferDictionary.TryGetValue(sceneId, out mapTransferList))
        {
        }
        return mapTransferList;
    }

    public bool IsMieShiFuben(int sceneId)
    {
        var tbScene = Table.GetScene(sceneId);
        if (tbScene == null)
        {
            return false;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return false;
        }
        var type = (eDungeonAssistType)tbFuben.AssistType;
        return type == eDungeonAssistType.MieShiWar;        
    }

    public bool isInMieshiFuben()
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
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return false;
        }
        var type = (eDungeonAssistType)tbFuben.AssistType;
        return type == eDungeonAssistType.MieShiWar;
    }

    public void RegisterLoadSceneOverAction(Action<bool> act)
    {
        if (!mIsLoadSceneOver)
        {
            mLoadSceneOverActions.Enqueue(act);
        }
        else
        {
            act(false);
        }
    }

    private void ShowChangeSceneMessage()
    {
        if (!string.IsNullOrEmpty(mChangeSceneOverMessage))
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(mChangeSceneOverMessage));
            mChangeSceneOverMessage = string.Empty;
        }
    }

    //地形文件

    public IEnumerator Init()
    {
        yield return null;
    }

    public void Reset()
    {
        mIsLoadSceneOver = false;
        mChangeSceneOverMessage = string.Empty;
        mLoadSceneOverActions.Clear();
        EnterSceneCounter = 0;
    }

    public void Tick(float delta)
    {
    }

    public void Destroy()
    {
    }
}