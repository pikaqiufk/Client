using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class PlayAnimationOnShow : MonoBehaviour
{
	private Animation Ani; 
	public AnimationClip Clip;

	void Awake()
	{
#if !UNITY_EDITOR
try
{
#endif

		Ani = gameObject.GetComponent<Animation>();
		if(Clip!=null)
		{
			Ani.AddClip(Clip,Clip.name);
		}
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	void OnEnable()
	{
#if !UNITY_EDITOR
try
{
#endif

		if(null!=Ani && null!=Clip)
		{
			Ani.Play(Clip.name);
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
