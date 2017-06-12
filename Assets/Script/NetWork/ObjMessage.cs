#region using

using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public partial class NetManager : ClientAgentBase, ILogin9xServiceInterface, ILogic9xServiceInterface,
                                  IScene9xServiceInterface, IRank9xServiceInterface, IActivity9xServiceInterface,
                                  IChat9xServiceInterface, ITeam9xServiceInterface
{
    private void AssignInitBaseData(InitBaseData init, ObjData data)
    {
        init.ObjId = data.ObjId;
        init.DataId = data.DataId;
        init.Name = data.Name;
        init.Level = data.Level;
        init.DirX = GameUtils.DividePrecision(data.Pos.Dir.x);
        init.DirZ = GameUtils.DividePrecision(data.Pos.Dir.y);
        init.X = GameUtils.DividePrecision(data.Pos.Pos.x);
        init.Z = GameUtils.DividePrecision(data.Pos.Pos.y);
        init.Y = GameLogic.GetTerrainHeight(init.X, init.Z);

        init.HpNow = data.HpNow;
        init.HpMax = data.HpMax;
        init.MpNow = data.MpMow;
        init.MpMax = data.MpMax;
        init.Reason = (ReasonType)data.Reason;
        init.ModelId = data.ModelId;
    }

    private void AssignInitCharacterData(InitCharacterData init, ObjData data)
    {
        AssignInitBaseData(init, data);
        init.Camp = data.Camp;
        init.IsDead = data.IsDead;
        init.IsMoving = data.IsMoving;
        init.MoveSpeed = data.Movspeed;
        init.PkModel = data.PkModel;
        init.PkValue = data.PkValue;
        init.Reborn = data.Reborn;
        init.EquipModel = data.EquipsModel;
        init.AreaState = (eAreaState)data.AreaState;
        init.TargetPos = data.TargetPos.Select(item =>
        {
            var x = GameUtils.DividePrecision(item.x);
            var z = GameUtils.DividePrecision(item.y);

            return new Vector3(x, GameLogic.GetTerrainHeight(x, z), z);
        }).ToList();
    }

    private void DelectObj(int reason, ulong objId)
    {
        if (reason == (int)ReasonType.Dead)
        {
            var obj = ObjManager.Instance.FindCharacterById(objId);
            if (obj == null)
            {
                return;
            }
            obj.PlayFadeOutAnimationAndRemove();
            return;
        }
        ObjManager.Instance.RemoveObj(objId);
    }

    public void CreateObj(CreateObjMsg msg)
    {
        if (ObjManager.Instance == null)
        {
            Logger.Log2Bugly("ObjManager.Instance == null");
            return;
        }
        if (PlayerDataManager.Instance == null)
        {
            Logger.Log2Bugly("PlayerDataManager.Instance == null");
            return;
        }
        {
            var __list1 = msg.Data;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var data = __list1[__i1];
                {
                    if (data.ObjId == PlayerDataManager.Instance.GetGuid())
                    {
                        continue;
                    }
                    switch ((OBJ.TYPE)data.Type)
                    {
                        case OBJ.TYPE.NPC:
                            {
                                var init = new InitNPCData();
                                AssignInitCharacterData(init, data);
                                ObjManager.Instance.CreateNPCAsync(init);
                            }
                            break;
                        case OBJ.TYPE.AUTOPLAYER:
                        case OBJ.TYPE.OTHERPLAYER:
                            {
                                var init = new InitOtherPlayerData();
                                if (data.ExtData.Count > 0)
                                {
                                    init.ServerId = data.ExtData[0];
                                }
                                for (var i = 0; i < data.Titles.Count; i++)
                                {
                                    init.TitleList.Add(i, data.Titles[i]);
                                }
                                if (data.Owner != null && data.Owner.Items != null && data.Owner.Items.Count == 1)
                                {
                                    init.RobotId = data.Owner.Items[0];
                                }
                                else
                                {
                                    init.RobotId = 0ul;
                                }
                                init.AllianceName = data.AllianceName;
                                AssignInitCharacterData(init, data);
                                ObjManager.Instance.CreateOtherPlayerAsync(init);
                            }
                            break;
                        case OBJ.TYPE.RETINUE:
                            {
                                var init = new InitRetinueData();
                                AssignInitCharacterData(init, data);
                                if (data.Owner != null && data.Owner.Items != null && data.Owner.Items.Count == 1)
                                {
                                    init.Owner = data.Owner.Items[0];
                                }
                                ObjManager.Instance.CreateRetinueAsync(init);
                            }
                            break;
                        case OBJ.TYPE.DROPITEM:
                            {
                                var init = new InitDropItemData();
                                AssignInitBaseData(init, data);
                                init.Owner.AddRange(data.Owner.Items);
                                init.RemianSeconds = data.ExtData[0];
                                init.PlayDrop = 1 == data.ExtData[1] ? true : false;
                                init.TargetPos = new Vector2(GameUtils.DividePrecision(data.TargetPos[0].x),
                                    GameUtils.DividePrecision(data.TargetPos[0].y));

                                ObjManager.Instance.CreateDropItemAsync(init);
                            }
                            break;
                        default:
                            {
                                Logger.Fatal("Obj[{0}] is unknow type[{1}]", data.ObjId, data.Type);
                            }
                            break;
                    }
                }
            }
        }
    }

    public void DeleteObjList(DeleteObjMsgList dels)
    {
        {
            // foreach(var msg in dels.Datas)
            var __enumerator3 = (dels.Datas).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var msg = __enumerator3.Current;
                {
                    if (msg.ObjId == PlayerDataManager.Instance.GetGuid())
                    {
                        continue;
                    }
                    DelectObj(msg.reason, msg.ObjId);
                    Logger.Debug("DeleteObj[{0}]", msg.ObjId);
                }
            }
        }
    }

    public void DeleteObj(Uint64Array objs, uint reason)
    {
        {
            var __list2 = objs.Items;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var id = __list2[__i2];
                {
                    if (reason == (uint)ReasonType.Dead)
                    {
                        var obj = ObjManager.Instance.FindCharacterById(id);
                        if (obj == null)
                        {
                            continue;
                        }
                        if (obj.Dead && obj.DeleteObjTime > Time.time)
                        {
                            obj.DelayPlayFadeOutAnimationAndRemove(obj.DeleteObjTime - Time.time);
                        }
                        else
                        {
                            obj.PlayFadeOutAnimationAndRemove();
                        }
                        continue;
                    }

                    ObjManager.Instance.RemoveObj(id);
                }
            }
        }
    }

    public void SyncModelId(int modelId)
    {
        var player = ObjManager.Instance.MyPlayer;
        PlayerDataManager.Instance.mInitBaseAttr.ModelId = modelId;
        if (player == null)
        {
            return;
        }
        player.ModelId = modelId;
    }

    public void ObjSpeak(ulong id, int dictId, string content)
    {
        var obj = ObjManager.Instance.FindCharacterById(id);
        if (obj == null)
        {
            return;
        }

        var strContent = "";
        if (dictId > 0)
        {
            //显示泡泡
            var str = GameUtils.GetDictionaryText(dictId);
            strContent = str;
                EventDispatcher.Instance.DispatchEvent(new ShowPopTalk_Event(strContent));
            


        }
        else
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }
            strContent = content;
        }

        if (strContent != "")
        {
            var time = 0.001f * Table.GetClientConfig(218).ToInt();
            obj.PopTalk(strContent, time);

            var chat = new ChatMessageDataModel
            {
                Type = (int)eChatChannel.Scene,
                Name = obj.Name,
                CharId = 0,
                Content = strContent
            };

            EventDispatcher.Instance.DispatchEvent(new Event_PushMessage(chat));
        }
    }
}