using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class WingPartTrainningHelper : MonoBehaviour
{
    // Use this for initialization
    private List<Transform> balls;
    private List<Transform> lines;

    private void Awake()
    {
        balls = new List<Transform>();
        lines = new List<Transform>();
        var c = gameObject.transform.childCount + 1;
        for (var i = 1; i < c; i++)
        {
            var ball = transform.FindChild("ball" + i);
            balls.Add(ball);
            if (ball != null)
            {
                var line = ball.FindChild("line" + i);
                lines.Add(line);
            }
            else
            {
                lines.Add(null);
            }
        }
    }

    public void RefreshTrainningPart(int index, bool active, bool doAnimation)
    {
        if (index >= lines.Count) return;

        var ball = balls[index];
        if (null != ball)
        {
            SetGery(ball, !active);
            if (active && doAnimation)
            {
                DoBallEffect(ball);
            }
        }

        if (index == 0) return;

        var line = lines[index - 1];
        if (null == line) return;

        if (line.gameObject.activeSelf == false)
        {
            line.gameObject.SetActive(true);
        }

        var sp = line.transform.GetComponent<UISprite>();
        if (sp != null)
        {
            sp.fillAmount = 1;
        }

        SetGery(line, !active);
        if (doAnimation)
        {
            DoAnimation(line);
        }
    }

    private static void SetGery(Transform lineTrans, bool isGery)
    {
        var sp = lineTrans.GetComponent<UISprite>();
        if (sp != null)
        {
            var atlasName = sp.atlas.name;
            if (isGery)
            {
                if (atlasName.Contains("Grey"))
                {
                    return;
                }
                ResourceManager.PrepareResource<GameObject>("UI/Atlas/" + atlasName + "Grey.prefab", res =>
                {
                    if (lineTrans == null)
                    {
                        return;
                    }

                    sp.atlas = res.GetComponent<UIAtlas>();
                }, true, true, true, true, true);
            }
            else
            {
                if (!atlasName.Contains("Grey"))
                {
                    return;
                }

                atlasName = atlasName.Remove(atlasName.Length - 4, 4);
                ResourceManager.PrepareResource<GameObject>("UI/Atlas/" + atlasName + ".prefab", res =>
                {
                    if (lineTrans == null)
                    {
                        return;
                    }

                    sp.atlas = res.GetComponent<UIAtlas>();
                },true, true, true,true,true);
            }
        }
    }

    private void DoAnimation(Transform obj)
    {
        var sprite = obj.GetComponent<UISprite>();
        if (sprite != null)
        {
            StartCoroutine(AnimationCoroutine(sprite));
        }
    }

    private void DoBallEffect(Transform trans)
    {
        var child = trans.FindChild("UI_TuJianHeCheng(Clone)");
        NGUITools.Destroy(child);

        ResourceManager.PrepareResource<GameObject>("Effect/UI/UI_TuJianHeCheng.prefab", res =>
        {
            NGUITools.AddChild(trans.gameObject, res);
            StartCoroutine(DestroyEffect(1f, () =>
            {
                var effect = trans.FindChild("UI_TuJianHeCheng(Clone)");
                NGUITools.Destroy(effect);
            }));
        }, true, true, true, true, true);
    }

    private IEnumerator DestroyEffect(float delay , Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    private IEnumerator AnimationCoroutine(UISprite sp)
    {
        sp.type = UIBasicSprite.Type.Filled;
        sp.fillAmount = 0;
        sp.fillDirection = UIBasicSprite.FillDirection.Horizontal;
        float amount = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            amount += 0.06f;
            sp.fillAmount = amount;
            if (amount >= 1.0f)
            {
                sp.fillAmount = 1.0f;
                yield break;
            }
        }
    }
}
