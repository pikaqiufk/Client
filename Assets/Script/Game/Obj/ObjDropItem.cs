#region using

using System;
using System.Collections;
using DataTable;
using EventSystem;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class ObjDropItem : ObjBase
{
    public const float MAXSPEED = 20;
    private static int TerrainLayerMask = -1;
    [Tooltip("速度增量")] [Range(0, 2)] public float AccSpeed = 0.1f;
    [Tooltip("控制点离我的直线距离")] [Range(8, 10)] public float ControlPointOffsetDistanceMax = 8.0f;
    [Tooltip("控制点离我的直线距离")] [Range(5, 8)] public float ControlPointOffsetDistanceMin = 5.0f;
    [Tooltip("控制点高度(最高)")] [Range(2, 5)] public float ControlPointOffsetHeightMax = 4.0f;
    [Tooltip("控制点高度(最低)")] [Range(1, 2)] public float ControlPointOffsetHeightMin = 2.0f;
    [Tooltip("控制点速度,必须要大于物品速度")] [Range(15, 25)] public float ControlPointSpeed = 15.0f;
    [Tooltip("多长时间后出现")] public float DelayTime = 0.5f;
    [Tooltip("掉落都地上高度")] public float DropHight = 0.5f;
    //[Tooltip("自动拾取距离")]
    //public float AutoPickUpDistance = 2.0f;

    [Tooltip("在地上呆的时间")] public float DropOverDelayTime = 0.2f;
    [Tooltip("掉落都地上用时")] public float DropTime = 0.3f;
    [Tooltip("距离多少是已经飞到玩家身上")] public float MaxDistance = 0.2f;
    //是否可以自动拾取
    private bool mCanAutoPickup;
    private DropItemController mController;
    //控制点
    private Vector3 mControlPoint;
    //控制点速度
    private float mControlPointMoveSpeed;
    //控制点
    private bool mControlPointReached;
    //特效
    private GameObject mEffect;
    //生命时间
    private DateTime mLifeTime = DateTime.Now;
    //移动速度
    private float mMoveSpeed;
    [Tooltip("物品速度")] [Range(0, 15)] public float MoveSpeed = 10.0f;
    //拥有者id
    private bool mOwnerIsMe;
    //拥有者拾取保护时间
    private DateTime mOwnerPickUpProtectionTime;
    //状态
    private DropItemState mState = DropItemState.Load;
    public DropItemState STATUS { get{return mState;}}
    //表格
    private ItemBaseRecord mTableData;
    //飞往目标点（也就是服务端实际坐标）
    private Vector3 mTargetPos;
    //外置表现参数
    [Tooltip("飞到玩家身上那个高度位置")] public float PlayerOffset = 1.4f;
    [Tooltip("开启安全模式(TotalLifeTime秒后强制删除)")] public bool SafeMode = true;
    [Tooltip("存在最长时间(秒)")] public float TotalLifeTime = 300;
    //挂机中是否已经尝试移动拾取,例如背包满的情况不能获得不会重复移动过去
    public bool HasAutoFightMove { get; set; }

    public virtual void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        base.Awake();

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }
    public bool IsMine()
    {
        return mOwnerIsMe;
    }
    public bool BagIsFull()
    {
        if(mTableData == null)
        {
            return true;
        }
        int bagId = mTableData.InitInBag;
        if (bagId >= 0 &&
            bagId < PlayerDataManager.Instance.PlayerDataModel.Bags.Bags.Count &&
            PlayerDataManager.Instance.PlayerDataModel.Bags.Bags[bagId].Capacity <= PlayerDataManager.Instance.PlayerDataModel.Bags.Bags[bagId].Size)
        {
            return true;
        }
        return false;
    }
    //是否可以被拾取
    public bool CanPickup()
    {
        if (DropItemState.StayOnGround != mState &&
            DropItemState.Pickup != mState)
        {
//只有停留在地上这种状态才能被拾取
            return false;
        }

        if (Game.Instance.ServerTime < mOwnerPickUpProtectionTime)
        {
//当前时间在拥有者保护时间内
            return mOwnerIsMe;
        }
        //过了拥有者保护时间谁都可以拾取
        return true;
    }

    //造品质特效
    private void CreateQualityEffect(string res, Action<GameObject> callback)
    {
        DestroyQualityEffect();

        ComplexObjectPool.NewObject(res, o =>
        {
            mEffect = o;
            var xform = o.transform;
            xform.parent = mModel.transform;
            xform.localPosition = Vector3.zero;
            xform.localRotation = Quaternion.identity;
            xform.localScale = Vector3.one;

            callback(mEffect);
        });
    }

    //掉落延迟
    private IEnumerator Delay()
    {
        mState = DropItemState.Delay;

        if (null != mModel)
        {
            mModel.SetActive(false);
        }
        yield return new WaitForSeconds(DelayTime);
        if (null != mModel)
        {
            mModel.SetActive(true);
        }

        DropToTargetPos();
    }

    public override void Destroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EquipBagNotFullChange.EVENT_TYPE, OnEquipRecycleSuccess);
        EventDispatcher.Instance.RemoveEventListener(UIEvent_PickSettingChanged.EVENT_TYPE, OnPickupSettingChanged);

        if (DropItemState.FlyToPlayer == mState)
        {
            return;
        }

        if (mEffect)
        {
            ComplexObjectPool.Release(mEffect);
            mEffect = null;
        }

        base.Destroy();
    }

    //删除品质特效
    private void DestroyQualityEffect()
    {
        if (mEffect)
        {
            ComplexObjectPool.Release(mEffect);
            mEffect = null;
        }
    }

    //飞到目标点结束
    private void DropOver()
    {
        StartCoroutine(DropOverStay());
    }

    //飞到目标点结束
    private IEnumerator DropOverStay()
    {
        // 当掉落落地时，特效
        if (mTableData != null && mTableData.Quality > 0 && !mEffect)
        {
            var resource = Table.GetClientConfig(600 + mTableData.Quality);

            if (!string.IsNullOrEmpty(resource.Value))
            {
                CreateQualityEffect(resource.Value, o =>
                {
                    if (DropItemState.StayOnGround != mState && DropItemState.Droping != mState)
                    {
                        ComplexObjectPool.Release(o);
                        mEffect = null;
                    }
                });
            }
        }

        yield return new WaitForSeconds(DropOverDelayTime);
        mState = DropItemState.StayOnGround;
        PlayDropSound();
    }

    //飞到目标点
    private void DropToTargetPos()
    {
        mState = DropItemState.Droping;

        var paths = new Vector3[3];
        paths[0] = Position;
        paths[1] = (mTargetPos + Position)/2;
        paths[1].y = Position.y + DropHight;
        paths[2] = mTargetPos;

        iTween.MoveTo(gameObject,
            iTween.Hash("path", paths, "movetopath", true, "orienttopath", false, "time", DropTime, "easetype",
                iTween.EaseType.linear, "oncomplete", "DropOver"));
    }

    //飞向玩家身上
    public void FlyToPlayer()
    {
        //先把特效关掉
        if (mEffect)
        {
            ComplexObjectPool.Release(mEffect);
            mEffect = null;
        }

        if (null != mController)
        {
            mController.ShowModel(false);
            mController.ShowEffect(true);
        }
        var myPos = ObjManager.Instance.MyPlayer.Position;
        var dir = (Position - myPos).normalized;

        mControlPoint = Position + dir*Random.Range(ControlPointOffsetDistanceMin, ControlPointOffsetDistanceMax);
        mControlPoint.y += Random.Range(ControlPointOffsetHeightMin, ControlPointOffsetHeightMax);
        mControlPointMoveSpeed = ControlPointSpeed;
        mMoveSpeed = MoveSpeed;
        mControlPointReached = false;

        mState = DropItemState.FlyToPlayer;
    }

    //类型
    public override OBJ.TYPE GetObjType()
    {
        return OBJ.TYPE.DROPITEM;
    }

    //初始化
    public override bool Init(InitBaseData initData, Action callback = null)
    {
        base.Init(initData);

        Reset();

        var data = initData as InitDropItemData;
        if (data == null)
        {
            return false;
        }
        mOwnerIsMe = false;
        mCanAutoPickup = true;
        HasAutoFightMove = false;
        var player = ObjManager.Instance.MyPlayer;
        if (player)
        {
            if (data.Owner.Contains(player.GetObjId()))
            {
                mOwnerIsMe = true;
                //mCanAutoPickup = true;
            }
            else if (data.Owner.Count <= 0)
            {
//新增，这种是谁都可以拾取的
                mOwnerIsMe = true;
            }
        }
        mOwnerPickUpProtectionTime = Game.Instance.ServerTime.AddSeconds(data.RemianSeconds);
        mLifeTime = DateTime.Now.AddSeconds(TotalLifeTime);

        //float height = GameLogic.Instance.Scene.GetTerrainHeight(data.TargetPos) + 0.1f;
        //mTargetPos = new Vector3(data.TargetPos.x, height, data.TargetPos.y);

        if (-1 == TerrainLayerMask)
        {
            TerrainLayerMask = LayerMask.GetMask(GAMELAYER.ShadowReceiver);
        }
        var ray = new Ray(new Vector3(data.TargetPos.x, 50, data.TargetPos.y), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 80, TerrainLayerMask))
        {
            mTargetPos = hit.point;
            mTargetPos.y += 0.1f;
        }
        else
        {
            var height = GameLogic.GetTerrainHeight(data.TargetPos.x, data.TargetPos.y) + 0.1f;
            mTargetPos = new Vector3(data.TargetPos.x, height, data.TargetPos.y);
        }

        LoadResourceAction = () =>
        {
            if (State == ObjState.Deleted)
            {
                return;
            }

            State = ObjState.LoadingResource;
            LoadRes(() =>
            {
                State = ObjState.Normal;
                if (true != data.PlayDrop)
                {
                    //不需要播放掉落动画
                    Position = mTargetPos;
                    mState = DropItemState.StayOnGround;

                    // 初始特效
                    if (mTableData != null && mTableData.Quality > 0 && !mEffect)
                    {
                        var resource = Table.GetClientConfig(600 + mTableData.Quality);

                        if (!string.IsNullOrEmpty(resource.Value))
                        {
                            CreateQualityEffect(resource.Value, o =>
                            {
                                if (DropItemState.StayOnGround != mState && DropItemState.Droping != mState)
                                {
                                    ComplexObjectPool.Release(o);
                                    mEffect = null;
                                }
                            });
                        }
                    }
                }
                else
                {
                    //需要播放掉落动画
                    StartCoroutine(Delay());
                    mState = DropItemState.Delay;
                }

                if (null != callback)
                {
                    callback();
                }

                mController = mModel.GetComponent<DropItemController>();
                if (null != mController)
                {
                    mController.ShowModel(true);
                    mController.ShowEffect(false);
                }
            });
        };

        EventDispatcher.Instance.AddEventListener(EquipBagNotFullChange.EVENT_TYPE, OnEquipRecycleSuccess);
        EventDispatcher.Instance.AddEventListener(UIEvent_PickSettingChanged.EVENT_TYPE, OnPickupSettingChanged);
        if(mOwnerIsMe)
        {
            ObjManager.Instance.MyPlayer.AutoCombat.AddDropPos(this);
        }
        return true;
    }

    protected virtual void LoadRes(Action callback = null)
    {
        var prafabPath = "";

        mTableData = Table.GetItemBase(mDataId);
        if (null == mTableData)
        {
            Logger.Error("NULL==Table.GetItemBase({0})", mDataId);
        }
        else
        {
            var tableDrop = Table.GetDropModel(mTableData.DropModel);
            if (null == tableDrop)
            {
                Logger.Error("NULL==Table.GetDropModel({0})", mTableData.DropModel);
            }
            else
            {
                prafabPath = tableDrop.ModelPath;
            }
        }

        if ("" == prafabPath)
        {
            Logger.Error("null==prafabPath)");
            return;
        }

        UniqueResourceId = GetNextUniqueResourceId();
        var resId = UniqueResourceId;
        ComplexObjectPool.NewObject(prafabPath, model =>
        {
            if (resId != UniqueResourceId)
            {
                return;
            }

            if (State == ObjState.Deleted)
            {
                ComplexObjectPool.Release(model);
                return;
            }
            SetModel(model);
            //SetScale((float)tableDrop.aScale); 这个缩放放到prefab上

            if (null != callback)
            {
                callback();
            }
        });
    }

    private void OnEquipRecycleSuccess(IEvent ievent)
    {
        if (DropItemState.Pickup == mState)
        {
            mState = DropItemState.StayOnGround;
            mCanAutoPickup = true;
        }
    }

    private void OnPickupSettingChanged(IEvent ievent)
    {
        if (DropItemState.StayOnGround == mState && false == mCanAutoPickup)
        {
            mCanAutoPickup = true;
        }
    }

    //拾取
    public void Pickup()
    {
        if (CanPickup())
        {
            NetManager.Instance.SendPickUpItemRequest(GetObjId());
            mState = DropItemState.Pickup;
        }
        mCanAutoPickup = false;
    }

    //播放掉落资源
    private void PlayDropSound()
    {
        var table = Table.GetItemBase(mDataId);
        if (null != table)
        {
            var tableDrop = Table.GetDropModel(table.DropModel);
            if (null != tableDrop)
            {
                SoundManager.Instance.PlaySoundEffect(tableDrop.SoundId);
            }
        }
    }

    //重置
    public override void Reset()
    {
        base.Reset();
        mState = DropItemState.Load;
        mLifeTime = DateTime.Now.AddSeconds(TotalLifeTime);
        mCanAutoPickup = false;
        mOwnerPickUpProtectionTime = Game.Instance.ServerTime;
    }

    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (State == ObjState.Deleted)
        {
            return;
        }

        if (State == ObjState.LogicDataInited)
        {
            if (ObjManager.Instance.CanLoad() && LoadResourceAction != null)
            {
                LoadResourceAction();
            }
        }

        if (SafeMode)
        {
            if (DateTime.Now > mLifeTime)
            {
                //为了安全，怎么也该消失了
                Logger.Warn("Drop item[{0}] stays too long", mObjId);
                mState = DropItemState.Destory;
                var obj = ObjManager.Instance.FindObjById(mObjId);
                if (null != obj && obj.Equals(this))
                {
                    ObjManager.Instance.RemoveObj(mObjId);
                }
                else
                {
                    base.Destroy();
                }
                return;
            }
        }


        var player = ObjManager.Instance.MyPlayer;
        if (null == player)
        {
            return;
        }

        if (DropItemState.StayOnGround == mState)
        {
            //如果是装备，装备包裹满了
            if (null != mTableData)
            {
                var type = Shared.CheckGeneral.GetItemType(mTableData.Id);
                if (eItemType.Equip == type)
                {
                    if (PlayerDataManager.Instance.GetRemaindCapacity(eBagType.Equip) <= 0)
                    {
                        if (PlayerDataManager.Instance.mPickIntervalTrigger == null)
                        {
                            PlayerDataManager.Instance.mPickIntervalTrigger =
                                TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime.AddSeconds(2f), () =>
                                {
                                    TimeManager.Instance.DeleteTrigger(PlayerDataManager.Instance.mPickIntervalTrigger);
                                    PlayerDataManager.Instance.mPickIntervalTrigger = null;
                                    //包裹已满提示
                                    var e = new ShowUIHintBoard(302);
                                    EventDispatcher.Instance.DispatchEvent(e);
                                });
                        }
                        return;
                    }
                }
                else if (eItemType.BaseItem == type)
                {
                    if (PlayerDataManager.Instance.GetRemaindCapacity(eBagType.BaseItem) <= 0)
                    {
                        if (PlayerDataManager.Instance.mPickIntervalTrigger == null)
                        {
                            PlayerDataManager.Instance.mPickIntervalTrigger =
                                TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime.AddSeconds(2f), () =>
                                {
                                    TimeManager.Instance.DeleteTrigger(PlayerDataManager.Instance.mPickIntervalTrigger);
                                    PlayerDataManager.Instance.mPickIntervalTrigger = null;
                                    //包裹已满提示
                                    var e = new ShowUIHintBoard(302);
                                    EventDispatcher.Instance.DispatchEvent(e);
                                });
                        }
                        return;
                    }
                }
            }

            //停在地上
            if (CanPickup() && mCanAutoPickup)
            {
                //可以自动拾取
                var distance = Vector2.Distance(player.Position.xz(), Position.xz());
                if (distance <= GameUtils.AutoPickUpDistance)
                {
                    //距离足够近
                    var control = UIManager.Instance.GetController(UIConfig.SettingUI);
                    if ((bool) control.CallFromOtherClass("CanPiackUpItem", new object[] {mDataId}))
                    {
                        //是否可拾取这种类型物品
                        Pickup();
                    }
                    mCanAutoPickup = false;
                }
            }
        }
        else if (DropItemState.FlyToPlayer == mState)
        {
            //我拾取成功，物品正在往我身上飞
            var delta = Time.deltaTime;

            var temp = player.Position;
            temp.y += PlayerOffset;
            //控制点朝着主角飞
            if (!mControlPointReached)
            {
                var distance = Vector2.Distance(temp, mControlPoint);
                if (distance <= MaxDistance)
                {
                    //当前已经飞到了
                    mControlPointReached = true;
                }
                else
                {
                    mControlPointMoveSpeed += delta*AccSpeed;
                    mControlPointMoveSpeed = Math.Min(mControlPointMoveSpeed, MAXSPEED);
                    var moveDis = mControlPointMoveSpeed*delta;
                    if (moveDis >= distance)
                    {
                        //这一步就飞到了
                        mControlPointReached = true;
                    }
                    else
                    {
                        //继续移动
                        var controlPointDir = (temp - mControlPoint).normalized;
                        mControlPoint += controlPointDir*moveDis;
                    }
                }
            }

            if (mControlPointReached)
            {
                mControlPoint = temp;
            }


            {
                //自己朝着控制点飞
                var dis = Vector3.Distance(mControlPoint, Position);

                if (dis <= MaxDistance && mControlPointReached)
                {
                    mState = DropItemState.Destory;
                    Destroy();
                }
                else
                {
                    mMoveSpeed += AccSpeed*delta;
                    mMoveSpeed = Math.Min(mMoveSpeed, MAXSPEED);
                    var moveDis = mMoveSpeed*delta;
                    if (moveDis >= dis)
                    {
                        Position = mControlPoint;
                        if (mControlPointReached)
                        {
                            mState = DropItemState.Destory;
                            Destroy();
                        }
                    }
                    else
                    {
                        var dir = (mControlPoint - Position).normalized;
                        Position = Position + dir*moveDis;
                    }
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

    public enum DropItemState
    {
        Load,
        Delay,
        Droping,
        StayOnGround,
        Pickup,
        FlyToPlayer,
        Destory
    }
}