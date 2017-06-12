#region using

using ClientDataModel;

#endregion

public static class HandBookDataModelExtensions
{
    public static void Copy(this HandBookItemDataModel dataModel, HandBookItemDataModel other)
    {
        dataModel.BookId = other.BookId;
        dataModel.BookSortId = other.BookSortId;
        dataModel.ItemId = other.ItemId;
        dataModel.BookCount = other.BookCount;
        dataModel.BookMaxCapacity = other.BookMaxCapacity;
        dataModel.BookPieceBagIndex = other.BookPieceBagIndex;
        dataModel.BookPieceCount = other.BookPieceCount;
        dataModel.BookUpgradeRequestCast = other.BookUpgradeRequestCast;
        dataModel.BountyActive = other.BountyActive;
        dataModel.BountyBookAttr = other.BountyBookAttr;
        dataModel.BountyMoney = other.BountyMoney;
        dataModel.MonsterLevel = other.MonsterLevel;
        dataModel.MonsterName = other.MonsterName;
        dataModel.UpGradeRequestBookId = other.UpGradeRequestBookId;
        dataModel.UpGradeRequestCount = other.UpGradeRequestCount;
        dataModel.Composeable = other.Composeable;
        dataModel.ComposeButtonShow = other.ComposeButtonShow;
        dataModel.BookComposeTableId = other.BookComposeTableId;
        dataModel.locationName = other.locationName;
        for (var i = 0; i < dataModel.TrackParam.Count; i++)
        {
            dataModel.TrackParam[i] = other.TrackParam[i];
        }
        dataModel.TrackType = other.TrackType;
    }
}