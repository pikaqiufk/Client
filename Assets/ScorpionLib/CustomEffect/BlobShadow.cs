using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class BlobShadow : MonoBehaviour
{
    //cache
    [HideInInspector]
    public Transform transform;
    public float size;

    //static
    private static Dictionary<int, GameObject> rendererGameObjects = new Dictionary<int, GameObject>();
    private static Dictionary<int, BatchedBlobShadowRender> batchedBlobShadowRenders = new Dictionary<int, BatchedBlobShadowRender>();

    public static void ActiveAllRenders(bool act)
    {
        {
            // foreach(var renderer in batchedBlobShadowRenders)
            var __enumerator2 = (batchedBlobShadowRenders).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var renderer = __enumerator2.Current;
                {
                    if (renderer.Value != null)
                    {
                        renderer.Value.gameObject.SetActive(act);
                    }
                }
            }
        }
    }

    void Awake()
    {
        transform = gameObject.transform;
    }

    // Use this for initialization
    void Start()
    {
    }
    void OnEnable()
    {
        GameObject rendererGameObject;
        BatchedBlobShadowRender batchedBlobShadowRender;
        if (rendererGameObjects.TryGetValue(gameObject.layer, out rendererGameObject) && rendererGameObject)
        {
            batchedBlobShadowRender = batchedBlobShadowRenders[gameObject.layer];
        }
        else
        {
            rendererGameObject = new GameObject("BatchedBlobShadowRender" + gameObject.layer);
            batchedBlobShadowRender = rendererGameObject.AddComponent<BatchedBlobShadowRender>();
            var renderer = rendererGameObject.AddComponent<SkinnedMeshRenderer>();

            rendererGameObject.layer = gameObject.layer;

            rendererGameObjects[gameObject.layer] = rendererGameObject;
            batchedBlobShadowRenders[gameObject.layer] = batchedBlobShadowRender;

            rendererGameObject.SetActive(GameSetting.Instance.ShowBlobShadow);

            renderer.updateWhenOffscreen = true;
            // 材质加载好之前先不显示
            batchedBlobShadowRender.MaterialLoaded = false;
            ResourceManager.PrepareResource<Material>(Resource.Material.BlobShadow, (mat) =>
            {
                renderer.material = mat;
                batchedBlobShadowRender.MaterialLoaded = true;
            });
        }

        if (batchedBlobShadowRender)
            batchedBlobShadowRender.Add(this);
    }
    void OnDisable()
    {
        var batchedBlobShadowRender = batchedBlobShadowRenders[gameObject.layer];
        if (batchedBlobShadowRender)
            batchedBlobShadowRender.Remove(this);
    }
}

public class BatchedBlobShadowRender : MonoBehaviour
{
    private bool needUpdateMesh = true;
    HashSet<BlobShadow> shadows = new HashSet<BlobShadow>();
    public void Add(BlobShadow shadow)
    {
        shadows.Add(shadow);
        needUpdateMesh = true;
        if (gameObject && GameSetting.Instance.ShowBlobShadow)
            gameObject.SetActive(true);
    }

    public void Remove(BlobShadow shadow)
    {
        shadows.Remove(shadow);
        needUpdateMesh = true;

        if (shadows.Count == 0 && gameObject)
            gameObject.SetActive(false);
    }

    public bool MaterialLoaded = false;
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

    private void AddPlaneMesh(int boneIndex, float size)
    {
        var vertexIndex = vertices.Count;

        vertices.Add(new Vector3(size, 0.2f, size));
        vertices.Add(new Vector3(size, 0.2f, -size));
        vertices.Add(new Vector3(-size, 0.2f, size));
        vertices.Add(new Vector3(-size, 0.2f, -size));

        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(0, 1));
        uv.Add(new Vector2(0, 0));

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
        if (needUpdateMesh && MaterialLoaded)
        {
            GameObject o = gameObject;

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
                // foreach(var shadow in shadows)
                var __enumerator1 = (shadows).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var shadow = __enumerator1.Current;
                    {
                        AddPlaneMesh(bones.Count, shadow.size);
                        bones.Add(shadow.transform);
                        bindPose.Add(Matrix4x4.identity);
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

            //renderer.localBounds = new Bounds(ObjManager.Instance.MyPlayer.Position, Vector3.one);

            needUpdateMesh = false;
        }
    }
}