#region using

using System;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class FriendFrame : MonoBehaviour
    {
        public BindDataRoot Binding;
        public GameObject BlockLayer;
        public UIInput ChatInput;
        public UIInput FindInput;
        public UIGridSimple GridContact;
        private float blockTime = -1.0f;
        private bool enable;
        private bool canBind = true;
        public List<UIButton> TabList;

        private void LateUpdate()
        {
#if !UNITY_EDITOR
            try
            {
#endif

                if (enable)
                {
                    GridContact.enabled = true;
                    enable = false;
                }

#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }

        public void OnClickBag()
        {
            var arg = new ChatItemListArguments();
            arg.Type = 3;
            var e = new Show_UI_Event(UIConfig.ChatItemList, arg);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        //-------------------------------------------------------Chat------------
        public void OnClickChatSend()
        {
            var e = new FriendClickType(1);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void OnClickChatTab()
        {
            enable = true;
            var e = new FriendClickType(4);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void OnClickClearRecord()
        {
            var e = new FriendClickType(3);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void OnClickClose()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.FriendUI));
        }

        public void OnClickFace()
        {
            var arg = new FaceListArguments();
            arg.Type = 3;
            var e = new Show_UI_Event(UIConfig.FaceList, arg);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void OnClickQuickSeekBtn()
        {
            EventDispatcher.Instance.DispatchEvent(new FriendSeekBtnClick(1));
        }

        public void OnClickSeekBtn()
        {
            EventDispatcher.Instance.DispatchEvent(new FriendSeekBtnClick(0));
        }

        private void OnEvent_SelectTab(int i)
        {
            var e = new FriendClickTabEvent(i);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        private void OnEvent_CancleBind(IEvent ievent)
        {
            var e = ievent as CloseUiBindRemove;
            if (e.Config != UIConfig.FriendUI)
            {
                return;
            }
            if (e.NeedRemove == 0)
            {
                canBind = false;
            }
            else
            {
                if (canBind == false)
                {
                    RemoveBindEvent();
                }
                canBind = true;
            }
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR
            try
            {
#endif
                if (canBind == false)
                {
                    RemoveBindEvent();
                }
                canBind = true;
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
                if (canBind)
                {
                    foreach (var button in TabList)
                    {
                        var toggle = button.GetComponent<UIToggle>();
                        //toggle.value = false;
                        toggle.Set(false);
                        var objs = toggle.GetComponent<UIToggledObjects>();
                        if (objs)
                        {
                            foreach (var o in objs.activate)
                            {
                                if (o)
                                {
                                    o.SetActive(false);
                                }
                            }
                        }
                    }
                    RemoveBindEvent();
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
                SetBlockLayer(false);

                if (canBind)
                {
                    EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CancleBind);
                    EventDispatcher.Instance.AddEventListener(FriendNotify.EVENT_TYPE, OnEvent_FriendNotify);
                    var controllerBase = UIManager.Instance.GetController(UIConfig.FriendUI);
                    if (controllerBase == null)
                    {
                        return;
                    }
                    Binding.SetBindDataSource(controllerBase.GetDataModel(""));
                    Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
                }
                canBind = true;
#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }

        private void OnEvent_FriendNotify(IEvent ievent)
        {
            var e = ievent as FriendNotify;
            if (e.Type == 1)
            {
                blockTime = -1.0f;
                SetBlockLayer(false);
            }
            else if (e.Type == 2)
            {
                blockTime = 30.0f; //30s µ¹¼ÆÊ±
                SetBlockLayer(true);
            }
        }

        public void OnInputFindForcus()
        {
            var e = new FriendClickType(6);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void OnInputFindLostForcus()
        {
            if (String.IsNullOrEmpty(FindInput.label.text))
            {
                FindInput.label.text = GameUtils.GetDictionaryText(240612);
            }
        }

        public void OnInputForcus()
        {
            var e = new FriendClickType(5);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void OnInputLostForcus()
        {
            if (String.IsNullOrEmpty(ChatInput.label.text))
            {
                ChatInput.label.text = GameUtils.GetDictionaryText(100001058);
            }
        }

        private void RemoveBindEvent()
        {
            Binding.RemoveBinding();
            EventDispatcher.Instance.RemoveEventListener(FriendNotify.EVENT_TYPE, OnEvent_FriendNotify);
            EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CancleBind);
        }

        private void SetBlockLayer(bool value)
        {
            if (BlockLayer != null)
            {
                BlockLayer.SetActive(value);
            }
        }

        private void Start()
        {
#if !UNITY_EDITOR
            try
            {
#endif
                var index = 0;
                foreach (var tab in TabList)
                {
                    var j = index;
                    var deleget = new EventDelegate(() => { OnEvent_SelectTab(j); });
                    index++;
                    tab.onClick.Add(deleget);
                }
#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }

        private void Update()
        {
#if !UNITY_EDITOR
            try
            {
#endif
                if (blockTime > 0)
                {
                    blockTime -= Time.deltaTime;
                    if (blockTime < 0)
                    {
                        SetBlockLayer(false);
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
    }
}
