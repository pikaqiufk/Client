using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PropertyDisplayE : MonoBehaviour, IPropertyChange
	{
	    public UILabel ChangeLabel;
	    public UISprite DownSprite;
	    private AttributerRangeDataModel propertyData;
	    public UILabel NameLabel;
	    public UILabel RangLabel;
	    public UISprite UpSprite;
	    public UILabel ValueLabel;
	
	    public AttributerRangeDataModel AttributeData
	    {
	        get { return propertyData; }
	        set
	        {
	            if (propertyData != null)
	            {
	                propertyData.PropertyChanged -= OnPropertyChange;
	            }
	            propertyData = value;
	            RefresDisplayName();
	            RefresDisplayChange();
	            RefresDisplayRange();
	            propertyData.PropertyChanged += OnPropertyChange;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (AttributeData != null)
	        {
	            AttributeData.PropertyChanged -= OnPropertyChange;
	        }
	
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
	        if (AttributeData != null && AttributeData.Type != -1)
	        {
	            RefresDisplayName();
	            RefresDisplayChange();
	            RefresDisplayRange();
	        }
	    }
	
	    public void OnPropertyChange(object sender, PropertyChangedEventArgs e)
	    {
	        if (e.PropertyName == "Type")
	        {
	            RefresDisplayName();
	        }
	        else if (e.PropertyName == "Value")
	        {
	            RefresDisplayValue();
	        }
	        else if (e.PropertyName == "Change")
	        {
	            RefresDisplayChange();
	        }
	        else if (e.PropertyName == "MinValue")
	        {
	            RefresDisplayRange();
	        }
	        else if (e.PropertyName == "MaxValue")
	        {
	            RefresDisplayRange();
	        }
	    }
	
	    private void RefresDisplayChange()
	    {
	        var fix = true;
	        var strValue = "";
	        if (AttributeData.Type == 99 || AttributeData.Type == 98)
	        {
	            fix = false;
	            strValue = GameUtils.AttributeValue(AttributeData.Type, AttributeData.Change + AttributeData.Value);
	        }
	        else
	        {
	            strValue = GameUtils.AttributeValue(AttributeData.Type, Mathf.Abs(AttributeData.Change));
	        }
	
	        ChangeLabel.text = strValue;
	
	        if (AttributeData.Change == 0)
	        {
	            UpSprite.gameObject.SetActive(false);
	            DownSprite.gameObject.SetActive(false);
	            ChangeLabel.gameObject.SetActive(false);
	        }
	        else
	        {
	            var chg = AttributeData.Change;
	            if (fix == false && AttributeData.Value != 0)
	            {
                    if (chg + AttributeData.Value != 0)
    	                chg = -chg;
                    else
                        chg = -1; // 等级/0 特殊处理为下降
	            }
	            if (chg > 0)
	            {
	                UpSprite.gameObject.SetActive(true);
	                DownSprite.gameObject.SetActive(false);
	                ChangeLabel.gameObject.SetActive(true);
	            }
	            else if (chg < 0)
	            {
	                UpSprite.gameObject.SetActive(false);
	                DownSprite.gameObject.SetActive(true);
	                ChangeLabel.gameObject.SetActive(true);
	            }
	            else
	            {
	                UpSprite.gameObject.SetActive(false);
	                DownSprite.gameObject.SetActive(false);
	                ChangeLabel.gameObject.SetActive(false);
	            }
	        }
	    }
	
	    private void RefresDisplayName()
	    {
	        var strName = GameUtils.AttributeName(AttributeData.Type);
	        NameLabel.text = strName;
	        RefresDisplayValue();
	    }
	
	    private void RefresDisplayRange()
	    {
	        if (AttributeData.MinValue == 0 || AttributeData.MaxValue == 0)
	        {
	            return;
	        }
	        var fix = true;
	        if (AttributeData.Type == 99 || AttributeData.Type == 98)
	        {
	            fix = false;
	        }
	        var minValue = GameUtils.AttributeValue(AttributeData.Type, AttributeData.MinValue, fix);
	        var maxValue = GameUtils.AttributeValue(AttributeData.Type, AttributeData.MaxValue, fix);
	        var strValue = "(" + minValue + "-" + maxValue + ")";
	        RangLabel.text = strValue;
	    }
	
	    private void RefresDisplayValue()
	    {
	        var strValue = GameUtils.AttributeValue(AttributeData.Type, AttributeData.Value);
	        ValueLabel.text = strValue;
	    }
	
	    public void RemovePropertyChange()
	    {
	        OnDestroy();
	    }
	}
}