using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class UVColorAnimation : MonoBehaviour
{
    public bool Loop = false;
    public float StartDelay = 0;
    public float StartLifetime = 0.1f;
    public bool UseColorOverLifetime = false;
    public Gradient ColorOverLifetime = new Gradient();
    public Color StartColor = Color.white;
    public bool UseSizeOverLifetime = false;
    public AnimationCurve SizeOverLifetime = new AnimationCurve();
    public bool UseUOffsetOverLifetime = false;
    public AnimationCurve UOffsetOverLifetime = new AnimationCurve();
    public bool UseVOffsetOverLifetime = false;
    public AnimationCurve VOffsetOverLifetime = new AnimationCurve();
    public string TextureName = "_MainTex";
    public string ColorName = "_TintColor";
    public bool DisableAfterFinished = true;

    //cache
    private Transform transform;
    private float lifeTime;
    private Vector3 scale;
    private Material material;
    private Renderer renderer;

    //values
    private Color currentColor;
    private float uOffset = 0;
    private float vOffset = 0;
    
    void Awake()
    {
        transform = gameObject.transform;
        lifeTime = 0;
        scale = transform.localScale;
        renderer = transform.renderer;
    }

    void OnEnable()
    {
        Awake();
        Start();
    }

    // Use this for initialization
    void Start()
    {
        currentColor = StartColor;
        material = renderer.material;
        renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime > StartDelay + StartLifetime)
        {
            if (UseColorOverLifetime)
            {
                currentColor = ColorOverLifetime.Evaluate(1);
            }

            if (UseUOffsetOverLifetime)
            {
                uOffset = UOffsetOverLifetime.Evaluate(1);
            }

            if (UseVOffsetOverLifetime)
            {
                vOffset = VOffsetOverLifetime.Evaluate(1);
            }

            material.SetTextureOffset(TextureName, new Vector2(uOffset, vOffset));
            material.SetColor(ColorName, currentColor);

            if (UseSizeOverLifetime)
            {
                var s = SizeOverLifetime.Evaluate(1);
                transform.localScale = scale * s;
            }

            if (Loop)
            {
                lifeTime = 0;
                return;
            }

            if (DisableAfterFinished)
            {
                this.enabled = false;
            }
            return;
        }

        if (lifeTime < StartDelay)
        {
            return;
        }

        if (!renderer.enabled)
            renderer.enabled = true;
        
        var percentage = (lifeTime - StartDelay) / StartLifetime;

        if (UseColorOverLifetime)
        {
            currentColor = ColorOverLifetime.Evaluate(percentage);
        }

        if (UseUOffsetOverLifetime)
        {
            uOffset = UOffsetOverLifetime.Evaluate(percentage);
        }

        if (UseVOffsetOverLifetime)
        {
            vOffset = VOffsetOverLifetime.Evaluate(percentage);
        }

        material.SetTextureOffset(TextureName, new Vector2(uOffset, vOffset));
        material.SetColor(ColorName, currentColor);

        if (UseSizeOverLifetime)
        {
            var s = SizeOverLifetime.Evaluate(percentage);
            transform.localScale = scale * s;
        }
    }

}
