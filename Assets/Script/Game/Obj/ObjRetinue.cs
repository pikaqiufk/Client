#region using

using System;

#endregion

public class ObjRetinue : ObjNPC
{
    //拥有者id
    public ulong OwnerId;

    public bool GetIsMe()
    {
        return OwnerId == PlayerDataManager.Instance.GetGuid();
    }

    public override OBJ.TYPE GetObjType()
    {
        return OBJ.TYPE.RETINUE;
    }

    public override bool Init(InitBaseData initData, Action callback = null)
    {
        if (!base.Init(initData))
        {
            return false;
        }
        var retinueData = initData as InitRetinueData;
        if (retinueData == null)
        {
            return false;
        }
        OwnerId = retinueData.Owner;
        return true;
    }

    protected override void InitNavMeshAgent()
    {
        if (null == TableCharacter)
        {
            return;
        }
        if (null == TableNPC)
        {
        }
    }
}