#region using

using System;
using System.Collections;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class FightingCapacityFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private int modifyKind;
	    public GameObject DownArrow;
	    private bool isAssigned;
	    private bool isEnable;
	    public List<UILabel> LabelList1;
	    public List<UILabel> LabelList2;
	    private int startValue;
	    private int endValue;
	    private List<ChangeNode> nodeList;
	    private Coroutine overCoroutine;
	    public GameObject UpArrow;
	    public UILabel NumberLable;
	    public GameObject AniLable;
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	            //Binding.RemoveBinding();
	            EventDispatcher.Instance.RemoveEventListener(FightValueChange.EVENT_TYPE, OnFightValueChange);
	            isEnable = false;
	            //this.gameObject.SetActive(false);
	            overCoroutine = null;
	            var e = new Close_UI_Event(UIConfig.ForceUI);
	            EventDispatcher.Instance.DispatchEvent(e);
	
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
	
	            if (isAssigned == false)
	            {
	                isAssigned = true;
	                OnInitList();
	            }
	            EventDispatcher.Instance.AddEventListener(FightValueChange.EVENT_TYPE, OnFightValueChange);
	            UpArrow.SetActive(false);
	            DownArrow.SetActive(false);
	            //Refresh(mBeinValue, mEndValue);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnFightValueChange(IEvent ievent)
	    {
	        var e = ievent as FightValueChange;
	        {
	            var __list1 = nodeList;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var changeNode = __list1[__i1];
	                {
	                    changeNode.Reset();
	                }
	            }
	        }
	
	        var num = (e.EndValue - e.BeginValue);
	        if (num > 0)
	        {
	            NumberLable.text = num.ToString();
	            AniLable.SetActive(true);
	            var ani = AniLable.GetComponent<Animation>();
	            if (ani)
	            {
	                ani.Play();
	            }
	        }
	        else
	        {
	            AniLable.SetActive(false);
	        }
	
	        Refresh(e.BeginValue, e.EndValue);
	    }
	
	    private void OnInitList()
	    {
	        nodeList = new List<ChangeNode>();
	        for (var i = 0; i < 9; i++)
	        {
	            var node = new ChangeNode();
	            node.IsOver = true;
	            node.Label1 = LabelList1[i];
	            node.Label1.gameObject.SetActive(false);
	            node.Label2 = LabelList2[i];
	            node.Label2.gameObject.SetActive(false);
	            node.Index = 0;
	            node.Init();
	            nodeList.Add(node);
	        }
	    }
	
	    public IEnumerator OverEnumerator()
	    {
	        yield return new WaitForSeconds(3.5f);
	        isEnable = false;
	        gameObject.SetActive(false);
	        var e = new Close_UI_Event(UIConfig.ForceUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	        overCoroutine = null;
	    }
	
	    public void Refresh(int b, int e)
	    {
	        if (overCoroutine != null)
	        {
	            StopCoroutine(overCoroutine);
	        }
	        var beginValue = 0;
	        var endValue = 0;
	        beginValue = b;
	        endValue = e;
	        endValue = e;
	        startValue = b;
	        if (beginValue < endValue)
	        {
	            modifyKind = 1;
	            UpArrow.SetActive(true);
	            DownArrow.SetActive(false);
	        }
	        else if (beginValue > endValue)
	        {
	            modifyKind = -1;
	            UpArrow.SetActive(false);
	            DownArrow.SetActive(true);
	        }
	        else
	        {
	            return;
	        }
	        var start = false;
	        var totalLegth = 0;
	        var LastLegth = -1;
	        var diffCount = 0;
	        ChangeNode LastNode = null;
	        for (var i = 9 - 1; i >= 0; i--)
	        {
	            var pow = (int) Mathf.Pow(10, i);
	            var begin = beginValue/pow;
	            var end = endValue/pow;
	            var node = nodeList[i];
	            if (!start)
	            {
	                if (begin == 0 && end == 0)
	                {
	                    node.Reset();
	                    continue;
	                }
	                start = true;
	            }
	            beginValue = beginValue%pow;
	            endValue = endValue%pow;
	            if (LastNode == null)
	            {
	                LastNode = nodeList[i];
	                node.Before = null;
	            }
	            else
	            {
	                node.Before = LastNode;
	                LastNode.After = node;
	                LastNode = node;
	            }
	            var label1Trans = node.Label1.transform;
	            var label2Trans = node.Label2.transform;
	            if (begin == end)
	            {
	                if (diffCount == 0)
	                {
	                    node.Label1.gameObject.SetActive(true);
	                    label1Trans.localPosition = new Vector3(totalLegth*20.0f, 0, 0);
	                    node.Label2.gameObject.SetActive(false);
	                    node.SetText1(begin);
	                    node.IsOver = true;
	                    totalLegth ++;
	                    continue;
	                }
	            }
	
	            node.Index = i;
	            node.IsOver = false;
	            node.Bgein = begin;
	            node.Label1.gameObject.SetActive(true);
	            node.Label1.text = begin.ToString();
	            node.Now = nodeList[i].Bgein;
	            node.End = end;
	            label1Trans.localPosition = new Vector3(totalLegth*20.0f, 0, 0);
	            node.Label2.gameObject.SetActive(true);
	
	            if (modifyKind == -1)
	            {
	                label2Trans.localPosition = new Vector3(totalLegth*20.0f, 30, 0);
	            }
	            else
	            {
	                label2Trans.localPosition = new Vector3(totalLegth*20.0f, -30, 0);
	            }
	            node.Length = end - begin + diffCount*10*modifyKind;
	            if (node.Length > 20)
	            {
	                node.Length = 20 + diffCount*2; //node.Length%20 + 10;//  node.Length % 20 + 10;
	            }
	            else if (node.Length < -20)
	            {
	                node.Length = -20 - diffCount*2; //node.Length % 20 - 10;//
	            }
	            node.SetText2(ChangeNode.GetNext(node.Now, modifyKind));
	            node.Now = ChangeNode.GetNext(node.Now, modifyKind);
	            totalLegth++;
	            diffCount++;
	            //node.Now = diffCount;
	        }
	
	        overCoroutine = StartCoroutine(OverEnumerator());
	        isEnable = true;
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
	
	    private void Update()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	            if (!isEnable)
	            {
	                return;
	            }
	            for (var i = 0; i < 9; i++)
	            {
	                var node = nodeList[i];
	                if (node.IsOver)
	                {
	                    continue;
	                }
	                // (2 + node.Index/10.0f)
	                if (startValue.ToString().Length < endValue.ToString().Length)
	                {
	                    var dif = Math.Abs(Time.deltaTime/(2 + node.Index/5.0f)*node.GetLength()*30);
	                    node.SetDiff(dif, modifyKind);
	                }
	                else
	                {
	                    var dif = Math.Abs(Time.deltaTime/(2 - node.Index/5.0f)*node.GetLength()*30);
	                    node.SetDiff(dif, modifyKind);
	                }
	                node.CheckEnd();
	            }
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public class ChangeNode
	    {
	        public ChangeNode After;
	        public ChangeNode Before;
	        public int Bgein;
	        public int End;
	        public int Index;
	        public UILabel Label1;
	        public UILabel Label2;
	        public bool Over;
	        private Transform label1Transform;
	        private Transform label2Transform;
	        //public int Next;
	        public int Now;
	        public bool SpeedSlow;
	        public float TotleDiff;
	
	        public bool IsOver
	        {
	            get { return Over; }
	            set
	            {
	                Over = value;
	                if (value)
	                {
	                    TotleDiff = 0;
	                    SpeedSlow = false;
	                }
	            }
	        }
	
	        public int Length { get; set; }
	
	        public void CheckEnd()
	        {
	            if (Math.Abs(Length*30) < Math.Abs(TotleDiff))
	            {
	                label1Transform.localPosition = new Vector3(label1Transform.localPosition.x, 0, 0);
	                Label2.gameObject.SetActive(false);
	                IsOver = true;
	                SetText1(End);
	            }
	        }
	
	        public float CheckPos(float nowY)
	        {
	            if (nowY > 30)
	            {
	                nowY = 30;
	            }
	            return nowY;
	        }
	
	        public float GetLength()
	        {
	            if (SpeedSlow)
	            {
	                return Length*0.4f;
	            }
	            float lenths = Math.Abs(Length*30);
	            if (lenths*0.9f < Math.Abs(TotleDiff))
	            {
	                SpeedSlow = true;
	                if (Math.Abs(Length) < 20)
	                {
	                    return Length*0.4f;
	                }
	                var ttt = ((lenths - Math.Abs(TotleDiff))/9);
	                var count = (int) ttt/30;
	                var e = End;
	                for (var i = 0; i != 10; ++i)
	                {
	                    if (count%10 == 8 - i)
	                    {
	                        break;
	                    }
	                    if (Length > 0)
	                    {
	                        e = GetNext(e, 1);
	                    }
	                    else
	                    {
	                        e = GetNext(e, -1);
	                    }
	                }
	                Now = e;
	                SetText1(Now);
	                if (Length > 0)
	                {
	                    SetText2(GetNext(e, 1));
	                }
	                else
	                {
	                    SetText2(GetNext(e, -1));
	                }
	                return Length*0.4f;
	            }
	            return Length;
	        }
	
	        public static int GetNext(int now, int diff)
	        {
	            if (diff == -1)
	            {
	                if (now == 0)
	                {
	                    return 9;
	                }
	            }
	            else
	            {
	                if (now == 9)
	                {
	                    return 0;
	                }
	            }
	            return now + diff;
	        }
	
	        public void Init()
	        {
	            label1Transform = Label1.transform;
	            label2Transform = Label2.transform;
	        }
	
	        public void Reset()
	        {
	            Label1.gameObject.SetActive(false);
	            Label2.gameObject.SetActive(false);
	            IsOver = true;
	            Before = null;
	        }
	
	        public void SetDiff(float dif, int ChangeType)
	        {
	            TotleDiff += dif;
	            dif = CheckPos(dif);
	            var pos1 = label1Transform.localPosition;
	            if (ChangeType == -1)
	            {
	                label1Transform.localPosition = new Vector3(pos1.x, pos1.y - dif, 0);
	                var pos2 = label2Transform.localPosition;
	                label2Transform.localPosition = new Vector3(pos2.x, pos2.y - dif, 0);
	                // SetText1(Now);
	                if (pos1.y - dif <= -30)
	                {
	                    SwapLab();
	                    var next = GetNext(Now, ChangeType);
	                    Now = next;
	                    SetText2(next);
	                    label2Transform.localPosition = new Vector3(pos2.x, label1Transform.localPosition.y + 30, 0);
	                }
	            }
	            else if (ChangeType == 1)
	            {
	                label1Transform.localPosition = new Vector3(pos1.x, pos1.y + dif, 0);
	                var pos2 = label2Transform.localPosition;
	                label2Transform.localPosition = new Vector3(pos2.x, pos2.y + dif, 0);
	                //SetText1(Now);
	                if (pos1.y + dif >= 30)
	                {
	                    SwapLab();
	                    var next = GetNext(Now, ChangeType);
	                    Now = next;
	                    SetText2(next);
	                    label2Transform.localPosition = new Vector3(pos2.x, label1Transform.localPosition.y - 30, 0);
	                }
	            }
	        }
	
	        public void SetText1(int t)
	        {
	            if (Before == null)
	            {
	                if (t == 0)
	                {
	                    Label1.text = "";
	                    if (After != null)
	                    {
	                        After.Before = null;
	                    }
	                }
	                else
	                {
	                    Label1.text = t.ToString();
	                }
	            }
	            else
	            {
	                Label1.text = t.ToString();
	            }
	        }
	
	        public void SetText2(int t)
	        {
	            if (Before == null && t == 0)
	            {
	                Label2.text = "";
	            }
	            else
	            {
	                Label2.text = t.ToString();
	            }
	        }
	
	        public void SwapLab()
	        {
	            var lab = Label1;
	            Label1 = Label2;
	            Label2 = lab;
	            label1Transform = Label1.transform;
	            label2Transform = Label2.transform;
	            Label1.gameObject.SetActive(true);
	            Label2.gameObject.SetActive(true);
	        }
	    }
	}
}
