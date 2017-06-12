using System;
using UnityEngine;

public class CameraFadeEffect : MonoBehaviour {

	public Texture Tex;
	public float Duration =1.0f;
	public Color BeginColor = new Color(0,0,0,1);
	public Color EndColor = new Color(0, 0, 0, 0);
	private Color CurrentColor;
	private float TotalTime = 1;
	public Action CallBack = null;
	// Use this for initialization
	void Start ()
	{
#if !UNITY_EDITOR
try
{
#endif

		TotalTime = Duration;
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
	
	// Update is called once per frame
	void Update ()
	{
#if !UNITY_EDITOR
try
{
#endif

		Duration -= Time.deltaTime;
		CurrentColor = Color.Lerp(BeginColor, EndColor, (TotalTime - Duration) / TotalTime);
		if (Duration <= 0)
		{
			if (null != CallBack)
			{
				CallBack();
			}
			Destroy(this);
		}
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	void OnGUI()
	{
		GUI.depth = -9999999;
		GUI.color = CurrentColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Tex);
	}

	public static void DoCameraFade(GameObject obj,Color begin, Color end, float duration, Action call=null,Texture tex=null)
	{
		var effect = obj.AddComponent<CameraFadeEffect>();
		effect.BeginColor = begin;
		effect.EndColor = end;
		effect.Duration = duration;
		effect.Tex = tex ?? new Texture2D(32, 32);
		effect.CallBack = call;
	}
}
