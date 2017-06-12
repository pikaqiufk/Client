#region using

using System;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

#endregion

public class ObjFakeCharacter : ObjCharacter
{
    public static int Offset;
    protected bool SyncLoadModel;
    public int iType;
    protected override void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (EffectRoot == null && ObjTransform != null)
        {
            EffectRoot = new GameObject();
            EffectRoot.name = "EffectRoot";
            var transform = EffectRoot.transform;
            transform.parent = ObjTransform;
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
        }

        mTransform = gameObject.transform;


        mActorAvatar = gameObject.GetComponent<ActorAvatar>();

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public static ObjFakeCharacter Create(int dateId,
                                          Dictionary<int, int> equipList = null,
                                          Action<ObjFakeCharacter> callback = null,
                                          int layer = 0,
                                          bool sync = false,
                                          int renderQueue = -1,
                                          ulong objId = 0)
    {
        var initData = new InitOtherPlayerData
        {
            ObjId = objId,
            DataId = dateId,
            X = Offset += 10,
            Y = -1000,
            Z = -2000,
            DirX = 0,
            DirZ = 1,
            Name = "",
            IsDead = false,
            IsMoving = false,
            MoveSpeed = 0,
            Camp = 0,
            RobotId = 0ul
        };

        if (null != equipList)
        {
            {
                // foreach(var pair in equipList)
                var __enumerator2 = (equipList).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var pair = __enumerator2.Current;
                    {
                        initData.EquipModel.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        var go = ComplexObjectPool.NewObjectSync(Resource.PrefabPath.FakeCharacter);
        go.layer = layer;
        var character = go.GetComponent<ObjFakeCharacter>();
        character.SyncLoadModel = sync;
        character.RenderQueue = renderQueue;
#if UNITY_EDITOR
        go.name = "ObjFakeCharacter" + "_" + initData.ObjId;
#endif
        go.SetActive(true);
        var model = character.GetModel();
        if (model != null)
        {
            model.SetActive(false);
        }
        character.Init(initData, () =>
        {
            callback(character);
            model = character.GetModel();
            if (model != null)
            {
                model.SetActive(true);
            }
        });

        return character;
    }

    public override void Destroy()
    {
        if (mActorAvatar)
        {
            mActorAvatar.Destroy();
        }
        base.Destroy();
    }

    public override OBJ.TYPE GetObjType()
    {
        return OBJ.TYPE.FAKE_CHARACTER;
    }

    //初始化
    public override bool Init(InitBaseData initData, Action callback = null)
    {
        State = ObjState.LogicDataInited;

        var init = initData as InitOtherPlayerData;

        mObjId = init.ObjId;
        //mObjId = ulong.MaxValue;
        mDataId = init.DataId;
        EquipList = new Dictionary<int, int>(init.EquipModel);

        Position = new Vector3(initData.X, initData.Y, initData.Z);
        Direction = new Vector3(initData.DirX, 0, initData.DirZ);

        InitTableData();
        InitAnimation();

        PrepareAnimation(OBJ.CHARACTER_ANI.STAND);

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
                if (GetAnimationController() == null)
                {
                    return;
                }
                InitEquip(SyncLoadModel);
                PlayAnimation(OBJ.CHARACTER_ANI.STAND);

                if (null != callback)
                {
                    callback();
                }
            });
        };

        return true;
    }

    protected override void InitTableData()
    {
        TableCharacter = Table.GetCharacterBase(mDataId);

        var modelId = TableCharacter.CharModelID;
        CharModelRecord = Table.GetCharModel(modelId);
    }

    //是否是角色
    public override bool IsCharacter()
    {
        return false;
    }

    protected override void LoadModelAsync(Action callback = null)
    {
        var tableModel = Table.GetCharModel(TableCharacter.CharModelID);
        var modelPath = Resource.GetModelPath(tableModel.ResPath);

        UniqueResourceId = GetNextUniqueResourceId();
        var resId = UniqueResourceId;
        if (null != mActorAvatar)
        {
            mActorAvatar.Layer = gameObject.layer;
            mActorAvatar.RenderQueue = RenderQueue;
        }
        ComplexObjectPool.NewObject(modelPath, model =>
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
            mAnimationController.Animation = mModel.animation;
            mAnimationController.Animation.Stop();
            SetScale((float) tableModel.Scale);
            if (null != callback)
            {
                callback();
            }
        }, null, o =>
        {
            OptList<Renderer>.List.Clear();
            o.GetComponentsInChildren(OptList<Renderer>.List);
            {
                var __array1 = OptList<Renderer>.List;
                var __arrayLength1 = __array1.Count;
                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var renderer = __array1[__i1];
                    {
                        renderer.enabled = false;
                    }
                }
            }
        });
    }

    protected override void OnSetModel()
    {
        mMountPoints.Clear();
        if (mActorAvatar)
        {
            mActorAvatar.Body = mModel;
        }
        gameObject.SetLayerRecursive(gameObject.layer);
    }

    public override void RefreshAnimation()
    {
        var currentState = GetCurrentStateName();
        switch (currentState)
        {
            case OBJ.CHARACTER_STATE.RUN:
            {
                if (HasWing())
                {
                    if (IsInSafeArea())
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.Walk);
                    }
                    else
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.FlyMove);
                    }
                }
                else
                {
                    if (IsInSafeArea())
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.Walk);
                    }
                    else
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
                    if (IsInSafeArea())
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.HIT, aniName => { DoIdle(); });
                    }
                    else
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.FLYHIT, aniName => { DoIdle(); });
                    }
                }
                else
                {
                    if (IsInSafeArea())
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.HIT, aniName => { DoIdle(); });
                    }
                    else
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
                    if (IsInSafeArea())
                    {
                        PlayAnimation(OBJ.CHARACTER_ANI.STAND);
                    }
                    else
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
                if (IsInSafeArea())
                {
//在安全区
                    //安全区肯定是走的状态，翅膀慢速
                    PlayWingAnimation(WingState.Idle);

                    if (NameBoard)
                    {
                        NameBoard.ResetOffset();
                    }
                }
                else
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
    }

    public override void StartAttributeSync()
    {
    }

    public override void StopAttributeSync()
    {
    }

    protected override void Tick(float Delta)
    {
        if (State == ObjState.LogicDataInited)
        {
            if (ObjManager.Instance.CanLoad() && LoadResourceAction != null)
            {
                LoadResourceAction();
            }
        }
    }
}