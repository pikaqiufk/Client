using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using EpPathFinding.cs;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CircleCollider : MonoBehaviour, ICollider
{
    public float Radius = 1.0f;

    PathFinder mFinder;
    void Start()
    {
        var o = GameObject.Find("ServerObstacleRoot");
        mFinder = o.GetComponent<PathFinder>();

        mFinder.Obstacle.AddCollider(this);
    }

    void OnDestroy()
    {
        mFinder.Obstacle.RemoveCollider(this);
    }

    public bool IsInside(float x, float y)
    {
        if ((transform.position.xz() - new Vector2(x/2.0f, y/2.0f)).sqrMagnitude < Radius *Radius)
        {
            return true;
        }

        return false;
    }
}
