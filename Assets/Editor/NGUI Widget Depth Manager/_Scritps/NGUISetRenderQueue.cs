using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NGUISetRenderQueue : MonoBehaviour {
	
	public int m_RenderQueueNum;
	UIWidget m_NguiWidget;	
	
	void Awake () {
		
		m_NguiWidget = (UIWidget)this.GetComponent("UIWidget");
		m_NguiWidget.material.renderQueue = m_RenderQueueNum;

	}
	
}
