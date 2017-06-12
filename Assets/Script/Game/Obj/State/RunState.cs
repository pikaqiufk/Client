using System.Runtime.InteropServices;
using System.Collections;
using EventSystem;
using System;
#region using

using UnityEngine;

#endregion

public class RunState : BaseState
{
    public const float MaxErrorTime = 0.5f;
    private Vector2 mLastPostion;
    private float mTheTimeAtHere; 
    protected int mColliderLayer = LayerMask.GetMask(GAMELAYER.Collider);

    private void LateUpdate()
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

    protected override void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        base.OnDisable();
// 		if(mCharacter.HasWing())
// 		{
// 			mCharacter.PlayWingAnimation(false);
// 		}

        if (mCharacter.GetObjType() == OBJ.TYPE.MYPLAYER)
        {
            var player = mCharacter as ObjMyPlayer;
            if (null != player.MoveOverCallBack)
            {
                player.MoveOverCallBack();
                player.MoveOverCallBack = null;
            }

            // 隐藏快速到达
            EventDispatcher.Instance.DispatchEvent(new ShowFastReachEvent(false));
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    protected override void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        base.OnEnable();
        mCharacter.GetNavMeshAgent().updatePosition = false;
        mCharacter.GetNavMeshAgent().Warp(mCharacter.Position);
        mCharacter.RefreshAnimation();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private IEnumerator WaitSecondsDoSomething(float seconds, Action act)
    {
        yield return new WaitForSeconds(seconds);
        if (act != null)
        {
            act();
        }
    }

    protected override void ResetState()
    {
        mTheTimeAtHere = 0.0f;
        mLastPostion = Vector2.zero;
    }

    protected override void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        base.Update();

        //有路经点
        if (mCharacter.TargetPos.Count > 0)
        {
            var nav = mCharacter.GetNavMeshAgent();
            var pos = mCharacter.TargetPos[0].xz();
            var targetPos = mCharacter.TargetPos;

            var direction = (pos - mCharacter.Position.xz()).normalized;
            mCharacter.TargetDirection = new Vector3(direction.x, 0, direction.y);

            var stepLenth = mCharacter.GetMoveSpeed()*Time.deltaTime;

            if (Physics.Raycast(mCharacter.Position, mCharacter.Direction, 1, mColliderLayer))
            {
                mCharacter.StopMove();
                if (mCharacter.GetObjType() == OBJ.TYPE.MYPLAYER)
                {
                    ((ObjMyPlayer)mCharacter).SendStopMoveToServer();
                }
                return;
            }

            var dif = Vector2.Distance(pos, mCharacter.Position.xz());

            if (stepLenth > dif)
            {
                var v = stepLenth - dif;
                pos = targetPos[0].xz();
                targetPos.RemoveAt(0);
                while (v > 0 && targetPos.Count > 0)
                {
                    var dir = (targetPos[0].xz() - pos);
                    var l = dir.magnitude;

                    if (l < v)
                    {
                        v -= l;
                        pos = targetPos[0].xz();
                        targetPos.RemoveAt(0);
                    }
                    else
                    {
                        dir.Normalize();
                        pos += dir*v;
                        nav.Warp(new Vector3(pos.x, nav.nextPosition.y, pos.y));
                        mCharacter.Position = new Vector3(pos.x, nav.nextPosition.y, pos.y);
                        break;
                    }
                }

                if (targetPos.Count == 0)
                {
                    nav.Warp(new Vector3(pos.x, nav.nextPosition.y, pos.y));
                    mCharacter.Position = new Vector3(pos.x, nav.nextPosition.y, pos.y);
                }
            }
            else
            {
                pos = mCharacter.Position.xz() + direction*stepLenth;
                nav.Warp(new Vector3(pos.x, nav.nextPosition.y, pos.y));
                mCharacter.Position = new Vector3(pos.x, nav.nextPosition.y, pos.y);
            }
        }

        var myPostion = new Vector2(mCharacter.Position.x, mCharacter.Position.z);

        //走完了
        if (mCharacter.TargetPos.Count <= 0)
        {
            mCharacter.OnMoveOver();
            return;
        }


        //计算下站在原地的时间
        if (mLastPostion != myPostion)
        {
            mTheTimeAtHere = 0.0f;
            mLastPostion = myPostion;
        }
        else
        {
            mTheTimeAtHere += Time.deltaTime;
        }

        //如果一直在原地说明无法走到目标点

        //那就停止移动了
        if (mTheTimeAtHere >= MaxErrorTime)
        {
            //mCharacter.DoIdle();
        }


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}