
namespace GameUI
{
	public class NpcOverheadFrame : OverheadFrame
	{
	    public UISprite Finish;
	    private MissionState mState;
	    public UISprite Quest;
	    public UISprite Unfinish;
	
	    public enum MissionState
	    {
	        None = 0,
	        Quest,
	        Unfinish,
	        Finish
	    }
	
	    public MissionState State
	    {
	        set
	        {
	            mState = value;
	            Quest.gameObject.SetActive(false);
	            Unfinish.gameObject.SetActive(false);
	            Finish.gameObject.SetActive(false);
	            if (MissionState.Quest == mState)
	            {
	                Quest.gameObject.SetActive(true);
	            }
	            else if (MissionState.Unfinish == mState)
	            {
	                Unfinish.gameObject.SetActive(true);
	            }
	            else if (MissionState.Finish == mState)
	            {
	                Finish.gameObject.SetActive(true);
	            }
	        }
	    }
	}
}