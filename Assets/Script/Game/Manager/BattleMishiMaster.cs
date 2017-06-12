using System;
#region using

using System.Collections;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

//战盟雕像

public class BattleMishiMaster : MonoBehaviour
{
    public Vector3 ForwardAngle;
    private ObjFakeCharacter mFackeCharacter;
    public Vector3 Offset;
    public Vector3 Scale = Vector3.one;

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (null != mFackeCharacter)
        {
            mFackeCharacter.Destroy();
            mFackeCharacter.OnWingLoadedCallback = null;
            mFackeCharacter = null;
        }

        EventDispatcher.Instance.RemoveEventListener(BattleMishiRefreshModelMaster.EVENT_TYPE, OnModelRefresh);
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    private void OnModelRefresh(IEvent ievent)
    {
        RefreshModel();
    }

    private void OnWingLoaded(GameObject go)
    {
        var ani = go.GetComponent<Animation>();
        if (null != ani)
        {
            ani.enabled = false;
        }
    }

    private void RefreshModel()
    {
        var info = PlayerDataManager.Instance.BattleMishiMaster;
        if (null == info)
        {
            if (null != mFackeCharacter)
            {
                mFackeCharacter.Destroy();
                mFackeCharacter.OnWingLoadedCallback = null;
                mFackeCharacter = null;
            }

            return;
        }
        var dataId = info.TypeId;
        var objId = info.Id;
        var equip = info.EquipsModel;
        var name = String.Format(GameUtils.GetDictionaryText(300000059),info.Name);


        /*
		var info = ObjManager.Instance.MyPlayer;
		var dataId = info.GetDataId();
		var objId = info.GetObjId();
		var equip = info.EquipList;
		var name = info.Name;
		var allianceName = "WWWWW";
		*/
        if (mFackeCharacter != null)
        {
            mFackeCharacter.Destroy();
        }
        mFackeCharacter = ObjFakeCharacter.Create(dataId, equip, character =>
        {
            if (null == mFackeCharacter)
            {
                character.Destroy();
                return;
            }

            if (character.GetObjId() != mFackeCharacter.GetObjId())
            {
                character.Destroy();
                return;
            }

            var collider = character.gameObject.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0, 1, 0);
            collider.height = 2;

            //character.transform.parent = transform;
            character.transform.position = gameObject.transform.position + Offset;
            character.transform.forward = Quaternion.Euler(ForwardAngle.x, ForwardAngle.y, ForwardAngle.z)*
                                          Vector3.forward;
            character.transform.localScale = Scale;
            //var anis = character.gameObject.GetComponentsInChildren<Animation>();
            //StartCoroutine(StopAni(0.8f));
            character.CreateNameBoard(name);
        }, 0, false, -1, objId);
        mFackeCharacter.SetObjId(objId);
        //mFackeCharacter.OnWingLoadedCallback = OnWingLoaded;
        mFackeCharacter.gameObject.layer = LayerMask.NameToLayer("ObjLogic");
        mFackeCharacter.iType = (int)eFakeCharacterTypeDefine.MieShiFakeCharacterType;
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        RefreshModel();
        EventDispatcher.Instance.AddEventListener(BattleMishiRefreshModelMaster.EVENT_TYPE, OnModelRefresh);
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    private IEnumerator StopAni(float time)
    {
        yield return new WaitForSeconds(time);
        if (null != mFackeCharacter)
        {
            mFackeCharacter.GetAnimationController().Stop(true);
        }
    }
}