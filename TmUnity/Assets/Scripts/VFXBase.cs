using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
namespace TmUnity
{
    class VFXBase : MonoBehaviour
    {
        void OnAnimationEnd() => LeanPool.Despawn(gameObject);
    }
}
