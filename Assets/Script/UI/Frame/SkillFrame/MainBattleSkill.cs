using System;
#region using

using System.Collections;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MainBattleSkill : MonoBehaviour
	{
	    //技能cd文字
	    public UILabel CdLabel;
	    //充能层数
	    public UILabel ChargeLabel;
	    //公共cd的光圈
	    public UISprite ChargeRadialCover;
	    //冷却遮罩
	    public UISprite Cover;
	    private bool isShowCover;
	    private Coroutine effectCoroutine;
	    public GameObject EffectObj;
	    private eEffectState effectState = eEffectState.Init;
	    public UISprite Mask;
	    //冷却光圈
	    public UISprite RadialCover;
	    private bool isShowRadial;
	    public UIButton SkillButton;
	
	    public enum eEffectState
	    {
	        Init = 0,
	        Cd = 1,
	        Show = 2
	    }
	
	    public SkillItemDataModel SkillItemDataModel { get; set; }
	
	    public IEnumerator EffectCoroutineFun(float seconds)
	    {
	        yield return new WaitForSeconds(0.1f);
	        if (!EffectObj.activeSelf)
	        {
	            EffectObj.SetActive(true);
	        }
	        yield return new WaitForSeconds(seconds);
	        if (EffectObj.activeSelf)
	        {
	            EffectObj.SetActive(false);
	        }
	    }
	
	    private void OnDestroy()
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
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        effectState = eEffectState.Init;
	        if (EffectObj.activeSelf)
	        {
	            EffectObj.SetActive(false);
	        }
	    
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	}
	
	    public void OnSkillUse(BattleSkillRootFrame.DelegateBtnClick delegateBtnClick)
	    {
	        if (SkillItemDataModel.SkillId <= 0)
	        {
	            return;
	        }
	
	        if (PlayerDataManager.Instance.PlayerDataModel.SkillData.CommonCoolDown > 0)
	        {
	            return;
	        }
	
	        if ((SkillItemDataModel.CoolDownTime > 0 && SkillItemDataModel.ChargeLayerTotal <= 1) ||
	            SkillItemDataModel.ChargeLayer == 0)
	        {
	            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(714));
	        }
	        //delegateBtnClick(1, SkillItemDataModel.SkillId);
	    }
	
	    private void UpdateCoolDown()
	    {
	        if (PlayerDataManager.Instance.PlayerDataModel.SkillData.CommonCoolDown == 0
	            && (SkillItemDataModel.CoolDownTime) == 0)
	        {
	            isShowRadial = false;
	            ChargeRadialCover.gameObject.SetActive(false);
	            isShowCover = false;
	            if (effectState == eEffectState.Cd)
	            {
	                effectState = eEffectState.Show;
	            }
	        }
	
	
	        if (PlayerDataManager.Instance.PlayerDataModel.SkillData.CommonCoolDown > 0)
	        {
	            isShowRadial = true;
	            isShowCover = true;
	        }
	        else
	        {
	            if (SkillItemDataModel.CoolDownTime > 0)
	            {
	                isShowCover = true;
	                if (SkillItemDataModel.ChargeLayerTotal > 1)
	                {
	                    isShowRadial = false;
	                    ChargeRadialCover.gameObject.SetActive(true);
	                    if (SkillItemDataModel.ChargeLayer > 0)
	                    {
	                        isShowCover = false;
	                    }
	                }
	                else
	                {
	                    isShowRadial = true;
	                }
	            }
	            else
	            {
	                isShowCover = false;
	            }
	        }
	
	        if (SkillItemDataModel.CoolDownTime > 0)
	        {
	            CdLabel.gameObject.SetActive(true);
	        }
	        else
	        {
	            CdLabel.gameObject.SetActive(false);
	        }
	
	
	        if (Cover.gameObject.activeSelf != isShowCover)
	        {
	            Cover.gameObject.SetActive(isShowCover);
	        }
	
	        if (RadialCover.gameObject.activeSelf != isShowRadial)
	        {
	            RadialCover.gameObject.SetActive(isShowRadial);
	        }
	
	        if (effectState == eEffectState.Show && gameObject.activeInHierarchy)
	        {
	            if (effectCoroutine != null)
	            {
	                StopCoroutine(effectCoroutine);
	            }
	            effectCoroutine = StartCoroutine(EffectCoroutineFun(0.9f));
	            effectState = eEffectState.Init;
	        }
	    }
	
	    public void ChangeActive(bool flag)
	    {
	        Mask.gameObject.SetActive(!flag);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        Cover.gameObject.SetActive(false);
	        RadialCover.gameObject.SetActive(false);
	        CdLabel.gameObject.SetActive(false);
	        ChargeRadialCover.gameObject.SetActive(false);
	        EffectObj.SetActive(false);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void Tick()
	    {
	        if (null == SkillItemDataModel)
	        {
	            return;
	        }
	
	        if (SkillItemDataModel.CoolDownTime > 0)
	        {
	            RadialCover.fillAmount = SkillItemDataModel.CoolDownTime/SkillItemDataModel.CoolDownTimeTotal;
	            CdLabel.text = ((int) SkillItemDataModel.CoolDownTime + 1).ToString();
	            ChargeRadialCover.fillAmount = RadialCover.fillAmount;
	            effectState = eEffectState.Cd;
	        }
	
	        if (PlayerDataManager.Instance.PlayerDataModel.SkillData.CommonCoolDown > 0)
	        {
	            if (SkillItemDataModel.CoolDownTime < PlayerDataManager.Instance.PlayerDataModel.SkillData.CommonCoolDown)
	            {
	                RadialCover.fillAmount = PlayerDataManager.Instance.PlayerDataModel.SkillData.CommonCoolDown
	                                         /PlayerDataManager.Instance.PlayerDataModel.SkillData.CommonCoolDownTotal;
	            }
	        }
	        UpdateCoolDown();
	    }
	}
}