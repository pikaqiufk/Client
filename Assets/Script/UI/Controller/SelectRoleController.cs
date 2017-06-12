#region using

using System;
using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class SelectRoleController : IControllerBase
{
    public SelectRoleController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(UIEvent_SelectRole_Enter.EVENT_TYPE, Button_EnterGame);
        EventDispatcher.Instance.AddEventListener(UIEvent_SelectRole_Back.EVENT_TYPE, Button_Back);
        EventDispatcher.Instance.AddEventListener(UIEvent_SelectRole_Index.EVENT_TYPE, Button_Select);
        EventDispatcher.Instance.AddEventListener(UIEvent_ShowCreateRole.EVENT_TYPE, Button_ShowCreateRole);
        EventDispatcher.Instance.AddEventListener(UIEvent_GetRandomName.EVENT_TYPE, Button_GetRandomName);
        EventDispatcher.Instance.AddEventListener(UIEvent_CreateRoleType_Change.EVENT_TYPE, CreateRoleTypeChange);
        EventDispatcher.Instance.AddEventListener(UIEvent_CreateRole.EVENT_TYPE, Button_CreateRole);
        EventDispatcher.Instance.AddEventListener(NameChange.EVENT_TYPE, InPutName);
    }

    public LoginDataModel DataModel;
    public bool InPutname;
    public bool mBackToLogin;
    public CharacterLoginDataModel NowSelectRole;
    //取消按钮
    public void Button_Back(IEvent ievent)
    {
        Game.Instance.ExitToServerList();
    }

    public void Button_CreateRole(IEvent ievent)
    {
        DataModel.CreateName = DataModel.CreateName.Trim();
        if (!GameUtils.CheckName(DataModel.CreateName))
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 300900);
            return;
        }
        if (GameUtils.CheckSensitiveName(DataModel.CreateName))
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 200004120);
            return;
        }
        if (GameUtils.ContainEmoji(DataModel.CreateName))
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 725);
            return;
        }

        if (!GameUtils.CheckLanguageName(DataModel.CreateName))
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 725);
            return;
        }

        NetManager.Instance.StartCoroutine(CreateCharacterCoroutine());
    }

    //确认按钮
    public void Button_EnterGame(IEvent ievent)
    {
        var ee = ievent as UIEvent_SelectRole_Enter;
    }

    //随机一个姓名
    public void Button_GetRandomName(IEvent ievent)
    {
        var actorTable = Table.GetActor(DataModel.CreateType);
        DataModel.CreateName = StringConvert.GetRandomName(actorTable.Sex);
    }

    //选角色按钮
    public void Button_Select(IEvent ievent)
    {
        var ee = ievent as UIEvent_SelectRole_Index;
        var nIndex = ee.index;
        if (nIndex < 0 || nIndex >= DataModel.Characters.Count)
        {
            return;
        }
        if (DataModel.Characters[nIndex].CharacterId == ulong.MaxValue)
        {
            return;
        }

        PlayerDataManager.Instance.mInitBaseAttr.CharacterId = DataModel.Characters[nIndex].CharacterId;
        PlayerDataManager.Instance.mInitBaseAttr.RoleId = DataModel.Characters[nIndex].RoleId;

        // NowSelectRole = DataModel.Characters[nIndex];
        NowSelectRole.Clone(DataModel.Characters[nIndex]);
    }

    //创建角色按钮
    public void Button_ShowCreateRole(IEvent ievent)
    {
        if (string.IsNullOrEmpty(DataModel.CreateName))
        {
            Button_GetRandomName(null);
        }
        var ee = ievent as UIEvent_ShowCreateRole;

        if (mBackToLogin)
        {
            if (ee.index == 0)
            {
                Game.Instance.ExitToServerList();
            }
        }
        else
        {
            DataModel.showCreateFrame = ee.index;
        }
    }

    public IEnumerator CreateCharacterCoroutine()
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.CreateCharacter(PlayerDataManager.Instance.ServerId, DataModel.CreateType,
                DataModel.CreateName);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var list = PlayerDataManager.Instance.CharacterLists;
                    list = msg.Response.Info;
                    // RefreshDate(msg.Response.Info, msg.Response.SelectId);
                    //新需求,创建完人物直接进入游戏
                    GameLogic.PlayFirstEnterGameCG = 1;
                    var role = list[list.Count - 1];
                    var SelectedRoleId = role.CharacterId;

                    var serverName = PlayerDataManager.Instance.ServerName;
                    var serverId = PlayerDataManager.Instance.ServerId;
                    PlayerDataManager.Instance.CharacterFoundTime = Extension.FromServerBinary(msg.Response.CharacterFoundTime);
                    var ts = PlayerDataManager.Instance.CharacterFoundTime - DateTime.Parse("1970-1-1");
                    var time = (int) ts.TotalSeconds;
                    PlatformHelper.CollectionCreateRoleDataForKuaifa(SelectedRoleId.ToString(), DataModel.CreateName,
                        serverId.ToString(), serverName, time.ToString());

                    {
//这里实现给主角名字赋值，为了后面对话时用
                        PlayerDataManager.Instance.PlayerDataModel.CharacterBase.Name = role.Name;
                    }
#if UNITY_EDITOR
                    var skip = true;
#else
					bool skip = true;
					//bool skip = list.Count>1;
#endif
                    //播放创建角色时的CG
					/*
                    if (0 == DataModel.CreateType)
                    {
                        PlayCG.Instance.PlayCGFile("Video/jianshi.txt",
                            () => { NetManager.Instance.CallEnterGame(SelectedRoleId); }, skip, false);
                    }
                    else if (1 == DataModel.CreateType)
                    {
                        PlayCG.Instance.PlayCGFile("Video/fashi.txt",
                            () => { NetManager.Instance.CallEnterGame(SelectedRoleId); }, skip, false);
                    }
                    else if (2 == DataModel.CreateType)
                    {
                        PlayCG.Instance.PlayCGFile("Video/gongshou.txt",
                            () => { NetManager.Instance.CallEnterGame(SelectedRoleId); }, skip, false);
                    }
                    else
                    {
                        NetManager.Instance.CallEnterGame(SelectedRoleId);
                    }*/
					NetManager.Instance.CallEnterGame(SelectedRoleId);
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_NAME_IN_USE)
                    {
                        var dicId = msg.ErrorCode + 200000000;
                        var tbDic = Table.GetDictionary(dicId);
                        var info = "";
                        info = tbDic.Desc[GameUtils.LanguageIndex];
                        UIManager.Instance.ShowMessage(MessageBoxType.Ok, info, "");
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
            else
            {
                Logger.Debug("CreateCharacter.................." + msg.State);
            }
        }
    }

    //选择创建人物的职业
    public void CreateRoleTypeChange(IEvent ievent)
    {
        var ee = ievent as UIEvent_CreateRoleType_Change;
        if (ee != null)
        {
            DataModel.CreateType = ee.index;
        }
        if (!InPutname)
        {
            Button_GetRandomName(null);
        }
        var table = Table.GetActor(ee.index);
        if (null != table)
        {
            var index = UnityEngine.Random.Range(0, 3);
            SoundManager.Instance.StopAllSoundEffect();
            SoundManager.Instance.PlaySoundEffect(table.Dubbing[index]);
        }
    }

    public void InPutName(IEvent ievent)
    {
        var e = ievent as NameChange;
        InPutname = e.Idx;
    }

    public void RefreshCharacterDataModel(int index, CharacterSimpleInfo info)
    {
        var dataModel = DataModel.Characters[index];
        if (info != null)
        {
            dataModel.CharacterId = info.CharacterId;
            dataModel.Level = info.Level;
            dataModel.Name = info.Name;
            dataModel.RoleId = info.RoleId;
            // dataModel.Type = info.Type;
            dataModel.Type = info.RoleId;
        }
        else
        {
            //预备给删除功能
            var newCharacterData = new CharacterLoginDataModel();
            dataModel.CharacterId = newCharacterData.CharacterId;
            dataModel.Level = newCharacterData.Level;
            dataModel.Name = newCharacterData.Name;
            dataModel.RoleId = newCharacterData.RoleId;
            dataModel.Type = newCharacterData.Type;
        }
        dataModel.showCreateButton = dataModel.CharacterId == 0 ? 0 : 1;
    }

    public void CleanUp()
    {
        if (DataModel == null)
        {
            DataModel = new LoginDataModel();
            var DataModelCharactersCount0 = DataModel.Characters.Count;
            for (var i = 0; i < DataModelCharactersCount0; i++)
            {
                DataModel.Characters[i] = new CharacterLoginDataModel();
            }
            NowSelectRole = new CharacterLoginDataModel();
        }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name.Equals("GetCreateShow"))
        {
            return DataModel.showCreateFrame;
        }
        return null;
    }

    public void OnShow()
    {
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name.Equals("selectRole"))
        {
            return NowSelectRole;
        }
        return DataModel;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as SelectRoleArguments;
        if (null == args)
        {
            return;
        }

        DataModel.ServerName = args.ServerName;

        var selectId = args.SelectId;
        var characterSimpleInfos = args.CharacterSimpleInfos;
		
        var selectCount = 0;
        var DataModelCharactersCount1 = DataModel.Characters.Count;
        for (var i = 0; i < DataModelCharactersCount1; i++)
        {
            var info = characterSimpleInfos.Count > i ? characterSimpleInfos[i] : null;
            RefreshCharacterDataModel(i, info);
            if (info != null && selectId == info.CharacterId)
            {
                selectCount = i;
            }
        }
		 
        DataModel.CharacterCount = characterSimpleInfos.Count;
        //创建人物后选中刚创建的新角色
		DataModel.SelectIndex = selectCount;
        DataModel.showCreateFrame = 0;
        DataModel.CreateName = "";
	    if (args.Type == SelectRoleArguments.OptType.SelectMyRole)
	    {
			var e = new UIEvent_SelectRole_Index(selectCount);
			Button_Select(e);    
	    }
        

        //空号上来先创建人物
        if (characterSimpleInfos.Count == 0)
        {
            DataModel.showCreateFrame = 1;
            mBackToLogin = true;
        }
        else
        {
            DataModel.showCreateFrame = 0;
        }
    }

    public FrameState State { get; set; }
}