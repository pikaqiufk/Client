#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion

public class SkillFrameTipController : IControllerBase
{
    public SkillFrameTipController()
    {
        CleanUp();
    }

    private int currentShowTalentId;
    public SkillDataModel DataModel;
    public int mEquipSkillDirtyMark;
    public bool skillChanged;



    public void CleanUp()
    {
        DataModel = new SkillDataModel();
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
        var args = data as SkillTipFrameArguments;
        var skillData = Table.GetSkill(args.idSkill);
        DataModel.MieshiSkillItem.SkillId = args.idSkill;
        DataModel.MieshiSkillItem.SkillName = skillData.Name;
        DataModel.MieshiSkillItem.SkillNeedMp = skillData.NeedMp;
        DataModel.MieshiSkillItem.SkillTargetConut = skillData.TargetCount;
        DataModel.MieshiSkillItem.SkillCD = skillData.Cd;
        DataModel.MieshiSkillItem.SkillDes = skillData.Desc;
        if (args.idNextSkill > 0)
        {
            DataModel.MieshiSkillItem.NextLevelSkillDes = Table.GetSkill(args.idNextSkill).Desc;
        }
        else
        {
            DataModel.MieshiSkillItem.NextLevelSkillDes = "";
        }
        DataModel.MieshiSkillItem.SkillLv = String.Format(Table.GetDictionary(240302).Desc[0], args.iLevel);
    }

    

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}