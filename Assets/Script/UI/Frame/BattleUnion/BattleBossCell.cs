using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BattleBossCell: MonoBehaviour
	{
	    private ListItemLogic itemListLogic;
	
	    public void BossChallenge()
	    {
	        if (itemListLogic != null)
	        {
	            var itemData = itemListLogic.Item as BattleUnionBossInfoDataModel;
	            var e = new UIEvent_UnionBossClick();
	            e.BossIndex = itemListLogic.Index;
	            e.type = 0;
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    public void BossGetReward()
	    {
	        if (itemListLogic != null)
	        {
	            var itemData = itemListLogic.Item as BattleUnionBossInfoDataModel;
	            var e = new UIEvent_UnionBossClick();
	            e.BossIndex = itemListLogic.Index;
	            e.type = 1;
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        itemListLogic = gameObject.GetComponent<ListItemLogic>();
	
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