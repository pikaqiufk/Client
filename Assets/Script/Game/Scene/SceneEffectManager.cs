#region using

using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;
using EffectNode = System.Collections.Generic.KeyValuePair<ulong, UnityEngine.GameObject>;

#endregion

public class SceneEffectManager : MonoBehaviour
{
    private static ulong mGerneratorId;
    private readonly Dictionary<int, EffectNode> mEffectDic = new Dictionary<int, EffectNode>();

    private ulong NextId
    {
        get { return mGerneratorId++; }
    }

    private IEnumerator DelayDestroy(float time, ulong id)
    {
        yield return new WaitForSeconds(time);
        var key = -1;
        {
            // foreach(var pair in mEffectDic)
            var __enumerator2 = (mEffectDic).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var pair = __enumerator2.Current;
                {
                    var node = pair.Value;
                    if (node.Key == id)
                    {
                        key = pair.Key;
                    }
                }
            }
        }
        if (-1 != key)
        {
            StopEffect(key);
        }
    }

    public void OnAcceptMission(int id)
    {
        var mission = MissionManager.Instance.GetMissionById(id);
        if (null == mission)
        {
            return;
        }

        var table = Table.GetMissionBase(mission.MissionId);
        if (null == table)
        {
            return;
        }

        var state = (eMissionState) mission.Exdata[0];

        if (-1 != table.GetPlay)
        {
            PlayEffect(table.GetPlay);
        }

        if (-1 != table.GetStop)
        {
            StopEffect(table.GetStop);
        }
    }

    public void OnCommitMission(int id)
    {
        var table = Table.GetMissionBase(id);
        if (null == table)
        {
            return;
        }

        if (-1 != table.PayPlay)
        {
            PlayEffect(table.PayPlay);
        }

        if (-1 != table.PayStop)
        {
            StopEffect(table.PayStop);
        }
    }

    public void OnEnterScecne()
    {
        {
            // foreach(var data in MissionManager.Instance.MissionData.Datas)
            var __enumerator1 = (MissionManager.Instance.MissionData.Datas).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var data = __enumerator1.Current;
                {
                    var mission = data.Value;
                    var table = Table.GetMissionBase(mission.MissionId);
                    if (null == table)
                    {
                        continue;
                    }

                    var state = (eMissionState) mission.Exdata[0];

                    if (-1 != table.GetPlay)
                    {
                        if (eMissionState.Unfinished == state)
                        {
                            PlayEffect(table.GetPlay);
                        }
                    }

                    if (-1 != table.FinishPlay)
                    {
                        if (eMissionState.Finished == state)
                        {
                            PlayEffect(table.FinishPlay);
                        }
                    }
                }
            }
        }
    }

    public void OnMissionComplete(int id)
    {
        var mission = MissionManager.Instance.GetMissionById(id);
        if (null == mission)
        {
            return;
        }

        var table = Table.GetMissionBase(mission.MissionId);
        if (null == table)
        {
            return;
        }

        var state = (eMissionState) mission.Exdata[0];
        if (-1 != table.FinishPlay)
        {
            PlayEffect(table.FinishPlay);
        }

        if (-1 != table.FinishStop)
        {
            StopEffect(table.FinishStop);
        }
    }

    public void PlayEffect(int id)
    {
        var table = Table.GetSceneEffect(id);
        if (null == table)
        {
            return;
        }
        if (table.SceneID != GameLogic.Instance.Scene.SceneTypeId)
        {
            return;
        }

        if (mEffectDic.ContainsKey(id))
        {
            return;
        }
        var root = GameLogic.Instance.Scene.EffectRoot;
        ComplexObjectPool.NewObject(table.Path, go =>
        {
            if (null == root)
            {
                ComplexObjectPool.Release(go);
                return;
            }

            if (mEffectDic.ContainsKey(id))
            {
                ComplexObjectPool.Release(go);
                return;
            }
#if UNITY_EDITOR
            go.name = "SceneEffect_" + id.ToString();
#endif
            var goTransform = go.transform;
            goTransform.parent = root.transform;
            goTransform.position = new Vector3(table.PosX, table.PosY, table.PosZ);
            goTransform.localScale = Vector3.one*(table.Zoom*0.0001f);
            var guid = NextId;
            mEffectDic.Add(id, new EffectNode(guid, go));
            if (table.LastTime > 0)
            {
                StartCoroutine(DelayDestroy(table.LastTime*1.0f/1000.0f, guid));
            }
        });
    }

    public void StopEffect(int id)
    {
        EffectNode node;
        if (!mEffectDic.TryGetValue(id, out node))
        {
            return;
        }
        ComplexObjectPool.Release(node.Value);
        mEffectDic.Remove(id);
    }
}