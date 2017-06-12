#region using

using UnityEngine;

#endregion

namespace ObjCommand
{
	public class MoveCommand : BaseCommand
	{
	    public MoveCommand(ObjCharacter character, Vector3 targetPos, float offset = 0.2f)
	    {
	        mCharacter = character;
	        mTargetPos = targetPos;
	        mDistanceSqr = offset*offset;
	    }
	
	    private readonly ObjCharacter mCharacter;
	    private readonly float mDistanceSqr;
	    private Vector3 mLastPosition;
	    private float mStayTime;
	    private readonly Vector3 mTargetPos;
	
	    public override CommandResult Execute()
	    {
	        if (null == mCharacter)
	        {
	            Logger.Warn("Character is null here.");
	            return CommandResult.Interrupt;
	        }
	
	        var diff = mTargetPos - mCharacter.Position;
	        diff.y = 0;
	        if (diff.sqrMagnitude <= mDistanceSqr)
	        {
	            return CommandResult.Finished;
	        }
	
	        if (!mCharacter.IsMoving())
	        {
	            mCharacter.MoveTo(mTargetPos);
	        }
	
	        if (mLastPosition != mCharacter.Position)
	        {
	            mLastPosition = mCharacter.Position;
	        }
	        else
	        {
	            mStayTime += Time.deltaTime;
	
	            if (mStayTime > 2.0f)
	            {
	                return CommandResult.Interrupt;
	            }
	        }
	
	        return CommandResult.Unfinished;
	    }
	
	    public override void OnBegin()
	    {
	        if (null == mCharacter)
	        {
	            return;
	        }

            if (mCharacter.GetObjType() == OBJ.TYPE.MYPLAYER)
	        {
                var myPlayer = mCharacter as ObjMyPlayer;
	            if (myPlayer != null)
	            {
                    myPlayer.fastReachSceneID = GameLogic.Instance.Scene.SceneTypeId;
                    myPlayer.fastReachPos = mTargetPos; 
	            }
	        }
            
	        if (mCharacter.IsMoving())
	        {
	            mCharacter.StopMove();
	        }
	        var diff = mTargetPos - mCharacter.Position;
	        diff.y = 0;
	        if (diff.sqrMagnitude <= mDistanceSqr)
	        {
	            return;
	        }
	        if (!mCharacter.MoveTo(mTargetPos, 0.05f, true))
	        {
	            //Logger.Error("Can't reach Pos{0}", mTargetPos);
	        }
	    }
	
	    public override void OnFinish()
	    {
	        mCharacter.StopMove();
	    }
	
	    public override void OnPause()
	    {
	        mCharacter.StopMove();
	    }
	
	    public override void OnResume()
	    {
	        mCharacter.MoveTo(mTargetPos);
	    }
	
	    public override void OnStop()
	    {
	        mCharacter.StopMove();
	    }
	}
}