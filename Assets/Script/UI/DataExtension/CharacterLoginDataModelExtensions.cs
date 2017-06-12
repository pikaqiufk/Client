#region using

using ClientDataModel;

#endregion

public static class CharacterLoginDataModelExtensions
{
    public static void Clone(this CharacterLoginDataModel dataModel, CharacterLoginDataModel otherModel)
    {
        dataModel.CharacterId = otherModel.CharacterId;
        dataModel.Level = otherModel.Level;
        dataModel.Name = otherModel.Name;
        dataModel.RoleId = otherModel.RoleId;
        dataModel.showCreateButton = otherModel.showCreateButton;
        dataModel.Type = otherModel.Type;
    }
}