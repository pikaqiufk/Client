/********************************************************************************* 
                         Scorpion

  *FileName:ActiveAwardFarmeCtrler
  *Version:1.0
  *Date:2017-06-06
  *Description:
**********************************************************************************/  
#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion

public class ActiveAwardFarmeCtrler : IControllerBase
{

    #region 静态变量
    #endregion

    #region 成员变量

    private ActivityRewardFrameDataModel m_DataModel;
    private ItemIdDataModel m_ExpItemData;
    private int m_iFubenId;
    private int m_iRewardExp;
    private int m_iRewardExpTimes;

    #endregion

    #region 构造函数

    public ActiveAwardFarmeCtrler()
    {
        CleanUp();
    }

    #endregion

    #region 固有函数

    public void CleanUp()
    {
        m_DataModel = new ActivityRewardFrameDataModel();
        Resetting();
    }

    public void OnShow()
    {
        EventDispatcher.Instance.AddEventListener(UIEvent_ButtonClicked_1.EVENT_TYPE, OnBttonTapEvent);
    }

    public void Close()
    {
        EventDispatcher.Instance.RemoveEventListener(UIEvent_ButtonClicked_1.EVENT_TYPE, OnBttonTapEvent);

        Resetting();
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var _args = data as ActivityRewardArguments;
        if (_args == null)
        {
            return;
        }

        var seconds = _args.Seconds;
        var formatStr = GameUtils.GetDictionaryText(1052);
        m_DataModel.UseTime = string.Format(formatStr, seconds / 60, seconds % 60);

        var completeType = _args.CompleteType;
        var level = _args.PlayerLevel;
        var complete = completeType != eDungeonCompleteType.Quit;
        m_DataModel.IsSuccess = completeType == eDungeonCompleteType.Success;

        m_iFubenId = _args.FubenId;
        var _tbFuben = Table.GetFuben(m_iFubenId);
        var _rewards = new List<ItemIdDataModel>();

        var _idx = 0;
        if (complete)
        {
            var _itemDict = new Dictionary<int, int>();
            var _rewardIds = _tbFuben.RewardId;
            var _rewardCounts = _tbFuben.RewardCount;
            for (var _imax = _rewardIds.Count; _idx < _imax; ++_idx)
            {
                var _id = _rewardIds[_idx];
                if (_id == -1)
                {
                    break;
                }
                var _count = _rewardCounts[_idx];

                _itemDict.modifyValue(_id, _count);
            }

            if (_tbFuben.IsDynamicExp == 1)
            {
                var _count = (int)(1.0 * _tbFuben.DynamicExpRatio * Table.GetLevelData(level).DynamicExp / 10000);
                _itemDict.modifyValue((int)eResourcesType.ExpRes, _count);
            }

            var _ids = _itemDict.Keys.ToList();
            _ids.Sort();
            foreach (var id in _ids)
            {
                var _reward = new ItemIdDataModel();
                _reward.ItemId = id;
                _reward.Count = _itemDict[id];
                _rewards.Add(_reward);

                if (id == (int)eResourcesType.ExpRes)
                {
                    m_ExpItemData = _reward;
                    m_iRewardExp = _reward.Count;
                }
            }
        }
        else
        {
            var _exp = _tbFuben.ScanExp;
            if (_tbFuben.IsDynamicExp == 1)
            {
                _exp = (int)(1.0 * _tbFuben.ScanDynamicExpRatio * Table.GetLevelData(level).DynamicExp / 10000);
            }

            var _reward = new ItemIdDataModel();
            _rewards.Add(_reward);
            _reward.ItemId = 1;
            _reward.Count = _exp;

            m_iRewardExp = _reward.Count;
            m_ExpItemData = _reward;

            _reward = new ItemIdDataModel();
            _rewards.Add(_reward);
            _reward.ItemId = 2;
            _reward.Count = _tbFuben.ScanGold;
        }
        m_DataModel.Rewards = new ObservableCollection<ItemIdDataModel>(_rewards);
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return m_DataModel;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }

    #endregion

    #region 逻辑函数

    private void Resetting()
    {
        m_iFubenId = 0;
        m_iRewardExp = 0;
        m_iRewardExpTimes = 1;
        m_ExpItemData = null;
        m_DataModel.Times[0] = true;
        m_DataModel.Times[1] = false;
    }

    private void ChooseDungeonAward(int radioIdx)
    {
        NetManager.Instance.StartCoroutine(ChooseDungeonAwardCoroutine(radioIdx));
    }

    private IEnumerator ChooseDungeonAwardCoroutine(int radioIdx)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.SelectDungeonReward(m_iFubenId, radioIdx);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State != MessageState.Reply)
            {
                yield break;
            }
            if (_msg.ErrorCode != (int)ErrorCodes.OK)
            {
                UIManager.Instance.ShowNetError(_msg.ErrorCode);
            }
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ActivityRewardFrame));
        }
    }

    #endregion

    #region 事件

    public void OnBttonTapEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_ButtonClicked_1;
        switch (_e.Index)
        {
            case 0:
                {
                    var _times = m_DataModel.Times;
                    var _radioIdx = 0;
                    for (int i = 0, imax = _times.Count; i < imax; i++)
                    {
                        if (_times[i])
                        {
                            _radioIdx = i;
                            break;
                        }
                    }
                    if (_radioIdx == 1)
                    {
                        //使用钻石获得双倍经验
                        var _resId = (int)eResourcesType.DiamondRes;
                        if (PlayerDataManager.Instance.GetItemCount(_resId) < 50)
                        {
                            //材料不足
                            var _tbItem = Table.GetItemBase(_resId);
                            var _content = string.Format(GameUtils.GetDictionaryText(467), _tbItem.Name);
                            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(_content));
                            return;
                        }
                    }
                    ChooseDungeonAward(_radioIdx);
                }
                break;
            case 1:
            case 2:
                {
                    if (m_ExpItemData == null)
                    {
                        break;
                    }
                    var _beilv = _e.Index;
                    if (m_iRewardExpTimes == _beilv)
                    {
                        break;
                    }
                    m_iRewardExpTimes = _beilv;
                    m_ExpItemData.Count = m_iRewardExp * _beilv;
                }
                break;
        }
    }

    #endregion
     
}