using System;
#region using

using System.Collections;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TouchSpinning : MonoBehaviour
	{
	    public static TouchSpinning s_tsInstance;
	    //alpha???????????
	    public float AlphaDist = 1.2f;
	    //????????????
	    public float autoRoteSpeed = 10f;
	    //??????????????????????
	    private Vector3 centerVec3;
	    //??????????????
	    public float fixDegree = 10f;
	    private bool isSkillTalentGuide;
	    private int LastSkillId;
	    private Vector3 initScale;
	    private Coroutine rotateCoroutine;
	    //????
	    public float Radius = 0.4f;
	    //?洢??????????
	    private readonly List<Transform> boxTransList = new List<Transform>();
	    //?????????????
	    private readonly List<SkillOutBox> skillBoxList = new List<SkillOutBox>();
	    //????????alpha??????
	    private readonly List<UIWidget> skillWidgetList = new List<UIWidget>();
	    public GameObject SkyCube;
	    //?????????
	    public float speed = 1f;
	    //???????????
	    public Transform Target;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	     //   s_tsInstance = this;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void SetFrontScale()
	    {
// 	        var count = boxTransList.Count;
// 	        for (var i = 0; i < count; i++)
// 	        {
// 	            var child = boxTransList[i];
// 	            child.transform.forward = Vector3.forward;
// 	            //var scale = (7 - child.transform.position.z) / (7 - 1.678f) * 0.5f + 0.5f;
// 	            var scale = (7 - child.transform.position.z)/10.664f + 0.5f;
// 	            child.transform.localScale = initScale*scale;
// 	
// 	            var boxLogic = skillBoxList[i];
// 	            var widget = skillWidgetList[i];
// 	
// 	            var dist = (child.transform.position - centerVec3).magnitude;
// 	            if (dist > AlphaDist)
// 	            {
// 	                widget.alpha = 0;
// 	            }
// 	            else
// 	            {
// 	                widget.alpha = 1 - dist/AlphaDist;
// 	                if (widget.alpha < 0.1f)
// 	                {
// 	                    widget.alpha = 0;
// 	                }
// 	                if (LastSkillId != boxLogic.SkillId && widget.alpha > 0.8)
// 	                {
// 	                    LastSkillId = boxLogic.SkillId;
// 	                    EventDispatcher.Instance.DispatchEvent(
// 	                        new UIEvent_SkillFrame_OnSkillTalentSelected(LastSkillId));
// 	                }
// 	            }
// 	        }
	    }
	
	    public void Initialize(Transform t)
	    {
// 	        Target = t;
// 	        Target.localPosition = new Vector3(520, 78, 1346);
// 	        Target.localScale = new Vector3(2000f, 2000f, 2000f);
// 	        Target.localRotation = Quaternion.Euler(82, 0, -130);
// 	        var skillboxs = Target.gameObject.GetComponentsInChildren<SkillOutBox>(true);
// 	        var count = skillboxs.Length;
// 	        float singleDegree = 360f/count;
// 	        boxTransList.Clear();
// 	        skillBoxList.Clear();
// 	        skillWidgetList.Clear();
// 	
// 	        for (var i = 0; i < count; ++i)
// 	        {
// 	            var skillbox = skillboxs[i];
// 	            var trans = skillbox.gameObject.transform;
// 	            var angle = i*Mathf.Deg2Rad*360/count;
// 	            trans.localPosition = new Vector3(Radius*Mathf.Cos(angle), Radius*Mathf.Sin(angle), 0);
// 	            initScale = trans.localScale;
// 	
// 	            var skillbtn = trans.FindChild("SkillBall");
// 	            skillbtn = skillbtn.FindChild("SkillBallBtn");
// 	            var trigger = skillbtn.gameObject.AddComponent<UIEventTrigger>();
// 	            trigger.onClick.Add(new EventDelegate(() => { MoveSkillToCenter(skillbox.gameObject); }));
// 	
// 	            boxTransList.Add(trans);
// 	            skillBoxList.Add(skillbox);
// 	
// 	            var widget = skillbox.TalentBox.GetComponent<UIWidget>();
// 	            if (null == widget)
// 	            {
// 	                widget = skillbox.TalentBox.AddComponent<UIWidget>();
// 	            }
// 	            skillWidgetList.Add(widget);
// 	        }
// 	
// 	        var fristBoxPos = boxTransList[0].position;
// 	        centerVec3 = new Vector3(fristBoxPos.x, fristBoxPos.y, fristBoxPos.z);
// 	
// 	        SetFrontScale();
// 	        InitialSkyCube();
	    }
	
	    private void InitialSkyCube()
	    {
// 	        var skycube = UICamera.mainCamera.transform.Find("SkyCube");
// 	        if (skycube == null)
// 	        {
// 	            var xform = SkyCube.transform;
// 	            xform.parent = UICamera.mainCamera.transform;
// 	            xform.localPosition = Vector3.zero;
// 	            xform.localScale = Vector3.one*3000;
// 	            SkyCube.SetRenderQueue(3001);
// 	            SkyCube.SetActive(false);
// 	        }
// 	        else
// 	        {
// 	            if (skycube != SkyCube.transform)
// 	            {
// 	                Destroy(SkyCube);
// 	                SkyCube = skycube.gameObject;
// 	            }
// 	        }
	    }
	
	    public void MoveSkillToCenter(GameObject obj)
	    {
// 	        if (null == obj)
// 	        {
// 	            return;
// 	        }
// 	
// 	        if (rotateCoroutine != null)
// 	        {
// 	            StopCoroutine(rotateCoroutine);
// 	        }
// 	
// 	        rotateCoroutine = StartCoroutine(MoveCenterCoroutine(obj));
	    }
	
	    private IEnumerator MoveCenterCoroutine(GameObject obj)
	    {
// 	        var direction = 1;
// 	        if (obj.transform.position.z < centerVec3.z)
// 	        {
// 	            direction = -1;
// 	        }
// 	
// 	        while (true)
// 	        {
// 	            yield return new WaitForFixedUpdate();
// 	            var angles = Target.localRotation.eulerAngles;
// 	            var dis = (obj.transform.position - centerVec3).magnitude;
// 	            if (dis < 0.05f)
// 	            {
// 	                //obj.transform.position = centerPos;
// 	                SetFrontScale();
// 	                yield break;
// 	            }
// 	            var fspeed = autoRoteSpeed*(1.2f*dis + 0.8f)*direction;
// 	            angles.z += 0.1f*fspeed;
// 	            Target.localRotation = Quaternion.Euler(angles);
// 	            SetFrontScale();
// 	        }
            yield break;
	    }
	
	    private void MoveNearest()
	    {
// 	        GameObject obj = null;
// 	        var c = boxTransList.Count;
// 	        var minIndex = 0;
// 	        float dis = 0;
// 	        for (var i = 0; i < c; i++)
// 	        {
// 	            var trans = boxTransList[i];
// 	            if (i == 0)
// 	            {
// 	                dis = (trans.position - centerVec3).magnitude;
// 	            }
// 	            else
// 	            {
// 	                var d = (trans.position - centerVec3).magnitude;
// 	                if (dis > d)
// 	                {
// 	                    dis = d;
// 	                    minIndex = i;
// 	                }
// 	            }
// 	        }
// 	
// 	        MoveSkillToCenter(boxTransList[minIndex].gameObject);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
//	        SkyCube.SetActive(false);
// 	        if (UICamera.mainCamera != null)
// 	        {
// 	            UICamera.mainCamera.nearClipPlane = -10f;
// 	        }
//	        Target.gameObject.SetActive(false);
	
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_UpdateGuideEvent.EVENT_TYPE, OnGuideEvent);
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnDrag(Vector2 delta)
	    {
// 	        if (null == Target)
// 	        {
// 	            return;
// 	        }
// 	
// 	        //UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
// 	
// 	        var angle = Target.localRotation.eulerAngles;
// 	        angle.z += Time.fixedDeltaTime*speed*delta.x;
// 	        Target.localRotation = Quaternion.Euler(angle);
// 	        SetFrontScale();
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
// 	        if (null == Target)
// 	        {
// 	            return;
// 	        }
// 	
// 	        SkyCube.SetActive(true);
// 	       // UICamera.mainCamera.nearClipPlane = -1f;
// 	        StartCoroutine(RefreshUI());
	        EventDispatcher.Instance.AddEventListener(UIEvent_UpdateGuideEvent.EVENT_TYPE, OnGuideEvent);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnGuideEvent(IEvent ievent)
	    {
// 	        var e = ievent as UIEvent_UpdateGuideEvent;
// 	        var guideid = e.Step;
// 	        if (guideid == 120 || guideid == 130 || guideid == 140)
// 	        {
// 	            isSkillTalentGuide = true;
// 	        }
	    }
	
	    public void OnPress(bool pressed)
	    {
// 	        if (!pressed)
// 	        {
// 	            MoveNearest();
// 	        }
// 	        else
// 	        {
// 	            var e = new UIEvent_SkillFrame_TalentBallClick(-1);
// 	            EventDispatcher.Instance.DispatchEvent(e);
// 	        }
	    }
	
	    public void OnRelease()
	    {
	    }
	
	    private void RunGuide()
	    {
// 	        if (isSkillTalentGuide)
// 	        {
// 	            var roleClass = PlayerDataManager.Instance.GetRoleId();
// 	            var skillid = -1;
// 	            switch (roleClass)
// 	            {
// 	                case 0:
// 	                    skillid = 6;
// 	                    break;
// 	                case 1:
// 	                    skillid = 105;
// 	                    break;
// 	                case 2:
// 	                    skillid = 204;
// 	                    break;
// 	            }
// 	
// 	            var index = -1;
// 	            var c = skillBoxList.Count;
// 	            for (var i = 0; i < c; i++)
// 	            {
// 	                var skill = skillBoxList[i];
// 	                if (skill.SkillId == skillid)
// 	                {
// 	                    index = i;
// 	                    break;
// 	                }
// 	            }
// 	
// 	            if (index != -1)
// 	            {
// 	                MoveSkillToCenter(boxTransList[index].gameObject);
// 	            }
// 	            isSkillTalentGuide = false;
// 	        } //补充功能 移动到有技能点的技能上
// 	        else
// 	        {
// 	            var c = skillBoxList.Count;
// 	            var c2 = c/2;
// 	            var index = -1;
// 	            for (var i = 0; i < c2; i++)
// 	            {
// 	                if (skillBoxList[i].BoxDataModel.LastCount >= 1)
// 	                {
// 	                    index = i;
// 	                    break;
// 	                }
// 	            }
// 	
// 	            if (index == -1)
// 	            {
// 	                for (var j = c - 1; j >= c2; j--)
// 	                {
// 	                    if (skillBoxList[j].BoxDataModel.LastCount >= 1)
// 	                    {
// 	                        index = j;
// 	                        break;
// 	                    }
// 	                }
// 	            }
// 	
// 	            if (index != -1)
// 	            {
// 	                MoveSkillToCenter(boxTransList[index].gameObject);
// 	            }
// 	        }
	    }
	
	    private IEnumerator RefreshUI()
	    {
// 	        using (new BlockingLayerHelper())
// 	        {
// 	            Target.gameObject.SetActive(false);
// 	            Target.localPosition = new Vector3(520, 78, 1346);
// 	            Target.localScale = new Vector3(2000f, 2000f, 2000f);
// 	            Target.localRotation = Quaternion.Euler(82, 0, -130);
// 	            SetFrontScale();
// 	            yield return new WaitForFixedUpdate();
// 	
// 	            Target.position = new Vector3(0, 0, 10);
// 	            Target.localScale = Vector3.one;
// 	            Target.localRotation = Quaternion.Euler(0, 0, 0);
// 	            Target.gameObject.SetActive(true);
// 	            iTween.MoveTo(Target.gameObject,
// 	                iTween.Hash("position", new Vector3(520, 78, 1346), "time", 1, "islocal", true, "easetype",
// 	                    iTween.EaseType.linear));
// 	            iTween.ScaleTo(Target.gameObject,
// 	                iTween.Hash("scale", new Vector3(2000f, 2000f, 2000f), "time", 1, "easetype", iTween.EaseType.linear));
// 	            iTween.RotateTo(Target.gameObject,
// 	                iTween.Hash("rotation", new Vector3(82, 0, -130), "time", 1, "easetype", iTween.EaseType.linear));
// 	            yield return new WaitForSeconds(1.1f);
// 	            RunGuide();
// 	        }
            yield break;
	    }
	}
}