#region using

using System;
using System.Collections;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class RewardCardFrame : MonoBehaviour
	{
	    private Action endAnimation;
	    // Use this for initialization
	    public UISprite cardSprite;
	    private GameObject effect;
	    private bool animPlaying;
	
	    private void DestroyEffect()
	    {
	        animPlaying = false;
	        if (null != endAnimation)
	        {
	            endAnimation();
	        }
	        if (effect)
	        {
	            NGUITools.Destroy(effect);
	            effect = null;
	        }
	        cardSprite.fillAmount = 1;
	    }
	
	    private IEnumerator DoActive()
	    {
	        yield return new WaitForSeconds(0.8f);
	        DestroyEffect();
	        var ee = new UIEvent_HandBookFrame_ShowAnimationBlocker(false);
	        EventDispatcher.Instance.DispatchEvent(ee);
	    }
	
	    public void OnActiveClick()
	    {
	        if (animPlaying)
	        {
	            return;
	        }
	
	        ResourceManager.PrepareResource<GameObject>
	            ("Effect/UI/UI_TuJianJiHuo.prefab", res =>
	            {
	                if (gameObject)
	                {
	                    var height = cardSprite.height;
	                    effect = NGUITools.AddChild(gameObject, res);
	                    effect.transform.localPosition = new Vector3(0, -height/2, 0);
	                    iTween.MoveTo(effect,
	                        iTween.Hash("position", new Vector3(0, height/2 - 60, 0), "time", 0.8f, "islocal", true,
	                            "easetype",
	                            iTween.EaseType.linear));
	                    ResourceManager.Instance.StartCoroutine(DoActive());
	                    animPlaying = true;
	                    var listitem = gameObject.GetComponent<ListItemLogic>();
	                    var dataModel = listitem.Item as HandBookItemDataModel;
	                    endAnimation = () =>
	                    {
	                        var e = new UIEvent_HandBookFrame_OnBountyBookActive(dataModel);
	                        EventDispatcher.Instance.DispatchEvent(e);
	                    };
	                    var ee = new UIEvent_HandBookFrame_ShowAnimationBlocker(true);
	                    EventDispatcher.Instance.DispatchEvent(ee);
	                }
	            });
	    }
	
	    public void OnComposeClick()
	    {
	        var listitem = gameObject.GetComponent<ListItemLogic>();
	        if (listitem != null)
	        {
	            var e = new UIEvent_HandBookFrame_OnBookClick(listitem.Item as HandBookItemDataModel);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (!animPlaying)
	        {
	            return;
	        }
	
	        var height = cardSprite.height;
	        var h = effect.transform.localPosition.y + height/2 + 40;
	        var percent = 1 - h/height;
	        cardSprite.fillAmount = percent;
	
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