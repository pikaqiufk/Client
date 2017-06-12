using System;
#region using

using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

#endregion

public class DropItemController : MonoBehaviour
{
    public GameObject Effect;
    public GameObject Model;

    public void ShowEffect(bool flag)
    {
        if (flag)
        {
            Effect.SetActive(true);

            OptList<TrailRenderer_Base>.List.Clear();
            Effect.GetComponentsInChildren(OptList<TrailRenderer_Base>.List);
            {
                var __array1 = OptList<TrailRenderer_Base>.List;
                var __arrayLength1 = __array1.Count;
                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var t = __array1[__i1];
                    {
                        t.ClearSystem(false);
                        t.Emit = true;
                    }
                }
            }
            OptList<ParticleSystem>.List.Clear();
            Effect.GetComponentsInChildren(OptList<ParticleSystem>.List);
            {
                var __array2 = OptList<ParticleSystem>.List;
                var __arrayLength2 = __array2.Count;
                for (var __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var particleSystem = __array2[__i2];
                    {
                        particleSystem.Simulate(0, true, true);
                        particleSystem.Play();
                    }
                }
            }
        }
        else
        {
            Effect.SetActive(false);
        }
    }

    public void ShowModel(bool flag)
    {
        Model.SetActive(flag);
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}