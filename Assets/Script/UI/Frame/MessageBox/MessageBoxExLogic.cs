using System;
using EventSystem;
using UnityEngine;


public class MessageBoxExLogic : MonoBehaviour
{
	private BindDataRoot Binding;
	void Start ()
	{
	    Binding = gameObject.GetComponent<BindDataRoot>();
        if (Binding == null)
        {
            return;
        }
		var controllerBase = UIManager.Instance.GetController(UIConfig.MessageBoxEx);
        if (controllerBase == null) return;
        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	}

	// Update is called once per frame
	void Update () {
	
	}

    public void OnClickExitDungeon()
    {
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBoxEx));

        var e = new ExitFuBenWithOutMessageBoxEvent();
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnClickBuyTili()
    {
        EventDispatcher.Instance.DispatchEvent(new OnClickBuyTiliEvent(1));
    }
}