
#region using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientService;
using DataContract;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;
#endregion

public class BattryLevelUpLogic : MonoBehaviour {
    public BindDataRoot Binding;

    public MonsterDataModel MonsterModel;

    public List<UISprite> TowerStataIcons = new List<UISprite>();
    //六个炮台按钮
    public List<UIButton> PaoTaiBtnList = new List<UIButton>();
    //提升技能的道具按钮
    public List<UIButton> UpLevelBtnList = new List<UIButton>();
    public UILabel PaoTaiNameLabel;
   
    public UILabel SkillTime;
    private bool mRemoveBind = true;

    private IControllerBase mController;
    void Start () {
        for (int i = 0; i < PaoTaiBtnList.Count; i++)
        {
            int PaotaiIndexd = i;
            PaoTaiBtnList[i].onClick.Add(new EventDelegate(() => ShowPaotaiInfo(PaotaiIndexd)));
        }
        for (int i = 0; i < UpLevelBtnList.Count; i++)
        {
            int idx = i;
            UpLevelBtnList[i].onClick.Add(new EventDelegate(() => UpLevelBtn(ID, idx)));
        }
	}
	
	// Update is called once per frame

    private void RefreshTowers(IEvent ievent)
    {
        RefreshTwoerIconStata();
    }
    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif
        if (mRemoveBind)
        {
            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
            EventDispatcher.Instance.AddEventListener(MieShiRefreshTowers_Event.EVENT_TYPE, RefreshTowers);
            mController = UIManager.Instance.GetController(UIConfig.BattryLevelUpUI);
            if (mController == null)
            {
                return;
            }
            MonsterModel = mController.GetDataModel("") as MonsterDataModel ;
            MonsterModel.PropertyChanged += OnMonsterPropertyChangedEvent;
            OnPaotaiBtn();
            ShowPaotaiInfo(0);
            Binding.SetBindDataSource(MonsterModel);
            EventDispatcher.Instance.DispatchEvent(new MieShiOnGXRankingBtn_Event());
            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);

        }
        // CreateFakeObj(DataModel.BossDataId);
        mRemoveBind = true;
      //  MonsterModel.UseDiamond = Table.GetMieShiPublic(1).CostNum;
      //  MonsterModel.UseProp = Table.GetMieShiPublic(1).ItemNum;
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
    void RefreshTwoerIconStata()
    {
        if (MonsterModel == null)
        {
            return;
        }

        if (MonsterModel.MonsterTowers == null)
        {
            return;
        }

        for (int i = 0; i < MonsterModel.MonsterTowers.Count; i++)
        {
            MonsterTowerDataModel mtdm = MonsterModel.MonsterTowers[i];

            if (TowerStataIcons != null && TowerStataIcons[i] != null)
            {
                if (mtdm.BloodPer <= 150)
                {
                    TowerStataIcons[i].spriteName = Table.GetIcon(2310000).Sprite;
                    if (TowerStataIcons[i].transform != null && TowerStataIcons[i].transform.GetComponent<UIButton>() != null)
                    {
                        TowerStataIcons[i].transform.GetComponent<UIButton>().normalSprite = TowerStataIcons[i].spriteName;
                    }

                }
                else if (mtdm.BloodPer > 150 && mtdm.BloodPer <= 300)
                {
                    TowerStataIcons[i].spriteName = Table.GetIcon(2310001).Sprite;
                    if (TowerStataIcons[i].transform != null && TowerStataIcons[i].transform.GetComponent<UIButton>() != null)
                    {
                        TowerStataIcons[i].transform.GetComponent<UIButton>().normalSprite = TowerStataIcons[i].spriteName;
                    }
                }
                else if (mtdm.BloodPer > 300 && mtdm.BloodPer <= 500)
                {
                    TowerStataIcons[i].spriteName = Table.GetIcon(2310002).Sprite;
                    if (TowerStataIcons[i].transform != null && TowerStataIcons[i].transform.GetComponent<UIButton>() != null)
                    {
                        TowerStataIcons[i].transform.GetComponent<UIButton>().normalSprite = TowerStataIcons[i].spriteName;
                    }
                }
            }
        }

        
    }
    private void OnCloseUiBindRemove(IEvent ievent)
    {
        var e = ievent as CloseUiBindRemove;
        if (e.Config != UIConfig.BattryLevelUpUI)
        {
            return;
        }
        if (e.NeedRemove == 0)
        {
            mRemoveBind = false;
        }
        else
        {
            if (mRemoveBind == false)
            {
                RemoveBindEvent();
            }
            mRemoveBind = true;
        }
    }
    private void RemoveBindEvent()
    {
        Binding.RemoveBinding();
        if (MonsterModel != null)
        {
            MonsterModel.PropertyChanged -= OnMonsterPropertyChangedEvent;
        }

        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
    }
    private void OnMonsterPropertyChangedEvent(object o, PropertyChangedEventArgs args)
    {
       RefreshTwoerIconStata();
    }
    public void OnPaotaiBtn()
    {
        EventDispatcher.Instance.DispatchEvent(new MieShiOnPaotaiBtn_Event());
    }
    private int ID { get; set; }
    private void ShowPaotaiInfo(int PaotaiIndexd)
    {
        MonsterModel.MonsterTowerIdx = PaotaiIndexd;
        ID = MonsterModel.CurMonsterTowers.TowerId;
        PaoTaiNameLabel.text = Table.GetNpcBase(226000 + PaotaiIndexd).Name.ToString();
        EventDispatcher.Instance.DispatchEvent(new MieShiOnPaotaiBtn_Event());
        for (int i = 0; i < PaoTaiBtnList.Count; i++)
        {

            if (i == PaotaiIndexd)
            {
                PaoTaiBtnList[PaotaiIndexd].transform.FindChild("guang").gameObject.SetActive(true);
            }
            else
            {
                PaoTaiBtnList[i].transform.FindChild("guang").gameObject.SetActive(false);
            }
        }
    }
    public void OnClickClose()
    {
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.BattryLevelUpUI));
    }
    public void OnGXRankingBtn()
    {
        EventDispatcher.Instance.DispatchEvent(new MieShiOnGXRankingBtn_Event());
    }
    public void Update()
    {
 
        if (MonsterModel.MonsterTowers != null && ID - 1 >= 0 && ID - 1 < MonsterModel.MonsterTowers.Count)
        {
            if (MonsterModel.MonsterTowers[ID - 1].Level > 1)
            {
                DateTime tm = MonsterModel.MonsterTowers[ID - 1].SkillTime.AddSeconds(-(double)(Table.GetMieShiPublic(1).LevelKeepTime));
                TimeSpan ts = MonsterModel.MonsterTowers[ID - 1].SkillTime - Game.Instance.ServerTime;
                if (ts.Milliseconds <= 0)
                {
                    SkillTime.text = "";
                }
                else
                {
                    SkillTime.text = ts.Minutes.ToString().PadRight(1, '0') + ":" + ts.Seconds.ToString().PadRight(1, '0');
                }
            }
            else
            {
                SkillTime.text = "";
            }
        }
    }
    public void ShowSkillTip()
    {
        EventDispatcher.Instance.DispatchEvent(new MieShiShowSkillTip_Event());
    }
    private void OnDestroy()
    {
        if (mRemoveBind == false)
        {
            RemoveBindEvent();
        }
        mRemoveBind = true;
    }
    private void OnDisable()
    {
        if (mRemoveBind)
        {
            RemoveBindEvent();
        }
    }
    private void UpLevelBtn(int id, int idx)
    {
        EventDispatcher.Instance.DispatchEvent(new BattryLevelUpUpLevelBtn_Event(id, idx));
    }
}
