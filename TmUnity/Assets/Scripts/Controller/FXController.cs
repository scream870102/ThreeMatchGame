﻿using System.Collections.Generic;
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
        [SerializeField] RectTransform gamePanel = null;
        [SerializeField] RectTransform enemyRectTF = null;
        [SerializeField] RectTransform defRectTF = null;
        [SerializeField] RectTransform energyRectTF = null;
        [SerializeField] float fireballVel = 0f;

#if UNITY_EDITOR
        async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerVFX>().Attack(new Vector3(55.7f, 454.1f, 0f), enemyRectTF.position, starColors[0], fireballVel);
        }
#endif

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
            }
        }

        async void HandlePlayerVFXPlay(OnPlayerVFXPlay e)
        {
            switch (e.Type)
            {
                case NodeType.NORMAL:
                    await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerVFX>().Attack(e.StartPos, enemyRectTF.position, starColors[(int)e.Type], fireballVel);
                    break;
                case NodeType.CHARGE:
                    await LeanPool.Spawn(starPrefab, gamePanel).GetComponent<PlayerVFX>().Attack(e.StartPos, enemyRectTF.position, starColors[(int)e.Type], fireballVel);
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