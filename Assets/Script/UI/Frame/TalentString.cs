using System;
#region using

using UnityEngine;

#endregion

public class TalentString : MonoBehaviour
{
    public static TalentString cu;
    public GameObject bg1;
    public GameObject bg2;
    private bool bOpen;
    public UIButton TalentSprite;

    public void Close()
    {
        if (bg1 != null)
        {
            bg1.SetActive(false);
        }
        if (bg2 != null)
        {
            bg2.SetActive(false);
        }

        cu = null;
        bOpen = false;
        //TalentSprite.defaultColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
        //TalentSprite.normalSprite = "xueqiu";
    }

    public void Open()
    {
        if (bg1 != null)
        {
            bg1.SetActive(true);
        }
        if (bg2 != null)
        {
            bg2.SetActive(true);
        }
        cu = this;
        bOpen = true;
        //TalentSprite.defaultColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        //TalentSprite.normalSprite = "huangxueqiu";
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

    public void TalentSpriteClick()
    {
        ///*
        if (cu != null)
        {
            if (cu == this)
            {
                Close();
                return;
            }
            cu.Close();
        }
        //*/
        if (bOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    // Update is called once per frame
    private void Update()
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