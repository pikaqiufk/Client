#region using

using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

#endregion

public class CGLogic : MonoBehaviour
{
    public Camera CGCamera;
    public TweenAlpha Fadein;
    public TweenAlpha Fadeout;
    public UILabel Label;
    public List<PopTalkBoard> mListPop = new List<PopTalkBoard>();
    private DateTime mShowDialogTime;
    //public UIPanel PopTalkRoot;
    public Transform ObjRoot;
    public GameObject PopTalk;
    public GameObject SkipBtn;
    public Camera UICamera;

    public void DoFadein(float time)
    {
        Fadein.gameObject.SetActive(true);
        Fadeout.gameObject.SetActive(false);
        Fadein.ResetToBeginning();
        Fadein.duration = time;
        Fadein.Play();
    }

    public void DoFadeout(float time)
    {
        Fadeout.gameObject.SetActive(true);
        Fadein.gameObject.SetActive(false);
        Fadeout.ResetToBeginning();
        Fadeout.duration = time;
        Fadeout.Play();
    }

    public PopTalkBoard GetFreePop()
    {
        {
            var __list1 = mListPop;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var pop = __list1[__i1];
                {
                    if (!pop.gameObject.active)
                    {
                        return pop;
                    }
                }
            }
        }
        var go = Instantiate(PopTalk.transform.gameObject) as GameObject;
        go.transform.parent = PopTalk.transform.parent;
        var constraint = go.GetComponent<WorldTo2DCameraConstraint>();
        constraint.targetCamera = CGCamera;
        constraint.orthoCamera = UICamera;
        var p = go.GetComponent<PopTalkBoard>();
        mListPop.Add(p);

        return p;
    }

    public void ShowDialog(string str, float time)
    {
        Label.text = str;
        mShowDialogTime = DateTime.Now.AddSeconds(time);
        Label.gameObject.SetActive(true);
    }

    public void Skip()
    {
        SkipBtn.gameObject.SetActive(false);
        StartCoroutine(SkipCoroutine(2));
    }

    private IEnumerator SkipCoroutine(float time)
    {
        Fadeout.gameObject.SetActive(true);
        Fadeout.ResetToBeginning();
        Fadeout.Play();
        SoundManager.Instance.StopAllSoundEffect();
        yield return new WaitForSeconds(time);
        PlayCG.Instance.Stop(true);
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

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        if (Label.gameObject.active)
        {
            if (DateTime.Now > mShowDialogTime)
            {
                Label.gameObject.SetActive(false);
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