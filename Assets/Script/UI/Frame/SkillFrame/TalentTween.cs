using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TalentTween : MonoBehaviour
	{
	    private readonly List<TweenPosition> leftTweenPosList = new List<TweenPosition>();
	    private readonly List<TweenPosition> rightTweenPosList = new List<TweenPosition>();
	    private readonly List<TweenScale> scaleTweenList = new List<TweenScale>();
	    private readonly List<GameObject> skillBoxList = new List<GameObject>();
	
	    private void SetupTween()
	    {
	        var skillBoxListCount1 = skillBoxList.Count;
	        for (var i = 0; i < skillBoxListCount1; i++)
	        {
	            var box = skillBoxList[i];
	            var boxTransform = box.transform;
	
	            var tweenRight = box.AddComponent<TweenPosition>();
	            tweenRight.enabled = false;
	            tweenRight.from = boxTransform.localPosition;
	            tweenRight.to = boxTransform.localPosition;
	            tweenRight.duration = 0.3f;
	            tweenRight.to.x += 65;
	
	            var tweenLeft = box.AddComponent<TweenPosition>();
	            tweenLeft.enabled = false;
	            tweenLeft.from = boxTransform.localPosition;
	            tweenLeft.to = boxTransform.localPosition;
	            tweenLeft.duration = 0.3f;
	            tweenLeft.to.x -= 65;
	
	            var t = box.gameObject.transform.FindChild("TalentInfo");
	            var scale = t.gameObject.AddComponent<TweenScale>();
	            scale.enabled = false;
	            scale.duration = 0.3f;
	            scale.from = new Vector3(0.1f, 1, 1);
	            scale.to = new Vector3(1, 1, 1);
	            rightTweenPosList.Add(tweenRight);
	            leftTweenPosList.Add(tweenLeft);
	            scaleTweenList.Add(scale);
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_SkillFrame_OnSkillBallPlayTween.EVENT_TYPE,
	            OnPlayTalent);
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnPlayTalent(IEvent ievent)
	    {
	        var e = ievent as UIEvent_SkillFrame_OnSkillBallPlayTween;
	        var index = -1;
	        var skillBoxListCount2 = skillBoxList.Count;
	        for (var i = 0; i < skillBoxListCount2; i++)
	        {
	            if (e != null && skillBoxList[i] == e.obj)
	            {
	                index = i;
	                break;
	            }
	        }
	
	        if (e.bFoward)
	        {
	            RunForward(index);
	        }
	        else
	        {
	            RunReverse(index);
	        }
	    }
	
	    private void RunForward(int index)
	    {
	        if (index == -1)
	        {
	            Logger.Error("SkillBox PlayForward error!");
	            return;
	        }
	
	        var current = scaleTweenList[index];
	        current.ResetToBeginning();
	        current.PlayForward();
	
	        var mTweenPositionListLeftCount3 = leftTweenPosList.Count;
	        for (var i = 0; i < mTweenPositionListLeftCount3; i++)
	        {
	            leftTweenPosList[i].PlayReverse();
	        }
	
	        var mTweenPositionListRightCount4 = rightTweenPosList.Count;
	        for (var i = 0; i < mTweenPositionListRightCount4; i++)
	        {
	            rightTweenPosList[i].PlayReverse();
	        }
	
	
	        if (index > 0)
	        {
	            var left = leftTweenPosList[index - 1];
	            left.PlayForward();
	        }
	
	        if (index < skillBoxList.Count - 1)
	        {
	            var right = rightTweenPosList[index + 1];
	            right.PlayForward();
	        }
	    }
	
	    private void RunReverse(int index)
	    {
	        if (index == -1)
	        {
	            Logger.Error("SkillBox PlayReverse error!");
	            return;
	        }
	
	        if (index > 0)
	        {
	            var left = leftTweenPosList[index - 1];
	            left.PlayReverse();
	        }
	
	        if (index < skillBoxList.Count - 1)
	        {
	            var right = rightTweenPosList[index + 1];
	            right.PlayReverse();
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        var logics = gameObject.GetComponentsInChildren<SkillOutBox>();
	        skillBoxList.Clear();
	        var logicsLength0 = logics.Length;
	        for (var i = 0; i < logicsLength0; i++)
	        {
	            skillBoxList.Add(logics[i].gameObject);
	        }
	        SetupTween();
	        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_OnSkillBallPlayTween.EVENT_TYPE, OnPlayTalent);
	
	
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