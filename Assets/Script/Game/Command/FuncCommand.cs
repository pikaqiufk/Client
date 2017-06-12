#region using

using System;
using ObjCommand;

#endregion

namespace ObjCommand
{
	public class FuncCommand : BaseCommand
	{
	    public FuncCommand(Action func)
	    {
	        mFunc = func;
	    }
	
	    private readonly Action mFunc;
	
	    public override CommandResult Execute()
	    {
	        mFunc();
	        return CommandResult.Finished;
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