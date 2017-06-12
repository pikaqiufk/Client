#region using

using System.Linq;
using ClientService;
using DataContract;
using ScorpionNetLib;
using UnityEngine;

#endregion

public partial class NetManager : ClientAgentBase, ILogin9xServiceInterface, ILogic9xServiceInterface,
                                  IScene9xServiceInterface, IRank9xServiceInterface, IActivity9xServiceInterface,
                                  IChat9xServiceInterface, ITeam9xServiceInterface
{
    public void SyncStopMove(SyncPostionMsg msg)
    {
        if (msg == null)
        {
            Logger.Log2Bugly(" SyncPostionMsg msg =null");
            return;
        }
        var character = ObjManager.Instance.FindCharacterById(msg.ObjId);
        if (null == character)
        {
            Logger.Warn("StopMove: obj[{0}]=null", msg.ObjId);
            return;
        }
        if (msg == null)
        {
            Logger.Log2Bugly(" SyncPostionMsg msg =null");
            return;
        }
        if (msg.Pos == null || msg.Pos.Pos == null)
        {
            Logger.Log2Bugly(" msg.Pos  ||  msg.Pos.Pos=null");
            return;
        }
        var des = GameLogic.GetTerrainPosition(GameUtils.DividePrecision(msg.Pos.Pos.x),
            GameUtils.DividePrecision(msg.Pos.Pos.y));
        var dir = new Vector3(GameUtils.DividePrecision(msg.Pos.Dir.x), 0, GameUtils.DividePrecision(msg.Pos.Dir.y));

        var diff = (des.xz() - character.Position.xz()).magnitude;

        if (character.GetObjType() == OBJ.TYPE.MYPLAYER)
        {
            if (diff > 5)
            {
                character.StopMove();
                character.Position = des;
                character.TargetDirection = dir;
            }
        }
        else
        {
            if (GameSetting.Instance == null)
            {
                Logger.Log2Bugly(" GameSetting.Instance =null");
                return;
            }
            var errorDis = character.GetObjType() == OBJ.TYPE.OTHERPLAYER
                ? GameSetting.Instance.OtherPlayerStopPosErrorDistance
                : GameSetting.Instance.NPCStopPosErrorDistance;

            if (character.GetObjType() == OBJ.TYPE.NPC && diff < 0.8f)
            {
                character.StopMove();
                character.TargetDirection = dir;
            }
            else if (diff < errorDis)
            {
                character.MoveTo(des);
            }
            else
            {
                character.StopMove();
                character.Position = des;
                character.TargetDirection = dir;
                Logger.Warn("Foce   StopMove:" + des.x + " " + des.z);
            }

            if (character.GetObjType() == OBJ.TYPE.NPC)
            {
                (character as ObjNPC).DelayedMove = null;
            }
        }

        //Logger.Info("SyncStopMove Obj[{0}] pos[{1},{2}]", msg.ObjId, des.x, des.z);
    }

    public void SyncDirection(ulong characterId, int dirX, int dirZ)
    {
        if (null == ObjManager.Instance || null == ObjManager.Instance.MyPlayer)
        {
            return;
        }

        if (ObjManager.Instance.MyPlayer.GetObjId() == characterId)
        {
            return;
        }
        var character = ObjManager.Instance.FindCharacterById(characterId);
        if (null == character)
        {
            Logger.Warn("SyncDirection: obj[{0}]=null", characterId);
            return;
        }

        character.TargetDirection = new Vector3(GameUtils.DividePrecision(dirX), 0, GameUtils.DividePrecision(dirZ));
    }

    public void SyncMoveTo(CharacterMoveMsg msg)
    {
        var character = ObjManager.Instance.FindCharacterById(msg.ObjId);
        if (null == character)
        {
            Logger.Warn("NotifyMoveTo: obj[{0}]=null", msg.ObjId);
            return;
        }
        if (msg.ObjId == PlayerDataManager.Instance.GetGuid())
        {
            return;
        }
        var v =
            msg.TargetPos.Select(
                item =>
                    new Vector3(GameUtils.DividePrecision(item.x), 0, GameUtils.DividePrecision(item.y))).ToList();
        character.MoveTo(v, 0);
        if (character.GetObjType() == OBJ.TYPE.NPC)
        {
            (character as ObjNPC).DelayedMove = null;
        }
    }

    public void SyncMoveToList(CharacterMoveMsgList msg)
    {
        {
            // foreach(var move in msg.Moves)
            var __enumerator1 = (msg.Moves).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var move = __enumerator1.Current;
                {
                    SyncMoveTo(move);
                }
            }
        }
    }

    public void DebugObjPosition(ulong characterId, PositionData pos)
    {
#if UNITY_EDITOR
        var character = ObjManager.Instance.FindCharacterById(characterId);
        if (null == character)
        {
            //Logger.Error("DebugObjPosition: obj[]=null", characterId);
            return;
        }
        if (null != GameLogic.Instance.Scene)
        {
            character.ServerRealPos = GameLogic.GetTerrainPosition(GameUtils.DividePrecision(pos.Pos.x),
                GameUtils.DividePrecision(pos.Pos.y));
            character.ServerRealDir = new Vector3(GameUtils.DividePrecision(pos.Dir.x), 0,
                GameUtils.DividePrecision(pos.Dir.y));
        }
#endif
    }

    public void SyncObjPosition(SyncPathPosMsg msg)
    {
        for (var i = 0; i < msg.Data.Count; ++i)
        {
            var data = msg.Data[i];

            var character = ObjManager.Instance.FindCharacterById(data.ObjId);
            if (null == character)
            {
                Logger.Warn("SyncObjPosition: obj[{0}]=null", data.ObjId);
                return;
            }

            character.SyncPathPosition(data);
        }
    }
}