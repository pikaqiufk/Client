using System;
using UnityEngine;
using System.Collections;

/*-------------------------------------------------------------------
bailu				2016-01-26 14:45:52
			带流光的UITexture
*/

[ExecuteInEditMode]
[RequireComponent(typeof(UITexture))]
public class UIFlowTexture : MonoBehaviour {

	//流光颜色
	[SerializeField]
	private Texture FlowTexture = null;

	//流光时间间隔
	[SerializeField]
	private float Interval = 5f;


	//流光颜色
	[SerializeField]
	private Color FlowColor = Color.white;

	public bool Flowing = true;

	//被流光的UITexture
	private UITexture mUITexture;
    private Material _material;

	void Awake()
	{
#if !UNITY_EDITOR
try
{
#endif

	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	// Use this for initialization
	void Start () 
	{
#if !UNITY_EDITOR
try
{
#endif

		ResourceManager.PrepareResource<Material>(Resource.Dir.Material + "UITexture_Flow.mat", material =>
		{
            _material = new Material(material);
			_material.hideFlags = HideFlags.DontSave;
			RefreshMaterialProperty();
		});
	
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

        mUITexture = gameObject.GetComponent<UITexture>();
        if (mUITexture!=null && _material != null)
        {
            mUITexture.material = _material;
        }
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	[ContextMenu("Refresh Material Property")]
	public void RefreshMaterialProperty()
	{
		if (null == mUITexture)
		{
			return;
		}

		var mat = mUITexture.material;
		if (null == mat)
		{
			return;
		}

		if (null != FlowTexture)
		{
			mat.SetTexture("_FlashTex", FlowTexture);
		}

		mat.SetFloat("_Interval", Interval);
		if (!Flowing)
		{
			mat.SetColor("_FlashColor", new Color(0,0,0,0));		
		}
		else
		{
			mat.SetColor("_FlashColor", FlowColor);		
		}

		mUITexture.enabled = false;
		mUITexture.enabled = true;
	}

	public void SetFlow(bool flag)
	{
		Flowing = flag;
		RefreshMaterialProperty();
	}
}
