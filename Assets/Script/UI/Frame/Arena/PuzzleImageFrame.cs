using System;
#region using

using System.Collections;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PuzzleImageFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public Particle2D EffectParticle;
	    public TweenAlpha EffecttTween;
	    public PuzzleMoveFrame PuzzleMove;
	    public TimerLogic PuzzlePlay;
	    public UISliderNormal PuzzleSlider;
	    public UILabel PuzzleSliderTime;
	    public GameObject PuzzleStart;
	    public TimerLogic PuzzleWarn;
	
	    public void OnClickPuzzelClose()
	    {
	//设置结束
	        if (PuzzleMove.IsStart)
	        {
	            PuzzlePlay.TargetTime = Game.Instance.ServerTime;
	        }
	        else
	        {
	            var e1 = new Close_UI_Event(UIConfig.PuzzleImage);
	            EventDispatcher.Instance.DispatchEvent(e1);
	        }
	    }
	
	    public void OnClickPuzzelStart()
	    {
	        PuzzleWarn.TargetTime = Game.Instance.ServerTime.AddSeconds(3);
	        PuzzleWarn.gameObject.SetActive(true);
	        PuzzleStart.SetActive(false);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (PuzzleMove != null)
	        {
	            if (PuzzleMove.OnPuzzleFinish != null)
	            {
	                PuzzleMove.OnPuzzleFinish -= OnPuzzleFinish;
	            }
	        }
	
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
	
	        Binding.RemoveBinding();
	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.PuzzleImage);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	
	        OnPuzzelShow();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnPuzzleFinish()
	    {
	        //挑战成功
	
	        EffecttTween.gameObject.SetActive(true);
	        EffecttTween.ResetToBeginning();
	        EffecttTween.PlayForward();
	        EffectParticle.gameObject.SetActive(true);
	
	        var e1 = new ShowUIHintBoard(270004);
	        EventDispatcher.Instance.DispatchEvent(e1);
	        NetManager.Instance.StartCoroutine(ShowPuzzleFinish());
	    }
	
	    public void OnFormatPuzzelPlay(UILabel lable)
	    {
	        if (lable == null)
	        {
	            return;
	        }
	        var timer = lable.GetComponent<TimerLogic>();
	        if (timer == null)
	        {
	            return;
	        }
	        var taget = timer.TargetTime;
	        if (taget > Game.Instance.ServerTime)
	        {
	            var dif = taget - Game.Instance.ServerTime;
	            lable.text = string.Format("{0}", (int) dif.TotalSeconds);
	            PuzzleSlider.value = (float) dif.TotalSeconds/120.0f;
	        }
	        else
	        {
	            if (!PuzzleMove.IsFinish)
	            {
	                OnClosePuzzel();
	            }
	        }
	    }
	
	    public void OnFormatPuzzelWarn(UILabel lable)
	    {
	        if (lable == null)
	        {
	            return;
	        }
	        var timer = lable.GetComponent<TimerLogic>();
	        if (timer == null)
	        {
	            return;
	        }
	        var taget = timer.TargetTime;
	        if (taget > Game.Instance.ServerTime)
	        {
	            lable.text = string.Format("{0}", (int) (taget - Game.Instance.ServerTime).TotalSeconds + 1);
	        }
	        else
	        {
	            lable.gameObject.SetActive(false);
	            PuzzleMove.ReStart();
	            PuzzlePlay.TargetTime = Game.Instance.ServerTime.AddSeconds(120);
	        }
	    }
	
	    public void OnClosePuzzel()
	    {
	        if (PuzzleMove.IsStart && !PuzzleMove.IsFinish)
	        {
	            var e = new SatueOperateEvent(2);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	        var e1 = new Close_UI_Event(UIConfig.PuzzleImage);
	        EventDispatcher.Instance.DispatchEvent(e1);
	        OnResetPuzzel();
	    }
	
	    public void OnResetPuzzel()
	    {
	        EffecttTween.gameObject.SetActive(false);
	        EffectParticle.gameObject.SetActive(false);
	        PuzzleMove.Reset();
	    }
	
	    public void OnPuzzelShow()
	    {
	        OnResetPuzzel();
	        PuzzleStart.SetActive(true);
	        PuzzleWarn.gameObject.SetActive(false);
	        PuzzleSlider.value = 1.0f;
	        PuzzleSliderTime.text = "120";
	    }
	
	    public IEnumerator ShowPuzzleFinish()
	    {
	        yield return new WaitForSeconds(1.5f);
	        PuzzlePlay.TargetTime = Game.Instance.ServerTime;
	        OnClosePuzzel();
	        var e = new SatueOperateEvent(11);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        PuzzleMove.OnPuzzleFinish += OnPuzzleFinish;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	}
}