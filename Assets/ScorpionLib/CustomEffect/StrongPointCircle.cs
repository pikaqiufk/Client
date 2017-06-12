using EventSystem;
using UnityEngine;

[RequireComponent(typeof(CircleMaker))]
public class StrongPointCircle : MonoBehaviour
{
    private CircleMaker mCircleMaker;
    private MeshRenderer mRenderer;

    //模型的半径
    public int Id;

    private int Camp = -1;

    private bool bTick;

    public const float NeedSec = 4.9f;

	void Awake ()
	{
	    mCircleMaker = GetComponent<CircleMaker>();
        mRenderer = GetComponent<MeshRenderer>();
	}

    void Start()
    {
        mCircleMaker.FillAmount = 0f;
    }

    void OnEnable()
    {
        EventDispatcher.Instance.AddEventListener(StrongpointStateChangedEvent.EVENT_TYPE, OnStrongpointStateChanged);
    }

    void OnDisable()
    {
        EventDispatcher.Instance.RemoveEventListener(StrongpointStateChangedEvent.EVENT_TYPE, OnStrongpointStateChanged);
    }

    void Update()
    {
        if (bTick)
        {
            mCircleMaker.FillAmount += Time.deltaTime/NeedSec;
            if (mCircleMaker.FillAmount >= 1f)
            {
                bTick = false;
                mCircleMaker.FillAmount = 1f;
            }
        }
    }


    void OnStrongpointStateChanged(IEvent ievent)
    {
        var e = ievent as StrongpointStateChangedEvent;
        if (e.Index != Id) return;
        var effectColor = string.Empty;
        switch (e.Camp)
        {
            case 4:
                mCircleMaker.Color = Color.blue;
                effectColor = "Blue";
                break;
            case 5:
                mCircleMaker.Color = Color.red;
                effectColor = "Red";
                break;
            case 7:
                mCircleMaker.Color = Color.yellow;
                effectColor = "Yellow";
                break;
            case 8:
                mCircleMaker.Color = Color.green;
                effectColor = "Green";
                break;
            case 9:
                mCircleMaker.Color = Color.blue;
                effectColor = "Blue";
                break;
        }

        var state = (eStrongpointState)e.State;
        switch (state)
        {
            case eStrongpointState.Idle:
                bTick = false;
                mCircleMaker.enabled = false;
                mRenderer.enabled = false;
                break;
            case eStrongpointState.Occupied:
                bTick = false;
                mCircleMaker.enabled = false;
                mRenderer.enabled = false;
                if (Camp != e.Camp)
                {
                    Camp = e.Camp;
                    if (Camp != -1 )//&& state == eStrongpointState.Occupied)
                    {
                        var battleEffect = gameObject.GetComponent<AllianceBattleEffectLogic>();
                        if (battleEffect != null)
                        {
                            battleEffect.Play(effectColor);
                        }
                    }
                }
                break;
            case eStrongpointState.Occupying:
                bTick = true;
                mCircleMaker.enabled = true;
                mRenderer.enabled = true;
                mCircleMaker.FillAmount = e.Time/NeedSec;
                break;
            case eStrongpointState.OccupyWait:
                bTick = false;
                break;
            case eStrongpointState.Contending:
                mCircleMaker.Color = Color.red;
                bTick = true;
                mCircleMaker.enabled = true;
                mRenderer.enabled = true;
                mCircleMaker.FillAmount = e.Time/NeedSec;
                break;
        }

      
    }
}
