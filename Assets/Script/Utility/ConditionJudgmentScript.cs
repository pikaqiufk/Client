using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

internal class ConditionJudgmentScript : MonoBehaviour
{
    private bool _isAddListener;
    public int IsShowConditionId = -1;
    private UIEventTrigger myeventTrigger;
    public int onClickConditionId = -1;

    public int OnClickConditionId
    {
        get { return onClickConditionId; }
        set
        {
            if (onClickConditionId == value)
            {
                return;
            }
            onClickConditionId = value;
            Refresh(null);
        }
    }

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (GameSetting.Instance.IgnoreButtonCondition == true)
        {
            return;
        }

        if (PlayerDataManager.Instance.CheckCondition(IsShowConditionId) == 0)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (!_isAddListener)
        {
            _isAddListener = true;
            EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, Refresh);
            EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, Refresh);
            EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, Refresh);
            EventDispatcher.Instance.AddEventListener(BagItemCountChangeEvent.EVENT_TYPE, Refresh);
        }

        if (PlayerDataManager.Instance.CheckCondition(IsShowConditionId) == 0)
        {
            if (PlayerDataManager.Instance.CheckCondition(OnClickConditionId) != 0)
            {
                if (gameObject.GetComponent<UIButton>() != null)
                {
                    gameObject.GetComponent<UIButton>().enabled = false;
                }
                if (gameObject.GetComponent<UIToggle>() != null)
                {
                    gameObject.GetComponent<UIToggle>().enabled = false;
                }

                if (myeventTrigger == null)
                {
                    myeventTrigger = gameObject.AddComponent<UIEventTrigger>();
                    var ad = new EventDelegate(this, "OnClickJudg");
                    myeventTrigger.onClick.Add(ad);
                }
            }
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnClickJudg()
    {
        var hint = PlayerDataManager.Instance.CheckCondition(OnClickConditionId);
        if (hint == 0)
        {
            var myComponent = gameObject.GetComponent<UIEventTrigger>();
            if (myComponent != null)
            {
                Destroy(myComponent);
            }

            if (gameObject.GetComponent<UIButton>() != null)
            {
                gameObject.GetComponent<UIButton>().enabled = true;
            }

            if (gameObject.GetComponent<UIToggle>() != null)
            {
                gameObject.GetComponent<UIToggle>().enabled = true;
            }
        }
        else
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(hint));
        }
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (_isAddListener)
        {
            _isAddListener = false;
            EventDispatcher.Instance.RemoveEventListener(FlagUpdateEvent.EVENT_TYPE, Refresh);
            EventDispatcher.Instance.RemoveEventListener(BagItemCountChangeEvent.EVENT_TYPE, Refresh);
            EventDispatcher.Instance.RemoveEventListener(ExDataUpDataEvent.EVENT_TYPE, Refresh);
            EventDispatcher.Instance.RemoveEventListener(Resource_Change_Event.EVENT_TYPE, Refresh);
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void Refresh(IEvent ievent)
    {
        if (PlayerDataManager.Instance.CheckCondition(IsShowConditionId) == 0)
        {
            gameObject.SetActive(true);

            if (PlayerDataManager.Instance.CheckCondition(OnClickConditionId) != 0)
            {
                if (gameObject.GetComponent<UIButton>() != null)
                {
                    gameObject.GetComponent<UIButton>().enabled = false;
                }
                if (gameObject.GetComponent<UIToggle>() != null)
                {
                    gameObject.GetComponent<UIToggle>().enabled = false;
                }

                if (gameObject.GetComponent<UIEventTrigger>() == null)
                {
                    var myeventTrigger = gameObject.AddComponent<UIEventTrigger>();
                    var ad = new EventDelegate(this, "OnClickJudg");
                    myeventTrigger.onClick.Add(ad);
                }
            }
            else
            {
                if (gameObject.GetComponent<UIButton>() != null)
                {
                    gameObject.GetComponent<UIButton>().enabled = true;
                }

                if (null != myeventTrigger)
                {
                    Destroy(myeventTrigger);
                }
            }
        }
    }
}