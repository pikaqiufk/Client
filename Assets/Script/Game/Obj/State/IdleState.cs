
#region using

using System;
using UnityEngine;

#endregion

public class IdleState : BaseState
{
    private float mNextPlaySpecialIdleTime;
    public float SpecialIdleAnimationIntervalMax = 15.0f;
    public float SpecialIdleAnimationIntervalMin = 8.0f;

    protected override void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        base.OnEnable();
        if (mCharacter.HasWaitingSkill())
        {
            return;
        }

        mCharacter.RefreshAnimation();

        ProcessSpecialIdleAnimation(false);

        if (mCharacter.Target != null)
        {
            if (mCharacter.GetObjType() == OBJ.TYPE.NPC)
            {
                if(((ObjNPC)mCharacter).TowardPlayer)
                    mCharacter.FaceTo(mCharacter.Target.Position);
            }
            else
            {
                mCharacter.FaceTo(mCharacter.Target.Position);
            }

            if (mCharacter.DelayedMove == null)
            {
                mCharacter.Target = null;
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

    protected virtual void ProcessSpecialIdleAnimation(bool play)
    {
        if (!mCharacter.IsPlaySpecialIdleAnimation())
        {
            return;
        }

        mNextPlaySpecialIdleTime = mTime +
                                   UnityEngine.Random.Range(SpecialIdleAnimationIntervalMin, SpecialIdleAnimationIntervalMax);
        if (play)
        {
            var idx = UnityEngine.Random.Range(0, mCharacter.mSpecialIdleAnimationId.Count);
            var id = mCharacter.mSpecialIdleAnimationId[idx];
            mCharacter.PlayAnimation(id, str => { mCharacter.RefreshAnimation(); });
        }
    }

    protected override void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (mCharacter.DelayedMove != null)
        {
            mCharacter.MoveTo(mCharacter.DelayedMove.Value);
            mCharacter.DelayedMove = null;
            return;
        }

        base.Update();
        if (mTime >= mNextPlaySpecialIdleTime)
        {
            ProcessSpecialIdleAnimation(true);
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