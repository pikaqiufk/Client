//----------------------------------------------
// Particle2D for NGUI
// Copyright © 2014 Universal Creators
// Version 1.0.3
//----------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[AddComponentMenu("NGUI/UI/Particle2D")]
[ExecuteInEditMode]
public class Particle2D : UIWidget
{

#if UNITY_EDITOR
	readonly int REFRESH_RATE = 30;
#endif // UNITY_EDITOR

    public enum Emitter { POINT, CIRCLE, RECT }

    //--------------------------------------------------------------------------------
    // Particle2D Settings

    [HideInInspector]
    [SerializeField]
    public UIAtlas atlas;
    [HideInInspector]
    [SerializeField]
    public List<string> sprites;

    [HideInInspector]
    [SerializeField]
    public bool worldSpace = false;

    // Emitter
    [HideInInspector]
    [SerializeField]
    public Emitter emitter = Emitter.POINT;
    [HideInInspector]
    [SerializeField]
    public float spwnRate = 0.1f;               // Per second
    [HideInInspector]
    [SerializeField]
    public bool looping = true;
    [HideInInspector]
    [SerializeField]
    public float duration = 2f;             // Second
    [HideInInspector]
    [SerializeField]
    public int maxParticles = 10;
    [HideInInspector]
    [SerializeField]
    public bool playOnce = false;

    // Circle emitter
    [HideInInspector]
    [SerializeField]
    public float emitterRadius = 100.0f;

    // Rect emitter
    [HideInInspector]
    [SerializeField]
    public float emitterWidth = 100.0f;
    [HideInInspector]
    [SerializeField]
    public float emitterHeight = 100.0f;

    // Life time
    [HideInInspector]
    [SerializeField]
    public float minLifeTime = 1.0f;
    [HideInInspector]
    [SerializeField]
    public float maxLifeTime = 1.0f;

    // Size
    [HideInInspector]
    [SerializeField]
    public float minSize = 1.0f;
    [HideInInspector]
    [SerializeField]
    public float maxSize = 1.0f;

    [HideInInspector]
    [SerializeField]
    public bool useSizeRate = false;
    [HideInInspector]
    [SerializeField]
    public AnimationCurve sizeRate;

    // Velocity
    [HideInInspector]
    [SerializeField]
    public bool useVelocity = false;
    [HideInInspector]
    [SerializeField]
    public float minAngle = -180f;
    [HideInInspector]
    [SerializeField]
    public float maxAngle = 180f;
    [HideInInspector]
    [SerializeField]
    public float minPower = 200f;
    [HideInInspector]
    [SerializeField]
    public float maxPower = 300f;

    [HideInInspector]
    [SerializeField]
    public bool useAcceleration = false;
    [HideInInspector]
    [SerializeField]
    public AnimationCurve accelerationRate;

    // Rotation
    [HideInInspector]
    [SerializeField]
    public bool useRotation = false;
    [HideInInspector]
    [SerializeField]
    public Vector3 minRotation = new Vector3(0, 0, 90f);
    [HideInInspector]
    [SerializeField]
    public Vector3 maxRotation = new Vector3(0, 0, 180f);

    [HideInInspector]
    [SerializeField]
    public bool useRotationRate = false;
    [HideInInspector]
    [SerializeField]
    public AnimationCurve rotationRateX;
    [HideInInspector]
    [SerializeField]
    public AnimationCurve rotationRateY;
    [HideInInspector]
    [SerializeField]
    public AnimationCurve rotationRateZ;

    // Force
    [HideInInspector]
    [SerializeField]
    public bool useForce = false;
    [HideInInspector]
    [SerializeField]
    public Vector2 force = new Vector3(0, 500f);

    // Color
    [HideInInspector]
    [SerializeField]
    public bool useAnimatedColor = true;
    [HideInInspector]
    [SerializeField]
    public List<Color> colors;


    //--------------------------------------------------------------------------------
    // Member variables

    float startTime;
    float lastGenParticleTime = 0f;
    public int curParticleIndex = 0;


    //--------------------------------------------------------------------------------
    // Pool management

    int _poolSize = 0;
    List<ParticleUnit> _objects = new List<ParticleUnit>();
    Queue<ParticleUnit> _freeObjects = new Queue<ParticleUnit>();
    List<ParticleUnit> _removableObjects = new List<ParticleUnit>();

    float scaleFactor = 1.5f;

    protected void SetPoolSize(int newSize)
    {
        if (newSize <= _poolSize) return;

        ParticleUnit unit;
        for (int i = _poolSize; i < newSize; i++)
        {
            unit = new ParticleUnit();
            _freeObjects.Enqueue(unit);
        }

        _poolSize = newSize;
    }

    ParticleUnit Generate(float generateTime)
    {
        if (_freeObjects.Count <= 0)
        {
            // Resize pool size
            int newSize = (int)(_poolSize * scaleFactor);
            if (newSize <= _poolSize) newSize = _poolSize + 1;
            SetPoolSize(newSize);
        }

        ParticleUnit unit = _freeObjects.Dequeue();
        unit.CreateParticleUnit(this, generateTime);
        _objects.Add(unit);

        return unit;
    }

    void Remove(ParticleUnit obj)
    {
        _freeObjects.Enqueue(obj);
        _objects.Remove(obj);
    }

    public void Reset()
    {
        startTime = GetTime();
        lastGenParticleTime = startTime - spwnRate;
        curParticleIndex = 0;
        {
            var __list1 = _objects;
            var __listCount1 = __list1.Count;
            for (int __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var obj = (ParticleUnit)__list1[__i1];
                {
                    _freeObjects.Enqueue(obj);
                }
            }
        }
        _objects.Clear();
    }

    public void AddRemovableObject(ParticleUnit unit)
    {
        _removableObjects.Add(unit);
    }

    void ClearRemovableObjects()
    {
        for (int i = _removableObjects.Count - 1; i >= 0; i--)
        {
            Remove(_removableObjects[i]);
        }
        _removableObjects.Clear();
    }


    //------------------------------------------------------------

    public override Material material { get { return (atlas != null) ? atlas.spriteMaterial : null; } }

    protected bool _changed = true;
    protected override void OnStart()
    {
        base.OnStart();

        mWidth = 0;
        mHeight = 0;

        if (!looping)
        {
            float rate = duration / spwnRate;
            if (rate > 1.0f) rate = 1.0f;
            int size = (int)(maxParticles * rate);
            size = (size <= 0) ? 1 : size;
            SetPoolSize(size);

        }
        else
        {
            if (maxLifeTime > spwnRate)
            {
                SetPoolSize((int)(maxLifeTime / spwnRate));
            }
            else
            {
                SetPoolSize(1);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

#if UNITY_EDITOR
		startUnixTime = now_unix_time();
#endif // UNITY_EDITOR

        Reset();
    }

#if UNITY_EDITOR
	long startUnixTime = 0;
	long now_unix_time () {
		long curTime = Convert.ToInt64((DateTime.UtcNow.Ticks - 621355968000000000) / 10000);
		if (startUnixTime == 0) startUnixTime = curTime;

		return curTime;
	}
#endif // UNITY_EDITOR

    float GetTime()
    {
#if UNITY_EDITOR
		return (Application.isPlaying) ? Time.time : (now_unix_time() - startUnixTime) * 0.001f;
#else
        return Time.time;
#endif // UNITY_EDITOR
    }

#if UNITY_EDITOR
	float lastTime = 0f;
#endif // UNITY_EDITOR
    float GetDeltaTime()
    {
#if UNITY_EDITOR
		float dt;
		if (Application.isPlaying) {
			dt = Time.deltaTime;
		} else {
			dt = currentTime - lastTime;
			lastTime = currentTime;
		}
		return dt;
#else
        return Time.deltaTime;
#endif // UNITY_EDITOR
    }

    void GenerateParticle()
    {
        float lastTime = currentTime - maxLifeTime;
        if (lastTime > lastGenParticleTime)
        {
            lastGenParticleTime = lastTime;
        }

        while (lastGenParticleTime + spwnRate <= currentTime)
        {
            lastGenParticleTime += spwnRate;

            if (curParticleIndex >= maxParticles) break;

            Generate(lastGenParticleTime);
            curParticleIndex++;
        }
    }

    void GenerateLoopingParticle()
    {
        float lastTime = currentTime - maxLifeTime;
        if (lastTime > lastGenParticleTime)
        {
            lastGenParticleTime = lastTime;
        }

        while (lastGenParticleTime + spwnRate <= currentTime)
        {
            lastGenParticleTime += spwnRate;
            Generate(lastGenParticleTime);
            curParticleIndex++;
        }
    }

#if UNITY_EDITOR
	void OnDrawGizmos () {
		if (isVisible && NGUITools.GetActive(this)) {
			if (UnityEditor.Selection.activeGameObject != gameObject) return;

			Color outline = new Color(1f, 1f, 1f, 0.2f);
			Gizmos.color = (UnityEditor.Selection.activeGameObject == cachedTransform) ? Color.white : outline;
			Gizmos.matrix = cachedTransform.localToWorldMatrix;

			if (emitter == Emitter.RECT) {
				Gizmos.DrawWireCube(Vector3.zero, new Vector3(emitterWidth, emitterHeight, 1f));

			} else if (emitter == Emitter.CIRCLE) {
				Gizmos.DrawWireSphere(Vector3.zero, emitterRadius);

			} else {
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			}

			if (Application.isPlaying == false && waitForRefresh == false) {
				IEnumerator e = AutoRefresh ();
				while (e.MoveNext());
			}
		}
	}

	bool waitForRefresh = false;
	IEnumerator AutoRefresh () {
		waitForRefresh = true;

		long st = now_unix_time();
		while (now_unix_time() - st < REFRESH_RATE) {
			yield return null;
		}

		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		Refresh();

		waitForRefresh = false;
	}
	
	public void Refresh () {
		OnUpdate();
	}
#endif // UNITY_EDITOR

    float currentTime = 0f;
    float currentDeltaTime = 0f;
    protected override void OnUpdate()
    {
        base.OnUpdate();

        currentTime = GetTime();
        currentDeltaTime = GetDeltaTime();

        if (atlas == null || atlas.texture == null || sprites.Count <= 0) return;

        // Generate particle unit
        if (!looping)
        {
            if (currentTime >= startTime + duration)
            {
                if (!playOnce)
                {
                    startTime = currentTime;
                    lastGenParticleTime = startTime - spwnRate;
                    curParticleIndex = 0;
                }
            }
            GenerateParticle();

        }
        else
        {
            GenerateLoopingParticle();
        }

        mChanged = true;

#if UNITY_EDITOR
		if (panel && Application.isPlaying == false) panel.Refresh();
#endif // UNITY_EDITOR
    }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        {
            var __list2 = _objects;
            var __listCount2 = __list2.Count;
            for (int __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var unit = (ParticleUnit)__list2[__i2];
                {
                    if (unit.UpdateParticleUnit(this, currentTime, currentDeltaTime))
                    {
                        SimpleFill(geometry.verts, geometry.uvs, geometry.cols, unit.texCoordsOuter, unit.texCoordsInner, unit);
                    }
                }
            }
        }
        ClearRemovableObjects();
    }

    public string GetSpriteNameRandomly()
    {
        return sprites[UnityEngine.Random.Range(0, sprites.Count)];
    }

    public Color GetColor(float t)
    {
        float gap = 1.0f / (colors.Count - 1);
        int idx = (int)(t / gap);
        return Color.Lerp(colors[idx], colors[idx + 1], (t - gap * idx) / gap);
    }

    void SimpleFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Rect outer, Rect inner, ParticleUnit unit)
    {
        // Set scale
        float spriteWidth = unit.spriteData.width * unit.scale.x * unit.localScale.x;
        float spriteHeight = unit.spriteData.height * unit.scale.y * unit.localScale.y;

        float x0 = -(spriteWidth * 0.5f);
        float y0 = -(spriteHeight * 0.5f);
        float x1 = x0 + spriteWidth;
        float y1 = y0 + spriteHeight;

        Vector2 v0 = new Vector2(x0, y0);
        Vector2 v1 = new Vector2(x0, y1);
        Vector2 v2 = new Vector2(x1, y1);
        Vector2 v3 = new Vector2(x1, y0);

        // Set rotation
        RotateVertex(ref v0, ref v1, ref v2, ref v3, unit.localRotation);

        // Set position
        Vector2 pos = (Vector2)transform.InverseTransformPoint(unit.distanceFromParticle2D + transform.position) + unit.localPosition;
        v0 += pos;
        v1 += pos;
        v2 += pos;
        v3 += pos;

        // Add vertices
        verts.Add(v0);
        verts.Add(v1);
        verts.Add(v2);
        verts.Add(v3);

        // Add texcoords
        Vector4 u = new Vector4(outer.xMin, outer.yMin, outer.xMax, outer.yMax);
        uvs.Add(new Vector2(u.x, u.y));
        uvs.Add(new Vector2(u.x, u.w));
        uvs.Add(new Vector2(u.z, u.w));
        uvs.Add(new Vector2(u.z, u.y));

        // Add colors
        Color32 c = unit.color * mColor;
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
    }

    void RotateVertex(ref Vector2 v0, ref Vector2 v1, ref Vector2 v2, ref Vector2 v3, Quaternion q)
    {
        v0 = q * v0;
        v1 = q * v1;
        v2 = q * v2;
        v3 = q * v3;
    }
}

public class ParticleUnit
{
    public float startTime;
    public float endTime;

    public float maxLife;
    public Vector3 velocity;
    public Vector2 scale;

    // Position
    public Vector3 distanceFromParticle2D = Vector3.zero;   // Distance from particle2D object based on world
    public Vector2 generatedPosition = Vector2.zero;        // Generated position based on local
    public Vector2 localPosition = Vector2.zero;

    // Rotation
    public Quaternion localRotation = Quaternion.identity;

    // Scale
    public Vector2 localScale = Vector2.one;

    // Texcoords
    public UISpriteData spriteData;
    public Rect texCoordsOuter;
    public Rect texCoordsInner;

    // Color
    public Color color = Color.white;

    // Local variables
    Vector3 startPoint;     // Started position based on world


    public void CreateParticleUnit(Particle2D particle, float curTime)
    {
        // Sprite
        Texture tex = particle.atlas.texture;
        spriteData = particle.atlas.GetSprite(particle.GetSpriteNameRandomly());
        Rect outer = new Rect(spriteData.x, spriteData.y, spriteData.width, spriteData.height);
        Rect inner = new Rect(spriteData.x + spriteData.borderLeft, spriteData.y + spriteData.borderTop, spriteData.width - spriteData.borderLeft - spriteData.borderRight, spriteData.height - spriteData.borderBottom - spriteData.borderTop);
        texCoordsOuter = NGUIMath.ConvertToTexCoords(outer, tex.width, tex.height);
        texCoordsInner = NGUIMath.ConvertToTexCoords(inner, tex.width, tex.height);

        // Emitter
        if (particle.emitter == Particle2D.Emitter.RECT)
        {
            float halfWidth = particle.emitterWidth * 0.5f;
            float halfHeight = particle.emitterHeight * 0.5f;
            generatedPosition = new Vector3(UnityEngine.Random.Range(-halfWidth, halfWidth), UnityEngine.Random.Range(-halfHeight, halfHeight), 0);

        }
        else if (particle.emitter == Particle2D.Emitter.CIRCLE)
        {
            float rad = particle.emitterRadius;
            float r = UnityEngine.Random.Range(-rad, rad);
            float angle = UnityEngine.Random.Range(0, Mathf.PI);
            float x = Mathf.Cos(angle) * r;
            float y = Mathf.Sin(angle) * r;
            generatedPosition = new Vector3(x, y, 0);

        }
        else
        {
            generatedPosition = Vector3.zero;
        }

        startPoint = particle.transform.position;

        // Life
        maxLife = UnityEngine.Random.Range(particle.minLifeTime, particle.maxLifeTime);

        // Size
        float size = UnityEngine.Random.Range(particle.minSize, particle.maxSize);
        scale = new Vector2(size, size);

        // Rotation
        if (particle.useRotation)
        {
            localRotation = Quaternion.Euler(new Vector3(-UnityEngine.Random.Range(particle.minRotation.x, particle.maxRotation.x), -UnityEngine.Random.Range(particle.minRotation.y, particle.maxRotation.y), -UnityEngine.Random.Range(particle.minRotation.z, particle.maxRotation.z)));
        }
        else
        {
            localRotation = Quaternion.identity;
        }

        // Velocity
        velocity = Quaternion.AngleAxis(UnityEngine.Random.Range(particle.minAngle, particle.maxAngle), Vector3.back) * new Vector3(0f, UnityEngine.Random.Range(particle.minPower, particle.maxPower), 0f);

        // Time
        startTime = curTime;
        endTime = startTime + maxLife;
    }

    public bool UpdateParticleUnit(Particle2D particle, float curTime, float curDeltaTime)
    {
        // Check the life
        if (curTime >= endTime)
        {
            particle.AddRemovableObject(this);
            return false;
        }

        float timeFactor = 1.0f - (endTime - curTime) / maxLife;

        //---------------------------------------
        // Size
        if (particle.useSizeRate)
        {
            localScale = scale * particle.sizeRate.Evaluate(timeFactor);
        }
        else
        {
            localScale = Vector2.one;
        }

        //---------------------------------------
        // Position
        if (particle.worldSpace)
        {
            distanceFromParticle2D = startPoint - particle.transform.position;
        }
        else
        {
            distanceFromParticle2D = Vector3.zero;
        }

        if (particle.useVelocity)
        {
            float elapsedTime = curTime - startTime;

            Vector2 v = velocity;

            // Acceleration
            if (particle.useAcceleration)
            {
                v *= particle.accelerationRate.Evaluate(timeFactor);
            }

            // Gravity
            if (particle.useForce)
            {
                localPosition = generatedPosition + (v * elapsedTime) + (particle.force * Mathf.Pow(elapsedTime, 2f));

            }
            else
            {
                localPosition = generatedPosition + v * elapsedTime;
            }
        }
        else
        {
            localPosition = generatedPosition;
        }

        //---------------------------------------
        // Rotation rate
        if (particle.useRotation && particle.useRotationRate)
        {
            localRotation *= Quaternion.Euler(new Vector3(-particle.rotationRateX.Evaluate(timeFactor) * 10.0f, -particle.rotationRateY.Evaluate(timeFactor) * 10.0f, -particle.rotationRateZ.Evaluate(timeFactor) * 10.0f));
        }

        //---------------------------------------
        // Color
        if (particle.useAnimatedColor)
        {
            color = particle.GetColor(timeFactor);
        }
        else
        {
            color = Color.white;
        }

        color.a *= particle.finalAlpha;

        return true;
    }
}
