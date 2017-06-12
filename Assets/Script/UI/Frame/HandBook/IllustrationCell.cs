#region using

using System.Collections;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class IllustrationCell : MonoBehaviour
	{
	    private bool isInEffect;
	    public GameObject EffectRoot;
	    public ListItemLogic ItemLogic;
	
	    private void ClearEffect()
	    {
	        if (EffectRoot)
	        {
	            var child = EffectRoot.transform.FindChild("UI_TuJianHeCheng(Clone)");
	            while (child != null)
	            {
	                NGUITools.Destroy(child);
	                child = EffectRoot.transform.FindChild("UI_TuJianHeCheng(Clone)");
	            }
	        }
	    }
	
	    private IEnumerator DoCompose()
	    {
	        yield return new WaitForSeconds(0.6f);
	        ClearEffect();
	
	        var scrollView = transform.parent.parent.GetComponent<UIScrollViewSimple>();
	        var lastPos = scrollView.transform.localPosition;
	        var lastOffset = scrollView.oldoffset;
	        var e1 = new UIEvent_HandBookFrame_SetScrollViewLastPostion(lastPos, lastOffset);
	        EventDispatcher.Instance.DispatchEvent(e1);
	
	
	        var e = new UIEvent_HandBookFrame_ComposeBookPiece(ItemLogic.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
	
	        isInEffect = false;
	        var ee = new UIEvent_HandBookFrame_ShowAnimationBlocker(false);
	        EventDispatcher.Instance.DispatchEvent(ee);
	    }
	
	    public void OnCardClick()
	    {
	        var e = new UIEvent_HandBookFrame_OnBookClick(ItemLogic.Item as HandBookItemDataModel);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnComposeButtonClick()
	    {
	        if (isInEffect)
	        {
	            return;
	        }
	
	        ResourceManager.PrepareResource<GameObject>
	            ("Effect/UI/UI_TuJianHeCheng.prefab", res =>
	            {
	                if (EffectRoot)
	                {
	                    NGUITools.AddChild(EffectRoot, res);
	                }
	                isInEffect = true;
	                ResourceManager.Instance.StartCoroutine(DoCompose());
	                var ee = new UIEvent_HandBookFrame_ShowAnimationBlocker(true);
	                EventDispatcher.Instance.DispatchEvent(ee);
	            });
	    }
	}
}