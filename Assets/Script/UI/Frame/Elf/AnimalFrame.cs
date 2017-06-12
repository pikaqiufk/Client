#region using

using System;
using System.Collections;
using System.Collections.Generic;
using AnimationOrTween;
using ClientDataModel;
using DataTable;
using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class AnimalFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    private GameObject baoXiangAni; //宝箱动画
	    public GameObject BaoXiangpos; //宝箱动画
	    public BindDataRoot Binding;
	    public GameObject BlurObject; //模糊层
	    private GameObject cellClick;
	    private ElfDataModel DataModel;
	    public List<UIEventTrigger> DrawShowObjs;
	    public GameObject ElfLevelUpParticle; //精灵升级的粒子特效
	    public UIScrollViewSimple ElfScrollView;
	    public Animation FormationAnimation; //精灵阵型界面的动画
	    //用来获取飞行动画的终点
	    public List<CreateFakeCharacter> FormationModelRoot;
	    public List<ParticleScaler> FormationParticles; //阵法升级的粒子特效
	    public Transform Grid;
	    public UIScrollView InfoScroll;
	    public List<StackLayout> LayoutList;
	    private Coroutine LevelUpCoroutine;
	    public List<UIToggle> MenuList;
	    private bool flag;
	    private float lastScrollViewOffset;
	    private Vector3 lastScrollViewPos;
	    public UIDragRotate ModelDrag;
	    public CreateFakeCharacter ModelRoot;
	    public Animation TenAnimation;
	    public List<GameObject> TenDrawObj; //十连抽父物体
	    public GameObject TenGetOrigPosition; //十连抽初始位置
	    private List<GameObject> varTenList; //动态加载的十连抽prefab
	
	    private IEnumerator EndAnimation(float delay, GameObject obj)
	    {
	        yield return new WaitForSeconds(delay);
	        obj.SetActive(false);
	        LevelUpCoroutine = null;
	    }
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        //foreach (var f in FormationModelRoot)
	        //{
	        //    var fly = f.GetComponent<FlyLogic>();
	        //    fly.Init();
	        //}
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void CreateFormatAnimalModel(int index)
	    {
	        if (index >= FormationModelRoot.Count)
	        {
	            Logger.Error("In OnFormationElfModelRefresh(), e.Index >= FormationModelRoot.Count!!!!!!!");
	            return;
	        }
	        var root = FormationModelRoot[index];
	        if (root != null)
	        {
	            root.DestroyFakeCharacter();
	        }
	        var elfId = DataModel.Formations[index].ElfData.ItemId;
	        var tbElf = Table.GetElf(elfId);
	        if (tbElf == null)
	        {
	            return;
	        }
	        var dataId = tbElf.ElfModel;
	        if (dataId == -1)
	        {
	            return;
	        }
	        var tableNpc = Table.GetCharacterBase(dataId);
	        if (null == tableNpc)
	        {
	            Logger.Error("In CreateFormationElfModel(), null == tableNpc!!!!!!!");
	            return;
	        }
	
	        var offset = tableNpc.CameraHeight/10000.0f;
	        root.Create(dataId, null, character =>
	        {
	            character.SetScale(tableNpc.CameraMult/10000.0f);
	            character.ObjTransform.localPosition = new Vector3(0, offset, 0);
	        });
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (!flag)
	        {
	            return;
	        }
	        flag = false;
	        var listCount3 = LayoutList.Count;
	        for (var i = 0; i < listCount3; ++i)
	        {
	            var layout = LayoutList[i];
	            if (layout != null)
	            {
	                layout.ResetLayout();
	            }
	        }
	    
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	}
	
	    private void LookScrollVievCenter(IEvent ievent)
	    {
	        var e = ievent as UIEvent_ElfSetGridLookIndex;
	        if (e.Index == -1)
	        {
	            ElfScrollView.MoveToOffset(lastScrollViewPos, lastScrollViewOffset);
	        }
	        else
	        {
	            ElfScrollView.SetLookIndex(e.Index, false);
	        }
	    }
	
	    public void OnClickDecompose()
	    {
	        lastScrollViewOffset = ElfScrollView.oldoffset;
	        lastScrollViewPos = ElfScrollView.transform.localPosition;
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(12));
	    }
        public void OnClick_EnhanceStar()
        {
            EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(13));
        }
	
	    public void OnClickDisFight()
	    {
	        var e = new ElfOperateEvent(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickFight()
	    {
	        var e = new ElfOperateEvent(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickFormationLevelUp()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(10));
	    }
	
	    public void OnClickGetOneShow()
	    {
	        //BlurObject.SetActive(false);
	        var e = new ElfGetOneShowEvent(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickGetTenShow()
	    {
	        //BlurObject.SetActive(false);
	        var e = new ElfGetOneShowEvent(10);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickLevelUp()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(11));
	    }
	
	    private void OnClickMenu(int index)
	    {
	        if (index == 1)
	        {
	            SetAnimalDragEnable(true);
	        }
	        else
	        {
	            SetAnimalDragEnable(false);
	        }
	    }
	
	    public void OnClickShow()
	    {
	        var e = new ElfOperateEvent(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickShowClose()
	    {
	        var e = new ElfShowCloseEvent();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnClickShowDrawItems(int index)
	    {
	        var e = new ElfOperateEvent(63 + index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnCloseClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ElfUI));
	    }
	
	    public void OnCloseElfList()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(22));
	    }
	
	    public void OnCloseFormationInfo()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(21));
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        EventDispatcher.Instance.RemoveEventListener(ElfPlayAnimationEvent.EVENT_TYPE, OnAnimalPlayAnimation);
	        EventDispatcher.Instance.RemoveEventListener(ElfGetDrawResultBack.EVENT_TYPE, DrawAnimationStart);
	        //EventDispatcher.Instance.RemoveEventListener(UIEvent_ElfBaoXiangOverEvent.EVENT_TYPE, BaoxiangAnimOver);
	
	        if (ModelRoot != null)
	        {
	            ModelRoot.DestroyFakeCharacter();
	        }
	        foreach (var formationRoot in FormationModelRoot)
	        {
	            formationRoot.DestroyFakeCharacter();
	        }
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	
	        EventDispatcher.Instance.RemoveEventListener(ElfModelRefreshEvent.EVENT_TYPE, OnAnimalModelRefresh);
	        EventDispatcher.Instance.RemoveEventListener(FormationElfModelRefreshEvent.EVENT_TYPE, OnFormationAnimalModelRefresh);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_ElfSetGridLookIndex.EVENT_TYPE, LookScrollVievCenter);
	        EventDispatcher.Instance.RemoveEventListener(FormationLevelupEvent.EVENT_TYPE, OnFormationLvUp);
	        EventDispatcher.Instance.RemoveEventListener(ElfLevelupEvent.EVENT_TYPE, OnAnimalLevelup);
	        EventDispatcher.Instance.RemoveEventListener(ElfCell1ClickEvent.EVENT_TYPE, OnElfCell1Click);
	        EventDispatcher.Instance.RemoveEventListener(ElfFlyEvent.EVENT_TYPE, OnAnimalFly);
	
	        if (ModelRoot != null)
	        {
	            ModelRoot.DestroyFakeCharacter();
	        }
	        foreach (var formationRoot in FormationModelRoot)
	        {
	            formationRoot.DestroyFakeCharacter();
	        }
	        while (varTenList.Count > 10)
	        {
	            Destroy(varTenList[0]);
	            varTenList.RemoveAt(0);
	        }
	        Binding.RemoveBinding();
	        DataModel.IsAnimating = false;
	        Destroy(baoXiangAni);
	        baoXiangAni = null;
	        BlurObject.SetActive(false);
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnElfCell1Click(IEvent ievent)
	    {
	        var e = ievent as ElfCell1ClickEvent;
	        cellClick = e.Go;
	    }
	
	    public void OnElfCellClicked0()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(50));
	    }
	
	    public void OnElfCellClicked1()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(51));
	    }
	
	    public void OnElfCellClicked2()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(52));
	    }
	
	    private void OnAnimalFly(IEvent ievent)
	    {
	        var e = ievent as ElfFlyEvent;
	
	        if (DataModel.IsAnimating)
	        {
	            return;
	        }
	        DataModel.IsAnimating = true;
	
	        var inverse = false;
	        var swap = false;
	        if (e.FromIdx < 3 && e.ToIdx < 3)
	        {
	            swap = true;
	        }
	        else
	        {
	            inverse = e.FromIdx > e.ToIdx;
	        }
	
	        if (swap)
	        {
	            var fromNode = FormationModelRoot[e.FromIdx].GetComponent<FlyAniLogic>();
	            var toNode = FormationModelRoot[e.ToIdx].GetComponent<FlyAniLogic>();
	
	            fromNode.StartFly(toNode.transform.position, false, false, () =>
	            {
	                DataModel.IsAnimating = false;
	                if (e.NeedOverEvent)
	                {
	                    EventDispatcher.Instance.DispatchEvent(new ElfFlyOverEvent(e.FromIdx, e.ToIdx));
	                }
	            });
	            toNode.StartFly(fromNode.transform.position, false, false, null);
	        }
	        else
	        {
	            var flyNodeIdx = Math.Min(e.FromIdx, e.ToIdx);
	            var flyNode = FormationModelRoot[flyNodeIdx].GetComponent<FlyAniLogic>();
	            flyNode.StartFly(cellClick.transform.position, true, inverse, () =>
	            {
	                DataModel.IsAnimating = false;
	                if (e.NeedOverEvent)
	                {
	                    EventDispatcher.Instance.DispatchEvent(new ElfFlyOverEvent(e.FromIdx, e.ToIdx));
	                }
	            });
	        }
	    }
	
	    private void OnAnimalLevelup(IEvent ievent)
	    {
	        //var p = ElfLevelUpParticle;
	        // p.Play();
	        if (LevelUpCoroutine != null)
	        {
	            StopCoroutine(LevelUpCoroutine);
	            ElfLevelUpParticle.SetActive(false);
	        }
	        ElfLevelUpParticle.SetActive(true);
	        LevelUpCoroutine = StartCoroutine(EndAnimation(5f, ElfLevelUpParticle));
	    }
	
	    private void OnAnimalModelRefresh(IEvent ievent)
	    {
	        var e = ievent as ElfModelRefreshEvent;
	
	        if (ModelRoot != null)
	        {
	            ModelRoot.DestroyFakeCharacter();
	        }
	        var dataId = e.CharId;
	        if (dataId == -1)
	        {
	            return;
	        }
	        var tableNpc = Table.GetCharacterBase(dataId);
	        if (null == tableNpc)
	        {
	            return;
	        }
	
	        var offset = tableNpc.CameraHeight/10000.0f;
	        ModelRoot.Create(dataId, null, character =>
	        {
	            character.SetScale(tableNpc.CameraMult/10000.0f);
	            character.ObjTransform.localPosition = new Vector3(0, offset, 0);
	            ModelDrag.Target = character.transform;
	        });
	        InfoScroll.ResetPosition();
	    }
	
	    private void OnAnimalPlayAnimation(IEvent ievent)
	    {
	        var e = ievent as ElfPlayAnimationEvent;
	        var animName = e.Type == 0 ? "ShowFormationInfo" : "ShowElfList";
	        var direction = e.IsForward ? Direction.Forward : Direction.Reverse;
	        if (e.IsInstant)
	        {
	            var enumerator = FormationAnimation.GetEnumerator();
	            while (enumerator.MoveNext())
	            {
	                var state = (AnimationState) enumerator.Current;
	                if (state.name == animName)
	                {
	                    state.time = e.IsForward ? state.length : 0;
	                    if (!e.IsForward)
	                    {
	                        state.speed = -Mathf.Abs(state.speed);
	                    }
	                    FormationAnimation.Play(animName);
	                    FormationAnimation.Sample();
	                    break;
	                }
	            }
	        }
	        else
	        {
	            ActiveAnimation.Play(FormationAnimation, animName, direction);
	        }
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        EventDispatcher.Instance.AddEventListener(ElfModelRefreshEvent.EVENT_TYPE, OnAnimalModelRefresh);
	        EventDispatcher.Instance.AddEventListener(FormationElfModelRefreshEvent.EVENT_TYPE, OnFormationAnimalModelRefresh);
	        EventDispatcher.Instance.AddEventListener(UIEvent_ElfSetGridLookIndex.EVENT_TYPE, LookScrollVievCenter);
	        EventDispatcher.Instance.AddEventListener(ElfCell1ClickEvent.EVENT_TYPE, OnElfCell1Click);
	        EventDispatcher.Instance.AddEventListener(ElfFlyEvent.EVENT_TYPE, OnAnimalFly);
	        EventDispatcher.Instance.AddEventListener(FormationLevelupEvent.EVENT_TYPE, OnFormationLvUp);
	        EventDispatcher.Instance.AddEventListener(ElfLevelupEvent.EVENT_TYPE, OnAnimalLevelup);
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.ElfUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        DataModel = (ElfDataModel) controllerBase.GetDataModel("");
	        Binding.SetBindDataSource(DataModel);
	        Binding.SetBindDataSource(controllerBase.GetDataModel("Resource"));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.WeakNoticeData);
	
	        var controller = UIManager.Instance.GetController(UIConfig.ShareFrame);
	        Binding.SetBindDataSource(controller.GetDataModel(""));
	
	        for (int i = 0, imax = DataModel.Formations.Count; i < imax; ++i)
	        {
	            CreateFormatAnimalModel(i);
	        }
	        BlurObject.SetActive(false);
	        ElfLevelUpParticle.SetActive(false);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnFormationAnimalModelRefresh(IEvent ievent)
	    {
	        var e = ievent as FormationElfModelRefreshEvent;
	        CreateFormatAnimalModel(e.Index);
	    }
	
	    public void OnFormationInfoBtnClicked()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(20));
	    }
	
	    private void OnFormationLvUp(IEvent ievent)
	    {
	        var formations = DataModel.Formations;
	        for (int i = 0, imax = formations.Count; i < imax; ++i)
	        {
	            var f = formations[i];
	            if (!f.ShowElf)
	            {
	                continue;
	            }
	            var p = FormationParticles[i];
	            p.gameObject.SetActive(true);
	            p.Play();
	            StartCoroutine(EndAnimation(0.5f, p.gameObject));
	        }
	    }
	
	    public void OnShowElf1()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(41));
	    }
	
	    public void OnShowElf2()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(42));
	    }
	
	    public void OnShowElfInfo0()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(30));
	    }
	
	    public void OnShowElfInfo1()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(31));
	    }
	
	    public void OnShowElfInfo2()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(32));
	    }
	
	    public void OnTabClicked0()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(60));
	    }
	
	    public void OnTabClicked1()
	    {
	        //PlayerDataManager.Instance.WeakNoticeData.ElfCanUpgrade = false;
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(61));
	    }
	
	    public void OnTabClicked2()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(62));
	    }

        public void OnTabClickedSkill()
        {
            EventDispatcher.Instance.DispatchEvent(new ElfOperateEvent(59));
        }
	
	    private void SetAnimalDragEnable(bool value)
	    {
	        var count = Grid.childCount;
	        for (var i = 0; i < count; i++)
	        {
	            var t = Grid.GetChild(i);
	            var d = t.GetComponentsInChildren<AnimalDragItem>();
	            var c = d.Length;
	            for (var j = 0; j < c; j++)
	            {
	                var e = d[j];
	                e.enabled = value;
	            }
	        }
	    }
	
	    public void ShowOneDrawInfo()
	    {
	        var e = new ElfOneDrawInfoEvent();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        EventDispatcher.Instance.AddEventListener(ElfPlayAnimationEvent.EVENT_TYPE, OnAnimalPlayAnimation);
	        //EventDispatcher.Instance.AddEventListener(UIEvent_ElfBaoXiangOverEvent.EVENT_TYPE, BaoxiangAnimOver);
	        EventDispatcher.Instance.AddEventListener(ElfGetDrawResultBack.EVENT_TYPE, DrawAnimationStart);
	
	        varTenList = new List<GameObject>();
	
	        for (var i = 0; i < MenuList.Count; i++)
	        {
	            var menu = MenuList[i];
	            var btn = menu.GetComponent<UIButton>();
	            var j = i;
	            btn.onClick.Add(new EventDelegate(() => { OnClickMenu(j); }));
	        }
	        for (var i = 0; i < DrawShowObjs.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { OnClickShowDrawItems(j); });
	
	            DrawShowObjs[i].onClick.Add(deleget);
	        }
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public void Listen<T>(T message)
	    {
	        if (message is string && (message as string) == "ActiveChanged")
	        {
	            flag = true;
	        }
	    }
	
	    #region 十连抽动画效果
	
	    public bool IsOneDraw;
	
	    private void DrawAnimationStart(IEvent ievent)
	    {
	        var e = ievent as ElfGetDrawResultBack;
	        BlurObject.SetActive(true);
	        if (baoXiangAni == null)
	        {
	            baoXiangAni =
	                Instantiate(ResourceManager.PrepareResourceSync<GameObject>("Model/UI/UI_BaoXiang.prefab")) as
	                    GameObject;
	            baoXiangAni.transform.parent = BaoXiangpos.transform;
	            baoXiangAni.transform.localPosition = Vector3.zero;
	            baoXiangAni.transform.localScale = new Vector3(318f, 318f, 318f);
	            baoXiangAni.AddComponent<WishingAnimation>();
	            var baoxiangAnim = baoXiangAni.GetComponent<Animation>();
	            baoXiangAni.SetActive(true);
	            baoxiangAnim.Play();
	        }
	        else
	        {
	            var baoxiangAnim = baoXiangAni.GetComponent<Animation>();
	            baoXiangAni.SetActive(true);
	            baoxiangAnim.Play();
	        }
	        if (e.DrawType == 1)
	        {
	            IsOneDraw = true;
	        }
	        else if (e.DrawType == 10)
	        {
	            IsOneDraw = false;
	        }
	        BaoxiangAnimOver();
	    }
	
	    //public void BaoxiangAnimOver(IEvent ievent)
	    //{
	    //    if (IsOneDraw)
	    //    {
	    //        NetManager.Instance.StartCoroutine(WaitSeconds(0.4f));
	
	    //    }
	    //    else
	    //    {
	    //        CreateTenPrefab();
	    //        NetManager.Instance.StartCoroutine(WaitSeconds(0.4f));
	    //    }
	    //}
	
	    public void BaoxiangAnimOver()
	    {
	        if (IsOneDraw)
	        {
	            NetManager.Instance.StartCoroutine(WaitSeconds(1.5f));
	        }
	        else
	        {
	            NetManager.Instance.StartCoroutine(WaitSeconds(1.5f));
	        }
	    }
	
	    private IEnumerator WaitSeconds(float seconds)
	    {
	        yield return new WaitForSeconds(seconds);
	        if (IsOneDraw)
	        {
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_ElfShowDrawGetEvent());
	        }
	        else
	        {
	            CreateTenPrefab();
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_ElfShowDrawGetEvent());
	        }
	
	        BlurObject.SetActive(false);
	        if (baoXiangAni != null)
	        {
	            baoXiangAni.SetActive(false);
	        }
	    }
	
	    private readonly string prefabPath = "UI/Elf/ElfBattleCell.prefab";
	    //创建10个抽奖物品
	    private void CreateTenPrefab()
	    {
	        for (var i = 0; i < varTenList.Count; i++)
	        {
	            Destroy(varTenList[i]);
	        }
	        varTenList.Clear();
	        NetManager.Instance.StartCoroutine(InitPreBagCellfab());
	    }
	
	    //加载资源，绑定十连抽源
	    private IEnumerator InitPreBagCellfab()
	    {
	        var controllerBase = UIManager.Instance.GetController(UIConfig.ElfUI);
	        if (controllerBase == null)
	        {
	            yield break;
	        }
	        var draw = controllerBase.GetDataModel("") as ElfDataModel;
	        var holder = ResourceManager.PrepareResourceWithHolder<GameObject>(prefabPath);
	        for (var i = 0; i < draw.TenGetItem.Count; i++)
	        {
	            yield return holder.Wait();
	            var BagCell = Instantiate(holder.Resource) as GameObject;
	            BagCell.GetComponent<UIWidget>().depth = 70;
	            var bagCellTransform = BagCell.transform;
	            bagCellTransform.parent = TenDrawObj[i].transform;
	            bagCellTransform.localScale = Vector3.one;
	            bagCellTransform.localPosition = Vector3.zero;
	            bagCellTransform.localRotation = new Quaternion(0, 0, 0, 0);
	            BagCell.SetActive(true);
	
	            var bindroot = TenDrawObj[i].GetComponent<BindDataRoot>();
	            var itemDataModel = draw.TenGetItem[i];
	            bindroot.SetBindDataSource(itemDataModel);
	            var itemLogic = BagCell.GetComponent<ListItemLogic>();
	            itemLogic.Index = i;
	            itemLogic.Item = itemDataModel;
	            varTenList.Add(itemLogic.gameObject);
	        }
	    }
	
	    #endregion
	}
}