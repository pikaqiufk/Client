#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion



public class UIOperationActivityFrameController : IControllerBase
{


	public OperationActivityTotalDataModel OperationActivityDataModel { get; set; }
	private DateTime LastRequestTime = DateTime.MinValue;
	private bool Dirty = false;
	private List<OperationActivityItemDataModel> mItemCache = new List<OperationActivityItemDataModel>();
	private Coroutine mCoroutine = null;
	private List<KeyValuePair<int, int>> LotteryRecord = new List<KeyValuePair<int, int>>();
	public UIOperationActivityFrameController()
	{
		CleanUp();

		EventDispatcher.Instance.AddEventListener(OperationActivityPageClickEvent.EVENT_TYPE, OnClickPageBtn);
		EventDispatcher.Instance.AddEventListener(OperationActivitySubPageClickEvent.EVENT_TYPE, OnClickSubTableBtn);
		//EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnDataChanged);
		//EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnDataChanged);
		//EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnDataChanged);
		EventDispatcher.Instance.AddEventListener(SyncOperationActivityItemEvent.EVENT_TYPE, OnSyncOperationActivityItem);
		EventDispatcher.Instance.AddEventListener(SyncOperationActivityTermEvent.EVENT_TYPE, OnSyncOperationActivityTerm);
		EventDispatcher.Instance.AddEventListener(OperationActivityClaimReward.EVENT_TYPE, OnOperationActivityClaimReward);
		EventDispatcher.Instance.AddEventListener(OperationActivityDataInitEvent.EVENT_TYPE, OnOperationActivityDataInitEvent);

		LastRequestTime = DateTime.MinValue;
	}

    private void Init()
    {

    }

    public void CleanUp()
    {
		LastRequestTime = DateTime.MinValue;
		OperationActivityDataModel = new OperationActivityTotalDataModel();
	    foreach (var termModel in OperationActivityDataModel.ActivityTermList)
	    {
		    termModel.Type = -1;
	    }
		mItemCache.Clear();
	    mCoroutine = null;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
       
        return null;
    }

    public void OnShow()
    {
    }

    public void Close()
    {

    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
	    ApplyOperationActivity();

	    ChooseActivityMenu(0);
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
		return OperationActivityDataModel;
    }

    public FrameState State { get; set; }


	private void LoadLotteryRecord()
	{
		try
		{
			var id = PlayerDataManager.Instance.CharacterGuid;
			var key = id + "_Lottery";

			var value = PlayerPrefs.GetString(key, "");
			if (!string.IsNullOrEmpty(value))
			{
				var array = value.Split('|');
				foreach (var s in array)
				{
					var temp = s.Split(',');
					if (temp.Length < 2)
					{
						continue;
					}
					LotteryRecord.Add(new KeyValuePair<int, int>(int.Parse(temp[0]), int.Parse(temp[1])));
					if (LotteryRecord.Count > 10)
					{
						LotteryRecord.RemoveAt(0);
					}
				}
			}
			ParseLotteryRecord();
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);	
		}

	}

	private void AddLotteryRecord(int item, int count)
	{
		LotteryRecord.Add(new KeyValuePair<int, int>(item,count));
		if (LotteryRecord.Count > 10)
		{
			LotteryRecord.RemoveAt(0);
		}
		SaveLotteryRecord();
		ParseLotteryRecord();
	}

	private void SaveLotteryRecord()
	{
		try
		{
			var id = PlayerDataManager.Instance.CharacterGuid;
			var key = id + "_Lottery";

			string temp = "";
			foreach (var kv in LotteryRecord)
			{
				temp += kv.Key + "," + kv.Value + "|";
			}
			PlayerPrefs.SetString(key, temp);
			PlayerPrefs.Save();
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);	
		}

	}

	private void ParseLotteryRecord()
	{
		try
		{
			if (LotteryRecord.Count <= 0)
			{
				return;
			}

			OperationActivityTypeLotteryDataModel model = null;
			foreach (var term in OperationActivityDataModel.ActivityTermList)
			{
				if ((OperationActivityType)term.Type == OperationActivityType.Lottery)
				{
					model = term.LotteryData;
					break;
				}
			}
			if (null == model)
			{
				return;
			}
			model.Log = "";
			for (int i=LotteryRecord.Count-1; i>=0; i-- )
			{
				var kv = LotteryRecord[i];
				if (-1 == kv.Key || kv.Value <= 0)
				{
					continue;
				}
				var tbItem = Table.GetItemBase(kv.Key);
				if (null == tbItem)
				{
					continue;
				}
				var temp = tbItem.Name + "X" + kv.Value.ToString();
				model.Log += temp + "\n";
			}
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}

	}

	private void ApplyOperationActivity()
	{
 		if (Dirty || (DateTime.Now - LastRequestTime).TotalMinutes>=10)
 		{
			PlayerDataManager.Instance.ApplyOperationActivity();
 			Dirty = false;
 #if UNITY_EDITOR
 			Dirty = true;
 #endif
 		}
		
	}

	public static int SortCompare(MsgOperActivtyTerm a, MsgOperActivtyTerm b)
	{
		if (a.SortId > b.SortId)
		{
			return -1;
		}
		else if (a.SortId == b.SortId)
		{
			return 0;
		}
		return 1;
	}

	private void OnOperationActivityDataInitEvent(IEvent ievent)
	{
		var e = ievent as OperationActivityDataInitEvent;
		OperationActivityDataInit(e.Msg);
		LoadLotteryRecord();
	}

	private void OperationActivityDataInit(MsgOperActivty msg)
	{
		LastRequestTime = DateTime.Now;

		var msgTerms = msg.Terms;
		msgTerms.Sort(SortCompare);


		foreach (var termModel in OperationActivityDataModel.ActivityTermList)
		{
			termModel.ActivityId = -1;
		}
		mItemCache.Clear();
		
		int i = 0;
		foreach (var msgTerm in msgTerms)
		{
			if (i >= OperationActivityDataModel.ActivityTermList.Count)
			{
				break;
			}

			if (-1 != msgTerm.ParentTypeId)
			{
				continue;
			}

			var termModel = OperationActivityDataModel.ActivityTermList[i];


			termModel.ActivityId = msgTerm.Id;
			termModel.Name = msgTerm.Name;
			termModel.BkgIconId = msgTerm.BkgIconId;
			termModel.SmallIcon = msgTerm.SmallIcon;
			termModel.Desc = msgTerm.Desc;
			termModel.EndTime =  DateTime.FromBinary(msgTerm.EndTime);
			termModel.StartTime = DateTime.FromBinary(msgTerm.StarTime);
			termModel.ScoreTime = DateTime.FromBinary(msgTerm.ScoreTime);
			termModel.ModelPath = msgTerm.ModelPath;
			termModel.UIType = msgTerm.UIType;

			if (termModel.StartTime == DateTime.MinValue || termModel.EndTime==DateTime.MaxValue)
			{
				termModel.TimeString = "";
			}
			else
			{
				var str = GameUtils.GetDictionaryText(270251);
				termModel.TimeString = termModel.StartTime.ToString(str) + "-" + termModel.EndTime.ToString(str);
			}

			termModel.Type = msgTerm.Type;

			if ((OperationActivityType)termModel.Type == OperationActivityType.Guide)
			{
				termModel.UIType = (int)OperationActivityUIType.Guide;
			}
			else if ((OperationActivityType)termModel.Type == OperationActivityType.Recharge)
			{
				if (termModel.UIType != (int)OperationActivityUIType.Charge &&
					termModel.UIType != (int)OperationActivityUIType.ShowModel)
				{
					termModel.UIType = (int)OperationActivityUIType.Charge;	
				}
				
			}
			else if ((OperationActivityType)termModel.Type == OperationActivityType.Rank)
			{
				termModel.UIType = (int)OperationActivityUIType.Rank;
				termModel.Desc += string.Format("[{0}]", termModel.ScoreTime.ToString("yyyy/MM/dd/HH:mm"));
			}
			else if ((OperationActivityType)termModel.Type == OperationActivityType.Lottery)
			{
				termModel.UIType = (int)OperationActivityUIType.Lottery;
				//termModel.Desc += string.Format("[{0}]", termModel.ScoreTime.ToString("yyyy/MM/dd/HH:mm"));
			}
			else
			{
				termModel.UIType = msgTerm.UIType;
			}
			var uitype = (OperationActivityUIType)termModel.UIType;

			var tempList = new ObservableCollection<OperationActivityItemDataModel>();
			foreach (var itemData in msgTerm.Items)
			{
				var itemModel = new OperationActivityItemDataModel();

				AssignActivityItemData(itemData, termModel.ActivityId, itemModel);
				ParseRewardItem(itemData.Rewards, itemModel.Rewards);
				if (Game.Instance.ServerTime < itemModel.StartTime || Game.Instance.ServerTime > itemModel.EndTime)
				{
					continue;
				}
				tempList.Add(itemModel);
			}



			if (OperationActivityUIType.Guide == uitype)
			{
				termModel.GuideData.ActivityItemList = tempList;
				foreach (var itemDataModel in termModel.GuideData.ActivityItemList)
				{
					mItemCache.Add(itemDataModel);
				}
			}
			else if (OperationActivityUIType.Normal == uitype)
			{
				termModel.NormalData.ActivityItemList = tempList;
				foreach (var itemDataModel in termModel.NormalData.ActivityItemList)
				{
					mItemCache.Add(itemDataModel);
				}
			}
			else if (OperationActivityUIType.Charge == uitype)
			{
				termModel.ChargeData.ActivityItemList = tempList;
				foreach (var itemDataModel in termModel.ChargeData.ActivityItemList)
				{
					mItemCache.Add(itemDataModel);
				}
			}
			else if (OperationActivityUIType.Table == uitype)
			{
				foreach (var tbItem in termModel.TableData.TableList)
				{
					tbItem.ActivityItemList.Clear();
				}
			}
			else if (OperationActivityUIType.Rank == uitype)
			{
				termModel.RankData.ActivityItemList = tempList;
				foreach (var itemDataModel in termModel.RankData.ActivityItemList)
				{
					mItemCache.Add(itemDataModel);
				}
			}
			else if (OperationActivityUIType.Discount == uitype)
			{
				termModel.DiscountData.ActivityItemList = tempList;
				foreach (var itemDataModel in termModel.DiscountData.ActivityItemList)
				{
					mItemCache.Add(itemDataModel);
				}
			}
			else if (OperationActivityUIType.ShowModel == uitype)
			{
				termModel.ShowModelData.ActivityItemList = tempList;
				foreach (var itemDataModel in termModel.ShowModelData.ActivityItemList)
				{
					mItemCache.Add(itemDataModel);
				}
			}
			else if (OperationActivityUIType.Lottery == uitype)
			{
				var lotteryModel = termModel.LotteryData;

				lotteryModel.CostList.Clear();
				for (int ci = 2; ci < msgTerm.Param.Count; ci++)
				{
					lotteryModel.CostList.Add(msgTerm.Param[ci]);
				}//顺序不要条还
				lotteryModel.Times = msgTerm.Param[0];
				lotteryModel.ResetCost = msgTerm.Param[1];
				
				lotteryModel.ActivityItemList = tempList;
				foreach (var itemDataModel in lotteryModel.ActivityItemList)
				{
					mItemCache.Add(itemDataModel);
				}
				for (int k = 0; k < lotteryModel.Rewards.Count; k++)
				{
					var rewardItemModel = lotteryModel.Rewards[k];
					if (k < lotteryModel.ActivityItemList.Count)
					{
						var reward = lotteryModel.ActivityItemList[k].Rewards[0];
						rewardItemModel.ItemId = reward.ItemId;
						rewardItemModel.Count = reward.Count;
					}
					else
					{
						rewardItemModel.ItemId = -1;
					}
				}
			}
			else
			{
				termModel.NormalData.ActivityItemList = tempList;
				foreach (var itemDataModel in termModel.NormalData.ActivityItemList)
				{
					mItemCache.Add(itemDataModel);
				}
				
				Logger.Error("uitype={0}", uitype);
			}
			i++;
		}

		foreach (var msgTermItem in msgTerms)
		{
			if (-1 == msgTermItem.ParentTypeId)
			{
				continue;
			}

			OperationActivityTermDataModel parent = null;
			foreach (var item in OperationActivityDataModel.ActivityTermList)
			{
				if (msgTermItem.ParentTypeId == item.ActivityId)
				{
					parent = item;
					break;
				}
			}

			if (null == parent)
			{
				Logger.Error("null == parent");
				continue;
			}

			OperationActivityTypeNormalDataModel termModel1 = null;
			for (int j=0; j<parent.TableData.TableList.Count; j++)
			{
				if (0 == parent.TableData.TableList[j].ActivityItemList.Count)
				{
					termModel1 = parent.TableData.TableList[j];
					break;
				}
			}

			if (null == termModel1)
			{
				Logger.Error("null == normalDataModel");
				continue;
			}

			termModel1.ActivityId = msgTermItem.Id;
			termModel1.Name = msgTermItem.Name;
			termModel1.BkgIconId = msgTermItem.BkgIconId;
			termModel1.AllItemActive = false;

			var tempList1 = new ObservableCollection<OperationActivityItemDataModel>();
			foreach (var itemData1 in msgTermItem.Items)
			{
				var itemModel1 = new OperationActivityItemDataModel();

				AssignActivityItemData(itemData1, termModel1.ActivityId, itemModel1);
				ParseRewardItem(itemData1.Rewards, itemModel1.Rewards);
				if (Game.Instance.ServerTime < itemModel1.StartTime || Game.Instance.ServerTime > itemModel1.EndTime)
				{
					itemModel1.InTime = false;
					termModel1.AllItemActive = true;
				}
				else
				{
					itemModel1.InTime = true;
				}
				tempList1.Add(itemModel1);
			}
			termModel1.ActivityItemList = tempList1;
			foreach (var temp in termModel1.ActivityItemList)
			{
				mItemCache.Add(temp);
			}
		}

		OperationActivityDataModel.OnPropertyChanged("ActivityCount");
		CheckAllCondition();

	}

	private IEnumerator OperationActivityClaimRewardCoroutine(int parentId,int id,float delay=0)
	{
		if (delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}

		var msg = NetManager.Instance.ClaimOperationReward(parentId,id);
		yield return msg.SendAndWaitUntilDone();
		if (msg.State != MessageState.Reply)
		{
			EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200005000));
			mCoroutine = null;
			yield break;
		}

		if (msg.ErrorCode != 0)
		{
			UIManager.Instance.ShowNetError(msg.ErrorCode);
			mCoroutine = null;
			yield break;
		}


		var ret = msg.Response;
		for (int i = 0; i < OperationActivityDataModel.ActivityTermList.Count; i++)
		{
			var termModel = OperationActivityDataModel.ActivityTermList[i];
			if (-1!=termModel.ActivityId && termModel.ActivityId == parentId && OperationActivityUIType.Lottery == (OperationActivityUIType) termModel.UIType)
			{
				for (int j=0;j<termModel.LotteryData.ActivityItemList.Count; j++)
				{
					var actItem = termModel.LotteryData.ActivityItemList[j];
					if (ret == actItem.ItemId)
					{
						var e = new OperationActivityDrawLotteryEvent(j, actItem.Rewards[0].ItemId, actItem.Rewards[0].Count);
						EventDispatcher.Instance.DispatchEvent(e);

						AddLotteryRecord(actItem.Rewards[0].ItemId, actItem.Rewards[0].Count);
						break;
					}
				}
				break;
			}
		}

		mCoroutine = null;
	}

	private void OnClickPageBtn(IEvent ievent)
	{
		var e = ievent as OperationActivityPageClickEvent;
		ChooseActivityMenu(e.Idx);
	}

	private void OnClickSubTableBtn(IEvent ievent)
	{
		var e = ievent as OperationActivitySubPageClickEvent;
		var model = OperationActivityDataModel.ActivityTermList[OperationActivityDataModel.CurrentSelectPageIdx].TableData;

		model.SubTabIdx = e.Idx;
		EventDispatcher.Instance.DispatchEvent(new OperationActivitySubPagekEvent(model.SubTabIdx));

	}
	private void OnDataChanged(IEvent ievent)
	{
		CheckAllCondition();
	}
	private void OnSyncOperationActivityItem(IEvent ievent)
	{
		var e = ievent as SyncOperationActivityItemEvent;
		if (null == e.Data)
		{
			Dirty = true;
			return;
		}

		bool needRefreshLotteryMask = false;
		foreach (var item in mItemCache)
 		{
			if (item.ItemId == e.Data.Id)
			{
				needRefreshLotteryMask = (0 == e.Data.AquiredTimes && item.AquiredTimes > 0);
				item.Count = Math.Min(e.Data.Count, item.Need);
				item.AquiredTimes = e.Data.AquiredTimes;
				item.RemainAquireTimes = string.Format(GameUtils.GetDictionaryText(100001156), Math.Max(item.AquiredTotalTimes - item.AquiredTimes, 0));
				CheckAllCondition();

				foreach (var term in OperationActivityDataModel.ActivityTermList)
				{
					if (term.ActivityId == item.ParentId)
					{
						term.OnPropertyChanged("NoticeCount");
						OperationActivityDataModel.OnPropertyChanged("NoticeCount");
						break;
					}
					else if(term.UIType==(int)OperationActivityUIType.Table)
					{
						foreach (var item2 in term.TableData.TableList)
						{
							if (item2.ActivityId == item.ParentId)
							{
								item2.OnPropertyChanged("NoticeCount");
								term.OnPropertyChanged("NoticeCount");
								OperationActivityDataModel.OnPropertyChanged("NoticeCount");
								break;
							}
						}
					}
				}
				
				/*
				if (!needRefreshLotteryMask && item.AquiredTimes == 0)
				{
					if (OperationActivityDataModel.CurrentSelectPageIdx >= 0 && OperationActivityDataModel.CurrentSelectPageIdx < OperationActivityDataModel.ActivityTermList.Count)
					{
						var currentData = OperationActivityDataModel.ActivityTermList[OperationActivityDataModel.CurrentSelectPageIdx];
						if ((OperationActivityUIType) currentData.UIType == OperationActivityUIType.Lottery)
						{
							needRefreshLotteryMask = true;	
						}
					}
					
				}
				*/

				break;
			}
 		}

		if (needRefreshLotteryMask)
		{
			var ee = new OperationActivityDrawLotteryEvent(-2);
			EventDispatcher.Instance.DispatchEvent(ee);
		}
	}

	private void OnSyncOperationActivityTerm(IEvent ievent)
	{
		var e = ievent as SyncOperationActivityTermEvent;
		foreach (var term in OperationActivityDataModel.ActivityTermList)
		{
			if (term.ActivityId == e.Id)
			{
				if (OperationActivityUIType.Lottery == (OperationActivityUIType)term.UIType)
				{
					term.LotteryData.Times = e.Param;				
				}
				break;
			}
		}
		
	}

	private void OnOperationActivityClaimReward(IEvent ievent)
	{
		var e = ievent as OperationActivityClaimReward;
		if (null == e)
		{
			return;
		}
		ClaimRewardOperationActivity(e.Id);
	}

	private void ClaimRewardOperationActivity(int id)
	{
		//判断是否是抽奖
		if (OperationActivityDataModel.CurrentSelectPageIdx >= 0 &&
		    OperationActivityDataModel.CurrentSelectPageIdx < OperationActivityDataModel.ActivityTermList.Count)
		{
			var current = OperationActivityDataModel.ActivityTermList[OperationActivityDataModel.CurrentSelectPageIdx];
			if ((OperationActivityUIType)current.UIType == OperationActivityUIType.Lottery)
			{
				
				int opt = id;//0重置，1抽奖
				int need = 0;
				var lotterModel = current.LotteryData;
				if (0 == opt)
				{
					if (lotterModel.Times < lotterModel.TotalTimes)
					{
						return;
					}
					need = lotterModel.ResetCost;
				}
				else
				{
					if (lotterModel.Times >= lotterModel.TotalTimes)
					{
						return;
					}
					need = lotterModel.CurrentCost;
					if (null != mCoroutine)
					{
						Logger.Error("ClaimRewardOperationActivity not finish");
						return;
					}
				}
				if (PlayerDataManager.Instance.GetRes((int) eResourcesType.DiamondRes)<need )
				{
					EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(210102)));
					GameUtils.GotoUiTab(79, 0);
				}
				else
				{
					if (null != GameLogic.Instance)
					{
						float delay = 0 == opt?0:1f;
						mCoroutine = GameLogic.Instance.StartCoroutine(OperationActivityClaimRewardCoroutine(current.ActivityId, id, delay));
						if (0 != opt)
						{
							var e = new OperationActivityDrawLotteryEvent(-1);
							EventDispatcher.Instance.DispatchEvent(e);	
						}
					}
				}
				return;
			}
		}

		OperationActivityItemDataModel item = null;
		foreach (var itemDataModel in mItemCache)
		{
			if (itemDataModel.ItemId == id)
			{
				item = itemDataModel;
				break;
			}
		}

		if (null == item)
		{
			return;
		}

		if (-1 != item.GuideActivityId)
		{//导航
			
			for(int i = 0; i<OperationActivityDataModel.ActivityTermList.Count; i++)
			{
				if (OperationActivityDataModel.ActivityTermList[i].ActivityId == item.GuideActivityId)
				{
					ChooseActivityMenu(i);
					return;
				}
			}
			
			return;
		}

		if (!item.CanGetReward)
		{
			if (-1 != item.GuideUI)
			{
				GameUtils.GotoUiTab(item.GuideUI,0);
				return;
			}
			return;
		}

		var type = item.ParentId;
		if (-1 != item.NeedItemId)
		{
			var itemRecord = Table.GetItemBase(item.NeedItemId);
			if (null == itemRecord)
			{
				return;
			}

			var haveItemCount = PlayerDataManager.Instance.GetItemTotalCount(item.NeedItemId).Count;
			if (haveItemCount < item.NeedItemCount)
			{
				var str = GameUtils.GetDictionaryText(270002);
				if (3 == item.NeedItemId)
				{
					str = GameUtils.GetDictionaryText(210102);
					GameUtils.GotoUiTab(79, 0);
				}
				EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(str));
				return;
			}
		}
		NetManager.Instance.StartCoroutine(OperationActivityClaimRewardCoroutine(type, id));

	}

	private void CheckAllCondition()
	{
 		foreach (var itemDataModel in mItemCache)
 		{
 			if (1==itemDataModel.HasGotReward)
 			{
 				itemDataModel.CanGetReward = false;
 			}
 			else
 			{
 				if (0 == PlayerDataManager.Instance.CheckCondition(itemDataModel.ConditionId) &&
 					itemDataModel.Count >= itemDataModel.Need &&
 					Game.Instance.ServerTime >= itemDataModel.StartTime && Game.Instance.ServerTime < itemDataModel.EndTime
 					)
 				{
 					itemDataModel.CanGetReward = true;
 				}
 				else
 				{
 					itemDataModel.CanGetReward = false;
 				}
 			}
 		}
	
	}

	private void ChooseActivityMenu(int idx)
	{
		OperationActivityDataModel.CurrentSelectPageIdx = idx;

		EventDispatcher.Instance.DispatchEvent(new OperationActivityPage_Event(idx));

		
		if (OperationActivityDataModel.ActivityTermList[idx].UIType == (int)OperationActivityUIType.Table)
		{
			OperationActivityDataModel.ActivityTermList[idx].TableData.SubTabIdx = 0;
			EventDispatcher.Instance.DispatchEvent(new OperationActivitySubPagekEvent(idx));
		}

		if (OperationActivityDataModel.ActivityTermList[idx].UIType == (int) OperationActivityUIType.ShowModel)
		{
			OperationActivityDataModel.ModelId = OperationActivityDataModel.ActivityTermList[idx].ModelPath;
		}
		else
		{
			OperationActivityDataModel.ModelId = -1;
		}
	}

	static OperationActivityItemDataModel AssignActivityItemData(MsgOperActivtyItem itemData, int type,OperationActivityItemDataModel itemModel = null)
	{
		if (null == itemModel)
		{
			itemModel = new OperationActivityItemDataModel();
		}
		itemModel.ItemId = itemData.Id;
		itemModel.ParentId = type;
		itemModel.ItemName = itemData.Name;
		itemModel.Desc = itemData.Desc;
		itemModel.Icon = itemData.Icon;
		itemModel.Need = itemData.Need;
		itemModel.Count = Math.Min(itemData.Need, itemData.Count);
		itemModel.StartTime = DateTime.FromBinary(itemData.StarTime);
		itemModel.EndTime = DateTime.FromBinary(itemData.EndTime);
		itemModel.ConditionId = itemData.Condition;
		//item.FlagId = itemData.FlagId;
		itemModel.AquiredTimes = itemData.AquiredTimes;
		itemModel.AquiredTotalTimes = itemData.TotalTimes;
		itemModel.RemainAquireTimes = string.Format(GameUtils.GetDictionaryText(100001156), Math.Max(itemModel.AquiredTotalTimes - itemModel.AquiredTimes, 0));
		itemModel.NeedItemId = itemData.NeedItemId;
		itemModel.NeedItemCount = itemData.NeedItemCount;
		itemModel.GuideActivityId = itemData.GuideActivityId;
		itemModel.GuideUI = itemData.GuideUI;
#if UNITY_EDITOR
		itemModel.Desc += string.Format("[{0}|{1}]", itemModel.StartTime.ToString("yyyy/MM/dd/HH:mm"), itemModel.EndTime.ToString("yyyy/MM/dd/HH:mm"));
#endif
		return itemModel;
	}

	public static List<ItemIdDataModel> ParseRewardItem(string str,ReadonlyObjectList<ItemIdDataModel> rewards)
	{
		var list = new List<ItemIdDataModel>();
		var strList = str.Split('|');
		for (int k = 0; k < rewards.Count; k++)
		{
			var rewardData = rewards[k];
			if (k < strList.Length)
			{
				
				var ret = strList[k].Split(',');
				if (2 == ret.Length)
				{
					rewardData.ItemId = int.Parse(ret[0]);
					rewardData.Count = int.Parse(ret[1]);
				}
				else
				{
					rewardData.ItemId = -1;
				}
			}
			else
			{
				rewardData.ItemId = -1;
			}
		}

		return list;
	}
}
