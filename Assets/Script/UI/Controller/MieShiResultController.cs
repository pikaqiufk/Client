#region using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using Shared;

#endregion

public class MieShiResultController : IControllerBase
{
    public MieShiResultController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(MieshiResultEvent.EVENT_TYPE, RefreshResultData);
    }

    public MieshiResultDataModel DataModel;
   
    public void CleanUp()
    {
        DataModel = new MieshiResultDataModel();
    }

    

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
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
    public void OnChangeScene(int sceneId)
    {
    }
    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }
    public void RefreshResultData(IEvent ievent)
    {
        DataModel.AwardItems.Clear();
        DataModel.RankRewards.Clear();
        var info = ievent as MieshiResultEvent;
        DataModel.KillCount = info.msg.KillCount;
        DataModel.Score = info.msg.Score;
        DataModel.Contribution = info.msg.Contribution;
        DataModel.FightResult = info.msg.IsWin;
        DataModel.TowerCount = info.msg.TowerCount;
        Table.ForeachDefendCityReward(record =>
        {
            if (record.Rank[0] <= info.msg.Rank && record.Rank[1] >= info.msg.Rank)
            {
                if (info.msg.TowerCount > 0 && info.msg.TowerCount-1 < record.Rate.Count && info.msg.IsWin == 1)
                {
                    DataModel.Rate = record.Rate[info.msg.TowerCount-1];
                }
                else
                {
                    DataModel.Rate = -100;
                }
                if (info.msg.IsWin == 0)
                    return true;
                MailRecord tbMail = null;
                switch(info.msg.TowerCount)
                {
                    case 6:
                        tbMail = Table.GetMail(record.MailId6);
                        break ;
                    case 5:
                        tbMail = Table.GetMail(record.MailId5);
                        break ;
                    case 4:
                        tbMail = Table.GetMail(record.MailId4);
                         break ;
                    case 3:
                         tbMail = Table.GetMail(record.MailId3);
                          break ;
                    case 2:
                          tbMail = Table.GetMail(record.MailId2);
                         break ;
                    default:
                         tbMail = Table.GetMail(record.MailId);
                        break ;
                }
                if(tbMail != null)
                {
                    var strDic = GameUtils.GetDictionaryText(300000125);
                    string _s = string.Format(strDic, DataModel.Rate);
                    for(int i=0;i<tbMail.ItemId.Length&&i<tbMail.ItemCount.Length;i++)
                    {
                        BagItemDataModel item = new BagItemDataModel();
                        if (tbMail.ItemId[i] >= 0 && tbMail.ItemCount[i]>0)
                        {
                            item.ItemId = tbMail.ItemId[i];
                            item.Count = tbMail.ItemCount[i];

                            item.strParam = _s;
                            DataModel.RankRewards.Add(item);
                        }

                    }
                    {//灭世积分奖励
                        BagItemDataModel item = new BagItemDataModel();
                        item.ItemId = (int)eResourcesType.MieshiScore;
                        item.Count = info.msg.Score;
                        item.strParam = _s;
                        DataModel.RankRewards.Add(item);
                    }

                }
                return false;
            }
            return true;
        });
        
        
        for(int i=0;i<info.msg.ItemId.Count&&i<info.msg.ItemNum.Count;i++)
        {
            BagItemDataModel item = new BagItemDataModel() ;
            item.ItemId = info.msg.ItemId[i] ;
            item.Count = info.msg.ItemNum[i] ;
            DataModel.AwardItems.Add(item);
        }
        var str = GameUtils.GetDictionaryText(300000127);
        DataModel.strExplain = string.Format(str, info.msg.TowerCount, DataModel.Rate);
       
    }

    public void RefreshData(UIInitArguments data)
    {
        
    }
}