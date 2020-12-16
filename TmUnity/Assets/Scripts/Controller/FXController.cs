using System.Collections.Generic;
using UnityEngine;
using Eccentric;
using Lean.Pool;
namespace TmUnity
{
    class FXController : MonoBehaviour
    {
        [SerializeField] List<Color> starColors = null;
        [SerializeField] GameObject magicBarrierPrefab = null;
        [SerializeField] GameObject starPrefab = null;
        [SerializeField] GameObject holyPrefab = null;
        [SerializeField] RectTransform gamePanel = null;
        [SerializeField] RectTransform enemyRectTF = null;
        [SerializeField] float fireballVel = 0f;

#if UNITY_EDITOR
        async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerAttackVFX>().Attack(new Vector3(55.7f, 454.1f, 0f), enemyRectTF.position, starColors[0], fireballVel);
        }
#endif

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

        async void HandlePlayerAtkAnim(OnPlayerAtkAnim e) => await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerAttackVFX>().Attack(e.StartPos, enemyRectTF.position, starColors[(int)e.Type], fireballVel);

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