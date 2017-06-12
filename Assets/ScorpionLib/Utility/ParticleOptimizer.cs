using UnityEngine;
using System.Collections;

public class ParticleOptimizer : MonoBehaviour
{

    private ParticleSystem mParticleSystem;

    private void Start()
    {
        mParticleSystem = transform.GetComponent<ParticleSystem>();
    }

    private void OnBecameVisible()
    {
        mParticleSystem.Play();
    }

    private void OnBecameInvisible()
    {
        mParticleSystem.Pause();
    }
}
