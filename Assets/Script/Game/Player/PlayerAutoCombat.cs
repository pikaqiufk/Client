#region using

using System;
using System.Collections.Generic;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

public class PlayerAutoCombat : MonoBehaviour
{
    private enum AUTO_STAT
    {
        AUTO_STAT_NORMAL                            = 0,
        AUTO_STAT_SKILL_CD_NOMOVE                   = 1,
        AUTO_STAT_MOVE_TARGET                       = 2,
        AUTO_STAT_COMMIT_MMISSION                   = 3,
        AUTO_STAT_MOVE_DROPITEM                     = 4,
        AUTO_STAT_MOVE_POSITION                     = 5,
        AUTO_STAT_SKILL_CD                          = 6,
        AUTO_STAT_WAIT_FUBEN                        = 7,
        AUTO_STAT_PICK_DROP                         = 8,

    }
    public ObjMyPlayer MainPlayer;
    private Vector3 mBeginPostion;
    private DateTime mCommondCdTime;
    private int mGotoMissionId;
    private bool mIsCheckFubenLogicId;
    private bool mIsDropFirst;
    private bool mIsFuben;

    private AUTO_STAT _loopstate ;
    private AUTO_STAT mLoopState 
    {
        get{return _loopstate;}
        set
        {
            _loopstate = value ;
        }
    }
    private float mLastSkillTime = Time.time;
    private float mNormalSkillTime = Time.time;
    private bool mMoveStopped = false;
    private List<int> mMissionList = new List<int>();
    private bool mMonsterPoint;
    private DateTime mNextUpdataTime;
    private Vector3 mTargetPostion;
    private ObjCharacter mYindaoCharacter;
    private int SceneId { get; set; }

    private AUTO_STAT mLastStatus { get; set; }
    private float mPickTimer { get; set; }
    private float mAutoPickTimer { get; set; }   //用于自动挂机拾取逻辑
    private List<Vector3> mDropPos = new List<Vector3>();
    public void AddDropPos(ObjDropItem item)
    {
        //Debug.LogError("=============================================添加物品");
       var control = UIManager.Instance.GetController(UIConfig.SettingUI);
       if ((bool)control.CallFromOtherClass("CanPiackUpItem", new object[] {item.GetDataId()}))
       {
           var dis = GameUtils.Vector3PlaneDistance(MainPlayer.Position, item.Position);
           if (dis < 10.0f)
           {
               mDropPos.Add(item.Position);
               //Debug.LogError("=============================================成功添加");
           }
       }
    }
    private void AutoFight()
    {
        var skillId = GetCurrentSkill();
        var tbSkill = Table.GetSkill(skillId);
        if (tbSkill == null)
        {
            CancelLoop();
            return;
        }

        var skillTargetType = (SkillTargetType) ObjMyPlayer.GetSkillData_Data(tbSkill, eModifySkillType.TargetType);


        if (mIsCheckFubenLogicId)
        {
            if (CheckDungeonLogic())
            {
                return;
            }
        }

        if (tbSkill.CampType != 0)
        {
            if (skillTargetType == SkillTargetType.SELF
                || skillTargetType == SkillTargetType.CIRCLE
                || skillTargetType == SkillTargetType.SECTOR)
            {
                UseSkill(skillId);
                return;
            }
        }

        var targetObj = GameControl.Instance.TargetObj;
        if (targetObj == null || targetObj.Dead)
        {
            targetObj = null;
            //是否离挂机点太远
            if (IsMoveBack())
            {
                GameControl.Instance.TargetObj = null;
                MoveStartPosition();
                return;
            }

            //找下一个目标,包含掉落物
            var ret = SerchNearObj(true);
            if (ret != null)
            {
                if (ret.IsCharacter())
                {
                    //找到一个敌人
                    targetObj = ret as ObjCharacter;
                    if (targetObj != null)
                    {
                        if (IsFarFromBegin(targetObj.Position))
                        {
                            //最近的目标太远了，不去移动，而去返回
                            GameControl.Instance.TargetObj = null;
                            MoveStartPosition();
                            return;
                        }
                        GameControl.Instance.TargetObj = targetObj;
                    }
                }
                else
                {
                    if (ret.GetObjType() == OBJ.TYPE.DROPITEM)
                    {
                        //找到一个掉落
                        var drop = ret as ObjDropItem;
                        if (drop != null)
                        {
                            mTargetPostion = drop.Position;
                            drop.HasAutoFightMove = true;
                            MoveDrop();
                        }
                        else
                        {
                            //异常处理
                            CancelLoop();
                        }
                        return;
                    }
                }
            }
        }
        if (targetObj == null)
        {
            if (mIsFuben)
            {
                if (CheckDungeonLogic())
                {
                    return;
                }                
            }

            if (mMonsterPoint == false)
            {
                mMonsterPoint = true;
                MoveMonsterArea();
                return;
            }

            var logciId = Scene.LogicId;
            if (logciId == -1)
            {
                //没有目标 回到原点
                MoveStartPosition();
            }
            return;
        }
        //检查距离 并放技能
        CheckNearObjDistance();
    }

    private void BeginAutoFight()
    {
        Logger.Info("AutoCombat Start!");
        //mIsCheckFubenLogicId = true;
        //开始挂机时如果目标不为空，判断目标是否合适
        //GameControl.Instance.TargetObj = null;
        mIsDropFirst = false;
        var objTarget = GameControl.Instance.TargetObj;
        if (objTarget != null)
        {
            if (objTarget.Dead || !MainPlayer.IsMyEnemy(objTarget) || IsFarFromBegin(objTarget.Position))
            {
                GameControl.Instance.TargetObj = null;
            }
        }
        mLoopState = AUTO_STAT.AUTO_STAT_NORMAL;
        mLastStatus = AUTO_STAT.AUTO_STAT_NORMAL;
        SceneId = GameLogic.Instance.Scene.SceneTypeId;
        var tbScene = Table.GetScene(SceneId);
        if (tbScene != null && tbScene.Type == (int)eSceneType.Fuben)
        {
            mIsFuben = true;
        }
        else
        {
            mIsFuben = false;
        }
        mBeginPostion = MainPlayer.Position;
        mMonsterPoint = false;
        //如果需要找怪点，先去怪点
        AutoFight();
        mAutoPickTimer = Time.realtimeSinceStartup;        
    }

    //计算出下一个技能，可能为-1
    private int CalculateSkillId()
    {
        var skillStates = PlayerDataManager.Instance.PlayerDataModel.SkillData.SkillStates;
        var skills = PlayerDataManager.Instance.PlayerDataModel.SkillData.EquipSkillsPriority;
        var norskillId = PlayerDataManager.Instance.GetNormalSkill(true);
        var skillId = -1;
        SkillStateData skillState = null;
        SkillRecord tbSkill = null;
        var cd = 0;


        var skillsCount0 = skills.Count;
        for (var i = skillsCount0 - 1; i >= 0; i--)
        {
            skillId = skills[i];
            if (skillId == -1)
            {
                continue;
            }

            if (skillId == norskillId)
            {
                continue;
            }
            if (ErrorCodes.OK == GameControl.Instance.CheckSkill(skillId, true))
            {
                return skillId;
            }
        }


        skillId = norskillId;
        if (ErrorCodes.OK == GameControl.Instance.CheckSkill(skillId, true))
        {
            return skillId;
        }

        return -1;
    }

    public void CancelLoop()
    {
        mLoopState = AUTO_STAT.AUTO_STAT_SKILL_CD_NOMOVE;
        mLastStatus = AUTO_STAT.AUTO_STAT_NORMAL;
        mNextUpdataTime = Game.Instance.ServerTime.AddSeconds(2);
    }

    public void ChangeFubenLogicId()
    {
        if (enabled == false)
        {
            return;
        }
        mIsCheckFubenLogicId = true;
        if (mLoopState == AUTO_STAT.AUTO_STAT_SKILL_CD_NOMOVE || mLoopState == AUTO_STAT.AUTO_STAT_SKILL_CD)
        {
        }
        else
        {
            CheckDungeonLogic();
        }
    }

    private bool CheckDungeonLogic()
    {
        mIsCheckFubenLogicId = false;
        if (PlayerDataManager.Instance.PlayerDataModel.DungeonState >= (int) eDungeonState.WillClose)
        {
            SetNextLoopTime(Game.Instance.ServerTime.AddMilliseconds(100));
            return false;
        }
        var logciId = Scene.LogicId;
        if (logciId == -1)
        {
            SetNextLoopTime(Game.Instance.ServerTime.AddMilliseconds(100));
            return false;
        }
        var tbDungeonLogic = Table.GetFubenLogic(logciId);
        if (tbDungeonLogic == null)
        {
            CancelLoop();
            return false;
        }
        mTargetPostion = new Vector3(tbDungeonLogic.Hang1PosX, 0, tbDungeonLogic.Hang1PosZ);

        if (tbDungeonLogic.DelayHang > 0)
        {
            mLoopState = AUTO_STAT.AUTO_STAT_WAIT_FUBEN;
            mNextUpdataTime = Game.Instance.ServerTime.AddMilliseconds(tbDungeonLogic.DelayHang);
        }
        else
        {
            MovePostion();
            MainPlayer.MoveTo(mTargetPostion, 0.1f);
        }
        GameControl.Instance.TargetObj = null;
        return true;
    }
    private bool IsAutoMission()
    {
        return mMissionList.Count > 0;
    }
    //如果任务都完成了 就回去交
    private bool CheckMission()
    {
        if (mMissionList.Count <= 0)
        {
            return false;
        }
        {
            var __list1 = mMissionList;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var id = __list1[__i1];
                {
                    var state = MissionManager.Instance.GetMissionState(id);
                    if (eMissionState.Finished != state)
                    {
                        return false;
                    }
                }
            }
        }
        mGotoMissionId = mMissionList[0];
        {
            var __list2 = mMissionList;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var id = __list2[__i2];
                {
                    var table = Table.GetMissionBase(id);
                    if (eMissionMainType.MainStoryLine == (eMissionMainType) table.ViewType)
                    {
                        mGotoMissionId = id;
                        break;
                    }
                }
            }
        }

        
        return true;
    }

    private bool CheckNearObjDistance()
    {
        var targetObj = GameControl.Instance.TargetObj;
        PlayerDataManager.Instance.SetSelectTargetData(targetObj, 3);
        mTargetPostion = new Vector3(targetObj.Position.x, 0, targetObj.Position.z);
        var distance = GameUtils.Vector3PlaneDistance(MainPlayer.Position, mTargetPostion);
        if (GetSkillCastDisance() >= distance)
        {
//距离足够
            if (mCommondCdTime > Game.Instance.ServerTime)
            {
                WaiteCommonCd();
                return false;
            }
            UseSkill();
            return true;
        }
        //距离不够
        var dif = GetSkillMoveDisance();
        var ret = MainPlayer.MoveTo(mTargetPostion, dif);
        if (ret == false)
        {
            if (CheckDungeonLogic() == false)
            {
                return false;
            }
        }
        else
        {
            StartMove();
        }
        return true;
    }

    private bool CommitMission()
    {
        if (OBJ.CHARACTER_STATE.ATTACK == ObjManager.Instance.MyPlayer.GetCurrentStateName())
        {
            return false ;
        }

        if (!ObjManager.Instance.MyPlayer.CanMove())
        {
            return false ;
        }
        SerchNearDrop();
        mLastStatus = mLoopState;
        mPickTimer = Time.realtimeSinceStartup;
        mLoopState = AUTO_STAT.AUTO_STAT_PICK_DROP;
        mTargetPostion = Vector3.zero;
        return true;
    }
    private void AutoPickDrop()
    {

    }

    private void MissionComplete()
    {
        mLastStatus = AUTO_STAT.AUTO_STAT_NORMAL;
        ObjManager.Instance.MyPlayer.LeaveAutoCombat();
        MissionManager.Instance.NextMissionAction(mGotoMissionId, -1, 0.5f);
        Logger.Info("AutoCombat Stop! CommitMission={0}", mGotoMissionId);
    }

    //得到挂机距离类型
    private int GetAutoCombatRange()
    {
        var autoCombatData =
            UIManager.Instance.GetController(UIConfig.SettingUI).GetDataModel("AutoCombat") as AutoCombatData;

        if (autoCombatData == null)
        {
            return 0;
        }
        if (autoCombatData.Ranges[0] || mMissionList.Count > 0)
        {
            return 0;
        }
        if (autoCombatData.Ranges[1])
        {
            return 1;
        }
        if (autoCombatData.Ranges[2])
        {
            return 2;
        }
        return 0;
    }

    //得到一个技能，都不符合时返回普通技能
    private int GetCurrentSkill()
    {
        var skillId = CalculateSkillId();
        if (skillId == -1)
        {
            skillId = PlayerDataManager.Instance.GetNormalSkill(true);
        }
        return skillId;
    }

    public List<int> GetMissionList()
    {
        return mMissionList;
    }

    //技能的施法距离是配置-0.5
    private float GetSkillCastDisance()
    {
        var record = Table.GetSkill(GetCurrentSkill());
        var dif = GameControl.GetSkillReleaseDistance(record) - 0.5f;
        if (dif <= 0)
        {
            dif = 0.5f;
        }
        return dif;
    }

    //技能的移动距离是配置-1
    private float GetSkillMoveDisance()
    {
        var record = Table.GetSkill(GetCurrentSkill());
        var dif = GameControl.GetSkillReleaseDistance(record) - 1.0f;
        if (dif <= 0)
        {
            dif = 0.5f;
        }
        return dif;
    }

    private bool IsFarFromBegin(Vector3 pos)
    {
        if (mIsFuben)
        {
//副本中不再检查是离开始挂机点的距离
            return false;
        }
        var type = GetAutoCombatRange();
        if (type == 0 || mMissionList.Count > 0)
        {
//原地挂机或者有任务
            if (GameUtils.Vector3PlaneDistance(pos, mBeginPostion) > GameUtils.AutoFightShortDistance)
            {
                return true;
            }
        }
        else if (type == 1)
        {
            if (GameUtils.Vector3PlaneDistance(pos, mBeginPostion) > GameUtils.AutoFightLongDistance)
            {
                return true;
            }
        }
        else if (type == 2)
        {
        }
        return false;
    }

    private bool IsMoveBack()
    {
        if (IsFarFromBegin(MainPlayer.Position))
        {
            MoveStartPosition();
            return true;
        }
        return false;
    }

    private void LateUpdate()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (0 == Time.frameCount%2)
        {
            if (mLoopState != AUTO_STAT.AUTO_STAT_COMMIT_MMISSION && mLoopState != AUTO_STAT.AUTO_STAT_PICK_DROP && CheckMission())
            {
                MissionComple();
                CommitMission();
            }
            else
            {
                if(IsAutoMission() == false && mLoopState != AUTO_STAT.AUTO_STAT_PICK_DROP)
                {
                    if (Time.realtimeSinceStartup - mAutoPickTimer  > 120.0f)
                    {
                        CommitMission();
                    }
                                            
                }

                switch (mLoopState)
                {
                    case AUTO_STAT.AUTO_STAT_SKILL_CD_NOMOVE: //放技能的CD,no move
                    {
                        if (Game.Instance.ServerTime > mNextUpdataTime)
                        {
                            AutoFight();
                        }
                        else if (mYindaoCharacter != null)
                        {
                            if (mYindaoCharacter.Dead)
                            {
                                var ret = SerchNearObj();
                                ObjCharacter targetObj = null;
                                if (ret != null && ret.IsCharacter())
                                {
                                    targetObj = ret as ObjCharacter;
                                }
                                if (targetObj == null)
                                {
                                    mYindaoCharacter = null;
                                }
                                else
                                {
                                    GameControl.Instance.TargetObj = targetObj;
                                    MainPlayer.FaceTo(targetObj.Position);
                                    MainPlayer.SendDirToServer(true);
                                    mYindaoCharacter = targetObj;
                                }
                            }
                        }
                    }
                        break;
                    case AUTO_STAT.AUTO_STAT_MOVE_TARGET: //向目标移动
                    {
                        var moveLength = GetSkillMoveDisance();
                        var skillLength = GetSkillCastDisance();

                        if (GameControl.Instance.TargetObj)
                        {
                            mTargetPostion = GameControl.Instance.TargetObj.Position;
                        }
                        var dis = GameUtils.Vector3PlaneDistance(MainPlayer.Position, mTargetPostion);
                        if (skillLength <= 0.5f || dis < skillLength)
                        {
                            AutoFight();
                        }
                        else
                        {
                            var ret = MainPlayer.MoveTo(mTargetPostion, moveLength - 0.1f, false);
                            if (ret == false)
                            {
                                GameControl.Instance.TargetObj = null;
                                AutoFight();
                            }
                        }
                    }
                        break;
                    case AUTO_STAT.AUTO_STAT_COMMIT_MMISSION:
                    {
                        CommitMission();
                    }
                        break;
                    case AUTO_STAT.AUTO_STAT_MOVE_DROPITEM:
                    {
//向掉落物移动
                        var dif = GameUtils.AutoPickUpDistance - 0.5f;
                        if (dif < 0.0f)
                        {
                            dif = 0.5f;
                        }
                        var dis = GameUtils.Vector3PlaneDistance(MainPlayer.Position, mTargetPostion);
                        if (dis < dif)
                        {
                            AutoFight();
                        }
                        else
                        {
                            var ret = MainPlayer.MoveTo(mTargetPostion, dif - 0.3f);
                            if (ret == false)
                            {
                                GameControl.Instance.TargetObj = null;
                                AutoFight();
                            }
                        }
                    }
                        break;
                    case AUTO_STAT.AUTO_STAT_MOVE_POSITION:
                    {
//向指定目的地移动
                        var dif = 0.05;
                        var dis = GameUtils.Vector3PlaneDistance(MainPlayer.Position, mTargetPostion);
                        if (dis < dif)
                        {
                            AutoFight();
                        }
                        else
                        {
                            var ret = MainPlayer.MoveTo(mTargetPostion, 0.05f);
                            if (ret == false)
                            {
                                GameControl.Instance.TargetObj = null;
                                AutoFight();
                            }
                        }
                    }
                        break;
                    case AUTO_STAT.AUTO_STAT_SKILL_CD: //放技能的CD,Common Cd
                    {
                        if (Game.Instance.ServerTime > mNextUpdataTime)
                        {
                            AutoFight();
                        }
                    }
                        break;
                    case AUTO_STAT.AUTO_STAT_WAIT_FUBEN:
                    {
//向副本点等待
                        if (Game.Instance.ServerTime > mNextUpdataTime)
                        {
                            MovePostion();
                            MainPlayer.MoveTo(mTargetPostion, 0.1f);
                        }
                    }
                        break;
                    case AUTO_STAT.AUTO_STAT_PICK_DROP:
                        {//阶段性战斗完成后拾取物品,完成后继续之前的状态
                            PickUpLogic();
                        }
                        break;
                    default:
                    {
                        CancelLoop();
                    }
                        break;
                
                }
                    
            }
        }
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void MissionComple()
    {
        mLoopState = AUTO_STAT.AUTO_STAT_COMMIT_MMISSION;
    }

    private void MoveDrop()
    {
        mLoopState = AUTO_STAT.AUTO_STAT_MOVE_DROPITEM;
    }

    private void MoveMonsterArea()
    {
        MapTransferRecord mapTransferRecord = null;
        var distance = -1.0f;

        Table.ForeachMapTransfer(record =>
        {
            if (record.SceneID != SceneId
                || record.Type != 2)
            {
                return true;
            }
            var dis = Vector2.Distance(
                new Vector2(MainPlayer.Position.x, MainPlayer.Position.z),
                new Vector2(record.PosX, record.PosZ));
            if (!(distance < 0.0f) && !(distance > dis))
            {
                return true;
            }
            mapTransferRecord = record;
            distance = dis;
            return true;
        });
        if (mapTransferRecord != null)
        {
            var pos = new Vector3(mapTransferRecord.PosX, 0, mapTransferRecord.PosZ);
            mTargetPostion = pos;
            StartMove();
        }
        else
        {
            CancelLoop();
            //Logger.Error("MoveMonsterArea error not find");
        }
    }

    private void MovePostion()
    {
//向指定目的地移动
        mLoopState = AUTO_STAT.AUTO_STAT_MOVE_POSITION;
    }

    private void MoveStartPosition()
    {
        mTargetPostion = mBeginPostion;

        var dis = GameUtils.Vector3PlaneDistance(MainPlayer.Position, mTargetPostion);
        if (dis < 0.5)
        {
//这时已经在初始点了
            CancelLoop();
        }
        else
        {
            MainPlayer.MoveTo(mTargetPostion, 0.3f);
            MovePostion();
        }
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        EventDispatcher.Instance.RemoveEventListener(SkillReleaseNetBack.EVENT_TYPE, OnSkillReleaseNetBack);
        EventDispatcher.Instance.RemoveEventListener(DungeonFightOver.EVENT_TYPE, OnDungeonFightOver);
        if (MainPlayer == null)
        {
            return;
        }
        if (ObjManager.Instance.MyPlayer != null)
        {
            ObjManager.Instance.MyPlayer.StopMove();
        }
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void OnDungeonFightOver(IEvent ievent)
    {
        mIsDropFirst = true;
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (MainPlayer == null)
        {
            return;
        }
        BeginAutoFight();
        EventDispatcher.Instance.AddEventListener(SkillReleaseNetBack.EVENT_TYPE, OnSkillReleaseNetBack);
        EventDispatcher.Instance.AddEventListener(DungeonFightOver.EVENT_TYPE, OnDungeonFightOver);
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void OnSkillReleaseNetBack(IEvent ievent)
    {
        var e = ievent as SkillReleaseNetBack;
        var skillId = e.SkillId;
        var tbSkill = Table.GetSkill(skillId);
        if (e.IsOk)
        {
            mLastSkillTime = Time.time;
            var skillCd = tbSkill.NoMove;
            mYindaoCharacter = null;
            if (tbSkill.CastType == 2)
            {
                if (tbSkill.CastParam[0] > skillCd)
                {
                    skillCd = tbSkill.CastParam[0];
                    //只有极光才有的效果
                    var targetType =
                        (SkillTargetType) ObjMyPlayer.GetSkillData_Data(tbSkill, eModifySkillType.TargetType);
                    //if (tbSkill.TargetType == 4)
                    if (targetType == SkillTargetType.RECT)
                    {
                        if (GameControl.Instance != null && GameControl.Instance.TargetObj != null)
                        {
                            mYindaoCharacter = GameControl.Instance.TargetObj;
                        }
                    }
                }
            }
            mCommondCdTime = Game.Instance.ServerTime.AddMilliseconds(tbSkill.CommonCd);
            SetNextLoopTime(Game.Instance.ServerTime.AddMilliseconds(skillCd));
        }
        else
        {
            SetNextLoopTime(Game.Instance.ServerTime.AddMilliseconds(100));
        }
    }
    private void SetDropItem()
    {
        mDropPos.Clear();
        
        ObjManager.Instance.SelectObj(obj =>
        {
            if (obj.GetObjType() == OBJ.TYPE.DROPITEM)
            {
                var dropItem = obj as ObjDropItem;
                if (dropItem != null)
                {
                    if (dropItem.IsMine() && false == dropItem.BagIsFull() && dropItem.STATUS != ObjDropItem.DropItemState.Pickup)
                    {
                        AddDropPos(dropItem);
                    }
                }
            }
            return true;
        });
    }
    private void SerchNearDrop()
    {
        if (MainPlayer == null)
        {
            Logger.Log2Bugly("SerchNearObj  MainPlayer = null");
            return;
        }
        if (ObjManager.Instance == null)
        {
            Logger.Log2Bugly("SerchNearObj  ObjManager.Instance = null");
            return;
        }
        SetDropItem();
        //Debug.LogError("List 初始化个数" + mDropPos.Count.ToString());
    }
    private ObjBase SerchNearObj(bool hasDrop = false)
    {
        if (MainPlayer == null)
        {
            Logger.Log2Bugly("SerchNearObj  MainPlayer = null");
            return null;
        }
        if (ObjManager.Instance == null)
        {
            Logger.Log2Bugly("SerchNearObj  ObjManager.Instance = null");
            return null;
        }
        ObjBase targetObj = null;
        if (mIsDropFirst)
        {
            targetObj = ObjManager.Instance.SelectNearestObj(MainPlayer.Position, obj =>
            {
                if (obj.GetObjType() == OBJ.TYPE.DROPITEM)
                {
                    if (hasDrop)
                    {
                        var dropItem = obj as ObjDropItem;
                        if (dropItem != null)
                        {
                            if (dropItem.HasAutoFightMove == false && dropItem.CanPickup())
                            {
                                var control = UIManager.Instance.GetController(UIConfig.SettingUI);
                                if (
                                    (bool)
                                        control.CallFromOtherClass("CanPiackUpItem", new object[] {dropItem.GetDataId()}))
                                {
//是否可拾取这种类型物品
                                    var distance = Vector2.Distance(MainPlayer.Position.xz(), dropItem.Position.xz());
                                    if (distance > GameUtils.AutoPickUpDistance)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            });
        }

        if (targetObj == null)
        {
            targetObj = ObjManager.Instance.SelectNearestObj(MainPlayer.Position, obj =>
            {
                if (obj.IsCharacter())
                {
                    var character = obj as ObjCharacter;

                    if (character == null || character.Dead || character.CharacterBaseData.Hp == 0)
                    {
                        return false;
                    }
                    if (!MainPlayer.IsMyEnemy(character))
                    {
                        return false;
                    }
                    if (mIsDropFirst)
                    {
                        var diff = Vector2.Distance(character.Position.xz(), MainPlayer.Position.xz());
                        if (diff > 5.0f)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                if (mIsDropFirst == false)
                {
                    if (obj.GetObjType() == OBJ.TYPE.DROPITEM)
                    {
                        if (hasDrop)
                        {
                            var dropItem = obj as ObjDropItem;
                            if (dropItem != null)
                            {
                                if (dropItem.HasAutoFightMove == false && dropItem.CanPickup())
                                {
                                    var control = UIManager.Instance.GetController(UIConfig.SettingUI);
                                    if (
                                        (bool)
                                            control.CallFromOtherClass("CanPiackUpItem",
                                                new object[] {dropItem.GetDataId()}))
                                    {
//是否可拾取这种类型物品
                                        var distance = Vector2.Distance(MainPlayer.Position.xz(), dropItem.Position.xz());
                                        if (distance > GameUtils.AutoPickUpDistance)
                                        {
                                            if (distance < GameUtils.AutoPickUpDistanceMax || mIsDropFirst)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            });
        }

        return targetObj;
    }

    public void SetMissionList(List<int> list)
    {
        mMissionList = list;
    }

    private void SetNextLoopTime(DateTime t)
    {
        mNextUpdataTime = t;
        mLoopState = AUTO_STAT.AUTO_STAT_SKILL_CD_NOMOVE;
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void StartMove()
    {
        mMoveStopped = false;
        mLoopState = AUTO_STAT.AUTO_STAT_MOVE_TARGET;
    }

    private void UseSkill(int skillId = -1)
    {
        if (!mMoveStopped)
        {
            mMoveStopped = true;
            // 如果之前移动过，移动完之后，先使用普通攻击
            var moveDuration = Time.time - mLastSkillTime;
            if (moveDuration > 1)
            {
                mNormalSkillTime = Time.time + 0.5f;
            }
        }

        if (skillId == -1)
        {
            skillId = CalculateSkillId();
        }
        if (skillId == -1)
        {
            SetNextLoopTime(Game.Instance.ServerTime.AddMilliseconds(100));
            return;
        }
        if (skillId == PlayerDataManager.Instance.GetNormalSkill(true) || mNormalSkillTime > Time.time)
        {
            //如果使用的空手技能，发包前还需要改过来
            skillId = PlayerDataManager.Instance.GetNormalSkill();
        }
        if (GameControl.Instance.OnAttackBtnClick(skillId, false))
        {
            CancelLoop();
        }
        else
        {
            SetNextLoopTime(Game.Instance.ServerTime.AddMilliseconds(100));
        }
    }

    //等待已经释放技能的公共cd时间
    private void WaiteCommonCd()
    {
        mNextUpdataTime = mCommondCdTime;
        mLoopState = AUTO_STAT.AUTO_STAT_SKILL_CD;
    }
    private void PickUpLogic()
    {
        if (Time.realtimeSinceStartup - mPickTimer < 1.0f)
        {
            //Debug.LogError("wait==========================" + (Time.realtimeSinceStartup - mPickTimer).ToString());
            return;
        }
        else if(mDropPos.Count == 0)
        {
            if(mLastStatus == AUTO_STAT.AUTO_STAT_COMMIT_MMISSION)
            {
                //Debug.LogError("=============================================空list");
                MissionComplete();
            }
            else
            {//继续自动挂机
                mAutoPickTimer = Time.realtimeSinceStartup;
                if(mIsFuben)
                    AutoFight();
                else 
                    MoveStartPosition();
            }
        }
        else if(mDropPos.Count>0)
        {
            if(mTargetPostion == Vector3.zero)
            {
                //Debug.LogError("=============================================首次进入 list个数" + mDropPos.Count.ToString());
                mTargetPostion = mDropPos[0];
                if (false == MainPlayer.MoveTo(mTargetPostion))
                {
                    mTargetPostion = Vector3.zero;
                    mDropPos.RemoveAt(0);
                }
            }
            var dis = GameUtils.Vector3PlaneDistance(MainPlayer.Position, mTargetPostion);
            //Debug.LogError("========================"+dis.ToString()+"=====================目标位置x" + mTargetPostion.x.ToString() + " y"+mTargetPostion.y.ToString());
            if(dis<=1.5f)
            {
                mTargetPostion = Vector3.zero;
                SetDropItem();
            }
        }
    }
    
}