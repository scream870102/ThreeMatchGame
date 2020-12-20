using System.Collections.Generic;
using UnityEngine;
using Eccentric;
using Lean.Pool;
using TmUnity.Node;
namespace TmUnity
{
    class FXController : MonoBehaviour
    {
        [SerializeField] List<Color> starColors = null;
        [SerializeField] GameObject magicBarrierPrefab = null;
        [SerializeField] GameObject starPrefab = null;
        [SerializeField] GameObject holyPrefab = null;
        [SerializeField] GameObject shinePrefab = null;
        [SerializeField] GameObject eyePrefab = null;
        [SerializeField] RectTransform gamePanel = null;
        [SerializeField] RectTransform damageRectTF = null;
        [SerializeField] RectTransform enemyRectTF = null;
        [SerializeField] RectTransform defRectTF = null;
        [SerializeField] RectTransform energyRectTF = null;
        [SerializeField] float fireballVel = 0f;

        void HandleVFXPlay(OnVFXPlay e)
        {
            switch (e.Type)
            {
                case VFXType.ELIMINATE:
                    LeanPool.Spawn(magicBarrierPrefab, e.Pos, Quaternion.identity, gamePanel);
                    break;
                case VFXType.BUFF:
                    LeanPool.Spawn(holyPrefab, e.Pos, Quaternion.identity, gamePanel);
                    break;
                case VFXType.HEAL:
                    LeanPool.Spawn(shinePrefab, e.Pos, Quaternion.identity, gamePanel);
                    break;
                case VFXType.STARE:
                    LeanPool.Spawn(eyePrefab, enemyRectTF.position, Quaternion.identity, gamePanel);
                    break;
            }
        }

        async void HandlePlayerVFXPlay(OnPlayerVFXPlay e)
        {
            switch (e.Type)
            {
                case NodeType.ATTACK:
                    await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerVFX>().Attack(e.StartPos, damageRectTF.position, starColors[(int)e.Type], fireballVel);
                    break;
                case NodeType.MANA:
                    await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerVFX>().Attack(e.StartPos, damageRectTF.position, starColors[(int)e.Type], fireballVel);
                    break;
                case NodeType.DEFENSE:
                    await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerVFX>().Attack(e.StartPos, defRectTF.position, starColors[(int)e.Type], fireballVel);
                    break;
                case NodeType.ENERGY:
                    await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerVFX>().Attack(e.StartPos, energyRectTF.position, starColors[(int)e.Type], fireballVel);
                    break;
            }
        }

        void OnEnable()
        {
            DomainEvents.Register<OnVFXPlay>(HandleVFXPlay);
            DomainEvents.Register<OnPlayerVFXPlay>(HandlePlayerVFXPlay);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnVFXPlay>(HandleVFXPlay);
            DomainEvents.UnRegister<OnPlayerVFXPlay>(HandlePlayerVFXPlay);
        }
    }

}