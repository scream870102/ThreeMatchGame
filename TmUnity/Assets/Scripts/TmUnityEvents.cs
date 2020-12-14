using Eccentric;
using UnityEngine;
using TmUnity.Node;
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

    class OnNodeEliminate : IDomainEvent
    {
        public EliminateInfo Info { get; private set; } = null;
        public OnNodeEliminate(EliminateInfo info) => Info = info;
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

    class OnComboChange : IDomainEvent
    {
        public int Combos { get; private set; } = 0;
        public bool IsZeroDisplay { get; private set; } = false;
        public OnComboChange(int combos, bool isZeroDisplay = false)
        {
            Combos = combos;
            IsZeroDisplay = isZeroDisplay;
        }
    }

    class OnForceEndDrag : IDomainEvent
    {
        public OnForceEndDrag() { }
    }


}


namespace TmUnity
{
    class OnGameStateChange : IDomainEvent
    {
        public GameState NewState { get; private set; }
        public OnGameStateChange(GameState newState) => NewState = newState;
    }

    class OnPlayerDead : IDomainEvent
    {
        public OnPlayerDead() { }
    }

    class OnPlayerBeAttacked : IDomainEvent
    {
        public int Atk { get; private set; } = 0;
        public OnPlayerBeAttacked(int atk) => Atk = atk;
    }

    class OnPlayerHPChanged : IDomainEvent
    {
        public int NewHP { get; private set; } = 0;
        public OnPlayerHPChanged(int newHP) => NewHP = newHP;
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

    class OnRemainTimeChanged : IDomainEvent
    {
        public float Remain { get; private set; } = 0f;
        public OnRemainTimeChanged(float remain) => Remain = remain;
    }

    class OnMaxTimeSet : IDomainEvent
    {
        public float MaxTime { get; private set; } = 0f;
        public OnMaxTimeSet(float maxTime) => MaxTime = maxTime;
    }

    class OnEnemyHPChanged : IDomainEvent
    {
        public int NewHP { get; private set; } = 0;
        public int MaxHP { get; private set; } = 0;
        public OnEnemyHPChanged(int newHP, int maxHP)
        {
            NewHP = newHP;
            MaxHP = maxHP;
        }
    }

    class OnEnemyBeAttacked : IDomainEvent
    {
        public int Atk { get; private set; } = 0;
        public OnEnemyBeAttacked(int atk) => Atk = atk;
    }

    class OnEnemyDead : IDomainEvent
    {
        public OnEnemyDead() { }
    }

    class OnVFXPlay : IDomainEvent
    {
        public Vector3 Pos { get; private set; } = default(Vector3);
        public VFXType Type { get; private set; } = default(VFXType);
        public OnVFXPlay(Vector3 pos, VFXType type)
        {
            Pos = pos;
            Type = type;
        }
    }

    class OnPlayerAtkAnim : IDomainEvent
    {
        public Vector3 StartPos { get; private set; } = default(Vector3);
        public Vector3 EndPos { get; private set; } = default(Vector3);
        public NodeType Type { get; private set; } = default(NodeType);
        public float Vel { get; private set; } = 0f;
        public OnPlayerAtkAnim(Vector3 startPos, Vector3 endPos, NodeType type, float vel)
        {
            StartPos = startPos;
            EndPos = endPos;
            Type = type;
            Vel = vel;
        }
    }

}