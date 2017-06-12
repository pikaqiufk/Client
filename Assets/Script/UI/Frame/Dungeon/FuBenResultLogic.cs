using System;
#region using

using System.Collections;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class FuBenResultLogic : MonoBehaviour
	{
	    public UILabel AutoDrawLabel;
	    public List<GameObject> AwardBack;
	    public List<GameObject> AwardFront;
	    public List<GameObject> AwardList;
	    public BindDataRoot Binding;
	    public GameObject ConfirmBtn;
	    public Coroutine DrawCoroutine;
	    private readonly int AutoSecond = 10;
	    public List<GameObject> PaticleLists;
	    private int SelectIndex;
	
	    private IEnumerator AutoDrawCoroutine()
	    {
	        var nowtime = Game.Instance.ServerTime;
	        var dt = Game.Instance.ServerTime.AddSeconds(AutoSecond);
	        while (nowtime < dt)
	        {
	            AutoDrawLabel.text = GameUtils.GetTimeDiffString(dt);
	            yield return new WaitForSeconds(0.3f);
	            nowtime = Game.Instance.ServerTime;
	        }
	        OnClickAward(0);
	    }
	
	    private void OnClickAward(int index)
	    {
	        var e = new DungeonResultChoose(index);
	        EventDispatcher.Instance.DispatchEvent(e);
	        SelectIndex = index;
	        ConfirmBtn.SetActive(true);
	
	        var AwardListCount1 = AwardList.Count;
	        for (var i = 0; i < AwardListCount1; i++)
	        {
	            var o = AwardList[i];
	            o.GetComponent<BoxCollider>().enabled = false;
	            if (i == index)
	            {
	                var tweens = o.GetComponentsInChildren<TweenRotation>(true);
	                {
	                    var __array4 = tweens;
	                    //var __arrayLength4 = __array4.Length;
	                    //for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
	                    {
	                        var position = __array4[0];
	                        {
	                            position.ResetToBeginning();
	                            position.PlayForward();
	                        }
	                    }
	                }
	            }
	        }
	        StartCoroutine(ShowOtherAward(index));
	    }
	
	    public void OnClickBtnClose()
	    {
	        var e = new Close_UI_Event(UIConfig.DungeonResult);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        Binding.RemoveBinding();
	        if (DrawCoroutine != null)
	        {
	            NetManager.Instance.StopCoroutine(DrawCoroutine);
	            DrawCoroutine = null;
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.DungeonResult);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        {
	            var __list1 = AwardList;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var o = __list1[__i1];
	                {
	                    o.transform.localRotation = Quaternion.Euler(0, 0, 0);
	                    o.GetComponent<BoxCollider>().enabled = true;
	                    var tweens = o.GetComponentsInChildren<TweenRotation>(true);
	                    {
	                        var __array6 = tweens;
	                        var __arrayLength6 = __array6.Length;
	                        for (var __i6 = 0; __i6 < __arrayLength6; ++__i6)
	                        {
	                            var position = __array6[__i6];
	                            {
	                                position.enabled = false;
	                            }
	                        }
	                    }
	                }
	            }
	        }
	        {
	            var __list2 = AwardBack;
	            var __listCount2 = __list2.Count;
	            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	            {
	                var o = __list2[__i2];
	                {
	                    o.gameObject.SetActive(true);
	                }
	            }
	        }
	        {
	            var __list3 = AwardFront;
	            var __listCount3 = __list3.Count;
	            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
	            {
	                var o = __list3[__i3];
	                {
	                    o.gameObject.SetActive(false);
	                }
	            }
	        }
	        ConfirmBtn.SetActive(false);
	
	        for (var k = 0; k < PaticleLists.Count; k++)
	        {
	            PaticleLists[k].SetActive(false);
	        }
	
	        StartAutoDraw();
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private IEnumerator ShowOtherAward(int index)
	    {
	        yield return new WaitForSeconds(1.0f);
	        var AwardListCount2 = AwardList.Count;
	        for (var i = 0; i < AwardListCount2; i++)
	        {
	            if (i == index)
	            {
	                continue;
	            }
	            var o = AwardList[i];
	            var tweens = o.GetComponentsInChildren<TweenRotation>(true);
	            {
	                var __array5 = tweens;
	                //var __arrayLength5 = __array5.Length;
	                //for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
	                {
	                    var position = __array5[0];
	                    {
	                        position.ResetToBeginning();
	                        position.PlayForward();
	                    }
	                }
	            }
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        var AwardListCount0 = AwardList.Count;
	        for (var i = 0; i < AwardListCount0; i++)
	        {
	            var tweens = AwardList[i].GetComponents<TweenRotation>();
	            var tween = tweens[0];
	            tween.enabled = false;
	            var j = i;
	            var deleget = new EventDelegate(() =>
	            {
	                AwardFront[j].gameObject.SetActive(true);
	                AwardBack[j].gameObject.SetActive(false);
	
	                var tween2 = tweens[1];
	                tween2.ResetForPlay();
	                tween2.PlayForward();
	                for (var k = 0; k < PaticleLists.Count; k++)
	                {
	                    if (k == SelectIndex)
	                    {
	                        PaticleLists[k].SetActive(true);
	                        continue;
	                    }
	                    PaticleLists[k].SetActive(false);
	                }
	                if (DrawCoroutine != null)
	                {
	                    NetManager.Instance.StopCoroutine(DrawCoroutine);
	                    DrawCoroutine = null;
	                }
	            });
	            tween.onFinished.Add(deleget);
	
	            var deleget1 = new EventDelegate(() => { OnClickAward(j); });
	
	            var btn = AwardList[i].GetComponent<UIEventTrigger>();
	            btn.onClick.Add(deleget1);
	        }
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void StartAutoDraw()
	    {
	        if (DrawCoroutine != null)
	        {
	            NetManager.Instance.StopCoroutine(DrawCoroutine);
	            DrawCoroutine = null;
	        }
	        DrawCoroutine = NetManager.Instance.StartCoroutine(AutoDrawCoroutine());
	    }
	}
}