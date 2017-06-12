using UnityEngine;
using System.Collections;

public class ConstForce : MonoBehaviour
{
    public float speed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        {
            var __array1 = GetComponents<PigeonCoopToolkit.Effects.Trails.SmokePlume>();
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var plume = __array1[__i1];
                {
                    plume.ConstantForce = transform.forward * speed;
                }
            }
        }
    }
}
