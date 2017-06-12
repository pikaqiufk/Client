/********************************************************************
	purpose:	UV动画
*********************************************************************/

using UnityEngine;
using System.Collections;

public class UVAnimation : MonoBehaviour
{
    public float SpeedX = 0;
    public float SpeedY = 0;

    private Material material;
    // Use this for initialization
    void Start()
    {
        material = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (!material)
        {
            material = renderer.material;
        }
        
        material.SetTextureOffset("_MainTex", new Vector2(Time.time * SpeedX, Time.time * SpeedY));
    }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }
}
