#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class TitleUIController : IControllerBase
{
    public TitleUIController()
    {
        EventDispatcher.Instance.AddEventListener(UIEvent_TitleItemOption.EVENT_TYPE, TitleItemOption);
        EventDispatcher.Instance.AddEventListener(FlagInitEvent.EVENT_TYPE, InitFlag);
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, UpdateFlag);

        CleanUp();
    }

    public TitleDataModel DataModel;
    public Dictionary<int, int> DicFlagId = new Dictionary<int, int>(); //flagId字典
    public Dictionary<int, List<int>> Group = new Dictionary<int, List<int>>();
    public Dictionary<int, bool> IdList = new Dictionary<int, bool>(); //<nameTitle表id,是否激活>
    private bool IsFirstOpen = false;
    public int SelectedState = (int) TitleState.NoActive;
    public int SelectedTitleId = -1;
    public int SelectGroupId = -1;
    public int SelectListId = -1;

    public enum TitleState
    {
        NoActive, //0 未激活 1激活未佩戴 2佩戴中
        ActiveNoPutOn,
        PutOn
    }

    private void GetGroupId(TitleItemDataModel item)
    {
        foreach (var ii in Group)
        {
            var count = ii.Value.Count;
            for (var i = 0; i < count; i++)
            {
                if (item.Id == ii.Value[i])
                {
                    SelectedState = item.State;
                    SelectGroupId = ii.Key;
                    SelectListId = i;
                    return;
                }
            }
        }
        SelectGroupId = -1;
        SelectListId = -1;
    }

    private TitleItemDataModel GetSetItem(int titleId)
    {
        var item = new TitleItemDataModel();
        item.Id = titleId;
        if (titleId == SelectedTitleId)
        {
            item.State = SelectedState;
        }
        else
        {
            item.State = (int) TitleState.NoActive;
        }
        GameUtils.TitleAddAttr(item, Table.GetNameTitle(titleId));
        return item;
    }

    private void InitFlag(IEvent ievent)
    {
        var keys = IdList.Keys.ToList();
        foreach (var key in keys)
        {
            var tbNameTitle = Table.GetNameTitle(key);
            if (tbNameTitle.Pos != 1)
            {
                continue;
            }
            IdList[key] = PlayerDataManager.Instance.GetFlag(tbNameTitle.FlagId);
        }
    }

    public void ListSort(List<TitleItemDataModel> list)
    {
        // var varList = list;
        list.Sort((a, b) =>
        {
            if (a.State > b.State)
            {
                if (b.State == 0)
                {
                    return -1;
                }
                if (b.State == 1)
                {
                    return a.Id - b.Id;
                }
                return -1;
            }
            if (a.State == b.State)
            {
                return a.Id - b.Id;
            }
            if (a.State == 0)
            {
                return 1;
            }
            if (a.State == 1)
            {
                return a.Id - b.Id;
            }
            return 1;
        });
    }

    private void PutOn(int index)
    {
        NetManager.Instance.StartCoroutine(SelectTitle(DataModel.Lists[index].Id, index));
    }

    public void RefleshTitle()
    {
        var list = new List<TitleItemDataModel>();

        foreach (var g in Group)
        {
            var i = g.Value.Count - 1;
            for (; i >= 0; i--)
            {
                var id = g.Value[i];
                bool active;
                if ((IdList.TryGetValue(id, out active) && active) || i == 0)
                {
                    var item = new TitleItemDataModel();
                    item.Id = id;
                    item.State = active ? (int) TitleState.ActiveNoPutOn : (int) TitleState.NoActive;
                    GameUtils.TitleAddAttr(item, Table.GetNameTitle(id));
                    list.Add(item);
                    break;
                }
            }
        }

        var titleList = PlayerDataManager.Instance.TitleList;
        for (var i = 0; i < list.Count; i++)
        {
            var ii = list[i];
            if (titleList.ContainsValue(ii.Id))
            {
                ii.State = (int) TitleState.PutOn;
            }
            else
            {
                if (ii.State != (int) TitleState.NoActive)
                {
                    ii.State = (int) TitleState.ActiveNoPutOn;
                }
            }
        }

        ListSort(list);
        DataModel.Lists = new ObservableCollection<TitleItemDataModel>(list);
    }

    public IEnumerator SelectTitle(int Id, int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.SelectTitle(Id);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    foreach (var model in DataModel.Lists)
                    {
                        if (model.State == (int) TitleState.PutOn)
                        {
                            model.State = (int) TitleState.ActiveNoPutOn;
                            break;
                        }
                    }

                    DataModel.Lists[index].State = (int) TitleState.PutOn;
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    private void SetBtnShowState()
    {
        if (SelectListId == -1 || SelectListId == 0)
        {
            DataModel.ShowBackBtn = false;
        }
        else
        {
            DataModel.ShowBackBtn = true;
        }
        if (SelectListId == -1 || (SelectListId >= Group[SelectGroupId].Count - 1))
        {
            DataModel.ShowFrontBtn = false;
        }
        else
        {
            DataModel.ShowFrontBtn = true;
        }
    }

    private void TitleItemOption(IEvent ievent)
    {
        var e = ievent as UIEvent_TitleItemOption;
        switch (e.Type)
        {
            case 0:
            {
                PutOn(e.Idx);
                break;
            }
            case 1:
            {
                DataModel.ItemSelected = DataModel.Lists[e.Idx];
                DataModel.ItemShowed = DataModel.Lists[e.Idx];
                SelectedTitleId = DataModel.Lists[e.Idx].Id;
                GetGroupId(DataModel.ItemSelected);
                SetBtnShowState();
                DataModel.ShowInfo = true;
                break;
            }
            case 2:
            {
                DataModel.ShowInfo = false;
                break;
            }
            case 3:
            {
                DataModel.ItemSelected = GetSetItem(Group[SelectGroupId][SelectListId - 1]);
                SelectListId -= 1;
                SetBtnShowState();
                break;
            }
            case 4:
            {
                DataModel.ItemSelected = GetSetItem(Group[SelectGroupId][SelectListId + 1]);
                SelectListId += 1;
                SetBtnShowState();
                break;
            }
        }
    }

    private void UpdateFlag(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        var key = DicFlagId.ContainsKey(e.Index);
        if (key)
        {
            IdList[DicFlagId[e.Index]] = e.Value;
        }

        RefleshTitle();
    }

    public void CleanUp()
    {
        DataModel = new TitleDataModel();
        IdList.Clear();
        DicFlagId.Clear();

        Table.ForeachNameTitle(table =>
        {
            if (table.Pos == -1)
            {
                return true;
            }
            IdList.Add(table.Id, false);
            if (!DicFlagId.ContainsKey(table.FlagId))
            {
                DicFlagId.Add(table.FlagId, table.Id);
            }
            return true;
        });

        Group.Clear();
        Table.ForeachNameTitle(record =>
        {
            if (record.Pos != 1)
            {
                return true;
            }

            if (record.FrontId != -1)
            {
                return true;
            }

            List<int> set;
            if (!Group.TryGetValue(record.Id, out set))
            {
                set = new List<int>();

                set.Add(record.Id);
                Group[record.Id] = set;

                while (record != null && record.PostId != -1)
                {
                    set.Add(record.PostId);
                    record = Table.GetNameTitle(record.PostId);
                }
            }

            return true;
        });
    }

    public void RefreshData(UIInitArguments data)
    {
        DataModel.ShowInfo = false;
        RefleshTitle();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public void OnShow()
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }
}