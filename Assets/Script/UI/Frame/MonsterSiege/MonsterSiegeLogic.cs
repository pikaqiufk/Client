using ClientDataModel;
using UnityEngine;
using DataTable;
using EventSystem;

public class MonsterSiegeLogic : MonoBehaviour
{

    void OnEnable()
    {
        IControllerBase mController = UIManager.Instance.GetController(UIConfig.MonsterSiegeUI);
        //ActivityDataModel dataModule = (mController.GetDataModel(string.Empty)) as ActivityDataModel;
        MonsterDataModel MonsterModel = mController.GetDataModel("MonsterModel") as MonsterDataModel;
        if (mController != null)
        {
           // mController.DataModule.CurMonsterFuben.activity.ActivityTime;
            /**BindDataRoot bdr = GetComponent<BindDataRoot>();
            if (bdr != null)
            {
                if (MonsterModel.CurMonsterFuben.activity == null)
                {
                    MonsterModel.CurMonsterFuben.activity = new MonsterDataModel();
                    MonsterModel.CurMonsterFuben.MomsterTower=new MonsterTowerDataModel();
                }
                bdr.SetBindDataSource(MonsterModel.CurMonsterFuben.activity);
                bdr.SetBindDataSource(MonsterModel.CurMonsterFuben.MomsterTower);
            }**/
        }

    }

}
