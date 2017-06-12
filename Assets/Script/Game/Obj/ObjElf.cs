#region using

using System;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

#endregion

public class ObjElf : ObjCharacter
{
    public Action OnElfMoveOver;

    public override OBJ.TYPE GetObjType()
    {
        return OBJ.TYPE.ELF;
    }

    public override bool Init(InitBaseData initData, Action callback = null)
    {
        State = ObjState.LogicDataInited;

        var init = initData as InitOtherPlayerData;

        mObjId = init.ObjId;
        mDataId = init.DataId;
        EquipList = new Dictionary<int, int>(init.EquipModel);

        Position = new Vector3(initData.X, initData.Y, initData.Z);
        Direction = new Vector3(initData.DirX, 0, initData.DirZ);
        InitTableData();
        InitAnimation();
        InitStateMachine();
        State = ObjState.LoadingResource;
        LoadModelAsync(() =>
        {
            State = ObjState.Normal;
            if (GetAnimationController() == null)
            {
                return;
            }
            GetAnimationController().Play(OBJ.CHARACTER_ANI.STAND);
            InitEquip();
            if (null != callback)
            {
                callback();
            }
        });
        return true;
    }

    protected override void InitTableData()
    {
        TableCharacter = Table.GetCharacterBase(mDataId);

        var modelId = TableCharacter.CharModelID;
        CharModelRecord = Table.GetCharModel(modelId);
    }

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
        });
    }

    public override bool MoveTo(Vector3 point, float offset = 0.05f, bool isSendFastReach = false)
    {
        var path = new NavMeshPath();
        point.y = GameLogic.GetTerrainHeight(point);
        NavMesh.CalculatePath(Position, point, -1, path);
        if (path.corners.Length <= 0)
        {
            return false;
        }

        mTargetPos.Clear();

        var vec = new List<Vector3>();
        var pathcornersLength0 = path.corners.Length;
        for (var j = 0; j < pathcornersLength0; j++)
        {
            vec.Add(path.corners[j]);
        }

        mTargetPos.AddRange(vec);
        if (GetCurrentStateName() != OBJ.CHARACTER_STATE.RUN)
        {
            DoMove();
        }

        return true;
    }

    public override void OnMoveOver()
    {
        if (OnElfMoveOver != null)
        {
            OnElfMoveOver();
        }
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
                    EffectManager.Instance.CreateEffect(tableData, this);
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
            case OBJ.CHARACTER_STATE.IDLE:
            default:
            {
                PlayAnimation(OBJ.CHARACTER_ANI.STAND);
            }
                break;
        }
    }
}