#region using

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class AllianceBattleEffectLogic : MonoBehaviour
{
    private readonly Dictionary<string, GameObject> _colorObjs = new Dictionary<string, GameObject>();

    private readonly Dictionary<string, string> _resPaths = new Dictionary<string, string>
    {
        {"Red", "Effect/TongYongEffect/Npc_FangYuTa_ZhanLing_Hong.prefab"},
        {"Yellow", "Effect/TongYongEffect/Npc_FangYuTa_ZhanLing_Huang.prefab"},
        {"Blue", "Effect/TongYongEffect/Npc_FangYuTa_ZhanLing_Lan.prefab"},
        {"Green", "Effect/TongYongEffect/Npc_FangYuTa_ZhanLing_Lv.prefab"}
    };

    private string ResPath = String.Empty;

    public IEnumerator EffectRemoveCoroutine(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        if (obj != null && obj.activeInHierarchy)
        {
            if (obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }

    private void InitRes(string color)
    {
        if (_resPaths.ContainsKey(color))
        {
            if (!_colorObjs.ContainsKey(color))
            {
                _colorObjs[color] = ComplexObjectPool.NewObjectSync(_resPaths[color]);
                if (_colorObjs[color] == null)
                {
                    return;
                }
            }

            var trans = _colorObjs[color].transform;
            trans.parent = gameObject.transform;
            trans.localPosition = Vector3.zero;
            trans.localPosition += new Vector3(0, -0.58f, 0); // 特效位置修正
            trans.localScale = Vector3.one;
            _colorObjs[color].SetActive(true);
            StartCoroutine(EffectRemoveCoroutine(2f, _colorObjs[color]));
        }
    }

    public void Play(string color)
    {
        InitRes(color);
    }
}