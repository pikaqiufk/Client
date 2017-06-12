#region using

using System;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipMentAttrCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	    private AttributeBaseDataModel mBaseData;
	    public UILabel NameLab;
	
	    public AttributeBaseDataModel BaseData
	    {
	        get { return mBaseData; }
	        set
	        {
	            mBaseData = value;
	            RefreshAttribute();
	        }
	    }
	
	    private void RefreshAttribute()
	    {
	        var type = BaseData.Type;
	        var value = BaseData.Value;
	        var name = GameUtils.AttributeName(type);
	
	        if (value > 0)
	        {
	            var str = GameUtils.GetDictionaryText(230025);
	            var att = GameUtils.AttributeValue(type, value);
	            str = String.Format(str, name, att);
	            str = String.Format("[ADFF00]{0}[-]", str);
	            NameLab.text = str;
	        }
	        else
	        {
	            var str = GameUtils.GetDictionaryText(230028);
	            var att = GameUtils.AttributeValue(type, Mathf.Abs(value));
	            str = String.Format(str, name, att);
	            str = String.Format("[FC3737]{0}[-]", str);
	            NameLab.text = str;
	        }
	    }
	
	    private void Start()
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