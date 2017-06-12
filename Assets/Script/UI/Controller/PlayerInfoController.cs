#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using EventSystem;

#endregion

public class PlayerInfoController : IControllerBase
{
    public static Dictionary<int, int> equipList = new Dictionary<int, int>
    {
        {700, 0},
        {800, 1},
        {900, 2},
        {1000, 3},
        {1100, 4},
        {1200, 5},
        {1300, 6},
        {1301, 7},
        {1400, 8},
        {1500, 9},
        {1600, 10},
        {1700, 11},
        {1800, 12}
    };

    public PlayerInfoController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(PlayerInfoOperation.EVENT_TYPE, OnPlayerInfoOperation);
    }

    public PlayerInfoDataModel DataModel;

    public static int GetEquipIndex(int bagIdandIndex)
    {
        int result;
        if (equipList.TryGetValue(bagIdandIndex, out result))
        {
            return result;
        }
        return -1;
    }

    public void OnClickElfIco()
    {
        if (DataModel.ElfData.ItemId == -1)
        {
            return;
        }
        var e = new Show_UI_Event(UIConfig.ElfInfoUI,
            new ElfInfoArguments {DataModel = DataModel.ElfData, ShowButton = false});
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void OnClickEquipIndex(int index)
    {
        if (index < 0 || index >= DataModel.EquipDatas.Count)
        {
            return;
        }
        var data = DataModel.EquipDatas[index];
        GameUtils.ShowItemDataTip(data, eEquipBtnShow.None, DataModel.Level);
    }

    public void OnPlayerInfoOperation(IEvent ievent)
    {
        var e = ievent as PlayerInfoOperation;
        switch (e.Type)
        {
            case 0:
            {
//chat
                if (GameUtils.CharacterIdIsRobot(DataModel.CharacterId))
                {
                    GameUtils.ShowHintTip(200000003);
                    return;
                }
                var d = new OperationListData();
                d.CharacterId = DataModel.CharacterId;
                d.CharacterName = DataModel.Name;
                d.RoleId = DataModel.Type;
                d.Level = DataModel.Level;
                d.Ladder = DataModel.Ladder;
                var ee = new ChatMainPrivateChar(d);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                break;
            case 1:
            {
//team
                if (GameUtils.CharacterIdIsRobot(DataModel.CharacterId))
                {
                    GameUtils.ShowHintTip(200000003);
                    return;
                }
                var e1 = new Event_TeamInvitePlayer(DataModel.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
                break;
            case 2:
            {
//friend
                if (GameUtils.CharacterIdIsRobot(DataModel.CharacterId))
                {
                    GameUtils.ShowHintTip(200000003);
                    return;
                }
                var ee = new FriendOperationEvent(0, 1, DataModel.Name, DataModel.CharacterId);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                break;
            case 3:
            {
//trend
                //该功能暂未开放
                var e1 = new ShowUIHintBoard(270216);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
                break;
            case 4:
            {
//elf
                OnClickElfIco();
            }
                break;
            case 10:
            {
//elf
                OnClickEquipIndex(e.Index);
            }
                break;
        }
    }

    public void CleanUp()
    {
        DataModel = new PlayerInfoDataModel();
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        throw new NotImplementedException(name);
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
        var args = data as PlayerInfoArguments;
        if (args == null)
        {
            return;
        }
        var msg = args.PlayerInfoMsg;
        DataModel.CharacterId = msg.Id;
        DataModel.Type = msg.TypeId;
        DataModel.Name = msg.Name;
        DataModel.FightValue = msg.FightPoint;
        DataModel.Level = msg.Level;
        DataModel.Ladder = msg.Ladder;
        if (string.IsNullOrEmpty(msg.GuildName))
        {
            DataModel.GuildName = GameUtils.GetDictionaryText(270024);
        }
        else
        {
            DataModel.GuildName = msg.GuildName;
        }

        DataModel.HpMax = msg.HpMax;
        DataModel.MpMax = msg.MpMax;
        DataModel.PhyArmor = msg.PhyArmor;
        DataModel.MagArmor = msg.MagArmor;
        DataModel.PhyPowerMax = msg.PhyPowerMax;
        DataModel.PhyPowerMin = msg.PhyPowerMin;
        DataModel.MagPowerMax = msg.MagPowerMax;
        DataModel.MagPowerMin = msg.MagPowerMin;
        DataModel.EquipsModel = msg.EquipsModel;
        DataModel.ElfData.Reset();
        for (var i = 0; i < DataModel.EquipDatas.Count; i++)
        {
            var equip = DataModel.EquipDatas[i];
            equip.Reset();
        }
        if (msg.Equips != null)
        {
            {
                // foreach(var itemBaseData in msg.Equips.ItemsChange)
                var enumerator1 = (msg.Equips.ItemsChange).GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    var itemBaseData = enumerator1.Current;
                    {
                        if (itemBaseData.Key == (int) eBagType.Elf)
                        {
                            DataModel.ElfData.ItemId = itemBaseData.Value.ItemId;
                            DataModel.ElfData.Exdata.InstallData(itemBaseData.Value.Exdata);
                            continue;
                        }
                        var tempIndex = GetEquipIndex(itemBaseData.Key);
                        if (tempIndex < 0 || tempIndex >= DataModel.EquipDatas.Count)
                        {
                            continue;
                        }
                        DataModel.EquipDatas[tempIndex].Exdata.InstallData(itemBaseData.Value.Exdata);
                        DataModel.EquipDatas[tempIndex].ItemId = itemBaseData.Value.ItemId;
                    }
                }
            }
        }
        var e = new PlayerInfoRefreshModelView(DataModel.Type, DataModel.EquipsModel, DataModel.ElfData.ItemId);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}