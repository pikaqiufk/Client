using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Disappear : MonoBehaviour
{
    private MeshCollider collider;
    private MeshRenderer renderer;
    private Camera camera;
    private Transform cameraTransform;
    private float duration = 0.5f;
    private CameraController controller;

    public List<GameObject> CoveredObjects;
    public List<GameObject> DisappearedObjects;

    void Start()
    {
        collider = GetComponent<MeshCollider>();
        renderer = GetComponent<MeshRenderer>();
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

#if UNITY_EDITOR
        camera = Camera.allCameras[0] ?? GameLogic.Instance.MainCamera;
#else
        camera = GameLogic.Instance.MainCamera;
        
#endif
        controller = camera.GetComponent<CameraController>();
        cameraTransform = camera.transform;
        {
            var __list1 = CoveredObjects;
            var __listCount1 = __list1.Count;
            for (int __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var o = __list1[__i1];
                {
                    if (o == null)
                    {
                        continue;
                    }

                    o.isStatic = false;

                    var r = o.renderer;
                    if (r == null)
                    {
                        continue;
                    }

                    r.enabled = false;
                }
            }
        }
    }

    private void Update()
    {
        if (renderer.enabled)
        {
            return;
        }

        if (duration > 0)
        {
            duration -= Time.deltaTime;
            return;
        }

        RaycastHit hit;
        if (!collider.Raycast(
            new Ray(cameraTransform.position - cameraTransform.forward * 100, cameraTransform.forward), out hit, controller.Length + 100))
        {
            renderer.enabled = true;

            for (int i = 0; i < renderer.gameObject.transform.childCount; i++)
            {
                renderer.gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }

            for (int i = 0; i < DisappearedObjects.Count; i++)
            {
                DisappearedObjects[i].SetActive(true);
            }

            {
                var __list2 = CoveredObjects;
                var __listCount2 = __list2.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var o = __list2[__i2];
                    {
                        if (o == null)
                        {
                            continue;
                        }

                        var r = o.renderer;
                        if (r == null)
                        {
                            continue;
                        }

                        r.enabled = false;
                    }
                }
            }
        }
    }

    // if the object is not visible, this function is not called
    // so no cost at all.
    private void OnWillRenderObject()
    {
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            return;
        }

        RaycastHit hit;
        if (collider.Raycast(
            new Ray(cameraTransform.position - cameraTransform.forward * 100, cameraTransform.forward), out hit, controller.Length + 100))
        {
            renderer.enabled = false;

            for (int i = 0; i < renderer.gameObject.transform.childCount; i++)
            {
                renderer.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < DisappearedObjects.Count; i++)
            {
                DisappearedObjects[i].SetActive(false);
            }

            {
                var __list3 = CoveredObjects;
                var __listCount3 = __list3.Count;
                for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var o = __list3[__i3];
                    {
                        if (o == null)
                        {
                            continue;
                        }

                        var r = o.renderer;
                        if (r == null)
                        {
                            continue;
                        }

                        r.enabled = true;
                    }
                }
            }
        }

    }

}
