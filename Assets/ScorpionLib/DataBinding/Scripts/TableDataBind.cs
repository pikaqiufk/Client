#region using

using System.Collections;
using System.Collections.Specialized;
using UnityEngine;

#endregion

public class TableDataBind : MonoBehaviour
{
    private UITable mGrid;
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
        var childList = mGrid.GetChildListEx();
        if (childList.Count == transform.childCount)
        {
            AddNewItem(i);
        }
        else
        {
            for (var j = 0; j < transform.childCount; j++)
            {
                var t = transform.GetChild(j);
                var b = t.GetComponent<BindDataRoot>();

                if (b.IsBind == false)
                {
                    t.gameObject.SetActive(true);
                    if (mGrid.enabled == false)
                    {
                        mGrid.enabled = true;
                    }
                    mGrid.AddChild(t);
                    break;
                }
            }
            var mList = Source as IList;
            childList = mGrid.GetChildListEx();
            for (var j = i; j < childList.Count; j++)
            {
                var t = childList[j];
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
    }

    private void AddNewItem(int i)
    {
        //var res = ResourceManager.PrepareResourceSync<GameObject>(PrefabPath);
        ResourceManager.PrepareResource<GameObject>(PrefabPath, res =>
        {
            if (res == null)
            {
                Debug.LogError(string.Format("ResourceManager.PrepareResource......Error {0}", PrefabPath));
                return;
            }
            if (mGrid.enabled == false)
            {
                mGrid.enabled = true;
            }
            var obj = Instantiate(res) as GameObject;
#if UNITY_EDITOR
            obj.name += i.ToString();
#endif
            mGrid.AddChild(obj.transform);
            obj.transform.localScale = Vector3.one;

            var mList = Source as IList;

            var childList = mGrid.GetChildListEx();
            for (var j = i; j < childList.Count && j < mList.Count; j++)
            {
                var t = childList[j];
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
            );
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
        if (mGrid == null)
        {
            mGrid = GetComponent<UITable>();
            mGrid.hideInactive = true;
        }
        if (mGrid == null)
        {
            return;
        }

        Reset();

        var mList = Source as IList;
        collectionChanged.CollectionChanged += CollectionChanged;

        var nSourCount = mList.Count;
        var nCellCount = transform.childCount;
        var maxCount = nSourCount > nCellCount ? nSourCount : nCellCount;
        //List<Transform> cellList = new List<Transform>();
        for (var i = 0; i < maxCount; i++)
        {
            if (i < nSourCount && i < nCellCount)
            {
                GameObject obj = null;
                for (var j = 0; j < nCellCount; j++)
                {
                    var o = transform.GetChild(j).gameObject;
                    var b = o.GetComponent<BindDataRoot>();
                    if (b != null)
                    {
                        if (b.IsBind == false)
                        {
                            obj = o;
                            break;
                        }
                    }
                }
                if (mGrid.enabled == false)
                {
                    mGrid.enabled = true;
                }

                mGrid.AddChild(obj.transform);
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
    }

    public int GetSoureListCount()
    {
        var c = 0;
        var mList = Source as IList;
        {
            // foreach(var i in mList)
            var __enumerator2 = (mList).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var i = __enumerator2.Current;
                {
                    c++;
                }
            }
        }
        return c;
    }

    public void RemoveBind()
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

    private void RemoveItem(int i)
    {
        var childList = mGrid.GetChildListEx();
        var remove = childList[i];
        mGrid.RemoveChild(remove);

        remove.gameObject.SetActive(false);

        var b = remove.GetComponent<BindDataRoot>();
        if (b != null)
        {
            b.RemoveBinding();
        }


        var mList = Source as IList;
        childList = mGrid.GetChildListEx();
        for (var j = i; j < childList.Count; j++)
        {
            var t = childList[j];
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

    public void Reset()
    {
        if (mGrid == null)
        {
            return;
        }
        var childList = mGrid.GetChildListEx();
        {
            var __list1 = childList;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var t = __list1[__i1];
                {
                    var b = t.GetComponent<BindDataRoot>();
                    if (b != null)
                    {
                        b.RemoveBinding();
                    }
                    mGrid.RemoveChild(t);
                }
            }
        }
    }
}