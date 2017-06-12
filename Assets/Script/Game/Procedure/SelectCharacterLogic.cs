using UnityEngine;
using System.Collections;
using EventSystem;
using System.Collections.Generic;
using System;
using System.Linq;
using GameUI;

public class SelectCharacterLogic : MonoBehaviour {

	//状态类型
	public enum StateType
	{
		None,
		SelectMyRole,
		CreateNewRole,
	}

	public Vector3 CameraOffset = Vector3.zero;
	//主摄像机
	public CameraMoveToTarget MainCamera;
	
	//资源路径
// 	public string[] ModelPath =
//     {
//         "Model/CreateFrameModel/DL_JianShi_New.prefab",
//         "Model/CreateFrameModel/DL_FaShi.prefab",
//         "Model/CreateFrameModel/DL_GongShou.prefab"
//     };
	public GameObject[] CreateNewRolePrefab;

	//选择角色根节点
	public GameObject SelectModelRoot;
	//选择角色的锚点
	public GameObject[] SelectRoleAnchor;
	//选择角色用的角色
	private List<CreateFakeCharacter> mSelectRoleList = new List<CreateFakeCharacter>();

	//创建角色根节点
	public GameObject CreateModelRoot;
	//创建角色的锚点
	public GameObject[] CreateRoleAnchor;
	//创建角色用的角色
	private List<EstablishRole> mCreateRoleList = new List<EstablishRole>();

	//当前状态类型
	private StateType mStateType = StateType.None;

	//当前选中的新角色索引
	private int mCurrentSelectNewRoleIdx = -1;
	//当前所选的索引
	private int mCurrentSelectIdx = 0;


	void Awake()
	{
#if !UNITY_EDITOR
try
{
#endif

		if (null == MainCamera)
		{
			MainCamera = Camera.main.GetComponent<CameraMoveToTarget>();
			if (null == MainCamera)
			{
				Camera.main.gameObject.AddComponent<CameraMoveToTarget>();
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

	void Start () {
#if !UNITY_EDITOR
try
{
#endif


		EventDispatcher.Instance.AddEventListener(UIEvent_SelectRole_Index.EVENT_TYPE, OnSelectMyRoleEvent);
		EventDispatcher.Instance.AddEventListener(UIEvent_CreateRoleType_Change.EVENT_TYPE, OnSelectCreateRoleEvent);
		EventDispatcher.Instance.AddEventListener(UIEvent_SelectRole_Back.EVENT_TYPE, OnSelectRoleEvent);
		EventDispatcher.Instance.AddEventListener(UIEvent_ShowCreateRole.EVENT_TYPE, OnCreateRoleEvent);
		EventDispatcher.Instance.AddEventListener(CreateRole_DragEvent.EVENT_TYPE, OnCreateRoleDragEvent);

		if(PlayerDataManager.Instance.CharacterLists.Count>0)
		{//有角色进入上次选择的最后一个角色
			var idx = GetLastSelectRoleIdx();

			//显示UI
			UIManager.Instance.ShowUI(UIConfig.SelectRoleUI, new SelectRoleArguments
			{
				CharacterSimpleInfos = PlayerDataManager.Instance.CharacterLists,
				SelectId = PlayerDataManager.Instance.SelectedRoleIndex,
				Type = SelectRoleArguments.OptType.SelectMyRole
			});

			//进入选择角色流程
			SwitchMode(StateType.SelectMyRole, idx);
		}
		else
		{
			UIManager.Instance.ShowUI(UIConfig.SelectRoleUI, new SelectRoleArguments
			{
				CharacterSimpleInfos = PlayerDataManager.Instance.CharacterLists,
				SelectId = 0,
				Type = SelectRoleArguments.OptType.CreateNewRole
			});

			//进入创建角色流程
			SwitchMode(StateType.CreateNewRole,0); 
		}

		//显示转圈
		UIManager.Instance.ShowBlockLayer();
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	void OnDestroy()
	{
#if !UNITY_EDITOR
try
{
#endif

		EventDispatcher.Instance.RemoveEventListener(UIEvent_SelectRole_Index.EVENT_TYPE, OnSelectMyRoleEvent);
		EventDispatcher.Instance.RemoveEventListener(UIEvent_CreateRoleType_Change.EVENT_TYPE, OnSelectCreateRoleEvent);
		EventDispatcher.Instance.RemoveEventListener(UIEvent_SelectRole_Back.EVENT_TYPE, OnSelectRoleEvent);
		EventDispatcher.Instance.RemoveEventListener(UIEvent_ShowCreateRole.EVENT_TYPE, OnCreateRoleEvent);
		EventDispatcher.Instance.RemoveEventListener(CreateRole_DragEvent.EVENT_TYPE, OnCreateRoleDragEvent);
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	void OnSelectMyRoleEvent(IEvent ievent)
	{
		var ee = ievent as UIEvent_SelectRole_Index;
		var nIndex = ee.index;
		SelectMyRole(nIndex);
	}

	void OnSelectCreateRoleEvent(IEvent ievent)
	{
		var ee = ievent as UIEvent_CreateRoleType_Change;
		var nIndex = ee.index;

		SelectNewRole(nIndex);
	}
	void OnSelectRoleEvent(IEvent ievent)
	{
		
	}
	
	void OnCreateRoleEvent(IEvent ievent)
	{
		var ee = ievent as UIEvent_ShowCreateRole;
		var nIndex = ee.index;
		if (0 == nIndex)
		{
			SwitchMode(StateType.SelectMyRole, GetLastSelectRoleIdx());
		}
		else
		{
			SwitchMode(StateType.CreateNewRole,0);

            var eeee = new UIEvent_CreateRoleType_Change(0);
            EventDispatcher.Instance.DispatchEvent(eeee);
		}
		
	}

	private void OnCreateRoleDragEvent(IEvent ievent)
	{
		var e = ievent as CreateRole_DragEvent;
		if (MainCamera.IsMoving)
		{
			return;
		}

        if (StateType.SelectMyRole == mStateType)
	    {
            if (mCurrentSelectIdx < 0 || mCurrentSelectIdx >= mSelectRoleList.Count)
	        {
	            return;
	        }
            var role = mSelectRoleList[mCurrentSelectIdx];
            if (null == role)
            {
                return;
            }
            if (role.Character == null)
            {
                return;
            }
            if (role.Character.transform == null)
	        {
	            return;
	        }
            role.Character.transform.RotateAround(Vector3.up, -e.Delta.x * Time.deltaTime);
	    }
        else if (StateType.CreateNewRole == mStateType)
        {
            if (mCurrentSelectNewRoleIdx < 0 || mCurrentSelectNewRoleIdx >= mCreateRoleList.Count)
            {
                return;
            }

            var role = mCreateRoleList[mCurrentSelectNewRoleIdx];
            if (null == role)
            {
                return;
            }
            if (role.IsPlayingAction)
            {
                return;
            }
            role.transform.RotateAround(Vector3.up, -e.Delta.x * Time.deltaTime);
        }
	}

	//获得上次选择的角色索引
	int GetLastSelectRoleIdx()
	{
		var data = PlayerDataManager.Instance;
		if (null != data && null != data.CharacterLists)
		{
			for (int i = 0; i < data.CharacterLists.Count; i++)
			{
				if (data.SelectedRoleIndex == data.CharacterLists[i].CharacterId)
				{
					return i;
				}
			}
		}
		return 0;
	}

	//切换流程
	void SwitchMode(StateType type,int idx)
	{
		if (type == mStateType)
		{
			return;
		}
		
		mStateType = type;
		mCurrentSelectIdx = idx;

		if (StateType.SelectMyRole == mStateType)
		{
			CreateModelRoot.SetActive(false);
			SelectModelRoot.SetActive(true);

			EnterSelectRole(idx);
		}
		else if (StateType.CreateNewRole == mStateType)
		{
			CreateModelRoot.SetActive(true);
			SelectModelRoot.SetActive(false);

			EnterCreateRole();
		}
		
	}
	void EnterSelectRole(int idx)
	{
		CameraFadeEffect.DoCameraFade(gameObject,
		new Color(0, 0, 0, 1),
		new Color(0, 0, 0, 0),
		1.5f);

		StartCoroutine(CreateMyRoleCoroutine(() =>
		{
			for (int i = 0; i < mCreateRoleList.Count; i++)
			{
				mCreateRoleList[i].PlayStand();
			}
			//load over
			SelectMyRole(idx);

			UIManager.Instance.RemoveBlockLayer();
		}));

	}

	//造我的角色列表
	IEnumerator CreateMyRoleCoroutine(Action onLoadOver)
	{
		if (mSelectRoleList.Count > 0)
		{
			if (null != onLoadOver)
			{
				onLoadOver();
			}
			yield break;
		}

		var roleList = PlayerDataManager.Instance.CharacterLists;

	    int loadCount = 0;
		for (int i=0; i<roleList.Count; i++)
		{
		    int idx = i;
            var roleInfo = roleList[idx];
			var elfId = roleInfo.FightElfId;
			if (elfId <= 0)
			{
				elfId = -1;
			}

            var creater = SelectRoleAnchor[idx].GetComponent<CreateFakeCharacter>();
			creater.DestroyFakeCharacter();
			creater.ElfOffset.x = 0.6f;
			creater.ElfOffset.z = 0;
            mSelectRoleList.Add(creater);

			creater.Create(roleInfo.RoleId, roleInfo.EquipsModel, character =>
			{

				character.transform.rotation = Quaternion.Euler(0, 180, 0);
				character.transform.localPosition = Vector3.zero;
				/*
				var characterController = GameObject.Find("SelectDrag");
				if (null != characterController)
				{
					var draglogic = characterController.GetComponent<CreateRoleDragLogic>();
					if (draglogic)
					{
						draglogic.Model = character;
						draglogic.CharacterTypeId = roleInfo.RoleId;
						draglogic.IsEquipWeapon = roleInfo.EquipsModel.ContainsKey(17);
						var drag = draglogic.gameObject.GetComponent<UIDragRotate>();
						drag.Target = character.transform;
					}
				}

				ModelLoadFinish = true;
				 */
			    loadCount++;

				SetColor(creater.gameObject, Color.black);

                //fix 边上人物武器跑到当前人物身边的问题
			    var model = character.GetModel();
			    if (null != model)
			    {
			        var ani = model.GetComponent<Animation>();
			        if (ani != null) ani.cullingType = AnimationCullingType.AlwaysAnimate;
			    }
			}, elfId, false, 0, false);
		}

		while (true)
		{
            if (loadCount >= roleList.Count)
			{
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}

		if (null != onLoadOver)
		{
			onLoadOver();
		}
	}

	void EnterCreateRole()
	{
		CameraFadeEffect.DoCameraFade(gameObject,
		new Color(0, 0, 0, 1),
		new Color(0, 0, 0, 0),
		1.5f);

		StartCoroutine(CreateNewRoleCoroutine(()=>
		{
			//load over
			SelectNewRole(0);
			UIManager.Instance.RemoveBlockLayer();
		}
	));
	}

	//造可创建的职业
	IEnumerator CreateNewRoleCoroutine(Action onLoadOver)
	{
		if (mCreateRoleList.Count>0)
		{
			if (null != onLoadOver)
			{
				onLoadOver();
			}
			yield break;
		}

		/*
		for (int i=0 ; i<ModelPath.Length; i++)
		{
			var path = ModelPath[i];
			var holder = ResourceManager.PrepareResourceWithHolder<GameObject>(path);
			yield return holder.Wait();
		
			var res = holder.Resource;
		*/
		for(int i=0; i<CreateNewRolePrefab.Length; i++)
		{
			var res = CreateNewRolePrefab[i];
			if (null == res)
			{
				Logger.Error("CreateRoleCoroutine null == res");
				continue;
			}
			var role = Instantiate(res) as GameObject;
			if (role == null)
			{
				Logger.Error("CreateRoleCoroutine null == res");
				continue;
			}
			var roleLogic = role.GetComponent<EstablishRole>();
			var t = role.transform;
			t.parent = CreateRoleAnchor[i].transform;
			t.gameObject.SetActive(true);
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.Euler(0, 180, 0);
			//t.localScale = Vector3.one;

			mCreateRoleList.Add(roleLogic);
			roleLogic.PlayStand();
			//SetColor(roleLogic.gameObject, Color.black);
			//roleLogic.ShowEffect(false);
			yield return new WaitForEndOfFrame();
		}

		if (null != onLoadOver)
		{
			onLoadOver();
		}
	}

	// Update is called once per frame
	void Update () {
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

	//选择某个角色
	void SelectMyRole(int idx)
	{
	    mCurrentSelectIdx = idx;

        if (idx < mSelectRoleList.Count && mSelectRoleList[idx].Character != null && mSelectRoleList[idx].Character.transform != null)
	    {
            mSelectRoleList[idx].Character.transform.rotation = Quaternion.Euler(0, 180, 0);
	    }

	    EventDispatcher.Instance.DispatchEvent(new UIEvent_SelectCharacter_ShowUIAnimation(0, idx, StateType.SelectMyRole));
		MainCamera.MoveTo(SelectRoleAnchor[idx].transform.position + CameraOffset, () =>
		{
			for (int i = 0; i < mSelectRoleList.Count; i++)
			{
				if (i == idx)
				{
					SetColor(mSelectRoleList[i].gameObject, Color.white); 
				}
				else
				{
					SetColor(mSelectRoleList[i].gameObject, Color.black);
				}
			}
            EventDispatcher.Instance.DispatchEvent(new UIEvent_SelectCharacter_ShowUIAnimation(1, idx, StateType.SelectMyRole));
		});
	}

	//选择某个职业
	void SelectNewRole(int idx)
	{
		mCurrentSelectNewRoleIdx = idx;

        EventDispatcher.Instance.DispatchEvent(new UIEvent_SelectCharacter_ShowUIAnimation(0, idx, StateType.CreateNewRole));
		mCreateRoleList[idx].gameObject.transform.rotation = Quaternion.Euler(0,180,0);
		MainCamera.MoveTo(CreateRoleAnchor[idx].transform.position, () =>
		{
			for(int i=0; i<mCreateRoleList.Count;i++)
			{
				if (i==idx)
				{
					SetColor(mCreateRoleList[i].gameObject, Color.white);
					mCreateRoleList[i].ShowEffect(true);
					mCreateRoleList[i].PlayAction(() =>
					{
						EventDispatcher.Instance.DispatchEvent(new UIEvent_SelectCharacter_ShowUIAnimation(1, idx, StateType.CreateNewRole));
					});
				}
				else
				{
					SetColor(mCreateRoleList[i].gameObject, Color.black);
					mCreateRoleList[i].ShowEffect(false);
					mCreateRoleList[i].PlayStand();
				}
			}
            
			
		});
	}

	void SetColor(GameObject go,Color col)
	{
		/*
		var list = go.GetComponentsInChildren<Renderer>();
		foreach (var renderer in list)
		{
			renderer.material.color = col;
		}
		 * */
	}
}
