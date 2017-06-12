using UnityEngine;
using System.Collections;
using EventSystem;
using ClientDataModel;

public class GXRankingLogic : MonoBehaviour {
    
    public BindDataRoot Binding;
    private IControllerBase mController;

    // Use this for initialization
    void Start () {
	
	}

    private void OnEnable()
    {
        mController = UIManager.Instance.GetController(UIConfig.GXRankingUI);
        if (mController == null)
        {
            return;
        }
        Binding.SetBindDataSource(mController.GetDataModel(""));
    }

    // Update is called once per frame
    void Update () {
	
	}


   public void CloseBtn()
    {
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.GXRankingUI));
    }

    //显示排行数据
    public void ShowItemInfo()
    {
        
    }


}
