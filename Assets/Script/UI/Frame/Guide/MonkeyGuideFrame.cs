using System;
#region using

using System.Collections;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class MonkeyGuideFrame : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    private GuideDataModel DataModel;
	
	    public GameObject Mask;
	    public GameObject Guide;
	
	    public UITexture L;
	    public UITexture R;
	    public UITexture U;
	    public UITexture D;
	    public UITexture Hollow;
	    public UILabel Label;
	    public UISprite Pointer;
	    public UISprite Image;
	
	    private Transform hollowTrans;
	    private Transform lblTrans;
	    private Transform directTrans;
	    private Transform imgTrans;
	
	    private Coroutine endCoroutine;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        hollowTrans = Hollow.transform;
	        lblTrans = Label.transform;
	        directTrans = Pointer.gameObject.transform;
	        imgTrans = Image.transform;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
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
	
	    // Update is called once per frame
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (Guide.active)
	        {
	            //var q = mPointerTransform.rotation;
	            //float from = q.eulerAngles.z;
	            //Quaternion r = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(from, DataModel.PointerAngel, 5*Time.deltaTime)));
	            //mPointerTransform.rotation = r;
	
	            var fromPos = directTrans.localPosition;
	            directTrans.localPosition = Vector3.Lerp(fromPos, DataModel.PointerPos, 5*Time.deltaTime);
	        }
	
	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.NewbieGuide);
	        DataModel = controllerBase.GetDataModel("") as GuideDataModel;
	        BindRoot.SetBindDataSource(DataModel);
	
	        EventDispatcher.Instance.AddEventListener(UIEvent_NextGuideEvent.EVENT_TYPE, OnEvent_NextStep);
	
	        Mask.SetActive(false);
	        Guide.SetActive(true);
	
	        var data = GuideManager.Instance.GetCurrentGuideData();
	        if (null != data)
	        {
	            PostponeShow(data.DelayTime*0.001f);
	        }
	
	        EventDispatcher.Instance.DispatchEvent(new UI_BlockMainUIInputEvent(0));
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
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_NextGuideEvent.EVENT_TYPE, OnEvent_NextStep);
	        BindRoot.RemoveBinding();
	        if (null != endCoroutine)
	        {
	            StopCoroutine(endCoroutine);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	#if UNITY_EDITOR
	    [ContextMenu("Print Log")]
	    private void PrintDebugState()
	    {
	        var str = "1" + "\t";
	        str += Label.text + "\t";
	        str += Label.width + "\t";
	        str += Image.height + "\t";
	        str += lblTrans.localPosition.x + "\t";
	        str += lblTrans.localPosition.y + "\t";
	        var id = -1;
	        if (null != Image.atlas && !string.IsNullOrEmpty(Image.spriteName))
	        {
	            Table.ForeachIcon(table =>
	            {
	                if (table.Atlas == Image.atlas.name && Image.spriteName == table.Sprite)
	                {
	                    id = table.Id;
	                    return false;
	                }
	                return true;
	            });
	        }
	        str += id + "\t";
	        str += imgTrans.localPosition.x + "\t";
	        str += imgTrans.localPosition.y + "\t";
	        str += "0" + "\t";
	        str += "99" + "\t";
	        str += (Hollow.color.a*255) + "\t";
	        str += Hollow.width + "\t";
	        str += Hollow.height + "\t";
	        str += hollowTrans.localPosition.x + "\t";
	        str += hollowTrans.localPosition.y + "\t";
	        str += (Pointer.active ? "1" : "0") + "\t";
	        str += directTrans.localPosition.x + "\t";
	        str += directTrans.localPosition.y + "\t";
	        str += ((int) directTrans.rotation.eulerAngles.z) + "\t";
	        str += "1" + "\t";
	        str += "-1" + "\t";
	        str += "0" + "\t";
	        str += "-1" + "\t";
	
	        Debug.Log(str);
	    }
	
	
	#endif
	
	    private void OnEvent_NextStep(IEvent ievent)
	    {
	        var e = ievent as UIEvent_NextGuideEvent;
	        if (null == e)
	        {
	            return;
	        }
	
	        var delay = e.Delay;
	        PostponeShow(delay);
	    }
	
	    private void PostponeShow(float time)
	    {
	        if (null != endCoroutine)
	        {
	            StopCoroutine(endCoroutine);
	            endCoroutine = null;
	        }
	
	        StartCoroutine(PostponeShowCoroutine(time));
	    }
	
	    private IEnumerator PostponeShowCoroutine(float time)
	    {
	        if (time <= 0)
	        {
	            endCoroutine = null;
	            yield break;
	        }
	
	        Mask.SetActive(true);
	        Guide.SetActive(false);
	
	        yield return new WaitForSeconds(time);
	
	        Mask.SetActive(false);
	        Guide.SetActive(true);
	        endCoroutine = null;
	    }
	
	    public void NextStep()
	    {
	        if (DataModel.ClickAnyWhereToNext)
	        {
	            GuideManager.Instance.NextStep();
	        }
	    }
	
	    private bool IsShowing(Vector3 worldPoint, GameObject go)
	    {
	        var panel = NGUITools.FindInParents<UIPanel>(go);
	
	        while (panel != null)
	        {
	            if (!panel.IsVisible(worldPoint))
	            {
	                return false;
	            }
	            panel = panel.parentPanel;
	        }
	        return true;
	    }
	
	    private struct DepthEntry
	    {
	        public int depth;
	        public GameObject go;
	        public RaycastHit hit;
	        public Vector3 point;
	    }
	
	#if UNITY_FLASH
		static bool IsShowing (DepthEntry de)
	#else
	    private static bool IsShowing(ref DepthEntry de)
	#endif
	    {
	        var panel = NGUITools.FindInParents<UIPanel>(de.go);
	
	        while (panel != null)
	        {
	            if (!panel.IsVisible(de.point))
	            {
	                return false;
	            }
	            panel = panel.parentPanel;
	        }
	        return true;
	    }
	
	    private void MessageNotify(GameObject go, string funcName, object obj)
	    {
	        if (NGUITools.GetActive(go))
	        {
	            go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
	        }
	    }
	
	    public void ClickCenter()
	    {
	        var mHit = new DepthEntry();
	        var mHits = new BetterList<DepthEntry>();
	
	        var cam = UIManager.Instance.UICamera.GetComponent<UICamera>();
	
	        UIEventTrigger.current = null;
	
	        // Convert to view space
	        var currentCamera = cam.cachedCamera;
	
	        // Cast a ray into the screen
	        var p = Hollow.transform.position;
	        p.z = currentCamera.nearClipPlane;
	        var ray = new Ray(p, Vector3.forward);
	
	        // Raycast into the screen
	        var mask = currentCamera.cullingMask & cam.eventReceiverMask;
	        var dist = (cam.rangeDistance > 0f)
	            ? cam.rangeDistance
	            : currentCamera.farClipPlane - currentCamera.nearClipPlane;
	
	        var hits = Physics.RaycastAll(ray, dist, mask);
	
	        if (hits.Length > 1)
	        {
	            for (var b = 0; b < hits.Length; ++b)
	            {
	                var go = hits[b].collider.gameObject;
	
	                if (go == Hollow.gameObject)
	                {
	                    continue;
	                }
	
	                var w = go.GetComponent<UIWidget>();
	
	                if (w != null)
	                {
	                    if (!w.isVisible)
	                    {
	                        continue;
	                    }
	                    if (w.hitCheck != null && !w.hitCheck(hits[b].point))
	                    {
	                        continue;
	                    }
	                }
	                else
	                {
	                    var rect = NGUITools.FindInParents<UIRect>(go);
	                    if (rect != null && rect.finalAlpha < 0.001f)
	                    {
	                        continue;
	                    }
	                }
	
	                mHit.depth = NGUITools.CalculateRaycastDepth(go);
	
	                if (mHit.depth != int.MaxValue)
	                {
	                    mHit.hit = hits[b];
	                    mHit.point = hits[b].point;
	                    mHit.go = hits[b].collider.gameObject;
	                    mHits.Add(mHit);
	                }
	            }
	
	            mHits.Sort(delegate(DepthEntry r1, DepthEntry r2) { return r2.depth.CompareTo(r1.depth); });
	
	            for (var b = 0; b < mHits.size; ++b)
	            {
	#if UNITY_FLASH
							if (IsShowing(mHits.buffer[b]))
	#else
	                if (IsShowing(ref mHits.buffer[b]))
	#endif
	                {
	                    MessageNotify(mHits.buffer[b].go, "OnClick", null);
	                    return;
	                }
	            }
	            mHits.Clear();
	        }
	        else if (hits.Length == 1)
	        {
	            var go = hits[0].collider.gameObject;
	
	            if (go == Hollow.gameObject)
	            {
	                return;
	            }
	
	            var w = go.GetComponent<UIWidget>();
	
	            if (w != null)
	            {
	                if (!w.isVisible)
	                {
	                    return;
	                }
	                if (w.hitCheck != null && !w.hitCheck(hits[0].point))
	                {
	                    return;
	                }
	            }
	            else
	            {
	                var rect = NGUITools.FindInParents<UIRect>(go);
	                if (rect != null && rect.finalAlpha < 0.001f)
	                {
	                    return;
	                }
	            }
	
	            if (IsShowing(hits[0].point, hits[0].collider.gameObject))
	            {
	                MessageNotify(hits[0].collider.gameObject, "OnClick", null);
	            }
	        }
	    }
	
	    public void Skip()
	    {
	        GuideManager.Instance.Skip();
	    }
	}
}