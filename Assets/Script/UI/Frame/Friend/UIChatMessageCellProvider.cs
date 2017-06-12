#region using

using System;
using System.Collections;
using System.Collections.Specialized;
using ClientDataModel;
using DataContract;
using EventSystem;
using GameUI;
using UnityEngine;

#endregion

public class UIChatMessageCellProvider : MonoBehaviour, IUIScrollViewOptimizedWidgetProvider
{
    private object mSource;
    public string PrefabPath;
    public UIScrollViewOptimized ScrollView;

    public int DataCount
    {
        get
        {
            var temp = Source as ICollection;
            if (temp != null)
            {
                return temp.Count;
            }
            return 0;
        }
    }

    public object Source
    {
        get { return mSource; }
        set
        {
            //if (mSource != value)
            {
                mSource = value;
                if (mSource != null)
                {
                    if (ScrollView)
                    {
                        if (ScrollView.widgetProvider == null)
                        {
                            ScrollView.widgetProvider = this;
                        }
                        ScrollView.Reset(DataCount - 1);
                        CreateBind();
                        ScrollView.NofifyUpdate();
                        ScrollView.MoveToTop(DataCount - 1);
                    }
                    else
                    {
                        CreateBind();
                    }
                }
            }
        }
    }

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        ScrollView.widgetProvider = this;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender != Source)
        {
            return;
        }
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                if (ScrollView == null || ScrollView.panel == null)
                {
                    return;
                }

                var bounds = ScrollView.bounds;

                if (e.NewStartingIndex == 0)
                {
                    ScrollView.InsertBefore(e.NewStartingIndex);
                    ScrollView.NofifyUpdate(false);
                    ScrollView.MoveToTop(DataCount - 1);
                }
                else
                {
                    ScrollView.NofifyUpdate(true);
                    var offset = -ScrollView.panel.GetViewSize().y/2 + ScrollView.panel.clipOffset.y - bounds.min.y;

                    if (Math.Abs(offset) < 10.0f)
                    {
                        ScrollView.MoveToTop(DataCount - 1);
                    }
                }
            }
                break;
            case NotifyCollectionChangedAction.Move:
                if (e.NewStartingIndex > e.OldStartingIndex)
                {
                }
                break;
            case NotifyCollectionChangedAction.Remove:
            {
                ScrollView.NofifyUpdate();
            }
                break;
            case NotifyCollectionChangedAction.Replace:
            {
            }
                break;
            case NotifyCollectionChangedAction.Reset:
            {
                ScrollView.Reset();
            }
                break;
            default:
                break;
        }
    }

    public void CreateBind()
    {
        if (Source is INotifyCollectionChanged)
        {
            RemoveCollectionBind();
            CreateCollectionBind();
        }
    }

    public void CreateCollectionBind()
    {
        var collectionChanged = Source as INotifyCollectionChanged;
        if (collectionChanged != null)
        {
            var mList = Source as IList;
            collectionChanged.CollectionChanged += CollectionChanged;
        }
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(FrindChatNotifyProvider.EVENT_TYPE, OnFrindChatNotifyProvider);
        RemoveCollectionBind();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnFrindChatNotifyProvider(IEvent ievent)
    {
        var e = ievent as FrindChatNotifyProvider;
        var w = e.Widget;
        if (ScrollView != null)
        {
            var isTop = false;
            var bounds = ScrollView.bounds;
            ScrollView.NofifyUpdate(true);
            var offset = -ScrollView.panel.GetViewSize().y/2 + ScrollView.panel.clipOffset.y - bounds.min.y;
            if (Math.Abs(offset) < 10.0f)
            {
                isTop = true;
            }

            ScrollView.UpdateCell(w);


            if (isTop)
            {
                ScrollView.MoveToTop(DataCount - 1);
            }
        }
    }

    public void RemoveCollectionBind()
    {
        if (Source == null)
        {
            return;
        }
        var collectionChanged = Source as INotifyCollectionChanged;
        if (collectionChanged != null)
        {
            ((INotifyCollectionChanged) Source).CollectionChanged -= CollectionChanged;
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        ScrollView.NofifyUpdate();
        ScrollView.MoveToTop(DataCount - 1);
        EventDispatcher.Instance.AddEventListener(FrindChatNotifyProvider.EVENT_TYPE, OnFrindChatNotifyProvider);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public UIWidget GetWidget(int index)
    {
        var objres = ResourceManager.PrepareResourceSync<GameObject>(PrefabPath);
        var obj = Instantiate(objres) as GameObject;
        if (obj == null)
        {
            return null;
        }

        var logic = obj.GetComponent<FriendChatCell>();
        var temp = Source as IList;
        if (temp == null)
        {
            return null;
        }
        var data = temp[index] as FriendMessageDataModel;
        if (data == null)
        {
            return null;
        }
        logic.ChatMessageData = data.MessageData as ChatMessageData;

        return logic.GetComponent<UIWidget>();
        return null;
    }

    public bool HasNextWidget(int index)
    {
        if (index < 0 || index >= DataCount)
        {
            return false;
        }
        return true;
    }

    public void BeforeDestroy(UIWidget widget)
    {
        var b = widget.GetComponent<BindDataRoot>();
        b.RemoveBinding();
    }

    public void RequestUpperWidget()
    {
        var e = new FriendClickType(2);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public int Count()
    {
        return DataCount;
    }
}