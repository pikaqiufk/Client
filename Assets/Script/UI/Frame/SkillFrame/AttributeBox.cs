using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class AttributeBox : MonoBehaviour
	{
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_SkillFrame_RefreshTalentPanel.EVENT_TYPE, UpdateAttrBox);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void UpdateAttrBox(IEvent unuse)
	    {
	        var dataModel = PlayerDataManager.Instance.PlayerDataModel.SkillData;
	
	        var labels = gameObject.GetComponentsInChildren<UILabel>();
	        const int defaultHeight = 50;
	        var sprites = gameObject.GetComponentsInChildren<UISprite>();
	        var count = sprites.Length;
	        for (var j = 0; j < count; j++)
	        {
	            sprites[j].height = defaultHeight + dataModel.AttrPanel.Count*23;
	        }
	
	        var i = 0;
	        {
	            var __array1 = labels;
	            var __arrayLength1 = __array1.Length;
	            for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
	            {
	                var label = __array1[__i1];
	                {
	                    if (i < dataModel.AttrPanel.Count)
	                    {
	                        var attr = dataModel.AttrPanel[i];
	                        label.text = string.Format("{0}+{1}", attr.AttrName, attr.AttrStringValue);
	                    }
	                    else
	                    {
	                        label.text = string.Empty;
	                    }
	                    ++i;
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
	
	        var dataModel = PlayerDataManager.Instance.PlayerDataModel.SkillData;
	        UpdateAttrBox(null);
	        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_RefreshTalentPanel.EVENT_TYPE, UpdateAttrBox);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    // Update is called once per frame
	    private void Update()
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
	}
}