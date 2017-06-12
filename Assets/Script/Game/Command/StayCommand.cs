#region using

using UnityEngine;

#endregion

namespace ObjCommand
{
	public class StayCommand : BaseCommand
	{
	    public StayCommand(float duration)
	    {
	        mTimeOver = Time.time + duration;
	    }
	
	    private ObjCharacter mCharacter;
	    private float mDistance;
	    private readonly float mTimeOver;
	
	    public override CommandResult Execute()
	    {
	        return Time.time >= mTimeOver ? CommandResult.Finished : CommandResult.Unfinished;
	    }
	
	    public override void OnBegin()
	    {
	    }
	
	    public override void OnFinish()
	    {
	    }
	
	    public override void OnPause()
	    {
	    }
	
	    public override void OnResume()
	    {
	    }
	}
}