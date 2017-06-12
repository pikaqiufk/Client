using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PropertyDisplayD : MonoBehaviour, IPropertyChange
	{
	    public UILabel ChangeLabel;
	    public UISprite DownSprite;
	    private AttributeChangeDataModel propertyData;
	    public UILabel NameLabel;
	    public UISprite UpSprite;
	    public UILabel ValueLabel;
	
	    public AttributeChangeDataModel AttributeData
	    {
	        get { return propertyData; }
	        set
	        {
	            if (propertyData != null)
	            {
	                propertyData.PropertyChanged -= OnPropertyChange;
	            }
	            propertyData = value;
	            RefresDisplay();
	            RefresDisplayChange();
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
	            RefresDisplay();
	            RefresDisplayChange();
	        }
	    }
	
	    public void OnPropertyChange(object sender, PropertyChangedEventArgs e)
	    {
	        if (e.PropertyName == "Type")
	        {
	            RefresDisplay();
	        }
	        else if (e.PropertyName == "Value")
	        {
	            RefresDisplay();
	        }
	        else if (e.PropertyName == "ValueEx")
	        {
	            RefresDisplay();
	        }
	        else if (e.PropertyName == "Change")
	        {
	            RefresDisplayChange();
	        }
	        else if (e.PropertyName == "ChangeEx")
	        {
	            RefresDisplayChange();
	        }
	    }
	
	    private void RefresDisplay()
	    {
	        var strName = GameUtils.AttributeName(AttributeData.Type);
	        NameLabel.text = strName;
	        var strValue = GameUtils.AttributeValue(AttributeData.Type, AttributeData.Value);
	        if (AttributeData.ValueEx > 0)
	        {
	            var strValueEx = GameUtils.AttributeValue(AttributeData.Type, AttributeData.ValueEx);
	            strValue = strValue + "-" + strValueEx;
	        }
	        ValueLabel.text = strValue;
	    }
	
	    private void RefresDisplayChange()
	    {
	        if (AttributeData.Change == 0)
	        {
	            ChangeLabel.gameObject.SetActive(false);
	            if (UpSprite != null)
	            {
	                UpSprite.gameObject.SetActive(false);
	            }
	            if (DownSprite)
	            {
	                DownSprite.gameObject.SetActive(false);
	            }
	            return;
	        }
	        if (AttributeData.Change > 0)
	        {
	            ChangeLabel.gameObject.SetActive(true);
	            if (UpSprite != null)
	            {
	                UpSprite.gameObject.SetActive(true);
	            }
	            if (DownSprite)
	            {
	                DownSprite.gameObject.SetActive(false);
	            }
	        }
	        else
	        {
	            ChangeLabel.gameObject.SetActive(true);
	            if (UpSprite != null)
	            {
	                UpSprite.gameObject.SetActive(false);
	            }
	            if (DownSprite)
	            {
	                DownSprite.gameObject.SetActive(true);
	            }
	        }
	        var strValue = GameUtils.AttributeValue(AttributeData.Type, AttributeData.Change);
	
	        if (AttributeData.ChangeEx > 0)
	        {
	            var strValueEx = GameUtils.AttributeValue(AttributeData.Type, AttributeData.ChangeEx);
	            strValue = strValue + "-" + strValueEx;
	        }
	        ChangeLabel.text = strValue;
	    }
	
	    public void RemovePropertyChange()
	    {
	        OnDestroy();
	    }
	}
}