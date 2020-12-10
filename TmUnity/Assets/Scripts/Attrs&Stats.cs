using UnityEngine;
using Eccentric.Utils;
using Eccentric;
using TmUnity.Node;
namespace TmUnity
{
    [System.Serializable]
    class NodeAttr
    {
        [SerializeField] int normalAtk = 0;
        [SerializeField] int chargeAtk = 0;
        [SerializeField] int def = 0;
        [SerializeField] float energy = 0f;
        public int NormalAtk => normalAtk;
        public int ChargeAtk => chargeAtk;
        public int Def => def;
        public float Energy => energy;

    }
    [System.Serializable]
    class PlayerAttr
    {
        [SerializeField] int basicNormalAtk = 0;
        [SerializeField] int basicChargeAtk = 0;
        [SerializeField] int basicDef = 0;
        [SerializeField] float basicEnergy = 0f;
        [SerializeField] int maxChargeNum = 100;
        [SerializeField] int maxChargeAtk = 100;
        [SerializeField] int hp = 4600;
        public int BasicNormalAtk => basicNormalAtk;
        public int BasicChargeAtk => basicChargeAtk;
        public int BasicDef => basicDef;
        public float BasicEnergy => basicEnergy;
        public int MaxChargeNum => maxChargeNum;
        public int MaxChargeAtk => maxChargeAtk;
        public int HP => hp;
    }



    [System.Serializable]
    class GameStats
    {
        [ReadOnly] [SerializeField] int currentAtk = 0;
        [ReadOnly] [SerializeField] int currentChargeAtk = 0;
        [ReadOnly] [SerializeField] int currentDef = 0;
        [ReadOnly] [SerializeField] float nextRoundDuration = 0f;
        [ReadOnly] [SerializeField] int currentChargeCount = 0;
        [ReadOnly] [SerializeField] int currentHP = 0;
        [ReadOnly] [SerializeField] int currentCombo = 0;
        public int CurrentAtk
        {
            get => currentAtk;
            set
            {
                DomainEvents.Raise<OnAtkChanged>(new OnAtkChanged(CurrentAtk));
                currentAtk = value;
            }
        }
        public int CurrentChargeAtk
        {
            get => currentChargeAtk;
            set
            {
                DomainEvents.Raise<OnChargeAtkChanged>(new OnChargeAtkChanged(CurrentChargeAtk));
                currentChargeAtk = value;
            }
        }
        public int CurrentDef
        {
            get => currentDef;
            set
            {
                DomainEvents.Raise<OnDefChanged>(new OnDefChanged(CurrentDef));
                currentDef = value;
            }
        }
        public float NextRoundDuration
        {
            get => nextRoundDuration;
            set
            {
                DomainEvents.Raise<OnEnergyChanged>(new OnEnergyChanged(NextRoundDuration));
                nextRoundDuration = value;
            }
        }
        public int CurrentChargeCount
        {
            get => currentChargeCount;
            set
            {
                currentChargeCount = value;
                DomainEvents.Raise<OnChargeCountChange>(new OnChargeCountChange(CurrentChargeCount));
            }
        }
        public int CurrentHP
        {
            get => currentHP;
            set
            {
                currentHP = value;
                DomainEvents.Raise<OnPlayerHPChanged>(new OnPlayerHPChanged(CurrentHP));
            }
        }
        public int CurrentCombo
        {
            get => currentCombo;
            set
            {
                currentCombo = value;
                DomainEvents.Raise<OnComboChange>(new OnComboChange(CurrentCombo));
            }
        }

    }
}