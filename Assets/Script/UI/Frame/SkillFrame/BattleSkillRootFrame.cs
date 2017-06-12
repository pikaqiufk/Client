#region using

using System;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BattleSkillRootFrame : MonoBehaviour
	{
	    public UIButton AttackBtn;
	    private EventSystem.EventDelegate eventHandler;
	    public DelegateBtnClick OnClickEvent = null;
	    public List<MainBattleSkill> SkillItems;
	    public UIButton SwitchBtn;
	
	    private static void RegisterPressEvent(MonoBehaviour behaviour, MainBattleSkill logic)
	    {
	        if (behaviour == null)
	        {
	            return;
	        }
	        var trigger = behaviour.gameObject.AddComponent<UIEventTrigger>();
	        var pressed = new EventDelegate(() =>
	        {
	            try
	            {
	                UIEvent_SkillButtonPressed evt;
	                if (logic == null)
	                {
	                    evt = new UIEvent_SkillButtonPressed(-1);
	                }
	                else
	                {
	                    evt = new UIEvent_SkillButtonPressed(logic.SkillItemDataModel.SkillId);
	                }
	
	                EventDispatcher.Instance.DispatchEvent(evt);
	            }
	            catch (Exception ex)
	            {
	                Logger.Error(ex.ToString());
	            }
	        });
	        var released = new EventDelegate(() =>
	        {
	            try
	            {
	                UIEvent_SkillButtonReleased evt;
	                if (logic == null)
	                {
	                    evt = new UIEvent_SkillButtonReleased(true, -1);
	                }
	                else
	                {
	                    evt = new UIEvent_SkillButtonReleased(true, logic.SkillItemDataModel.SkillId);
	                }
	
	                EventDispatcher.Instance.DispatchEvent(evt);
	            }
	            catch (Exception ex)
	            {
	                Logger.Error(ex.ToString());
	            }
	        });
	        trigger.onPress.Add(pressed);
	        trigger.onRelease.Add(released);
	    }
	
	    private void OnClick_SkillButton(int idx, int skillId = -1)
	    {
	        //skillId = 0 表示装备了个空技能
	        if (skillId == 0)
	        {
	            return;
	        }
	
	        if (null != OnClickEvent)
	        {
	            if (PlayerDataManager.Instance.PlayerDataModel.SkillData.CommonCoolDown <= 0)
	            {
	                OnClickEvent(0, 0);
	            }
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        if (null != eventHandler)
	        {
	            EventDispatcher.Instance.RemoveEventListener(Attr_Change_Event.EVENT_TYPE, eventHandler);
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
	        var evt = new UIEvent_SkillButtonReleased(false, 0);
	        EventDispatcher.Instance.DispatchEvent(evt);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        // 普通攻击按钮
	        var del = new EventDelegate(() =>
	        {
	            try
	            {
	                OnClick_SkillButton(0);
	            }
	            catch (Exception ex)
	            {
	                Logger.Error(ex.ToString());
	            }
	        });
	
	        AttackBtn.onClick.Add(del);
	        RegisterPressEvent(AttackBtn, null);
	        {
	            var __list1 = SkillItems;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var skillBarItemLogic = __list1[__i1];
	                {
	                    var logic = skillBarItemLogic;
	                    del = new EventDelegate(() =>
	                    {
	                        try
	                        {
	                            logic.OnSkillUse(OnClickEvent);
	                        }
	                        catch (Exception ex)
	                        {
	                            Logger.Error(ex.ToString());
	                        }
	                    });
	
	                    logic.SkillButton.onClick.Add(del);
	                    RegisterPressEvent(logic, logic);
	                }
	            }
	        }
	        // 换目标
	        del = new EventDelegate(() =>
	        {
	            try
	            {
	                OnClick_SkillButton(2);
	            }
	            catch (Exception ex)
	            {
	                Logger.Error(ex.ToString());
	            }
	        });
	        //SwitchBtn.onClick.Add(del);
	
	        eventHandler = evn =>
	        {
	            var e = evn as Attr_Change_Event;
	            if (eAttributeType.MpNow != e.Type)
	            {
	                return;
	            }
	
	            var mp = e.NewValue;
	            {
	                var __list2 = SkillItems;
	                var __listCount2 = __list2.Count;
	                for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	                {
	                    var btn = __list2[__i2];
	                    {
	                        var skillId = btn.SkillItemDataModel.SkillId;
	                        if (-1 == skillId)
	                        {
	                            continue;
	                        }
	
	                        if (null != GameControl.Instance)
	                        {
	                            if (ErrorCodes.Error_MpNoEnough == GameControl.Instance.CheckSkill(skillId))
	                            {
	                                btn.ChangeActive(false);
	                            }
	                            else
	                            {
	                                btn.ChangeActive(true);
	                            }
	                        }
	                    }
	                }
	            }
	        };
	        EventDispatcher.Instance.AddEventListener(Attr_Change_Event.EVENT_TYPE, eventHandler);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public void Tick()
	    {
	        {
	            var __list4 = SkillItems;
	            var __listCount4 = __list4.Count;
	            for (var __i4 = 0; __i4 < __listCount4; ++__i4)
	            {
	                var btn = __list4[__i4];
	                {
	                    btn.Tick();
	                }
	            }
	        }
	    }
	
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
	
	    public delegate void DelegateBtnClick(int idx, int arg1);
	}
}