#region using

using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using GameUI;
using UnityEngine;

#endregion

public class UINewbieGuideController : IControllerBase
{
    public static bool mInit;
    public static Dictionary<int, GuidanceRecord> TableCache = new Dictionary<int, GuidanceRecord>();
	public static List<Vector2> s_AnchorPosition = new List<Vector2>(5);
    public UINewbieGuideController()
    {
	    if (0 == s_AnchorPosition.Count)
	    {
			var size = NGUITools.screenSize;
			s_AnchorPosition.Add(Vector2.zero);
			s_AnchorPosition.Add(new Vector2(-size.x / 2, size.y / 2));
			s_AnchorPosition.Add(new Vector2(size.x / 2, size.y / 2));
			s_AnchorPosition.Add(new Vector2(size.x / 2, -size.y / 2));
			s_AnchorPosition.Add(new Vector2(-size.x / 2, -size.y / 2));    
	    }

        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_UpdateGuideEvent.EVENT_TYPE, e => { UpdateNextStep(); });

        if (!mInit)
        {
            Table.ForeachGuidance(table =>
            {
                if (-1 != table.Flag && !string.IsNullOrEmpty(table.Name))
                {
                    if (!TableCache.ContainsKey(table.Flag))
                    {
                        TableCache.Add(table.Flag, table);
                    }
                    else
                    {
                        Logger.Debug("TableCache same key[{0}]", table.Flag);
                    }
                }
                return true;
            });
            mInit = true;
        }
    }

    public GuideDataModel DataModel = new GuideDataModel();

    public void HollowMoveToBagItem(eBagType bagType, int itemId)
    {
        const float ItemIconWidth = 80;
        const int ItemColumn = 5;
        var bag = PlayerDataManager.Instance.GetBag((int) bagType);
        if (null == bag)
        {
            Logger.Debug("PlayerDataManager.Instance.GetBag((int)bagType)");
            return;
        }

        var idx = -1;
        for (var i = 0; i < bag.Items.Count; i++)
        {
            var item = bag.Items[i];
            if (null == item || itemId != item.ItemId)
            {
                continue;
            }

            idx = i;
            break;
        }

        if (-1 == idx)
        {
            Logger.Debug("Can't find item[{0}]", itemId);
            return;
        }
        var offset = DataModel.PointerPos - DataModel.HollowPos;
        var temp = DataModel.HollowPos;
        var offsetX = idx%ItemColumn*ItemIconWidth;
        float offsetY = 5;
        temp.x += offsetX;
        //temp.y -= offsetY;
        DataModel.HollowPos = temp;

        DataModel.PointerPos = DataModel.HollowPos + offset;
    }

    public void OnGuilde11()
    {
        var equipId = -1;
        var type = PlayerDataManager.Instance.GetRoleId();
        if (0 == type)
        {
            equipId = 213101;
        }
        else if (1 == type)
        {
            equipId = 313101;
        }
        else if (2 == type)
        {
            equipId = 413101;
        }

        if (-1 == equipId)
        {
            return;
        }

        HollowMoveToBagItem(eBagType.Equip, equipId);
    }

    public void OnGuilde201()
    {
        var itemId = -1;
        var type = PlayerDataManager.Instance.GetRoleId();
        if (0 == type)
        {
            itemId = 20092;
        }
        else if (1 == type)
        {
            itemId = 20191;
        }
        else if (2 == type)
        {
            itemId = 20290;
        }

        if (-1 == itemId)
        {
            Logger.Error("OnGuilde201 can't find item");
            return;
        }

        HollowMoveToBagItem(eBagType.BaseItem, itemId);
    }

    public void OnGuilde6000()
    {
        var p = Vector3.zero;
        if (null != MainButtonAnimation.s_MainBtn)
        {
            p = MainButtonAnimation.s_MainBtn.GetPosition("BtnPet");
        }

        var offset = DataModel.PointerPos - DataModel.HollowPos;
        DataModel.HollowPos = p;
        DataModel.PointerPos = DataModel.HollowPos + offset;
    }

    public void OnGuilde71()
    {
        var itemId = -1;
        var type = PlayerDataManager.Instance.GetRoleId();
        if (0 == type)
        {
            itemId = 20091;
        }
        else if (1 == type)
        {
            itemId = 20191;
        }
        else if (2 == type)
        {
            itemId = 20291;
        }

        if (-1 == itemId)
        {
            Logger.Error("OnGuilde71 can't find item");
            return;
        }

        HollowMoveToBagItem(eBagType.BaseItem, itemId);
    }

    public void OnGuildeNewFunc(string btnName)
    {
	    var p = MainButton.GetOriginPosition() + DataModel.HollowPos;
        DataModel.HollowPos = p;
		DataModel.PointerPos = DataModel.HollowPos + DataModel.PointerPos;
    }

    public void OnGuildeTianFu()
    {
        var itemId = -1;
        var type = PlayerDataManager.Instance.GetRoleId();
        if (0 == type)
        {
            itemId = 20002;
        }
        else if (1 == type)
        {
            itemId = 20101;
        }
        else if (2 == type)
        {
            itemId = 20200;
        }

        if (-1 == itemId)
        {
            return;
        }
        HollowMoveToBagItem(eBagType.BaseItem, itemId);
    }

    public void OnUpdateNextStep(int id)
    {
        if (id == 11)
        {
            OnGuilde11();
        }
        else if (id == 71)
        {
            OnGuilde71();
        }
        else if (id == 201)
        {
            OnGuilde201();
        }
// 		else if (120 == id ||
// 			130 == id ||
// 			140 == id)
// 		{
// 			OnGuildeTianFu();
// 		}
        else if (TableCache.ContainsKey(id))
        {
             GuidanceRecord table = null;
             if (TableCache.TryGetValue(id, out table))
             {
                 OnGuildeNewFunc(table.Name);
             }
        }
    }

    public void UpdateNextStep()
    {
        var data = GuideManager.Instance.GetCurrentGuideData();
        if (null == data)
        {
            return;
        }
        DataModel.Label = data.Desc;
        DataModel.LabelWidth = data.FontX;
        DataModel.LabelHeight = data.FontY;
        DataModel.LabelPos = new Vector3(data.PosX, data.PoxY, 0);
        DataModel.ImageId = data.Icon;
        DataModel.ImageIdPos = new Vector3(data.IconX, data.IconY, 0);
        DataModel.ClickAnyWhereToNext = 0 != data.NextStep;
        var tb = Table.GetColorBase(data.Color);
        if (tb == null)
        {
            DataModel.Col = MColor.white;
        }
        else
        {
            DataModel.Col = new Color(tb.Red/255.0f, tb.Green/255.0f, tb.Blue/255.0f, data.Transparency/255.0f);
        }
		var orginalPoint = (data.CenterPoint >= 0 && data.CenterPoint < s_AnchorPosition.Count)
			? s_AnchorPosition[data.CenterPoint]
		    : s_AnchorPosition[0];
		
        DataModel.HollowSizeX = data.SeeSizeX;
        DataModel.HollowSizeY = data.SeeSizeY;
		//DataModel.HollowPos = new Vector3(orginalPoint.x + data.SeePosX, orginalPoint.y + data.SeePosY, 0);
		DataModel.HollowPos = new Vector3(data.SeePosX,data.SeePosY, 0);
        DataModel.ShowPointer = 0 != data.IsShowPointer;
		DataModel.PointerPos = new Vector3(orginalPoint.x + data.PointerX, orginalPoint.y + data.PointerY, 0);
        DataModel.PointerAngel = data.Rotation;
        DataModel.Skippable = 0 != data.IsSkip;

        OnUpdateNextStep(data.Id);

        Logger.Debug("UpdateNextStep id=[{0}], pos=[{1},{2}]", data.Id, DataModel.HollowPos.x, DataModel.HollowPos.y);

        EventDispatcher.Instance.DispatchEvent(new UIEvent_NextGuideEvent(data.DelayTime*0.001f));
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void CleanUp()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnShow()
    {
    }

    public FrameState State { get; set; }
}