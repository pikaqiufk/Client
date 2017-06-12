#region using

using System;
using DataTable;
using EventSystem;
using ServiceBase;
using UnityEngine;

#endregion

public class ObjOtherPlayer : ObjCharacter
{
    public int serverId;
    public ulong RobotId { get; set; }

    public override OBJ.TYPE GetObjType()
    {
        return OBJ.TYPE.OTHERPLAYER;
    }

    public override bool Init(InitBaseData initData, Action callback = null)
    {
        var data = initData as InitOtherPlayerData;
        foreach (var item in data.TitleList)
        {
            if (TitleList.ContainsKey(item.Key))
            {
                TitleList[item.Key] = item.Value;
            }
            else
            {
                TitleList.Add(item.Key, item.Value);
            }
        }
        AllianceName = data.AllianceName;

        RobotId = data.RobotId;

        if (!base.Init(initData))
        {
            return false;
        }
        serverId = data.ServerId;
        AreaState = data.AreaState;

        var tbPvpRule = Table.GetPVPRule(GameLogic.Instance.Scene.TableScene.PvPRule);
        if (tbPvpRule.NameColorRule == (int)NameColorRule.FightingEachOther && GameLogic.Instance.Scene.TableScene.IsHideName == 1)
        {
            Name = GameUtils.GetDictionaryText(220697 + RoleId);
        }
        return true;
    }

    protected override void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(UIEvent_RefleshNameBoard.EVENT_TYPE, RefleshNameBoard);
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ServerRealPos, 0.6f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(Position, Position + ServerRealDir*1.0f);
#endif
    }

    public override void OnNameBoardRefresh()
    {
        if (NameBoard == null)
        {
            return;
        }
        var nameStr = "";
        if (serverId == PlayerDataManager.Instance.ServerId)
        {
            nameStr = Name;
        }
        else
        {
            nameStr = string.Format("[{0}]{1}", GameUtils.GetServerName(serverId), Name);
        }

        if (GameLogic.Instance == null || GameLogic.Instance.Scene == null)
        {
            return;
        }

        var nameRule = 0;
        var sceneid = GameLogic.Instance.Scene.SceneTypeId;
        var tbScene = Table.GetScene(sceneid);
        if (tbScene == null)
        {
            return;
        }
        var isEnemy = ObjManager.Instance.MyPlayer.IsMyEnemy(this);
        var camp = GetCamp();
        if (tbScene.Type == (int)eSceneType.Pvp)
        {
            var campStr = string.Empty;
            if (camp == 4)
            {
                campStr = GameUtils.GetDictionaryText(220448);
            }
            else if (camp == 5)
            {
                campStr = GameUtils.GetDictionaryText(220447);
            }

            var colStr = string.Empty;
            if (camp == 7 || camp == 8 || camp == 9)
            {
                switch (camp)
                {
                    case 7:
                    {
                        colStr = GameUtils.GetTableColorString(503);
                    }
                        break;
                    case 8:
                    {
                        colStr = GameUtils.GetTableColorString(502);
                    }
                        break;
                    case 9:
                    {
                        colStr = GameUtils.GetTableColorString(501);
                    }
                        break;
                }
            }
            else
            {
                if (isEnemy)
                {
                    colStr = GameUtils.ColorToString(Color.red);
                }
                else
                {
                    colStr = GameUtils.ColorToString(Color.green);
                }
            }

            SetNameBoardName(nameStr, campStr, CharacterBaseData.Reborn, colStr);
        }
        else
        {
            var rule = tbScene.PvPRule;
            var tbRule = Table.GetPVPRule(rule);
            if (tbRule != null)
            {
                nameRule = tbRule.NameColorRule;
            }
            var c = Color.green;
            if (nameRule == (int) NameColorRule.PkValue)
            {
//根据杀气值显示颜色
                if (PkValue >= 10000)
                {
//灰[C0C0C0]
                    c = GameUtils.GetTableColor(96);
                }
                else if (PkValue > 0 && PkValue < 10000)
                {
                    var colorId = GameUtils.PkValueToColorId(PkValue);
                    if (colorId != -1)
                    {
                        c = GameUtils.GetTableColor(colorId);
                    }
                }
            }
            else
            {
//根据阵营
                if (isEnemy)
                {
//是敌人直接设为红色
                    c = Color.red;
                }
            }

            SetNameBoardName(nameStr, String.Empty, CharacterBaseData.Reborn, GameUtils.ColorToString(c));
        }
        NameBoardUpdate();
    }

    protected override void OnSetModel()
    {
        mMountPoints.Clear();
        if (mActorAvatar)
        {
            mActorAvatar.LayerMask = 0;
            mActorAvatar.Body = mModel;
        }
        gameObject.SetLayerRecursive(gameObject.layer);
    }

    private void RefleshNameBoard(IEvent ievent)
    {
        NameBoardUpdate();
    }

    private void SetTitle(int id, int value)
    {
        if (TitleList.ContainsKey(id))
        {
            TitleList[id] = value;
        }
        else
        {
            TitleList.Add(id, value);
        }
        NameBoardUpdate();
    }

    protected override void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.AddEventListener(UIEvent_RefleshNameBoard.EVENT_TYPE, RefleshNameBoard);
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    public override void StartAttributeSync()
    {
        base.StartAttributeSync();

        var characterId = GetObjId();

        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncLevel,
            (int i) => { SetLevel(i); });

        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncReborn,
            (int i) => { CharacterBaseData.Reborn = i; });
        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncPkModel,
            (int i) => { PkModel = i; });

        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncPkValue,
            (int i) =>
            {
                PkValue = i;
                OnNameBoardRefresh();
            });

        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncAreaState,
            (int i) =>
            {
                var state = i;
                AreaState = (eAreaState) state;
                RefreshAnimation();
            });

        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncTitle0,
            (int i) => { SetTitle(0, i); });

        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncTitle1,
            (int i) => { SetTitle(1, i); });

        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncTitle2,
            (int i) => { SetTitle(2, i); });

        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncTitle3,
            (int i) => { SetTitle(3, i); });
        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId,
            (uint) eSceneSyncId.SyncAllianceName,
            (string name) =>
            {
                AllianceName = name;
                NameBoardUpdate();
            });
    }

    public override void StopAttributeSync()
    {
        base.StopAttributeSync();
        var characterId = GetObjId();
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncLevel);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncPkModel);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncPkValue);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncAreaState);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncTitle0);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncTitle1);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncTitle2);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncTitle3);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncAllianceName);
    }
}