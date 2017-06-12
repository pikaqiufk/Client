using System;
#region using

using System.Collections;
using System.Collections.Generic;
using DataTable;
using PathologicalGames;
using UnityEngine;


#endregion

namespace GameUI
{
	public class OverheadFrame : MonoBehaviour
	{
	    public UILabel Label;
	    public StackLayout LayoutTitle;
	    public WorldTo2DCameraConstraint mConstraint;
	    private float mOffset;
	    private bool mRefreshTitle;
	    public UILabel PopTalkLabel;
	    public List<OverheadTitleFrame> TitleList;
	
	    private IEnumerator AutoHide(float time)
	    {
	        yield return new WaitForSeconds(time);
	        PopTalkLabel.gameObject.SetActive(false);
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mRefreshTitle)
	        {
	            if (LayoutTitle != null)
	            {
	                LayoutTitle.ResetLayout();
	            }
	            mRefreshTitle = false;
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
	
	        StopAllCoroutines();
	        PopTalkLabel.gameObject.SetActive(false);
	
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
	        PopTalkLabel.gameObject.SetActive(false);
	        mRefreshTitle = true;
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public virtual void Reset()
	    {
	        foreach (var nameBoardTitleLogic in TitleList)
	        {
	            nameBoardTitleLogic.gameObject.SetActive(false);
	        }
	    }
	
	    public void ResetOffset()
	    {
	        mConstraint.offset = new Vector3(0, mOffset, 0);
	    }
	
	    public void RestLayoutTitle()
	    {
	        mRefreshTitle = true;
	    }
	
	    public void SetBattleColor(int camp)
	    {
	        TitleList[0].SetBattleColor(camp);
	    }
	
	    public void SetFlyOffset()
	    {
	        mConstraint.offset = new Vector3(0, mOffset - 0.2f, 0);
	    }
	
	    public void SetOwner(GameObject owner, GameObject root, float offset)
	    {
	        var objTransform = gameObject.transform;
	        //objTransform.parent = root.transform;
	        objTransform.SetParentEX(root.transform);
	        objTransform.localScale = new Vector3(1, 1, 1);
	
	        mOffset = offset;
	
	        mConstraint.target = owner.transform;
	        mConstraint.offset = new Vector3(0, offset, 0);
	        mConstraint.orthoCamera = UIManager.Instance.UICamera;
	        mConstraint.targetCamera = GameLogic.Instance.MainCamera;
	    }
	
	    public void SetText(string str)
	    {
	        Label.text = str;
	    }
	
	    public void SetText(string str, Color col)
	    {
	        var colStr = GameUtils.ColorToString(col);
	        str = string.Format("[{0}]{1}[-]", colStr, str);
	        SetText(str);
	    }
	
	    public void SetTitle(int pos, int titleId, string TitleName, bool isMySelf)
	    {
	        var go = TitleList[pos].gameObject;
	        if (titleId == -1)
	        {
	            go.SetActive(false);
	        }
	        else
	        {
	            var tbNameTitle = Table.GetNameTitle(titleId);
	            if (tbNameTitle != null)
	            {
	                var active = true;
	                TitleList[pos].SetTitle(tbNameTitle, TitleName, ref active);
	                if (go.activeSelf != active)
	                {
	                    go.SetActive(active);
	                }
	            }
	        }

            if (!GameSetting.Instance.ShowOtherPlayer && !isMySelf)
            {
                go.SetActive(false);
            }
	    }

        public void ShowHideOtherTitle(bool isSHow, int pos, int titleId, string TitleName)
	    {
            if (isSHow)
            {
                SetTitle(pos, titleId, TitleName, false);
            }
            else
            {
                if (pos < TitleList.Count)
                {
                    var go = TitleList[pos].gameObject;
                    go.SetActive(false);
                } 
            }
	    }
	
	    // Use this for initialization
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
	
	    public void Talk(string str, float time = 4)
	    {
	        PopTalkLabel.gameObject.SetActive(true);
	        PopTalkLabel.text = str;
	        StopAllCoroutines();
	        StartCoroutine(AutoHide(time));
	        RestLayoutTitle();
	    }
	}
}