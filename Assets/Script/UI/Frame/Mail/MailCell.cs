#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MailCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	    public UIToggle Toggle;
	
	    public void OnClickMailCell()
	    {
	        var e = new MailCellClickEvent(ItemLogic.Index, 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickMailCellCheck()
	    {
	        var isSelect = Toggle.value ? 1 : 0;
	        var e = new MailCellClickEvent(ItemLogic.Index, 2, isSelect);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnFormatTime(UILabel lable)
	    {
	        if (lable == null)
	        {
	            return;
	        }
	        var timer = lable.GetComponent<TimerLogic>();
	        if (timer == null)
	        {
	            return;
	        }
	        var str = "";
	        if (timer.TargetTime > Game.Instance.ServerTime)
	        {
	            var time = timer.TargetTime - Game.Instance.ServerTime;
	            if (time.Days > 0)
	            {
	                //{0}天{1}时{2}分
	                str = string.Format(GameUtils.GetDictionaryText(3200004), time.Days, time.Hours, time.Minutes);
	            }
	            else
	            {
	                //{0}时{1}分
	                str = string.Format(GameUtils.GetDictionaryText(3200005), time.Hours, time.Minutes);
	            }
	        }
	        else
	        {
	            //邮件已经过期，请尽快收取
	            str = GameUtils.GetDictionaryText(3200003);
	        }
	        lable.text = str;
	    }
	}
}