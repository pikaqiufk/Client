using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JiangliItemRankIconLogic : MonoBehaviour {

	public UILabel label;
    public UISprite sprite;
    public string rank;
    public int ICionId
    {
        set
        {
            
           
            if (value == 0)
            {
                label.cachedGameObject.SetActive(true);
                sprite.cachedGameObject.SetActive(false);
                if (!string.IsNullOrEmpty(rank))
                {
                    //if (rank.Count > 0)
                   // {
                        label.text = rank;// string.Format("{0}-{1}", rank[0].ToString(), rank[rank.Count - 1].ToString());
                   // }
                }
            }
            else
            {
               // if (!string.IsNullOrEmpty(rank))
                {
                    sprite.cachedGameObject.SetActive(true);
                    label.cachedGameObject.SetActive(false);
                }
            }
        }
    }
}
