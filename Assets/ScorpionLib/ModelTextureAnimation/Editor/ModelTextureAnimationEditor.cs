using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModelTextureAnimation))]
public class ModelTextureAnimationEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		ModelTextureAnimation mta = target as ModelTextureAnimation;

		AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", mta.Curve, GUILayout.Width(200f), GUILayout.Height(62f));


		if (GUI.changed)
		{
			mta.OnResetTile();
			mta.Curve = curve;
		}
	}
}
