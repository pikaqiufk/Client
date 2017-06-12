#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class PropertyDisplayB : MonoBehaviour
	{
	    public UILabel ChangeLabel;
	    private int mAttributeChange = -1;
	    private int mAttributeId = -1;
	    private int mAttributeValue = -1;
	    private int mAttributeValueEx;
	    public UILabel NameLabel;
	    public UILabel ValueLabel;
	
	    public int AttributeChange
	    {
	        get { return mAttributeChange; }
	        set
	        {
	            mAttributeChange = value;
	            if (mAttributeId != -1)
	            {
	                RefresChangeDisplay();
	            }
	        }
	    }
	
	    public int AttributeId
	    {
	        get { return mAttributeId; }
	        set
	        {
	            mAttributeId = value;
	//             if (mAttributeValue != -1)
	//             {
	//                 
	//             }
	            RefresDisplay();
	        }
	    }
	
	    public int AttributeValue
	    {
	        get { return mAttributeValue; }
	        set
	        {
	            mAttributeValue = value;
	            if (mAttributeId != -1)
	            {
	                RefresDisplay();
	            }
	        }
	    }
	
	    public int AttributeValueEx
	    {
	        get { return mAttributeValueEx; }
	        set
	        {
	            mAttributeValueEx = value;
	            if (mAttributeValue != -1 && mAttributeValue != -1 && mAttributeValueEx > 0)
	            {
	                RefresDisplay();
	            }
	        }
	    }
	
	    private void RefresChangeDisplay()
	    {
	        var strValue = GameUtils.AttributeValue(AttributeId, mAttributeChange);
	        ChangeLabel.text = strValue;
	    }
	
	    private void RefresDisplay()
	    {
	        var strName = GameUtils.AttributeName(AttributeId);
	        NameLabel.text = strName;
	        var strValue = GameUtils.AttributeValue(AttributeId, AttributeValue);
	        if (AttributeValueEx > 0)
	        {
	            var strValueEx = GameUtils.AttributeValue(AttributeId, AttributeValueEx);
	            strValue = strValue + "-" + strValueEx;
	        }
	        ValueLabel.text = strValue;
	    }
	}
}