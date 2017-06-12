#region using

using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EstablishRoleDrag : MonoBehaviour
	{
		
	    public bool IsEquipWeapon = false;
	    public EstablishRole Logic;
	    private int mAnimationIndex;
	    public int CharacterTypeId { get; set; }
	    // Use this for initialization
	
	    public ObjFakeCharacter Model { get; set; }
	
	    public void CreateFrameClick()
	    {
	        if (null != Logic)
	        {
				Logic.PlayAction();
	        }
	    }
	
	    public void SelectFrameClick()
	    {
	        if (!IsEquipWeapon)
	        {
	            return;
	        }
	
	        var actorTable = Table.GetActor(CharacterTypeId);
	        if (null != Model)
	        {
	            Model.PlayAnimation(actorTable.Action[mAnimationIndex++%actorTable.Action.Length],
	                strAni => { Model.PlayAnimation(OBJ.CHARACTER_ANI.STAND); });
	        }
	    }

		private void OnDrag(Vector2 delta)
		{
			EventDispatcher.Instance.DispatchEvent(new CreateRole_DragEvent(delta));
		}

	}
}