#region using

using System;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class BlacksmithShopFly : MonoBehaviour
	{
	    public List<UIPanel> AnimPanels;
	    private readonly List<BoxCollider> BoxColliders = new List<BoxCollider>();
	    public List<GameObject> FlyObj;
	    public Vector3 FromPos = Vector3.zero;
	    private bool isPlay;
	    private readonly List<Vector3> OrgPosition = new List<Vector3>();
	    public List<UIPanel> Panels;
	    private readonly Dictionary<int, int> PosDict = new Dictionary<int, int> {{0, 0}, {1, 1}, {2, 2}};
	    public Vector3 ScaleParam = new Vector3(0.7f, 0.7f, 0.7f);
	    private readonly List<TweenScale> Scales = new List<TweenScale>();
	    public Vector3 ToPos = Vector3.zero;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        OrgPosition.Clear();
	        for (var i = 0; i < FlyObj.Count; i++)
	        {
	            OrgPosition.Add(FlyObj[i].transform.localPosition);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void FlyToPos(bool rotation, int index) //true ???? //false?????
	    {
	        if (FlyObj == null)
	        {
	            return;
	        }
	        if (FlyObj.Count != 3)
	        {
	            return;
	        }
	        SetParam(rotation);
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithItemClick(0, index));
	        if (rotation)
	        {
	            StartFly(FlyObj[PosDict[0]], OrgPosition[2], null);
	            StartFly(FlyObj[PosDict[1]], OrgPosition[0], null);
	            StartFly(FlyObj[PosDict[2]], OrgPosition[1], () =>
	            {
	                var temp = PosDict[0];
	                PosDict[0] = PosDict[1];
	                PosDict[1] = PosDict[2];
	                PosDict[2] = temp;
	                isPlay = false;
	                BoxColliders[PosDict[0]].enabled = true;
	                BoxColliders[PosDict[1]].enabled = false;
	                BoxColliders[PosDict[2]].enabled = true;
	
	                //EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithItemClick(1,index));
	            });
	        }
	        else
	        {
	            StartFly(FlyObj[PosDict[0]], OrgPosition[1], null);
	            StartFly(FlyObj[PosDict[1]], OrgPosition[2], null);
	            StartFly(FlyObj[PosDict[2]], OrgPosition[0], () =>
	            {
	                var temp = PosDict[0];
	                PosDict[0] = PosDict[2];
	                PosDict[2] = PosDict[1];
	                PosDict[1] = temp;
	                isPlay = false;
	                BoxColliders[PosDict[0]].enabled = true;
	                BoxColliders[PosDict[1]].enabled = false;
	                BoxColliders[PosDict[2]].enabled = true;
	                //EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithItemClick(1,index));
	            });
	        }
	    }
	
	    private void InitScale()
	    {
	        FlyObj[0].transform.localScale = ScaleParam;
	        FlyObj[0].transform.localPosition = OrgPosition[0];
	        FlyObj[1].transform.localScale = Vector3.one;
	        FlyObj[1].transform.localPosition = OrgPosition[1];
	        FlyObj[2].transform.localScale = ScaleParam;
	        FlyObj[2].transform.localPosition = OrgPosition[2];
	        PosDict[0] = 0;
	        PosDict[1] = 1;
	        PosDict[2] = 2;
	        Scales.Clear();
	        for (var i = 0; i < 3; i++)
	        {
	            Scales.Add(FlyObj[i].GetComponent<TweenScale>());
	            BoxColliders.Add(FlyObj[i].GetComponent<BoxCollider>());
	        }
	        BoxColliders[0].enabled = true;
	        BoxColliders[1].enabled = false;
	        BoxColliders[2].enabled = true;
	
	        isPlay = false;
	    }
	
	    private void CellClick(int objIndex)
	    {
	        if (objIndex > 2 || objIndex < 0)
	        {
	            return;
	        }
	        var mIndex = 0;
	        foreach (var item in PosDict)
	        {
	            if (item.Value == objIndex)
	            {
	                mIndex = item.Key;
	                break;
	            }
	        }
	        if (mIndex == 0)
	        {
	            FlyToPos(false, objIndex);
	        }
	        else if (mIndex == 2)
	        {
	            FlyToPos(true, objIndex);
	        }
	    }
	
	    public void ItemClick0()
	    {
	        if (!isPlay)
	        {
	            CellClick(0);
	        }
	    }
	
	    public void ItemClick1()
	    {
	        if (!isPlay)
	        {
	            CellClick(1);
	        }
	    }
	
	    public void ItemClick2()
	    {
	        if (!isPlay)
	        {
	            CellClick(2);
	        }
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        BoxColliders.Clear();
	        Scales.Clear();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        InitScale();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void SetParam(bool rotation)
	    {
	        if (rotation)
	        {
	            Scales[PosDict[0]].from = ScaleParam;
	            Scales[PosDict[0]].to = ScaleParam;
	            Scales[PosDict[1]].from = Vector3.one;
	            Scales[PosDict[1]].to = ScaleParam;
	            Scales[PosDict[2]].from = ScaleParam;
	            Scales[PosDict[2]].to = Vector3.one;
	
	            Panels[PosDict[0]].depth = 9;
	            Panels[PosDict[1]].depth = 19;
	            Panels[PosDict[2]].depth = 11;
	
	            AnimPanels[PosDict[0]].depth = 10;
	            AnimPanels[PosDict[1]].depth = 21;
	            AnimPanels[PosDict[2]].depth = 13;
	        }
	        else
	        {
	            Scales[PosDict[2]].from = ScaleParam;
	            Scales[PosDict[2]].to = ScaleParam;
	            Scales[PosDict[1]].from = Vector3.one;
	            Scales[PosDict[1]].to = ScaleParam;
	            Scales[PosDict[0]].from = ScaleParam;
	            Scales[PosDict[0]].to = Vector3.one;
	
	            Panels[PosDict[0]].depth = 11;
	            Panels[PosDict[1]].depth = 19;
	            Panels[PosDict[2]].depth = 9;
	
	            AnimPanels[PosDict[0]].depth = 13;
	            AnimPanels[PosDict[1]].depth = 21;
	            AnimPanels[PosDict[2]].depth = 10;
	        }
	    }
	
	    private void Start()
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
	
	    public void StartFly(GameObject from, Vector3 to, Action onFinished)
	    {
	        var TweenPos = from.GetComponent<TweenPosition>();
	        var tweenScale = from.GetComponent<TweenScale>();
	        TweenPos.onFinished.Clear();
	        TweenPos.AddOnFinished(() =>
	        {
	            //TweenPos.ResetForPlay();
	            //tweenScale.ResetForPlay();
	            if (onFinished != null)
	            {
	                onFinished();
	            }
	        });
	        TweenPos.from = from.transform.localPosition;
	        TweenPos.to = to;
	        TweenPos.ResetForPlay();
	        TweenPos.PlayForward();
	        tweenScale.ResetForPlay();
	        tweenScale.PlayForward();
	        isPlay = true;
	    }
	}
}