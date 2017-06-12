#region using

using DataTable;
using UnityEngine;

#endregion

namespace GameUI
{
	public class AnimSpriteLogic : MonoBehaviour
	{
	    public UISpriteAnimation Animation;
	    private int sequenceFrameId;
	    public UISprite Sprite;
	
	    public int SeqFrameId
	    {
	        get { return sequenceFrameId; }
	        set
	        {
	            sequenceFrameId = value;
	            RefreshAnimat();
	        }
	    }
	
	    private void RefreshAnimat()
	    {
	        var tbSeqFrame = Table.GetSeqFrame(SeqFrameId);
	        if (tbSeqFrame == null)
	        {
	            return;
	        }
	        if (Sprite != null && Sprite.atlas != null)
	        {
	            var atlasName = tbSeqFrame.AtlasName;
	            var stringname = Sprite.atlas.name;
	
	            if (!stringname.Contains(atlasName))
	            {
	                ResourceManager.PrepareResource<GameObject>("UI/Atlas/" + atlasName + ".prefab",
	                    res => { Sprite.atlas = res.GetComponent<UIAtlas>(); });
	            }
	        }
	
	        Animation.namePrefix = tbSeqFrame.PicName;
	        Animation.framesPerSecond = tbSeqFrame.Frame;
	    }
	}
}