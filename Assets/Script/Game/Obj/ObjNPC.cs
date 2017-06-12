#region using

using GameUI;
using System;
using System.Collections.Generic;
using DataContract;
using DataTable;
using EventSystem;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

public class ObjNPC : ObjCharacter
{
    public static int LastNpcSoundId = -1;
    public static DateTime LastSoundTime = DateTime.Now;
    private static readonly float MAX_SECOND_POP = 60*2;
    private static readonly float MIN_SECOND_POP = 60;
    public static int MissionSoundId = -1;
    public Vector3 mBackupDir;
    private DateTime mNextPopTime = DateTime.Now.AddSeconds(Random.Range(MIN_SECOND_POP, MAX_SECOND_POP));
    private bool mTowardPlayer;
    //表格数据索引
    public NpcBaseRecord TableNPC;

    public bool TowardPlayer
    {
        get { return mTowardPlayer; }

        set
        {
            if (null == TableNPC)
            {
                return;
            }
            if (0 == TableNPC.IsForwardYou)
            {
                return;
            }

            mTowardPlayer = value;
            if (!mTowardPlayer)
            {
                TargetDirection = mBackupDir;
            }
            else
            {
                var table = Table.GetAnimation(OBJ.CHARACTER_ANI.STAND);
                var controller = GetAnimationController();
                if (null != controller)
                {
                    if (!controller.Animation.IsPlaying(table.AinmName))
                    {
                        controller.Play(OBJ.CHARACTER_ANI.STAND);
                    }
                }
            }
        }
    }

    //是否可交互
    public bool CanBeInteractive()
    {
        if (TableNPC == null)
        {
            return false;
        }
        return 0 != TableNPC.Interactive;
    }

    public override void CreateNameBoard(string str = "", Dictionary<int, string> titleList = null)
    {
        if (!CanBeInteractive())
        {
            return;
        }

        base.CreateNameBoard(str);
    }

    public override void Destroy()
    {
        EventDispatcher.Instance.RemoveEventListener(Event_UpdateMissionData.EVENT_TYPE, OnMissionUpdate);
        base.Destroy();

        var collider = gameObject.GetComponent<Collider>();
        if (null != collider)
        {
            Destroy(collider);
        }
        gameObject.layer = LayerMask.NameToLayer(GAMELAYER.ObjLogic);
    }

    public void DoDialogue()
    {
        TowardPlayer = true;

        if (-1 != MissionSoundId && (DateTime.Now - LastSoundTime).TotalSeconds < 1)
        {
            return;
        }

        if (null != TableNPC)
        {
            var soundId = TableNPC.DialogSound;
            if (-1 != soundId)
            {
                if (SoundManager.Instance.IsPlaying(MissionSoundId))
                {
                    return;
                }

                if (LastNpcSoundId != soundId)
                {
                    SoundManager.Instance.StopSoundEffect(LastNpcSoundId);
                }

                if (!SoundManager.Instance.IsPlaying(soundId))
                {
                    var isPlayingNpcSound = Table.GetClientConfig(1204);
                    if (int.Parse(isPlayingNpcSound.Value) == 1)
                    {
                        SoundManager.Instance.PlaySoundEffect(soundId);
                        LastNpcSoundId = soundId;
                    }
                }
            }
        }
    }

    public override void DoDie()
    {
        base.DoDie();
        var collider = gameObject.GetComponent<Collider>();
        if (null != collider)
        {
            collider.enabled = false;
        }
        ShowNameBoard(false);
    }

    //自言自语
    public void DoMonology()
    {
        if (null == TableNPC)
        {
            return;
        }

        if (mNextPopTime > DateTime.Now)
        {
            return;
        }
        mNextPopTime = DateTime.Now.AddSeconds(Random.Range(MIN_SECOND_POP, MAX_SECOND_POP));
        var str = TableNPC.pop[Random.Range(0, TableNPC.pop.Length)];
        if (string.IsNullOrEmpty(str))
        {
            return;
        }
        PopTalk(str, 3);
    }

    public override string GetNameBoardRes()
    {
        return Resource.PrefabPath.NPCNameBoard;
    }

    public override OBJ.TYPE GetObjType()
    {
        return OBJ.TYPE.NPC;
    }

    public override bool Init(InitBaseData initData, Action callback = null)
    {
        gameObject.layer = LayerMask.NameToLayer(GAMELAYER.ObjLogic);

        if (!base.Init(initData))
        {
            return false;
        }
        mBackupDir = Direction;
        TargetDirection = Direction;
        mNextPopTime = DateTime.Now.AddSeconds(Random.Range(5, 12));
        EventDispatcher.Instance.AddEventListener(Event_UpdateMissionData.EVENT_TYPE, OnMissionUpdate);


        {
//如果是带Collider的就先给加上，免得后面Load模型有延迟
            if (null != TableNPC && TableNPC.NPCStopRadius > 0.1f)
            {
                var radius = TableNPC.NPCStopRadius;

                var collider = gameObject.GetComponent<SphereCollider>();
                if (null == collider)
                {
                    collider = gameObject.AddComponent<SphereCollider>();
                }
                collider.radius = radius;
                gameObject.layer = LayerMask.NameToLayer(GAMELAYER.Collider);
            }
        }
        return true;
    }

    protected override void InitNavMeshAgent()
    {
        if (null == TableCharacter)
        {
            return;
        }
        if (null == TableNPC)
        {
        }
    }

    //初始化表格数据
    protected override void InitTableData()
    {
        base.InitTableData();
#if UNITY_EDITOR
// 		if (OBJ.TYPE.NPC != (OBJ.TYPE)TableCharacter.Type)
// 		{
// 			Logger.Error("OBJ.TYPE.NPC != (OBJ.TYPE)TableCharacter.Type");
// 		}
#endif
        TableNPC = Table.GetNpcBase(TableCharacter.ExdataId);
        //var tbCharacter = Table.GetCharacterBase(TableCharacter.ExdataId);
        //if (tbCharacter != null)
        //{
        //    CharacterBaseData.Level = tbCharacter.Attr[0];    
        //}
    }

    //是否是主动攻击的怪
    public bool IsAgressive()
    {
        return TableNPC.ViewDistance > 0;
    }

    public override bool IsPlaySpecialIdleAnimation()
    {
        return mSpecialIdleAnimationId.Count > 0 && !TowardPlayer;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.color = Color.black;
        var player = ObjManager.Instance.MyPlayer;
// 		if(null!=player)
// 		{
// 			var pos = transform.position;
// 			pos.y += 2.2f;
// 
// 			var distance = Vector2.Distance(player.Position.xz(), Position.xz());
// 			Handles.Label(pos, "["+distance.ToString()+"]");
// 		}


        Handles.color = Color.green;
        var p = Position;
        var TargetPosCount0 = TargetPos.Count;
        for (var i = 0; i < TargetPosCount0; i++)
        {
            Gizmos.DrawLine(p, TargetPos[i]);
            p = TargetPos[i];
        }


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ServerRealPos, 0.6f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(Position, Position + ServerRealDir*1.0f);
#endif
    }

    private void OnMissionUpdate(IEvent ievent)
    {
        RefreshMissionState();
    }

    public override void OnNameBoardRefresh()
    {
        if (!CanBeInteractive())
        {
            return;
        }
        base.OnNameBoardRefresh();
        RefreshMissionState();
    }

    public override void RefreshAnimation()
    {
        var currentState = GetCurrentStateName();
        switch (currentState)
        {
            case OBJ.CHARACTER_STATE.RUN:
            {
                PlayAnimation(OBJ.CHARACTER_ANI.RUN);
            }
                break;
            case OBJ.CHARACTER_STATE.BORN:
            {
                PlayAnimation(OBJ.CHARACTER_ANI.BORN, aniName => { DoIdle(); });
                if (-1 != CharModelRecord.BornEffectId)
                {
                    var tableData = Table.GetEffect(CharModelRecord.BornEffectId);
                    EffectManager.Instance.CreateEffect(tableData, this, null, null, null, false);
                }
            }
                break;
            case OBJ.CHARACTER_STATE.HURT:
            {
                PlayAnimation(OBJ.CHARACTER_ANI.HIT, str =>
                {
                    if (!this)
                    {
                        return;
                    }

                    PlayAnimation(OBJ.CHARACTER_ANI.STAND);
                });
            }
                break;
            case OBJ.CHARACTER_STATE.DIZZY:
            {
                PlayAnimation(OBJ.CHARACTER_ANI.DIZZY);
            }
                break;
            case OBJ.CHARACTER_STATE.IDLE:
            default:
            {
                PlayAnimation(OBJ.CHARACTER_ANI.STAND);
            }
                break;
        }
    }

    public void RefreshMissionState()
    {
        if (!CanBeInteractive())
        {
            return;
        }

        if (null == NameBoard)
        {
            return;
        }

        var player = ObjManager.Instance.MyPlayer;
        if (null == player)
        {
            return;
        }

        var npcNameBoard = NameBoard as NpcOverheadFrame;

        if (player.IsMyEnemy(this))
        {
            npcNameBoard.State = NpcOverheadFrame.MissionState.None;
            return;
        }

        //先用任务里的那个，这里面带优先级任务类型排序
        var id = MissionManager.Instance.HasMission(mDataId);
        if (-1 == id)
        {
            var tempState = NpcOverheadFrame.MissionState.None;
            {
                // foreach(var pair in MissionManager.Instance.MissionData.Datas)
                var __enumerator1 = (MissionManager.Instance.MissionData.Datas).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var pair = __enumerator1.Current;
                    {
                        var missionId = pair.Key;
                        var tableData = Table.GetMissionBase(missionId);
                        if (null == tableData)
                        {
                            continue;
                        }

                        if (tableData.FinishNpcId != mDataId)
                        {
                            continue;
                        }

                        if (eMissionState.Unfinished == (eMissionState) pair.Value.Exdata[0])
                        {
                            tempState = NpcOverheadFrame.MissionState.Unfinish;
                            break;
                        }
                    }
                }
            }

            npcNameBoard.State = tempState;
        }
        else
        {
            var state = MissionManager.Instance.GetMissionState(id);
            if (eMissionState.Acceptable == state)
            {
                npcNameBoard.State = NpcOverheadFrame.MissionState.Quest;
            }
            else if (eMissionState.Finished == state)
            {
                npcNameBoard.State = NpcOverheadFrame.MissionState.Finish;
            }
            else if (eMissionState.Unfinished == state)
            {
                npcNameBoard.State = NpcOverheadFrame.MissionState.Unfinish;
            }
            else
            {
                npcNameBoard.State = NpcOverheadFrame.MissionState.None;
            }
        }
        EventDispatcher.Instance.DispatchEvent(new Event_RefreshFuctionOnState());

    }

    public virtual void Relive()
    {
        base.Relive();
        ShowNameBoard(true);
    }

    public override void ShowDamage(BuffResult result)
    {
        if (!CanBeInteractive())
        {
            return;
        }

        base.ShowDamage(result);
    }

    protected override void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        base.Start();
        mBackupDir = Direction;
        if (!CanBeInteractive())
        {
            if (null != mModel)
            {
                if (null != mModel.gameObject.collider)
                {
                    Destroy(mModel.gameObject.collider);
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

    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        Tick(Time.deltaTime);

        if (CanBeInteractive())
        {
            if (TowardPlayer)
            {
                if (null != ObjManager.Instance && null != ObjManager.Instance.MyPlayer)
                {
                    var player = ObjManager.Instance.MyPlayer;
                    var dis = (Position - player.Position).magnitude;
                    if (dis <= 5)
                    {
                        FaceTo(player.Position);
                    }
                    else
                    {
                        TowardPlayer = false;
                        TargetDirection = mBackupDir;
                    }
                }
            }
        }
        DoMonology();
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }
}