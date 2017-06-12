namespace ObjCommand
{
	public class BaseCommand
	{
	    public bool HasBegan = false;
	
	    public enum CommandResult
	    {
	        Unfinished,
	        Finished,
	        Interrupt
	    }
	
	    public virtual CommandResult Execute()
	    {
	        return CommandResult.Finished;
	    }
	
	    public virtual void OnBegin()
	    {
	    }
	
	    public virtual void OnFinish()
	    {
	    }
	
	    public virtual void OnPause()
	    {
	    }
	
	    public virtual void OnResume()
	    {
	    }
	
	    public virtual void OnStop()
	    {
	    }
	}
}