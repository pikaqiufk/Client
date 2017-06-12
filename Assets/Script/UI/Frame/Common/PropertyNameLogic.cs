#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class PropertyNameLogic : MonoBehaviour
	{
	    public UILabel Name;
	
	    public int AttributeId
	    {
	        set { Name.text = GameUtils.AttributeName(value); }
	    }
	}
}