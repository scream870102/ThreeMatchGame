using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eccentric;
using Lean.Pool;
namespace TmUnity
{
    class FXController : MonoBehaviour
    {
        [SerializeField] GameObject magicBarrierPrefab = null;
        [SerializeField] RectTransform gamePanel = null;

        void HandleVFXPlay(OnVFXPlay e)
        {
            LeanPool.Spawn(magicBarrierPrefab, e.Pos, Quaternion.identity, gamePanel);
        }
        void OnEnable()
        {
            DomainEvents.Register<OnVFXPlay>(HandleVFXPlay);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnVFXPlay>(HandleVFXPlay);
        }
    }

}
