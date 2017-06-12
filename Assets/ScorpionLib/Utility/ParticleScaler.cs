using UnityEngine;
using System.Collections;

public class ParticleScaler : MonoBehaviour
{
    private ParticleSystem[] mParticles;
    private float mScale;
    private Transform mTransform;
    public bool IsUi = false;
    void Awake()
    {
        mParticles = GetComponentsInChildren<ParticleSystem>();
        mScale = 1;
        mTransform = transform;
    }

    private void Update()
    {
        var scale = 1.0f;
        if (IsUi)
        {
            scale = mTransform.localScale.x;
        }
        else
        {
            scale = mTransform.lossyScale.x;
        }
        if (Mathf.Approximately(mScale, scale))
        {
            return;
        }

        // revert last changes.
        for (var i = 0; i < mParticles.Length; ++i)
        {
            var particle = mParticles[i];
            particle.startSize /= mScale;
            particle.startSpeed /= mScale;
        }

        mScale = scale;

        // apply changes.
        for (var i = 0; i < mParticles.Length; ++i)
        {
            var particle = mParticles[i];
            particle.startSize *= scale;
            particle.startSpeed *= scale;
        }
    }

    void OnDisable()
    {
        // revert last changes.
        for (var i = 0; i < mParticles.Length; ++i)
        {
            var particle = mParticles[i];
            if (!particle) continue;
            particle.startSize /= mScale;
            particle.startSpeed /= mScale;
        }

        mScale = 1;
    }

    public void Play()
    {
        for (var i = 0; i < mParticles.Length; ++i)
        {
            var particle = mParticles[i];
            particle.Simulate(0, true, true);
            particle.Play();
        }
    }
}