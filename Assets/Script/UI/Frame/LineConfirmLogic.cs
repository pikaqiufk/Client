using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

public class LineConfirmLogic : MonoBehaviour
{
    public BindDataRoot Binding;
    public List<UIEventTrigger> OtherList;
    public List<UIEventTrigger> SelfList1;
    public List<UIEventTrigger> SelfList2;

    public void OnClickCharInfo(int type, int index)
    {
        var e = new LineMemberClickEvent(type, index);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnClickCloseInfo()
    {
        var e = new LineMemberClickEvent(2);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnClickHide()
    {
        var e = new LineMemberClickEvent(4);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnClickShow()
    {
        var e = new LineMemberClickEvent(3);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        Binding.RemoveBinding();

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
#if !UNITY_EDITOR
try
{
#endif

        var controllerBase = UIManager.Instance.GetController(UIConfig.LineConfim);
        if (controllerBase == null)
        {
            return;
        }
        Binding.SetBindDataSource(controllerBase.GetDataModel(""));

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void OnFormatCooldown(UILabel lable)
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
        var taget = timer.TargetTime;
        if (taget > Game.Instance.ServerTime)
        {
            var dif = taget - Game.Instance.ServerTime;
            lable.text = string.Format("{0}", (int) dif.TotalSeconds);
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        for (var i = 0; i < 5; i++)
        {
            var s = SelfList1[i];
            var j = i;
            s.onClick.Add(new EventDelegate(() => { OnClickCharInfo(0, j); }));
            s = SelfList2[i];
            s.onClick.Add(new EventDelegate(() => { OnClickCharInfo(0, j); }));
            var o = OtherList[i];
            o.onClick.Add(new EventDelegate(() => { OnClickCharInfo(1, j); }));
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

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