using System;
#region using

using System.Collections;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

//战盟雕像

public class BattleUnionMaster : MonoBehaviour
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

        EventDispatcher.Instance.RemoveEventListener(BattleUnionRefreshModelView.EVENT_TYPE, OnModelRefresh);
    
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
        var info = PlayerDataManager.Instance.BattleUnionMaster;
        if (null == info)
        {
            return;
        }
        var dataId = info.TypeId;
        var objId = info.Id;
        var equip = info.EquipsModel;
        var name = info.Name;
        var allianceName = string.Empty;
        var battleDic = PlayerDataManager.Instance._battleCityDic;
        foreach (var item in battleDic)
        {
            if (item.Value.Type == 0)
            {
                allianceName = item.Value.Name;
                break;
            }
        }

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
            var titles = new Dictionary<int, string>();
            titles.Add(2000, allianceName);
            titles.Add(5000, allianceName);
            character.CreateNameBoard(name, titles);
        }, 0, false, -1, objId);
        mFackeCharacter.SetObjId(objId);
        //mFackeCharacter.OnWingLoadedCallback = OnWingLoaded;
        mFackeCharacter.gameObject.layer = LayerMask.NameToLayer("ObjLogic");
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        RefreshModel();
        EventDispatcher.Instance.AddEventListener(BattleUnionRefreshModelView.EVENT_TYPE, OnModelRefresh);
    
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