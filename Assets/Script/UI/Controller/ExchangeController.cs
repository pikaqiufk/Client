/********************************************************************************* 

                         Scorpion




  *FileName:ExchangeController

  *Version:1.0

  *Date:2017-06-06

  *Description:

**********************************************************************************/
#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion
public class CommutationFrameCtrler : IControllerBase
{

    #region 静态变量
    private const int STOREID = 120000;
    #endregion

    #region 成员变量
    ExchangeDataModel DataModel;
   
    private StoreRecord m_tbStore;
    #endregion

    #region 构造函数
    public CommutationFrameCtrler()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(ExChangeInit_Event.EVENT_TYPE, OnExchangeDataInitEvent);
        EventDispatcher.Instance.AddEventListener(ExChange_Event.EVENT_TYPE, OnPurchasePressMsgBuyEvent);

    }
    #endregion

    #region 固有函数
    public FrameState State { get; set; }
    public void CleanUp()
    {
        DataModel = new ExchangeDataModel();
        m_tbStore = Table.GetStore(STOREID);
        if (m_tbStore == null)
        {
            return;
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

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {

    }



    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }
    #endregion
    #region 普通函数

    private IEnumerator ShopPurchaseCoroutine(int index, int count = 1)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.StoreBuy(index, count, -1);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    DataModel.RemainTimes -= 1;
                    AlterPrice();
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                    Logger.Error("StoreBuy....StoreId= {0}...ErrorCode...{1}", index, _msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("StoreBuy............State..." + _msg.State);
            }
        }
    }
    private void AlterPrice()
    {
        int times = DataModel.CurTimes - DataModel.RemainTimes;
        var _tbSkillUpGrading = Table.GetSkillUpgrading(m_tbStore.WaveValue);
        if (null == _tbSkillUpGrading || 0 == _tbSkillUpGrading.Values.Count)
            return;
        if (times >= _tbSkillUpGrading.Values.Count)
            times = _tbSkillUpGrading.Values.Count - 1;
        DataModel.strTimes = DataModel.RemainTimes.ToString();//times.ToString() + "/" + DataModel.CurTimes.ToString();
        DataModel.Diamond = _tbSkillUpGrading.Values[times];
        DataModel.Gold = m_tbStore.ItemCount;
    }
    #endregion


    #region 事件
    private void OnExchangeDataInitEvent(IEvent ievent)
    {
        //每日购买限制
        var _tbEx = Table.GetExdata(m_tbStore.DayCount);
        if (_tbEx == null) return;

        int vipCount = 0;
        for (int i = 0; i < PlayerDataManager.Instance.TbVip.BuyItemId.Length; i++)
        {
            if (PlayerDataManager.Instance.TbVip.BuyItemId[i] == STOREID && i < PlayerDataManager.Instance.TbVip.BuyItemCount.Length)
            {
                vipCount = PlayerDataManager.Instance.TbVip.BuyItemCount[i];
            }
        }

        DataModel.CurTimes = _tbEx.InitValue + vipCount;
        DataModel.RemainTimes = PlayerDataManager.Instance.GetExData(m_tbStore.DayCount) + vipCount;
        AlterPrice();
    }
    private void OnPurchasePressMsgBuyEvent(IEvent ievent)
    {
        if (m_tbStore == null)
            return;
        var _index = 120000;
        var _count = 1;

        //每日购买限制
        var _tbEx = Table.GetExdata(m_tbStore.DayCount);
        if (_tbEx == null) return;

        int vipCount = 0;
        for (int i = 0; i < PlayerDataManager.Instance.TbVip.BuyItemId.Length; i++)
        {
            if (PlayerDataManager.Instance.TbVip.BuyItemId[i] == _index && i < PlayerDataManager.Instance.TbVip.BuyItemCount.Length)
            {
                vipCount = PlayerDataManager.Instance.TbVip.BuyItemCount[i];
            }
        }
        var _dayCount = _tbEx.InitValue + vipCount;


        //当前剩余购买次数
        var _curCount = PlayerDataManager.Instance.GetExData(m_tbStore.DayCount);
        var _times = _dayCount - _curCount;

        var _tbSkillUpGrading = Table.GetSkillUpgrading(m_tbStore.WaveValue);
        if (null == _tbSkillUpGrading || 0 == _tbSkillUpGrading.Values.Count)
            return;


        var _idx = _times >= _tbSkillUpGrading.Values.Count ? _tbSkillUpGrading.Values.Count - 1 : _times;
        var _cost = _tbSkillUpGrading.Values[_idx];
        if (PlayerDataManager.Instance.GetRes(m_tbStore.NeedType) < _cost)
        {
            var _tbItemCost = Table.GetItemBase(m_tbStore.NeedType);
            //{0}不足！
            var _str = GameUtils.GetDictionaryText(701);
            _str = string.Format(_str, _tbItemCost.Name);
            GameUtils.ShowHintTip(_str);
            PlayerDataManager.Instance.ShowItemInfoGet(m_tbStore.NeedType);
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ExchangeUI));
            return;
        }
        NetManager.Instance.StartCoroutine(ShopPurchaseCoroutine(_index, _count));
    }
    #endregion
 
  

  

  


}
