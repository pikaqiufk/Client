using UnityEngine;
using System.Collections;
using EventSystem;
public class MieShiTapLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnCloseBtn()
    {
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MieShiTapUI));
    }


}
