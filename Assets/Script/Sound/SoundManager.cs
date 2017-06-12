#region using

using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

#endregion

[Serializable]
public class SoundClip
{
    private AudioClip m_Audioclip;
    //运行时数据
    public float m_LastActiveTime; //上次活跃时间,上次播放时间
    public int mCurMaxPlayingCount = 1;
    public float mDelay;
    public bool mIsLoop;
    public float mMinDistance = 10;
    public float mPanLevel;
    //public string mName = string.Empty;
    public string mPath = string.Empty;
    //表格数据
    public short mPriority = 128;
    public float mSpread;
    public int mUID = -1;
    public float mVolume = 1.0f;

    public AudioClip Audioclip
    {
        get { return m_Audioclip; }
        set { m_Audioclip = value; }
    }
}

public class SoundClipPools
{
    public Dictionary<int, SoundClip> m_SoundClipMap = new Dictionary<int, SoundClip>(); //音效列表，限制最大数量MAX
    //force Remove clip from pool, special for bgmusic
    public void ForceRemoveClipByID(int uid)
    {
        if (uid != -1)
        {
            var nNeeddelId = -1;
            {
                // foreach(var clip in m_SoundClipMap.Values)
                var __enumerator2 = (m_SoundClipMap.Values).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var clip = __enumerator2.Current;
                    {
                        if (clip.mUID == uid)
                        {
                            nNeeddelId = clip.mUID;
                            break;
                        }
                    }
                }
            }
            m_SoundClipMap.Remove(nNeeddelId);

#if UNITY_WP8
    //Tab_Sounds soundsTab = TableManager.GetSoundsByID(nNeeddelId, 0);
    //BundleManager.ReleaseUnreferencedBundle(BundleManager.GetBundleLoadUrl("", soundsTab.FullPathName + ".data"));
    //BundleManager.DoUnloadUnuseBundle();
#endif
        }
    }

    /// <summary>
    ///     根据声音名称得到SoundClip，不存在会自动添加
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns></returns>
    public void GetSoundClip(int nSoundId, GetSoundClipDelegate delFun, SoundClipParam param, bool reset)
    {
        if (nSoundId >= 0)
        {
            if (m_SoundClipMap.ContainsKey(nSoundId))
            {
                //更新上次播放时间
                m_SoundClipMap[nSoundId].m_LastActiveTime = Time.realtimeSinceStartup;
                if (null != delFun)
                {
                    delFun(m_SoundClipMap[nSoundId], param, reset);
                }
            }
            else
            {
                if (m_SoundClipMap.Count > SoundManager.mSFXChannelsCount) //超过最大值，删除最长时间未使用的
                {
                    RemoveLastUnUsedClip();
                }

                var soundsTab = Table.GetSound(nSoundId);


                if (soundsTab == null)
                {
                    Logger.Info("sound id " + nSoundId + " is null");
                    if (null != delFun)
                    {
                        delFun(null, param, reset);
                    }
                    return;
                }

                var fullsoundName = soundsTab.FullPathName;
                if (string.IsNullOrEmpty(fullsoundName))
                {
                    if (null != delFun)
                    {
                        delFun(null, param, reset);
                    }
                    return;
                }

                if (ResourceManager.Instance == null)
                {
                    if (null != delFun)
                    {
                        delFun(null, param, reset);
                    }
                    return;
                }

                ResourceManager.PrepareSoundResource(fullsoundName, OnLoadSound, soundsTab, delFun, param);
            }
        }
    }

    private void OnLoadSound(string soundPath,
                             AudioClip curAudioClip,
                             object param1,
                             object param2,
                             object param3 = null)
    {
        var clip = new SoundClip();
        clip.Audioclip = curAudioClip;
        var delFun = param2 as GetSoundClipDelegate;
        var soundClipParam = param3 as SoundClipParam;
        var soundsTab = param1 as SoundRecord;
        if (null == clip.Audioclip)
        {
            Logger.Info("sound clip " + soundPath + " is null");
            if (null != delFun)
            {
                delFun(null, soundClipParam, false);
            }
            return;
        }

        if (!clip.Audioclip.isReadyToPlay)
        {
            Logger.Info("Cann't decompress the sound resource " + soundPath);
            if (null != delFun)
            {
                delFun(null, soundClipParam, false);
            }
            return;
        }
        clip.m_LastActiveTime = Time.realtimeSinceStartup;
        clip.mDelay = soundsTab.Delay;
        clip.mMinDistance = soundsTab.MinDistance;
        clip.mPanLevel = soundsTab.PanLevel;
        clip.mSpread = soundsTab.Spread;
        clip.mVolume = soundsTab.Volume;
        clip.mIsLoop = soundsTab.IsLoop != 0;
        clip.mPath = soundPath;
        clip.mPriority = (short) soundsTab.Priority;
        //clip.mName = soundsTab.Name;
        clip.mUID = soundsTab.Id;
        clip.mCurMaxPlayingCount = soundsTab.CurMaxPlayingCount;

        if (!m_SoundClipMap.ContainsKey(soundsTab.Id))
        {
            m_SoundClipMap.Add(soundsTab.Id, clip);
        }


        if (null != delFun)
        {
            delFun(clip, soundClipParam, false);
        }
    }

    /// <summary>
    ///     删除最长时间未使用的条目
    /// </summary>
    private void RemoveLastUnUsedClip()
    {
        var fSmallestTime = 99999999.0f;
        var smallestId = -1;
        {
            // foreach(var clip in m_SoundClipMap.Values)
            var __enumerator1 = (m_SoundClipMap.Values).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var clip = __enumerator1.Current;
                {
                    if (fSmallestTime > clip.m_LastActiveTime)
                    {
                        smallestId = clip.mUID;
                        fSmallestTime = clip.m_LastActiveTime;
                    }
                }
            }
        }

        Logger.Info("RemoveLastUnUsedClip( " + smallestId + " )"); //以后注释掉

        m_SoundClipMap.Remove(smallestId);

#if UNITY_WP8
    //Tab_Sounds soundsTab = TableManager.GetSoundsByID(smallestId, 0);
    //if (null != soundsTab)
    //    BundleManager.ReleaseUnreferencedBundle(BundleManager.GetBundleLoadUrl("", soundsTab.FullPathName + ".data"));
    //BundleManager.DoUnloadUnuseBundle();
#endif
    }

    public delegate void GetSoundClipDelegate(SoundClip soundClip, SoundClipParam param, bool reset);

    public class SoundClipParam
    {
        public SoundClipParam(float volumeFactor)
        {
            m_volumeFactor = volumeFactor;
            m_fadeInTime = 0;
            m_fadeOutTime = 0;
        }

        public SoundClipParam(int clipId, float fadeOutTime, float fadeInTime)
        {
            m_volumeFactor = 1;
            m_fadeInTime = fadeInTime;
            m_fadeOutTime = fadeOutTime;
            m_clipID = clipId;
        }

        public SoundClipParam(float volumeFactor, uint tag)
        {
            m_volumeFactor = volumeFactor;
            m_fadeInTime = 0;
            m_fadeOutTime = 0;
            m_tag = tag;
        }

        public int m_clipID;
        public float m_fadeInTime;
        public float m_fadeOutTime;
        public uint m_tag;
        public float m_volumeFactor;
    }
}

public class SoundManager : MonoBehaviour, IManager
{
    public const string BGMPrefsKey = "soundmusic"; //用来收集存储的key
    public static bool mEnableBGM = true; //是否启用背景音乐
    public static bool mEnableSFX = true; //是否启用环境音效
    public static int mSFXChannelsCount = 30; //最大声道数量
    public const string SFXPrefsKey = "soundeffect";
    private static SoundManager sInstance;
    private static uint sUniqueTagId;
    private readonly MyAudioSource mBGAudioSource = new MyAudioSource(); //背景音乐源
    public float mBgmVolume = 1.0f; //场景背景音乐音量系数
    private float mCurBGVolume; //当前背景音乐音量
    private float mFadeInTime;
    private float mFadeInTimer;
    private FadeMode mFadeMode;
    private float mFadeOutTime;
    private float mFadeOutTimer;
    private int mLastMusicID = -1; //上次播放的场景背景音乐Id，用于中断后重播
    private SoundClip mNextSoundClip;
    private readonly MyAudioSource[] mSFXChannel = new MyAudioSource[mSFXChannelsCount]; //Sound FX，声音特效。
    public float mSfxVolume = 1.0f; //音效音量系数
    public SoundClipPools mSoundClipPools; //声音数据池 
    private readonly Dictionary<uint, MyAudioSource> mTaggedAudio = new Dictionary<uint, MyAudioSource>();
    private bool mVoicePlaying;

    public bool EnableBGM
    {
        get { return mEnableBGM; }
        set
        {
            if (mEnableBGM && !value)
            {
                if (mBGAudioSource != null)
                {
                    if (mBGAudioSource.m_AudioSource.isPlaying)
                    {
                        mBGAudioSource.m_AudioSource.Stop();
                    }
                }
            }
            else if (!mEnableBGM && value)
            {
                if (mBGAudioSource != null)
                {
                    PlayBGMWithFade(mLastMusicID, 0.1f, 0);
                }
            }
            mEnableBGM = value;
        }
    }

    public bool EnableSFX
    {
        get { return mEnableSFX; }
        set
        {
            if (mEnableSFX != value)
            {
                mEnableSFX = value;
                SetAreaSoundMute(value);
            }
        }
    }

    public static SoundManager Instance
    {
        get
        {
            if (null == sInstance)
            {
                var GameObject = new GameObject("SoundManager");
                DontDestroyOnLoad(GameObject);
                sInstance = GameObject.AddComponent<SoundManager>();
            }
            return sInstance;
        }
    }

    public static uint NextTag
    {
        get { return ++sUniqueTagId; }
    }

    public bool VoicePlaying
    {
        set
        {
            mVoicePlaying = value;
            if (value)
            {
                mBGAudioSource.m_AudioSource.mute = true;

                if (mEnableSFX)
                {
                    StopAllSoundEffect();
                }
            }
            else
            {
                mBGAudioSource.m_AudioSource.mute = false;
            }
        }
    }

    ////////////////////////////////////方法实现//////////////////////////////////////////

    private void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        for (var i = 0; i < mSFXChannelsCount; ++i)
        {
            mSFXChannel[i] = new MyAudioSource();
            mSFXChannel[i].m_uID = -1;
            mSFXChannel[i].m_AudioSource = gameObject.AddComponent<AudioSource>();
        }
        mBGAudioSource.m_AudioSource = gameObject.AddComponent<AudioSource>();
        mBGAudioSource.m_uID = -1;
        mSoundClipPools = new SoundClipPools();

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void FixedUpdate()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        UpdateBGMusic();

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public bool IsBGMPlaying(int nSoundId)
    {
        if (nSoundId == mLastMusicID)
        {
            return mBGAudioSource.m_AudioSource.isPlaying;
        }

        return false;
    }

    public bool IsPlaying(int nSoundId)
    {
        SoundClip clip = null;
        if (mSoundClipPools.m_SoundClipMap.TryGetValue(nSoundId, out clip))
        {
            for (var nIndex = 0; nIndex < mSFXChannelsCount; ++nIndex) //先找已经在播放或者播放过的
            {
                if (null != mSFXChannel[nIndex] && mSFXChannel[nIndex].m_uID == clip.mUID)
                {
                    if (null != mSFXChannel[nIndex].m_AudioSource)
                    {
                        return mSFXChannel[nIndex].m_AudioSource.isPlaying;
                    }
                }
            }
        }
        return false;
    }

    private void OnPlayBGMWithFade(SoundClip bgSoundClip, SoundClipPools.SoundClipParam param, bool reset)
    {
        if (mBGAudioSource != null && bgSoundClip != null)
        {
            mLastMusicID = param.m_clipID;
            if (mBGAudioSource.m_AudioSource.isPlaying) //正常播放上一首背景音乐
            {
                if (mNextSoundClip != null && mNextSoundClip.mUID == bgSoundClip.mUID)
                {
                    return;
                }
                mFadeOutTime = param.m_fadeOutTime;
                mFadeInTime = param.m_fadeInTime;
                mFadeOutTimer = 0;
                mFadeInTimer = 0;
                if (mFadeOutTime <= 0)
                {
                    //force Remove clip from pool, special for bgmusic
                    var deluid = mBGAudioSource.m_uID;
                    SetAudioSource(mBGAudioSource, bgSoundClip, 1.0f);
                    mSoundClipPools.ForceRemoveClipByID(deluid);

                    mCurBGVolume = bgSoundClip.mVolume;
                    if (mFadeInTime <= 0)
                    {
                        mBGAudioSource.m_AudioSource.Play();
                        mFadeMode = FadeMode.None;
                    }
                    else
                    {
                        mBGAudioSource.m_AudioSource.volume = 0;
                        mBGAudioSource.m_AudioSource.Play();
                        mFadeMode = FadeMode.FadeIn;
                    }
                }
                else
                {
                    mNextSoundClip = bgSoundClip;
                    mFadeMode = FadeMode.FadeOut;
                }
            }
            else //没在播放，直接播放
            {
                mFadeInTime = param.m_fadeInTime;
                mFadeInTimer = 0;

                //force Remove clip from pool, special for bgmusic
                var deluid = mBGAudioSource.m_uID;
                SetAudioSource(mBGAudioSource, bgSoundClip, 1.0f);
                mSoundClipPools.ForceRemoveClipByID(deluid);

                mCurBGVolume = bgSoundClip.mVolume;
                if (mFadeInTime <= 0)
                {
                    mBGAudioSource.m_AudioSource.Play();
                    mFadeMode = FadeMode.None;
                }
                else
                {
                    mBGAudioSource.m_AudioSource.volume = 0;
                    mBGAudioSource.m_AudioSource.Play();
                    mFadeMode = FadeMode.FadeIn;
                }
            }
        }
    }

    private void OnPlaySoundEffect(SoundClip soundClip, SoundClipPools.SoundClipParam param, bool reset)
    {
        if (soundClip == null)
        {
            Logger.Error("soundClip is null");
            return;
        }

        PlaySoundEffect(soundClip, param, reset);
    }

    /// <summary>
    ///     淡入淡出播放背景音乐
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="fadeOutTime">淡出时间</param>
    /// <param name="fadeInTime">淡入时间</param>
    public void PlayBGMusic(int nClipID, float fadeOutTime, float fadeInTime, bool forcePlay = true)
    {
        if (AudioListener.volume == 0)
        {
            return;
        }

        if (nClipID < 0)
        {
            Logger.Error("PlayBGM id < 0");
            return;
        }

        if (!forcePlay && mLastMusicID == nClipID)
        {
            return;
        }
        mLastMusicID = nClipID;

        if (mEnableBGM)
        {
            PlayBGMWithFade(nClipID, fadeOutTime, fadeInTime);
        }
    }

    /// <summary>
    ///     淡入淡出播放背景音乐
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="fadeInTime">淡入时间</param>
    /// <param name="fadeOutTime">淡出时间</param>
    private void PlayBGMWithFade(int nSoundclipId, float fadeOutTime, float fadeInTime)
    {
        mSoundClipPools.GetSoundClip(nSoundclipId, OnPlayBGMWithFade,
            new SoundClipPools.SoundClipParam(nSoundclipId, fadeOutTime, fadeInTime), false);
    }

    /// <summary>
    ///     播放音效，默认音量缩放系数可以不填，配表值
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volumeFactor">音量缩放系数</param>
    /// <returns></returns>
    public void PlaySoundEffect(int nSoundID, float volumeFactor = 1.0f, uint tag = 0, bool reset = false)
    {
        if (nSoundID == -1 || mVoicePlaying)
        {
            return;
        }

        if (AudioListener.volume == 0)
        {
            return;
        }

        if (!mEnableSFX)
        {
            return;
        }

        if (name == null)
        {
            Logger.Error("PlaySoundEffect name is null");
            return;
        }

        name = name.Trim();

        mSoundClipPools.GetSoundClip(nSoundID, OnPlaySoundEffect, new SoundClipPools.SoundClipParam(volumeFactor, tag),
            reset);
    }

    /// <summary>
    ///     播放soundClip中的音效
    /// </summary>
    /// <param name="soundClip"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
    private void PlaySoundEffect(SoundClip soundClip, SoundClipPools.SoundClipParam param, bool reset = false)
    {
        if (mVoicePlaying)
        {
            return;
        }

        if (mEnableSFX && !string.IsNullOrEmpty(soundClip.mPath))
        {
            if (soundClip.Audioclip == null)
            {
                Logger.Error("PlaySoundEffect soundClip.Audioclip is null");
                return;
            }

            var leastImportantIndex = 0;
            var nCurMaxPlayingCount = 0; //最大播放次数
            var nFirstEmptyIndex = -1; //第一个空位
            var nFirstSameClipValidIndex = -1; //第一个已经停止的上次播放过的位置

            var firstUse = -1;
            for (var nIndex = 0; nIndex < mSFXChannelsCount; ++nIndex) //先找已经在播放或者播放过的
            {
                if (mSFXChannel[nIndex] == null)
                {
                    return; //error
                }

                if (nFirstEmptyIndex == -1 && !mSFXChannel[nIndex].m_AudioSource.isPlaying) //记录第一个可用的空位置
                {
                    nFirstEmptyIndex = nIndex;
                }

                if (mSFXChannel[nIndex].m_AudioSource.clip == null)
                {
                    continue;
                }

                if (mSFXChannel[nIndex].m_uID == soundClip.mUID) //有播放过的内容
                {
                    if (nFirstSameClipValidIndex == -1 && !mSFXChannel[nIndex].m_AudioSource.isPlaying)
                        //记录第一个已经停止的上次播放过的位置
                    {
                        nFirstSameClipValidIndex = nIndex;
                    }


                    if (mSFXChannel[nIndex].m_AudioSource.isPlaying)
                    {
                        ++nCurMaxPlayingCount; //正在播放的计数
                        if (firstUse == -1)
                        {
                            firstUse = nIndex;
                        }
                    }
                    if (nCurMaxPlayingCount >= soundClip.mCurMaxPlayingCount) //已经超过了，不播放了
                    {
                        break;
                    }
                }

                if (mSFXChannel[leastImportantIndex].m_AudioSource.priority < mSFXChannel[nIndex].m_AudioSource.priority)
                    //记录最低优先级
                {
                    leastImportantIndex = nIndex;
                }
            }

            if (nCurMaxPlayingCount < soundClip.mCurMaxPlayingCount) //没到播放数量限制，直接播放
            {
                var nValidIndex = -1; //先选择已经停止播放的，如果没有，选第一个空的，如果也没有空的，替换优先数字最高的
                if (nFirstSameClipValidIndex != -1)
                {
                    nValidIndex = nFirstSameClipValidIndex;
                }
                else
                {
                    if (nFirstEmptyIndex != -1)
                    {
                        nValidIndex = nFirstEmptyIndex;
                    }
                    else
                    {
                        nValidIndex = leastImportantIndex;
                    }
                }

                if (nValidIndex >= 0 && nValidIndex < mSFXChannelsCount)
                {
                    var channel = mSFXChannel[nValidIndex];
                    channel.m_AudioSource.Stop();
                    if (channel.m_tag != 0)
                    {
                        mTaggedAudio.Remove(channel.m_tag);
                    }
                    SetAudioSource(channel, soundClip, param.m_volumeFactor);
                    if (param.m_tag != 0)
                    {
                        channel.m_tag = param.m_tag;
                        mTaggedAudio.Add(channel.m_tag, channel);
                    }
                    channel.m_AudioSource.PlayDelayed(soundClip.mDelay);
                }
            }
            else
            {
                if (reset)
                {
                    if (firstUse != -1)
                    {
                        mSFXChannel[firstUse].m_AudioSource.PlayDelayed(soundClip.mDelay);
                    }
                }


                //达到播放上限，不播放
                //Logger.Info("Warning PlaySoundEffect " + soundClip.m_name + " PlayingCount = " + nCurMaxPlayingCount);
            }
        }
    }

    //////////////////////////////////播放音效////////////////////////////////////
    /// <summary>
    ///     根据目标的坐标和接收者的坐标listenerPos的距离来确定音量,用于技能音效
    /// </summary>
    /// <param name="nSoundID"></param>
    /// <param name="playSoundPos"></param>
    /// <param name="listenerPos"></param>
    public void PlaySoundEffectAtPos(int nSoundID, Vector3 playSoundPos, Vector3 listenerPos, uint tag = 0)
    {
        if (nSoundID < 0)
        {
            return;
        }

        var volume = 1.0f;
        listenerPos.y = 0;
        playSoundPos.y = 0;

        var distance = Vector3.Distance(listenerPos, playSoundPos);

        volume = (0.4f - distance/10.0f)*2.5f;
        if (volume < 0.01f)
        {
            volume = 0.01f;
            return; //声音过低就返回
        }
        if (volume > 1.0f)
        {
            volume = 1.0f;
        }

        PlaySoundEffect(nSoundID, volume, tag);
    }

    //////////////////////////////////播放音效////////////////////////////////////
    /// <summary>
    ///     根据目标的坐标和接收者的坐标listenerPos的距离来确定音量，用于受击、死亡
    /// </summary>
    /// <param name="nSoundID"></param>
    /// <param name="playSoundPos"></param>
    /// <param name="listenerPos"></param>
    public void PlaySoundEffectAtPos2(int nSoundID,
                                      Vector3 playSoundPos,
                                      Vector3 listenerPos,
                                      uint tag = 0,
                                      bool reset = false)
    {
        if (nSoundID < 0)
        {
            return;
        }

        var volume = 1.0f;
        listenerPos.y = 0;
        playSoundPos.y = 0;

        var distance = Vector3.Distance(listenerPos, playSoundPos);

        volume = (0.6f - distance/10.0f)*1.67f;
        if (volume < 0.05f)
        {
            volume = 0.05f;
            return; //声音过低就返回
        }
        if (volume > 1.0f)
        {
            volume = 1.0f;
        }

        PlaySoundEffect(nSoundID, volume, tag, reset);
    }

    public void SetAreaSoundMute(bool enable)
    {
        var obj = GameObject.Find("AreaSound");
        if (null != obj)
        {
            OptList<AudioSource>.List.Clear();
            obj.GetComponentsInChildren(OptList<AudioSource>.List);
            var audios = OptList<AudioSource>.List;
            var count = audios.Count;
            for (var i = 0; i < count; i++)
            {
                audios[i].mute = !enable;
            }
        }
    }

    //赋值
    private void SetAudioSource(MyAudioSource audioSource, SoundClip clip, float volumeFactor)
    {
        if (clip == null)
        {
            Logger.Error("Error Clip null, please resolve");
            return;
        }
        audioSource.m_AudioSource.clip = clip.Audioclip;
        audioSource.m_AudioSource.volume = clip.mVolume*volumeFactor*mSfxVolume*10;
        audioSource.m_AudioSource.spread = clip.mSpread;
        audioSource.m_AudioSource.priority = clip.mPriority;
        audioSource.m_AudioSource.panLevel = clip.mPanLevel;
        audioSource.m_AudioSource.minDistance = clip.mMinDistance;
        audioSource.m_AudioSource.loop = clip.mIsLoop;
        audioSource.m_uID = clip.mUID;
    }

    public IEnumerator SetBgmPause(bool b = true)
    {
        if (mBGAudioSource != null)
        {
            if (mEnableBGM)
            {
                if (b)
                {
                    if (mBGAudioSource.m_AudioSource.isPlaying)
                    {
                        var time = 0.2f;
                        while (time > 0)
                        {
                            mBGAudioSource.m_AudioSource.volume = (time/0.2f)*mBgmVolume;
                            time -= Time.deltaTime;
                            yield return null;
                        }
                        mBGAudioSource.m_AudioSource.Pause();
                    }
                }
                else
                {
                    if (mBGAudioSource.m_AudioSource.isPlaying)
                    {
                        yield break;
                    }
                    mBGAudioSource.m_AudioSource.volume = 0;
                    mBGAudioSource.m_AudioSource.Play();
                    float time = 0;
                    while (time < 1.4f)
                    {
                        mBGAudioSource.m_AudioSource.volume = (time/1.4f)*mBgmVolume;
                        time += Time.deltaTime;
                        yield return null;
                    }
                    mBGAudioSource.m_AudioSource.volume = mBgmVolume;
                }
                //var tbSound = Table.GetSound(mBGAudioSource.m_uID);
                //mBGAudioSource.m_AudioSource.volume = v * mBgmVolume;
            }
        }
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

    public void StopAllSoundEffect()
    {
        if (mSFXChannel != null)
        {
            for (var nIndex = 0; nIndex < mSFXChannelsCount; ++nIndex)
            {
                if (mSFXChannel[nIndex] != null)
                {
                    mSFXChannel[nIndex].m_AudioSource.Stop();
                }
            }
        }
    }

    /// <summary>
    ///     停止背景音乐
    /// </summary>
    /// <param name="_fadeTime"></param>
    public void StopBGM(float _fadeTime)
    {
        if (mEnableBGM)
        {
            StartCoroutine(StopBGMWithFade(_fadeTime));
        }
    }

    private IEnumerator StopBGMWithFade(float _fadeTime)
    {
        if (mBGAudioSource != null)
        {
            if (mBGAudioSource.m_AudioSource.isPlaying)
            {
                var time = _fadeTime;
                while (time > 0)
                {
                    mBGAudioSource.m_AudioSource.volume = (time/mFadeOutTime)*mBgmVolume;
                    time -= Time.deltaTime;
                    yield return null;
                }
                mBGAudioSource.m_AudioSource.Stop();
            }
        }
    }

    /// <summary>
    ///     停止音效
    /// </summary>
    /// <param name="name">名称,声音表格第一列</param>
    public void StopSoundEffect(int nSoundID)
    {
        if (mSFXChannel == null)
        {
            return;
        }

        for (var nIndex = 0; nIndex < mSFXChannelsCount; nIndex++)
        {
            if (mSFXChannel[nIndex].m_AudioSource == null)
            {
                continue;
            }

            if (mSFXChannel[nIndex].m_uID == nSoundID)
            {
                mSFXChannel[nIndex].m_AudioSource.Stop();
            }
        }
    }

    public void StopSoundEffectByTag(uint tag)
    {
        MyAudioSource s;
        if (mTaggedAudio.TryGetValue(tag, out s))
        {
            if (s.m_tag == tag)
            {
                s.m_AudioSource.Stop();
            }
        }
    }

    private void UpdateBGMusic()
    {
        if (mFadeMode == FadeMode.FadeOut)
        {
            if (Math.Abs(mFadeOutTime) < 0.001f)
            {
                return; //保护代码
            }

            mFadeOutTimer += Time.deltaTime;
            mBGAudioSource.m_AudioSource.volume = (1 - mFadeOutTimer/mFadeOutTime)*mBgmVolume;
            if (mFadeOutTimer >= mFadeOutTime)
            {
                //force Remove clip from pool, special for bgmusic
                var deluid = mBGAudioSource.m_uID;
                SetAudioSource(mBGAudioSource, mNextSoundClip, 1.0f);
                mSoundClipPools.ForceRemoveClipByID(deluid);
                if (mFadeInTime > 0)
                {
                    mFadeMode = FadeMode.FadeIn;
                    mFadeOutTimer = 0;
                    mBGAudioSource.m_AudioSource.volume = 0;
                }
                else
                {
                    mFadeMode = FadeMode.None;
                    //m_BGAudioSource.volume = m_bgmVolume;
                }
                mBGAudioSource.m_AudioSource.Play();
            }
        }
        else if (mFadeMode == FadeMode.FadeIn)
        {
            if (Math.Abs(mFadeInTime) < 0.001f)
            {
                return; //保护代码
            }

            mFadeInTimer += Time.deltaTime;
            mBGAudioSource.m_AudioSource.volume = mFadeInTimer/mFadeInTime*mBgmVolume;
            if (mFadeInTimer >= mFadeInTime)
            {
                mFadeMode = FadeMode.None;
                mFadeInTimer = 0;
                mBGAudioSource.m_AudioSource.volume = mCurBGVolume;
            }
        }
    }

    //////////////////////////////////播放音效结束////////////////////////////////////

    public IEnumerator Init()
    {
        yield return null;
    }

    public void Reset()
    {
        mTaggedAudio.Clear();
        StopAllSoundEffect();
    }

    public void Tick(float delta)
    {
    }

    public void Destroy()
    {
    }

    public class MyAudioSource
    {
        public MyAudioSource()
        {
            m_uID = -1;
            m_AudioSource = null;
        }

        public AudioSource m_AudioSource;
        public uint m_tag;
        public int m_uID; //声音表格配的唯一标示符
    }

    private enum FadeMode //播放模式
    {
        None,
        FadeIn, //淡入
        FadeOut //淡出
    }
}