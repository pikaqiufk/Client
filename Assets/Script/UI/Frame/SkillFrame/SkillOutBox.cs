using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SkillOutBox : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public UILabel CurrentCount;
	    private TweenPosition mTweenPosition;
	    private TweenScale mTweenScale;
	    public int SkillId;
	    public UILabel skillName1;
	    //展开后的skillbox
	    public GameObject TalentBox;
	    public SkillBoxDataModel BoxDataModel { get; set; }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        BoxDataModel.PropertyChanged -= OnPropertyChangedEvent;
	        Binding.RemoveBinding();
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnPropertyChangedEvent(object o, PropertyChangedEventArgs args)
	    {
	        if (args.PropertyName == "CurrentCount")
	        {
	            RefreshCurrentCount();
	        }
	    }
	
	    private void OnSkillBallClose()
	    {
// 	        var e = new UIEvent_SkillFrame_OnSkillBallClose(BoxDataModel);
// 	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnSkillBallOpen()
	    {
	        var e = new UIEvent_SkillFrame_OnSkillTalentSelected(BoxDataModel.SkillId);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void RefreshCurrentCount()
	    {
	        if (null != CurrentCount)
	        {
	            CurrentCount.text = string.Format("{0}/{1}", BoxDataModel.CurrentCount, BoxDataModel.MaxCount);
	        }
	    }
	
	    private void RefreshSkillBox(bool doTween)
	    {
	        if (BoxDataModel.ShowSkillBox == 0)
	        {
	            if (null != TalentBox)
	            {
	                TalentBox.SetActive(false);
	                if (doTween)
	                {
	                    var e = new UIEvent_SkillFrame_OnSkillBallPlayTween(gameObject, false);
	                    EventDispatcher.Instance.DispatchEvent(e);
	                }
	            }
	        }
	        else
	        {
	            if (null != TalentBox)
	            {
	                TalentBox.SetActive(true);
	                if (doTween)
	                {
	                    var e = new UIEvent_SkillFrame_OnSkillBallPlayTween(gameObject, true);
	                    EventDispatcher.Instance.DispatchEvent(e);
	                }
	            }
	        }
	    }
	
	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        //优先查找完成之后再设置隐藏
	        RefreshCurrentCount();
	        BoxDataModel.PropertyChanged += OnPropertyChangedEvent;
	        Binding.SetBindDataSource(BoxDataModel);
	        var name = Table.GetSkill(SkillId).Name;
	        if (skillName1 == null)
	        {
	            Logger.Error("talent skillname uilabel is null skillID = {0}", SkillId);
	        }
	        else
	        {
	            skillName1.text = name;
	        }
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	}
}