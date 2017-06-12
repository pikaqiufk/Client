using System;
#region using

using System.Collections;
using System.Collections.Generic;
using DataContract;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class LeaderboardFrame : MonoBehaviour
	{
		public BindDataRoot Binding;
		public List<UIToggle> MenuList;
		//private ObjFakeCharacter ObjFake;
		public UIDragRotate ModelDrag;
		public CreateFakeCharacter ModelRoot;
		private bool hasRemoveBinding = true;
		public List<Transform> RankList;
		public List<ParticleSystem> WorshipAnimation;

		private IEnumerator AfterAnimation(float delay, GameObject obj)
		{
			yield return new WaitForSeconds(delay);
			obj.SetActive(false);
		}

		public void OnClickCloseBtn()
		{
			OnUIReset();
			var e = new Close_UI_Event(UIConfig.RankUI, true);
			EventDispatcher.Instance.DispatchEvent(e);
		}

		public void OnClickViewInfo()
		{
			var e = new RankOperationEvent(1);
			EventDispatcher.Instance.DispatchEvent(e);
		}

		public void OnClickWorship()
		{
			var e = new RankOperationEvent(2);
			EventDispatcher.Instance.DispatchEvent(e);
		}

		private void OnCloseUiBindRemove(IEvent ievent)
		{
			var e = ievent as CloseUiBindRemove;
			if (e.Config != UIConfig.RankUI)
			{
				return;
			}
			if (e.NeedRemove == 0)
			{
				hasRemoveBinding = false;
			}
			else
			{
				if (hasRemoveBinding == false)
				{
					RemoveBindEvent();
				}
				hasRemoveBinding = true;
			}
		}

		private void OnDestroy()
		{
#if !UNITY_EDITOR
	        try
	        {
#endif
			if (hasRemoveBinding == false)
			{
				RemoveBindEvent();
			}
			hasRemoveBinding = true;
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
			if (hasRemoveBinding)
			{
				if (ModelRoot.Character)
				{
					ModelRoot.Character.gameObject.SetActive(false);
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
			foreach (var system in WorshipAnimation)
			{
				system.gameObject.SetActive(false);
			}

			if (hasRemoveBinding)
			{
				EventDispatcher.Instance.AddEventListener(RankNotifyLogic.EVENT_TYPE, OnRankNotifyLogic);
				EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
				EventDispatcher.Instance.AddEventListener(RankRefreshModelView.EVENT_TYPE, OnRankRefreshModelView);
				var controllerBase = UIManager.Instance.GetController(UIConfig.RankUI);
				if (controllerBase == null)
				{
					return;
				}
				Binding.SetBindDataSource(controllerBase.GetDataModel(""));
				var controller = UIManager.Instance.GetController(UIConfig.ShareFrame);
				Binding.SetBindDataSource(controller.GetDataModel(""));

				if (ModelRoot.Character)
				{
					ModelRoot.Character.gameObject.SetActive(true);
				}
			}
			hasRemoveBinding = true;

#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
#endif
		}

		private void OnRankNotifyLogic(IEvent ievent)
		{
			var e = ievent as RankNotifyLogic;
			switch (e.Type)
			{
				case 1:
					{
						foreach (var system in WorshipAnimation)
						{
							system.gameObject.SetActive(true);
							system.Simulate(0, true, true);
							system.Play();
							StartCoroutine(AfterAnimation(1.3f, system.gameObject));
						}

						if (ModelRoot && ModelRoot.Character)
						{
							ModelRoot.Character.PlayAnimation(e.Index, arg =>
							{
								if (ModelRoot && ModelRoot.Character)
								{
									ModelRoot.Character.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
								}
							});
						}
					}
					break;
			}
		}

		private void OnRankRefreshModelView(IEvent ievent)
		{
			var e = ievent as RankRefreshModelView;
			var info = e.Info;
			ModelRoot.DestroyFakeCharacter();
			ItemBaseData elfData = null;
			var elfId = -1;
			if (info.Equips.ItemsChange.TryGetValue((int)eBagType.Elf, out elfData))
			{
				elfId = elfData.ItemId;
			}
			if (e.Iselfrank)
			{
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

				var offset = tableNpc.CameraHeight / 10000.0f;
				ModelRoot.Create(dataId, null, character =>
				{
					character.SetScale(tableNpc.CameraMult / 10000.0f);
					character.ObjTransform.localPosition = new Vector3(0, offset - 0.48f, 0);
				});
			}
			else
			{
				ModelRoot.Create(info.TypeId, info.EquipsModel, character => { ModelDrag.Target = character.transform; },
					elfId, true);
			}
		}

		private void OnUIReset()
		{
			{
				var __list2 = MenuList;
				var __listCount2 = __list2.Count;
				for (var __i2 = 0; __i2 < __listCount2; ++__i2)
				{
					var toggle = __list2[__i2];
					{
						toggle.value = false;
						toggle.mIsActive = false;
						if (toggle.activeSprite != null)
						{
							toggle.activeSprite.alpha = 0.0f;
						}
					}
				}
			}
			{
				var __list3 = RankList;
				var __listCount3 = __list3.Count;
				for (var __i3 = 0; __i3 < __listCount3; ++__i3)
				{
					var transform1 = __list3[__i3];
					{
						transform1.gameObject.SetActive(false);
					}
				}
			}
			ModelRoot.DestroyFakeCharacter();
		}

		private void RemoveBindEvent()
		{
			EventDispatcher.Instance.RemoveEventListener(RankRefreshModelView.EVENT_TYPE, OnRankRefreshModelView);
			EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
			EventDispatcher.Instance.RemoveEventListener(RankNotifyLogic.EVENT_TYPE, OnRankNotifyLogic);
			Binding.RemoveBinding();
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
}