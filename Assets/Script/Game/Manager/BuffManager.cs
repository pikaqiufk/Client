#region using

using System;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

#endregion

public enum BuffState
{
    Added,
    LoadingEffect,
    LoadingCompleted,
    Deleted
}

public class Buff
{
    public uint BuffId;
    public int BuffTypeId;
    public ulong CasterId;
    public List<ulong> EffectId = new List<ulong>();
    public List<EffectRef> EffectRef = new List<EffectRef>();
    public BuffRecord Record;
    public uint SoundId;
    public BuffState State;
    public ulong TargetId;
}

public class EffectRef
{
    public int RefCount;
    public int TypeId;
    public ulong Uuid;
}

public class BuffRef
{
    public uint BuffId;
    public int EffectTypeId;
}

public class BuffManager
{
    //同时加载最大数
    public const int MAXBUFF_LOADING = 10;

    public BuffManager(ObjCharacter character)
    {
        mCharacter = character;
    }

    //加载队列
    private readonly List<BuffRef> LoadQueue = new List<BuffRef>();
    //buff列表
    private readonly List<Buff> mBuffList = new List<Buff>();
    //buff拥有者
    private readonly ObjCharacter mCharacter;
    //buff的特效引用计数
    private readonly Dictionary<int, EffectRef> mEffectRefCount = new Dictionary<int, EffectRef>();
    //加buff
    public Buff AddBuff(uint buffId, int buffTypeId, ulong caster, ulong target, bool showFireEffect = true)
    {
        var table = Table.GetBuff(buffTypeId);
        if (null == table)
        {
            Logger.Fatal("Table.GetBuff({0})", buffTypeId);
            return null;
        }

        var buff = new Buff
        {
            BuffId = buffId,
            BuffTypeId = buffTypeId,
            CasterId = caster,
            TargetId = target,
            State = BuffState.Added,
            Record = table
        };

        mBuffList.Add(buff);

        var __array1 = table.Effect;
        var __arrayLength1 = __array1.Length;
        for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
        {
            var effect = __array1[__i1];
            {
                if (-1 != effect)
                {
                    if (!showFireEffect && !Mathf.Approximately(Table.GetEffect(effect).Duration, 0))
                    {
                        continue;
                    }

                    var buffref = new BuffRef();
                    buffref.BuffId = buff.BuffId;
                    buffref.EffectTypeId = effect;
                    LoadQueue.Add(buffref);
                    buff.State = BuffState.LoadingEffect;
                }
            }
        }

        if (showFireEffect)
        {
            if (-1 != table.Sound)
            {
                var sound = Table.GetSound(table.Sound);
                if (sound.IsLoop != 0)
                {
                    buff.SoundId = SoundManager.NextTag;
                    SoundManager.Instance.PlaySoundEffect(table.Sound, 1.0f, buff.SoundId);
                }
                else
                {
                    SoundManager.Instance.PlaySoundEffect(table.Sound);
                }
            }
        }

        if (LoadQueue.Count > MAXBUFF_LOADING)
        {
            LoadQueue.RemoveRange(0, LoadQueue.Count - MAXBUFF_LOADING);
        }

        return buff;
    }

    //获得buff
    public Buff GetBuff(uint buffId)
    {
        {
            var __list4 = mBuffList;
            var __listCount4 = __list4.Count;
            for (var __i4 = 0; __i4 < __listCount4; ++__i4)
            {
                var buff = __list4[__i4];
                {
                    if (buff.BuffId == buffId)
                    {
                        return buff;
                    }
                }
            }
        }
        return null;
    }

    //获得buff数据
    public List<Buff> GetBuffData()
    {
        return mBuffList;
    }

    public bool HasBuff(int type)
    {
        var count = mBuffList.Count;
        for (var i = 0; i < count; i++)
        {
            var buff = mBuffList[i];
            if (buff.BuffTypeId == type)
            {
                return true;
            }
        }
        return false;
    }

    //删除全部buff
    public void RemoveAllBuff()
    {
        var i = 0;
        while (mBuffList.Count > 0)
        {
            RemoveBuff(mBuffList[0]);
            i++;
            if (i >= 500)
            {
                break;
            }
        }
        mBuffList.Clear();
    }

    //移除buff
    public bool RemoveBuff(uint buffId)
    {
        var buff = GetBuff(buffId);
        return RemoveBuff(buff);
    }

    private bool RemoveBuff(Buff buff)
    {
        if (null != buff)
        {
            buff.State = BuffState.Deleted;

            {
                var __list2 = buff.EffectRef;
                var __listCount2 = __list2.Count;
                for (var __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var effectRef = __list2[__i2];
                    {
                        effectRef.RefCount--;
                        if (effectRef.RefCount == 0)
                        {
                            mEffectRefCount.Remove(effectRef.TypeId);
                            EffectManager.Instance.StopLoop(effectRef.Uuid);
                        }
                    }
                }
            }

            {
                var __list2 = buff.EffectId;
                var __listCount2 = __list2.Count;
                for (var __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var effectId = __list2[__i2];
                    {
                        EffectManager.Instance.StopLoop(effectId);
                    }
                }
            }

            if (buff.SoundId > 0)
            {
                SoundManager.Instance.StopSoundEffectByTag(buff.SoundId);
            }

            mBuffList.Remove(buff);
            return true;
        }

        return false;
    }

    public void RemoveBuffWhenDie()
    {
        var count = mBuffList.Count;
        for (var i = count - 1; i >= 0; i--)
        {
            var buff = mBuffList[i];
            if (buff.Record.DieDisappear != 0)
            {
                RemoveBuff(buff);
            }
        }
    }

    //重置
    public void Reset()
    {
        RemoveAllBuff();
        LoadQueue.Clear();
    }

    //update
    public void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (null == mCharacter)
        {
            return;
        }
        {
            var __list3 = LoadQueue;
            var __listCount3 = __list3.Count;
            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
            {
                var buffRef = __list3[__i3];
                {
                    var buffId = buffRef.BuffId;
                    var buff = GetBuff(buffId);
                    if (null == buff)
                    {
                        continue;
                    }

                    if (buff.State == BuffState.Deleted)
                    {
                        continue;
                    }

                    var tableEffct = Table.GetEffect(buffRef.EffectTypeId);
                    var playerId = ObjManager.Instance.MyPlayer.GetObjId();
                    if (tableEffct.MaxOwnNum != -1)
                    {
                        EffectRef effectRef;
                        if (mEffectRefCount.TryGetValue(tableEffct.Id, out effectRef))
                        {
                            var tempBuff = GetBuff(buffId);
                            tempBuff.EffectRef.Add(effectRef);
                            effectRef.RefCount++;
                        }
                        else
                        {
                            effectRef = new EffectRef();
                            effectRef.TypeId = tableEffct.Id;
                            effectRef.RefCount++;

                            EffectManager.Instance.CreateEffect(tableEffct, mCharacter, null, (e, id) =>
                            {
                                if (Math.Abs(tableEffct.Duration) < 0.001f)
                                {
                                    var tempBuff = GetBuff(buffId);
                                    if (null == tempBuff)
                                    {
                                        EffectManager.Instance.StopLoop(id);
                                    }
                                    else
                                    {
                                        effectRef.Uuid = id;
                                        tempBuff.EffectRef.Add(effectRef);
                                        mEffectRefCount[effectRef.TypeId] = effectRef;
                                        tempBuff.State = BuffState.LoadingCompleted;
                                    }
                                }
                            }, null,
                                (tableEffct.BroadcastType == 0 && buff.CasterId == playerId || buff.TargetId == playerId) ||
                                tableEffct.BroadcastType == 1);
                        }
                    }
                    else
                    {
                        EffectManager.Instance.CreateEffect(tableEffct, mCharacter, null, (e, id) =>
                        {
                            if (Math.Abs(tableEffct.Duration) < 0.001f)
                            {
                                var tempBuff = GetBuff(buffId);
                                if (null == tempBuff)
                                {
                                    EffectManager.Instance.StopLoop(id);
                                }
                                else
                                {
                                    tempBuff.EffectId.Add(id);
                                    tempBuff.State = BuffState.LoadingCompleted;
                                }
                            }
                        }, null,
                            (tableEffct.BroadcastType == 0 && buff.CasterId == playerId || buff.TargetId == playerId) ||
                            tableEffct.BroadcastType == 1);
                    }
                }
            }
        }
        LoadQueue.Clear();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}