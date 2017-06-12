#region using

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace GameUI
{
	public class WingLinkedNodeTransfer : MonoBehaviour
	{
	    public float Distance = 8;
	    public int mCurrentIndex;
	    public Dictionary<int, WingClickAndMoveTo> mDict = new Dictionary<int, WingClickAndMoveTo>();
	    public List<GameObject> mLine1s = new List<GameObject>();
	    public List<GameObject> mLines = new List<GameObject>();
	    private Vector3 mOriginalPositionLocal;
	    private Quaternion mOriginalRotationLocal;
	    public Transform mParent;
	    public Vector3 mScale;
	    public Transform mTransform;
	    public Action OnBeginRotate;
	    public Action OnMoveAction;
	    public Action OnZoomInAction;
	    public Action OnZoomOutAction;
	    public int RenderQueue = 3030;
	    public GameObject Sky;
	    public float Speed = 5.0f;
	    public float ZoomDistance = 15;
	
	    public int LookAtIndex
	    {
	        set { MoveTo(value, false); }
	    }
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        mTransform = transform;
	        var s = mTransform.GetComponentsInChildren<WingClickAndMoveTo>();
	        {
	            var __array1 = s;
	            var __arrayLength1 = __array1.Length;
	            for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
	            {
	                var t = __array1[__i1];
	                {
	                    t.Parent = this;
	                    mDict[t.Id] = t;
	                    t.Offset = mParent.position;
	                    t.Distance = Distance;
	                    t.Speed = Speed;
	                }
	            }
	        }
	        mOriginalPositionLocal = mTransform.localPosition;
	        mOriginalRotationLocal = mTransform.localRotation;
	        var line = mTransform.GetComponentInChildren<LineRenderer>();
	        for (var i = 0; i < s.Length - 1; ++i)
	        {
	            var l = (GameObject) Instantiate(line.gameObject);
	            mLines.Add(l);
	            l.transform.parent = mTransform;
	            l.transform.localPosition = Vector3.zero;
	            l.transform.localScale = Vector3.one;
	            l.transform.localRotation = Quaternion.identity;
	            var render = l.GetComponent<LineRenderer>();
	            render.SetVertexCount(2);
	            render.SetWidth(0.01f, 0.01f);
	            render.SetPosition(0, s[i].transform.localPosition);
	            render.SetPosition(1, s[i + 1].transform.localPosition);
	
	
	            var l1 = (GameObject) Instantiate(line.gameObject);
	            mLine1s.Add(l1);
	            l1.transform.parent = mTransform;
	            l1.transform.localPosition = Vector3.zero;
	            l1.transform.localScale = Vector3.one;
	            l1.transform.localRotation = Quaternion.identity;
	            render = l1.GetComponent<LineRenderer>();
	            render.SetVertexCount(2);
	            render.SetWidth(0.05f, 0.05f);
	            render.SetPosition(0, s[i].transform.localPosition);
	            render.SetPosition(1, s[i + 1].transform.localPosition);
	        }
	        gameObject.SetRenderQueue(RenderQueue);
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public void BeginRotate()
	    {
	        if (OnBeginRotate != null)
	        {
	            OnBeginRotate();
	        }
	    }
	
	    public void MoveOver()
	    {
	        if (OnMoveAction != null)
	        {
	            OnMoveAction();
	        }
	    }
	
	    public void MoveTo(int id, bool anim = true, Action onMove = null)
	    {
	        WingClickAndMoveTo obj;
	        if (mDict.TryGetValue(id, out obj))
	        {
	            if (anim)
	            {
	                obj.OnPress();
	                OnMoveAction = onMove;
	            }
	            else
	            {
	                obj.LookAt();
	            }
	        }
	    }
	
	    private void MoveToZoomInPosition()
	    {
	        iTween.MoveBy(gameObject,
	            iTween.Hash("amount", new Vector3(0, 0, ZoomDistance), "time", 0.6, "easetype", iTween.EaseType.linear,
	                "oncomplete", "ZoomOutOver"));
	    }
	
	    [ContextMenu("Next")]
	    public void Next()
	    {
	        if (mCurrentIndex >= mDict.Count)
	        {
	            mCurrentIndex = 0;
	        }
	        MoveTo(mCurrentIndex);
	        mCurrentIndex++;
	    }
	
	    public void Reset()
	    {
	        mTransform.localPosition = mOriginalPositionLocal;
	        mTransform.localRotation = mOriginalRotationLocal;
	        {
	            // foreach(var moveTo in mDict)
	            var __enumerator2 = (mDict).GetEnumerator();
	            while (__enumerator2.MoveNext())
	            {
	                var moveTo = __enumerator2.Current;
	                {
	                    moveTo.Value.Active = false;
	                }
	            }
	        }
	
	        var itweens = mTransform.GetComponentsInChildren<iTween>();
	        {
	            var __array3 = itweens;
	            var __arrayLength3 = __array3.Length;
	            for (var __i3 = 0; __i3 < __arrayLength3; ++__i3)
	            {
	                var t = __array3[__i3];
	                {
	                    Destroy(t);
	                }
	            }
	        }
	    }
	
	    public void SetBallActive(int id, bool active)
	    {
	        WingClickAndMoveTo obj;
	        if (mDict.TryGetValue(id, out obj))
	        {
	            obj.Active = active;
	        }
	    }
	
	    public void SetLineActive(int id, bool active)
	    {
	        if (id >= 0 && id < mLines.Count)
	        {
	            var l = mLines[id];
	            l.renderer.material.SetColor("_TintColor",
	                active ? new Color(0.48f, 0.7f, 0f) : new Color(0.2f, 0.2f, 0.2f));
	
	
	            var l1 = mLine1s[id];
	            l1.renderer.material.SetColor("_TintColor",
	                active ? new Color(0.48f, 0.7f, 0f) : new Color(0.0f, 0.0f, 0.0f));
	        }
	    }
	
	    private void Update()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    [ContextMenu("ZoomIn")]
	    public void ZoomIn(Action act = null)
	    {
	        OnZoomInAction = act;
	        Reset();
	        iTween.MoveBy(gameObject,
	            iTween.Hash("amount", new Vector3(0, 0, -ZoomDistance), "time", 1, "easetype", iTween.EaseType.linear,
	                "oncomplete", "ZoomInOver"));
	    }
	
	    public void ZoomInOver()
	    {
	        MoveTo(0, true, OnZoomInAction);
	    }
	
	    public void ZoomOut(Action act = null)
	    {
	        OnZoomOutAction = act;
	        iTween.RotateTo(gameObject,
	            iTween.Hash("rotation", mOriginalRotationLocal.eulerAngles, "time", 0.5, "islocal", true, "easetype",
	                iTween.EaseType.linear, "oncomplete", "ZoomOutStep1"));
	    }
	
	    public void ZoomOutOver()
	    {
	        if (OnZoomOutAction != null)
	        {
	            OnZoomOutAction();
	        }
	    }
	
	    public void ZoomOutStep1()
	    {
	        iTween.MoveTo(gameObject,
	            iTween.Hash("position",
	                mOriginalPositionLocal - new Vector3(0, 0, ZoomDistance/mTransform.root.localScale.x), "time", 1,
	                "islocal", true, "easetype", iTween.EaseType.linear, "oncomplete", "MoveToZoomInPosition"));
	    }
	
	    [ContextMenu("ZoomOut")]
	    public void ZoomOutTest()
	    {
	        ZoomOut(null);
	    }
	}
}