using System;
#region using

using System.Collections;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SynthesisFrame : MonoBehaviour
	{
	    public UVColorAnimation Anim;
	    public Animation AnimFail;
	    public Animation AnimSuccess;
	    public BindDataRoot Binding;
	    public GameObject FlyObj;
	    public GameObject GuangHuan;
	    public GameObject HomeObj;
	    public List<Transform> MaterialList;
	    private GameObject flyPrefab;
	    private bool lockAnimation;
	    public List<ParticleSystem> Particles;
	
	    private IEnumerator LaterAnimation(float delay)
	    {
	        yield return new WaitForSeconds(delay);
	
	        foreach (var p in Particles)
	        {
	            p.gameObject.SetActive(false);
	        }
	        lockAnimation = false;
	    }
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        EventDispatcher.Instance.AddEventListener(ComposeItemEffectEvent.EVENT_TYPE, OnSynthesisItemEffect);
	        EventDispatcher.Instance.AddEventListener(UIEvent_ComposeFlyAnim.EVENT_TYPE, OnSynthesisFlyAnim);
	
	        flyPrefab = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFly.prefab");
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnClickBtnBack()
	    {
			var e = new Close_UI_Event(UIConfig.ComposeUI);
			EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnClose()
	    {
	        var e = new Close_UI_Event(UIConfig.ComposeUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickComposeItem()
	    {
	        if (lockAnimation)
	        {
	            return;
	        }
	        EventDispatcher.Instance.DispatchEvent(new ComposeItemOnClick());
	    }

        public void OnClickOpenZhuZao()
        {
            var ee = new Show_UI_Event(UIConfig.SmithyUI, new SmithyFrameArguments
            {
                BuildingData = null
            });
            EventDispatcher.Instance.DispatchEvent(ee);
        }
	
	    private void OnSynthesisFlyAnim(IEvent ievent)
	    {
	        var e = ievent as UIEvent_ComposeFlyAnim;
	        var obj = Instantiate(flyPrefab) as GameObject;
	        PlayerDataManager.Instance.PlayFlyItem(obj, FlyObj.transform, HomeObj.transform, 12, e.Exp);
	    }
	
	    private void OnSynthesisItemEffect(IEvent ievent)
	    {
	        var e = ievent as ComposeItemEffectEvent;
	
	        Animation anim;
	        if (e.IsSuccess)
	        {
	            anim = AnimSuccess;
	            foreach (var p in Particles)
	            {
	                p.gameObject.SetActive(true);
	                p.Simulate(0, true, true);
	                p.Play();
	            }
	            Anim.gameObject.SetActive(true);
	            Anim.enabled = true;
	            GuangHuan.SetActive(true);
	            var tweens = GuangHuan.GetComponents<UITweener>();
	            foreach (var tween in tweens)
	            {
	                tween.ResetForPlay();
	                tween.PlayForward();
	            }
	            StartCoroutine(LaterAnimation(1.1f));
	        }
	        else
	        {
	            anim = AnimFail;
	        }
	
	        anim.gameObject.SetActive(true);
	        anim.Sample();
	        anim.Play(PlayMode.StopAll);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(ComposeItemEffectEvent.EVENT_TYPE, OnSynthesisItemEffect);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_ComposeFlyAnim.EVENT_TYPE, OnSynthesisFlyAnim);
	
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
	
	        AnimSuccess.gameObject.SetActive(false);
	        AnimFail.gameObject.SetActive(false);
	        foreach (var p in Particles)
	        {
	            p.gameObject.SetActive(false);
	        }
	        GuangHuan.SetActive(false);
	        Anim.gameObject.SetActive(false);
	        Anim.enabled = false;
	        lockAnimation = false;
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
	
	        IControllerBase controllerBase = null;
	        controllerBase = UIManager.Instance.GetController(UIConfig.ComposeUI);
	
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
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
	}
}