#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using ClientDataModel;
using SignalChain;
using UnityEngine;

#endregion

public class UIListBindItem : MonoBehaviour
{
    public List<Transform> ItemList;
    private BubbleSignal<IChainRoot> mBubbleSignal;
    private object mSource;
    public string PrefabPath;

    public object Source
    {
        get { return mSource; }
        set
        {
            if (mSource != value)
            {
                mSource = value;
                CreateBind();
            }
        }
    }

    public void AddItem(int index)
    {
        var objres = ResourceManager.PrepareResourceSync<GameObject>(PrefabPath);
        var obj = Instantiate(objres) as GameObject;
        var grid = gameObject.GetComponent<UIGrid>();
        if (grid != null)
        {
            if (!grid.active)
            {
                grid.active = true;
            }
            grid.AddChild(obj.transform);
        }
        else
        {
            obj.transform.parent = transform;
        }
        obj.transform.localScale = Vector3.one;
        var mList = Source as IList;
        var binding = obj.GetComponent<BindDataRoot>();
        if (binding != null)
        {
            binding.SetBindDataSource(mList[index]);
        }

        var itemLogic = obj.GetComponent<ListItemLogic>();
        if (itemLogic != null)
        {
            itemLogic.Index = index;
            itemLogic.Item = mList[index];
        }
        SignalActiveChanged();
    }

    public void Awake()
    {
        mBubbleSignal = new BubbleSignal<IChainRoot>(gameObject.transform);
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
                AddItem(e.NewStartingIndex);
            }
                break;
            case NotifyCollectionChangedAction.Move:
                if (e.NewStartingIndex > e.OldStartingIndex)
                {
                }
                break;
            case NotifyCollectionChangedAction.Remove:
            {
                RemoveItem(e.OldStartingIndex);
                RemoveCollectionBind();
                CreateCollectionBind();
            }
                break;
            case NotifyCollectionChangedAction.Replace:
                break;
            case NotifyCollectionChangedAction.Reset:
            {
                for (var i = transform.childCount - 1; i >= 0; i--)
                {
                    RemoveItem(i);
                }
            }
                break;
            default:
                break;
        }
    }

    public void CreateBind()
    {
        if (Source == null)
        {
            return;
        }
        if (Source is INotifyCollectionChanged)
        {
            RemoveCollectionBind();
            CreateCollectionBind();
        }
        else if (Source is IReadonlyList)
        {
            CreateListBind();
        }
    }

    public void CreateCollectionBind()
    {
        var collectionChanged = Source as INotifyCollectionChanged;
        if (collectionChanged != null)
        {
            var mList = Source as IList;
            collectionChanged.CollectionChanged += CollectionChanged;

            var nSourCount = mList.Count;
            var nCellCount = transform.childCount;
            var maxCount = nSourCount > nCellCount ? nSourCount : nCellCount;

            if (nCellCount != 0)
            {
                for (var i = maxCount - 1; i >= 0; i--)
                {
                    if (i < nSourCount && i < nCellCount)
                    {
                        var obj = transform.GetChild(i).gameObject;

                        var binding = obj.GetComponent<BindDataRoot>();
                        if (binding != null)
                        {
                            binding.SetBindDataSource(mList[i]);
                        }

                        var itemLogic = obj.GetComponent<ListItemLogic>();
                        if (itemLogic != null)
                        {
                            itemLogic.Index = i;
                            itemLogic.Item = mList[i];
                        }
                    }

                    else if (i < nSourCount && i >= nCellCount)
                    {
                        AddItem(i);
                    }
                    else if (i >= nSourCount && i < nCellCount)
                    {
                        RemoveItem(i);
                    }
                }
            }
            else
            {
                for (var i = 0; i < maxCount; i++)
                {
                    AddItem(i);
                }
            }
        }
    }

    public void CreateListBind()
    {
        var list = Source as IReadonlyList;
        if (list == null)
        {
            return;
        }

        for (var i = 0; i < ItemList.Count; i++)
        {
            if (i < list.Count)
            {
                var item = ItemList[i].GetComponent<BindDataRoot>();
                item.SetBindDataSource(list[i]);
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

    public void RemoveItem(int index)
    {
        if (index >= transform.childCount)
        {
            return;
        }
        var obj = transform.GetChild(index);
        var grid = gameObject.GetComponent<UIGrid>();
        if (grid != null)
        {
            grid.RemoveChild(obj.transform);
        }

        SignalActiveChanged();
        DestroyImmediate(obj.gameObject);
    }

    public void SignalActiveChanged()
    {
        if (mBubbleSignal == null)
        {
            return;
        }
        mBubbleSignal.TargetedDispatch("ActiveChanged");
    }
}