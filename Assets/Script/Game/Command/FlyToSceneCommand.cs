#region using

using DataTable;

#endregion

namespace ObjCommand
{
	public class FlyToSceneCommand : BaseCommand
	{
	    public FlyToSceneCommand(int targetSceneId)
	    {
	        mTargetSceneId = targetSceneId;
	    }
	
	    private CommandResult mResult = CommandResult.Unfinished;
	    private readonly int mTargetSceneId;
	
	    public override CommandResult Execute()
	    {
	        return mResult;
	    }
	
	    public override void OnBegin()
	    {
	        var player = ObjManager.Instance.MyPlayer;
	
	        if (null == player)
	        {
	            return;
	        }
	
	        if (player.IsMoving())
	        {
	            player.StopMove();
	        }
	
	        var tbScene = Table.GetScene(mTargetSceneId);
	        GameUtils.FlyTo(mTargetSceneId, (float) tbScene.Entry_x, (float) tbScene.Entry_z,
	            i => mResult = i == (int) ErrorCodes.OK ? CommandResult.Finished : CommandResult.Interrupt);
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
	    }
	
	    public override void OnStop()
	    {
	        ObjManager.Instance.MyPlayer.StopMove();
	    }
	}
}