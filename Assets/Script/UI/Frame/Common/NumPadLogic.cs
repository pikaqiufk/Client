#region using

using System;
using System.Collections;
using EventSystem;
using GameUI;
using UnityEngine;

#endregion

namespace GameUI
{
	public class NumPadLogic : MonoBehaviour
	{
	    /// <summary>
	    ///     通过关闭按钮返回,result 回调中返回-1
	    /// </summary>
	    /// <param name="minValue"></param>
	    /// <param name="MaxValue"></param>
	    /// <param name="result"></param>
	    public static void ShowNumberPad(int minValue, int MaxValue, Action<int> result)
	    {
	        var uiroot = FindObjectOfType<UIRoot>();
	        if (null == uiroot)
	        {
	            return;
	        }
	
	        var res = ResourceManager.PrepareResourceSync<GameObject>(prefab);
	        Instance = Instantiate(res) as GameObject;
	        if (Instance != null)
	        {
	            var logic = Instance.GetComponent<NumPadLogic>();
	            logic.MinValue = minValue;
	            logic.MaxValue = MaxValue;
	            var t = Instance.transform;
	            //t.parent = uiroot.transform;
	            t.SetParentEX(uiroot.transform);
	            t.localScale = Vector3.one;
	            var collider = Instance.AddComponent<BoxCollider>();
	            collider.isTrigger = true;
	            collider.size = new Vector3(1136, 1136, 0);
	            Instance.SetActive(true);
	        }
	        callBack = result;
	    }
	
	    #region 私有
	
	    // Use this for initialization
	    public int MinValue { get; set; }
	    public int MaxValue { get; set; }
	
	    private int mValue;
	    private UIButton mEnterButton;
	
	    public int outPutValue
	    {
	        get { return mValue; }
	        set
	        {
	            if (mValue != value)
	            {
	                mValue = value;
	                if (null != OutPutLabel)
	                {
	                    OutPutLabel.text = mValue.ToString();
	                }
	            }
	        }
	    }
	
	    public static GameObject Instance;
	    private const string prefab = "UI/Common/NumberPad.prefab";
	    private static Action<int> callBack;
	    public UILabel OutPutLabel;
	    public UILabel TipLabel;
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var scripts = gameObject.GetComponentsInChildren<BtnValue>();
	
	        var count = scripts.Length;
	        for (var i = 0; i < count; i++)
	        {
	            var script = scripts[i];
	            var label = script.gameObject.GetComponentInChildren<UILabel>();
	            var btn = script.gameObject.GetComponentInChildren<UIButton>();
	            btn.onClick.Add(new EventDelegate(script.NumberClick));
	
	            if (script.Value == 10)
	            {
	                label.text = "Del";
	            }
	            else if (script.Value == 11)
	            {
	                label.text = "Enter";
	                mEnterButton = btn;
	            }
	            else
	            {
	                label.text = script.Value.ToString();
	            }
	        }
	        InputNumber(new UIEvent_NumberPad_Click(0));
	        TipLabel.text = string.Format("({0}-{1})", MinValue, MaxValue);
	
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
	        EventDispatcher.Instance.AddEventListener(UIEvent_NumberPad_Click.EVENT_TYPE, InputNumber);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_NumberPad_Click.EVENT_TYPE, InputNumber);
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void InputNumber(IEvent ievent)
	    {
	        var e = ievent as UIEvent_NumberPad_Click;
	        var input = e.keyValue;
	
	        if (input == 10)
	        {
	            outPutValue = outPutValue/10;
	        }
	        else if (input == 11)
	        {
	            if (callBack != null)
	            {
	                try
	                {
	                    callBack(outPutValue);
	                }
	                catch (Exception exp)
	                {
	                    Logger.Error("NumberPad callback error:{0}", exp);
	                }
	            }
	            NetManager.Instance.StartCoroutine(DestoryNextFrame());
	            return;
	        }
	        else
	        {
	            if (outPutValue == 0)
	            {
	                outPutValue = input;
	            }
	            else
	            {
	                outPutValue = outPutValue*10 + input;
	            }
	        }
	
	
	        if (outPutValue < MinValue)
	        {
	            OutPutLabel.color = Color.red;
	            mEnterButton.isEnabled = false;
	        }
	        else
	        {
	            OutPutLabel.color = Color.white;
	            mEnterButton.isEnabled = true;
	        }
	
	        if (outPutValue > MaxValue)
	        {
	            outPutValue = MaxValue;
	        }
	
	        OutPutLabel.text = outPutValue.ToString();
	    }
	
	    private IEnumerator DestoryNextFrame()
	    {
	        yield return new WaitForEndOfFrame();
	        Destroy(Instance);
	        Instance = null;
	    }
	
	    public void OnExitClick()
	    {
	        if (callBack != null)
	        {
	            try
	            {
	                callBack(-1);
	            }
	            catch (Exception exp)
	            {
	                Logger.Error("NumberPad callback error:{0}", exp);
	            }
	        }
	        NetManager.Instance.StartCoroutine(DestoryNextFrame());
	    }
	
	    #endregion
	}
}