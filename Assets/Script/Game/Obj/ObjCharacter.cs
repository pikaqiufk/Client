#region using

using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourMachine;
using ClientDataModel;
using DataContract;
using DataTable;
using EventSystem;
using GameUI;
using ServiceBase;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#endregion

public enum MountPoint
{
    LeftWeapen,
    RightWeapen,
    Center,
    Top,
    Bottom
}


public class ObjCharacter : ObjBase
{
    public static readonly string[] MountPointName = {"LeftWeapen", "RightWeapen", "Center", "Top", "Bottom"};
    public const float MOVESPEED_RATE = 0.01f;
    //构造函数
    public ObjCharacter()
    {
        mBuff = new BuffManager(this);
    }

    //称号名称
    public string AllianceName = "";
    //模型CharaModelID,如果不是-1就造这个模型，否则用表里配的
    protected int mCharaModelID = -1;
    //挂载点缓存
    protected Dictionary<int, Transform> mMountPoints = new Dictionary<int, Transform>();
    //RenderQueue
    protected int mRenderQueue = -1;
    public List<int> mSpecialIdleAnimationId = new List<int>();
    //当前是不是在使用假模型
    private bool mUsingFakeModel;
    //播放下一个休闲声音时间
    public float NextPlayIdleSoundTime;
    public OnWingLoaded OnWingLoadedCallback;
    //称号列表
    public Dictionary<int, int> TitleList = new Dictionary<int, int>();
    private bool ShowInMiniMap;
    private const float MiniMapInterval = 0.5f;
    private float miniMapTime = MiniMapInterval;

    public enum WingState
    {
        Idle = 0,
        Move,
        Dead
    }

    public virtual CharacterBaseDataModel CharacterBaseData { get; set; }
    public float DeleteObjTime { get; set; }
    //特效挂载体
    public GameObject EffectRoot { get; protected set; }
    //重新加载模型，不要随便调
    public int ModelId
    {
        get
        {
            if (-1 != mCharaModelID)
            {
                return mCharaModelID;
            }

            if (null == TableCharacter)
            {
                TableCharacter = Table.GetCharacterBase(mDataId);
            }

            return TableCharacter.CharModelID;
        }
        set
        {
            if (mCharaModelID != value)
            {
                mCharaModelID = value;
                LoadModelAsync();
            }
        }
    }

    public int RenderQueue
    {
        get { return mRenderQueue; }
        set { mRenderQueue = value; }
    }

    #region Character属性

    public List<Vector3> TargetPos
    {
        get { return mTargetPos; }
        set { mTargetPos = value; }
    }

    #endregion

    public bool UsingFakeModel
    {
        get { return mUsingFakeModel; }
        set
        {
            if (mUsingFakeModel == value)
            {
                return;
            }

            mUsingFakeModel = value;

            if (!mUsingFakeModel)
            {
                LoadModelAsync(() =>
                {
                    State = ObjState.Normal;
                    CreateNameBoard();
                    InitShadow();
                    InitEquip();
                    RefreshAnimation();
                });
            }
        }
    }

    public static Color CalculateNameColor(ObjCharacter obj)
    {
        var isMyEnemy = ObjManager.Instance.MyPlayer.IsMyEnemy(obj);
        var objType = obj.GetObjType();
        if (!isMyEnemy)
        {
            if (OBJ.TYPE.NPC == objType)
            {
                return Color.cyan;
            }
            return Color.green;
        }

        if (OBJ.TYPE.NPC == objType)
        {
            var npc = obj as ObjNPC;
            if (!npc.IsAgressive())
            {
                return Color.yellow;
            }
        }

        return Color.red;
    }

    public void ChangeAttributeHp(int value)
    {
//         if (value < 0)
//         {
//             value = 0;
//         }
//         else if (value > max)
//         {
//             value = max;
//         }
        CharacterBaseData.Hp = value;
    }

    public IEnumerable DelayPlayFadeOutAnimationAndRemove(float delay)
    {
        yield return new WaitForSeconds(delay);
        DeleteObjTime = 0;
        PlayFadeOutAnimationAndRemove();
    }

    private IEnumerator DoDeadAnimationAndRemove(ulong objId, Material mat)
    {
        var totalTime = 2.3f;
        var time = totalTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            var x = (totalTime - time)/totalTime;
            mat.SetFloat("_Cutoff", x*x*x);
            yield return null;
        }

        ObjManager.Instance.RemoveObj(objId);
        //Logger.Debug("ObjManager.Instance.RemoveObj({0});", objId);
    }

    internal void ForceMoveTo(float x, float y, float speed)
    {
        var target = GameLogic.GetTerrainPosition(x, y);
        var diff = (target - Position);
        var dist = diff.magnitude;

        NavMeshHit hit;
        if (NavMesh.Raycast(Position, target, out hit, -1))
        {
            target = hit.position;
        }

        RaycastHit pHit;
        if (Physics.Raycast(Position, diff/dist, out pHit, dist, mColliderLayer))
        {
            if (hit.hit)
            {
                if (hit.distance > pHit.distance)
                {
                    target = pHit.point;
                }
            }
            else
            {
                target = pHit.point;
            }
        }

        var dir = new Vector2(target.x, target.z) - Position.xz();
        mForceMoveRemindTime = (dir.magnitude - 0.1f)/speed;
        dir.Normalize();
        if (dir == Vector2.zero)
        {
            dir = Vector2.one;
        }
        mForceMoveSpeed = dir*speed;
    }

    //名字板资源
    public virtual string GetNameBoardRes()
    {
        return Resource.PrefabPath.NameBoard;
    }

    public bool HasWing()
    {
        {
            // foreach(var pair in EquipList)
            var __enumerator6 = (EquipList).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var pair = __enumerator6.Current;
                {
                    if ((int) eBagType.Wing == pair.Key && -1 != pair.Value)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public virtual void InitShadow(bool isDynamicShadow = false, int dynamicCullingMask = -1)
    {
        if (GameSetting.Instance.GameQualityLevel > 1)
        {
            if (isDynamicShadow)
            {
                //场景的meshtree改为进入场景后异步加载了，加载好后会resetshadow，所以这里就不创建角色阴影了
                if (null == GameLogic.Instance.Scene.MeshTree)
                {
                    return;
                }

                //先清理，后添加
                var shadowRoot = GameLogic.Instance.Scene.ShadowRoot.transform;
                if (null != shadowRoot)
                {
                    for (int i = 0; i < shadowRoot.childCount; i++)
                    {
                        var child = shadowRoot.GetChild(i);
                        if (null != child)
                        {
                            GameObject.Destroy(child.gameObject);
                        }
                    }
                }

                var model = FindChildTransform(mModelTransform, "Center");
                GameLogic.Instance.Scene.CreateDynamicShadow(new Vector3(0, 0, 0), model.gameObject, mModel,
                    mDestroyObjectsWhenDestroy, gameObject.layer, dynamicCullingMask);
            }
            else
            {
                GameLogic.Instance.Scene.CreateBlobShadow(gameObject, CharModelRecord.ShadowSize, mDestroyObjectsWhenDie);
                var collider = gameObject.GetComponent<CapsuleCollider>();
                if (collider)
                {
                    collider.radius = CharModelRecord.ShadowSize;
                }
            }
        }
    }

    //是否是角色
    public override bool IsCharacter()
    {
        return true;
    }

    public virtual int GetLayerForEffect()
    {
        switch (GetObjType())
        {
            case OBJ.TYPE.OTHERPLAYER:
                return LayerMask.NameToLayer("OtherPlayer");
            case OBJ.TYPE.MYPLAYER:
                return LayerMask.NameToLayer("IgnoreShadow");
            case OBJ.TYPE.FAKE_CHARACTER:
                return LayerMask.NameToLayer("ObjLogic");
            case OBJ.TYPE.ELF:
                return LayerMask.NameToLayer("Elf");
            case OBJ.TYPE.NPC:
                return LayerMask.NameToLayer("ObjLogic");
            case OBJ.TYPE.RETINUE:
                return LayerMask.NameToLayer("ObjLogic");
            default:
                return 0;
        }
    }

    public bool IsMoving()
    {
        return GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN;
    }

    private IEnumerator MoveDownAndRemove(ulong objId, Transform trans)
    {
        var totalTime = 2.3f;
        var time = 0.0f;
        while (State == ObjState.Dieing && time < totalTime)
        {
            time += Time.deltaTime;
            var p = Vector3.zero;
            p.y = -time/totalTime;
            trans.localPosition = p;
            yield return null;
        }

        ObjManager.Instance.RemoveObj(objId);
    }

    public void NameBoardUpdate()
    {
        if (null != NameBoard)
        {
            var camp = GetCamp();
            if (camp >= 7 && camp <= 9)
            {
                NameBoard.SetBattleColor(camp);
            }
            foreach (var item in TitleList)
            {
                var isSelf = GetObjType() == OBJ.TYPE.MYPLAYER;
                NameBoard.SetTitle(item.Key, item.Value, AllianceName, isSelf);
            }
            NameBoard.RestLayoutTitle();
        }
    }
    public void ShowHideOtherTitle(bool isSHow)
    {
        if (null != NameBoard)
        {
            foreach (var item in TitleList)
            {
                NameBoard.ShowHideOtherTitle(isSHow, item.Key, item.Value, AllianceName);
            }
            NameBoard.RestLayoutTitle();
        }
    }

    //设置Top的偏移
    protected void OffsetTopMountPositon(Vector3 offset)
    {
        var topMount = GetMountPoint((int) MountPoint.Top);
        if (null != topMount && Vector3.zero != mTopMountPointPos)
        {
            topMount.localPosition = mTopMountPointPos + offset;
        }
    }

    protected override void OnSetModel()
    {
        mMountPoints.Clear();
        if (mActorAvatar)
        {
            mActorAvatar.Body = mModel;
        }
        gameObject.SetLayerRecursive(gameObject.layer, LayerMask.GetMask(GAMELAYER.IgnoreShadow, GAMELAYER.Collider));
    }

    public void PlayFadeOutAnimationAndRemove()
    {
        if (CharModelRecord.DieAnimation == -1)
        {
            ObjManager.Instance.RemoveObj(GetObjId());
        }
        else if (CharModelRecord.DieAnimation == 0)
        {
            var objId = GetObjId();
            ResourceManager.PrepareResource<Material>(Resource.Dir.Material + "Dead.mat", material =>
            {
                OptList<SkinnedMeshRenderer>.List.Clear();
                GetComponentsInChildren(OptList<SkinnedMeshRenderer>.List);
                var renderers = OptList<SkinnedMeshRenderer>.List;
                if (renderers.Count > 0)
                {
                    var __array5 = renderers;
                    var __arrayLength5 = __array5.Count;
                    for (var __i5 = 0; __i5 < __arrayLength5; ++__i5)
                    {
                        var skinnedMeshRenderer = __array5[__i5];
                        {
                            var texture = skinnedMeshRenderer.sharedMaterial.GetTexture("_MainTex");
                            var mat = new Material(material);
                            mat.SetTexture("_MainTex", texture);
                            skinnedMeshRenderer.material = mat;

                            State = ObjState.Dieing;
                            StartCoroutine(DoDeadAnimationAndRemove(objId, mat));
                        }
                    }
                }
                else
                {
                    ObjManager.Instance.RemoveObj(GetObjId());
                }
            });
        }
        else if (CharModelRecord.DieAnimation == 1)
        {
            if (GetModel())
            {
                State = ObjState.Dieing;
                StartCoroutine(MoveDownAndRemove(GetObjId(), mModelTransform));
            }
            else
            {
                ObjManager.Instance.RemoveObj(GetObjId());
            }
        }
    }

    public void PlayWingAnimation(WingState state)
    {   
        if (mActorAvatar != null && mActorAvatar.WingGameObject != null)
        {
            var anim = mActorAvatar.WingGameObject.GetComponentInChildren<Animation>();
            if (anim == null)
            {
                return;
            }

            switch (state)
            {
                case WingState.Idle:
                {
                    anim.CrossFade("FlyIdle", 0.5f);
                }
                    break;
                case WingState.Move:
                {
                    anim.CrossFade("FlyMove", 0.5f);
                }
                    break;
                case WingState.Dead:
                {
                    anim.CrossFade("Dead", 0.01f);
                }
                    break;
            }
        }
    }

    public void PopTalk(string str, float time = 4)
    {
        if (null != NameBoard)
        {
            if (NameBoard.active)
            {
                NameBoard.Talk(str, time);
            }
        }
    }

    public virtual void RefreshAnimation()
    {
        var currentState = GetCurrentStateName();
        switch (currentState)
        {
            case OBJ.CHARACTER_STATE.RUN:
            {
                if (HasWing())
                {
//                     if (IsInSafeArea())
//                     {
//                         PlayAnimation(OBJ.CHARACTER_ANI.Walk);
//                     }
//                     else
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.FlyMove);
                    }
                }
                else
                {
//                     if (IsInSafeArea())
//                     {
//                         PlayAnimation(OBJ.CHARACTER_ANI.Walk);
//                     }
//                     else
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.AttackMove);
                    }
                }
            }
                break;
            case OBJ.CHARACTER_STATE.HURT:
            {
                if (HasWing())
                {
//                     if (IsInSafeArea())
//                     {
//                         PlayAnimation(OBJ.CHARACTER_ANI.HIT, (aniName) => { DoIdle(); });
//                     }
//                     else
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.FLYHIT, aniName => { DoIdle(); });
                    }
                }
                else
                {
//                     if (IsInSafeArea())
//                     {
//                         PlayAnimation(OBJ.CHARACTER_ANI.HIT, (aniName) => { DoIdle(); });
//                     }
//                     else
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.HIT, aniName => { DoIdle(); });
                    }
                }
            }
                break;
            case OBJ.CHARACTER_STATE.BORN:
            {
                PlayAnimation(OBJ.CHARACTER_ANI.BORN, aniName => { DoIdle(); });
            }
                break;
            case OBJ.CHARACTER_STATE.DIE:
            {
                PlayAnimation(OBJ.CHARACTER_ANI.DIE);
            }
                break;
            case OBJ.CHARACTER_STATE.DIZZY:
            {
                if (HasWing())
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.FlyDizzy);
                }
                else
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.DIZZY);
                }
            }
                break;
            case OBJ.CHARACTER_STATE.IDLE:
            default:
            {
                if (HasWing())
                {
//                      if (IsInSafeArea())
//                      {
//                          PlayAnimation(OBJ.CHARACTER_ANI.STAND);
//                      }
//                      else
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.FlyIdle);
                    }
                }
                else
                {
                    if (IsInSafeArea())
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.STAND);
                    }
                    else
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.AttackIdle);
                    }
                }
            }
                break;
        }

        if (HasWing())
        {
//有翅膀
            if (GetCurrentStateName() == OBJ.CHARACTER_STATE.DIE)
            {
                PlayWingAnimation(WingState.Dead);
            }
            else
            {
// 				if (IsInSafeArea())
// 				{//在安全区
// 					//安全区肯定是走的状态，翅膀慢速
// 					PlayWingAnimation(WingState.Idle);
// 
//                     if (NameBoard)
//                     {
//                         NameBoard.ResetOffset();
//                     }
// 				}
// 				else
                {
                    if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
                    {
//翅膀播加速
                        PlayWingAnimation(WingState.Move);
                    }
                    else
                    {
                        PlayWingAnimation(WingState.Idle);
                    }

                    if (NameBoard)
                    {
                        NameBoard.SetFlyOffset();
                    }
                }
            }
        }
        else
        {
            if (NameBoard)
            {
                NameBoard.ResetOffset();
            }
        }
        /*
        if (HasWing())
        {//有翅膀
            if (IsInSafeArea())
            {//在安全区
                //安全区肯定是走的状态，翅膀慢速
                PlayWingAnimation(false);

                if (GetCurrentStateName() == OBJ.CHARACTER_STATE.IDLE)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.STAND);
                }
                else if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.Walk);
                }
                else if (GetCurrentStateName() == OBJ.CHARACTER_STATE.HURT)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.HIT, (aniName) => { DoIdle(); });
                }

                if (NameBoard)
                {
                    NameBoard.ResetOffset();
                }
            }
            else
            {//不在安全区
                if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
                {//翅膀播加速
                    PlayWingAnimation(true);
                }
                else
                {
                    PlayWingAnimation(false);
                }

                if (GetCurrentStateName() == OBJ.CHARACTER_STATE.IDLE)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.FlyIdle);
                }
                else if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.FlyMove);
                }
                else if (GetCurrentStateName() == OBJ.CHARACTER_STATE.HURT)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.FLYHIT, (aniName) => { DoIdle(); });
                }

                // 如果在飞的话，把NameBoard往上移一点
                if (NameBoard)
                {
                    NameBoard.SetFlyOffset();
                }
            }
        }
        else
        {//没有翅膀
            if (IsInSafeArea())
            {//在安全区
                //安全区肯定是走的状态，翅膀慢速
                PlayWingAnimation(false);

                if (GetCurrentStateName() == OBJ.CHARACTER_STATE.IDLE)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.STAND);
                }
                else if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.Walk);
                }
                else if (GetCurrentStateName() == OBJ.CHARACTER_STATE.HURT)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.HIT, (aniName) => { DoIdle(); });
                }

                if (NameBoard)
                {
                    NameBoard.ResetOffset();
                }
            }
            else
            {//不再安全区
                if (GetCurrentStateName() == OBJ.CHARACTER_STATE.IDLE)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.AttackIdle);
                }
                else if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.AttackMove);
                }
                else if (GetCurrentStateName() == OBJ.CHARACTER_STATE.HURT)
                {
                    PlayAnimation(OBJ.CHARACTER_ANI.HIT, (aniName) => { DoIdle(); });
                }

                if (NameBoard)
                {
                    NameBoard.ResetOffset();
                }
            }
        }
		 * */
    }

    //恢复Top的偏移
    protected void RestoreTopMountPositon()
    {
        var topMount = GetMountPoint((int) MountPoint.Top);
        if (null != topMount && Vector3.zero != mTopMountPointPos)
        {
            topMount.localPosition = mTopMountPointPos;
        }
    }

    public void ShowNameBoard(bool flag)
    {
        if (null != NameBoard)
        {
            NameBoard.gameObject.SetActive(flag);
        }
    }
    public void SetActive(bool flag)
    {
        if (null != gameObject)
        {
            gameObject.SetActive(flag);
        }
    }

    public virtual void StartAttributeSync()
    {
        var characterId = GetObjId();
        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncHpMax,
            i => { CharacterBaseData.MaxHp = i; });
        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncHpNow,
            i => { CharacterBaseData.Hp = i; });
        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncMpMax,
            i => { CharacterBaseData.MaxMp = i; });
        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncMpNow,
            i => { CharacterBaseData.Mp = i; });
        NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncMoveSpeed,
            i =>
            {
                int speed = i;
                CharacterBaseData.MoveSpeed = (float) speed*MOVESPEED_RATE;

                SetMoveSpeed(CharacterBaseData.MoveSpeed*GameSetting.Instance.CharacterSpeedDelta);
            });
    }

    public virtual void StopAttributeSync()
    {
        var characterId = GetObjId();
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncHpMax);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncHpNow);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncMpMax);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncMpNow);
        NetManager.Instance.SyncCenter.StopSyncData(ServiceType.Scene, characterId, (uint) eSceneSyncId.SyncMoveSpeed);
    }

    // 只有NPC才能收到这个包，所以放在Character也可以，如果有其他需求，就把这个函数变成虚函数，然后再NPC中override
    internal void SyncPathPosition(SyncPathPos data)
    {
        if (data.Index == 0)
        {
            if (mTargetPos.Count != 0)
            {
                // 服务器都走完了，客户端还没有走完，加快速度
                MoveSpeedAdjust = 5.0f;
                return;
            }

            // 都==0说明客户端和服务器都没在走了
            var diff = Vector2.Distance(Position.xz(), new Vector2(data.X, data.Y));
            if (diff > 1.0f && diff < 5.0f)
            {
                // 还不算太远，加速走过去
                MoveSpeedAdjust = 5.0f;
                MoveTo(GameLogic.GetTerrainPosition(data.X, data.Y));
                return;
            }
            if (diff >= 5.0f)
            {
                // 太远了，直接拉过来吧
                Position = GameLogic.GetTerrainPosition(data.X, data.Y);
            }

            return;
        }

        if (mTargetPos.Count == 0)
        {
            // 说明客户端走到了，服务器还没走到
            var diff = Vector2.Distance(Position.xz(), new Vector2(data.X, data.Y));
            if (diff >= 5.0f)
            {
                // 太远了，直接拉过来吧
                Position = GameLogic.GetTerrainPosition(data.X, data.Y);
            }

            return;
        }

        if (mTargetPos.Count > data.Index)
        {
            // 说明客户端走的太慢
            MoveSpeedAdjust = 1.05f;
        }
        else if (mTargetPos.Count == data.Index)
        {
            // 客户端和服务器在同一段路径上
            var distServer = Vector2.Distance(mTargetPos[0].xz(), new Vector2(data.X, data.Y));
            var distClient = Vector2.Distance(mTargetPos[0].xz(), Position.xz());

            if (distClient > 1)
            {
                // 保证能同时到达下一个点
                MoveSpeedAdjust = 1.0f + (distClient - distServer)/distClient;
            }
            else
            {
                MoveSpeedAdjust = 1.0f + (distClient - distServer)/(GetRawMoveSpeed()*5);
            }

            MoveSpeedAdjust = Mathf.Clamp(MoveSpeedAdjust, 0.8f, 1.2f);
        }
        else
        {
            // 说明客户端走的太快
            MoveSpeedAdjust = 0.95f;
        }
    }

    public delegate void OnWingLoaded(GameObject go);

    #region 基本属性

    //名字
    public string Name
    {
        get { return CharacterBaseData.Name; }
        set { CharacterBaseData.Name = value; }
    }

    //表格数据索引
    protected CharacterBaseRecord TableCharacter { get; set; }

    //阵营表格数据
    public CampRecord TableCamp { get; protected set; }

    //CharModel 表数据
    public CharModelRecord CharModelRecord { get; protected set; }

    public OverheadFrame NameBoard { get; set; }

    public Vector3? DelayedMove { get; set; }

    /// <summary>
    ///     用来记录当前的目标，主要用于技能释放间隔移动后，将朝向调整对，调整完成后将置为null
    /// </summary>
    public ObjCharacter Target { get; set; }

    #endregion

    #region 逻辑属性

    //是否死亡
    protected bool mDead;

    //目标位置
    protected List<Vector3> mTargetPos = new List<Vector3>();
    //阵营
    protected int mCamp = -1;

    public int PkModel
    {
        get { return CharacterBaseData.PkModel; }
        set { CharacterBaseData.PkModel = value; }
    }

    public int PkValue
    {
        get { return CharacterBaseData.PkValue; }
        set { CharacterBaseData.PkValue = value; }
    }

    public int Reborn
    {
        get { return CharacterBaseData.Reborn; }
        set { CharacterBaseData.Reborn = value; }
    }

    public int RoleId
    {
        get { return CharacterBaseData.RoleId; }
        set { CharacterBaseData.RoleId = value; }
    }

    //buff列表
    protected BuffManager mBuff;

    //装备
    public Dictionary<int, int> EquipList = new Dictionary<int, int>();

    //需要在死亡时释放的GameObject
    protected List<Object> mDestroyObjectsWhenDie = new List<Object>();
    //需要在销毁时释放的GameObject
    protected List<Object> mDestroyObjectsWhenDestroy = new List<Object>();

    //是否在强制移动
    public bool IsForceMoving
    {
        get { return mForceMoveRemindTime > 0; }
    }

    private float mMoveSpeedAdjust = 1.0f;

    //移动速度修正
    public float MoveSpeedAdjust
    {
        get { return mMoveSpeedAdjust; }
        set { mMoveSpeedAdjust = value; }
    }

    //强制移动速度
    protected Vector2 mForceMoveSpeed;

    //强制移动剩余时间
    protected float mForceMoveRemindTime;

    //区域状态[主城，野外]
    public eAreaState AreaState { get; set; }

    protected Vector3 mTopMountPointPos = Vector3.zero;

    #endregion

#if UNITY_EDITOR
    //这两个是在editor模式下显示服务端实际的坐标和朝向用的，别人不要用
    [NonSerialized] public Vector3 ServerRealPos;
    [NonSerialized] public Vector3 ServerRealDir;
    [NonSerialized] public bool LogBuff;
#endif

    #region 组件

    //骨骼动画控制器
    protected AnimationController mAnimationController;

    //状态机
    protected StateMachine mStateMachine;

    //导航系统
    protected NavMeshAgent mNavMeshAgent;

    protected ActorAvatar mActorAvatar;

    #endregion

    #region 基本属性方法

    //当设置名字时
    protected virtual void OnSetName()
    {
    }

    //是否已经死亡
    public bool Dead
    {
        get { return mDead; }
        set { mDead = value; }
    }

    //阵营
    public virtual void SetCamp(int camp)
    {
        if (camp == mCamp)
        {
            return;
        }
        mCamp = camp;
        TableCamp = Table.GetCamp(camp);
    }

    public virtual int GetCamp()
    {
        return mCamp;
    }

    //等级
    public virtual int GetLevel()
    {
        return CharacterBaseData.Level;
    }

    public void SetLevel(int lv)
    {
        CharacterBaseData.Level = lv;
    }

    public void ReLoadModel()
    {
        InitTableData();
        InitAnimation();
        LoadModelAsync(() =>
        {
            State = ObjState.Normal;
            CreateNameBoard();
            InitShadow();
            InitEquip();

            RefreshAnimation();
        });
    }

    #endregion

    #region 组件方法

    //初始化表格数据
    protected virtual void InitTableData()
    {
        if (!this)
            return;

        TableCharacter = Table.GetCharacterBase(mDataId);
        TableCamp = Table.GetCamp(mCamp);
        var modelId = ModelId;
        CharModelRecord = Table.GetCharModel(modelId);

        mSpecialIdleAnimationId.Clear();
        {
            var __array1 = TableCharacter.Idle;
            var __arrayLength1 = __array1.Length;
            for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var id = __array1[__i1];
                {
                    if (-1 == id)
                    {
                        break;
                    }
                    mSpecialIdleAnimationId.Add(id);
                }
            }
        }
    }

    //骨骼动画控制器初始化
    protected void InitAnimation(bool sync = false, bool firstPriority = false)
    {
        mAnimationController = gameObject.GetComponent<AnimationController>();
        mAnimationController.Init(string.Empty, string.Empty, CharModelRecord.AnimPath, true, sync, firstPriority);
        mAnimationController.mCharacter = this;
    }


    //播放动画
    public virtual void PlayAnimation(int id, Action<string> action = null)
    {
        if (string.IsNullOrEmpty(CharModelRecord.AnimPath))
        {
            return;
        }

        var tableData = Table.GetAnimation(id);
        if (null == tableData)
        {
            Logger.Error("Animation Id {0} can not found, [{1}]", id, Name);
            return;
        }

        if (null == mAnimationController)
        {
            Logger.Warn("mAnimationController has not been initialized");
            return;
        }
        if (GetModel() != null)
        {
            mAnimationController.Play(id, action);
        }
    }

    public void StopCurrentAnimation(bool b)
    {
        if (mAnimationController != null)
        {
            mAnimationController.Stop(b);
        }
    }

    public void SetAnimationListener(IAnimationListener listener)
    {
        if (mAnimationController != null)
        {
            mAnimationController.SetAnimationListener(listener);
        }
    }

    public AnimationController GetAnimationController()
    {
        return mAnimationController;
    }

    //状态机
    public StateMachine GetStateMachine()
    {
        return mStateMachine;
    }

    //初始化状态
    protected void InitStateMachine()
    {
        mStateMachine.enabled = true;
    }

    //获得当前状态名字
    public string GetCurrentStateName()
    {
        if (mStateMachine && mStateMachine.enabledState)
        {
            return mStateMachine.enabledState.stateName;
        }

        return string.Empty;
    }

    #endregion

    #region 导航系统

    protected virtual void InitNavMeshAgent()
    {
        if (null != mNavMeshAgent)
        {
            mNavMeshAgent.updatePosition = false;
        }
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return mNavMeshAgent;
    }

    #endregion

    #region Mono接口

    protected override void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        base.Awake();
        mStateMachine = gameObject.GetComponent<StateMachine>();
        mNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        mActorAvatar = gameObject.GetComponent<ActorAvatar>();

        if (EffectRoot == null && ObjTransform != null)
        {
            EffectRoot = new GameObject();
            EffectRoot.name = "EffectRoot";
            var transform = EffectRoot.transform;
            transform.parent = ObjTransform;
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
        }

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    protected override void OnDestroy()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        StopAttributeSync();
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public override void Destroy()
    {
        RestoreTopMountPositon();

        mBuff.RemoveAllBuff();
        {
            var __list2 = mDestroyObjectsWhenDestroy;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var destroyObject = __list2[__i2];
                {
                    Destroy(destroyObject);
                }
            }
        }

        {
            var __list2 = mDestroyObjectsWhenDie;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var destroyObject = __list2[__i2];
                {
                    Destroy(destroyObject);
                }
            }
        }

        if (null != NameBoard)
        {
            ComplexObjectPool.Release(NameBoard.gameObject);
            NameBoard = null;
        }

        var e = new Character_Remove_Event(mObjId);
        EventDispatcher.Instance.DispatchEvent(e);

        if (null != mStateMachine)
        {
            mStateMachine.enabled = false;
        }

        StopAttributeSync();

        PlayerDataManager.Instance.RemoveSelectTarget(this);
        //清除技能选择目标

        if (GameControl.Instance != null && GameControl.Instance.TargetObj == this)
        {
            GameControl.Instance.TargetObj = null;
        }

        if (mActorAvatar)
        {
            mActorAvatar.Destroy();
        }

        base.Destroy();
        OnWingLoadedCallback = null;
    }

    public virtual void CreateNameBoard(string str = "", Dictionary<int, string> titleList = null)
    {
        if (null != NameBoard)
        {
            ComplexObjectPool.Release(NameBoard.gameObject, false, false);
            NameBoard = null;
        }
        var isSelf = GetObjType() == OBJ.TYPE.MYPLAYER;

        var character = this;
        if (character == null)
        {
            Logger.Log2Bugly("--character==null--");
            return;
        }
        ComplexObjectPool.NewObject(GetNameBoardRes(), go =>
        {
            if (null == go)
            {
                return;
            }

            if (!character)
            {
                ComplexObjectPool.Release(go);
                return;
            }

            if (!gameObject.activeSelf)
            {
                ComplexObjectPool.Release(go);
                return;
            }

            if (character.State == ObjState.Deleted)
            {
                ComplexObjectPool.Release(go);
                return;
            }

            NameBoard = go.GetComponent<OverheadFrame>();
            if (null == NameBoard)
            {
                ComplexObjectPool.Release(go);
                Logger.Warn("null==NameBoard [{0}]_[{1}]", character.Name, character.mObjId);
                return;
            }

            NameBoard.Reset();

            var owner = character.GetMountPoint((int) MountPoint.Top);
            if (null == owner)
            {
                ComplexObjectPool.Release(go);
                Logger.Warn("null==owner [{0}]_[{1}]", character.Name, character.mObjId);
                return;
            }
            go.SetActive(false);
            NameBoard.SetOwner(owner.gameObject,
                UIManager.Instance.HeadBoardRoot,
                (float) CharModelRecord.HeadInfoHeight);
            go.SetActive(true);

            if (!string.IsNullOrEmpty(str))
            {
                NameBoard.SetText(str);
            }

            if (null != titleList)
            {
                var i = 0;
                foreach (var title in titleList)
                {
                    NameBoard.SetTitle(i++, title.Key, title.Value, isSelf);
                }
                NameBoard.RestLayoutTitle();
            }
            character.OnNameBoardRefresh();
        }, null, null, false, false, false);
    }

    public virtual void OnNameBoardRefresh()
    {
        if (NameBoard == null)
        {
            return;
        }
        NameBoard.SetText(Name, CalculateNameColor(this));
    }

    // 设置头顶面板名字
    public virtual void SetNameBoardName(string name, string camp, int reborn, string nameColor)
    {
        var rebornStr = String.Empty;
        if (reborn > 0)
        {
            rebornStr = string.Format("{0}{1}", reborn, GameUtils.GetDictionaryText(100000644));
        }

        var nameStr = string.Format("[{0}]{1}{2}{3}[-]", nameColor, rebornStr, Name, camp);
        NameBoard.SetText(nameStr);
    }

    #endregion

    #region 自身逻辑

    //初始化
    public override bool Init(InitBaseData initData, Action callback = null)
    {
        Reset();
        if (!base.Init(initData))
        {
            return false;
        }
        CharacterBaseData.CharacterId = initData.ObjId;
        var data = initData as InitCharacterData;
        if (data == null)
        {
            return false;
        }
        UsingFakeModel = data.UseFakeModel;
        Name = data.Name + data.ObjId;
        mDead = data.IsDead;
        DeleteObjTime = 0;
        PkModel = data.PkModel;
        PkValue = data.PkValue;
        Reborn = data.Reborn;
        RoleId = data.DataId;
        CharacterBaseData.Hp = data.HpNow;
        CharacterBaseData.MaxHp = data.HpMax;
        CharacterBaseData.Mp = data.MpNow;
        CharacterBaseData.MaxMp = data.MpMax;
        mCharaModelID = data.ModelId;
        miniMapTime = MiniMapInterval;
        SetCamp(data.Camp);
        EquipList = new Dictionary<int, int>(data.EquipModel);
        CharacterBaseData.Name = data.Name;
        CharacterBaseData.Level = initData.Level;
        Position = new Vector3(initData.X, initData.Y, initData.Z);
        Direction = new Vector3(initData.DirX, 0, initData.DirZ);
        StartAttributeSync();
        InitTableData();
        InitAnimation();
        InitStateMachine();
        InitNavMeshAgent();
        SetMoveSpeed(data.MoveSpeed);
        if (mDead)
        {
            DoDie();
        }
        else if (data.IsMoving)
        {
            mTargetPos = data.TargetPos;
            DoMove();
        }
        else
        {
            if (CharModelRecord.RefreshAnimation == -1)
            {
                DoIdle();
            }
            else if (CharModelRecord.RefreshAnimation == 0 && initData.Reason == ReasonType.Born)
            {
                DoBorn();
            }
            else
            {
                DoIdle();
            }
        }

        LoadResourceAction = () =>
        {
            if (State == ObjState.Deleted)
            {
                return;
            }
            State = ObjState.LoadingResource;
            LoadModelAsync(() =>
            {
                State = ObjState.Normal;
                CreateNameBoard();
                if (!UsingFakeModel)
                {
                    InitShadow();
                    InitEquip();
                    RefreshAnimation();
                }

                if (null != callback)
                {
                    callback();
                }
            });
        };

        return true;
    }

    //加载资源
    protected virtual void LoadModelAsync(Action callback = null)
    {
        if (!this)
            return;

        if (null != mModel)
        {
            ComplexObjectPool.Release(mModel);
            mModel = null;
        }

        CharModelRecord tableModel;
        if (UsingFakeModel)
        {
            tableModel = Table.GetCharModel(97);
        }
        else
        {
            tableModel = Table.GetCharModel(ModelId);
        }
        var modelPath = Resource.GetModelPath(tableModel.ResPath);

        UniqueResourceId = GetNextUniqueResourceId();
        var resId = UniqueResourceId;
        if (null != mActorAvatar)
        {
            mActorAvatar.Layer = gameObject.layer;
            mActorAvatar.LayerMask = LayerMask.GetMask(GAMELAYER.Collider, GAMELAYER.IgnoreShadow);
        }
        ComplexObjectPool.NewObject(modelPath, model =>
        {
            if (resId != UniqueResourceId)
            {
                Logger.Debug("Obj[{0}]  LoadModelAsync[resId != UniqueResourceId]", mObjId);
                return;
            }

            if (State == ObjState.Deleted)
            {
                ComplexObjectPool.Release(model);
                return;
            }

            SetModel(model);
            mAnimationController.Animation = mModel.animation;
            mAnimationController.Animation.Stop();
            SetScale((float) tableModel.Scale);

            //缓存top挂载点位置，为了后面装在翅膀偏移用
            var topMount = GetMountPoint((int) MountPoint.Top);
            if (null != topMount)
            {
                mTopMountPointPos = topMount.localPosition;
            }

            //最后再掉Callback
            if (null != callback)
            {
                callback();
            }
        }, null, o =>
        {
            // 老的模型不用隐藏renderer,因为动作都已经加载好了
            if (o.GetComponentsInChildren<Animation>().Length > 0)
            {
                OptList<Renderer>.List.Clear();
                o.GetComponentsInChildren(OptList<Renderer>.List);
                {
                    var __array3 = OptList<Renderer>.List;
                    var __arrayLength3 = __array3.Count;
                    for (var __i3 = 0; __i3 < __arrayLength3; ++__i3)
                    {
                        var renderer = __array3[__i3];
                        {
                            renderer.enabled = false;
                        }
                    }
                }
            }
        });
    }

    public void PrepareAnimation(int animId)
    {
        mAnimationController = gameObject.GetComponent<AnimationController>();
        {
            // foreach(var equip in EquipList)
            var __enumerator4 = (EquipList).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var equip = __enumerator4.Current;
                {
                    try
                    {
                        InitAnimationPathByEquip(equip.Key, -1, equip.Value);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                }
            }
        }

        mAnimationController.PrepareAnimationClip(animId);
    }

    public virtual void InitEquip(bool sync = false, bool firstPriority = false)
    {
        if (UsingFakeModel)
        {
            return;
        }

        {
            // foreach(var equip in EquipList)
            var __enumerator4 = (EquipList).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var equip = __enumerator4.Current;
                {
                    try
                    {
                        ChangeEquipModel(equip.Key, -1, equip.Value, sync, firstPriority);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                }
            }
        }
    }

    //重置
    public override void Reset()
    {
        base.Reset();
        mDead = false;
        DeleteObjTime = 0;
        mCurrentSkillId = -1;
        AreaState = eAreaState.City;
        CharacterBaseData = new CharacterBaseDataModel();
        mBuff.Reset();
        mMountPoints.Clear();
        mTargetPos.Clear();
        mMountPoints.Clear();
        mForceMoveRemindTime = 0;
        DelayedMove = null;
        var collider = gameObject.GetComponent<Collider>();
        if (null != collider)
        {
            collider.enabled = true;
        }
        mCharaModelID = -1;
        var mod = mObjId%10 + 1;
        DelayPlaySoundTime(mod*10 + Random.Range(0.0f, 10.0f));
        OnWingLoadedCallback = null;
    }

    //移除所有buff
    public virtual void RemoveAllBuff()
    {
        mBuff.RemoveAllBuff();
    }

    public BuffManager GetBuffManager()
    {
        return mBuff;
    }

    //基本逻辑
    protected override void Tick(float delta)
    {
        if (State == ObjState.LogicDataInited)
        {
            if (ObjManager.Instance.CanLoad() && LoadResourceAction != null)
            {
                LoadResourceAction();
            }
        }

        Direction = Vector3.RotateTowards(Direction, TargetDirection, delta*mAngularSpeed, 0);

        if (GetCurrentStateName() != OBJ.CHARACTER_STATE.ATTACK && HasWaitingSkill())
        {
            UseWaitingSkill();
        }

        if (null != mBuff)
        {
            mBuff.Update();
        }

        TickForceMove(delta);
        TickPlayIdleSound(delta);

        TickMiniMap(delta);
    }

    protected int mColliderLayer = LayerMask.GetMask(GAMELAYER.Collider);

    protected virtual void TickForceMove(float delta)
    {
        if (mForceMoveRemindTime <= 0)
        {
            return;
        }

        var nav = GetNavMeshAgent();
        if (!nav.enabled)
        {
            nav = null;
        }

        if (mForceMoveRemindTime - delta < 0)
        {
            delta = mForceMoveRemindTime;
            mForceMoveRemindTime = 0;
        }
        else
        {
            mForceMoveRemindTime -= delta;
        }

        var x = Position.x + mForceMoveSpeed.x*delta;
        var z = Position.z + mForceMoveSpeed.y*delta;
        var target = GameLogic.GetTerrainPosition(x, z);

        if (null != nav)
        {
            nav.Warp(target);
        }
        else
        {
            Position = target;
        }
    }

    protected virtual void TickPlayIdleSound(float delta)
    {
        if (null != CharModelRecord && -1 != CharModelRecord.RandomStand)
        {
            if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN ||
                GetCurrentStateName() == OBJ.CHARACTER_STATE.IDLE)
            {
                if (NextPlayIdleSoundTime <= 0)
                {
                    DelayPlaySoundTime();
                    SoundManager.Instance.PlaySoundEffect(CharModelRecord.RandomStand);
                }
                else
                {
                    NextPlayIdleSoundTime -= delta;
                }
            }
        }
    }

    protected virtual void TickMiniMap(float delta)
    {
        if (ObjManager.Instance.MyPlayer == null || this == ObjManager.Instance.MyPlayer)
            return;

        miniMapTime += delta;
        if (miniMapTime >= MiniMapInterval)
        {
            if (!Dead && (Position - ObjManager.Instance.MyPlayer.Position).sqrMagnitude <= (5 * 5 * 5))
            { // 显示
                if (!ShowInMiniMap)
                {
                    ShowInMiniMap = true;
                    EventDispatcher.Instance.DispatchEvent(new ShowCharacterInMinimap(ShowInMiniMap, mObjId));
                }
            }
            else
            { // 不显示
                if (ShowInMiniMap)
                {
                    ShowInMiniMap = false;
                    EventDispatcher.Instance.DispatchEvent(new ShowCharacterInMinimap(ShowInMiniMap, mObjId));
                }
            }
            miniMapTime = 0.0f;
        }
    }

    public void DelayPlaySoundTime(float t = 0)
    {
        if (t > 0)
        {
            NextPlayIdleSoundTime = t;
        }
        else
        {
            NextPlayIdleSoundTime = Random.Range(40.0f, 80.0f);
        }
    }

    #endregion

    #region 自定义接口	

    public virtual bool IsPlaySpecialIdleAnimation()
    {
        return mSpecialIdleAnimationId.Count > 0;
    }

    public virtual bool IsInSafeArea()
    {
        return eAreaState.City == AreaState;
    }

    #endregion

    #region 状态机相关

    //进入移动状态
    public void DoMove()
    {
        mStateMachine.SendEvent(OBJ.STATEMACHINE_EVENT.RUN);
    }

    public void DoIdle()
    {
        mStateMachine.SendEvent(OBJ.STATEMACHINE_EVENT.IDLE);
    }

    public void DoBorn()
    {
        mStateMachine.SendEvent(OBJ.STATEMACHINE_EVENT.BORN);
    }

    public void DoAttack()
    {
        mStateMachine.SendEvent(OBJ.STATEMACHINE_EVENT.ATTACK);
    }

    public void DoDizzy()
    {
        mStateMachine.SendEvent(OBJ.STATEMACHINE_EVENT.DIZZY);
    }

    public void DoHurt(bool bStopMove = true)
    {
        if (mDead)
        {
            return;
        }

        var TableCharacterHitEffectIdLength0 = TableCharacter.HitEffectId.Length;
        for (var i = 0; i < TableCharacterHitEffectIdLength0; i++)
        {
            var id = TableCharacter.HitEffectId[i];
            if (-1 != id)
            {
                var tableData = Table.GetEffect(id);
                EffectManager.Instance.CreateEffect(tableData, this, null, null, null,
                    (tableData.BroadcastType == 0 && GetObjType() == OBJ.TYPE.MYPLAYER) || tableData.BroadcastType == 1);
            }
        }

        mStateMachine.SendEvent(OBJ.STATEMACHINE_EVENT.HURT);
    }

    public void DelayDie(float delay)
    {
        Dead = true;
        StartCoroutine(DieCorotinue(delay));
    }

    private IEnumerator DieCorotinue(float delay)
    {
        yield return new WaitForSeconds(delay);
        DoDie();
    }

    public virtual void DoDie()
    {
        Dead = true;
        mStateMachine.SendEvent(OBJ.STATEMACHINE_EVENT.DIE);

        {
            var __list2 = mDestroyObjectsWhenDie;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var destroyObject = __list2[__i2];
                {
                    Destroy(destroyObject);
                }
            }
        }

        if (-1 != CharModelRecord.DieEffectId)
        {
            var tableData = Table.GetEffect(CharModelRecord.DieEffectId);
            EffectManager.Instance.CreateEffect(tableData, this, null, null, null,
                (tableData.BroadcastType == 0 && GetObjType() == OBJ.TYPE.MYPLAYER) || tableData.BroadcastType == 1);
        }

        if (null != CharModelRecord)
        {
            if (-1 != CharModelRecord.DeadSound)
            {
                var pos = ObjManager.Instance.MyPlayer.Position;
                SoundManager.Instance.PlaySoundEffectAtPos2(CharModelRecord.DeadSound, Position, pos);
            }
        }

        //删除身上的buff
        mBuff.RemoveBuffWhenDie();
    }

    public virtual void Relive()
    {
        mDead = false;
        DeleteObjTime = 0;
        mCurrentSkillId = -1;
        mForceMoveRemindTime = 0;
        RemoveAllBuff();
        if (null != collider)
        {
            collider.enabled = true;
        }
        StopMove();
        DoIdle();
    }

    #endregion

    #region 角色行为

    public virtual void FaceTo(Vector3 vec)
    {
        var dif = vec - Position;
        dif.Normalize();
        dif.y = 0;
        TargetDirection = dif;
    }

    public virtual void SetMoveSpeed(float speed)
    {
        var nav = GetNavMeshAgent();
        if (null != nav)
        {
            nav.speed = speed;
        }
    }

    public float GetRawMoveSpeed()
    {
        var nav = GetNavMeshAgent();
        if (null != nav)
        {
            return nav.speed;
        }
        return 0.0f;
    }


    public float GetMoveSpeed()
    {
        var nav = GetNavMeshAgent();
        if (null != nav)
        {
            return nav.speed*MoveSpeedAdjust;
        }
        return 0.0f;
    }

    public virtual bool MoveTo(Vector3 vec, float offset = 0.05f, bool isSendFastReach = false)
    {
        if (GetMoveSpeed() <= 0)
        {
            return false;
        }
        vec.y = GameLogic.GetTerrainHeight(vec.x, vec.z);
        TargetPos.Clear();
        vec -= (vec - Position).normalized*offset;
        TargetPos.Add(vec);
        DoMove();

        return true;
    }

    public virtual bool MoveTo(List<Vector3> vec, float offset = 0.05f)
    {
        if (GetMoveSpeed() <= 0)
        {
            return false;
        }

        var vecCount1 = vec.Count;
        for (var i = 0; i < vecCount1; ++i)
        {
            var v = vec[i];
            vec[i] = GameLogic.GetTerrainPosition(v.x, v.z);
        }
        TargetPos.Clear();

        if (PathBackward(offset, vec))
        {
            return true;
        }

        TargetPos.AddRange(vec);
        DoMove();

        return true;
    }


    protected bool PathBackward(float offset, List<Vector3> vec)
    {
        if (!this)
        {
            return false;
        }

        while (offset > 0 && vec.Count > 1)
        {
            var l = (vec[vec.Count - 1] - vec[vec.Count - 2]).magnitude;
            if (l < offset)
            {
                vec.RemoveAt(vec.Count - 1);
                offset -= l;
            }
            else
            {
                var p = vec[vec.Count - 1];
                vec[vec.Count - 1] -= (p - vec[vec.Count - 2]).normalized*offset;
                offset = -1;
                break;
            }
        }

        if (offset > 0)
        {
            if (vec.Count == 1)
            {
                if (Vector3.Distance(vec[0], Position) < offset + 0.01f)
                {
                    return true;
                }
                vec[0] -= (vec[0] - Position).normalized*offset;
            }
            else // == 0
            {
                return true;
            }
        }

        return false;
    }

    public virtual void OnMoveOver()
    {
        DoIdle();
    }

    public virtual void StopMove()
    {
        mTargetPos.Clear();
        if (GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
        {
            DoIdle();
        }
    }

    #endregion

    #region 逻辑接口

    public virtual bool IsMyEnemy(ObjCharacter obj)
    {
        return false;
    }

    private OBJ.CharacterAnimationState mAnimationState = OBJ.CharacterAnimationState.Normal;

    public OBJ.CharacterAnimationState AnimationState
    {
        get { return mAnimationState; }
        set { mAnimationState = value; }
    }

    //public OBJ.CharacterAnimationState AnimationState
    //{
    //    get
    //    {
    //        if (GetObjType() != OBJ.TYPE.MYPLAYER && GetObjType() != OBJ.TYPE.OTHERPLAYER)
    //        {
    //            return OBJ.CharacterAnimationState.Normal;
    //        }

    //        if (GetObjType() == OBJ.TYPE.OTHERPLAYER)
    //        {
    //            return OBJ.CharacterAnimationState.Fly;
    //        }

    //        if (UIManager.Instance.IsAttackState)
    //        {
    //            if (PlayerDataManager.Instance != null && 
    //                PlayerDataManager.Instance.GetWingId() == -1)
    //            {
    //                return OBJ.CharacterAnimationState.Attack;
    //            }
    //            else
    //            {
    //                return OBJ.CharacterAnimationState.Fly;
    //            }
    //        }

    //        return OBJ.CharacterAnimationState.Normal;
    //    }
    //}

    #endregion

    #region 换装

    private static readonly Dictionary<int, string> Part2Name = new Dictionary<int, string>
    {
        {7, "Head"},
        {11, "Chest"},
        {14, "Hand"},
        {15, "Leg"},
        {16, "Foot"}
    };

    public void InitAnimationPathByEquip(int part, int oldItemTypeId, int itemTypeId)
    {
        if (itemTypeId > 0)
        {
            itemTypeId = itemTypeId/100;
        }
        if (oldItemTypeId > 0)
        {
            oldItemTypeId = oldItemTypeId/100;
        }
        try
        {
            if (mActorAvatar == null)
            {
                return;
            }

            EquipBaseRecord equipInfo = null;
            if (-1 == itemTypeId)
            {
                if (oldItemTypeId != -1)
                {
                    equipInfo = Table.GetEquipBase(oldItemTypeId);
                }
                else
                {
                    return;
                }
            }
            else
            {
                equipInfo = Table.GetEquipBase(itemTypeId);
            }

            if (null == equipInfo)
            {
                Logger.Debug("null==equipInfo oldItemTypeId=[{0}],itemTypeId=[{1}]", oldItemTypeId, itemTypeId);
                return;
            }

            var mountInfo = Table.GetWeaponMount(equipInfo.EquipModel);
            if (mountInfo == null)
            {
                return;
            }

            // 主手
            if (part == 17)
            {
                int vicePartTypeId;
                var path = string.Empty;
                if (EquipList.TryGetValue(18, out vicePartTypeId))
                {
                    vicePartTypeId = vicePartTypeId/100;
                    var vicePartEquipInfo = Table.GetEquipBase(vicePartTypeId);
                    if (itemTypeId == -1)
                    {
                        path = vicePartEquipInfo.NullHandPath;
                    }
                    else
                    {
                        path = vicePartEquipInfo.ViceHandPath;
                    }
                }

                if (itemTypeId == -1)
                {
                    mAnimationController.InitPath(path, string.Empty, CharModelRecord.AnimPath);
                    return;
                }

                // 如果表里配的挂载点是-1，则按部位挂载
                if (mountInfo.Mount == -1)
                {
                    mAnimationController.InitPath(equipInfo.AnimPath, path, CharModelRecord.AnimPath);
                }
                else
                {
                    mAnimationController.InitPath(equipInfo.AnimPath, path, CharModelRecord.AnimPath);
                }
            }
            // 副手
            else if (part == 18)
            {
                // 获得主手武器的信息
                int mainPartTypeId;
                var path = string.Empty;
                if (EquipList.TryGetValue(17, out mainPartTypeId))
                {
                    mainPartTypeId = mainPartTypeId/100;
                    var mainPartEquipInfo = Table.GetEquipBase(mainPartTypeId);
                    path = mainPartEquipInfo.AnimPath;
                }
                else if (itemTypeId != -1) // 副手不为空
                {
                    path = equipInfo.NullHandPath;
                }

                if (itemTypeId == -1)
                {
                    mAnimationController.InitPath(path, string.Empty, CharModelRecord.AnimPath);
                    return;
                }

                // 如果表里配的挂载点是-1，则按部位挂载
                if (mountInfo.Mount == -1)
                {
                    mAnimationController.InitPath(path, equipInfo.ViceHandPath, CharModelRecord.AnimPath);
                }
                else
                {
                    mAnimationController.InitPath(path, equipInfo.ViceHandPath, CharModelRecord.AnimPath);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }

    public void ChangeEquip(int part, int itemTypeId)
    {
        var oldItemTypeId = -1;
        if (itemTypeId == -1)
        {
            EquipList.TryGetValue(part, out oldItemTypeId);
            EquipList.Remove(part);
        }
        else
        {
            if (EquipList.TryGetValue(part, out oldItemTypeId))
            {
                if (EquipList[part] == itemTypeId)
                {
                    return;
                }

                EquipList[part] = itemTypeId;
            }
            else
            {
                EquipList.Add(part, itemTypeId);
            }
        }

        if (UsingFakeModel)
        {
            return;
        }

        ChangeEquipModel(part, oldItemTypeId, itemTypeId);
    }

    public void ChangeEquipModel(int part,
                                 int oldItemTypeId,
                                 int itemTypeId,
                                 bool sync = false,
                                 bool firstPriority = false)
    {
        var EnchanceLevel = 0;
        if (itemTypeId > 0)
        {
            EnchanceLevel = itemTypeId%100;
            if (EnchanceLevel < 0 || EnchanceLevel > 15)
            {
                EnchanceLevel = 10;
            }
            itemTypeId = itemTypeId/100;
        }
        if (oldItemTypeId > 0)
        {
            oldItemTypeId = oldItemTypeId/100;
        }
        try
        {
            if (mActorAvatar == null)
            {
                return;
            }

            EquipBaseRecord equipInfo = null;
            if (-1 == itemTypeId)
            {
                if (oldItemTypeId != -1)
                {
                    equipInfo = Table.GetEquipBase(oldItemTypeId);
                }
                else
                {
                    return;
                }
            }
            else
            {
                equipInfo = Table.GetEquipBase(itemTypeId);
            }

            if (null == equipInfo)
            {
                Logger.Debug("null==equipInfo oldItemTypeId=[{0}],itemTypeId=[{1}]", oldItemTypeId, itemTypeId);
                return;
            }

            string value;
            if (Part2Name.TryGetValue(part, out value))
            {
                if (itemTypeId == -1)
                {
                    mActorAvatar.ChangePart(value, string.Empty, null, sync);
                    return;
                }

                var mountInfoPart = Table.GetWeaponMount(equipInfo.EquipModel);
                if (mountInfoPart == null)
                {
                    return;
                }

                var viewRecordPart = Table.GetEquipModelView(mountInfoPart.Enchance[EnchanceLevel]);

                var change = true;
                if (-1 != mCharaModelID)
                {
                    if (part != 17 && part != 18 && part != 12)
                    {
                        change = false;
                    }
                }

                if (change)
                {
                    mActorAvatar.ChangePart(value, mountInfoPart.Path, viewRecordPart, sync);
                }


                return;
            }

            var mountInfo = Table.GetWeaponMount(equipInfo.EquipModel);
            if (mountInfo == null)
            {
                return;
            }

            var viewRecord = Table.GetEquipModelView(mountInfo.Enchance[EnchanceLevel]);

            // 主手
            if (part == 17)
            {
                int vicePartTypeId;
                var path = string.Empty;
                if (EquipList.TryGetValue(18, out vicePartTypeId))
                {
                    vicePartTypeId = vicePartTypeId/100;
                    var vicePartEquipInfo = Table.GetEquipBase(vicePartTypeId);
                    if (itemTypeId == -1)
                    {
                        path = vicePartEquipInfo.NullHandPath;
                    }
                    else
                    {
                        path = vicePartEquipInfo.ViceHandPath;
                    }
                }

                if (itemTypeId == -1)
                {
                    mActorAvatar.MountWeapon((MountPoint) mountInfo.Mount, string.Empty, mountInfo, viewRecord, part,
                        sync);
                    mAnimationController.Init(path, string.Empty, CharModelRecord.AnimPath, false, sync, firstPriority);
                    return;
                }

                // 如果表里配的挂载点是-1，则按部位挂载
                if (mountInfo.Mount == -1)
                {
                    mActorAvatar.MountWeapon(MountPoint.RightWeapen, mountInfo.Path, mountInfo, viewRecord, part, sync);
                    mAnimationController.Init(equipInfo.AnimPath, path, CharModelRecord.AnimPath, false, sync,
                        firstPriority);
                }
                else
                {
                    mActorAvatar.MountWeapon((MountPoint) mountInfo.Mount, mountInfo.Path, mountInfo, viewRecord, part,
                        sync);
                    mAnimationController.Init(equipInfo.AnimPath, path, CharModelRecord.AnimPath, false, sync,
                        firstPriority);
                }
            }
            // 副手
            else if (part == 18)
            {
                // 获得主手武器的信息
                int mainPartTypeId;
                var path = string.Empty;
                if (EquipList.TryGetValue(17, out mainPartTypeId))
                {
                    mainPartTypeId = mainPartTypeId/100;
                    var mainPartEquipInfo = Table.GetEquipBase(mainPartTypeId);
                    path = mainPartEquipInfo.AnimPath;
                }
                else if (itemTypeId != -1) // 副手不为空
                {
                    path = equipInfo.NullHandPath;
                }

                if (itemTypeId == -1)
                {
                    mActorAvatar.MountWeapon((MountPoint) mountInfo.Mount, string.Empty, mountInfo, viewRecord, part,
                        sync);
                    mAnimationController.Init(path, string.Empty, CharModelRecord.AnimPath, false, sync, firstPriority);
                    return;
                }

                // 如果表里配的挂载点是-1，则按部位挂载
                if (mountInfo.Mount == -1)
                {
                    mActorAvatar.MountWeapon(MountPoint.LeftWeapen, mountInfo.Path, mountInfo, viewRecord, part, sync);
                    mAnimationController.Init(path, equipInfo.ViceHandPath, CharModelRecord.AnimPath, false, sync,
                        firstPriority);
                }
                else
                {
                    mActorAvatar.MountWeapon((MountPoint) mountInfo.Mount, mountInfo.Path, mountInfo, viewRecord, part,
                        sync);
                    mAnimationController.Init(path, equipInfo.ViceHandPath, CharModelRecord.AnimPath, false, sync,
                        firstPriority);
                }
            }
            // 翅膀
            else if (part == 12)
            {
                if (itemTypeId == -1)
                {
                    mActorAvatar.MountWeapon((MountPoint) mountInfo.Mount, string.Empty, mountInfo, viewRecord, part,
                        sync);
                    RestoreTopMountPositon();
                    return;
                }

                mActorAvatar.MountWeapon((MountPoint) mountInfo.Mount, mountInfo.Path, mountInfo, viewRecord, part, sync,
                    o =>
                    {
                        var path = equipInfo.AnimPath;
                        if (!path.EndsWith("/"))
                        {
                            path += "/";
                        }

                        ResourceManager.PrepareResource<AnimationClip, AnimationClip, AnimationClip>(
                            path + "FlyIdle.anim", path + "FlyMove.anim", path + "Dead.anim",
                            (flyIdle, flyMove, flyDead) =>
                            {
                                if (o == null || mActorAvatar == null)
                                {
                                    return;
                                }

                                flyIdle.wrapMode = WrapMode.Loop;
                                flyMove.wrapMode = WrapMode.Loop;
                                flyDead.wrapMode = WrapMode.Clamp;

                                var anim = o.GetComponentInChildren<Animation>();
                                if (anim)
                                {
                                    anim.AddClip(flyIdle, flyIdle.name);
                                    anim.AddClip(flyMove, flyMove.name);
                                    anim.AddClip(flyDead, flyDead.name);
                                    anim.clip = flyIdle;
                                    anim.Play(flyIdle.name);
                                }

                                mActorAvatar.WingGameObject = o;
                                if (null != OnWingLoadedCallback)
                                {
                                    OnWingLoadedCallback(o);
                                }
                                // 特殊处理一下翅膀的影子
                                // 在UI里的时候，不需要处理，都改成UI
                                // 不在UI里的时候，特效处理成IgnoreShadow
                                if (mActorAvatar.LayerMask != 0)
                                {
                                    var layer = gameObject.layer;
                                    var ignoreShadow = LayerMask.NameToLayer("IgnoreShadow");
                                    // 特效不需要产生影子
                                    {
                                        OptList<Transform>.List.Clear();
                                        o.GetComponentsInChildren(OptList<Transform>.List);
                                        var __array1 = OptList<Transform>.List;
                                        var __arrayLength1 = __array1.Count;
                                        for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                                        {
                                            var trans = __array1[__i1];
                                            {
                                                if (trans.GetComponent<ParticleSystem>() ||
                                                    trans.gameObject.layer == ignoreShadow)
                                                {
                                                    trans.gameObject.layer = ignoreShadow;
                                                }
                                                else
                                                {
                                                    trans.gameObject.layer = layer;
                                                }
                                            }
                                        }
                                    }
                                }

                                mActorAvatar.WingGameObject.SetRenderQueue(mRenderQueue);

                                if (null != CharModelRecord)
                                {
                                    OffsetTopMountPositon(new Vector3(0, CharModelRecord.WingTop*0.001f, 0));
                                        //Top挂载点偏移量，毫米
                                }
                            });
                    });
            }
            else
            {
                if (-1 != mCharaModelID)
                {
                    return;
                }
                if (itemTypeId == -1)
                {
                    mActorAvatar.MountWeapon((MountPoint) mountInfo.Mount, string.Empty, mountInfo, viewRecord, part,
                        sync);
                    return;
                }

                mActorAvatar.MountWeapon((MountPoint) mountInfo.Mount, mountInfo.Path, mountInfo, viewRecord, part, sync);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }

    public Transform GetMountPoint(int mount)
    {
        if (mModel == null)
        {
            return null;
        }

        Transform t;
        if (mMountPoints.TryGetValue(mount, out t))
        {
            return t;
        }

        if (mount < 5)
        {
            t = FindChildTransform(mTransform, MountPointName[mount]);
        }
        else
        {
            t = ObjTransform.Find(Table.GetAttachPoint(mount).Path);
        }

        if (t != null)
        {
            mMountPoints[mount] = t;
        }

        return t;
    }

    public Transform FindChildTransform(Transform t, string name)
    {
        var o = t.Find(name);
        if (o != null)
        {
            return o;
        }
        var tchildCount2 = t.childCount;
        for (var i = 0; i < tchildCount2; i++)
        {
            var child = t.GetChild(i);
            var bone = FindChildTransform(child, name);
            if (bone != null)
            {
                return bone;
            }
        }

        return null;
    }

    #endregion

    #region 技能

    public bool IsInFrontOfMe(Vector3 p, float distance = 0, float angleRange = 90.0f)
    {
        var offset = p - Position;
        if (distance > 0)
        {
            if (offset.magnitude > distance)
            {
                return false;
            }
        }
        var f = Vector3.Angle(Direction, offset.normalized);
        if (f <= angleRange)
        {
            return true;
        }

        return false;
    }

    private readonly Queue<KeyValuePair<int, List<ulong>>> mQueue = new Queue<KeyValuePair<int, List<ulong>>>();
    private int mCurrentSkillId;
    private SkillRecord mCurrentSkillData;
    private List<ulong> mTargetIdList;

    public SkillRecord GetCurrentSkillData()
    {
        return mCurrentSkillData;
    }

    public void AddWaitingSkill(int skillId, List<ulong> targetList)
    {
        mQueue.Enqueue(new KeyValuePair<int, List<ulong>>(skillId, targetList));
    }

    public bool HasWaitingSkill()
    {
        return mQueue.Count > 0;
    }

    public void UseWaitingSkill()
    {
        var v = mQueue.Dequeue();
        var skillData = Table.GetSkill(v.Key);
        if (null == skillData)
        {
            Logger.Fatal("Can't find skill[{0}]", v.Key);
            return;
        }

        UseSkill(skillData, v.Value);
    }

    public virtual void UseSkill(SkillRecord skillData, List<ulong> targetId)
    {
        mCurrentSkillId = skillData.Id;
        mCurrentSkillData = skillData;
        mTargetIdList = targetId;

		if (-1 != skillData.ActionId)
		{
			DoAttack();
		}
        
        if (-1 != skillData.SkillSound)
        {
            var listenerPos = Position;
            var player = ObjManager.Instance.MyPlayer;
            if (null != player)
            {
                listenerPos = player.Position;
            }

            var isSelf = GetObjType() == OBJ.TYPE.MYPLAYER;
            SoundManager.Instance.PlaySoundEffectAtPos2(skillData.SkillSound, Position, listenerPos, 0, isSelf);
        }

        var id = skillData.Effect;
        if (TypeDefine.INVALID_ID != id)
        {
            var tableData = Table.GetEffect(id);
            EffectManager.Instance.CreateEffect(tableData, this, null, null, null,
                (tableData.BroadcastType == 0 && GetObjType() == OBJ.TYPE.MYPLAYER) || tableData.BroadcastType == 1);
        }
    }

    public bool IsUsingSkill()
    {
        return mCurrentSkillId != -1;
    }

    public void PrepareSkillResources(int skillId)
    {
        if (null == mAnimationController)
        {
            return;
        }

        var tbSkill = Table.GetSkill(skillId);
        if (null == tbSkill)
        {
            return;
        }

        mAnimationController.PrePareAnimationResources(skillId);
    }

    //添加buff
    public virtual void AddBuff(uint buffId, int buffTypeId, ulong caster, ulong target, bool showFireEffect = true)
    {
        var buff = mBuff.AddBuff(buffId, buffTypeId, caster, target, showFireEffect);
        if (null == buff)
        {
            return;
        }

#if UNITY_EDITOR
        if (LogBuff)
        {
            if (GetObjType() == OBJ.TYPE.MYPLAYER)
            {
                Logger.Info("ADD BUFF : Name[{0}] id[{1}] ", gameObject.name, buff.BuffId);
            }
        }
#endif

        OnAddBuff(buff);
    }

    protected virtual void OnAddBuff(Buff buff)
    {
    }

    //移除buff
    public virtual void RemoveBuff(uint buffId)
    {
        if (mBuff.RemoveBuff(buffId))
        {
#if UNITY_EDITOR
            if (LogBuff)
            {
                if (GetObjType() == OBJ.TYPE.MYPLAYER)
                {
                    Logger.Info("REMOVE BUFF :Name[{0}] id[{1}]", gameObject.name, buffId);
                }
            }
#endif
        }
    }

    public Buff GetBuff(uint buffid)
    {
        return mBuff.GetBuff(buffid);
    }

    public virtual void ShowDamage(BuffResult result)
    {
        var e = new ShowDamageBoardEvent(Position, result);
        EventDispatcher.Instance.DispatchEvent(e);

        var targetCharacter = ObjManager.Instance.FindCharacterById(result.TargetObjId);

        if (result.Type == BuffType.HT_NORMAL
            || result.Type == BuffType.HT_NORMAL
            || result.Type == BuffType.HT_REBOUND)
        {
            if (targetCharacter == ObjManager.Instance.MyPlayer)
            {
                if (result.Param.Count >= 2)
                {
                    PlayerDataManager.Instance.ChangeAttributeHp(result.Param[1]);
                }
            }
            else if (GameControl.Instance != null && GameControl.Instance.TargetObj == targetCharacter)
            {
                if (result.Param.Count >= 2)
                {
                    ChangeAttributeHp(result.Param[1]);
                }
            }
        }
        else if (result.Type == BuffType.HT_HEALTH)
        {
            if (targetCharacter == ObjManager.Instance.MyPlayer)
            {
                if (result.Param.Count >= 2)
                {
                    PlayerDataManager.Instance.ChangeAttributeHp(result.Param[1]);
                }
            }
            else if (GameControl.Instance != null && GameControl.Instance.TargetObj == targetCharacter)
            {
                if (result.Param.Count >= 2)
                {
                    ChangeAttributeHp(result.Param[1]);
                }
            }
        }
    }

    #endregion
}


public static class VectorExtension
{
    public static Vector2 xy(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector2 xz(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}