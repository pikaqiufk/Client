using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using UnityEngine;
using DataTable;

#endregion

namespace GameUI
{
	public class BatteryProgessFrame : MonoBehaviour
	{
	    private String batteryName;
	
	    public String BatteryName
        {
	        get { return batteryName; }
	        set
	        {
                batteryName = value;
	        }
	    }
	
	    private void OnDestroy()
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

        public void OnClickBattery()
        {
            var logic = GameLogic.Instance;
            if (logic != null)
            {
                var scene = logic.Scene;
                if (scene != null)
                {
                    Table.ForeachMapTransfer(record =>
                    {

                        if (record.SceneID == scene.SceneTypeId && batteryName.Contains(record.Name))
                        {
                            var vipLevel = PlayerDataManager.Instance.GetRes((int)eResourcesType.VipLevel);
                            if (vipLevel > 3)
                            {
                                GameUtils.FlyTo(scene.SceneTypeId, record.PosX, record.PosZ);
                            }
                            else
                            {
                                var command = GameControl.GoToCommand(scene.SceneTypeId, record.PosX, record.PosZ, 1.0f);
                                GameControl.Executer.PushCommand(command);
                            }

                            return false;
                        }
                        return true;
                    });
                }
            }


           
        }
	}
}