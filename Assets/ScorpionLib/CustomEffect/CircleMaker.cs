using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CircleMaker : MonoBehaviour {

    static protected Vector2[] mTempPos = new Vector2[4];
    static protected Vector2[] mTempUVs = new Vector2[4];
	// Use this for initialization
    public bool IsInvert = false;
    private float mFillAmount = 1.0f;
    public float FillAmount
    {
        get { return mFillAmount; }
        set
        {
            if (mFillAmount == value) return;
            mFillAmount = value;
            UpdateMesh();
        }
    }
    public Color Color = Color.white;

    private Mesh mMesh;

    private List<Vector3> verts = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>(); 
    private List<Color32> cols = new List<Color32>();
    private List<int> indices = new List<int>();

    private MeshFilter mFilter;
    private MeshRenderer mRenderer;

    //模型的半径
    public float Radius = 1f;

	void Awake () {

        mMesh = new Mesh();
        mMesh.MarkDynamic();

	    mFilter = GetComponent<MeshFilter>();
	    mFilter.mesh = mMesh;
	    mRenderer = GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void UpdateMesh ()
    {
        Vector4 v = new Vector4(-Radius, -Radius, Radius, Radius);
        Vector4 u = new Vector4(0, 0, 1, 1);

        var index = 0;

        for (int corner = 0; corner < 4; ++corner)
        {
            float fx0, fx1, fy0, fy1;

            if (corner < 2) { fx0 = 0f; fx1 = 0.5f; }
            else { fx0 = 0.5f; fx1 = 1f; }

            if (corner == 0 || corner == 3) { fy0 = 0f; fy1 = 0.5f; }
            else { fy0 = 0.5f; fy1 = 1f; }

            mTempPos[0].x = Mathf.Lerp(v.x, v.z, fx0);
            mTempPos[1].x = mTempPos[0].x;
            mTempPos[2].x = Mathf.Lerp(v.x, v.z, fx1);
            mTempPos[3].x = mTempPos[2].x;

            mTempPos[0].y = Mathf.Lerp(v.y, v.w, fy0);
            mTempPos[1].y = Mathf.Lerp(v.y, v.w, fy1);
            mTempPos[2].y = mTempPos[1].y;
            mTempPos[3].y = mTempPos[0].y;

            mTempUVs[0].x = Mathf.Lerp(u.x, u.z, fx0);
            mTempUVs[1].x = mTempUVs[0].x;
            mTempUVs[2].x = Mathf.Lerp(u.x, u.z, fx1);
            mTempUVs[3].x = mTempUVs[2].x;

            mTempUVs[0].y = Mathf.Lerp(u.y, u.w, fy0);
            mTempUVs[1].y = Mathf.Lerp(u.y, u.w, fy1);
            mTempUVs[2].y = mTempUVs[1].y;
            mTempUVs[3].y = mTempUVs[0].y;

            var val = IsInvert
                ? FillAmount*4f - NGUIMath.RepeatIndex(corner + 2, 4)
                : FillAmount*4f - (3 - NGUIMath.RepeatIndex(corner + 2, 4));

            if (RadialCut(mTempPos, mTempUVs, Mathf.Clamp01(val), IsInvert, NGUIMath.RepeatIndex(corner + 2, 4)))
            {
                for (int i = 0; i < 4; ++i)
                {
                    verts.Add(new Vector3(mTempPos[i].x, 0, mTempPos[i].y));
                    uvs.Add(mTempUVs[i]);
                    cols.Add(Color);
                }

                indices.Add(index + 0);
                indices.Add(index + 1);
                indices.Add(index + 2);
                indices.Add(index + 0);
                indices.Add(index + 2);
                indices.Add(index + 3);

                index += 4;
            }
        }

        mMesh.Clear();

        mMesh.vertices = verts.ToArray();
        mMesh.uv = uvs.ToArray();
        mMesh.colors32 = cols.ToArray();
        mMesh.triangles = indices.ToArray();

        verts.Clear();
        uvs.Clear();
        cols.Clear();
        indices.Clear();
    }

    static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
    {
        // Nothing to fill
        if (fill < 0.001f) return false;

        // Even corners invert the fill direction
        if ((corner & 1) == 1) invert = !invert;

        // Nothing to adjust
        if (!invert && fill > 0.999f) return true;

        // Convert 0-1 value into 0 to 90 degrees angle in radians
        float angle = Mathf.Clamp01(fill);
        if (invert) angle = 1f - angle;
        angle *= 90f * Mathf.Deg2Rad;

        // Calculate the effective X and Y factors
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        RadialCut(xy, cos, sin, invert, corner);
        RadialCut(uv, cos, sin, invert, corner);
        return true;
    }
    static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
    {
        int i0 = corner;
        int i1 = NGUIMath.RepeatIndex(corner + 1, 4);
        int i2 = NGUIMath.RepeatIndex(corner + 2, 4);
        int i3 = NGUIMath.RepeatIndex(corner + 3, 4);

        if ((corner & 1) == 1)
        {
            if (sin > cos)
            {
                cos /= sin;
                sin = 1f;

                if (invert)
                {
                    xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                    xy[i2].x = xy[i1].x;
                }
            }
            else if (cos > sin)
            {
                sin /= cos;
                cos = 1f;

                if (!invert)
                {
                    xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                    xy[i3].y = xy[i2].y;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }

            if (!invert) xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
            else xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
        }
        else
        {
            if (cos > sin)
            {
                sin /= cos;
                cos = 1f;

                if (!invert)
                {
                    xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                    xy[i2].y = xy[i1].y;
                }
            }
            else if (sin > cos)
            {
                cos /= sin;
                sin = 1f;

                if (invert)
                {
                    xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                    xy[i3].x = xy[i2].x;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }

            if (invert) xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
            else xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
        }
    }
}
