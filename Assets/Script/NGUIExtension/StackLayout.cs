#region using

using System;
using UnityEngine;

#endregion

public class StackLayout : UIWidget
{
    public bool mAutoLayout = true;
    public Pivot mChildrenPivot = Pivot.TopLeft;
    public int mLayoutType;
    public int mMaxCount;
    public int mMinHeight = -1;
    public int mOffset;
    public int mPadding;
    public bool mSetPivot = false;

    private void AdjustPovit(Pivot pivot, Transform trans)
    {
        var widget = trans.GetComponent<StackLayout>();
        // 调整锚点的位置
        if (pivot == Pivot.TopLeft)
        {
            switch (widget.pivot)
            {
                case Pivot.TopLeft:
                    SetOffsetForChildren(trans, new Vector3(0, 0, 0));
                    break;
                case Pivot.Top:
                    SetOffsetForChildren(trans, new Vector3(-widget.width/2, 0, 0));
                    break;
                case Pivot.TopRight:
                    SetOffsetForChildren(trans, new Vector3(-widget.width, 0, 0));
                    break;
                case Pivot.Left:
                    SetOffsetForChildren(trans, new Vector3(0, widget.height/2, 0));
                    break;
                case Pivot.Center:
                    SetOffsetForChildren(trans, new Vector3(-widget.width/2, widget.height/2, 0));
                    break;
                case Pivot.Right:
                    SetOffsetForChildren(trans, new Vector3(-widget.width, widget.height/2, 0));
                    break;
                case Pivot.BottomLeft:
                    SetOffsetForChildren(trans, new Vector3(0, widget.height, 0));
                    break;
                case Pivot.Bottom:
                    SetOffsetForChildren(trans, new Vector3(-widget.width/2, widget.height, 0));
                    break;
                case Pivot.BottomRight:
                    SetOffsetForChildren(trans, new Vector3(-widget.width, widget.height, 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else if (pivot == Pivot.Bottom)
        {
            switch (widget.pivot)
            {
                case Pivot.TopLeft:
                    SetOffsetForChildren(trans, new Vector3(widget.width/2, -widget.height, 0));
                    break;
                case Pivot.Top:
                    SetOffsetForChildren(trans, new Vector3(0, -widget.height, 0));
                    break;
                case Pivot.TopRight:
                    SetOffsetForChildren(trans, new Vector3(-widget.width/2, -widget.height, 0));
                    break;
                case Pivot.Left:
                    SetOffsetForChildren(trans, new Vector3(widget.width/2, -widget.height/2, 0));
                    break;
                case Pivot.Center:
                    SetOffsetForChildren(trans, new Vector3(0, -widget.height/2, 0));
                    break;
                case Pivot.Right:
                    SetOffsetForChildren(trans, new Vector3(-widget.width/2, -widget.height/2, 0));
                    break;
                case Pivot.BottomLeft:
                    SetOffsetForChildren(trans, new Vector3(widget.width/2, 0, 0));
                    break;
                case Pivot.Bottom:
                    SetOffsetForChildren(trans, new Vector3(0, 0, 0));
                    break;
                case Pivot.BottomRight:
                    SetOffsetForChildren(trans, new Vector3(-widget.width/2, 0, 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void LayoutGroup(Transform trans)
    {
        if (trans.childCount == 0 || !trans.gameObject.activeSelf)
        {
        }
        else
        {
            var transchildCount0 = trans.childCount;
            for (var i = 0; i < transchildCount0; i++)
            {
                var child = trans.GetChild(i);
                LayoutGroup(child);
            }

            var widget = trans.gameObject.GetComponent<StackLayout>();

            if (widget == null)
            {
                return;
            }

            // calculate width and height
            var lastPos = 0;
            var maxPos = 0;
            var accuPos = 0;
            var padding = widget.mPadding;
            var offset = widget.mOffset;
            var curIndex = 0;
            var lastCol = 0;
            var maxWidth = 0;
            var maxCount = widget.mMaxCount;
            var minHeight = widget.mMinHeight;
            var vertical = widget.mLayoutType == 0;
            var transchildCount1 = trans.childCount;
            var lastChange = 0;


            for (var i = 0; i < transchildCount1; i++)
            {
                var child = trans.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }
                var childWidget = child.gameObject.GetComponent<UIWidget>();
                if (childWidget == null)
                {
                    continue;
                }
                var localScale = childWidget.transform.localScale;
                var childWidgetWidth = (int) (childWidget.width*localScale.x);
                var childWidgetHeight = (int) (childWidget.height*localScale.y);
                if (widget.mLayoutType == 0)
                {
                    SetTopLeftCornerPosition(widget.mChildrenPivot, childWidget, offset, lastPos);
                    lastPos -= childWidgetHeight;
                    maxPos = Math.Max(maxPos, childWidgetWidth);
                    lastPos -= padding;
                    accuPos += childWidgetHeight;

                    var childLayout = child.gameObject.GetComponent<StackLayout>();
                    if (childLayout && childLayout.mLayoutType == 1)
                    {
                        lastPos -= childLayout.mOffset;
                    }
                }
                else if (widget.mLayoutType == 1)
                {
                    SetTopLeftCornerPosition(widget.mChildrenPivot, childWidget, lastPos, -offset);
                    if (maxCount != 0 && curIndex%maxCount == maxCount - 1)
                    {
                        accuPos += childWidgetWidth;
                        maxWidth = Math.Max(maxWidth, accuPos);
                        offset += childWidgetHeight;
                        offset += padding;
                        accuPos = 0;
                        lastPos = 0;
                        maxPos = offset;
                    }
                    else
                    {
                        lastPos += childWidgetWidth;
                        maxPos = Math.Max(maxPos, childWidgetHeight);
                        lastPos += padding;
                        accuPos += childWidgetWidth;
                        maxWidth = Math.Max(maxWidth, accuPos);
                    }
                    lastChange = childWidgetHeight + padding;
                    curIndex++;
                }
                else if (widget.mLayoutType == 2)
                {
                    SetTopLeftCornerPosition(widget.mChildrenPivot, childWidget, offset, lastPos);
                    lastPos += (int) (childWidget.height*localScale.y);
                    maxPos = Math.Max(maxPos, childWidgetWidth);
                    lastPos += padding;
                    accuPos += (int) (childWidget.height*localScale.y);

                    var childLayout = child.gameObject.GetComponent<StackLayout>();
                    if (childLayout && childLayout.mLayoutType == 1)
                    {
                        lastPos += childLayout.mOffset;
                    }
                }
                else if (widget.mLayoutType == 3)
                {
                    SetTopLeftCornerPosition(widget.mChildrenPivot, childWidget, lastPos, -offset);

                    lastPos -= childWidgetWidth;
                    maxPos = Math.Max(maxPos, childWidgetHeight);
                    lastPos -= padding;
                    accuPos -= childWidgetWidth;
                    maxWidth = Math.Max(maxWidth, accuPos);

                    lastChange = childWidgetHeight + padding;
                    curIndex++;
                }

                if (i != trans.childCount - 1)
                {
                    accuPos += padding;
                }
            }


            if (widget.mLayoutType == 0
                || widget.mLayoutType == 2)
            {
                if (minHeight > 0 && accuPos < minHeight)
                {
                    accuPos = minHeight;
                }
                var widgets = widget.gameObject.GetComponents<UIWidget>();
                {
                    var __array1 = widgets;
                    var __arrayLength1 = __array1.Length;
                    for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                    {
                        var widget1 = __array1[__i1];
                        {
                            widget1.width = maxPos;
                            widget1.height = accuPos;
                        }
                    }
                }
            }
            else
            {
                if (maxCount != 0 && curIndex%maxCount != 0)
                {
                    offset += lastChange;
                    maxPos = offset;
                }

                var widgets = widget.gameObject.GetComponents<UIWidget>();
                {
                    var __array2 = widgets;
                    var __arrayLength2 = __array2.Length;
                    for (var __i2 = 0; __i2 < __arrayLength2; ++__i2)
                    {
                        var widget1 = __array2[__i2];
                        {
                            widget1.width = maxWidth;
                            widget1.height = maxPos;
                        }
                    }
                }
            }

            if (mSetPivot)
            {
                AdjustPovit(widget.mChildrenPivot, trans);
            }
        }
    }

    //public int mUpdatedFrame = 0;

    private void OnEnable()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (mAutoLayout)
        {
            LayoutGroup(transform);
            //StartCoroutine(ResetLayoutCoroutineInternal());
        }


#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    [ContextMenu("ResetLayout")]
    public void ResetLayout()
    {
        //if(mUpdatedFrame == Time.frameCount)
        //    return;

        //mUpdatedFrame = Time.frameCount;
        LayoutGroup(transform);
    }

//     public IEnumerator ResetLayoutCoroutineInternal()
//     {
//         yield return new WaitForEndOfFrame();
//         yield return new WaitForEndOfFrame();
//         LayoutGroup(this.transform);
//         yield break;
//     }

    private void SetOffsetForChildren(Transform trans, Vector3 offset)
    {
        var transchildCount0 = trans.childCount;
        for (var i = 0; i < transchildCount0; i++)
        {
            var child = trans.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                child.localPosition += offset;
            }
        }
    }

    private void SetTopLeftCornerPosition(Pivot pivot, UIWidget childWidget, float x, float y)
    {
        var localScale = childWidget.transform.localScale;
        var width = childWidget.width*localScale.x;
        var height = childWidget.height*localScale.y;
        var childObjTransform = childWidget.gameObject.transform;
        if (pivot == Pivot.TopLeft)
        {
            switch (childWidget.pivot)
            {
                case Pivot.TopLeft:
                    childObjTransform.localPosition = new Vector3(x, y, 0);
                    break;
                case Pivot.Top:
                    childObjTransform.localPosition = new Vector3(x + width/2, y, 0);
                    break;
                case Pivot.TopRight:
                    childObjTransform.localPosition = new Vector3(x + width, y, 0);
                    break;
                case Pivot.Left:
                    childObjTransform.localPosition = new Vector3(x, y - height/2, 0);
                    break;
                case Pivot.Center:
                    childObjTransform.localPosition = new Vector3(x + width/2, y - height/2, 0);
                    break;
                case Pivot.Right:
                    childObjTransform.localPosition = new Vector3(x + width, y - height/2, 0);
                    break;
                case Pivot.BottomLeft:
                    childObjTransform.localPosition = new Vector3(x, y - height, 0);
                    break;
                case Pivot.Bottom:
                    childObjTransform.localPosition = new Vector3(x + width/2, y - height, 0);
                    break;
                case Pivot.BottomRight:
                    childObjTransform.localPosition = new Vector3(x + width, y - height, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else if (pivot == Pivot.Bottom)
        {
            switch (childWidget.pivot)
            {
                case Pivot.TopLeft:
                    childObjTransform.localPosition = new Vector3(x - width/2, y + height, 0);
                    break;
                case Pivot.Top:
                    childObjTransform.localPosition = new Vector3(x, y + height, 0);
                    break;
                case Pivot.TopRight:
                    childObjTransform.localPosition = new Vector3(x + width/2, y + height, 0);
                    break;
                case Pivot.Left:
                    childObjTransform.localPosition = new Vector3(x - width/2, y + height/2, 0);
                    break;
                case Pivot.Center:
                    childObjTransform.localPosition = new Vector3(x, y + height/2, 0);
                    break;
                case Pivot.Right:
                    childObjTransform.localPosition = new Vector3(x + width/2, y + height/2, 0);
                    break;
                case Pivot.BottomLeft:
                    childObjTransform.localPosition = new Vector3(x - width/2, y + height, 0);
                    break;
                case Pivot.Bottom:
                    childObjTransform.localPosition = new Vector3(x, y + height, 0);
                    break;
                case Pivot.BottomRight:
                    childObjTransform.localPosition = new Vector3(x + width/2, y + height, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}