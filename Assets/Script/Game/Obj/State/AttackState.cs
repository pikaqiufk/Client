using System;
#region using

using BehaviourMachine;
using DataTable;

#endregion

public class AttackState : BaseState, IAnimationListener
{
    private bool mCanBreak = true;
    private FsmEvent mHurtEvent;

    protected override void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

//      if (mCharacter.GetNavMeshAgent()) 
// 		{
// 			mCharacter.GetNavMeshAgent().enabled = false;
// 		}

        mCharacter.SetAnimationListener(null);

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

        //mCharacter.GetNavMeshAgent().enabled = false;
        mCharacter.SetAnimationListener(this);
        var skilldata = mCharacter.GetCurrentSkillData();
        mCharacter.PlayAnimation(skilldata.ActionId);
        mCanBreak = Table.GetAnimation(skilldata.ActionId).IsCanBreak != 0;
        mHurtEvent = rootStateMachine.blackboard.GetFsmEvent(OBJ.STATEMACHINE_EVENT.HURT);
        base.OnEnable();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public override bool ProcessEvent(int eventID)
    {
        if (!mCanBreak && null != mHurtEvent && mHurtEvent.id == eventID)
        {
            return true;
        }

        return base.ProcessEvent(eventID);
    }

    #region AnimationListener

    public void OnAnimationBegin(string animationName)
    {
    }

    public void OnAnimationInterrupt(string animationName)
    {
    }

    public void OnAnimationEnd(string animationName)
    {
        mCharacter.DoIdle();
    }

    #endregion
}