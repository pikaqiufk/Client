using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviourMachine;

public class Delay : MonoBehaviour {
	
	public float delayTime = 1.0f;

    public void StartDelay()
    { 
        ComplexObjectPool.SetActive(gameObject, false);
        Invoke("DelayFunc", delayTime);
    }

    private void DelayFunc()
    {
        ComplexObjectPool.SetActive(gameObject, true);
    }

}
