using UnityEngine;
using System.Collections;
using BehaviourMachine;
using Mono.Cecil.Cil;

[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
public class ScorpionDecal : MonoBehaviour
{
    private Mesh mBackMesh;
    private Mesh mMesh;
    private MeshFilter mFilter;
    private MeshRenderer mRenderer;
    private Transform mTransform;

    public int Quality = 3;

    public float Width = 1;
    public float Height = 1;

    public Vector3 Offset = new Vector3(0, 0.2f, 0);

    public float Lifetime = 1;
    public float Delay = 0;
    public bool UsingSimpleSize = true;
    public float SimpleSizeOverLifeStart = 1;
    public float SimpleSizeOverLifeEnd = 1;
    public AnimationCurve SizeOverLife = new AnimationCurve();
    public bool UsingSimpleColor = true;
    public Color SimpleColorOverLifeStart = Color.white;
    public Color SimpleColorOverLifeEnd = Color.white;
    public Gradient ColorOverLife;

    private float mLifeTime = 0;
    private int mLayerMask = -1;

    private void Awake()
    {
        mFilter = gameObject.GetComponent<MeshFilter>();
        mRenderer = gameObject.GetComponent<MeshRenderer>();
        mTransform = gameObject.transform;
        mMesh = new Mesh();
        mFilter.sharedMesh = mMesh;
        mBackMesh = new Mesh();
        mLayerMask = 1 << LayerMask.NameToLayer("ShadowReceiver");
    }

    private void OnEnable()
    {
        mLifeTime = 0;
        mRenderer.enabled = false;

        if (Quality < 3)
        {
            Quality = 3;
        }

        if (Quality%2 == 0)
        {
            Quality = Quality - 1;
        }

        if (vertices != null && vertices.Length == Quality*Quality)
        {
            return;
        }

        vertices = new Vector3[Quality*Quality];
        uv = new Vector2[Quality*Quality];
        colors = new Color[Quality*Quality];
        triangles = new int[(Quality - 1)*(Quality - 1)*2*3];

        var widthStep = 1.0f/(Quality - 1);
        var heightStep = 1.0f/(Quality - 1);
        var index = 0;
        for (int i = 0; i < Quality; i++)
        {
            for (int j = 0; j < Quality; j++)
            {
                var u = i*widthStep;
                var v = j*heightStep;

                colors[index] = new Color32(255, 255, 255, 255);
                uv[index++] = new Vector2(u, v);
            }
        }

        index = 0;
        for (int i = 0; i < Quality - 1; i++)
        {
            for (int j = 0; j < Quality - 1; j++)
            {
                var n = i + j*Quality;
                triangles[index++] = (n);
                triangles[index++] = (n + 1);
                triangles[index++] = (n + Quality);
                triangles[index++] = (n + Quality);
                triangles[index++] = (n + 1);
                triangles[index++] = (n + Quality + 1);
            }
        }
    }

    private Vector3[] vertices;
    private Vector2[] uv;
    private Color[] colors;
    private int[] triangles;

    private Vector3 mLastPosition;
    private Quaternion mLastOrientation;
    private float mLastScale;
    private Color mLastColor;

    // Update is called once per frame
    private void Update()
    {
        mLifeTime += Time.deltaTime;

        if (mLifeTime > Lifetime)
        {
            mRenderer.enabled = false;
            return;
        }

        if (mLifeTime > Delay)
        {
            mRenderer.enabled = true;
        }
        else
        {
            return;
        }


        var scale = 1.0f;
        if (UsingSimpleSize)
        {
            scale = Mathf.Lerp(SimpleSizeOverLifeStart, SimpleSizeOverLifeEnd, (mLifeTime - Delay)/Lifetime);
        }
        else
        {
            scale = SizeOverLife.Evaluate((mLifeTime - Delay)/Lifetime);
        }


        Color color;
        if (UsingSimpleColor)
        {
            color = Color.Lerp(SimpleColorOverLifeStart, SimpleColorOverLifeEnd, (mLifeTime - Delay)/Lifetime);
        }
        else
        {
            color = ColorOverLife.Evaluate((mLifeTime - Delay)/Lifetime);
        }

        if (Mathf.Approximately(mLastScale, scale) && mLastColor == color && mTransform.position == mLastPosition &&
            mTransform.rotation == mLastOrientation)
        {
            return;
        }


        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }

        var p = Vector3.zero;
        var r = (Quality - 1)/2;
        var widthStep = Width*scale/(Quality - 1);
        var heightStep = Height*scale/(Quality - 1);
        var ray = new Ray(Vector3.down, Vector3.down);
        var temp = new Vector3(0, 2, 0);
        var hit = new RaycastHit();
        var index = 0;
        for (int i = -r; i <= r; i++)
        {
            for (int j = -r; j <= r; j++)
            {
                p.x = i*widthStep;
                p.z = j*heightStep;
                temp.x = p.x;
                temp.z = p.z;

                var origin = mTransform.localToWorldMatrix.MultiplyPoint3x4(temp);
                ray.origin = origin;
                if (Physics.Raycast(ray, out hit, 4, mLayerMask))
                {
                    vertices[index++] = mTransform.worldToLocalMatrix.MultiplyPoint3x4(hit.point) + Offset;
                }
                else
                {
                    vertices[index++] = p + Offset;
                }
            }
        }

        mBackMesh.vertices = vertices;
        mBackMesh.uv = uv;
        mBackMesh.colors = colors;
        mBackMesh.triangles = triangles;

        mLastPosition = mTransform.position;
        mLastOrientation = mTransform.rotation;
        mLastScale = scale;
        mLastColor = color;

        // swap buffer
        var tmpMesh = mBackMesh;
        mBackMesh = mMesh;
        mMesh = tmpMesh;

        mFilter.sharedMesh = mMesh;
    }

    private void OnDestroy()
    {
        if (mMesh)
        {
            Destroy(mMesh);
        }

        if (mBackMesh)
        {
            Destroy(mBackMesh);
        }
    }
}
