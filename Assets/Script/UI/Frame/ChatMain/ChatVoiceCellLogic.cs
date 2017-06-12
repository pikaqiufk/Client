using System;
#region using

using System.Collections;
using EventSystem;
using UnityEngine;

#endregion

public class ChatVoiceCellLogic : MonoBehaviour
{
    public UIWidget BackGround;
    public UISprite BarSprite;
    public UISpriteAnimation IcoAnimation;
    public BoxCollider IcoCollider;
    public UISprite IcoSprite;
    public int ListItemIndex;
    private int mHeight;
    private float mLastTime;
    public UILabel TimeLabel;
    public int Seconds { get; set; }
    public byte[] SoundData { get; set; }

    public void EndPlay(IEvent ievent)
    {
        SoundManager.Instance.VoicePlaying = false;
        if (null != IcoAnimation)
        {
            IcoAnimation.ResetToBeginning();
            IcoAnimation.Pause();
            mLastTime = 0;
        }
    }

    public void OnClickIco()
    {
        EventDispatcher.Instance.DispatchEvent(new ChatMainStopVoiceAnimation());
        StartPlay();
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(ChatMainStopVoiceAnimation.EVENT_TYPE, EndPlay);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private IEnumerator PlayFinished()
    {
        while (mLastTime > 0)
        {
            mLastTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        EndPlay(null);
    }

    public void SetChatInfo(int height, int second, int maxLength)
    {
        mHeight = height;

        IcoSprite.width = mHeight;
        IcoSprite.height = mHeight;

        TimeLabel.fontSize = mHeight - 2;

        BarSprite.height = mHeight - 2;

        var w = (int) ((maxLength - 2 - 2 - mHeight*2)*(second/30.0f)) + mHeight*2;
        BarSprite.width = w;
        BarSprite.transform.localPosition = new Vector3(mHeight + 2, -mHeight/2.0f, 0);

        BackGround.height = mHeight;
        BackGround.width = w + mHeight + 2 + 2;

        var str = second + GameUtils.GetDictionaryText(1045);
        TimeLabel.text = str;
    }

    public void SetSeconds(int second)
    {
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        IcoAnimation.ResetToBeginning();
        IcoAnimation.Pause();
        mLastTime = 0;
        EventDispatcher.Instance.AddEventListener(ChatMainStopVoiceAnimation.EVENT_TYPE, EndPlay);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void StartPlay()
    {
        SoundManager.Instance.VoicePlaying = true;
        IcoAnimation.Play();
        if (mLastTime <= 0)
        {
            mLastTime = Seconds;
            ResourceManager.Instance.StartCoroutine(PlayFinished());
        }
        mLastTime = Seconds;
        EventDispatcher.Instance.DispatchEvent(new ChatMainPlayVoice(SoundData));
    }
}