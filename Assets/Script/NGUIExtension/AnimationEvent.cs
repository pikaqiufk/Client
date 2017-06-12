#region using

using UnityEngine;

#endregion

public class AnimationEvent : MonoBehaviour
{
    public void OnPlayOver()
    {
        Destroy(gameObject);
    }

    public void PlayOverDeactive()
    {
        gameObject.SetActive(false);
    }
}