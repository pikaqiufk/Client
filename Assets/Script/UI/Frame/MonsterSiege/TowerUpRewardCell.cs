using UnityEngine;
using System.Collections;
using DataTable;
using EventSystem;

public class TowerUpRewardCell : MonoBehaviour
{

    public UISprite mIcon;
    public UISprite mLingqu;
    public GameObject mEffect;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetLingqu(bool b)
    {
        mLingqu.gameObject.SetActive(b);
        if(b==true)
            mEffect.SetActive(false);
    }

    public void SetEffect(bool b)
    {
        mEffect.SetActive(b);
    }
    public void SetIcon(int iconId)
    {
        var tbIcon = Table.GetIcon(iconId);
        if (tbIcon != null)
        {
            //mIcon.atlas.name = tbIcon.Atlas;
            mIcon.spriteName = tbIcon.Sprite;
        }
    }
}
