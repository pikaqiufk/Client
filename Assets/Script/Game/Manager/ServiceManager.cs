#region using

using System;
using System.Collections.Generic;
using DataTable;
using EventSystem;

#endregion

public class ServiceManager
{
    public static void DoServeice(int npcId,ulong objId, int serveiceId)
    {
		Action callback = () => { NetManager.Instance.DoNpcServeice(npcId,objId, serveiceId); };

        var tableSerice = Table.GetService(serveiceId);
        if (null == tableSerice)
        {
            return;
        }

        var type = (NpcServeType) tableSerice.Type;
        switch (type)
        {
            case NpcServeType.Shop:
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.StoreUI,
                    new StoreArguments {Tab = tableSerice.Param[0]}));
            }
                break;
            case NpcServeType.EquipShop:
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.StoreEquip,
                    new StoreArguments {Tab = tableSerice.Param[0]}));
            }
                break;
            case NpcServeType.HeiShiShop:
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.StoreUI,
                    new StoreArguments { Tab = tableSerice.Param[0], Args = new List<int> { (int)NpcServeType.HeiShiShop } }));
            }
                break;
            case NpcServeType.Repair:
            {
                var needMoney = 0;
                {
                    // foreach(var quip in PlayerDataManager.Instance.EnumEquip())
                    var __enumerator1 = (PlayerDataManager.Instance.EnumEquip()).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var quip = __enumerator1.Current;
                        {
                            if (quip.ItemId < 0)
                            {
                                continue;
                            }
                            var tbEquip = Table.GetEquipBase(quip.ItemId);
                            if (tbEquip.DurableType == 0)
                            {
                                continue;
                            }
                            if (quip.Exdata.Count < 23)
                            {
                                continue;
                            }
                            var durable = quip.Exdata[22];
                            needMoney += (tbEquip.Durability - durable)*tbEquip.DurableMoney;
                        }
                    }
                }

                if (needMoney <= 0)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(423));
                    return;
                }

                if (needMoney > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(426));
                    return;
                }

                var str = string.Format(GameUtils.GetDictionaryText(424), needMoney);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "", callback);
            }
                break;
            case NpcServeType.Treat:
            {
                var hpNow = PlayerDataManager.Instance.GetAttribute(eAttributeType.HpNow);
                var hpMax = PlayerDataManager.Instance.GetAttribute(eAttributeType.HpMax);

                var mpNow = PlayerDataManager.Instance.GetAttribute(eAttributeType.MpNow);
                var mpMax = PlayerDataManager.Instance.GetAttribute(eAttributeType.MpMax);

                if (hpNow >= hpMax && mpNow >= mpMax)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(427));
                    return;
                }

                callback();
            }
                break;
            case NpcServeType.Warehouse:
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.DepotUI));
            }
                break;
            default:
                Logger.Warn("Unknow Service Type");
                return;
            case NpcServeType.OpenUI:
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.GetConfig(tableSerice.Param[0])));
            }
                break;
        }
    }
}