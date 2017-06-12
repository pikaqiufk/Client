using System;
#region using
using DataTable;
using EventSystem;
using UnityEngine;
using ClientDataModel;
/*-------------------------------------------------------------------
Copyright 2015 Minty Game LTD. All Rights Reserved.
Maintained by  wangxing 
-------------------------------------------------------------------*/

#endregion


public class TowerRewardLogic : MonoBehaviour
{
    public BindDataRoot Binding;
    private IControllerBase mController;

    public void OnBtnConfirm()
    {
        int cur = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e623);
        var tb = Table.GetClimbingTower(cur + 1);
        if (tb != null)
        {
            GameUtils.EnterFuben(tb.FubenId);
        }
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ClimbingTowerRewardUI));
    }

    public void OnBtnBack()
    {
        GameUtils.ExitFuben();
    }
    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif
        mController = UIManager.Instance.GetController(UIConfig.ClimbingTowerRewardUI);
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
