#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class PropertyDisplayA : MonoBehaviour
	{
	    public UILabel Lable;
	    private int propertyId = -1;
	    private int propertyValue = -1;
	    private int propertyValueEx;
	
	    public int AttributeId
	    {
	        get { return propertyId; }
	        set
	        {
	            propertyId = value;
	            if (propertyValue != -1)
	            {
	                RefresDisplay();
	            }
	        }
	    }
	
	    public int AttributeValue
	    {
	        get { return propertyValue; }
	        set
	        {
	            propertyValue = value;
	            if (propertyId != -1)
	            {
	                RefresDisplay();
	            }
	        }
	    }
	
	    public int AttributeValueEx
	    {
	        get { return propertyValueEx; }
	        set
	        {
	            propertyValueEx = value;
	            if (propertyValue != -1 && propertyValue != -1)
	            {
	                RefresDisplay();
	            }
	        }
	    }
	
	    private void RefresDisplay()
	    {
	        var strName = GameUtils.AttributeName(AttributeId);
	        var strValue = GameUtils.AttributeValue(AttributeId, AttributeValue);
	        var str = strName + ":" + strValue;
	        if (AttributeValueEx != -1)
	        {
	            var strValueEx = GameUtils.AttributeValue(AttributeId, AttributeValueEx);
	            str = str + "-" + strValueEx;
	        }
	        Lable.text = str;
	    }
	}
}