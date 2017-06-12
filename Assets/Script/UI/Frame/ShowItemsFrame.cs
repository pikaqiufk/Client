
#region using
using System;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class ShowItemsFrame : MonoBehaviour
	{
        public BindDataRoot Binding;
        public GameObject Effect;

        public void ShowEffect(IEvent ievent)
        {
            if (Effect != null)
            {
                Effect.SetActive(false);
                Effect.SetActive(true);
            }
        }

        void Start()
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

        private void OnEnable()
        {
#if !UNITY_EDITOR
	        try
	        {
#endif
            var controllerBase = UIManager.Instance.GetController(UIConfig.ShowItemsFrame);
            if (controllerBase == null)
            {
                return;
            }

            Binding.SetBindDataSource(controllerBase.GetDataModel(""));


            EventDispatcher.Instance.AddEventListener(ShowItemsFrameEffectEvent.EVENT_TYPE, ShowEffect);
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
            EventDispatcher.Instance.RemoveEventListener(ShowItemsFrameEffectEvent.EVENT_TYPE, ShowEffect);
#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
#endif
        }

        // Update is called once per frame
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

        private void OnDestroy()
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

        public void OnClick_Close()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ShowItemsFrame));
        }
	}
}