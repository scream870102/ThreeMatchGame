using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
namespace TmUnity
{
    class VFXBase : MonoBehaviour
    {
        ParticleSystem ptc = null;
        void Awake() => ptc = GetComponent<ParticleSystem>();
        void OnParticleSystemStopped() => LeanPool.Despawn(gameObject);
        void OnEnable() => ptc.Play();

    }
}
