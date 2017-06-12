#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DataTable;
using PigeonCoopToolkit.Effects.Trails;
using Thinksquirrel.Utilities;
using UnityEngine;

#endregion

public class EffectManager : Singleton<EffectManager>, IManager
{
    private static ulong idx;
    private readonly Dictionary<ulong, EffectController> mDict = new Dictionary<ulong, EffectController>();
    private readonly List<ulong> mRemoveList = new List<ulong>();

    public void CreateEffect(EffectRecord tableData,
                             Transform parent,
                             Vector3 dir,
                             int layer = 0,
                             Vector3? pos = null,
                             Action<EffectController, ulong> loadOverCallback = null,
                             Action<EffectController, ulong> playOverCallback = null,
                             bool firstPriority = false,
                             bool bPrepareResource = false,
                             bool camShake = true)
    {
        ComplexObjectPool.NewObject(tableData.Path, obj =>
        {
            if (obj == null)
            {
                return;
            }

            var Id = GenerateNextId();

            var p = new Vector3(tableData.X, tableData.Y, tableData.Z);
            var rotation =
                Quaternion.Euler(new Vector3(tableData.RotationX, tableData.RotationY, tableData.RotationZ));

            var objTransform = obj.transform;
            if (null != parent)
            {
                objTransform.parent = parent;
            }
            else
            {
                objTransform.parent = GameLogic.Instance.Scene.EffectRoot.transform;
            }

            if (tableData.IsZoom != 0)
            {
                objTransform.localScale = Vector3.one;
            }
            else
            {
                if (parent != null)
                {
                    var v = parent.transform.lossyScale;
                    objTransform.localScale = new Vector3(1.0f/v.x, 1.0f/v.y, 1.0f/v.z);
                }
                else
                {
                    objTransform.localScale = Vector3.one;
                }
            }

            objTransform.localPosition = p;
            objTransform.localRotation = rotation;

            obj.name = "Effect_" + Id.ToString() + "_" + Path.GetFileNameWithoutExtension(tableData.Path);

            obj.SetLayerRecursive(layer);

            ComplexObjectPool.SetActive(obj, false);

            {
                //根据setting设置特效显示
                if (GameSetting.Instance.GameQualityLevel < 3)
                {
                    {
                        // foreach(var data in EnumGameObjectRecursive(obj))
                        var __enumerator5 = (EnumGameObjectRecursive(obj)).GetEnumerator();
                        while (__enumerator5.MoveNext())
                        {
                            var data = __enumerator5.Current;
                            {
                                if (data.CompareTag("LowEffectParticle"))
                                {
                                    data.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }

            var effect = obj.GetComponent<EffectController>();
            if (effect == null)
            {
                effect = obj.AddComponent<EffectController>();
                effect.InitLoop();
            }
            effect.Id = Id;
            effect.Delay = tableData.DelayTime;
            effect.Duration = tableData.Duration == 0 ? float.MaxValue : tableData.Duration;
            effect.Follow = tableData.Follow != 0;
            effect.PlayOverCallback = playOverCallback;
            effect.LoopTime = tableData.LoopTime;
            effect.State = EffectState.WaitToFire;
            effect.Dir = dir;
            effect.Pos = pos;
            effect.trans = effect.transform;
            mDict.Add(effect.Id, effect);

            if (null != loadOverCallback)
            {
                loadOverCallback(effect, Id);
            }

            if (camShake)
            {
                var tableDataShakeDelayTimeLength0 = tableData.ShakeDelayTime.Length;
                for (var i = 0; i < tableDataShakeDelayTimeLength0; i++)
                {
                    if (tableData.ShakeDelayTime[i] != -1)
                    {
                        //创建人物时播放角色动画还没有GameLogic
                        if (null != GameLogic.Instance)
                        {
                            var shake = GameLogic.Instance.MainCamera.GetComponent<CameraShake>();
							if (null!=shake && shake.gameObject.active && (!bPrepareResource) && GameLogic.Instance.MainCamera.enabled)
                            {
                                shake.StartCoroutine(ShakeCamera(shake, tableData.ShakeDelayTime[i]/1000.0f,
                                    tableData.ShakeMagnitude[i], (int) tableData.ShakeCount[i], tableData.ShakeSpeed[i],
                                    tableData.ShakeReduction[i], tableData.ShakeType[i]));
                            }
                        }
                    }
                }
            }
        }, null, null, false, firstPriority, false);
    }

    public void CreateEffect(EffectRecord tableData,
                             ObjCharacter character,
                             Vector3? pos = null,
                             Action<EffectController, ulong> loadOverCallback = null,
                             Action<EffectController, ulong> playOverCallback = null,
                             bool shake = true,
                             bool firstPriority = false)
    {
        if (!ShouldShowEffect(character.GetObjId()))
        {
            return;
        }
        var mount = character.GetMountPoint(tableData.MountPoint);
        if (mount == null && character.GetModel() != null && character.State != ObjState.LoadingResource)
        {
            Logger.Warn("因为模型Id={2} 挂载点{0}找不到了，所以特效{1}没有播放。", tableData.MountPoint, tableData.Id, character.ModelId);
            return;
        }
        CreateEffect(tableData, mount, character.TargetDirection, character.GetLayerForEffect(), pos, loadOverCallback, playOverCallback, firstPriority,
            false, shake);
    }

    public static IEnumerable<GameObject> EnumGameObjectRecursive(GameObject go)
    {
        yield return go;
        var goTransform = go.transform;
        for (var i = 0; i < goTransform.childCount; i++)
        {
            {
                // foreach(var t in EnumGameObjectRecursive(goTransform.GetChild(i).gameObject))
                var __enumerator6 = (EnumGameObjectRecursive(goTransform.GetChild(i).gameObject)).GetEnumerator();
                while (__enumerator6.MoveNext())
                {
                    var t = __enumerator6.Current;
                    {
                        yield return t;
                    }
                }
            }
        }
    }

    private ulong GenerateNextId()
    {
        return idx++;
    }

    public bool HasEffect(ulong id)
    {
        return mDict.ContainsKey(id);
    }

    public void RemoveEffect(ulong id)
    {
        mDict.Remove(id);
    }

    /// <summary>
    ///     摄像机延迟震动
    /// </summary>
    /// <param name="shake"></param>
    /// <param name="delay"></param>
    /// <param name="magnitude"></param>
    /// <param name="count"></param>
    /// <param name="speed"></param>
    /// <param name="decay"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator ShakeCamera(CameraShake shake,
                                   float delay,
                                   float magnitude,
                                   int count,
                                   float speed,
                                   float decay,
                                   int type)
    {
        if (!GameSetting.Instance.CameraShakeEnable)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);

        var amount = Vector3.one;
        if (type == 1)
        {
            amount = Vector3.up;
        }
        else if (type == 2)
        {
            amount = Vector3.right;
        }

        shake.Shake(CameraShake.ShakeType.LocalPosition, count, amount, Vector3.zero, magnitude, speed, decay, 1.0f,
            true, null);
    }

    public bool ShouldShowEffect(ulong id)
    {
        return GameSetting.Instance.ShowEffect ||
               id < 10000000 || (ObjManager.Instance.MyPlayer != null && id == ObjManager.Instance.MyPlayer.GetObjId());
    }

    public void StopAllEffect()
    {
        {
            // foreach(var pair in mDict)
            var __enumerator4 = (mDict).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var pair = __enumerator4.Current;
                {
                    if (pair.Value == null)
                    {
                        continue;
                    }
                    ComplexObjectPool.Release(pair.Value.gameObject);
                }
            }
        }
        mDict.Clear();
    }

    public bool StopEffect(ulong id)
    {
        EffectController effect = null;
        if (mDict.TryGetValue(id, out effect))
        {
            OptList<ParticleSystem>.List.Clear();
            effect.gameObject.GetComponentsInChildren(OptList<ParticleSystem>.List);
            {
                var __array3 = OptList<ParticleSystem>.List;
                var __arrayLength3 = __array3.Count;
                for (var __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var particleSystem = __array3[__i3];
                    {
                        particleSystem.Clear();
                        particleSystem.Stop();
                    }
                }
            }

            OptList<TrailRenderer_Base>.List.Clear();
            effect.gameObject.GetComponentsInChildren(true, OptList<TrailRenderer_Base>.List);
            {
                var __array7 = OptList<TrailRenderer_Base>.List;
                var __arrayLength7 = __array7.Count;
                for (var __i7 = 0; __i7 < __arrayLength7; ++__i7)
                {
                    var t = __array7[__i7];
                    {
                        t.ClearSystem(false);
                    }
                }
            }

            ComplexObjectPool.Release(effect.gameObject);
            mDict.Remove(id);
            return true;
        }

        return false;
    }

    public void StopLoop(ulong id)
    {
        EffectController effect = null;
        if (mDict.TryGetValue(id, out effect))
        {
            effect.StopLoop();
        }
    }

    public IEnumerator Init()
    {
        yield return null;
    }

    public void Reset()
    {
        StopAllEffect();
    }

    public void Tick(float delta)
    {
        //Profiler.BeginSample("t1");
        {
            // foreach(var pair in mDict)
            var __enumerator1 = (mDict).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var pair = __enumerator1.Current;
                {
                    if (pair.Value == null)
                    {
                        mRemoveList.Add(pair.Key);
                        continue;
                    }
                    pair.Value.Tick();
                }
            }
        }
        //Profiler.EndSample();

        //Profiler.BeginSample("t2");
        {
            var __list2 = mRemoveList;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var item = __list2[__i2];
                {
                    mDict.Remove(item);
                }
            }
            if (__listCount2 > 0)
            {
                mRemoveList.Clear();
            }
        }


        //Profiler.EndSample();
    }

    public void Destroy()
    {
    }
}