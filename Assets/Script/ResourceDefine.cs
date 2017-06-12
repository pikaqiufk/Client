public static class Resource
{
    public static string GetModelPath(string modelName)
    {
        return Dir.Model + modelName + FileExt.Prefab;
    }

    public static string GetScenePath(string sceneName)
    {
        return Dir.Scene + sceneName + FileExt.Scene;
    }

    public static string GetTerrainPath(string file)
    {
        return Dir.SceneTerrainHeight + file + FileExt.Terrain;
    }

    public static class FileExt
    {
        public const string Animation = ".anim";
        public const string Prefab = ".prefab";
        public const string Scene = ".unity";
        public const string Terrain = ".bytes";
    }

    public static class PrefabPath
    {
        public const string Actor = "Prefab/CGModel.prefab";
		public const string BlobShadowCaster = "Prefab/Shadow/BlobCaster.prefab";
		public const string BlobShadowReceiver = "Prefab/Shadow/BlobReceiver.prefab";
		public const string DynamicShadowCaster = "Prefab/Shadow/DynamicCaster.prefab";
		public const string DynamicShadowReceiver = "Prefab/Shadow/DynamicReceiver.prefab";

		public const string DropItem = "Prefab/Loot.prefab";
        //shadow
		public const string Elf = "Prefab/Fairy.prefab";
		public const string FakeCharacter = "Prefab/UIModel.prefab";
        public const string MovingCircle = "UI/MovingCircle.prefab";
        //controller
		public const string MyPlayer = "Prefab/MyCharacter.prefab";
		public const string NPC = "Prefab/NpcCharacter.prefab";
		public const string OtherPlayer = "Prefab/OtherCharacter.prefab";
		public const string Retinue = "Prefab/SummonCreature.prefab";
        //name board
        public const string NameBoard = "UI/NameBoard.prefab";
		public const string NPCNameBoard = "UI/NpcNameBoard.prefab";

        public const string SelectReminder = "Model/SelectReminder.prefab";
        //skill indicator
        public const string SkillIndicatorCaster = "Prefab/SkillIndicator/SkillIndicatorCaster.prefab";
        public const string SkillIndicatorReceiver = "Prefab/SkillIndicator/SkillIndicatorReceiver.prefab";
        //res
        public const string Transfer = "Model/Transfer.prefab";
    }

    public static class Material
    {
        public const string BlobShadow = "Material/BlobShadow.mat";
        public const string BloomMaterial = "Material/Bloom.mat";
        public const string BlurMaterial = "Material/Blur.mat";
        public const string MainPlayerMaterial = "Material/CharacterThrough.mat";
    }

    public static class Dir
    {
        public const string Animation = "Animation/";
        public const string Controller = "Prefab/";
        public const string Effect = "Effect/";
        public const string Material = "Material/";
        public const string Model = "Model/";
        public const string Scene = "Scene/";
        public const string SceneSearchTree = "SceneSearchTree/";
        public const string SceneTerrainHeight = "SceneTerrainHeight/";
        public const string Sound = "Sound/";
        public const string UI = "UI/";
    }
}