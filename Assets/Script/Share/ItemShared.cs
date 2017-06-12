#region using

using DataTable;

#endregion

public static class CheckGeneral
{
    /// <summary>
    ///     判断物品类型
    /// </summary>
    /// <param name="nId">物品ID</param>
    /// <param name="it"></param>
    /// <returns></returns>
    public static bool CheckItemType(int nId, eItemType it)
    {
        if (GetItemType(nId) == it)
        {
            return true;
        }
        return false;
    }

    public static EquipBaseRecord GetEquip(int ItemId)
    {
        var tbItem = Table.GetItemBase(ItemId);
        if (tbItem == null)
        {
            return null;
        }
        var tbEquip = Table.GetEquipBase(tbItem.Exdata[0]);
        return tbEquip;
    }

    /// <summary>
    ///     获得物品类型
    /// </summary>
    /// <param name="nId"></param>
    /// <returns></returns>
    public static eItemType GetItemType(int nId)
    {
        if (nId >= 0 && nId < 10000)
        {
            return eItemType.Resources;
        }
        if (nId >= 20000 && nId < 30000)
        {
            return eItemType.BaseItem;
        }
        if (nId >= 90000 && nId < 100000)
        {
            return eItemType.BaseItem;
        }
        if (nId >= 30000 && nId < 40000)
        {
            return eItemType.Piece;
        }
        if (nId >= 40000 && nId < 50000)
        {
            return eItemType.Mission;
        }
        //if (nId >= 100000) 
        return eItemType.Equip;
        //return eItemType.Error;
    }
}