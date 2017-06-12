using System;
using System.Collections.Generic;
using System.Collections;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;
public class TowerUpRewardLogic : MonoBehaviour
{
    public UISprite mLineSprite;
    public TowerUpRewardCell mItem;
    public GameObject objReward;
    public GameObject objTips;
    private List<TowerUpRewardCell> lIcons = new List<TowerUpRewardCell>();
    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        EventDispatcher.Instance.AddEventListener(UIEvent_TowerRewardCallBack.EVENT_TYPE, FunctionCallback);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveEventListener(UIEvent_TowerRewardCallBack.EVENT_TYPE, FunctionCallback);
    }

    void Refresh(int flag)
    {
        
    }
    public void Init()
    {

        IControllerBase mController = UIManager.Instance.GetController(UIConfig.MonsterSiegeUI);
        MonsterDataModel MonsterModel = mController.GetDataModel("MonsterModel") as MonsterDataModel;


        lIcons.Clear();
        float wight = mLineSprite.localSize.x;
        Table.ForeachMieshiTowerReward(tb =>
        {
            var temp = NGUITools.AddChild(objReward, mItem.gameObject);
            if (tb.ShowValue.Count == 2)
            {
                temp.transform.localPosition = new Vector3(tb.ShowValue[0] / 100.0f * wight, temp.transform.localPosition.y,
                    temp.transform.localPosition.z);
                temp.SetActive(true);

                var trigger = temp.GetComponentInChildren<UIEventTrigger>();
                trigger.onPress.Add(new EventDelegate(
                    () =>
                    {
                       OpenTips(tb.Id);
                    }));
                trigger.onRelease.Add(new EventDelegate(CloseTips));



                var tbItem = Table.GetItemBase(tb.StepReward);

                var cell = temp.GetComponent<TowerUpRewardCell>();
                if(cell != null)
                if (MonsterModel != null)
                {
                    bool bLingqu = (MonsterModel.RewardFlag & (1 << tb.Id)) > 0;
                    cell.SetLingqu(bLingqu);
                    cell.SetEffect((!bLingqu) && MonsterModel.MyUpTimes >= tb.TimesStep[0]);
                    if (tbItem != null)
                    {
                        cell.SetIcon(tbItem.Icon);
                    }
                }
                lIcons.Add(temp.GetComponent<TowerUpRewardCell>());
            }
                
            return true;
        });
    }

    public void OpenTips(int idx)
    {
        EventDispatcher.Instance.DispatchEvent(new UIEvent_ClickTowerReward(idx));
        
    }
    private void FunctionCallback(IEvent e)
    {
        UIEvent_TowerRewardCallBack evt = e as UIEvent_TowerRewardCallBack;
        switch (evt.nType)
        {
            case 0://关闭tips
                objTips.SetActive(true);
                break;
            case 1://设置已领取
            {
                if (lIcons.Count >= evt.nParam && evt.nParam>0)
                {
                    lIcons[evt.nParam-1].SetLingqu(true);
                }
            }
                break;
            case 2://设置可领取效果
            {
                if (lIcons.Count >= evt.nParam && evt.nParam > 0)
                {
                    lIcons[evt.nParam-1].SetEffect(true) ;
                }
            }
                break;
            case 3://设置可领取效果
                {
                    if (lIcons.Count >= evt.nParam && evt.nParam > 0)
                    {
                        lIcons[evt.nParam - 1].SetEffect(false);
                    }
                }
                break;
        }

    }

    public void CloseTips()
    {
        objTips.SetActive(false);
    }
}
