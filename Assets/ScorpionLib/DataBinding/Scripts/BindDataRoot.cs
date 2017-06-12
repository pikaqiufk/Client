#region using

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public interface IPropertyChange
{
    void RemovePropertyChange();
}


public class BindDataRoot : MonoBehaviour
{
    public List<BindingClassName> BindingNamelList;
    private object mSource;
    public bool IsBind { get; set; }

    public object Source
    {
        get { return mSource; }
        set
        {
            mSource = value;
            SetBindDataSource(value);
        }
    }

    private void OnDestroy()
    {
        RemoveBinding();
        OptList.ClearAll();
    }

    public void RemoveBinding()
    {
        IsBind = false;

        OptList<UIClassBinding>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<UIClassBinding>.List);
        {
            var __array1 = OptList<UIClassBinding>.List;
            var __arrayLength1 = __array1.Count;
            for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var o = __array1[__i1];
                {
                    o.Unbinding();
                    o.DataSource = null;
                }
            }
        }
        OptList<UIListBindItem>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<UIListBindItem>.List);
        {
            var __array2 = OptList<UIListBindItem>.List;
            var __arrayLength2 = __array2.Count;
            for (var __i2 = 0; __i2 < __arrayLength2; ++__i2)
            {
                var o = __array2[__i2];
                {
                    o.RemoveCollectionBind();
                    o.Source = null;
                }
            }
        }
        OptList<UIGridSimple>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<UIGridSimple>.List);
        {
            var __array3 = OptList<UIGridSimple>.List;
            var __arrayLength3 = __array3.Count;
            for (var __i3 = 0; __i3 < __arrayLength3; ++__i3)
            {
                var o = __array3[__i3];
                {
                    o.RemoveCollectionBind();
                    o.Source = null;
                }
            }
        }
        OptList<ListDataBind>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<ListDataBind>.List);
        {
            var __array4 = OptList<ListDataBind>.List;
            var __arrayLength4 = __array4.Count;
            for (var __i4 = 0; __i4 < __arrayLength4; ++__i4)
            {
                var o = __array4[__i4];
                {
                    o.RemoveBind();
                    o.Source = null;
                }
            }
        }
        OptList<TableDataBind>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<TableDataBind>.List);
        {
            var __array5 = OptList<TableDataBind>.List;
            var __arrayLength5 = __array5.Count;
            for (var __i5 = 0; __i5 < __arrayLength5; ++__i5)
            {
                var o = __array5[__i5];
                {
                    o.RemoveBind();
                    o.Source = null;
                }
            }
        }

        OptList<GridDataBind>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<GridDataBind>.List);
        {
            var __array6 = OptList<GridDataBind>.List;
            var __arrayLength6 = __array6.Count;
            for (var __i6 = 0; __i6 < __arrayLength6; ++__i6)
            {
                var o = __array6[__i6];
                {
                    o.RemoveBind();
                    o.Source = null;
                }
            }
        }

        OptList<InverseBinding>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<InverseBinding>.List);
        {
            var __array7 = OptList<InverseBinding>.List;
            var __arrayLength7 = __array7.Count;
            for (var __i7 = 0; __i7 < __arrayLength7; ++__i7)
            {
                var o = __array7[__i7];
                {
                    var input = o.gameObject.GetComponent<UIInput>();
                    if (input != null)
                    {
                        input.onChange.Remove(o.EventDelegate);
                        o.EventDelegate = null;
                    }
                    var toggle = o.gameObject.GetComponent<UIToggle>();
                    if (toggle != null)
                    {
                        toggle.onChange.Remove(o.EventDelegate);
                        o.EventDelegate = null;
                    }
                    var slider = o.gameObject.GetComponent<UISlider>();
                    if (slider != null)
                    {
                        slider.onChange.Remove(o.EventDelegate);
                        o.EventDelegate = null;
                    }
                }
            }
        }

        OptList<MonoBehaviour>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<MonoBehaviour>.List);
        {
            var __array8 = OptList<MonoBehaviour>.List;
            var __arrayLength8 = __array8.Count;
            for (var __i8 = 0; __i8 < __arrayLength8; ++__i8)
            {
                var child = __array8[__i8];
                {
                    var dispose = child as IPropertyChange;
                    if (dispose != null)
                    {
                        dispose.RemovePropertyChange();
                    }
                }
            }
        }
    }

    public void SetBindDataSource(object source)
    {
        if (source == null)
        {
            return;
        }
        mSource = source;
        IsBind = true;
        var objs = gameObject.GetComponentsInChildren<UIClassBinding>(true);
        {
            var __array9 = objs;
            var __arrayLength9 = __array9.Length;
            for (var __i9 = 0; __i9 < __arrayLength9; ++__i9)
            {
                var o = __array9[__i9];
                {
                    try
                    {
                        if (o.BindingName.ClassName == source.GetType().Name)
                        {
                            o.DataSource = source;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(
                            string.Format(
                                "SetBindDataSource Binding Error:\nGameObject[{3}]\nBindingName[{0}]\nClassName[{1}]\n{2}",
                                o.BindingName, o.BindingName.ClassName, ex, gameObject.transform.FullPath()));
                    }
                }
            }
        }

        var InvObjs = gameObject.GetComponentsInChildren<InverseBinding>(true);
        {
            var __array10 = InvObjs;
            var __arrayLength10 = __array10.Length;
            for (var __i10 = 0; __i10 < __arrayLength10; ++__i10)
            {
                var o = __array10[__i10];
                {
                    try
                    {
                        if (o.BindingName.ClassName == source.GetType().Name)
                        {
                            o.DataSource = source;

                            var input = o.gameObject.GetComponent<UIInput>();
                            if (input != null)
                            {
                                o.EventDelegate = new EventDelegate(o.UpdateSourceData);
                                input.onChange.Add(o.EventDelegate);
                            }
                            var toggle = o.gameObject.GetComponent<UIToggle>();
                            if (toggle != null)
                            {
                                o.EventDelegate = new EventDelegate(o.UpdateSourceData);
                                toggle.onChange.Add(o.EventDelegate);
                            }
                            var slider = o.gameObject.GetComponent<UISlider>();
                            if (slider != null)
                            {
                                o.EventDelegate = new EventDelegate(o.UpdateSourceData);
                                slider.onChange.Add(o.EventDelegate);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(string.Format("SetBindDataSource InversBinding {0}/{1}  /n{2}", o.BindingName,
                            o.BindingName.ClassName, ex));
                    }
                }
            }
        }
    }
}