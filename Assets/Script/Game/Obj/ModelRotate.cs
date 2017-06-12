using System;
using UnityEngine;


public class ModelRotate : MonoBehaviour
{
    public Transform Target;
    public Vector3 localRotation;
    public float Speed = 1.0f;

    public void UseRotation()
    {
        Target.localRotation = Quaternion.Euler(localRotation);
    }

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

    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (Target == null)
            return;

        var angles = Target.localRotation.eulerAngles;
        angles.y += Time.deltaTime * Speed;
        Target.localRotation = Quaternion.Euler(angles);
        //var rotate  = Time.deltaTime * 100;  
        //Target.Rotate(Vector3.up * rotate, Space.Self);

    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
}
