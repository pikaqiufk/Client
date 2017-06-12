using System;
#region using

using UnityEngine;

#endregion

public class FaceCellLogic : MonoBehaviour
{
    public int FaceId { get; set; }

    public void OnClickFace()
    {
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
}