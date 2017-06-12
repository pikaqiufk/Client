using UnityEngine;
using System.Collections;

public class TriggerArea : MonoBehaviour {
	public int Id;
	public int SceneId { get; set; }
	public float Radius = 1.0f;
	private Color mColor = Color.white;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			var scene = GameObject.FindObjectOfType<Scene>();
			if (null != scene)
			{
				var pos = gameObject.transform.position;
				pos.y = GameLogic.GetTerrainHeight(pos.x,pos.z);
				gameObject.transform.position = pos;
				return;
			}
		}
#endif
	}

	void OnDrawGizmos()
	{
		Gizmos.color = mColor;
		Gizmos.DrawWireSphere(transform.position, Radius);
	}

	void OnTriggerEnter(Collider obj)
	{
		mColor = Color.red;
	}

	void OnTriggerExit(Collider obj)
	{
		mColor = Color.white;
	}
}
