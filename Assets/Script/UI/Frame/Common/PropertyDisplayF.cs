#region using

using System.ComponentModel;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PropertyDisplayF : MonoBehaviour, IPropertyChange
	{
	    private AttributeBaseDataModel propertyData;
	    public UILabel TextLabel;
	
	    public AttributeBaseDataModel AttributeData
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
	            propertyData.PropertyChanged += OnPropertyChange;
	        }
	    }
	
	    private void OnDestroy()
	    {
	        if (AttributeData != null)
	        {
	            AttributeData.PropertyChanged -= OnPropertyChange;
	        }
	    }
	
	    private void OnEnable()
	    {
	        if (AttributeData != null && AttributeData.Type != -1)
	        {
	            RefresDisplay();
	        }
	    }
	
	    private void OnPropertyChange(object sender, PropertyChangedEventArgs e)
	    {
	        if (sender == AttributeData)
	        {
	            RefresDisplay();
	        }
	    }
	
	    private void RefresDisplay()
	    {
	        var strName = GameUtils.AttributeName(AttributeData.Type);
	        var strValue = GameUtils.AttributeValue(AttributeData.Type, AttributeData.Value);
	        if (AttributeData.ValueEx != 0)
	        {
	            strValue = strValue + "-" + GameUtils.AttributeValue(AttributeData.Type, AttributeData.ValueEx);
	        }
	        strName += strValue;
	        TextLabel.text = strName;
	    }
	
	    private void Start()
	    {
	    }
	
	    public void RemovePropertyChange()
	    {
	        OnDestroy();
	    }
	}
}