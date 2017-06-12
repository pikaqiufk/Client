#region using

using System.Collections.Generic;
using DataTable;
using EventSystem;
using Shared;

#endregion

public class ConditionTrigger
{
    private static ConditionTrigger _instance;
    public ConditionModifyEvent Exdata = new ConditionModifyEvent();
    public ConditionModifyEvent FlagFalse = new ConditionModifyEvent();
    public ConditionModifyEvent FlagTrue = new ConditionModifyEvent();
    public ConditionModifyEvent Item = new ConditionModifyEvent();

    public static ConditionTrigger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ConditionTrigger();
            }
            return _instance;
        }
    }

    public void CleanUp()
    {
        FlagTrue.Reset();
        FlagFalse.Reset();
        Exdata.Reset();
        Item.Reset();
        EventDispatcher.Instance.RemoveEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpData);
        EventDispatcher.Instance.RemoveEventListener(Resource_Change_Event.EVENT_TYPE, OnResourceChange);
        EventDispatcher.Instance.RemoveEventListener(FlagUpdateEvent.EVENT_TYPE, OnFlagUpdate);
        EventDispatcher.Instance.RemoveEventListener(BagItemCountChangeEvent.EVENT_TYPE, OnBagItemCountChange);
    }

    public void Init()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpData);
        EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnResourceChange);
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnFlagUpdate);
        EventDispatcher.Instance.AddEventListener(BagItemCountChangeEvent.EVENT_TYPE, OnBagItemCountChange);
    }

    private void OnBagItemCountChange(IEvent ievent)
    {
        var e = ievent as BagItemCountChangeEvent;
        {
            // foreach(var i in e.ItemChanges)
            var __enumerator2 = (e.ItemChanges).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var i = __enumerator2.Current;
                {
                    Item.SendEvent(i.Key);
                }
            }
        }
    }

    private void OnExDataUpData(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        Exdata.SendEvent(e.Key);
    }

    private void OnFlagUpdate(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        if (e.Value)
        {
            FlagTrue.SendEvent(e.Index);
        }
        else
        {
            FlagFalse.SendEvent(e.Index);
        }
    }

    private void OnResourceChange(IEvent ievent)
    {
        var e = ievent as Resource_Change_Event;
        var type = (int) e.Type;
        Item.SendEvent(type);
    }

    public void PushCondition(int id)
    {
        var tbcon = Table.GetConditionTable(id);
        if (tbcon == null)
        {
            return;
        }
        PushCondition(tbcon);
    }

    public void PushCondition(ConditionTableRecord tbcon)
    {
        if (tbcon.Role != -1 && BitFlag.GetLow(tbcon.Role, PlayerDataManager.Instance.GetRoleId()) == false)
        {
            return;
        }

        for (var i = 0; i != tbcon.TrueFlag.Length; ++i)
        {
            var id = tbcon.TrueFlag[i];
            if (id == -1)
            {
                continue;
            }
            FlagTrue.Push(id, tbcon);
        }
        for (var i = 0; i != tbcon.FalseFlag.Length; ++i)
        {
            var id = tbcon.FalseFlag[i];
            if (id == -1)
            {
                continue;
            }
            FlagFalse.Push(id, tbcon);
        }
        for (var i = 0; i != tbcon.ExdataId.Length; ++i)
        {
            var id = tbcon.ExdataId[i];
            if (id == -1)
            {
                continue;
            }
            Exdata.Push(id, tbcon);
        }

        for (var i = 0; i != tbcon.ItemId.Length; ++i)
        {
            var id = tbcon.ItemId[i];
            if (id == -1)
            {
                continue;
            }
            Item.Push(id, tbcon);
        }
    }

    public void RemoveCondition(int id)
    {
        var tbcon = Table.GetConditionTable(id);
        if (tbcon == null)
        {
            return;
        }
        RemoveCondition(tbcon);
    }

    public void RemoveCondition(ConditionTableRecord tbcon)
    {
        if (tbcon.Role != -1 && BitFlag.GetLow(tbcon.Role, PlayerDataManager.Instance.GetRoleId()) == false)
        {
            return;
        }

        for (var i = 0; i != tbcon.TrueFlag.Length; ++i)
        {
            var id = tbcon.TrueFlag[i];
            if (id == -1)
            {
                continue;
            }
            FlagTrue.Remove(id, tbcon);
        }
        for (var i = 0; i != tbcon.FalseFlag.Length; ++i)
        {
            var id = tbcon.FalseFlag[i];
            if (id == -1)
            {
                continue;
            }
            FlagFalse.Remove(id, tbcon);
        }
        for (var i = 0; i != tbcon.ExdataId.Length; ++i)
        {
            var id = tbcon.ExdataId[i];
            if (id == -1)
            {
                continue;
            }
            Exdata.Remove(id, tbcon);
        }

        for (var i = 0; i != tbcon.ItemId.Length; ++i)
        {
            var id = tbcon.ItemId[i];
            if (id == -1)
            {
                continue;
            }
            Item.Remove(id, tbcon);
        }
    }

    public class ConditionModifyEvent
    {
        private readonly Dictionary<int, int> conditionCount = new Dictionary<int, int>();
        //public  ulong NextGuidId;
        //Dictionary<ulong,KeyValuePair<ConditionTableRecord,Action>> acts=new Dictionary<ulong, KeyValuePair<ConditionTableRecord, Action>>();
        //private Dictionary<int, List<ulong>> mCallBacks = new Dictionary<int, List<ulong>>();

        private readonly Dictionary<int, List<ConditionTableRecord>> mDatas =
            new Dictionary<int, List<ConditionTableRecord>>();

        public List<ConditionTableRecord> GetModify(int id)
        {
            List<ConditionTableRecord> Data = null;
            mDatas.TryGetValue(id, out Data);
            return Data;
        }

        public void Push(int id, ConditionTableRecord c)
        {
            conditionCount.modifyValue(id, 1);
            if (conditionCount[id] > 1)
            {
                return;
            }
            List<ConditionTableRecord> Data;
            if (!mDatas.TryGetValue(id, out Data))
            {
                Data = new List<ConditionTableRecord>();
                mDatas.Add(id, Data);
            }
            Data.Add(c);
        }

        public void Remove(int id, ConditionTableRecord c)
        {
            conditionCount.modifyValue(id, -1);
            if (conditionCount[id] > 0)
            {
                return;
            }
            conditionCount.Remove(id);
            List<ConditionTableRecord> Data;
            if (mDatas.TryGetValue(id, out Data))
            {
                Data.Remove(c);
            }
        }

        public void Reset()
        {
            mDatas.Clear();
        }

        public void SendEvent(int id)
        {
            List<ConditionTableRecord> conditions;
            if (mDatas.TryGetValue(id, out conditions))
            {
                {
                    var __list1 = conditions;
                    var __listCount1 = __list1.Count;
                    for (var __i1 = 0; __i1 < __listCount1; ++__i1)
                    {
                        var record = __list1[__i1];
                        {
                            var e = new ConditionChangeEvent(record.Id);
                            EventDispatcher.Instance.DispatchEvent(e);
                        }
                    }
                }
            }
        }
    }
}