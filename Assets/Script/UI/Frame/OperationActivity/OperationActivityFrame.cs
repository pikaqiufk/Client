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
	public class OperationActivityFrame : MonoBehaviour
	{
		public OperationActivityTotalDataModel DataModel;
		public BindDataRoot Binding;
		private bool removeBind = true;

		public BindDataRoot PageBindDataRoot;
		public BindDataRoot SubTableBindDataRoot;
		public Transform ModelRoot;
		private GameObject ShowModel;
		private int UniqueResourceId;
		private static int sUniqueResourceId = 12345;
		public UIDragRotate DragRotate;
		public List<Transform> LotteryRewards;
		public Transform LotteryCircle;
		private int LotteryIx = -1;
		private List<GameObject> mListMask = new List<GameObject>();

		private int RewardItemId = -1;
		private int RewardItemCount = 0;
		private void Awake()
		{
			foreach (var reward in LotteryRewards)
			{
				mListMask.Add(reward.transform.FindChild("Mask").gameObject);
			}
			
		}

		private void OnEnable()
		{
#if !UNITY_EDITOR
try
{
#endif

			if (removeBind)
			{
				var controller = UIManager.Instance.GetController(UIConfig.OperationActivityFrame);
				DataModel = controller.GetDataModel("") as OperationActivityTotalDataModel;
				Binding.SetBindDataSource(DataModel);
				PageBindDataRoot.SetBindDataSource(DataModel.ActivityTermList[DataModel.CurrentSelectPageIdx]);
				EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUIBindingRemove);
				EventDispatcher.Instance.AddEventListener(OperationActivityPage_Event.EVENT_TYPE, ChangePageEvent);
				EventDispatcher.Instance.AddEventListener(OperationActivitySubPagekEvent.EVENT_TYPE, ChangeSubTableEvent);
				EventDispatcher.Instance.AddEventListener(OperationActivityDrawLotteryEvent.EVENT_TYPE, OnOperationActivityDrawLotteryEvent);
				DataModel.PropertyChanged += OnEventPropertyChanged;
			}
			removeBind = true;

			if (-1 == LotteryIx)
			{
				LotteryCircle.gameObject.SetActive(false);
			}
			else
			{
				LotteryCircle.gameObject.SetActive(true);
				LotteryCircle.localPosition = LotteryRewards[LotteryIx].localPosition;	
			}

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
			StopCoroutine(LotteryAnimationEffectCoroutine());
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
			EventDispatcher.Instance.RemoveEventListener(OperationActivityPage_Event.EVENT_TYPE, ChangePageEvent);
			EventDispatcher.Instance.RemoveEventListener(OperationActivitySubPagekEvent.EVENT_TYPE, ChangeSubTableEvent);
			EventDispatcher.Instance.RemoveEventListener(OperationActivityDrawLotteryEvent.EVENT_TYPE, OnOperationActivityDrawLotteryEvent);
			PageBindDataRoot.RemoveBinding();
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
				if (null != ShowModel)
				{
					ComplexObjectPool.Release(ShowModel);
					ShowModel = null;
					DragRotate.Target = null;
				}
				return;
			}
			else
			{
				UniqueResourceId = GetNextUniqueResourceId();
				var resId = UniqueResourceId;
				var tb = Table.GetCharModel(dataId);
				if (null == tb)
				{
					return;
				}
				ComplexObjectPool.NewObject(tb.ResPath, go =>
				{
					if (resId != UniqueResourceId)
					{
						return;
					}
					ShowModel = go;
					go.transform.parent = ModelRoot.transform;
					go.transform.localPosition = Vector3.zero;
					go.transform.localRotation = Quaternion.identity;
					go.transform.localScale = Vector3.one;
					go.gameObject.SetLayerRecursive(LayerMask.NameToLayer(GAMELAYER.UI));
					DragRotate.Target = ShowModel.transform;
                    ModelRoot.GetComponent<ChangeRenderQueue>().RefleshRenderQueue();
					//go.gameObject.SetRenderQueue(ModelRenderQueue);
				});
			}

			
		}

		public void OnClickClose()
		{
			var e = new Close_UI_Event(UIConfig.OperationActivityFrame);
			EventDispatcher.Instance.DispatchEvent(e);
		}

		private void OnCloseUIBindingRemove(IEvent ievent)
		{
			var e = ievent as CloseUiBindRemove;
			if (e.Config != UIConfig.OperationActivityFrame)
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
			var e = ievent as OperationActivityPage_Event;

			if (null!=PageBindDataRoot && e.Idx>=0 && e.Idx<DataModel.ActivityTermList.Count)
			{
				PageBindDataRoot.SetBindDataSource(DataModel.ActivityTermList[DataModel.CurrentSelectPageIdx]);
				RefreshMask();
			}
		}

		private void ChangeSubTableEvent(IEvent ievent)
		{
			var data = DataModel.ActivityTermList[DataModel.CurrentSelectPageIdx].TableData;
			SubTableBindDataRoot.SetBindDataSource(data.TableList[data.SubTabIdx]);
		}

		private void OnOperationActivityDrawLotteryEvent(IEvent ievent)
		{
			var e = ievent as OperationActivityDrawLotteryEvent;
			int Idx = e.Idx;

			
			if (-1 == Idx)
			{
				LotteryIx = Idx;
				RewardItemId = e.ItemId;
				RewardItemCount = e.ItemCount;
				StartLotteryAnimation();
			}
			else if (-2 == Idx)
			{
				RefreshMask();
			}
			else
			{
				LotteryIx = Idx;
				RewardItemId = e.ItemId;
				RewardItemCount = e.ItemCount;
				EndLotteryAnimation(LotteryIx);
			}
		}


		private void StartLotteryAnimation()
		{
			LotteryCircle.gameObject.SetActive(true);
			StartCoroutine(LotteryAnimationEffectCoroutine());
		}
		IEnumerator LotteryAnimationEffectCoroutine()
		{
			int i = 0;
			float speed = 0.05f;
			while (i!=LotteryIx)
			{
				i = (++i)%LotteryRewards.Count;
				LotteryCircle.localPosition = LotteryRewards[i].localPosition;
				if (-1 != LotteryIx)
				{
					speed += 0.01f;
				}
				yield return new WaitForSeconds(speed);
			}
			yield return new WaitForSeconds(0.1f);
			if (-1 != RewardItemId && RewardItemCount > 0)
			{
				var dict = new Dictionary<int, int>();
				dict.Add(RewardItemId, RewardItemCount);
				var e1 = new ShowItemsArguments
				{
					Items = dict
				};
				EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ShowItemsFrame, e1));

			}

			RefreshMask();
		}

		private void EndLotteryAnimation(int idx)
		{
			//StopCoroutine(LotteryAnimationEffectCoroutine());
// 			StopAllCoroutines();
// 			if (idx >= 0 && idx < LotteryRewards.Count)
// 			{
// 				LotteryCircle.localPosition = LotteryRewards[idx].localPosition;	
// 			}
		}
		public void OnDrawLotteryBtnClick()
		{
			EventDispatcher.Instance.DispatchEvent(new OperationActivityClaimReward(1));
		}
		public void OnResetLotteryBtnClick()
		{
			EventDispatcher.Instance.DispatchEvent(new OperationActivityClaimReward(0));
		}

		private void RefreshMask()
		{
			if (null == mListMask )
			{
				return;
			}
			if (mListMask.Count <= 0)
			{
				return;
			}

			if (DataModel.CurrentSelectPageIdx < 0 || DataModel.CurrentSelectPageIdx >= DataModel.ActivityTermList.Count)
			{
				return;
			}

			var currentData = DataModel.ActivityTermList[DataModel.CurrentSelectPageIdx];
			if ((OperationActivityUIType) currentData.UIType == OperationActivityUIType.Lottery)
			{
				var tabModel = currentData.LotteryData;
				for (int i = 0; i < tabModel.ActivityItemList.Count && i < mListMask.Count; i++)
				{
					if (1==tabModel.ActivityItemList[i].HasGotReward)
					{
						mListMask[i].SetActive(true);
					}
					else
					{
						mListMask[i].SetActive(false);
					}
				}
			}
			
			
		}

	}
}