using UnityEngine;
using Eccentric.Utils;
namespace TmUnity
{
    [System.Serializable]
    class PlayerAttr
    {
        [SerializeField] int normalBasicAtk = 0;
        [SerializeField] int chargeBasicAtk = 0;
        [SerializeField] float energyBasicTimePlus = 0f;
        [SerializeField] int defBasicUp = 0;
        [SerializeField] int maxChargeNum = 100;
        [SerializeField] int maxChargeAtk = 100;
        public int NormalBasicAtk => normalBasicAtk;
        public int ChargeBasicAtk => chargeBasicAtk;
        public float EnergyBasicTimePlus => energyBasicTimePlus;
        public int DefBasicUp => defBasicUp;
        public int MaxChargeNum => maxChargeNum;
        public int MaxChargeAtk => maxChargeAtk;
    }

    [System.Serializable]
    class GameStats
    {
        [ReadOnly] [SerializeField] int currentAtk = 0;
        [ReadOnly] [SerializeField] int currentChargeCount = 0;
        [ReadOnly] [SerializeField] int currentDef = 0;
        [ReadOnly] [SerializeField] float nextRoundDuration = 0f;
        public int CurrentAtk { get => currentAtk; set { currentAtk = value; } }
        public int CurrentChargeCount { get => currentChargeCount; set { currentChargeCount = value; } }
        public int CurrentDef { get => currentDef; set { currentDef = value; } }
        public float NextRoundDuration { get => nextRoundDuration; set { nextRoundDuration = value; } }

    }
}