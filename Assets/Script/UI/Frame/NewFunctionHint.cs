#region using

using System;
using System.Collections;
using System.Collections.Generic;
using EventSystem;
using GameUI;
using UnityEngine;


#endregion

public class NewFunctionHint : MonoBehaviour
{
    private int CurrentLevel;
    public float DistanceOffset = 0.1f;
    public float LerpTimeRate = 2;
	public MainButton MainUiButtonLogic;
    public List<NewFunction> NewFunctionList;
    public float StayDelay = 2.0f;

    private IEnumerator AutoClose(float time, GameObject go, GameObject btn)
    {
        go.active = true;
        yield return new WaitForSeconds(time);

        var distanc = Vector3.zero;
        while (true)
        {
            distanc = btn.transform.position - go.transform.position;
            if (distanc.magnitude < DistanceOffset)
            {
                break;
            }
            go.transform.position = Vector3.Lerp(go.transform.position, btn.transform.position,
                Time.deltaTime*LerpTimeRate);
            yield return null;
        }


        go.active = false;
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(Event_LevelChange.EVENT_TYPE, OnLevelChanged);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Use this for initialization
    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        foreach (var item in NewFunctionList)
        {
            item.Go.active = false;
        }
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    private void OnLevelChanged(IEvent ievent)
    {
        var level = PlayerDataManager.Instance.GetRes((int) eResourcesType.LevelRes);

        NewFunction newFun = null;
        foreach (var item in NewFunctionList)
        {
            if (CurrentLevel < item.Level && level >= item.Level)
            {
                newFun = item;
            }
        }

        CurrentLevel = level;

        if (null != newFun)
        {
            Show(newFun.Go, newFun.Btn);
        }
    }

    private void Show(GameObject go, GameObject btn)
    {
        if (!gameObject.active)
        {
            return;
        }
		/*
        var name = btn.transform.parent.name;
        if (name.Contains("1"))
        {
            MainUiButtonLogic.OnClickType1();
        }
        else if (name.Contains("2"))
        {
            MainUiButtonLogic.OnClickType2();
        }
        else if (name.Contains("3"))
        {
            MainUiButtonLogic.OnClickType3();
        }
        else if (name.Contains("4"))
        {
            MainUiButtonLogic.OnClickType4();
        }
        StartCoroutine(AutoClose(StayDelay, go, btn));
		*/
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        CurrentLevel = PlayerDataManager.Instance.GetRes((int) eResourcesType.LevelRes);

        EventDispatcher.Instance.AddEventListener(Event_LevelChange.EVENT_TYPE, OnLevelChanged);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    [Serializable]
    public class NewFunction
    {
        public GameObject Btn;
        public GameObject Go;
        public int Level;
    }
}