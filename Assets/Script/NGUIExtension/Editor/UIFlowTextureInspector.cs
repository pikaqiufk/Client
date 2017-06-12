using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIFlowTexture))]
public class UIFlowTextureInspector : Editor
{

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		var ui = target as UIFlowTexture;
		if (GUI.changed)
		{
			ui.RefreshMaterialProperty();
		}
	}
}
