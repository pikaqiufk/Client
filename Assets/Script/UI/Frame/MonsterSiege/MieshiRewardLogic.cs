using System;
#region using

using EventSystem;
using UnityEngine;
using ClientDataModel;

#endregion


namespace GameUI
{

}
public class MieshiRewardLogic : MonoBehaviour
{
    public BindDataRoot Binding;
    private IControllerBase mController;

    public void OnBtnConfirm()
    {
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MishiResultUI));
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        mController = UIManager.Instance.GetController(UIConfig.MishiResultUI);
        if (mController == null)
        {
            return;
        }
        Binding.SetBindDataSource(mController.GetDataModel(""));

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

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

}
