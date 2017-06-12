using DataTable;
using System.ComponentModel;
using ClientDataModel;
using GameUI;
using System;
#region using

using EventSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

#endregion

namespace GameUI
{
	public class UIAcientBattleFieldFrame : MonoBehaviour
	{
		public AcientBattleFieldDataModel DataModel;
		public BindDataRoot Binding;
		private bool removeBind = true;

		public UIDragRotate DrageRotate;
		public CreateFakeCharacter ModelRoot;
		private int UniqueResourceId;
		private static int sUniqueResourceId = 12345;

		private void Awake()
		{

		}

		private void OnEnable()
		{
#if !UNITY_EDITOR
try
{
#endif

			if (removeBind)
			{
				var controller = UIManager.Instance.GetController(UIConfig.AcientBattleFieldFrame);
				DataModel = controller.GetDataModel("") as AcientBattleFieldDataModel;
				Binding.SetBindDataSource(DataModel);
				//PageBindDataRoot.SetBindDataSource(DataModel.ActivityTermList[DataModel.CurrentSelectPageIdx]);
				DataModel.PropertyChanged += OnEventPropertyChanged;
				CreateCopyObj(DataModel.ModelId);
			}
			removeBind = true;

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

			if (removeBind)
			{
				RemoveBindingEvent();
			}
			CreateCopyObj(-1);
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
		}

		private void OnDestroy()
		{
#if !UNITY_EDITOR
try
{
#endif


			if (removeBind == false)
			{
				RemoveBindingEvent();
			}
			removeBind = true;

		
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}



		private void RemoveBindingEvent()
		{
			EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUIBindingRemove);

			Binding.RemoveBinding();
			DataModel.PropertyChanged -= OnEventPropertyChanged;
		}

		private void OnEventPropertyChanged(object o, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "ModelId")
			{
				CreateCopyObj(DataModel.ModelId);
			}
		}

		public static int GetNextUniqueResourceId()
		{
			return sUniqueResourceId++;
		}

		private void CreateCopyObj(int dataId)
		{
			if (-1 == dataId)
			{
				ModelRoot.DestroyFakeCharacter();
				return;
			}


			ModelRoot.Create(dataId, null, character =>
			{
				character.transform.forward = Vector3.back;
				character.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
				DrageRotate.Target = character.transform;
			});

			
		}

		public void OnClickClose()
		{
			var e = new Close_UI_Event(UIConfig.AcientBattleFieldFrame);
			EventDispatcher.Instance.DispatchEvent(e);
		}

		private void OnCloseUIBindingRemove(IEvent ievent)
		{
			var e = ievent as CloseUiBindRemove;
			if (e.Config != UIConfig.AcientBattleFieldFrame)
			{
				return;
			}
			if (e.NeedRemove == 0)
			{
				removeBind = false;
			}
			else
			{
				if (removeBind == false)
				{
					RemoveBindingEvent();
				}
				removeBind = true;
			}
		}

		private void ChangePageEvent(IEvent ievent)
		{

		}


	}
}