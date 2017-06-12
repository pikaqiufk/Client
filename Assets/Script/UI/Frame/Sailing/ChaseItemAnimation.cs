using System;
#region using

using System.Collections;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ChaseItemAnimation : MonoBehaviour
	{
	    private bool isActive;
	    private GameObject myLoadObj;
	    public string ResPath = "Effect/UI/UI_ChuanZhiChuDong.prefab";
	    public UISpriteAnimation SpriteAnim;
	    public float WaitSeconds = 2f;
	
	    public bool IsActive
	    {
	        set
	        {
	            isActive = value;
	            if (isActive)
	            {
	                PlayAnimation();
	            }
	        }
	        get { return isActive; }
	    }
	
	    private IEnumerator DestroyLevelUpObj()
	    {
	        yield return new WaitForSeconds(WaitSeconds);
	        if (SpriteAnim != null)
	        {
	            SpriteAnim.ResetToBeginning();
	        }
	        myLoadObj.SetActive(false);
	    }
	
	    public void InitRes()
	    {
	        myLoadObj = Instantiate(ResourceManager.PrepareResourceSync<GameObject>(ResPath)) as GameObject;
	        var trans = myLoadObj.transform;
	        SpriteAnim = myLoadObj.GetComponentInChildren<UISpriteAnimation>();
	        var parentTrans = gameObject.transform;
	        trans.parent = parentTrans; //LightPoint[i].transform;
	        trans.localPosition = Vector3.zero;
	        trans.localScale = Vector3.one;
	        myLoadObj.SetActive(false);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (myLoadObj != null)
	        {
	            Destroy(myLoadObj);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        InitRes();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void PlayAnimation()
	    {
	        myLoadObj.SetActive(true);
	        if (SpriteAnim != null)
	        {
	            SpriteAnim.Play();
	        }
	        StartCoroutine(DestroyLevelUpObj());
	    }
	}
}