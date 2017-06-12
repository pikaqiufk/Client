using UnityEngine;
using System.Collections;
using System;
using MoPhoGames.USpeak.Core;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using MoPhoGames.USpeak.Codec;
using NSpeex;

public enum BandMode
{
	Narrow,
	Wide
}

public class WSpeak : MonoBehaviour {
	public int maxRecordingTime = 30;//second
	public int chunkSize = 640;//must>=640 and multiple of 640,because speex codec buf is 640
	public bool useVAD = false;
	public BandMode bandWidthMode = BandMode.Narrow;
	public delegate void OnRecordingEnd();
	public OnRecordingEnd onRecordingEnd;
	AudioSource sound;
	AudioClip recording;
	int  recordingUnit = 10;//second,must>=10
	int  sample;
	bool bRec;
	bool bPlay;
	bool bEnc;
	bool bMicOk;
	float volume;
	float speakTime;
	int lastReadPos;
	byte[] mData;
    //私聊
    public bool IsWhisper;
    public float RecordingLength
    {
        get { return speakTime; }
    }

    public GameObject InShow;
    public GameObject OutShow;
    public float VolumeFactor = 1.0f;
    private bool bSend;
    private bool bRecordingEnd;
    
    public static WSpeak instance = null;

	void Start ()
	{
	    instance = this;
		sound = GetComponent<AudioSource>();
		if(null == sound)throw new Exception("u3d not found the AudioSource");
		if(bandWidthMode!=BandMode.Narrow)throw new Exception("u3d SpeexCodec only support BandMode.Narrow now");
		sample = getFrequency(bandWidthMode);
		bPlay = false;
		bRec  = false;
		bMicOk= false;

	    bSend = false;
        InShow.SetActive(false);
        OutShow.SetActive(false);
	    onRecordingEnd = OnRecordEnd;
	}
	

	void Update () 
    {
		if(!bRec)return;
		
		bool bRecording = true;
		speakTime += Time.deltaTime;
		if(speakTime>=maxRecordingTime-1)
		{
			bRecording = false;
		}
		if(bRecording)bRecording=Microphone.IsRecording(null);
		
		//提取录音数据编码压缩/
		int readPos = Microphone.GetPosition( null );
		try
		{
			int sz = readPos>=lastReadPos?readPos-lastReadPos:readPos+recording.samples-lastReadPos;
			if(sz<1 || (sz<chunkSize && bRecording))return;
			float[] d = new float[ ( sz - 1 ) ];
            
			recording.GetData( d, lastReadPos );
			//rawData.AddRange(d);

            OnAudioAvailable( d );
			lastReadPos = readPos;
		}
		catch( System.Exception  e) 
		{
			Debug.Log("u3d exception="+e.Message);
			StopRecord();
			return;
		}
		
		//编码录音数据
		ProcessPendingEncodeBuffer();

		//停止并保存录音数据/
		if(!bRecording)
		{
			StopRecord();
			return;
		}
	}
	
	public static int getFrequency(BandMode bandMode)
	{
		return bandMode == BandMode.Wide?16000:8000;
	}

	public bool doBegin()
	{
		if(!isMicOk())return false;
		if(bEnc||bRec)return false;
		sound.Stop();
		setMute(true);
		//是否循环写buf/
		bool bLoop = maxRecordingTime>recordingUnit?true:false;
		recording = Microphone.Start(null, bLoop, recordingUnit, sample);
		if(recording == null)
		{
			setMute(false);
			bMicOk = false;
			Logger.Error("u3d open the mic device error");
			return false;
		}

		speakTime = 0.0f;
		lastReadPos = 0;
		bRec = true;
		bEnc = true;

		return true;
	}
	
	public void doEnd()
	{
      
        if(!bRec)return;
		setMute(false);
		if(Microphone.IsRecording(null))
		{
			Microphone.End(null);
		}
		bRec = false;
		StartCoroutine(ProcessPendingEncodeBufferEnd());
	}
	
	public void doPlay()
	{
		if(bEnc)return;
		sound.Stop();
		sound.mute = false;
		playEncodeData(speakData);
	}

    public static void PlayVoice(byte[] data)
    {
        if (instance != null)
        {
            instance.playEncodeData(data);
        }
    }

	void playEncodeData( byte[] data )
	{
	    if (sound.isPlaying)
	    {
	        sound.Stop();
	    }

	    var decodeData = GetDecodePcm(data);
        AudioClip stitch_clip = USpeakAudioClipConverter.ShortsToAudioClip(decodeData.ToArray(), 1, getFrequency(BandMode.Narrow), false, RemoteGain*VolumeFactor);
        sound.clip = stitch_clip;
        sound.Play();
	}

    public static short[] GetDecodePcm(byte[] data)
    {
        int offset = 0;
        var codec = new SpeexCodec();
        List<short> buffer = new List<short>();
        while (offset < data.Length)
        {
            USpeakFrameContainer cont = default(USpeakFrameContainer);
            var l = cont.LoadFrom(data, offset);
            short[] pcm = codec.Decode(cont.encodedData, BandMode.Narrow);
            offset += l;
            buffer.AddRange(pcm.ToArray());
        }
        return buffer.ToArray();
    }

	//for test
	void playRawData(List<float> raw)
	{
		AudioClip stitch_clip = AudioClip.Create( "clip", raw.Count, 1, sample, true, false );
		stitch_clip.SetData( raw.ToArray(), 0 );
	    sound.clip = stitch_clip;
	    sound.Play();
	}
	
	public byte[] speakData
	{
		get{return mData;}
		set{mData = value;}
	}
	
	void setMute(bool bMute)
	{
		if(bMute)
		{
			volume = AudioListener.volume;
			AudioListener.volume = 0;
		}
		else
		{
			AudioListener.volume = volume;
		}
	}
	
	bool isMicOk()
	{
		if(bMicOk)return true;
		if( !Application.HasUserAuthorization( UserAuthorization.Microphone ) )
		{
			Application.RequestUserAuthorization( UserAuthorization.Microphone );
			return false;
		}
		if( Microphone.devices.Length == 0 )
		{
			Logger.Debug( "Failed to find mic device");
			return false;
		}
		bMicOk = true;
		return true;
	}
	
	void OnDisable()
	{
		doEnd();
	}
//========================================
	private List<USpeakFrameContainer> sendBuffer = new List<USpeakFrameContainer>();
	private List<float[]> pendingEncode = new List<float[]>();
	private float lastVTime = 0.0f;
	private float vadHangover = 0.5f; // after detecting silence, USpeak waits for 500ms before disabling sending.
	private static float LocalGain = 1.0f;
	private static float RemoteGain = 1.0f;


	//过滤无效声音/
	void OnAudioAvailable( float[] pcmData )
	{
		if( useVAD && !CheckVAD( pcmData ) )
			return;

		List<float[]> audio_chunks = SplitArray( pcmData, chunkSize );
		foreach( float[] chunk in audio_chunks )
		{
			pendingEncode.Add( chunk );
		}
	}

	List<float[]> SplitArray( float[] array, int size )
	{
		List<float[]> chunksList = new List<float[]>();
		int skipCounter = 0;

		while( skipCounter < array.Length )
		{
			float[] chunk = array.Skip( skipCounter ).Take( size ).ToArray<float>();
			chunksList.Add( chunk );
			skipCounter += chunk.Length;
		}
		return chunksList;
	}

	void ProcessPendingEncodeBuffer()
	{
		int budget_ms = 10; //if time spent encoding exceeds this many milliseconds, abort and wait till next frame
		float budget_s = (float)budget_ms / 1000.0f;



		float t = Time.realtimeSinceStartup;
		while( Time.realtimeSinceStartup <= t + budget_s && pendingEncode.Count > 0 )
		{
			float[] pcm = pendingEncode[ 0 ];
			pendingEncode.RemoveAt( 0 );
			ProcessPendingEncode( pcm );
		}
	}
	
	//继续压缩未处理的数据/
	IEnumerator ProcessPendingEncodeBufferEnd()
	{
		while(pendingEncode.Count > 0 )
		{
			float[] pcm = pendingEncode[ 0 ];
			pendingEncode.RemoveAt( 0 );
			ProcessPendingEncode( pcm );
			yield return null;
		}
		
		List<byte> tempSendBytes = new List<byte>();
		foreach( USpeakFrameContainer frame in sendBuffer )
		{
			tempSendBytes.AddRange(frame.ToByteArray());
		}
		sendBuffer.Clear();
		mData = tempSendBytes.ToArray();
		bEnc = false;
		if(null!=onRecordingEnd)onRecordingEnd();
		yield return null;
	}

	void ProcessPendingEncode( float[] pcm )
	{
		// encode data and add it to the send buffer
		int s;
		byte[] b = USpeakAudioClipCompressor.CompressAudioData( pcm, 1, out s, bandWidthMode, LocalGain );

		USpeakFrameContainer cont = default( USpeakFrameContainer );
		cont.Samples = (ushort)s;
		cont.encodedData = b;
		sendBuffer.Add( cont );
	}

	bool CheckVAD( float[] samples )
	{
		if( Time.realtimeSinceStartup < lastVTime + vadHangover )
			return true;

		float max = 0.0f;
		foreach( float f in samples )
			max = Mathf.Max( max, Mathf.Abs( f ) );
		bool val = ( max >= 0.005f );
		if( val )
			lastVTime = Time.realtimeSinceStartup;
		return val;
	}
//========================================

    void OnPress(bool pressed)
    {
        if (pressed)
        {
            BegainRecord();
        }
        else
        {
            StopRecord();
        }
    }

    void BegainRecord()
    {
        InShow.SetActive(true);
        OutShow.SetActive(false);
        bSend = true;
        bRecordingEnd = false;
        doBegin();
    }

    void StopRecord()
    {
        InShow.SetActive(false);
        OutShow.SetActive(false);
        doEnd();
        if (bSend)
        {
            bSend = false;
            StartCoroutine(SendVoiceData());
        }
    }

    void OnDrag(Vector2 delta)
    {
        var obj = UICamera.hoveredObject;
        if (obj == gameObject)
        {
            InShow.SetActive(true);
            OutShow.SetActive(false);
            bSend = true;
        }
        else
        {
            InShow.SetActive(false);
            OutShow.SetActive(true);
            bSend = false;
        }
    }

    public void OnRecordEnd()
    {
        bRecordingEnd = true;
    }

    IEnumerator SendVoiceData()
    {
        int retryTimes = 0;
        while (!bRecordingEnd && retryTimes < 100)
        {
            retryTimes++;
            yield return new WaitForSeconds(0.1f);
        }

        if (speakData == null)
        {
            Logger.Debug("speakData = null");
            yield break;
        }

        if (speakData.Length > 1)
        {
            var ievent = new ChatMainSendVoiceData(speakData, RecordingLength, IsWhisper);
            EventDispatcher.Instance.DispatchEvent(ievent);    
        }
    }
}
