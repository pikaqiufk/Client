using UnityEngine;
using System.Collections;

public class BlurBackground : UITexture {

	// Use this for initialization
    void Start()
    {
        base.Start();
        try
        {
            UIRoot root = gameObject.transform.root.GetComponent<UIRoot>();
            if (root != null)
            {
                var s = (float)root.activeHeight / Screen.height;
                width = Mathf.CeilToInt(Screen.width * s);
                height = Mathf.CeilToInt(Screen.height * s);
            }

            mType = Type.Sliced;
        }
        catch
        {
            // do nothing. avoid throw exception in editor when game is not running.
        }
    }

    void OnEnable()
    {
        try
        {
            mainTexture = BlurTextureCreator.BlurredTexture;
            base.OnEnable();
        }
        catch
        {
            // do nothing. avoid throw exception in editor when game is not running.
        }
    }

    void OnDisable()
    {
        if (mainTexture && mainTexture is RenderTexture)
        {
            RenderTexture.ReleaseTemporary((RenderTexture) mainTexture);
            mainTexture = null;
        }
        base.OnDisable();
    }
}
