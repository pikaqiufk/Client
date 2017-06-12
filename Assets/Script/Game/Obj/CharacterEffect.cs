using System;
#region using

using UnityEngine;

#endregion

public class CharacterEffect : MonoBehaviour
{
    private float angle;
    public Color DiffuseColor = Color.white;
    public float EmitLevel = 3000f;
    public float ExpRange = 300f;
    private bool fast;
    public float FastSpeed = Mathf.PI;
    public Color FlowColor = new Color(1.0f, 0.01f, 0.01f, 1.0f);
    public float ScrollSpeed = 0.3f;
    public float SlowSpeed = Mathf.PI*0.3f;
    public Color SpecularColor = new Color(1.0f, 0.5f, 0.01f, 0.3f);
    private float texOffset;
    // Use this for initialization
    private void Start()
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

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        var renderer = gameObject.GetComponent<Renderer>();
        {
            var __array1 = renderer.sharedMaterials;
            var __arrayLength1 = __array1.Length;
            for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var material = __array1[__i1];
                {
                    //if (fast)
                    {
                        angle += FastSpeed*Time.deltaTime;
                        if (angle > Mathf.PI)
                        {
                            fast = false;
                        }
                    }
                    // 	    else
                    // 	    {
                    // 	        angle -= SlowSpeed * Time.deltaTime;
                    // 	        if (angle < 0)
                    // 	        {
                    // 	            fast = true;
                    // 	        }
                    // 	    }

                    var x = Mathf.Cos(angle);
                    var z = Mathf.Sin(angle);
                    var w = ExpRange;
                    var y = EmitLevel;

                    material.SetColor("_Color", DiffuseColor);
                    material.SetColor("_TexColor", SpecularColor);
                    material.SetColor("_BColor", FlowColor);

                    material.SetVector("_ViewDir", new Vector4(x, y, z, w));

                    texOffset += Time.deltaTime*ScrollSpeed;
                    material.SetTextureOffset("_AlphaTex", new Vector2(texOffset, 0));
                }
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