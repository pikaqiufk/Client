#region using
using System;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class QuickBuyFrame : MonoBehaviour
	{
        public BindDataRoot Binding;

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
            var controllerBase = UIManager.Instance.GetController(UIConfig.QuickBuyUi);
            if (controllerBase == null)
            {
                return;
            }

            Binding.SetBindDataSource(controllerBase.GetDataModel(""));

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

        public void OnClick_Reduce()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(0));
        }
        public void OnClick_ReducePress()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(1));

        }
        public void OnClick_ReduceUnPress()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(2));
        }

        public void OnClick_Add()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(3));
        }
        public void OnClick_AddPress()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(4));
        }
        public void OnClick_AddUnPress()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(5));
        }

        public void OnClick_Close()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.QuickBuyUi));
        }

        public void OnClick_OneBuy()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(11));            
        }
        public void OnClick_MultyBuy()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(12));
        }
        public void OnClick_GiftBuy()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(13));
        }
        public void OnClick_GotoShop()
        {
            EventDispatcher.Instance.DispatchEvent(new QuickBuyOperaEvent(21));
        }
    }
}