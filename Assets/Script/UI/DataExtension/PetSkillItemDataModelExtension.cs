#region using

using ClientDataModel;

#endregion

public static class PetSkillItemExtensions
{
    public static void Clone(this PetSkillItemDataModel dataModel, PetSkillItemDataModel other)
    {
        dataModel.SkillId = other.SkillId;
        dataModel.Active = other.Active;
        dataModel.Col = other.Col;
        dataModel.Name = other.Name;
        dataModel.Desc = other.Desc;
        dataModel.BuffId = other.BuffId;
        dataModel.BuffIconId = other.BuffIconId;
    }
}