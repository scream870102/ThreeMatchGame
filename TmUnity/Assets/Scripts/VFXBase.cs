using UnityEngine;
using Lean.Pool;
namespace TmUnity
{
    class VFXBase : MonoBehaviour
    {
        void OnAnimationEnd() => LeanPool.Despawn(gameObject);
    }
}
