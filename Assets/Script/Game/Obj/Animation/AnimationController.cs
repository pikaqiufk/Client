#region using

using System;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

#endregion

public interface IAnimationListener
{
    void OnAnimationBegin(string animationName);
    void OnAnimationEnd(string animationName);
    void OnAnimationInterrupt(string animationName);
}

public class StringBoolStruct
{
    public StringBoolStruct(string str, bool flag)
    {
        Str = str;
        Flag = flag;
    }

    public bool Flag;
    public string Str = string.Empty;
}

public class AnimationController : MonoBehaviour
{
    //动画文件扩展名
    public const string AniFileExt = ".anim";

    public AnimationController()
    {
        Animation = null;
    }

    //动画组件(挂在Model上的)
    private HashSet<AnimationClip> mAnimationClips = new HashSet<AnimationClip>();
    //缓存Transform
    private Transform mCachedTransform;
    //当前角色
    public ObjCharacter mCharacter = null;
    //回掉时间
    private float mCurrentAniEndTime;
    private int mCurrentAnimationId = TypeDefine.INVALID_ID;
    //当前正在播放的动画的名字
    private string mCurrentAnimationName = string.Empty;
    //每个动作的缓存 <动作名字, <动资源目录,是否为脏>>  当AnimationContoller重新初始化后所有动作都设置为脏，下次播放时在算一次目录
    private readonly Dictionary<string, StringBoolStruct> mDictAniCache = new Dictionary<string, StringBoolStruct>();
    //动作特效
    private readonly List<ulong> mEffectList = new List<ulong>();
    //角色所用动画资源目录
    private string mFirstAniResDirectory = string.Empty;
    //第一优先级加载
    private bool mFirstPriority;
    //动画时间监听者
    private IAnimationListener mListener;
    //动画播放结束是否需要回掉
    private bool mNeedProcessEndEvent;
    //下个动作id
    private int mNextAnimationId = TypeDefine.INVALID_ID;
    //动作回掉
    private Action<string> mPlayEndCallBack;
    private string mSecondAniResDirectory = string.Empty;
    private uint mSoundEffectTag;
    private string mSubDir = string.Empty;
    //同步加载
    private bool mSyncLoadResource;
    //动作表格数据
    private AnimationRecord mTableData;
    public Animation Animation { get; set; }

    private void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        mCachedTransform = transform;

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private string CalculateResPath(string aniName)
    {
        if (string.IsNullOrEmpty(mFirstAniResDirectory) && string.IsNullOrEmpty(mSecondAniResDirectory))
        {
            Logger.Info("empty = mFirstAniResDirectory && empty = mFirstAniResDirectory");
            return string.Empty;
        }

        var aniFile = string.Empty;
        if (!string.IsNullOrEmpty(mSubDir) && ResourceManager.Exist(mSubDir + aniName + AniFileExt))
        {
            aniFile = mSubDir + aniName + AniFileExt;
        }
        else if (!string.IsNullOrEmpty(mFirstAniResDirectory) &&
                 ResourceManager.Exist(mFirstAniResDirectory + aniName + AniFileExt))
        {
            aniFile = mFirstAniResDirectory + aniName + AniFileExt;
        }
        else
        {
            aniFile = mSecondAniResDirectory + aniName + AniFileExt;
        }
        return aniFile;
    }

    public string GetPlayingName()
    {
        return mCurrentAnimationName;
    }

    //初始化
    public bool Init(string fisrtAnimResDir,
                     string subDir,
                     string secondAnimResDir,
                     bool resetCurrentAnimation = false,
                     bool sync = false,
                     bool firstPriority = false)
    {
        mSyncLoadResource = sync;
        mFirstPriority = firstPriority;
        {
            // foreach(var pair in mDictAniCache)
            var __enumerator5 = (mDictAniCache).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var pair = __enumerator5.Current;
                {
                    mDictAniCache[pair.Key].Flag = true;
                }
            }
        }

        InitPath(fisrtAnimResDir, subDir, secondAnimResDir);

        if (resetCurrentAnimation)
        {
            mCurrentAnimationName = string.Empty;
        }

        if (!string.IsNullOrEmpty(mCurrentAnimationName))
        {
            Play(mCurrentAnimationId);
        }

        return true;
    }

    public void InitPath(string fisrtAnimResDir, string subDir, string secondAnimResDir)
    {
        mFirstAniResDirectory = fisrtAnimResDir;
        mSubDir = string.Empty;
        if (!string.IsNullOrEmpty(mFirstAniResDirectory) && !mFirstAniResDirectory.EndsWith("/"))
        {
            mFirstAniResDirectory += '/';

            if (!string.IsNullOrEmpty(subDir))
            {
                mSubDir = mFirstAniResDirectory + subDir;
                if (!mSubDir.EndsWith("/"))
                {
                    mSubDir += '/';
                }
            }
        }

        mSecondAniResDirectory = secondAnimResDir;
        if (!string.IsNullOrEmpty(mSecondAniResDirectory) && !mSecondAniResDirectory.EndsWith("/"))
        {
            mSecondAniResDirectory += '/';
        }
    }

    private void OnAnimationBegin(string animationName)
    {
        if (null != mListener)
        {
            mListener.OnAnimationBegin(animationName);
        }
        PlayAnimationEffect();
        PlayAnimationSound();
    }

    private void OnAnimationEnd(string animationName)
    {
        if (null != mListener)
        {
            mListener.OnAnimationEnd(animationName);
        }
        if (null != mPlayEndCallBack)
        {
            mPlayEndCallBack(animationName);
        }
        if (mSoundEffectTag != 0)
        {
            SoundManager.Instance.StopSoundEffectByTag(mSoundEffectTag);
            mSoundEffectTag = 0;
        }
    }

    private void OnAnimationInterrupt(string animationName)
    {
        if (null != mListener)
        {
            mListener.OnAnimationInterrupt(animationName);
        }
        mPlayEndCallBack = null;
    }

    //播放动画
    public bool Play(int animationId, Action<string> callback = null)
    {
        //读表
        var tableData = Table.GetAnimation(animationId);
        if (null == tableData)
        {
            Logger.Error("TableError:DataTable.Table.GetAnimation({0})", animationId);
            OnAnimationEnd("no animation");
            return false;
        }

        // 数据都是对的了，现在模型还没有加载完成，播不了动画，就不播了
        if (!Animation)
        {
            return false;
        }

        //动作名字
        var aniName = tableData.AinmName;

        //动作资源目录，到后面就判断这个是否为空，是空就不需要加载新的，否则需要加载
        var animFile = string.Empty;

        if (null == Animation[aniName] || null == Animation.GetClip(aniName))
        {
            //如果没有加载过这个动作
            animFile = CalculateResPath(aniName);
        }
        else
        {
            StringBoolStruct sb;
            if (mDictAniCache.TryGetValue(aniName, out sb))
            {
                if (sb.Flag)
                {
                    animFile = CalculateResPath(aniName);
                    if (0 == animFile.CompareTo(sb.Str))
                    {
                        animFile = string.Empty;
                        sb.Flag = false;
                    }
                }
            }
            else
            {
                animFile = CalculateResPath(aniName);
            }
        }

        //判断当前是否正在播放这个动作
        if (string.IsNullOrEmpty(animFile) && Animation.IsPlaying(aniName) && mCurrentAnimationName == aniName)
        {
            if ((WrapMode) tableData.WrapMode == WrapMode.Loop)
            {
                return false;
            }
        }

        OnAnimationInterrupt(mCurrentAnimationName);

        mTableData = tableData;
        mCurrentAnimationName = aniName;
        mCurrentAnimationId = animationId;
        mPlayEndCallBack = callback;
        //设置下一个动作id
        mNextAnimationId = tableData.NextAnimId;

        OnAnimationBegin(mCurrentAnimationName);

        //不需要加载新的
        if (string.IsNullOrEmpty(animFile))
        {
            var state = Animation[mCurrentAnimationName];
            if (null != state)
            {
                PlayAnimation(tableData);
                return true;
            }
        }

        var loadingAnimationName = mCurrentAnimationName;
        //加载新动作
        ResourceManager.PrepareResource<AnimationClip>(animFile, clip =>
        {
            if (null == clip)
            {
                Logger.Fatal("Load animation[{0}] failded!", animFile);
                return;
            }

            // animation has changed already
            if (mCurrentAnimationId != animationId)
            {
                // 如果当前的动作类型还没有换，虽然动作本身换了，加载好的动作还是要加进去
                StringBoolStruct sb;
                if (mDictAniCache.TryGetValue(loadingAnimationName, out sb))
                {
                    if (sb.Flag && sb.Str == animFile)
                    {
                        Animation.AddClip(clip, aniName);
                        sb.Flag = false;
                    }
                }
                return;
            }

            if (Animation == null)
            {
                return;
            }

            mDictAniCache[mCurrentAnimationName] = new StringBoolStruct(animFile, false);

            Animation.AddClip(clip, aniName);
            Animation.clip = clip;

            var state = Animation[aniName];
            state.speed = (float) tableData.SPEED;

            if (tableData.WrapMode == 0)
            {
                state.wrapMode = WrapMode.Default; //默认是once
            }
            else if (tableData.WrapMode == 1)
            {
                //state.wrapMode = WrapMode.Once;//动画播放完就停止播放状态
                state.wrapMode = WrapMode.ClampForever; //动画播完最后一帧就一直播放最后一帧
            }
            else if (tableData.WrapMode == 2)
            {
                state.wrapMode = WrapMode.Loop; //循环模式，动画播完就从头播放
            }
            else if (tableData.WrapMode == 3)
            {
                state.wrapMode = WrapMode.PingPong; //循环模式，动画播完就从尾巴往回播放
            }
            else if (tableData.WrapMode == 4)
            {
                state.wrapMode = WrapMode.ClampForever; //动画播完最后一帧就一直播放最后一帧
            }

            OptList<Renderer>.List.Clear();
            gameObject.GetComponentsInChildren(OptList<Renderer>.List);
            {
                var __array2 = OptList<Renderer>.List;
                var __arrayLength2 = __array2.Count;
                for (var __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var renderer = __array2[__i2];
                    {
                        renderer.enabled = true;
                    }
                }
            }

            PlayAnimation(tableData);
        }, true, true, mSyncLoadResource, mFirstPriority, true);

        return true;
    }

    private void PlayAnimation(AnimationRecord record)
    {
        var name = record.AinmName;
        var transitTime = (float) record.TransitTime;

        //记录下当前播放动画的名字
        if (Animation.isPlaying)
        {
            if (Animation.IsPlaying(name))
            {
                //直接播放
                Animation.Stop(name);
                Animation.Play(name);
            }
            else
            {
                //淡出播放(有transitTime控制)
                Animation.CrossFade(name, transitTime, PlayMode.StopAll);
            }
        }
        else
        {
            //直接播放
            Animation.Play(name);
        }

        //是否需要有动画结束通知
        var processEnd = 2 != record.WrapMode || record.LoopTime > 0;
        if (processEnd)
        {
            mNeedProcessEndEvent = true;
        }
        else
        {
            mNeedProcessEndEvent = false;
        }


        if (mNeedProcessEndEvent)
        {
            //计算动画总时长
            var state = Animation[name];
            var totalLength = state.length/state.speed;
            if (WrapMode.Loop == state.wrapMode)
            {
                if (record.LoopTime > 0)
                {
                    totalLength = totalLength*record.LoopTime;
                }
            }

            mCurrentAniEndTime = Time.time + Math.Abs(totalLength);
        }
    }

    private void PlayAnimationEffect()
    {
        StopAnimationEffect();

        if (gameObject == null)
        {
            return;
        }
        if (null != mTableData)
        {
            var mTableDataStartEffectLength0 = mTableData.StartEffect.Length;
            for (var i = 0; i < mTableDataStartEffectLength0; i++)
            {
                var id = mTableData.StartEffect[i];
                if (TypeDefine.INVALID_ID == id)
                {
                    continue;
                }
                var tableData = Table.GetEffect(id);
                EffectManager.Instance.CreateEffect(tableData, mCharacter, null,
                    (effect, effectId) => { mEffectList.Add(effectId); }, null,
                    (tableData.BroadcastType == 0 && mCharacter.GetObjType() == OBJ.TYPE.MYPLAYER) ||
                    tableData.BroadcastType == 1);
            }
        }
    }

    private void PlayAnimationSound()
    {
        try
        {
            if (mCachedTransform == null || ObjManager.Instance.MyPlayer == null)
            {
                return;
            }
            mSoundEffectTag = SoundManager.NextTag;
            SoundManager.Instance.PlaySoundEffectAtPos(mTableData.SoundID, mCachedTransform.position,
                ObjManager.Instance.MyPlayer.Position, mSoundEffectTag);
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }

    [ContextMenu("Play")]
    public void PlayStand()
    {
        Play(OBJ.CHARACTER_ANI.STAND);
    }

    public void PrepareAnimationClip(string anim)
    {
    }

    public void PrepareAnimationClip(int animId)
    {
        var tableData = Table.GetAnimation(animId);
        if (null == tableData)
        {
            Logger.Error("TableError:DataTable.Table.GetAnimation({0})", animId);
            return;
        }

        //动作名字
        var aniName = tableData.AinmName;
        var aniFile = CalculateResPath(aniName);
        ResourceManager.PrepareResource<AnimationClip>(aniFile, clip =>
        {
            //do nothing, resource is cache already.
        });
    }

    public void PrePareAnimationResources(int skillid)
    {
        var animationId = Table.GetSkill(skillid).ActionId;
        var tbAnim = Table.GetAnimation(animationId);
        if (tbAnim == null)
        {
            Logger.Error("Animation id {0} can not found", animationId);
            return;
        }

        var aniName = tbAnim.AinmName;
        var aniFile = string.Empty;
        var aniState = Animation[aniName];
        if (null == aniState)
        {
            if (!string.IsNullOrEmpty(mSubDir) && ResourceManager.Exist(mSubDir + aniName + AniFileExt))
            {
                aniFile = mSubDir + aniName + AniFileExt;
            }
            else if (!string.IsNullOrEmpty(mFirstAniResDirectory) &&
                     ResourceManager.Exist(mFirstAniResDirectory + aniName + AniFileExt))
            {
                aniFile = mFirstAniResDirectory + aniName + AniFileExt;
            }
            else
            {
                aniFile = mSecondAniResDirectory + aniName + AniFileExt;
            }

            ResourceManager.PrepareResource<AnimationClip>(aniFile, clip =>
            {
                if (Animation == null)
                {
                    return;
                }
                Animation.AddClip(clip, aniName);
                Animation.clip = clip;
                //mAnimationClips.Add(clip);
            });
        }

        PrepareEffectResources(skillid);
    }

    public void PrepareEffectResources(int skillid)
    {
        if (mCharacter == null || !EffectManager.Instance.ShouldShowEffect(mCharacter.GetObjId()))
        {
            return;
        }

        var skillData = Table.GetSkill(skillid);
        if (TypeDefine.INVALID_ID != skillData.Effect)
        {
            var tbEffect = Table.GetEffect(skillData.Effect);
            EffectManager.Instance.CreateEffect(tbEffect, null, Vector3.forward, mCharacter.GetLayerForEffect(), null,
                (effect, effectId) => { effect.Duration = 0; }, null, false,
                true);
        }
        //--------------------------------------------------------------------------------------------------------------------
        var tbAnim = Table.GetAnimation(skillData.ActionId);
        if (tbAnim == null)
        {
            Logger.Error("Animation id {0} can not found", skillData.ActionId);
            return;
        }

        var count = tbAnim.StartEffect.Length;
        for (var i = 0; i < count; i++)
        {
            var id = tbAnim.StartEffect[i];
            if (id == TypeDefine.INVALID_ID)
            {
                continue;
            }
            var tbEffect = Table.GetEffect(id);
            EffectManager.Instance.CreateEffect(tbEffect, null, Vector3.forward, mCharacter.GetLayerForEffect(), null,
                (effect, effectId) => { effect.Duration = 0; }, null, false, true);
        }
    }

    //设置监听者
    public void SetAnimationListener(IAnimationListener listener)
    {
        mListener = listener;
    }

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

    public bool Stop(bool stopEffect, bool immediateStop = false)
    {
        if (null == Animation)
        {
            return false;
        }

        Animation.Stop();
        if (stopEffect)
        {
            StopAnimationEffect(immediateStop);
        }
        mCurrentAnimationName = "";
        return true;
    }

    public void StopAnimationEffect(bool immediateStop = false)
    {
        if (immediateStop)
        {
            {
                var __list3 = mEffectList;
                var __listCount3 = __list3.Count;
                for (var __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var id = __list3[__i3];
                    {
                        EffectManager.Instance.StopEffect(id);
                    }
                }
            }
            mEffectList.Clear();
        }
        else
        {
            {
                var __list4 = mEffectList;
                var __listCount4 = __list4.Count;
                for (var __i4 = 0; __i4 < __listCount4; ++__i4)
                {
                    var id = __list4[__i4];
                    {
                        EffectManager.Instance.StopLoop(id);
                    }
                }
            }
            mEffectList.Clear();
        }
    }

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        if (mNeedProcessEndEvent)
        {
            if (Time.time >= mCurrentAniEndTime)
            {
                //mAnimation.Stop(mCurrentAnimationName);
                mNeedProcessEndEvent = false;
                if (TypeDefine.INVALID_ID != mNextAnimationId)
                {
                    Play(mNextAnimationId);
                }
                else
                {
                    OnAnimationEnd(mCurrentAnimationName);
                    StopAnimationEffect();
                }
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