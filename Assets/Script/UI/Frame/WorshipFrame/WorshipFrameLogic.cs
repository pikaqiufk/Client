using System;
#region using

using DataContract;
using EventSystem;
using UnityEngine;

#endregion

public class WorshipFrameLogic : MonoBehaviour
{
    public BindDataRoot Binding;
    public UIDragRotate ModelDrag;
    public CreateFakeCharacter ModelRoot;

    public void BtnClose()
    {
        ModelRoot.DestroyFakeCharacter();
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.WorshipFrame));
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif
        EventDispatcher.Instance.RemoveEventListener(WorshipRefreshModelView.EVENT_TYPE, OnRankRefreshModelView);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif
        EventDispatcher.Instance.AddEventListener(WorshipRefreshModelView.EVENT_TYPE, OnRankRefreshModelView);
        if (ModelRoot.Character)
        {
            ModelRoot.Character.gameObject.SetActive(true);
        }
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnRankRefreshModelView(IEvent ievent)
    {
        var e = ievent as WorshipRefreshModelView;
        var info = e.Info;
        ModelRoot.DestroyFakeCharacter();
        ItemBaseData elfData = null;
        var elfId = -1;
        if (info.Equips.ItemsChange.TryGetValue((int) eBagType.Elf, out elfData))
        {
            elfId = elfData.ItemId;
        }
        ModelRoot.Create(info.TypeId, info.EquipsModel, character => { ModelDrag.Target = character.transform; }, elfId,
            true);
    }

    public void ShowBattleUI()
    {
        EventDispatcher.Instance.DispatchEvent(new WorshipOpetion(2));
    }

    public void ShowCharacterInfo()
    {
        EventDispatcher.Instance.DispatchEvent(new WorshipOpetion(1));
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        //todo
        var controllerBase = UIManager.Instance.GetController(UIConfig.WorshipFrame);
        if (controllerBase == null)
        {
            return;
        }
        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
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

    public void WorshipBtn()
    {
        EventDispatcher.Instance.DispatchEvent(new WorshipOpetion(0));
    }

    public void WorShipCheckBtn0()
    {
        EventDispatcher.Instance.DispatchEvent(new WorshipOpetion(3));
    }

    public void WorShipCheckBtn1()
    {
        EventDispatcher.Instance.DispatchEvent(new WorshipOpetion(4));
    }
}