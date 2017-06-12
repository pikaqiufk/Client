using System;
public class DieState : BaseState
{
    protected override void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        base.OnEnable();
        mCharacter.StopMove();
        mCharacter.StopCurrentAnimation(true);
        mCharacter.PlayAnimation(OBJ.CHARACTER_ANI.DIE);
        if (mCharacter.HasWing())
        {
            mCharacter.PlayWingAnimation(ObjCharacter.WingState.Dead);
        }
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
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