public class TypeDefine
{
    public const int INVALID_ID = -1;
    public const ulong INVALID_ULONG = ULONG_MAX;
    public const ulong ULONG_MAX = 0xFFFFFFFFFFFFFFFF;
}

public static class GameObjName
{
    public const string DropItemRoot = "DropItemRoot";
    public const string EffectRoot = "EffectRoot";
    public const string GlobalSkillIndicatorRoot = "GlobalSkillIndicatorRoot";
    public const string NpcRootName = "NpcRoot";
    public const string OtherPlayerRootName = "OtherPlayerRoot";
    public const string ShadowRootName = "ShadowRoot";
    public const string TransferRootName = "TransferRoot";
}

public static class SpecialCode
{
    public const string ChatBegin = "{!";
    public const string ChatEnd = "!}";
}

public static class OBJ
{
    private const string State = "State";

    public enum CharacterAnimationState
    {
        Normal,
        Fly,
        Attack
    }

    public enum TYPE
    {
        INVALID = ObjType.INVALID,
        OTHERPLAYER = ObjType.PLAYER,
        NPC = ObjType.NPC,
        RETINUE = ObjType.RETINUE,
        DROPITEM = ObjType.DROPITEM,
        AUTOPLAYER = ObjType.AUTOPLAYER,

        MYPLAYER = 8888,
        FAKE_CHARACTER,
        ACTOR,
        ELF
    }

    //角色状态
    public static class STATEMACHINE_EVENT
    {
        public const string ATTACK = "Attack";
        public const string BORN = "Born";
        public const string DIE = "Die";
        public const string DIZZY = "Dizzy";
        public const string HURT = "Hurt";
        public const string IDLE = "Idle";
        public const string RUN = "Run";
    }

    //状态机的状态名字
    public static class CHARACTER_STATE
    {
        public const string ATTACK = STATEMACHINE_EVENT.ATTACK + State;
        public const string BORN = STATEMACHINE_EVENT.BORN + State;
        public const string DIE = STATEMACHINE_EVENT.DIE + State;
        public const string DIZZY = STATEMACHINE_EVENT.DIZZY + State;
        public const string EMPTY = "Empty" + State;
        public const string HURT = STATEMACHINE_EVENT.HURT + State;
        public const string IDLE = STATEMACHINE_EVENT.IDLE + State;
        public const string RUN = STATEMACHINE_EVENT.RUN + State;
    }

    //角色动作id(关联到Animation.txt)
    public static class CHARACTER_ANI
    {
        public const int ATTACK = 0;
        public const int AttackIdle = 14;
        public const int AttackMove = 15;
        public const int BORN = 7;
        public const int CAIJI = 11;
        public const int CaiJi02 = 17;
        public const int DEAD = 1;
        public const int DIE = 2;
        public const int DIZZY = 3;
        public const int FlyDizzy = 18;
        public const int FLYHIT = 8;
        public const int FlyIdle = 12;
        public const int FlyMove = 13;
        public const int HIT = 4;
        public const int RIDE = 20;
        public const int RUN = 5;
        public const int STAND = 6;
        public const int Stand01 = 16;
        public const int Walk = 10;
    }
}

public static class GAMELAYER
{
    public static string CG = "CG";
    public static string CGMainPlayer = "CGMainPlayer";
    public static string Collider = "Collider";
    public static string Hide = "Hide";
    public static string IgnoreShadow = "IgnoreShadow";
    public static string MainPlayer = "MainPlayer";
    public static string ObjLogic = "ObjLogic";
    public static string OhterPlayer = "OtherPlayer";
    public static string ShadowReceiver = "ShadowReceiver";
    public static string Terrain = "Terrain";
    public static string UI = "UI";
    public static string PerspectiveView = "PerspectiveView";
}