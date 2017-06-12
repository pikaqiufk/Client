namespace ObjCommand
{
	public class AttackCommand : BaseCommand
	{
	    public AttackCommand(ulong casterId, int skillId, ulong targetId)
	    {
	        mCasterId = casterId;
	        mTargetId = targetId;
	        mSkillId = skillId;
	    }
	
	    private readonly ulong mCasterId;
	    private readonly int mSkillId;
	    private readonly ulong mTargetId;
	
	    public override CommandResult Execute()
	    {
	        if (null != GameControl.Instance)
	        {
	            GameControl.Instance.UseSkill(mCasterId, mSkillId, mTargetId);
	        }
	
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