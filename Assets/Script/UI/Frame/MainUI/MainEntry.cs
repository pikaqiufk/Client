using DataTable;
using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MainEntry : MonoBehaviour
	{

	    public void OnClickAchievement()
	    {
	        var e = new Show_UI_Event(UIConfig.AchievementFrame);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickActivity()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RewardFrame));
	    }
	
	    public void OnClickBattleUnion()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.BattleUnionUI));
	    }
	
	    public void OnClickBtnRank()
	    {
	        var e = new Show_UI_Event(UIConfig.RankUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickCompose()
	    {
	        var e = new Show_UI_Event(UIConfig.ComposeUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickDepot()
	    {
	        var e = new Show_UI_Event(UIConfig.DepotUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickDiamondShop()
	    {
	        var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments {Tab = 2});
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickDungeon()
	    {
	        var e = new Show_UI_Event(UIConfig.DungeonUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickElf()
	    {
	        PlayerDataManager.Instance.WeakNoticeData.ElfTotal = false;
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ElfUI));
	    }
	
	    public void OnClickEquip()
	    {
	        var e = new Show_UI_Event(UIConfig.EquipUI, new EquipUIArguments {Tab = 0});
	        EventDispatcher.Instance.DispatchEvent(e);
	        PlayerDataManager.Instance.WeakNoticeData.EquipTotal = false;
	    }
	
	    public void OnClickHandBook()
	    {
	        var e = new Show_UI_Event(UIConfig.HandBook);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickHuodong()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ActivityUI, new ActivityArguments
	        {
	            Tab = -1
	        }));
	    }
	
	    public void OnClickMail()
	    {
	        var e = new Show_UI_Event(UIConfig.MailUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickMission()
	    {
	        var e = new Show_UI_Event(UIConfig.MissionList);
	        EventDispatcher.Instance.DispatchEvent(new Event_MissionList_TapIndex(1));
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickOffLine()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.OffLineExpFrame));
	    }
	
	    public void OnClickPack()
	    {
	        var e = new Show_UI_Event(UIConfig.CharacterUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	        PlayerDataManager.Instance.WeakNoticeData.BagTotal = false;
	    }
	
	    public void OnClickPet()
	    {
	        PlayerDataManager.Instance.NoticeData.CityLevel = false;
            //EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.CityUI));
	    }
	
	    public void OnClickRank()
	    {
	        var e = new Show_UI_Event(UIConfig.RankUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecharge()
	    {
	        var e = new Show_UI_Event(UIConfig.RechargeActivityUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSetting()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.SettingUI));
	    }
	
	    public void OnClickShop()
	    {
	        //Game.Instance.ExitSelectCharacter();
	    }
	
	    public void OnClickSkill()
	    {
	        PlayerDataManager.Instance.WeakNoticeData.SkillTotal = false;
	        var e = new Show_UI_Event(UIConfig.SkillFrameUI, new SkillFrameArguments());
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSocial()
	    {
	        var e = new Show_UI_Event(UIConfig.FriendUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSwith()
	    {
	        //mIsShow = !mIsShow;
	        //foreach (var btn in BtnList)
	        //{
	        //    btn.gameObject.SetActive(mIsShow);
	        //}
	    }
	
	    public void OnClickTeam()
	    {
	        var e = new Show_UI_Event(UIConfig.TeamFrame);
	        EventDispatcher.Instance.DispatchEvent(e);
	        var ee = new UIEvent_TeamFrame_NearTeam();
	        EventDispatcher.Instance.DispatchEvent(ee);
	    }

		public void OnClick1V1()
		{
// 			var e = new Show_UI_Event(UIConfig.P1VP1Frame);
// 			EventDispatcher.Instance.DispatchEvent(e);

            var ee = new Show_UI_Event(UIConfig.AreanaUI, new ArenaArguments
            {
                BuildingData = CityManager.Instance.GetBuildingByAreaId(6),
                Tab = 0
            });
            EventDispatcher.Instance.DispatchEvent(ee);
		}
		public void OnClickBattleField()
		{
			var e = new Show_UI_Event(UIConfig.BattleUI);
			EventDispatcher.Instance.DispatchEvent(e);
		}

		public void OnClickPVPIsland()
		{
			GameUtils.GotoUiTab(60, 13);
		}

		public void OnClickWishTree()
		{
			var e = new Show_UI_Event(UIConfig.WishingUI);
			EventDispatcher.Instance.DispatchEvent(e);
		}
        public void OnClickExChange()
        {
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ExchangeUI));
        }

		public void OnClicItemCompose()
		{
			var e = new Show_UI_Event(UIConfig.ComposeUI);
			EventDispatcher.Instance.DispatchEvent(e);
		}
        public void OnClickWing()
        {
            var e = new AttriFrameOperate(1);
            EventDispatcher.Instance.DispatchEvent(e);
            PlayerDataManager.Instance.WeakNoticeData.BagEquipWing = false;
        }
		public void OnClicSmithy()
		{
			var list = CityManager.Instance.BuildingDataList;
			foreach (var buildingData in list)
			{
				var tb = Table.GetBuilding(buildingData.TypeId);
				if (null != tb)
				{
					if (BuildingType.BlacksmithShop == (BuildingType)tb.Type)
					{
						var ee = new Show_UI_Event(UIConfig.SmithyUI,
						new SmithyFrameArguments
						{
							BuildingData = buildingData
						});
						EventDispatcher.Instance.DispatchEvent(ee);
					}
				}
			}

		}

		public void OnClicSailingHarbor()
		{
			var list = CityManager.Instance.BuildingDataList;
			foreach (var buildingData in list)
			{
				var tb = Table.GetBuilding(buildingData.TypeId);
				if (null != tb)
				{
					if (BuildingType.BraveHarbor == (BuildingType)tb.Type)
					{
						var ee = new Show_UI_Event(UIConfig.SailingUI,
						new SailingArguments
						{
							BuildingData = buildingData
						});
						EventDispatcher.Instance.DispatchEvent(ee);
					}
				}
			}
			
		}

	    public void OnClick_Artifact()
	    {
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ArtifactUi));
	    }

	    public void OnClickOperationAcitivty()
		{
			var ee = new Show_UI_Event(UIConfig.OperationActivityFrame);
			EventDispatcher.Instance.DispatchEvent(ee);
		}

	    public void OnClickEyeVisible()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_VisibleEyeClick(true));
	    }

	    public void OnClickEyeInvisible()
	    {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_VisibleEyeClick(false));
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
	
	    private enum BtnType
	    {
	        BtnPack = 0,
	        BtnEquip,
	        BtnSkill,
	        BtnPet,
	        BtnSetting,
	        BtnSocial,
	        BtnShop
	    }
	}
}