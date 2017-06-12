using UnityEngine;
using System.Collections;
using System;
using ClientDataModel;
using ClientService;

public static class LuaGameHelper 
{
	public static void ShowMessageBox(MessageBoxType boxType, string info, string title = "", Action okAction = null,Action cancelAction = null)
	{
		UIManager.Instance.ShowMessage(boxType, info, title, okAction, cancelAction);
	}

	public static void BindDataSource(BindDataRoot dataRoot, int controllerId, string dataName)
	{
		var ctrller = UIManager.Instance.GetControllerById(controllerId);
		if (null == ctrller)
		{
			Logger.Error("null==UIManager.Instance.GetControllerById({0})", controllerId);
			return;
		}

		dataRoot.SetBindDataSource(ctrller.GetDataModel(dataName));
	}

	public static void BindDataSourceNoticeData(BindDataRoot dataRoot)
	{
		dataRoot.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	}
    public static void BindDataSourcePlayerData(BindDataRoot dataRoot)
    {
        dataRoot.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
    }

	public static void SendJason(string str, Action<int, string> call,Action<int>errorCall)
	{
		NetManager.Instance.StartCoroutine(NetManager.Instance.SendJsonCoroutine(str, call, errorCall));
	}

	public static PlayerDataManager GetPlayerDataManager()
	{
		return PlayerDataManager.Instance;
	}
}

public static class LuaPlayerData
{
	public static string GetPlayerName()
	{
		return PlayerDataManager.Instance.GetName();
	}

	public static int GetLevel()
	{
		return PlayerDataManager.Instance.GetLevel();
	}

	public static int GetRole()
	{
		return PlayerDataManager.Instance.GetRoleId();
	}

	public static int GetServerId()
	{
		return PlayerDataManager.Instance.ServerId;
	}

	public static ulong GetPlayerId()
	{
		return PlayerDataManager.Instance.Guid;
	}

	public static int GetPkModel()
	{
		return PlayerDataManager.Instance.GetPkModel();
	}

	public static int GetRes(int resType)
	{
		return PlayerDataManager.Instance.GetRes(resType);
	}

	public static int GetExp()
	{
		return PlayerDataManager.Instance.GetExp();
	}

	public static int GetItemCount(int itemId)
	{
		return PlayerDataManager.Instance.GetItemCount(itemId);
	}

	public static int GetWingId()
	{
		return PlayerDataManager.Instance.GetWingId();
	}

	public static int GetTalentLayer(int nTalentId)
	{
		return PlayerDataManager.Instance.GetTalentLayer(nTalentId);
	}

	public static int GetNormalSkill(bool weapon = false)
	{
		return PlayerDataManager.Instance.GetNormalSkill(weapon);
	}

	public static int GetSkillNoWeapon()
	{
		return PlayerDataManager.Instance.GetSkillNoWeapon();
	}
	public static int GetSkillLevel(int nSkillId)
	{
		return PlayerDataManager.Instance.GetSkillLevel(nSkillId);
	}

	public static int GetExData(int index)
	{
		return PlayerDataManager.Instance.GetExData(index);
	}
	public static bool GetFlag(int index)
    {
		return PlayerDataManager.Instance.GetFlag(index);
    }
}
