#region using

using System;
using EventSystem;
using UnityEngine;

#endregion

[Serializable]
public class ScriptEventItem
{
    public string EventArg;
    [SerializeField] public string EventName;
}

[Serializable]
public class ScriptEvent : MonoBehaviour
{
    public ScriptEventItem EventItem;

    public void OnTriggerEvent()
    {
        var e = new CommonEvent(EventItem.EventName, EventItem.EventArg);
        EventDispatcher.Instance.DispatchEvent(e);
    }
}