using System;
public class DizzyState : BaseState
{
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