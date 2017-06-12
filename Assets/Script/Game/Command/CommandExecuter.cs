using ObjCommand;
using System;
#region using

using System.Collections.Generic;

#endregion

namespace ObjCommand
{
	public class CommandExecuter
	{
	    private readonly List<BaseCommand> mListCommand = new List<BaseCommand>();
	    public bool mPause;
	
	    public void ExeCommand(BaseCommand command)
	    {
	        Stop();
	        PushCommand(command);
	    }
	
	    public BaseCommand GetCurrentCommand()
	    {
	        if (mListCommand.Count > 0)
	        {
	            return mListCommand[0];
	        }
	        return null;
	    }
	
	    public void Pause()
	    {
	        mPause = true;
	        var command = GetCurrentCommand();
	        if (null != command && command.HasBegan)
	        {
	            command.OnPause();
	        }
	    }
	
	    public void PushCommand(BaseCommand command)
	    {
	        if (null == command)
	        {
	            return;
	        }
	        mListCommand.Add(command);
	    }
	
	    public void Resume()
	    {
	        mPause = false;
	        var command = GetCurrentCommand();
	        if (null != command && command.HasBegan)
	        {
	            command.OnResume();
	        }
	    }
	
	    public void Stop()
	    {
	        var command = GetCurrentCommand();
	        if (null != command && command.HasBegan)
	        {
	            command.OnStop();
	        }
	        mListCommand.Clear();
	    }
	
	    public void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mPause || mListCommand.Count <= 0)
	        {
	            return;
	        }
	        var command = GetCurrentCommand();
	        if (!command.HasBegan)
	        {
	            command.HasBegan = true;
	            command.OnBegin();
	        }
	
	        var ret = command.Execute();
	        if (BaseCommand.CommandResult.Finished == ret)
	        {
	            mListCommand.RemoveAt(0);
	            command.OnFinish();
	        }
	        else if (BaseCommand.CommandResult.Interrupt == ret)
	        {
	            mListCommand.Clear();
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	}
}