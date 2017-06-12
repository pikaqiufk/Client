#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class PropertyDisplayC : MonoBehaviour
	{
	    public UILabel ChangeLabel;
	    public UISprite DownSprite;
	    private int propertyId = -1;
	    private int propertyValue = -1;
	    private int PropertyValueEx;
	    private int valueChagne;
        private int valueChangeEx;
	    public UILabel NameLabel;
	    public UISprite UpSprite;
	    public UILabel ValueLabel;
	
	    public int AttributeId
	    {
	        get { return propertyId; }
	        set
	        {
	            propertyId = value;
	            if (propertyValue != -1)
	            {
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
	        get { return PropertyValueEx; }
	        set
	        {
	            PropertyValueEx = value;
	            if (propertyValue != -1 && propertyValue != -1 && PropertyValueEx > 0)
	            {
	                RefresDisplay();
	            }
	        }
	    }
	
	    public int ValueChagne
	    {
	        get { return valueChagne; }
	        set
	        {
	            valueChagne = value;
	            if (valueChagne > 0)
	            {
	                RefresDisplayChange();
	                UpSprite.gameObject.SetActive(true);
	                DownSprite.gameObject.SetActive(false);
	                ChangeLabel.gameObject.SetActive(true);
	            }
	            else if (valueChagne < 0)
	            {
	                RefresDisplayChange();
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

        public int ValueChangeEx
        {
            get { return valueChangeEx; }
            set
            {
                valueChangeEx = value;
                RefresDisplayChange();
            }
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
	
	    private void RefresDisplayChange()
	    {
	        var strValue = GameUtils.AttributeValue(AttributeId, ValueChagne);

            if (ValueChangeEx > 0)
	        {
                var strValueEx = GameUtils.AttributeValue(AttributeId, ValueChangeEx);
	            strValue = strValue + "-" + strValueEx;
	        }
	
	        ChangeLabel.text = strValue;
	    }
	}
}