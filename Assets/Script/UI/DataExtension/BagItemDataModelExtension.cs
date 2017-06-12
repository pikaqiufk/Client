#region using

using System.Collections.Generic;
using ClientDataModel;

#endregion

public static class BagItemDataModelExtension
{
    public static void Clone(this BagItemDataModel dataModel, BagItemDataModel otherModel)
    {
        dataModel.ItemId = otherModel.ItemId;
        dataModel.Count = otherModel.Count;
        dataModel.BagId = otherModel.BagId;
        dataModel.Index = otherModel.Index;
        dataModel.IsGrey = otherModel.IsGrey;
        dataModel.Exdata.InstallData(otherModel.Exdata);
        dataModel.itemWithSkill = otherModel.itemWithSkill;
    }

    public static void Reset(this BagItemDataModel dataModel)
    {
        dataModel.ItemId = -1;
        dataModel.Count = 0;
        dataModel.BagId = 0;
        dataModel.Index = -1;
        dataModel.IsGrey = false;
        if (dataModel.Exdata != null)
        {
            dataModel.Exdata.Clear();
        }
    }
}

public class BagItemDataModelComParer : IEqualityComparer<BagItemDataModel>
{
    public bool Equals(BagItemDataModel x, BagItemDataModel y)
    {
        return x.ItemId == y.ItemId && x.BagId == y.BagId && x.Index == y.Index;
    }

    public int GetHashCode(BagItemDataModel obj)
    {
        return 0;
    }
}