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
        [SerializeField] ChestNodeAttr chestNodeAttr = null;
        public int NormalAtk => normalAtk;
        public int ChargeAtk => chargeAtk;
        public int Def => def;
        public float Energy => energy;
        public ChestNodeAttr ChestNodeAttr => chestNodeAttr;

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
    class AttackAttr
    {
        [SerializeField] int atk = 0;
        [SerializeField] float time = 0f;
        [SerializeField] int cd = 0;
        public int Atk => atk;
        public float Time => time;
        public int CD => cd;

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
                currentAtk = value;
                DomainEvents.Raise<OnAtkChanged>(new OnAtkChanged(CurrentAtk));
            }
        }
        public int CurrentChargeAtk
        {
            get => currentChargeAtk;
            set
            {
                currentChargeAtk = value;
                DomainEvents.Raise<OnChargeAtkChanged>(new OnChargeAtkChanged(CurrentChargeAtk));
            }
        }
        public int CurrentDef
        {
            get => currentDef;
            set
            {
                currentDef = value;
                DomainEvents.Raise<OnDefChanged>(new OnDefChanged(CurrentDef));
            }
        }
        public float NextRoundDuration
        {
            get => nextRoundDuration;
            set
            {
                nextRoundDuration = value;
                DomainEvents.Raise<OnEnergyChanged>(new OnEnergyChanged(NextRoundDuration));
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
                if (currentHP <= 0)
                    DomainEvents.Raise<OnPlayerDead>(new OnPlayerDead());
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

    class EliminateInfo
    {
        public int NormalAtk { get; set; } = 0;
        public int ChargeAtk { get; set; } = 0;
        public int ChargeNum { get; set; } = 0;
        public float EnergyTime { get; set; } = 0f;
        public int Def { get; set; } = 0;
        public int HPRecover { get; set; } = 0;

    }

    [System.Serializable]
    class ChestNodeAttr
    {
        [SerializeField] int hpRecover = 0;
        [SerializeField] float energyUp = 0f;
        [SerializeField] int atkUp = 0;
        [SerializeField] int defUp = 0;
        [SerializeField] int chargeCount = 0;
        public int HPRecover => hpRecover;
        public float EnergyUp => energyUp;
        public int AtkUp => atkUp;
        public int DefUp => defUp;
        public int ChargeCount => chargeCount;
        public ChestNodeAttr(int hpRecover, int energyUp, int atkUp, int defUp, int chargeCount)
        {
            this.hpRecover = hpRecover;
            this.energyUp = energyUp;
            this.atkUp = atkUp;
            this.defUp = defUp;
            this.chargeCount = chargeCount;
        }
    }

}