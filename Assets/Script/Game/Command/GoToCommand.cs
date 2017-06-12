#region using

using DataTable;
using UnityEngine;

#endregion

namespace ObjCommand
{
	public class GoToCommand : BaseCommand
	{
	    public GoToCommand(int targetSceneId, Vector3 targetPos, float offset = 0.01f)
	    {
	        mTargetSceneId = targetSceneId;
	        mTargetPos = targetPos;
	        mDistance = offset;
	    }
	
	    private readonly float mDistance;
	    private Vector3 mTargetPos;
	    private readonly int mTargetSceneId;
	
	    public override CommandResult Execute()
	    {
	        if (null == GameLogic.Instance || null == GameLogic.Instance.Scene)
	        {
	            return CommandResult.Interrupt;
	        }
	        if (null == ObjManager.Instance.MyPlayer)
	        {
	            return CommandResult.Unfinished;
	        }
	
	        if (GameLogic.Instance.Scene.SceneTypeId == mTargetSceneId)
	        {
	            var diff = mTargetPos - ObjManager.Instance.MyPlayer.Position;
	            diff.y = 0;
	            if (diff.magnitude <= mDistance)
	            {
	                return CommandResult.Finished;
	            }
	        }
	        return CommandResult.Unfinished;
	    }
	
	    public override void OnBegin()
	    {
	        var scene = GameLogic.Instance.Scene;
	        var sceneId = scene.SceneTypeId;
	        var player = ObjManager.Instance.MyPlayer;
	
	        if (null == player)
	        {
	            return;
	        }

            player.fastReachSceneID = mTargetSceneId;
            player.fastReachPos = mTargetPos;

	        if (player.IsMoving())
	        {
	            player.StopMove();
	        }
	
	
	        if (!player.CanMove())
	        {
	            return;
	        }
	
	        if (sceneId == mTargetSceneId)
	        {
	            mTargetPos.y = GameLogic.GetTerrainHeight(mTargetPos);
	            if (!player.MoveTo(mTargetPos))
	            {
	                Logger.Info("Can't reach Pos{0}", mTargetPos);
	            }
	        }
	        else
	        {
	            var coordinateList = Dijkstra.FindWay(sceneId, player.Position.x, player.Position.z, mTargetSceneId,
	                mTargetPos.x, mTargetPos.y);
	            var i = 0;
	            while (i < coordinateList.Count)
	            {
	                coordinateList.RemoveAt(i++);
	            }
	            if (coordinateList.Count <= 0)
	            {
	                Logger.Warn("Can not go to {{0},({1},{2}})", mTargetSceneId, mTargetPos.x, mTargetPos.z);
	                return;
	            }
	
	            var coordinate = coordinateList[0];
	            var des = new Vector3(coordinate.PosX, 0, coordinate.PosY);
	            if (!player.MoveTo(des, 0.05f, true))
	            {
	                Logger.Info("Can't reach Pos{0}", des);
	            }
	        }
	    }
	
	    public override void OnFinish()
	    {
	        ObjManager.Instance.MyPlayer.StopMove();
	    }
	
	    public override void OnPause()
	    {
	        ObjManager.Instance.MyPlayer.StopMove();
	    }
	
	    public override void OnResume()
	    {
	        ObjManager.Instance.MyPlayer.MoveTo(mTargetPos);
	    }
	
	    public override void OnStop()
	    {
	        ObjManager.Instance.MyPlayer.StopMove();
	    }
	}
}