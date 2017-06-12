#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class ObjMyPlayer : ObjCharacter
{
    public PlayerAutoCombat AutoCombat;
    public ElfFollow ElfFollow;
    private SkillRecord mCurrentSkillRecord;
    private Vector2 mLastDir;
    private float mLastSyncDirTime;
    private Vector3 mLastSyncPos;
    private float mLastSyncPosTime;
    private bool mNeedUpdateSkillIndicator;
    public Action MoveOverCallBack;
    //清除技能指示器
    private readonly List<Action> mRemoveSkillIndicator = new List<Action>();
    protected bool mSendStopFlag;
    private bool mSkillIndicatorVisible;
    private int mTickCount;
    private GameObject TargetIndicator;
    //解决位置卡死用的上一秒合法地址
    private Vector3 mLastSafePos;

    public int fastReachSceneID = -1;
    public Vector3 fastReachPos;

    public override CharacterBaseDataModel CharacterBaseData
    {
        get { return PlayerDataManager.Instance.PlayerDataModel.CharacterBase; }
        set { PlayerDataManager.Instance.PlayerDataModel.CharacterBase = value; }
    }

    public float RemindNoMoveTime { get; private set; }

    //调整下高度
    public void AdjustHeightPosition()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(Position, out hit, 2, -1))
        {
            var temp = Position;
            temp.y = hit.position.y;
            Position = temp;
        }
    }

    public void AutoCombatSkill()
    {
        var autoCombat = AutoCombat;
        if (autoCombat == null)
        {
            Logger.Error("autoCombat cast is null");
            return;
        }
        //手动放技能推迟循环时间一点
        autoCombat.CancelLoop();
    }

    private void AutoCombatToggle(bool isAuto, List<int> list = null)
    {
        if (AutoCombat == null)
        {
            Logger.Error("AutoCombat is null");
            return;
        }
        var autoCombat = AutoCombat;
        if (autoCombat == null)
        {
            Logger.Error("autoCombat cast is null");
            return;
        }

        if (list == null)
        {
            AutoCombat.GetMissionList().Clear();
        }
        else
        {
            AutoCombat.SetMissionList(list);
        }
        AutoCombat.enabled = isAuto;
    }

    protected override void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        base.Awake();

        if (AutoCombat == null)
        {
            AutoCombat = gameObject.AddComponent<PlayerAutoCombat>();
            AutoCombat.enabled = false;
            AutoCombat.MainPlayer = this;
        }

        if (ElfFollow == null)
        {
            ElfFollow = gameObject.AddComponent<ElfFollow>();
            ElfFollow.Owner = this;
        }

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    //是否可以移动
    public bool CanMove()
    {
        if (mDead)
        {
            return false;
        }

        if (GetCurrentStateName() == OBJ.CHARACTER_STATE.DIZZY)
        {
            return false;
        }

        //在强制移动中
        if (IsForceMoving)
        {
            return false;
        }

        if (HaveImmovableBuff())
        {
            return false;
        }

        return true;
    }

    //是否可以转向
    public bool CanTurnFace()
    {
        if (mDead)
        {
            return false;
        }

        //在强制移动中
        if (mForceMoveRemindTime > 0)
        {
            return false;
        }

        //眩晕中
        if (GetCurrentStateName() == OBJ.CHARACTER_STATE.DIZZY)
        {
            return false;
        }

        return true;
    }

    public bool CheckPkModel(ObjOtherPlayer obj)
    {
        var caster = this;
        var target = obj;
        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        var tbScene = Table.GetScene(sceneId);
        if (tbScene == null)
        {
            return false;
        }
        var tbPvP = Table.GetPVPRule(tbScene.PvPRule);
        if (tbPvP == null)
        {
            return false;
        }
        if (tbPvP.CanPK == 1)
        {
            if (target.GetLevel() >= tbPvP.ProtectLevel && caster.GetLevel() >= tbPvP.ProtectLevel)
            {
                if (CheckPKModel(caster, target, tbPvP))
                {
                    return true;
                }
            }
        }
        var tbCasterCamp = Table.GetCamp(caster.GetCamp());
        if (tbCasterCamp == null)
        {
            return false;
        }
        var targetCamp = target.GetCamp();
        if (targetCamp < 0 || targetCamp >= tbCasterCamp.Camp.Length)
        {
            return false;
        }
        if (tbCasterCamp.Camp[targetCamp] == 1)
        {
            return true;
        }
        return false;
    }

    //战盟Id
    public static bool CheckPKModel(ObjMyPlayer casterPlayer, ObjOtherPlayer targetPlayer, PVPRuleRecord tbPvP)
    {
        if (casterPlayer == targetPlayer)
        {
            return false;
        }

        var tbPKModel = Table.GetPKMode(PlayerDataManager.Instance.GetPkModel());
        if (tbPKModel == null)
        {
            return false;
        }
        var PkValue = targetPlayer.PkValue;
        var isRedName = PkValue >= 100;
        if (isRedName)
        {
            if (PlayerDataManager.Instance.CheckSameTeam(targetPlayer.GetObjId()))
            {
                if (tbPKModel.RedTeam == 2)
                {
                    return true;
                }
                return false;
            }
            if (tbPKModel.RedTeam == 1 || tbPKModel.RedTeam == 2)
            {
                return true;
            }

            if (PlayerDataManager.Instance.CheckSameUnion(targetPlayer.GetObjId()))
            {
                if (tbPKModel.RedUnion == 2)
                {
                    return true;
                }
                return false;
            }
            if (tbPKModel.RedUnion == 1 || tbPKModel.RedUnion == 2)
            {
                return true;
            }

            if (tbPKModel.RedState == 1)
            {
                return true;
            }
            return false;
        }
        if (PlayerDataManager.Instance.CheckSameTeam(targetPlayer.GetObjId()))
        {
            if (tbPKModel.NomalTeam == 2)
            {
                return true;
            }
            return false;
        }
        if (tbPKModel.NomalTeam == 1 || tbPKModel.NomalTeam == 2)
        {
            return true;
        }

        if (PlayerDataManager.Instance.CheckSameUnion(targetPlayer.GetObjId()))
        {
            if (tbPKModel.NomalUnion == 2)
            {
                return true;
            }
            return false;
        }
        if (tbPKModel.NomalUnion == 1 || tbPKModel.NomalUnion == 2)
        {
            return true;
        }

        if (tbPKModel.NomalState == 1)
        {
            return true;
        }
        return false;
    }

    public override void Destroy()
    {
        Destroy(gameObject);
    }

    public override void DoDie()
    {
        base.DoDie();

        if (IsAutoFight())
        {
            LeaveAutoCombat();
        }

        EventDispatcher.Instance.DispatchEvent(new UIEvent_RemoveBuffsOnDead());
    }

    public void EnterAutoCombat(List<int> list = null)
    {
        var mainCon = UIManager.Instance.GetController(UIConfig.MainUI);
        if (mainCon != null)
        {
            var dataModel = mainCon.GetDataModel("MainUI") as MainUIDataModel;
            if (dataModel != null)
            {
                dataModel.IsAutoFight = 1;
            }
        }
        AutoCombatToggle(true, list);
    }

    //获得buff参数
    public static bool GetBuffEffectParam(int buffId, int effectid, int paramid, int value)
    {
        var tableBuff = Table.GetBuff(buffId);
        if (tableBuff == null)
        {
            Logger.Error("GetBuffEffectParam tableBuff is null,buffId={0}", buffId);
            return false;
        }
        for (var j = 0; j != tableBuff.effectid.Length; ++j)
        {
            if (tableBuff.effectid[j] == effectid)
            {
                if (j < tableBuff.effectparam.GetLength(0) && paramid >= 0 &&
                    paramid < tableBuff.effectparam.GetLength(0))
                {
                    if (BitFlag.GetLow(tableBuff.effectparam[j, paramid], value))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //等级
    public override int GetLevel()
    {
        return PlayerDataManager.Instance.GetLevel();
    }

    public override OBJ.TYPE GetObjType()
    {
        return OBJ.TYPE.MYPLAYER;
    }

    //获得天赋修改后的技能数据
    public static int GetSkillData_Data(SkillRecord data, eModifySkillType index)
    {
        var oldValue = 0;
        switch (index)
        {
            case eModifySkillType.Cd:
                oldValue = data.Cd;
                break;
            case eModifySkillType.Layer:
                oldValue = data.Layer;
                break;
            case eModifySkillType.TargetType:
                oldValue = data.TargetType;
                break;
            case eModifySkillType.CastType:
                oldValue = data.CastType;
                break;
            case eModifySkillType.TargetParam1:
                oldValue = data.TargetParam[0];
                break;
            case eModifySkillType.TargetParam2:
                oldValue = data.TargetParam[1];
                break;
            case eModifySkillType.TargetParam3:
                oldValue = data.TargetParam[2];
                break;
            case eModifySkillType.TargetParam4:
                oldValue = data.TargetParam[3];
                break;
            case eModifySkillType.ControlType:
                oldValue = data.ControlType;
                break;
            case eModifySkillType.TargetCount:
                oldValue = data.TargetCount;
                break;
        }
        {
            // foreach(var talent in PlayerDataManager.Instance.PlayerDataModel.SkillData.Talents)
            var __enumerator3 = (PlayerDataManager.Instance.PlayerDataModel.SkillData.Talents).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var talent = __enumerator3.Current;
                {
                    var talentId = talent.TalentId;
                    var talentCount = talent.Count;
                    if (talentCount <= 0)
                    {
                        continue;
                    }
                    var tbTalent = Table.GetTalent(talentId);
                    if (tbTalent == null)
                    {
                        continue;
                    }
                    if (tbTalent.ModifySkill != data.Id)
                    {
                        continue;
                    }
                    var tbBuff = Table.GetBuff(tbTalent.BuffId[talentCount - 1]);
                    if (tbBuff == null)
                    {
                        continue;
                    }
                    var effectIndex = -1;
                    {
                        // foreach(var effectId in tbBuff.effectid)
                        var __enumerator10 = (tbBuff.effectid).GetEnumerator();
                        while (__enumerator10.MoveNext())
                        {
                            var effectId = (int) __enumerator10.Current;
                            {
                                effectIndex++;
                                if (effectId != 14)
                                {
                                    continue;
                                }
                                if (tbBuff.effectparam[effectIndex, 0] != data.Id)
                                {
                                    continue;
                                }
                                if (tbBuff.effectparam[effectIndex, 1] != (int) index)
                                {
                                    continue;
                                }
                                switch (tbBuff.effectparam[effectIndex, 2])
                                {
                                    case 0: //覆盖
                                    {
                                        return tbBuff.effectparam[effectIndex, 3];
                                    }
                                        break;
                                    case 1: //万份比
                                    {
                                        return oldValue*(10000 + tbBuff.effectparam[effectIndex, 3])/10000;
                                    }
                                        break;
                                    case 2:
                                    {
//固定值
                                        return oldValue + tbBuff.effectparam[effectIndex, 3];
                                    }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        return oldValue;
    }

    //获得天赋修改后的技能范围数据
    public int GetSkillData_TargetParam(SkillRecord data, int index)
    {
        var oldValue = data.TargetParam[index];
        {
            // foreach(var talent in PlayerDataManager.Instance.PlayerDataModel.SkillData.Talents)
            var __enumerator2 = (PlayerDataManager.Instance.PlayerDataModel.SkillData.Talents).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var talent = __enumerator2.Current;
                {
                    var talentId = talent.TalentId;
                    var talentCount = talent.Count;
                    if (talentCount <= 0)
                    {
                        continue;
                    }
                    var tbTalent = Table.GetTalent(talentId);
                    if (tbTalent == null)
                    {
                        continue;
                    }
                    if (tbTalent.ModifySkill != data.Id)
                    {
                        continue;
                    }
                    var tbBuff = Table.GetBuff(tbTalent.BuffId[talentCount - 1]);
                    if (tbBuff == null)
                    {
                        continue;
                    }
                    var effectIndex = -1;
                    {
                        // foreach(var effectId in tbBuff.effectid)
                        var __enumerator9 = (tbBuff.effectid).GetEnumerator();
                        while (__enumerator9.MoveNext())
                        {
                            var effectId = (int) __enumerator9.Current;
                            {
                                effectIndex++;
                                if (effectId != 14)
                                {
                                    continue;
                                }
                                if (tbBuff.effectparam[effectIndex, 0] != data.Id)
                                {
                                    continue;
                                }
                                if (tbBuff.effectparam[effectIndex, 1] != index + 19)
                                {
                                    continue;
                                }
                                switch (tbBuff.effectparam[effectIndex, 2])
                                {
                                    case 0: //覆盖
                                    {
                                        return tbBuff.effectparam[effectIndex, 3];
                                    }
                                        break;
                                    case 1: //万份比
                                    {
                                        return oldValue*(10000 + tbBuff.effectparam[effectIndex, 3])/10000;
                                    }
                                        break;
                                    case 2:
                                    {
//固定值
                                        return oldValue + tbBuff.effectparam[effectIndex, 3];
                                    }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        return oldValue;
    }

    //是否有定身buff
    public bool HaveImmovableBuff()
    {
        var bl = mBuff.GetBuffData();
        var blc = bl.Count;
        for (var i = 0; i < blc; ++i)
        {
            var buff = bl[i];
            {
                if (GetBuffEffectParam(buff.BuffTypeId, 9, 0, 0))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool Init(InitBaseData initData, Action callback = null)
    {
        mObjId = initData.ObjId;
        mDataId = initData.DataId;
        RoleId = initData.DataId;
        var data = initData as InitMyPlayerData;
        Name = data.Name;
        mDead = data.IsDead;
        DeleteObjTime = 0;
        SetCamp(data.Camp);
        mActorAvatar.IsMainPlayer = true;
        EquipList = new Dictionary<int, int>(data.EquipModel);
        CharacterBaseData.Name = data.Name;
        CharacterBaseData.Reborn = PlayerDataManager.Instance.GetExData(51);
        CharacterBaseData.Level = PlayerDataManager.Instance.GetLevel();
        CharacterBaseData.CharacterId = data.ObjId;
        Position = new Vector3(initData.X, initData.Y, initData.Z);
        Direction = new Vector3(initData.DirX, 0, initData.DirZ);
        AreaState = data.AreaState;
        PlayerDataManager.Instance.SetAttribute(eAttributeType.HpMax, data.HpMax);
        PlayerDataManager.Instance.SetAttribute(eAttributeType.HpNow, data.HpNow);
        PlayerDataManager.Instance.SetAttribute(eAttributeType.MpMax, data.MpMax);
        PlayerDataManager.Instance.SetAttribute(eAttributeType.MpNow, data.MpNow);
        CharacterBaseData.MaxHp = data.HpMax;
        CharacterBaseData.MaxMp = data.MpMax;
        CharacterBaseData.Hp = data.HpNow;
        CharacterBaseData.Mp = data.MpNow;
        mCharaModelID = data.ModelId;
        AllianceName = PlayerDataManager.Instance.BattleName;
        UsingFakeModel = false;
        InitNameBoard();
        InitTableData();
        InitAnimation(true);
        InitStateMachine();
        InitNavMeshAgent();
        SetMoveSpeed(data.MoveSpeed);
        LoadModel();
        InitShadow(true);
        InitEquip();
        DoIdle();

        State = ObjState.Normal;

        //设置空手技能
        var tbCharacterBase = Table.GetCharacterBase(RoleId);
        var skillId = tbCharacterBase.InitSkill[13];
        PlayerDataManager.Instance.SetNoWeaponSkill(skillId);

        InitElfFollow();
        {
            // foreach(var buff in data.Buff)
            var __enumerator4 = (data.Buff).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var buff = __enumerator4.Current;
                {
                    AddBuff(buff.Key, buff.Value, 0, 0);
                }
            }
        }

        if (null != callback)
        {
            callback();
        }
        mLastSafePos = Vector3.zero;
        return true;
    }
    
    private void InitElfFollow()
    {
        var ret = (int) UIManager.Instance.GetController(UIConfig.ElfUI).CallFromOtherClass("GetFightModel", null);
        RefresElfFollow(ret);
    }

    private void InitNameBoard()
    {
        var list = PlayerDataManager.Instance.TitleList;
        foreach (var item in list)
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
    }

    public bool IsAutoFight()
    {
        if (AutoCombat == null)
        {
            return false;
        }
        return AutoCombat.enabled;
    }

    //判断是否是敌人
    public override bool IsMyEnemy(ObjCharacter obj)
    {
        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        var tbScene = Table.GetScene(sceneId);
        var otherPlayer = obj as ObjOtherPlayer;
        var retinue = obj as ObjRetinue;
        if (retinue != null)
        {
            //各自为战的场景pvp规则
            if (tbScene.PvPRule == 5)
            {
                return !retinue.GetIsMe();
            }
            var p = ObjManager.Instance.FindObjById(retinue.OwnerId) as ObjCharacter;
            if (p != null)
            {
                var player = p as ObjOtherPlayer;
                if (player != null)
                {
                    otherPlayer = player;
                }
                else
                {
                    obj = p;
                }
            }
        }
        if (otherPlayer != null)
        {
            //各自为战的场景pvp规则
            if (tbScene.PvPRule == 5)
            {
                return true;
            }

            if (tbScene.Type != 3)
            {
                return CheckPkModel(otherPlayer);
            }
        }

        if (null == TableCamp)
        {
            return false;
        }

        if (obj.GetCamp() < 0 || obj.GetCamp() >= TableCamp.Camp.Length)
        {
            return false;
        }

        return 1 == TableCamp.Camp[obj.GetCamp()];
    }

    public void LeaveAutoCombat()
    {
        var mainCon = UIManager.Instance.GetController(UIConfig.MainUI);
        if (mainCon != null)
        {
            var dataModel = mainCon.GetDataModel("MainUI") as MainUIDataModel;
            if (dataModel != null)
            {
                dataModel.IsAutoFight = 0;
            }
        }

        AutoCombatToggle(false);
    }

    private void LoadModel()
    {
        var tableModel = Table.GetCharModel(ModelId);
        var modelPath = Resource.GetModelPath(tableModel.ResPath);

        var res = ResourceManager.PrepareResourceSync<GameObject>(modelPath);

        if (res == null)
        {
            Logger.Error("load " + modelPath + " error.");
            return;
        }
        mActorAvatar.Layer = gameObject.layer;
        mActorAvatar.LayerMask = LayerMask.GetMask(GAMELAYER.Collider, GAMELAYER.IgnoreShadow);
        var model = Instantiate(res) as GameObject;
        model.layer = gameObject.layer;
        SetModel(model);
        mAnimationController.Animation = mModel.animation;
        mAnimationController.Animation.Stop();
        SetScale((float) tableModel.Scale);

        //缓存top挂载点位置，为了后面装在翅膀偏移用
        var topMount = GetMountPoint(3);
        if (null != topMount)
        {
            mTopMountPointPos = topMount.localPosition;
        }
    }

    protected override void OnAddBuff(Buff buff)
    {
        if (HaveImmovableBuff())
        {
            if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
            {
                StopMove();
            }
        }
    }

    protected override void OnDestroy()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        EventDispatcher.Instance.RemoveEventListener(UIEvent_SkillButtonPressed.EVENT_TYPE, OnSkillButtonPressed);
        EventDispatcher.Instance.RemoveEventListener(UIEvent_SkillButtonReleased.EVENT_TYPE, OnSkillButtonReleased);
        EventDispatcher.Instance.RemoveEventListener(Event_LevelUp.EVENT_TYPE, OnLevelUp);
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

        try
        {
            const int miles = 5;
            const int segment = 36;
            var perRandian = 2*Mathf.PI/segment;
            Color[] color = {Color.white, Color.black, Color.red, Color.green, Color.blue};

            var curPos = Position;
            for (var r = 0; r < miles; r++)
            {
                Gizmos.color = color[r];
                var nextPos = curPos + Direction;
                Gizmos.DrawLine(curPos, nextPos);

                float raidus = r + 1;
                for (var i = 0; i < segment*raidus; i++)
                {
                    var radian = perRandian*i;
                    var p1 = Position + new Vector3(Mathf.Cos(radian)*raidus, 0, Mathf.Sin(radian)*raidus);
                    var p2 = Position +
                             new Vector3(Mathf.Cos(radian + perRandian)*raidus, 0, Mathf.Sin(radian + perRandian)*raidus);
                    Gizmos.DrawLine(p1, p2);
                }
                curPos = nextPos;
            }

            var l = new[] {new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 7), new Vector3(-1, 0, 7)};
            var p = l.Select(item => (mTransform.localToWorldMatrix.MultiplyPoint3x4(item))).ToArray();


            Vector2 mDir;
            Vector2 mCenter;
            float w = 2;
            float h = 7;

            var targetObj = ObjManager.Instance.SelectNearestCharacter(Position,
                character => { return !character.Dead && IsMyEnemy(character); });

            if (targetObj != null)
            {
                mDir = (Direction.xz()).normalized;
                mCenter = Position.xz() + Direction.xz()*h/2;

                //var theta = -Mathf.Atan2(mDir.y, mDir.x);
                var sin = mDir.x;
                var cos = mDir.y;


                var mHalfWidth = w*0.5f;
                var mHalfLength = h*0.5f;


                var newx = (cos*(targetObj.Position.x - mCenter.x) - sin*(targetObj.Position.z - mCenter.y));
                var newy = (sin*(targetObj.Position.x - mCenter.x) + cos*(targetObj.Position.z - mCenter.y));


                if ((newy > -mHalfLength) && (newy < mHalfLength)
                    && (newx > -mHalfWidth) && (newx < mHalfWidth))
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.blue;
                }

                for (var i = 0; i < 4; i++)
                {
                    Gizmos.DrawLine(p[i], p[(i + 1)%4]);
                }
            }

            {
                Gizmos.color = Color.green;
                var pp = Position;
                var TargetPosCount2 = TargetPos.Count;
                for (var i = 0; i < TargetPosCount2; i++)
                {
                    Gizmos.DrawLine(pp, TargetPos[i]);
                    pp = TargetPos[i];
                }
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ServerRealPos, 0.6f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ServerRealPos, ServerRealPos + ServerRealDir*1.0f);

            Gizmos.color = Color.red;
// 			for(int i=0; i<VList.List.Count-1; i++)
// 			{
// 				var temp = VList.List[i];
// 				var p1 = GameLogic.Instance.Scene.GetTerrainPosition(temp.x / 100.0f, temp.y / 100.0f);
// 				var p2 = GameLogic.Instance.Scene.GetTerrainPosition(VList.List[i+1].x / 100.0f, VList.List[i+1].y / 100.0f);
// 				Gizmos.DrawLine(p1, p2);
// 			}
        }
        catch (Exception e)
        {
        }
#endif
    }

    /// <summary>
    ///     播放升级动画
    /// </summary>
    /// <param name="iEvent"></param>
    private void OnLevelUp(IEvent iEvent)
    {
        var effect = Table.GetEffect(int.Parse(Table.GetClientConfig(400).Value));
        EffectManager.Instance.CreateEffect(effect, this);
        var sound = int.Parse(Table.GetClientConfig(403).Value);
        SoundManager.Instance.PlaySoundEffect(sound);
    }

    public override void OnMoveOver()
    {
        base.OnMoveOver();
        GameLogic.Instance.Scene.DisableActiveMovingCircle();
    }

    public override void OnNameBoardRefresh()
    {
        RefreshnameBarod();
    }

    //当模型改变时
    protected override void OnSetModel()
    {
        base.OnSetModel();
        const string MainTexVariableName = "_MainTex";
        ResourceManager.PrepareResource<Material>(Resource.Material.MainPlayerMaterial, mat =>
        {
            OptList<Renderer>.List.Clear();
            mModel.GetComponentsInChildren(OptList<Renderer>.List);
            {
                var __array6 = OptList<Renderer>.List;
                var __arrayLength6 = __array6.Count;
                for (var __i6 = 0; __i6 < __arrayLength6; ++__i6)
                {
                    var render = __array6[__i6];
                    {
                        var oreginalMaterial = render.sharedMaterial;
                        var currentRender = render;

                        var newMat = new Material(mat);
                        newMat.SetTexture(MainTexVariableName, oreginalMaterial.GetTexture(MainTexVariableName));

                        currentRender.material = newMat;

                        ResourceManager.ChangeShader(mModelTransform);
                    }
                }
            }
        }, true, true, true);
    }

    private void OnSkillButtonPressed(IEvent evt)
    {
        var e = evt as UIEvent_SkillButtonPressed;

        if (e.SkillId == -1)
        {
            //GameControl.Instance.TargetObj = null;
            return;
        }

        var ret = GameControl.Instance.CheckSkill(e.SkillId);

        if (ErrorCodes.OK != ret)
        {
            if (ErrorCodes.Error_SkillNoCD == ret)
            {
                //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(714));
                return;
            }
            GameUtils.ShowNetErrorHint((int) ret);
            return;
        }

        mSkillIndicatorVisible = true;
        InputManager.Instance.SkillButtonPreesed = true;
        InputManager.Instance.SkillButtonPressedPosition = UICamera.currentTouch.pos;

        var data = Table.GetSkill(e.SkillId);
        mCurrentSkillRecord = data;

        var targetType = (SkillTargetType) GetSkillData_Data(data, eModifySkillType.TargetType); //data.TargetType;

        if (targetType == SkillTargetType.CIRCLE)
        {
            GameLogic.Instance.Scene.CreateSkillRangeIndicator(gameObject, Scene.SkillRangeIndicatorType.Circle,
                GetSkillData_TargetParam(data, 0), 0, Color.green, RemoveSkillIndicator);
        }
        else if (targetType == SkillTargetType.SINGLE ||
                 targetType == SkillTargetType.TARGET_CIRCLE ||
                 targetType == SkillTargetType.TARGET_RECT ||
                 targetType == SkillTargetType.TARGET_SECTOR ||
                 targetType == SkillTargetType.RECT ||
                 targetType == SkillTargetType.SECTOR)
        {
            var targetObj = ObjManager.Instance.SelectTargetForSkill(this, data);
            GameControl.Instance.TargetObj = targetObj;

            if (targetType == SkillTargetType.TARGET_CIRCLE ||
                targetType == SkillTargetType.TARGET_RECT ||
                targetType == SkillTargetType.TARGET_SECTOR)
            {
                mNeedUpdateSkillIndicator = true;
            }
            else
            {
                mNeedUpdateSkillIndicator = GameSetting.Instance.TargetSelectionAssistant;
            }


            ShowIndicatorForTarget(targetObj, data);
        }
    }

    private void OnSkillButtonReleased(IEvent evt)
    {
        var e = (UIEvent_SkillButtonReleased) evt;

        mSkillIndicatorVisible = false;
        if (InputManager.Instance != null)
        {
            InputManager.Instance.SkillButtonPreesed = false;
        }
        mNeedUpdateSkillIndicator = false;
        if (e.UseSkill)
        {
            var data = Table.GetSkill(e.SkillId);
            if (data != null)
            {
                var targetType = (SkillTargetType) GetSkillData_Data(data, eModifySkillType.TargetType);

                if (targetType == SkillTargetType.SINGLE || targetType == SkillTargetType.SELF ||
                    (TargetIndicator != null && (TargetIndicator.activeSelf)))
                {
                    // 圆形不需要控制方向
                    if (TargetIndicator && targetType != SkillTargetType.CIRCLE)
                    {
                        TargetDirection = TargetIndicator.transform.forward;
                    }
                    GameControl.Instance.OnClickEvent(1, e.SkillId);
                }
//                 else
//                 {
//                     if (IsInSafeArea())
//                     {//您当前在城镇中，无法施放技能
//                         EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(41003));
//                     }
//                 }
            }
        }

        RemoveAllSkillIndicator();
    }

    private void RefleshNameBoard(IEvent ievent)
    {
        ObjManager.Instance.MyPlayer.NameBoardUpdate();
    }

    public void RefresElfFollow(int dataId)
    {
        ElfFollow.CreateObj(dataId);
    }

    public void RefreshnameBarod()
    {
        if (NameBoard == null)
        {
            return;
        }
        if (GameLogic.Instance == null || GameLogic.Instance.Scene == null)
        {
            return;
        }

        var sceneid = GameLogic.Instance.Scene.SceneTypeId;
        var tbScene = Table.GetScene(sceneid);
        if (tbScene == null)
        {
            return;
        }

        NameBoard.mConstraint.enabled = false;

        if (HasWing())
        {
            NameBoard.transform.localPosition = new Vector3(0, 83, 0);
            NameBoard.SetFlyOffset();
        }
        else
        {
            NameBoard.transform.localPosition = new Vector3(0, 65, 0);
            NameBoard.ResetOffset();
        }

        var camp = GetCamp();
        if (tbScene.Type == (int)eSceneType.Pvp && camp != 0)
        {
//根据阵营去格式化
            var str = "";
            if (camp == 4)
            {
                str = GameUtils.GetDictionaryText(220448);
            }
            else if (camp == 5)
            {
                str = GameUtils.GetDictionaryText(220447);
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
                colStr = GameUtils.ColorToString(Color.green);
            }

            SetNameBoardName(name, str, CharacterBaseData.Reborn, colStr);
        }
        else
        {
            var nameRule = 0;
            var rule = tbScene.PvPRule;
            var tbRule = Table.GetPVPRule(rule);
            if (tbRule != null)
            {
                nameRule = tbRule.NameColorRule;
            }
            var c = Color.green;
            if (nameRule == (int) NameColorRule.PkValue)
            {
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

            SetNameBoardName(name, string.Empty, CharacterBaseData.Reborn, GameUtils.ColorToString(c));
        }
        NameBoardUpdate();
    }

    public override void Relive()
    {
        base.Relive();

        if (UIConfig.ReliveUI.Visible())
        {
            var e = new Close_UI_Event(UIConfig.ReliveUI);
            EventDispatcher.Instance.DispatchEvent(e);

            var e1 = new Show_UI_Event(UIConfig.MainUI);
            EventDispatcher.Instance.DispatchEvent(e1);
        }
    }

    private void RemoveAllSkillIndicator()
    {
        var __listCount1 = mRemoveSkillIndicator.Count;
        for (var __i1 = 0; __i1 < __listCount1; ++__i1)
        {
            var action = mRemoveSkillIndicator[__i1];
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
        }
    }

    private void RemoveSkillIndicator(GameObject caster, GameObject receiver)
    {
        if (!mSkillIndicatorVisible)
        {
            Destroy(caster);
            Destroy(receiver);
            return;
        }

        var o = new GameObject();
        var oTransform = o.transform;
        oTransform.parent = gameObject.transform;
        oTransform.localPosition = Vector3.zero;
        caster.transform.parent = oTransform;
        receiver.transform.parent = oTransform;
        TargetIndicator = o;

        mRemoveSkillIndicator.Add(() =>
        {
            Destroy(caster);
            Destroy(receiver);
            Destroy(o);
        });
    }

    private void RemoveSkillIndicator(GameObject o)
    {
        if (!mSkillIndicatorVisible)
        {
            Destroy(o);
            TargetIndicator = null;
            return;
        }

        mRemoveSkillIndicator.Add(() =>
        {
            Destroy(o);
            TargetIndicator = null;
        });
    }

    private bool ShouldShowIndicator()
    {
        if (Input.touchCount > 1)
        {
            var minDiff = float.MaxValue;
            for (var i = 0; i < Input.touchCount; ++i)
            {
                var touch = Input.touches[i];
                var touchPos = touch.position - InputManager.Instance.SkillButtonPressedPosition;
                var diff = touchPos.SqrMagnitude();
                if (diff < minDiff)
                {
                    minDiff = diff;
                }
            }

            if (minDiff > 22500)
            {
                return false;
            }
            return true;
        }
        if (Input.GetMouseButton((int) InputManager.MOUSE_BUTTON.MOUSE_BUTTON_LEFT))
        {
            var diff = Input.mousePosition.xy() -
                       InputManager.Instance.SkillButtonPressedPosition;

            if (diff.sqrMagnitude > 22500)
            {
                return false;
            }
            return true;
        }

        return true;
    }

    private void ShowIndicatorForTarget(ObjCharacter targetObj, SkillRecord data)
    {
        var targetType = (SkillTargetType) GetSkillData_Data(data, eModifySkillType.TargetType); //data.TargetType;

        if (targetType == SkillTargetType.TARGET_CIRCLE)
        {
            if (targetObj == null)
            {
                return;
            }

            GameLogic.Instance.Scene.CreateSkillRangeIndicator(targetObj.gameObject,
                Scene.SkillRangeIndicatorType.Circle,
                GetSkillData_TargetParam(data, 0), 0, Color.green, RemoveSkillIndicator);
        }
        else if (targetType == SkillTargetType.TARGET_SECTOR || targetType == SkillTargetType.TARGET_RECT ||
                 targetType == SkillTargetType.SECTOR || targetType == SkillTargetType.RECT)
        {
            var o = new GameObject();
            var oTransform = o.transform;
            oTransform.parent = gameObject.transform;
            oTransform.localPosition = Vector3.zero;
            if (targetObj != null)
            {
                var dir = (targetObj.Position.xz() - Position.xz()).normalized;
                var angle = dir.GetAngle(Direction.xz().normalized);
                oTransform.localRotation = Quaternion.Euler(0, angle*Mathf.Rad2Deg, 0);
            }
            else
            {
                oTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            TargetIndicator = o;

            if (targetType == SkillTargetType.TARGET_SECTOR || targetType == SkillTargetType.SECTOR)
            {
                GameLogic.Instance.Scene.CreateSkillRangeIndicator(o, Scene.SkillRangeIndicatorType.Fan,
                    GetSkillData_TargetParam(data, 0), GetSkillData_TargetParam(data, 1), Color.green,
                    (o1, o2) => { RemoveSkillIndicator(o); });
            }
            else if (targetType == SkillTargetType.TARGET_RECT || targetType == SkillTargetType.RECT)
            {
                GameLogic.Instance.Scene.CreateSkillRangeIndicator(o, Scene.SkillRangeIndicatorType.Rectangle,
                    GetSkillData_TargetParam(data, 0), GetSkillData_TargetParam(data, 1), Color.green,
                    (o1, o2) => { RemoveSkillIndicator(o); });
            }
        }
    }

    // Use this for initialization
    protected override void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        base.Start();
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillButtonPressed.EVENT_TYPE, OnSkillButtonPressed);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillButtonReleased.EVENT_TYPE, OnSkillButtonReleased);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnLevelUp);
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
    }

    public override void StopAttributeSync()
    {
    }

    protected override void Tick(float delta)
    {
        mTickCount++;

        base.Tick(delta);
        if (TargetIndicator != null)
        {
            var isUseButtonSelected = false;
            if (mNeedUpdateSkillIndicator)
            {
                if (ShouldShowIndicator())
                {
                    TargetIndicator.SetActive(true);
                    var targetObj = ObjManager.Instance.SelectTargetForSkill(this, mCurrentSkillRecord);
                    var skillTargetType =
                        (SkillTargetType) GetSkillData_Data(mCurrentSkillRecord, eModifySkillType.TargetType);

                    if (skillTargetType == SkillTargetType.TARGET_CIRCLE)
                    {
                        if (targetObj != GameControl.Instance.TargetObj)
                        {
                            RemoveAllSkillIndicator();
                            GameControl.Instance.TargetObj = targetObj;
                            ShowIndicatorForTarget(targetObj, mCurrentSkillRecord);
                        }
                    }
                    else if (targetObj != null)
                    {
                        UpdateTargetIndicatorDirection(targetObj);
                    }
                    else
                    {
                        isUseButtonSelected = true;
                    }
                }
                else
                {
                    TargetIndicator.SetActive(false);
                }
            }
            else if (mSkillIndicatorVisible) // 手动控制方向
            {
                isUseButtonSelected = true;
            }
            if (isUseButtonSelected)
            {
                if (Input.touchCount > 1)
                {
                    var minDiff = float.MaxValue;
                    var minTouchPos = Vector2.zero;
                    for (var i = 0; i < Input.touchCount; ++i)
                    {
                        var touch = Input.touches[i];
                        var touchPos = touch.position - InputManager.Instance.SkillButtonPressedPosition;
                        var diff = touchPos.SqrMagnitude();
                        if (diff < minDiff)
                        {
                            minTouchPos = touchPos;
                            minDiff = diff;
                        }
                    }

                    if (minDiff > 22500)
                    {
                        TargetIndicator.SetActive(false);
                    }
                    else if (minDiff > 100)
                    {
                        TargetIndicator.SetActive(true);
                        UpdateTargetIndicatorDirection(minTouchPos);
                    }
                    else
                    {
                        TargetIndicator.SetActive(true);
                    }
                }
                else if (Input.GetMouseButton((int) InputManager.MOUSE_BUTTON.MOUSE_BUTTON_LEFT))
                {
                    var diff = Input.mousePosition.xy() -
                               InputManager.Instance.SkillButtonPressedPosition;

                    if (diff.sqrMagnitude > 22500)
                    {
                        TargetIndicator.SetActive(false);
                    }
                    else if (diff.sqrMagnitude > 100)
                    {
                        TargetIndicator.SetActive(true);
                        UpdateTargetIndicatorDirection(diff);
                    }
                    else
                    {
                        TargetIndicator.SetActive(true);
                    }
                }
            }
        }

        SendDirToServer();


        if ((GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN && 0 == mTickCount%10) || 0 == mTickCount%100)
        {
            var e = new Postion_Change_Event(Position);
            EventDispatcher.Instance.DispatchEvent(e);
        }
       
    }

    private void UpdateTargetIndicatorDirection(ObjCharacter targetObj)
    {
        if (TargetIndicator != null)
        {
            var transform = TargetIndicator.transform;
            if (targetObj == null)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                return;
            }

            var dir = (targetObj.Position.xz() - Position.xz()).normalized;
            var angle = dir.GetAngle(Direction.xz().normalized);
            transform.localRotation = Quaternion.Euler(0, angle*Mathf.Rad2Deg, 0);
        }
    }

    private void UpdateTargetIndicatorDirection(Vector2 dir)
    {
        var angle = -Mathf.Atan2(dir.y, dir.x) + Mathf.PI*0.25f;
        TargetIndicator.transform.rotation = Quaternion.Euler(0, angle*Mathf.Rad2Deg, 0);
    }

    #region 控制器方法

    //朝向移动
    public void MoveDirection(Vector2 dir)
    {
        if (IsAttakNoMove())
        {
            return;
        }

        dir = dir.normalized;
        if (dir.magnitude > 0)
        {
            var distance = GetMoveSpeed();
            var vecOffset = new Vector3(dir.x, 0, dir.y)*distance;
            var targetPos = Position + vecOffset;

            // 如果移动到边界，最远也就到边界，不能越过边界
            NavMeshHit hit;
            if (NavMesh.Raycast(Position, targetPos, out hit, -1))
            {
                // 如果距离过近，说明已经贴边，按当前方向夹角的朝向进行转向
                if ((Position - hit.position).sqrMagnitude < 0.25)
                {
                    var a = -hit.normal;
                    var b = dir;
                    var dot = a.x*-b.y + a.z*b.x;
                    if (dot > 0)
                    {
                        dir = new Vector2(a.z, -a.x);
                    }
                    else
                    {
                        dir = new Vector2(-a.z, a.x);
                    }

                    vecOffset = new Vector3(dir.x, 0, dir.y)*distance;
                    targetPos = Position + vecOffset;

                    if (NavMesh.Raycast(Position, targetPos, out hit, -1))
                    {
                        targetPos = hit.position;
                    }
                }
                else
                {
                    targetPos = hit.position;
                }
            }

            MoveTo(targetPos);
            GameLogic.Instance.Scene.DisableActiveMovingCircle();
        }
        else
        {
            StopMove();
        }
    }

    private bool CheckPosition()
    {
        float x = (float)GameLogic.Instance.Scene.TableScene.Safe_x ;
        float z = (float)GameLogic.Instance.Scene.TableScene.Safe_z;
        float y = GameLogic.GetTerrainHeight(x,z);
        Vector3 pos = new Vector3(x, y, z);
        var path = new NavMeshPath();
        NavMesh.CalculatePath(Position, pos, -1, path);
        var result = path.corners.Length > 0;
        if (!result)
        {
            Logger.Error("场景安全坐标不安全，重新配置场景表数据 {0}", SceneManager.Instance.CurrentSceneTypeId);
        }

        return result;
    }
    //移动到目标点
    public override bool MoveTo(Vector3 point, float offset = 0.05f, bool isSendFastReach = false)
    {
        MoveOverCallBack = null;
        if (IsAttakNoMove())
        {
            return false;
        }

        if (!CanMove())
        {
            if (CanTurnFace())
            {
                FaceTo(point);
            }
            return false;
        }

        var path = new NavMeshPath();
        point.y = GameLogic.GetTerrainHeight(point);
        NavMesh.CalculatePath(Position, point, -1, path);
        if (path.corners.Length <= 0)
        {
            if(mLastSafePos != Vector3.zero&&CheckPosition() == false)
            {
                Position = mLastSafePos;
                StopMove();
            }
                
            return false;
        }

        mTargetPos.Clear();

        var vec = new List<Vector3>();
        var pathcornersLength0 = path.corners.Length;
        var p = Position;
        RaycastHit hit;
        float length = 0;
        for (var j = 1; j < pathcornersLength0; j++)
        {
            var d = (path.corners[j] - p);
            var l = d.magnitude;
            length += l;
            if (Physics.Raycast(p, d/l, out hit, l, mColliderLayer))
            {
                vec.Add(hit.point);
                break;
            }
            vec.Add(path.corners[j]);
            p = path.corners[j];
        }

        if (PathBackward(offset, vec))
        {
            return false;
        }
        if (vec.Count == 0)
        {
            return false;
        }

        mTargetPos.AddRange(vec);

        SendMoveToServer(mTargetPos.Count > 0 || length > 5);

        DoMove();

        if (isSendFastReach)
        {
            var reachPos = TargetPos[TargetPos.Count - 1];
            var curPos = Position;
            var distance = Vector3.Distance(reachPos, curPos);

            if (fastReachSceneID >= 0)
            {
                var tbScene = Table.GetScene(fastReachSceneID);
                if (tbScene != null && (tbScene.Type == (int)eSceneType.Normal || tbScene.Type == (int)eSceneType.City))
                {
                    if (fastReachSceneID != GameLogic.Instance.Scene.SceneTypeId)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowFastReachEvent(true));
                    }
                    else
                    {
                        var tbClientConfig = Table.GetClientConfig(295);
                        if (tbClientConfig != null && tbClientConfig.Value != null && distance > tbClientConfig.Value.ToInt())
                        {
                            EventDispatcher.Instance.DispatchEvent(new ShowFastReachEvent(true));
                        }
                        else
                        {
                            EventDispatcher.Instance.DispatchEvent(new ShowFastReachEvent(false));
                        }
                    }
                }
            } 
        }
        else
        {
            EventDispatcher.Instance.DispatchEvent(new ShowFastReachEvent(false));
        }
        return true;
    }

    //移动到目标点
    public bool MoveToWithCallback(Vector3 point, float offset = 0.05f, Action action = null)
    {
        var ret = MoveTo(point, offset);
        if (ret)
        {
            MoveOverCallBack = action;
        }
        return ret;
    }

    public List<Vector3> CalculatePath(Vector3 point, float offset = 0.05f)
    {
        var path = new NavMeshPath();
        point.y = GameLogic.GetTerrainHeight(point);
        NavMesh.CalculatePath(Position, point, -1, path);

        var vec = new List<Vector3>();
        var pathcornersLength1 = path.corners.Length;
        for (var j = 1; j < pathcornersLength1; j++)
        {
            vec.Add(path.corners[j]);
        }
        if (PathBackward(offset, vec))
        {
            vec.Clear();
        }

        return vec;
    }

    public override void StopMove()
    {
        MoveOverCallBack = null;
        if (IsMoving())
        {
            SendStopMoveToServer();
        }

        StopMoveWithoutNotifyServer();
    }

    public void StopMoveWithoutNotifyServer()
    {
        mTargetPos.Clear();

        if (IsMoving())
        {
            DoIdle();
        }

        if (GameLogic.Instance && GameLogic.Instance.Scene)
        {
            GameLogic.Instance.Scene.DisableActiveMovingCircle();
        }
    }

    #endregion

    #region Message

    public void SendMoveToServer(bool force = false)
    {
        if (mTargetPos.Count <= 0)
        {
            return;
        }

        var diff = mTargetPos[mTargetPos.Count - 1] - mLastSyncPos;
        diff.y = 0;
        if (diff.sqrMagnitude <= 0.1)
        {
            return;
        }

        if (mSendStopFlag ||
            force ||
            (Time.time - mLastSyncPosTime >= GameSetting.Instance.MoveSyncInterval) ||
            Vector2.SqrMagnitude(mTargetPos[mTargetPos.Count - 1] - mLastSyncPos) >=
            GameSetting.Instance.MoveSyncShreholdSqr)
        {
            StartCoroutine(SendMoveToServerCoroutine(mTargetPos));
            mLastSyncPosTime = Time.time;
            mLastSyncPos = mTargetPos[mTargetPos.Count - 1];
            mSendStopFlag = false;
        }
    }

// 	public void SendMoveToServer2(bool force = false)
// 	{
// 		if (mTargetPos.Count <= 0)
// 			return;
// 
// 		if (force)
// 		{
// 			this.StartCoroutine(SendMoveToServerCoroutine( mTargetPos));
// 			mLastSyncPosTime = Time.time;
// 			mLastSyncPos = mTargetPos[mTargetPos.Count - 1].xz();
// 			mSendStopFlag = false;
// 			VList.List.Clear();
// 		}
//         else if (mSendStopFlag ||
//             (Time.time - mLastSyncPosTime >= GameSetting.Instance.MoveSyncInterval) ||
//             Vector2.SqrMagnitude(mTargetPos[mTargetPos.Count - 1].xz() - mLastSyncPos) >= GameSetting.Instance.MoveSyncShrehold * GameSetting.Instance.MoveSyncShrehold)
// 		{
//             this.StartCoroutine(SendMoveToServerCoroutine2( mTargetPos));
//             mLastSyncPosTime = Time.time;
//             mLastSyncPos = mTargetPos[mTargetPos.Count - 1].xz();
//             mSendStopFlag = false;
//             VList.List.Clear();
// 		}
// 	}
// 
//     private static float NexSyncMoveTime;
//     private static Vec2Array VList = new Vec2Array();
// 
//     private IEnumerator SendMoveToServerCoroutine2( List<Vector3> targetList)
//     {
//         if (Time.time >= NexSyncMoveTime)
//         {
//             var msg = NetManager.Instance.MoveTo(VList, 0.1f);
//             NexSyncMoveTime = Time.time + GameSetting.Instance.MaxMoveSyncInterval;
//             yield return msg.SendAndWaitUntilDone(); 
//         }
// 
//     }


    private readonly Stopwatch watch = new Stopwatch();

    public ObjMyPlayer()
    {
        RemindNoMoveTime = 0;
    }

    private IEnumerator SendMoveToServerCoroutine(List<Vector3> targetList)
    {
        var original = ObjManager.Instance.MyPlayer.Position;
        var v = new Vec2Array();
        {
            v.List.Add(new Vector2Int32
            {
                x = GameUtils.MultiplyPrecision(ObjManager.Instance.MyPlayer.Position.x),
                y = GameUtils.MultiplyPrecision(ObjManager.Instance.MyPlayer.Position.z)
            });

            var __list8 = targetList;
            var __listCount8 = __list8.Count;
            for (var __i8 = 0; __i8 < __listCount8; ++__i8)
            {
                var p = __list8[__i8];
                {
                    var pp = new Vector2Int32();
                    pp.x = GameUtils.MultiplyPrecision(p.x);
                    pp.y = GameUtils.MultiplyPrecision(p.z);
                    v.List.Add(pp);
                }
            }
        }

        var game = Game.Instance;
        var msg = NetManager.Instance.MoveTo(v, 0.1f, game.ServerTime.ToServerBinary());
        watch.Reset();
        watch.Start();
        yield return msg.SendAndWaitUntilDone();
        watch.Stop();

        if (msg.State == MessageState.Reply)
        {
            game.ServerTimeDiff = (DateTime.Now - Extension.FromServerBinary(msg.Response.Time)) -
                                  TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds/2);

            switch (msg.ErrorCode)
            {
                case (int) ErrorCodes.Error_CannotMove:
                {
                    // wait a while and confirm again.
                    yield return new WaitForSeconds(0.1f);

                    var msg1 = NetManager.Instance.MoveTo(v, 0.1f, game.ServerTime.ToServerBinary());
                    watch.Reset();
                    watch.Start();
                    yield return msg1.SendAndWaitUntilDone();
                    watch.Stop();

                    if (msg1.State == MessageState.Reply)
                    {
                        game.ServerTimeDiff = (DateTime.Now - Extension.FromServerBinary(msg.Response.Time)) -
                                              TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds/2);

                        if (msg1.ErrorCode != (int) ErrorCodes.OK)
                        {
                            // still can not move， reset position as server side.
                            ResetPosAndStopMove(msg1.Response, watch.ElapsedMilliseconds/1000.0f);
                        }
                    }
                }
                    break;
                case (int) ErrorCodes.Error_CharacterNoScene:
                case (int) ErrorCodes.Error_CharacterDie:
                case (int) ErrorCodes.Error_PositionUnsync:
                case (int) ErrorCodes.Error_PathInvalid:
                    ResetPosAndStopMove(msg.Response, watch.ElapsedMilliseconds/1000.0f);
                    break;
                case (int) ErrorCodes.OK:
                    mLastSafePos = original;
                    CheckAndResetPosIfNeeded(msg.Response, watch.ElapsedMilliseconds/1000.0f);
                    EventDispatcher.Instance.DispatchEvent(new UiEventChangeOutLineTime());
                    break;
                default:
                    StopMoveWithoutNotifyServer();
                    break;
            }
        }
        else
        {
            StopMoveWithoutNotifyServer();
        }
    }

    private void CheckAndResetPosIfNeeded(PositionData p, float time)
    {
        var pos = GameLogic.GetTerrainPosition(GameUtils.DividePrecision(p.Pos.x), GameUtils.DividePrecision(p.Pos.y));

        var diff = (Position.xz() - pos.xz()).magnitude - time*ObjManager.Instance.MyPlayer.GetMoveSpeed();
        if (diff > GameSetting.Instance.MainPlayerSkillPosErrorDistance)
        {
            Position = pos;
            
        }
    }

    private void ResetPosAndStopMove(PositionData p, float time)
    {
        CheckAndResetPosIfNeeded(p, time);
        StopMoveWithoutNotifyServer();
    }

    public void SendStopMoveToServer()
    {
        StartCoroutine(SendStopMoveToServerCoroutine(Position, TargetDirection));
        mSendStopFlag = true;
        mLastSyncPosTime = Time.time;
        mLastSyncPos = Position;
    }

    private IEnumerator SendStopMoveToServerCoroutine(Vector3 p, Vector3 dir)
    {
        var Pos = new PositionData
        {
            Pos = new Vector2Int32
            {
                x = GameUtils.MultiplyPrecision(Position.x),
                y = GameUtils.MultiplyPrecision(Position.z)
            },
            Dir = new Vector2Int32
            {
                x = GameUtils.MultiplyPrecision(TargetDirection.x),
                y = GameUtils.MultiplyPrecision(TargetDirection.z)
            }
        };


        var msg = NetManager.Instance.StopMove(Pos);
        yield return msg.SendAndWaitUntilDone();

        Logger.Debug("SendStopMoveToServerCoroutine");
    }

    public bool SendDirToServer(bool force = false)
    {
        if (!force)
        {
            if (Time.time - mLastSyncDirTime < GameSetting.Instance.DirSyncInterval)
            {
                return false;
            }

            if (Math.Abs(mLastDir.x - TargetDirection.x) < GameSetting.Instance.DirSyncDelta &&
                Math.Abs(mLastDir.y - TargetDirection.z) < GameSetting.Instance.DirSyncDelta)
            {
                return false;
            }

            if (CanMove())
            {
                return false;
            }

            if (!CanTurnFace())
            {
                return false;
            }
        }

        StartCoroutine(SendDirToServerCoroutine(TargetDirection));
        mLastDir = TargetDirection.xz();
        mLastSyncDirTime = Time.time;

        return true;
    }

    private IEnumerator SendDirToServerCoroutine(Vector3 dir)
    {
        var msg = NetManager.Instance.DirectTo(GameUtils.MultiplyPrecision(dir.x), GameUtils.MultiplyPrecision(dir.z));
        yield return msg.SendAndWaitUntilDone();

        if (msg.State != MessageState.Reply)
        {
            Logger.Warn("SendDir2Server: msg.State != MessageState.Reply");
        }


        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            Logger.Warn("SendDir2Server:ErrorCode[{0}]", msg.ErrorCode);
        }
    }

    public bool IsAttakNoMove()
    {
        if (GetCurrentStateName() == OBJ.CHARACTER_STATE.ATTACK)
        {
            return RemindNoMoveTime > Time.time;
        }
        return false;
    }

    public override void UseSkill(SkillRecord skillData, List<ulong> targetId)
    {
        base.UseSkill(skillData, targetId);
        RemindNoMoveTime = Time.time + skillData.NoMove/1000.0f;
        mLastSyncPosTime = Time.time;
        mLastSyncPos = Position;
    }

    public override void Reset()
    {
        base.Reset();
        // EventDispatcher.Instance.DispatchEvent(new UIEvent_ClearBuffList());
    }

    public override void RemoveAllBuff()
    {
        base.RemoveAllBuff();
        // EventDispatcher.Instance.DispatchEvent(new UIEvent_ClearBuffList());
    }

    #endregion
}