using System;
#region using

using System.Collections.Generic;
using DataTable;
using UnityEngine;


#endregion

public class OverheadTitleFrame : MonoBehaviour
{
    private Color BattleColor = new Color(207f/255.0f, 229f/255.0f, 255f/255.0f);
    public List<UILabel> MyLabel;
    public List<UISprite> MyPic;
    public int TitleIndex = -1;

    public void SetBattleColor(int camp)
    {
        switch (camp)
        {
            case 7:
            {
                BattleColor = GameUtils.GetTableColor(503);
            }
                break;
            case 8:
            {
                BattleColor = GameUtils.GetTableColor(502);
            }
                break;
            case 9:
            {
                BattleColor = GameUtils.GetTableColor(501);
            }
                break;
            default:
            {
                BattleColor = GameUtils.GetTableColor(0);
            }
                break;
        }
        if (TitleIndex == 0)
        {
            for (var i = 0; i < MyLabel.Count; i++)
            {
                MyLabel[i].color = BattleColor;
            }
        }
    }

    public void SetTitle(NameTitleRecord record, string str, ref bool active)
    {
        for (var i = 0; i < MyLabel.Count; i++)
        {
            if (record.MyLabel[i] == "{AllianceName}")
            {
                if (str == "")
                {
                    active = false;
                    return;
                }
                MyLabel[i].text = str;
            }
            else
            {
                MyLabel[i].text = record.MyLabel[i];
            }

            if (record.MyLabel[i] == "")
            {
                if (MyLabel[i].gameObject.activeSelf)
                {
                    MyLabel[i].gameObject.SetActive(false);
                }
                continue;
            }
            if (!MyLabel[i].gameObject.activeSelf)
            {
                MyLabel[i].gameObject.SetActive(true);
            }

            MyLabel[i].fontSize = record.MyFont[i];
            if (record.MyFontColorA[i] != -1)
            {
                MyLabel[i].gradientTop = GameUtils.GetTableColor(record.MyFontColorA[i]);
            }
            if (record.MyFontColorB[i] != -1)
            {
                MyLabel[i].gradientBottom = GameUtils.GetTableColor(record.MyFontColorB[i]);
            }
            if (record.StrokeColor[i] != -1)
            {
                if (record.EffectType[i] != -1)
                {
                    MyLabel[i].effectStyle = (UILabel.Effect) record.EffectType[i];
                    MyLabel[i].effectColor = GameUtils.GetTableColor(record.StrokeColor[i]);
                }
            }
        }
        for (var i = 0; i < MyPic.Count; i++)
        {
            if (record.PicId[i] != -1)
            {
//                 var tbIcon = Table.GetIcon(record.PicId[i]);
//                 MyPic[i].atlas.name = tbIcon.Atlas;
//                 MyPic[i].spriteName = tbIcon.Sprite;
                GameUtils.SetSpriteIcon(MyPic[i], record.PicId[i]);
                if (!MyPic[i].gameObject.activeSelf)
                {
                    MyPic[i].gameObject.SetActive(true);
                }
            }
            else
            {
                if (MyPic[i].gameObject.activeSelf)
                {
                    MyPic[i].gameObject.SetActive(false);
                }
            }
        }
        if (TitleIndex == 0)
        {
            for (var i = 0; i < MyLabel.Count; i++)
            {
                MyLabel[i].color = BattleColor;
            }
        }
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