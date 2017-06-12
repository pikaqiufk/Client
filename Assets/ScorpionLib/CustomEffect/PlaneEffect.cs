using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class PlaneEffect : MonoBehaviour
{
    public bool Loop = false;
    public float StartDelay = 0;
    public float StartLifetime = 0.1f;
    public bool UseColorOverLifetime = false;
    public Gradient ColorOverLifetime = new Gradient();
    public Color StartColor = Color.white;
    public bool UseSizeOverLifetime = false;
    public AnimationCurve SizeOverLifetime = new AnimationCurve();
    public float RotationSpeed = 0;
    public int ColorIndex = 0;
    // Cached and saved values
    [HideInInspector]
    [SerializeField]
    UIAtlas mAtlas;
    [HideInInspector]
    [SerializeField]
    string mSpriteName;

    //cache
    [HideInInspector]
    public Transform transform;
    private float lifeTime;
    private Vector3 scale;
    private Material material;

    //values
    [HideInInspector]
    public Color CurrentColor;
    [HideInInspector]
    public Vector2 uvOffset;
    [HideInInspector]
    public Vector2 uvScale;

    [HideInInspector]
    public bool Active = false;

    //static
    private static GameObject rendererGameObject;
    private static BatchedPlaneEffectRender batchedPlaneEffectRender;

    void Awake()
    {
        Active = false;
        transform = gameObject.transform;
        lifeTime = 0;
        scale = transform.localScale;
    }

    void OnEnable()
    {
        Awake();
        Start();
    }

    void OnDisable()
    {
        Active = false;
        if (batchedPlaneEffectRender && this)
        {
            batchedPlaneEffectRender.Remove(material, this);
        }
    }

    // Use this for initialization
    void Start()
    {
        if (rendererGameObject == null)
        {
            rendererGameObject = new GameObject("PlaneEffectRenderer");
            batchedPlaneEffectRender = rendererGameObject.AddComponent<BatchedPlaneEffectRender>();
            rendererGameObject.AddComponent<SkinnedMeshRenderer>();
        }

        CurrentColor = StartColor;

        var sprite = mAtlas.GetSprite(mSpriteName);

        var w = (float)mAtlas.texture.width;
        var h = (float)mAtlas.texture.height;

        uvOffset = new Vector2(sprite.x / w, (h - sprite.height - sprite.y) / h);
        uvScale = new Vector2(sprite.width / w, sprite.height / h);
        material = mAtlas.spriteMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime > StartDelay + StartLifetime)
        {
            if (Loop)
            {
                lifeTime = 0;
                return;
            }

            batchedPlaneEffectRender.Remove(material, this);
            return;
        }

        if (lifeTime < StartDelay)
        {
            return;
        }

        if (!Active)
        {
            batchedPlaneEffectRender.Add(material, this);
            Active = true;
        }

        transform.Rotate(Vector3.up, Time.deltaTime * RotationSpeed);

        var percentage = (lifeTime - StartDelay) / StartLifetime;

        if (UseColorOverLifetime)
        {
            CurrentColor = ColorOverLifetime.Evaluate(percentage);
        }

        if (UseSizeOverLifetime)
        {
            var s = SizeOverLifetime.Evaluate(percentage);
            transform.localScale = scale * s;
        }
    }

    void OnDestroy()
    {
        batchedPlaneEffectRender.Remove(material, this);
    }

    //helper
    private static Vector3[] v = { new Vector3(0.5f, 0.2f, 0.5f), new Vector3(0.5f, 0.2f, -0.5f), new Vector3(-0.5f, 0.2f, -0.5f), new Vector3(-0.5f, 0.2f, 0.5f), };
    private void OnDrawGizmos()
    {
        var t = gameObject.GetComponent<Transform>();
        var p = v.Select(item => (t.localToWorldMatrix.MultiplyPoint3x4(item))).ToArray();
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(p[i], p[(i + 1) % 4]);
        }
    }
}

public class BatchedPlaneEffectRender : MonoBehaviour
{

    private Dictionary<int, Dictionary<Material, HashSet<PlaneEffect>>> dictionary = new Dictionary<int, Dictionary<Material, HashSet<PlaneEffect>>>();
    private Dictionary<int, Dictionary<Material, GameObject>> rendererDictionary = new Dictionary<int, Dictionary<Material, GameObject>>();
    private bool needUpdateMesh = true;

    public void Add(Material mat, PlaneEffect effect)
    {
        Dictionary<Material, HashSet<PlaneEffect>> dict;
        if (!dictionary.TryGetValue(effect.gameObject.layer, out dict))
        {
            dict = new Dictionary<Material, HashSet<PlaneEffect>>();
            dictionary.Add(effect.gameObject.layer, dict);
        }

        HashSet<PlaneEffect> set;
        if (!dict.TryGetValue(mat, out set))
        {
            set = new HashSet<PlaneEffect>();
            dict.Add(mat, set);
        }

        set.Add(effect);

        needUpdateMesh = true;
    }

    public void Remove(Material mat, PlaneEffect effect)
    {
        if (!effect)
        {
            return;
        }

        Dictionary<Material, HashSet<PlaneEffect>> dict;
        if (dictionary.TryGetValue(effect.gameObject.layer, out dict))
        {
            HashSet<PlaneEffect> set;
            if (dict.TryGetValue(mat, out set))
            {
                set.Remove(effect);
            }
        }

        needUpdateMesh = true;
    }

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uv = new List<Vector2>();
    private List<Color> colors = new List<Color>();
    private List<int> triangles = new List<int>();
    private List<BoneWeight> boneWeights = new List<BoneWeight>();

    private List<Transform> bones = new List<Transform>();
    private List<Matrix4x4> bindPose = new List<Matrix4x4>();
    private void Start()
    {
    }

    private void AddPlaneMesh(Vector2 uvOffset, Vector2 uvScale, int boneIndex)
    {
        var vertexIndex = vertices.Count;

        vertices.Add(new Vector3(0.5f, 0, 0.5f));
        vertices.Add(new Vector3(0.5f, 0, -0.5f));
        vertices.Add(new Vector3(-0.5f, 0, 0.5f));
        vertices.Add(new Vector3(-0.5f, 0, -0.5f));

        uv.Add(new Vector2(uvScale.x, uvScale.y) + uvOffset);
        uv.Add(new Vector2(uvScale.x, 0) + uvOffset);
        uv.Add(new Vector2(0, uvScale.y) + uvOffset);
        uv.Add(new Vector2(0, 0) + uvOffset);

        var c = new Color32((byte)boneIndex, (byte)boneIndex, (byte)boneIndex, (byte)boneIndex);
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);

        var bw = new BoneWeight { boneIndex0 = boneIndex, weight0 = 1.0f };
        boneWeights.Add(bw);
        boneWeights.Add(bw);
        boneWeights.Add(bw);
        boneWeights.Add(bw);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 3);

    }

    private void LateUpdate()
    {
        if (needUpdateMesh)
        {
            {
                // foreach(var kk in dictionary)
                var __enumerator1 = (dictionary).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var kk = __enumerator1.Current;
                    {
                        Dictionary<Material, GameObject> rendererObjects;
                        if (!rendererDictionary.TryGetValue(kk.Key, out rendererObjects))
                        {
                            rendererObjects = new Dictionary<Material, GameObject>();
                            rendererDictionary.Add(kk.Key, rendererObjects);
                        }
                        {
                            // foreach(var kV in kk.Value)
                            var __enumerator4 = (kk.Value).GetEnumerator();
                            while (__enumerator4.MoveNext())
                            {
                                var kV = __enumerator4.Current;
                                {
                                    GameObject o;
                                    if (!rendererObjects.TryGetValue(kV.Key, out o))
                                    {
                                        o = new GameObject(LayerMask.LayerToName(kk.Key) + "_" + kV.Key.name);
                                        o.transform.parent = gameObject.transform;
                                        rendererObjects.Add(kV.Key, o);
                                        o.AddComponent<SkinnedMeshRenderer>();
                                        o.layer = kk.Key;
                                    }

                                    var renderer = o.GetComponent<SkinnedMeshRenderer>();
                                    var mesh = renderer.sharedMesh;
                                    if (mesh == null)
                                    {
                                        mesh = new Mesh();
                                        mesh.MarkDynamic();
                                    }
                                    else
                                    {
                                        mesh.Clear();
                                    }

                                    vertices.Clear();
                                    uv.Clear();
                                    colors.Clear();
                                    boneWeights.Clear();
                                    triangles.Clear();

                                    bones.Clear();
                                    bindPose.Clear();
                                    {
                                        // foreach(var effect in kV.Value)
                                        var __enumerator7 = (kV.Value).GetEnumerator();
                                        while (__enumerator7.MoveNext())
                                        {
                                            var effect = __enumerator7.Current;
                                            {
                                                if (effect.Active)
                                                {
                                                    effect.ColorIndex = bones.Count;
                                                    AddPlaneMesh(effect.uvOffset, effect.uvScale, bones.Count);
                                                    bones.Add(effect.transform);
                                                    bindPose.Add(Matrix4x4.identity);
                                                }
                                            }
                                        }
                                    }

                                    if (vertices.Count == 0)
                                    {
                                        renderer.enabled = false;
                                    }
                                    else
                                    {
                                        renderer.enabled = true;
                                    }

                                    mesh.vertices = vertices.ToArray();
                                    mesh.uv = uv.ToArray();
                                    mesh.colors = colors.ToArray();
                                    mesh.boneWeights = boneWeights.ToArray();
                                    mesh.triangles = triangles.ToArray();

                                    mesh.bindposes = bindPose.ToArray();

                                    renderer.bones = bones.ToArray();
                                    renderer.sharedMesh = mesh;
                                    renderer.material = kV.Key;
                                }
                            }
                        }

                        needUpdateMesh = false;
                    }
                }
            }
        }
        {
            // foreach(var kk in dictionary)
            var __enumerator2 = (dictionary).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var kk = __enumerator2.Current;
                {
                    var dict = rendererDictionary[kk.Key];
                    {
                        // foreach(var kV in kk.Value)
                        var __enumerator6 = (kk.Value).GetEnumerator();
                        while (__enumerator6.MoveNext())
                        {
                            var kV = __enumerator6.Current;
                            {
                                GameObject o = dict[kV.Key];
                                var renderer = o.GetComponent<SkinnedMeshRenderer>();
                                {
                                    // foreach(var effect in kV.Value)
                                    var __enumerator8 = (kV.Value).GetEnumerator();
                                    while (__enumerator8.MoveNext())
                                    {
                                        var effect = __enumerator8.Current;
                                        {
                                            if (effect.Active)
                                            {
                                                renderer.material.SetColor("_Colors" + effect.ColorIndex, effect.CurrentColor);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}