#region using

using System;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EffectNode : MonoBehaviour
	{
	    //0 ???????
	    public int AnimationType = 0;
	    private bool isActive;
	    private GameObject myObj;
	    public string ResPath = String.Empty;
	
	    public bool IsActive
	    {
	        set
	        {
	            isActive = value;
	            SetActive(isActive);
	            if (isActive)
	            {
	                InitRes();
	                PlayAnimation();
	            }
	            else
	            {
	                ReleaseObj();
	            }
	        }
	
	        get { return isActive; }
	    }
	
	    public void InitRes()
	    {
	        //myObj = ComplexObjectPool.NewObjectSync(ResPath);
	        //ComplexObjectPool.NewObject(ResPath, (obj) =>
            ResourceManager.PrepareResource<GameObject>(ResPath, (obj)=>
	        {
	            if (obj == null)
	            {
	                return;
	            }
                myObj = NGUITools.AddChild(gameObject, obj);
                var trans = myObj.transform;
	            trans.localPosition = Vector3.zero;
	            trans.localScale = Vector3.one;
	            var render = gameObject.GetComponent<ChangeRenderQueue>();
	            if (render != null)
	            {
	                render.RefleshRenderQueue();
	            }
	        });
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        ReleaseObj();
	
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
	        if (myObj == null)
	        {
	            InitRes();
	        }
	        PlayAnimation();
	
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
	        switch (AnimationType)
	        {
	            case 0:
	                break;
	            case 1:
	                break;
	        }
	    }
	
	    public void ReleaseObj()
	    {
            if (myObj != null)
            {
                Destroy(myObj);
                myObj = null;
            }
	    }
	
	    public void SetActive(bool active)
	    {
	        if (gameObject.activeSelf != active)
	        {
	            gameObject.SetActive(active);
	        }
	    }
	}
}