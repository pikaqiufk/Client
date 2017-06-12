using UnityEngine;
using System.Collections;
using PigeonCoopToolkit.Effects.Trails;

public class TankTranslator : MonoBehaviour
{

    public float TranslateDistance;

    public bool TrailTranslationEnabled = false;

    // Update is called once per frame
    void Update()
    {

        Vector3 translationVector = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.A))
        {
            translationVector = transform.right * TranslateDistance;

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            translationVector = -transform.right * TranslateDistance;
        }

        if (translationVector != Vector3.zero)
        {
            transform.Translate(translationVector);

            if (TrailTranslationEnabled)
            {
                {
                    var __array1 = GetComponentsInChildren<TrailRenderer_Base>();
                    var __arrayLength1 = __array1.Length;
                    for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                    {
                        var trail = (TrailRenderer_Base)__array1[__i1];
                        {
                            trail.Translate(translationVector);
                        }
                    }
                }
            }
        }

    }
}
