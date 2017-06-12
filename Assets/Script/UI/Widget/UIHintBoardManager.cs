#region using

using System;
using System.Collections.Generic;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

public class UIHintBoardManager : MonoBehaviour
{
    public GameObject HighRoot;
    public GameObject LowRoot;
    private readonly Dictionary<int, HintInfoQueue> mHintInfoQueues = new Dictionary<int, HintInfoQueue>();
    private List<DamageBoardLogic> UiBoardLogics = new List<DamageBoardLogic>();

    public enum eRootLayer
    {
        High = 0,
        Low = 1
    }

    public static UIHintBoardManager Instace { get; private set; }

    private void OnShowErrorTip(IEvent ievent)
    {
        var e = ievent as UIEvent_ErrorTip;
        var errorCode = e.ErrorCode;
        var dicId = 200000000 + (int) errorCode;
        var table = Table.GetDictionary(dicId);
        if (null != table)
        {
            ShowInfo(14, GameUtils.GetDictionaryText(dicId));
        }
        else
        {
            ShowInfo(14, GameUtils.GetDictionaryText(200000001));
        }
    }

    private void OnShowUIHint(IEvent ievent)
    {
        var e = ievent as ShowUIHintBoard;
        PushInfoQuene(e.TableId, e.Info, e.WaitSec);
        if (e.DicId == 210100)
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ExchangeUI));
    }

    private void PushInfoQuene(int tableId, string info, int waitSec = -1)
    {
        HintInfoQueue infoQueue;
        var tbComText = Table.GetCombatText(tableId);
        if (tbComText == null)
        {
            return;
        }
        if (tbComText.QueueLimit == -1 && tbComText.IntervalTime == 0)
        {
            ShowInfo(tableId, info, -1, (eRootLayer) tbComText.TextLayer);
            return;
        }
        if (!mHintInfoQueues.TryGetValue(tbComText.Group, out infoQueue))
        {
            infoQueue = new HintInfoQueue();
            infoQueue.CdTime = tbComText.IntervalTime;
            infoQueue.CdTimer = Game.Instance.ServerTime.AddMilliseconds(-infoQueue.CdTime);
            mHintInfoQueues[tbComText.Group] = infoQueue;
        }
        if (tbComText.QueueLimit == -1 || infoQueue.Info.Count < tbComText.QueueLimit)
        {
            infoQueue.Info.Add(new HintParam
            {
                TableId = tableId,
                Text = info,
                WaitSec = waitSec,
                RootLayer = (eRootLayer) tbComText.TextLayer
            });
        }
    }

    private void ShowInfo(int tableId, string info, int waitSec = -1, eRootLayer rootLayer = eRootLayer.High)
    {
        ComplexObjectPool.NewObject("UI/DamageBoard.prefab", o =>
        {
            var oTransform = o.transform;
            if (rootLayer == eRootLayer.High)
            {
                oTransform.SetParentEX(HighRoot.transform);
            }
            else if (rootLayer == eRootLayer.Low)
            {
                oTransform.SetParentEX(LowRoot.transform);
            }
            o.SetActive(true);
            oTransform.localScale = Vector3.one;
            oTransform.localPosition = Vector3.zero;
            var logic = o.GetComponent<DamageBoardLogic>();
            logic.StartAction(Vector3.zero, tableId, info, DamageBoardLogic.BoardShowType.UI);
            if (waitSec >= 0)
            {
                logic.StayTime = waitSec*1000;
            }
        });
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (Instace)
        {
            EventDispatcher.Instance.RemoveEventListener(ShowUIHintBoard.EVENT_TYPE, Instace.OnShowUIHint);
            EventDispatcher.Instance.RemoveEventListener(UIEvent_ErrorTip.EVENT_TYPE, Instace.OnShowErrorTip);
            Instace = null;
        }

        Instace = this;
        EventDispatcher.Instance.AddEventListener(ShowUIHintBoard.EVENT_TYPE, Instace.OnShowUIHint);
        EventDispatcher.Instance.AddEventListener(UIEvent_ErrorTip.EVENT_TYPE, Instace.OnShowErrorTip);

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
        {
            // foreach(var infoQueue in mHintInfoQueues)
            var __enumerator1 = (mHintInfoQueues).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var infoQueue = __enumerator1.Current;
                {
                    if (infoQueue.Value.Info.Count == 0)
                    {
                        continue;
                    }
                    if (infoQueue.Value.CdTimer.AddMilliseconds(infoQueue.Value.CdTime) <= Game.Instance.ServerTime)
                    {
                        var info = infoQueue.Value.Info[0];
                        ShowInfo(info.TableId, info.Text, info.WaitSec, info.RootLayer);
                        infoQueue.Value.CdTimer = Game.Instance.ServerTime;
                        infoQueue.Value.Info.RemoveAt(0);
                    }
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

    public class HintParam
    {
        public eRootLayer RootLayer;
        public int TableId;
        public string Text;
        public int WaitSec;
    }

    public class HintInfoQueue
    {
        public DateTime CdTimer;
        public List<HintParam> Info = new List<HintParam>();
        public int CdTime { get; set; }
    }
}