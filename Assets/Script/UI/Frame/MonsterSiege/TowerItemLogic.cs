using UnityEngine;
using System.Collections;
using ClientDataModel;

public class TowerItemLogic : MonoBehaviour {

    private int index;
    public int TowerIndex
    {
        set {
            
            index = value;
            IControllerBase mController = UIManager.Instance.GetController(UIConfig.MonsterSiegeUI);
            if (mController == null)
            {
                return;
            }
            MonsterDataModel DataModel = mController.GetDataModel("") as MonsterDataModel;
            BindDataRoot bdr = GetComponent<BindDataRoot>();
			if(index <DataModel.MonsterTowers.Count)
			{
				bdr.SetBindDataSource(DataModel.MonsterTowers[index]);
			}
            
        }
    }
}
