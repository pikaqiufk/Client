using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshEffect : MonoBehaviour
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
    [HideInInspector]
    public MeshRenderer renderer;
    [HideInInspector]
    public MeshFilter meshFilter;

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
    private static BatchedMeshEffectRender batchedMeshEffectRender;

    void Awake()
    {
        transform = gameObject.transform;
        renderer = gameObject.GetComponent<MeshRenderer>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        lifeTime = 0;
        scale = transform.localScale;
    }

    // Use this for initialization
    void Start()
    {
        if (rendererGameObject == null)
        {
            rendererGameObject = new GameObject("BatchedMeshEffectRenderer");
            batchedMeshEffectRender = rendererGameObject.AddComponent<BatchedMeshEffectRender>();
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

            batchedMeshEffectRender.Remove(material, this);
            return;
        }

        if (lifeTime < StartDelay)
        {
            return;
        }

        if (!Active)
        {
            batchedMeshEffectRender.Add(material, this);
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
        batchedMeshEffectRender.Remove(material, this);
    }
}

public class BatchedMeshEffectRender : MonoBehaviour
{

    private Dictionary<int, Dictionary<Material, HashSet<MeshEffect>>> dictionary = new Dictionary<int, Dictionary<Material, HashSet<MeshEffect>>>();
    private Dictionary<int, Dictionary<Material, GameObject>> rendererDictionary = new Dictionary<int, Dictionary<Material, GameObject>>();
    private bool needUpdateMesh = true;

    public void Add(Material mat, MeshEffect effect)
    {
        Dictionary<Material, HashSet<MeshEffect>> dict;
        if (!dictionary.TryGetValue(effect.gameObject.layer, out dict))
        {
            dict = new Dictionary<Material, HashSet<MeshEffect>>();
            dictionary.Add(effect.gameObject.layer, dict);
        }

        HashSet<MeshEffect> set;
        if (!dict.TryGetValue(mat, out set))
        {
            set = new HashSet<MeshEffect>();
            dict.Add(mat, set);
        }

        set.Add(effect);

        effect.renderer.enabled = false;

        needUpdateMesh = true;
    }

    public void Remove(Material mat, MeshEffect effect)
    {
        Dictionary<Material, HashSet<MeshEffect>> dict;
        if (dictionary.TryGetValue(effect.gameObject.layer, out dict))
        {
            HashSet<MeshEffect> set;
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

    private void AddMesh(Vector2 uvOffset, Vector2 uvScale, int boneIndex, Mesh mesh)
    {
        var vertexIndex = vertices.Count;

        vertices.AddRange(mesh.vertices);
        var uvs = mesh.uv;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            uv.Add(Vector2.Scale(uvs[i], uvScale) + uvOffset);
        }

        var c = new Color32((byte)boneIndex, (byte)boneIndex, (byte)boneIndex, (byte)boneIndex);
        var bw = new BoneWeight { boneIndex0 = boneIndex, weight0 = 1.0f };
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            colors.Add(c);
            boneWeights.Add(bw);
        }

        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            triangles.Add(mesh.triangles[i] + vertexIndex);
        }
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
                                                    AddMesh(effect.uvOffset, effect.uvScale, bones.Count, effect.meshFilter.sharedMesh);
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
                                int i = 0;
                                {
                                    // foreach(var effect in kV.Value)
                                    var __enumerator8 = (kV.Value).GetEnumerator();
                                    while (__enumerator8.MoveNext())
                                    {
                                        var effect = __enumerator8.Current;
                                        {
                                            if (effect.Active)
                                            {
                                                renderer.material.SetColor("_Colors" + i, effect.CurrentColor);
                                                i++;
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