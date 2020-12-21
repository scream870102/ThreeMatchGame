using UnityEngine;
using Eccentric.Utils;
using Eccentric;
using TmUnity.Node;

namespace TmUnity.Node
{
    [System.Serializable]
    class NodeAttr
    {
        [SerializeField] int atk = 0;
        //[SerializeField] int chargeAtk = 0;
        [SerializeField] int mana = 0;
        [SerializeField] int def = 0;
        [SerializeField] float energy = 0f;
        [SerializeField] ChestNodeAttr chestNodeAttr = null;
        public int Atk => atk;
        //public int ChargeAtk => chargeAtk;
        public int Mana => mana;
        public int Def => def;
        public float Energy => energy;
        public ChestNodeAttr ChestNodeAttr => chestNodeAttr;

    }

    [System.Serializable]
    class ChestNodeAttr
    {
        [SerializeField] int hpRecover = 0;
        [SerializeField] float energyUp = 0f;
        [SerializeField] int atkUp = 0;
        [SerializeField] int defUp = 0;
        //[SerializeField] int chargeCount = 0;
        public int HPRecover => hpRecover;
        public float EnergyUp => energyUp;
        public int AtkUp => atkUp;
        public int DefUp => defUp;
        //public int ChargeCount => chargeCount;
        public ChestNodeAttr(int hpRecover, int energyUp, int atkUp, int defUp)
        {
            this.hpRecover = hpRecover;
            this.energyUp = energyUp;
            this.atkUp = atkUp;
            this.defUp = defUp;
            //this.chargeCount = chargeCount;
        }
    }
}

namespace TmUnity
{

    [System.Serializable]
    class PlayerAttr
    {
        [SerializeField] int basicAtk = 0;
        [SerializeField] int basicMana = 0;
        [SerializeField] int basicDef = 0;
        [SerializeField] float basicEnergy = 0f;
        [SerializeField] int hp = 4600;
        public int BasicAtk => basicAtk;
        public int BasicMana => basicMana;
        public int BasicDef => basicDef;
        public float BasicEnergy => basicEnergy;
        public int HP => hp;
    }

    [System.Serializable]
    class AttackAttr
    {
        [SerializeField] string animTrigger = "";
        [SerializeField] int atk = 0;
        [SerializeField] float time = 0f;
        [SerializeField] int cd = 0;
        public string AnimTrigger => animTrigger;
        public int Atk => atk;
        public float Time => time;
        public int CD => cd;

    }

    [System.Serializable]
    class GameStats
    {
#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] int currentAtk = 0;
#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] int currentMana = 0;
#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] int currentDef = 0;
#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] float nextRoundDuration = 0f;
#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] int currentHP = 0;
#if UNITY_EDITOR
        [ReadOnly]
# endif
        [SerializeField] int currentCombo = 0;
        public int CurrentAtk
        {
            get => currentAtk;
            set
            {
                currentAtk = value;
                DomainEvents.Raise<OnAtkChanged>(new OnAtkChanged(CurrentAtk));
            }
        }

        public int CurrentMana
        {
            get => currentMana;
            set
            {
                currentMana = value;
                DomainEvents.Raise<OnManaChanged>(new OnManaChanged(CurrentMana));
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
        public int Atk { get; set; } = 0;
        public int Mana { get; set; } = 0;
        public float EnergyTime { get; set; } = 0f;
        public int Def { get; set; } = 0;
        public int HPRecover { get; set; } = 0;

    }

    [System.Serializable]
    class GameResultStats
    {
        public int MaxDamage = 0;
        public float ElapsedTime = 0f;
    }

}