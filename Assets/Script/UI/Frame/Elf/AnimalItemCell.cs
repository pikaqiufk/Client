#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class AnimalItemCell : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    public ListItemLogic ListItem;
	    private ElfItemDataModel mItemData;
	
	    public ElfItemDataModel ItemData
	    {
	        get { return mItemData; }
	        set
	        {
	            mItemData = value;
	            if (ListItem)
	            {
	                ListItem.Item = mItemData;
	            }
	            if (BindRoot)
	            {
	                BindRoot.SetBindDataSource(mItemData);
	            }
	        }
	    }
	
	    public void OnClickElfIco()
	    {
	        var data = (ElfItemDataModel) ListItem.Item;
	        var e = new Show_UI_Event(UIConfig.ElfInfoUI,
	            new ElfInfoArguments {DataModel = data, ShowButton = false});
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnElfCell1Click()
	    {
	        var date = (ElfItemDataModel) ListItem.Item;
	        EventDispatcher.Instance.DispatchEvent(new ElfCell1ClickEvent(date, gameObject));
	    }
	
	    public void OnElfItemClick()
	    {
	        if (ListItem != null)
	        {
	            var date = (ElfItemDataModel) ListItem.Item;
	            var e = new ElfCellClickEvent(date, ListItem.Index);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	}
}