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



public class AcientBattleFieldFrameCtrler : IControllerBase
{
	private AcientBattleFieldDataModel DataModel;
	private bool Inited = false;
	public AcientBattleFieldFrameCtrler()
	{
		DataModel = new AcientBattleFieldDataModel();

		if (!Inited)
		{
			Inited = true;

			int i = 0;
			foreach (var item in DataModel.ItemList)
			{
				item.Id = 1;
				item.Show = 0;
			}
			Table.ForeachAcientBattleField((tb) =>
			{
				if (i >= DataModel.ItemList.Count)
				{
					return false;
				}
				var model = DataModel.ItemList[i];
				model.Id = tb.Id;
				model.Show = 1;
				i++;
				return true;
			});	
		}


		CleanUp();

		EventDispatcher.Instance.AddEventListener(UIAcientBattleFieldMenuItemClickEvent.EVENT_TYPE, OnClickPageBtn);
	
	}

    public void CleanUp()
    {
	    DataModel.CurrentSelectPageIdx = 0;
	    DataModel.ModelId = -1;
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
		DataModel.CurrentSelectPageIdx = 0;
		DataModel.ModelId = -1;
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
	    ChooseActivityMenu(0);
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
		return DataModel;
    }

    public FrameState State { get; set; }

	private void OnClickPageBtn(IEvent ievent)
	{
		var e = ievent as UIAcientBattleFieldMenuItemClickEvent;
		ChooseActivityMenu(e.Idx);
	}

	private void ChooseActivityMenu(int idx)
	{
		if (idx < 0 || idx >= DataModel.ItemList.Count)
		{
			return;
		}

		DataModel.CurrentSelectPageIdx = idx;
		DataModel.Info.Id = DataModel.ItemList[idx].Id;
		var tb = Table.GetAcientBattleField(DataModel.Info.Id);
		//var tbChar = Table.GetCharacterBase(tb.CharacterBaseId);
		//DataModel.ModelId = tbChar.CharModelID;
		DataModel.ModelId = tb.CharacterBaseId;
		for(int i=0; i<tb.Item.Length && i<DataModel.Info.Rewards.Count; i++)
		{
			DataModel.Info.Rewards[i].ItemId = tb.Item[i];
			DataModel.Info.Rewards[i].Count = tb.ItemCount[i];
		}
		
		
	}

}
