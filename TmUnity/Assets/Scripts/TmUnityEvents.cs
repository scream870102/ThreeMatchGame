using Eccentric;
namespace TmUnity.Node
{
    class OnNodeDragBegin : IDomainEvent
    {
        public ANode Node { get; private set; } = null;
        public OnNodeDragBegin(ANode node) => Node = node;
    }

    class OnNodeDragEnd : IDomainEvent
    {
        public ANode Node { get; private set; } = null;
        public OnNodeDragEnd(ANode node) => Node = node;
    }

    class OnNormalNodeEliminate : IDomainEvent
    {
        public int Atk { get; private set; } = 0;
        public OnNormalNodeEliminate(int atk) => Atk = atk;

    }

    class OnChargeNodeEliminate : IDomainEvent
    {
        public int Atk { get; private set; }
        public int Num { get; private set; }
        public OnChargeNodeEliminate(int atk, int num)
        {
            Atk = atk;
            Num = num;
        }
    }

    class OnEnergyNodeEliminate : IDomainEvent
    {
        public float Time { get; private set; }
        public OnEnergyNodeEliminate(float time) => Time = time;
    }

    class OnDefenseNodeEliminate : IDomainEvent
    {
        public int Def { get; private set; }
        public OnDefenseNodeEliminate(int def) => Def = def;
    }

    class OnChestNodeEliminate : IDomainEvent
    {
        public int Atk { get; private set; }
        public int Def { get; private set; }
        public float Energy { get; private set; }
        public int HPRecover { get; private set; }
        public int ChargeNodeReduce { get; private set; }
        public OnChestNodeEliminate(int atk, int def, float energy, int hpRecover, int chargeNodeReduce)
        {
            Atk = atk;
            Def = def;
            Energy = energy;
            HPRecover = hpRecover;
            ChargeNodeReduce = chargeNodeReduce;
        }
    }

    class OnGameStateChange : IDomainEvent
    {
        public GameState NewState { get; private set; }
        public OnGameStateChange(GameState newState) => NewState = newState;
    }

    class OnComboPlus : IDomainEvent
    {
        public OnComboPlus() { }
    }
    //FOR UI-------------------------------------------------------------------------------
    class OnAtkChanged : IDomainEvent
    {
        public int NewAtk { get; private set; } = 0;
        public OnAtkChanged(int newAtk) => NewAtk = newAtk;
    }

    class OnChargeAtkChanged : IDomainEvent
    {
        public int NewAtk { get; private set; } = 0;
        public OnChargeAtkChanged(int newAtk) => NewAtk = newAtk;
    }

    class OnDefChanged : IDomainEvent
    {
        public int NewDef { get; private set; } = 0;
        public OnDefChanged(int newDef) => NewDef = newDef;
    }

    class OnEnergyChanged : IDomainEvent
    {
        public float NewEnergy { get; private set; } = 0f;
        public OnEnergyChanged(float newEnergy) => NewEnergy = newEnergy;
    }

    class OnChargeCountChange : IDomainEvent
    {
        public int Current { get; private set; } = 0;
        public OnChargeCountChange(int current) => Current = current;
    }

    class OnPlayerHPChanged : IDomainEvent
    {
        public int NewHP { get; private set; } = 0;
        public OnPlayerHPChanged(int newHP) => NewHP = newHP;
    }


    class OnComboChange : IDomainEvent
    {
        public int Combos { get; private set; } = 0;
        public OnComboChange(int combos) => Combos = combos;
    }

    class OnPlayerStatsInit : IDomainEvent
    {
        public int MaxHP { get; private set; } = 0;
        public int MaxChargeNum { get; private set; } = 0;
        public OnPlayerStatsInit(int maxHP, int maxChargeNum)
        {
            MaxHP = maxHP;
            MaxChargeNum = maxChargeNum;
        }
    }


    class OnEnemyHPChanged : IDomainEvent
    {
        public int NewHP { get; private set; } = 0;
        public OnEnemyHPChanged(int newHP) => NewHP = newHP;
    }
}