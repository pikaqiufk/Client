using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/DepthManager/ObjSetRenderQueue")]
[RequireComponent(typeof(Renderer))]
public class ObjSetRenderQueue : MonoBehaviour {

	public int[] m_RenderQueueNums;
	int m_RenderQueueNum = 3000;
	
	void Awake () {		
		if (!renderer || !renderer.sharedMaterial || m_RenderQueueNums == null)
		{
			return;
		}
		renderer.sharedMaterial.renderQueue = m_RenderQueueNum;
		for (int i = 0; i < m_RenderQueueNums.Length && i < renderer.sharedMaterials.Length; i++)
		{
         renderer.sharedMaterials[i].renderQueue = m_RenderQueueNums[i];
		}
	}
	
}
