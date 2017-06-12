#region using

using System;
using UnityEngine;

#endregion

namespace GameUI
{
	public class WingClickAndMoveTo : MonoBehaviour
	{
	    public static Transform sCalcTransform;
	    [NonSerialized] public float Distance = 5.0f;
	    public int Id;
	    private bool mActive;
	    private Material mMaterial;
	    private float mOrignalScale;
	    private Transform mTransform;
	    [NonSerialized] public Vector3 Offset;
	    [NonSerialized] public WingLinkedNodeTransfer Parent;
	    [NonSerialized] public float Speed = 1;
	    private bool startRotate;
	
	    public bool Active
	    {
	        get { return mActive; }
	        set
	        {
	            mActive = value;
	            mMaterial.SetColor("_TintColor", mActive ? new Color(0.48f, 0.7f, 0f) : new Color(0.5f, 0.5f, 0.5f));
	            Parent.SetLineActive(Id, mActive);
	        }
	    }
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        mTransform = transform;
	        mOrignalScale = mTransform.localScale.x;
	        mMaterial = renderer.material;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void LookAt()
	    {
	        if (!sCalcTransform)
	        {
	            var o = new GameObject();
	            sCalcTransform = o.transform;
	        }
	
	        sCalcTransform.parent = mTransform;
	        sCalcTransform.position = Parent.mTransform.position;
	        sCalcTransform.rotation = Parent.mTransform.rotation;
	
	        var rot = mTransform.rotation;
	        var pos = mTransform.position;
	
	        mTransform.position = Offset + Vector3.forward*Distance;
	        mTransform.rotation = Quaternion.LookRotation(Vector3.forward);
	
	        var trot = sCalcTransform.rotation;
	        var tpos = sCalcTransform.position;
	
	        mTransform.position = pos;
	        mTransform.rotation = rot;
	
	        Parent.mTransform.position = tpos;
	        Parent.mTransform.rotation = trot;
	    }
	
	    public void OnPress()
	    {
	        var pLast = Offset + Vector3.forward*Distance;
	        var target = Parent.mTransform.position + pLast - mTransform.position;
	
	        Parent.OnBeginRotate = () => { startRotate = true; };
	
	        iTween.MoveTo(Parent.gameObject,
	            iTween.Hash("position", target, "time",
	                Mathf.Min(0.1f, (target - Parent.mTransform.position).magnitude/Speed), "easetype",
	                iTween.EaseType.linear, "oncomplete",
	                "BeginRotate"));
	    }
	
	    // Update is called once per frame
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        var scale = Distance/Mathf.Abs(mTransform.position.z)*mOrignalScale;
	        mTransform.localScale = new Vector3(scale, scale, scale);
	
	
	        if (startRotate)
	        {
	            if (!sCalcTransform)
	            {
	                var o = new GameObject();
	                sCalcTransform = o.transform;
	            }
	            sCalcTransform.parent = mTransform;
	            sCalcTransform.position = Parent.mTransform.position;
	            sCalcTransform.rotation = Parent.mTransform.rotation;
	
	            var rot = mTransform.rotation;
	            var pos = mTransform.position;
	
	            mTransform.position = Offset + Vector3.forward*Distance;
	            mTransform.rotation = Quaternion.RotateTowards(mTransform.rotation, Quaternion.LookRotation(Vector3.forward),
	                8);
	
	            if (mTransform.rotation == Quaternion.LookRotation(Vector3.forward))
	            {
	                startRotate = false;
	            }
	
	            var trot = sCalcTransform.rotation;
	            var tpos = sCalcTransform.position;
	
	            mTransform.position = pos;
	            mTransform.rotation = rot;
	
	            Parent.mTransform.position = tpos;
	            Parent.mTransform.rotation = trot;
	
	
	            if (!startRotate)
	            {
	                Parent.MoveOver();
	            }
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