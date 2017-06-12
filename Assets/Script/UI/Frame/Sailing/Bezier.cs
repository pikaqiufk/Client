#region using

using UnityEngine;

#endregion

public class Bezier
{
    // Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point

    // handle1 = v0 + v1

    // handle2 = v3 + v2

    public Bezier(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        p0 = v0;
        p1 = v1;
        //this.p2 = v2;
        p3 = v2;
    }

    private float Ax;
    private float Ay;
    private float Az;
    private Vector3 b0 = Vector3.zero;
    private Vector3 b1 = Vector3.zero;
    //private Vector3 b2 = Vector3.zero;
    private Vector3 b3 = Vector3.zero;
    private float Bx;
    private float By;
    private float Bz;
    private float Cx;
    private float Cy;
    private float Cz;
    public Vector3 p0;
    public Vector3 p1;
    //public Vector3 p2;
    public Vector3 p3;
    public float ti = 0f;
    // Check if p0, p1, p2 or p3 have changed

    private void CheckConstant()
    {
        if (p0 != b0 || p1 != b1 || p3 != b3) // || this.p2 != this.b2 || 
        {
            SetConstant();
            b0 = p0;
            b1 = p1;
            // this.b2 = this.p2;
            b3 = p3;
        }
    }

    // 0.0 >= t <= 1.0

    public Vector3 GetPointAtTime(float t)
    {
        CheckConstant();
        var t2 = t*t;
        var t3 = t*t*t;
        var x = Ax*t3 + Bx*t2 + Cx*t + p0.x;
        var y = Ay*t3 + By*t2 + Cy*t + p0.y;
        var z = Az*t3 + Bz*t2 + Cz*t + p0.z;
        return new Vector3(x, y, z);
    }

    private void SetConstant()
    {
        Cx = 3f*((p0.x + p1.x) - p0.x);
        // this.Bx = 3f*((this.p3.x + this.p2.x) - (this.p0.x + this.p1.x)) - this.Cx;
        Ax = p3.x - p0.x - Cx - Bx;
        Cy = 3f*((p0.y + p1.y) - p0.y);
        // this.By = 3f*((this.p3.y + this.p2.y) - (this.p0.y + this.p1.y)) - this.Cy;
        Ay = p3.y - p0.y - Cy - By;
        Cz = 3f*((p0.z + p1.z) - p0.z);
        // this.Bz = 3f*((this.p3.z + this.p2.z) - (this.p0.z + this.p1.z)) - this.Cz;
        Az = p3.z - p0.z - Cz - Bz;
    }
}