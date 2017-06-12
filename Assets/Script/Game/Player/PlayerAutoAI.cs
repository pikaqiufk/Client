#region using

using System;
using System.Collections;
using ClientDataModel;
using ClientService;
using DataTable;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class PlayerAutoAI : MonoBehaviour
{
    public DateTime HpCdTime;
    private float mDuration;
    public DateTime MpCdTime;
    // Use this for initialization
    public AutoCombatData AutoCombatData
    {
        get
        {
            return UIManager.Instance.GetController(UIConfig.SettingUI).GetDataModel("AutoCombat") as AutoCombatData;
        }
    }

    private void AutoUseMedicine(int isHp)
    {
        var playerDataModel = PlayerDataManager.Instance.PlayerDataModel;
        {
            // foreach(var item in PlayerDataModel.Bags.Bags[1].Items)
            var enumerator1 = (playerDataModel.Bags.Bags[1].Items).GetEnumerator();
            while (enumerator1.MoveNext())
            {
                var item = enumerator1.Current;
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (item.ItemId == -1)
                    {
                        continue;
                    }
                    var tbItem = Table.GetItemBase(item.ItemId);
                    if (tbItem == null)
                    {
                        continue;
                    }
                    if (tbItem.Type != 24000)
                    {
                        continue;
                    }
                    if (tbItem.UseLevel > PlayerDataManager.Instance.GetLevel())
                    {
                        continue;
                    }
                    if (tbItem.Exdata[2] == isHp)
                    {
                        StartCoroutine(UseItemCorotion(isHp, item.BagId, item.Index, 1));
                        break;
                    }
                }
            }
        }
    }

    private void MedicineCheck()
    {
        var playerDataModel = PlayerDataManager.Instance.PlayerDataModel;
        if (HpCdTime < DateTime.Now)
        {
            if (playerDataModel.Attributes.HpMax > 0
                && playerDataModel.Attributes.HpNow > 0
                && playerDataModel.Attributes.HpPercent < AutoCombatData.Hp)
            {
                AutoUseMedicine(0);
            }
        }

        if (MpCdTime < DateTime.Now)
        {
            if (playerDataModel.Attributes.MpMax > 0
                && playerDataModel.Attributes.MpNow > 0
                && playerDataModel.Attributes.MpPercent < AutoCombatData.Mp)
            {
                AutoUseMedicine(1);
            }
        }
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        StopAllCoroutines();
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        mDuration = 0.0f;
        HpCdTime = DateTime.Now;
        MpCdTime = DateTime.Now;
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        mDuration += Time.deltaTime;
        if (mDuration < 1.0f)
        {
            return;
        }
        mDuration = 0.0f;
        MedicineCheck();
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private IEnumerator UseItemCorotion(int isHp, int bagId, int itemIndex, int count = 1)
    {
        var msg = NetManager.Instance.UseItem(bagId, itemIndex, count);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                Logger.Debug("UseItem.................OK..");
                if (isHp == 0)
                {
                    HpCdTime = DateTime.Now.AddMilliseconds(GameUtils.AutoMedicineHpCd);
                    var skill = PlayerDataManager.Instance.PlayerDataModel.Bags.ItemWithSkillList[1];
                    skill.LastTime = skill.MaxTime;
                }
                else if (isHp == 1)
                {
                    MpCdTime = DateTime.Now.AddMilliseconds(GameUtils.AutoMedicineMpCd);
                    var skill = PlayerDataManager.Instance.PlayerDataModel.Bags.ItemWithSkillList[2];
                    skill.LastTime = skill.MaxTime;
                }
            }
            else if (msg.ErrorCode == (int) ErrorCodes.Error_SkillNoCD
                     || msg.ErrorCode == (int) ErrorCodes.Error_CharacterDie
                     || msg.ErrorCode == (int) ErrorCodes.Error_HpMax
                     || msg.ErrorCode == (int) ErrorCodes.Error_MpMax)
            {
                Logger.Warn("UseItem..................." + msg.ErrorCode);
            }
            else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterDie)
            {
//吃药需要先发到logic，再到scene上使用技能，scene上可能会有变化
                Logger.Warn("UseItem..................." + msg.ErrorCode);
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                Logger.Error("UseItem..................." + msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("UseItem..................." + msg.State);
        }
    }
}