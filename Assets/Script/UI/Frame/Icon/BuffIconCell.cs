using System.Collections.Generic;
using EventSystem;
using System;
#region using

using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BuffIconCell : MonoBehaviour
	{
        public BindDataRoot BindRoot;
        private BuffInfoCell itemIdDM = new BuffInfoCell();

	    public int BuffId
	    {
	        get
	        {
	            return itemIdDM.BuffId;
	        }
	        set
	        {
	            itemIdDM.BuffId = value;
                if (BindRoot != null)
                {
                    BindRoot.SetBindDataSource(itemIdDM);
                }	            
	        }
	    }

        public int Level
        {
            get
            {
                return itemIdDM.BuffLevel;
            }
            set
            {
                itemIdDM.BuffLevel = value;
                if (BindRoot != null)
                {
                    BindRoot.SetBindDataSource(itemIdDM);
                }
            }
        }
	
	    public void OnClickIcon()
	    {
            if (itemIdDM.BuffId != -1)
            {
                var arg = new UIInitArguments();
                arg.Args = new List<int>();
                arg.Args.Add(itemIdDM.BuffId);
                arg.Args.Add(itemIdDM.BuffLevel);
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.EquipSkillTipUI, arg));
	        }
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
}