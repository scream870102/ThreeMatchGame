using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eccentric;
using Lean.Pool;
using System.Threading.Tasks;
namespace TmUnity
{
    class FXController : MonoBehaviour
    {
        [SerializeField] List<Color> starColors = null;
        [SerializeField] GameObject magicBarrierPrefab = null;
        [SerializeField] GameObject starPrefab = null;
        [SerializeField] GameObject holyPrefab = null;
        [SerializeField] RectTransform gamePanel = null;

        void HandleVFXPlay(OnVFXPlay e)
        {
            switch (e.Type)
            {
                case VFXType.ELIMINATE:
                    LeanPool.Spawn(magicBarrierPrefab, e.Pos, Quaternion.identity, gamePanel);
                    break;
                case VFXType.HEAL:
                    LeanPool.Spawn(holyPrefab, e.Pos, Quaternion.identity, gamePanel);
                    break;
            }
        }

        async void HandlePlayerAtkAnim(OnPlayerAtkAnim e) => await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerAttackVFX>().Attack(e.StartPos, e.EndPos, starColors[(int)e.Type], e.Vel);

        void OnEnable()
        {
            DomainEvents.Register<OnVFXPlay>(HandleVFXPlay);
            DomainEvents.Register<OnPlayerAtkAnim>(HandlePlayerAtkAnim);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnVFXPlay>(HandleVFXPlay);
            DomainEvents.UnRegister<OnPlayerAtkAnim>(HandlePlayerAtkAnim);
        }
    }

}
