
/********************************************************************************* 

                         Scorpion



  *FileName:ColiseumResultFrameCtrler

  *Version:1.0

  *Date:2017-06-05

  *Description:

**********************************************************************************/
#region using
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class ColiseumResultFrameCtrler : IControllerBase
{

    #region 静态变量

    #endregion

    #region 成员变量

    private ArenaResultDataModel DataModel;

    #endregion

    #region 构造函数

    public ColiseumResultFrameCtrler()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(AreanResultExitEvent.EVENT_TYPE, OnPressResultOutEvent);
    }

    #endregion

    #region 固有函数

    public void CleanUp()
    {
        DataModel = new ArenaResultDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var _args = data as ArenaResultArguments;
        if (_args == null)
        {
            return;
        }
        var _datas = new List<ItemIdDataModel>();
        var _index = 0;
        {
            // foreach(var item in msgData.Items)
            var _enumerator1 = (_args.RewardData.Items).GetEnumerator();
            while (_enumerator1.MoveNext())
            {
                var _item = _enumerator1.Current;
                {
                    if (_item.Value > 0)
                    {
                        var _itemData = new ItemIdDataModel();
                        _itemData.ItemId = _item.Key;
                        _itemData.Count = _item.Value;
                        _datas.Add(_itemData);
                        _index++;
                    }
                }
            }
        }

        DataModel.RewardItems = new ObservableCollection<ItemIdDataModel>(_datas);
        DataModel.ResultType = _args.RewardData.Result;
        DataModel.RankChange = _args.RewardData.NewRank;
        DataModel.CloseScend = 9;
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

    #endregion

    #region 事件

    public void OnPressResultOutEvent(IEvent ievent)
    {
        var _tbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
        if (_tbScene == null || _tbScene.FubenId < 0)
        {
            return;
        }
        var _tbFuben = Table.GetFuben(_tbScene.FubenId);
        if (_tbFuben.AssistType != (int)eDungeonAssistType.Pvp1v1)
        {
            return;
        }
        NetManager.Instance.StartCoroutine(OutPitCoroutine());
    }

    #endregion

    

    public IEnumerator OutPitCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.ExitDungeon(0);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                var _e = new Close_UI_Event(UIConfig.AreanaResult);
                EventDispatcher.Instance.DispatchEvent(_e);
                if (_msg.ErrorCode == (int) ErrorCodes.OK)
                {
                }
                else
                {
                    Logger.Error(".....ExitDungeon.......{0}.", _msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....ExitDungeon.......{0}.", _msg.State);
            }
        }
    }     
}