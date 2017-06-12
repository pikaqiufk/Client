using System;
public class HurtState : BaseState
{
    private float mBeginTime;

    protected override void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        base.OnEnable();

        if (null != mCharacter.GetAnimationController())
        {
            mCharacter.GetAnimationController().Stop(false);
        }

        mCharacter.RefreshAnimation();

        mBeginTime = mTime;

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
        var fsmEvent = rootStateMachine.blackboard.GetFsmEvent(OBJ.STATEMACHINE_EVENT.HURT);
        if (null != fsmEvent)
        {
            if (fsmEvent.id == eventID)
            {
                if (mTime - mBeginTime < GameSetting.Instance.MinHurtActionInterval)
                {
                    return true;
                }
            }
        }

        return base.ProcessEvent(eventID);
    }

    protected override void Update()
    {
#if !UNITY_EDITOR
try
{
#endif


        base.Update();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}