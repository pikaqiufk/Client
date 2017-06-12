using System;
#region using

using UnityEngine;

#endregion

public class NormalAttackBtnLogic : MonoBehaviour
{
    public BindDataRoot Binding;

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        Binding.RemoveBinding();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.SkillData.NormailAttack[0]);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Use this for initialization
    private void Start()
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

    // Update is called once per frame
    private void Update()
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
}