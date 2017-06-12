#region using

using ClientDataModel;

#endregion

public static class TradingItemExtension
{
    public static void Clone(this TradingItemDataModel dataModel, TradingItemDataModel otherModel)
    {
        dataModel.BagItem.Clone(otherModel.BagItem);
        dataModel.NeedLevel = otherModel.NeedLevel;
        dataModel.MaxSellCount = otherModel.MaxSellCount;
        dataModel.SliderCanMove = otherModel.SliderCanMove;
        dataModel.MinSinglePrice = otherModel.MinSinglePrice;
        dataModel.MaxSinglePrice = otherModel.MaxSinglePrice;
        dataModel.SellCount = otherModel.SellCount;
        dataModel.SellPrice = otherModel.SellPrice;
        dataModel.IsPeddling = otherModel.IsPeddling;
        dataModel.PeddleTime = otherModel.PeddleTime;
        dataModel.PeddleDateTime = otherModel.PeddleDateTime;
        dataModel.SliderRate = otherModel.SliderRate;
        dataModel.TradingItemId = otherModel.TradingItemId;
        dataModel.SellType = otherModel.SellType;
        dataModel.State = otherModel.State;
    }

    public static void Clone(this ExchangeItemDataModel dataModel, ExchangeItemDataModel otherModel)
    {
        dataModel.BagItem.Clone(otherModel.BagItem);
        dataModel.ExchangeId = otherModel.ExchangeId;
        dataModel.Price = otherModel.Price;
        dataModel.SellCount = otherModel.SellCount;
        dataModel.SellPrice = otherModel.SellPrice;
        dataModel.SliderRate = otherModel.SliderRate;

        dataModel.SellGroupCount = otherModel.SellGroupCount;
        dataModel.SellGroupRate = otherModel.SellGroupRate;
        dataModel.SellGroupCountMax = otherModel.SellGroupCountMax;
    }
}