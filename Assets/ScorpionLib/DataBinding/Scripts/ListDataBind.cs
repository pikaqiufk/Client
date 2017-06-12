#region using

using System.Collections;
using System.Collections.Specialized;
using SignalChain;
using UnityEngine;

#endregion

public class ListDataBind : MonoBehaviour
{
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
                if (mSource != null)
                {
                    RemoveBind();
                    CreateBind();
                }
            }
        }
    }

    private void AddItem(int i)
    {
        var isFind = false;
        for (var j = 0; j < transform.childCount; j++)
        {
            var t = transform.GetChild(j);
            var b = t.GetComponent<BindDataRoot>();
            if (b.IsBind == false)
            {
                t.gameObject.SetActive(true);
                isFind = true;
                break;
            }
        }
        if (isFind == false)
        {
            AddNewItem(i);
            return;
        }

        var mList = Source as IList;
        for (var j = i; j < GetSoureListCount(); j++)
        {
            var t = transform.GetChild(j);
            var binding = t.GetComponent<BindDataRoot>();
            if (binding != null)
            {
                binding.SetBindDataSource(mList[j]);
            }

            var itemLogic = t.GetComponent<ListItemLogic>();
            if (itemLogic != null)
            {
                itemLogic.Index = j;
                itemLogic.Item = mList[j];
            }
        }
        SignalActiveChanged();
    }

    private void AddNewItem(int i)
    {
        var res = ResourceManager.PrepareResourceSync<GameObject>(PrefabPath);

        if (res == null)
        {
            Debug.LogError(string.Format("ResourceManager.PrepareResource......Error {0}", PrefabPath));
            return;
        }
        var obj = Instantiate(res) as GameObject;
        obj.transform.parent = transform;
        obj.transform.localScale = Vector3.one;

        var mList = Source as IList;

        for (var j = i; j < transform.childCount; j++)
        {
            var t = transform.GetChild(j);
            var binding = t.GetComponent<BindDataRoot>();
            if (binding != null)
            {
                binding.SetBindDataSource(mList[j]);
            }

            var itemLogic = t.GetComponent<ListItemLogic>();
            if (itemLogic != null)
            {
                itemLogic.Index = j;
                itemLogic.Item = mList[j];
            }
        }
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
            }
                break;
            case NotifyCollectionChangedAction.Replace:
                break;
            case NotifyCollectionChangedAction.Reset:
            {
                Reset();
            }
                break;
            default:
                break;
        }
    }

    private void CreateBind()
    {
        var collectionChanged = Source as INotifyCollectionChanged;
        if (collectionChanged == null)
        {
            return;
        }
        Reset();
        var mList = Source as IList;
        collectionChanged.CollectionChanged += CollectionChanged;

        var nSourCount = mList.Count;
        var nCellCount = transform.childCount;
        var maxCount = nSourCount > nCellCount ? nSourCount : nCellCount;
        for (var i = 0; i < maxCount; i++)
        {
            if (i < nSourCount && i < nCellCount)
            {
                var obj = transform.GetChild(i).gameObject;
                obj.SetActive(true);
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
                AddNewItem(i);
            }
            else if (i >= nSourCount && i < nCellCount)
            {
            }
        }
        SignalActiveChanged();
    }

    public int GetSoureListCount()
    {
        var c = 0;
        var mList = Source as IList;
        {
            // foreach(var i in mList)
            var __enumerator1 = (mList).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var i = __enumerator1.Current;
                {
                    c++;
                }
            }
        }
        return c;
    }

    public void RemoveBind()
    {
        var collectionChanged = Source as INotifyCollectionChanged;
        if (collectionChanged != null)
        {
            ((INotifyCollectionChanged) Source).CollectionChanged -= CollectionChanged;
        }
    }

    private void RemoveItem(int i)
    {
        var remove = transform.GetChild(i);

        remove.gameObject.SetActive(false);

        var b = remove.GetComponent<BindDataRoot>();
        if (b != null)
        {
            b.RemoveBinding();
        }


        var mList = Source as IList;
        for (var j = i; j < transform.childCount; j++)
        {
            var t = transform.GetChild(j);
            var binding = t.GetComponent<BindDataRoot>();
            if (binding != null)
            {
                binding.SetBindDataSource(mList[j]);
            }

            var itemLogic = t.GetComponent<ListItemLogic>();
            if (itemLogic != null)
            {
                itemLogic.Index = j;
                itemLogic.Item = mList[j];
            }
        }
        SignalActiveChanged();
    }

    private void Reset()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var t = transform.GetChild(i);
            t.gameObject.SetActive(false);
            var b = t.GetComponent<BindDataRoot>();
            if (b != null)
            {
                b.RemoveBinding();
            }
        }
        SignalActiveChanged();
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