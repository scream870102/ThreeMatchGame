using Eccentric;
using UnityEngine;
using TmUnity.Node;
namespace TmUnity.Node
{
    ///<summary>Raise event when a node drag start</summary>
    class OnNodeDragBegin : IDomainEvent
    {
        public ANode Node { get; private set; } = null;
        public OnNodeDragBegin(ANode node) => Node = node;
    }

    ///<summary>Raise event when a node drag end</summary>
    class OnNodeDragEnd : IDomainEvent
    {
        public ANode Node { get; private set; } = null;
        public OnNodeDragEnd(ANode node) => Node = node;
    }

    ///<summary>Raise event when Any Node Eliminate Also Pass EliminateInfo</sumarry>
    class OnNodeEliminate : IDomainEvent
    {
        public EliminateInfo Info { get; private set; } = null;
        public OnNodeEliminate(EliminateInfo info) => Info = info;
    }

    #region UI_ONLY

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
        public OnComboChange(int combos) => Combos = combos;
    }

    #endregion
}


namespace TmUnity
{
    ///<summary>Raise event when Game StateMachine start a new state</summary>
    class OnGameStateChange : IDomainEvent
    {
        public GameState NewState { get; private set; }
        public OnGameStateChange(GameState newState) => NewState = newState;
    }

    ///<summary>Raise event when Visual Effect should be play</summary>
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

    ///<summary>Raise event When Application Start</summary>
    class OnGameStart : IDomainEvent { }

    ///<summary>Raise event When Game Get result player or enemy Dead</summary>
    class OnGameEnd : IDomainEvent
    {
        public GameResultStats Result { get; private set; } = null;
        public bool IsWin { get; private set; } = false;
        public OnGameEnd(GameResultStats result, bool isWin)
        {
            Result = result;
            IsWin = isWin;
        }
    }

    #region PLAYER

    ///<summary>Raise event when player hp reaches zero</summary>
    class OnPlayerDead : IDomainEvent { }

    ///<summary>Raise event when player being attacked</summary>
    class OnPlayerBeAttacked : IDomainEvent
    {
        public int Atk { get; private set; } = 0;
        public OnPlayerBeAttacked(int atk) => Atk = atk;
    }

    ///<summary>Raise event when player use charge attack or normal attack</summary>
    class OnPlayerVFXPlay : IDomainEvent
    {
        public Vector3 StartPos { get; private set; } = default(Vector3);
        public NodeType Type { get; private set; } = default(NodeType);
        public OnPlayerVFXPlay(Vector3 startPos, NodeType type)
        {
            StartPos = startPos;
            Type = type;
        }
    }

    #endregion

    #region ENEMY

    ///<summary>Raise event when enemy be attacked</summary>
    class OnEnemyBeAttacked : IDomainEvent
    {
        public int Atk { get; private set; } = 0;
        public OnEnemyBeAttacked(int atk) => Atk = atk;
    }

    ///<summary>Raise event when Enemy hp reaches zero</summary>
    class OnEnemyDead : IDomainEvent { }

    ///<summary>When enemy attack animation end. A animation event should be invoke,In that method this event should be raise</summary>
    class OnEnemyAtkAnimFin : IDomainEvent
    {
        public AttackAttr Attr { get; private set; } = null;
        public OnEnemyAtkAnimFin(AttackAttr attr) => Attr = attr;
    }

    #endregion

    #region UI_ONLY

    ///<summary>Raise event when player hp changed</summary>
    class OnPlayerHPChanged : IDomainEvent
    {
        public int NewHP { get; private set; } = 0;
        public OnPlayerHPChanged(int newHP) => NewHP = newHP;
    }

    ///<summary>Raise event when Player maxHP and maxChargeNum changed</summary>
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

    ///<summary>Raise event when round remain time changed</summary>
    class OnRemainTimeChanged : IDomainEvent
    {
        public float Remain { get; private set; } = 0f;
        public OnRemainTimeChanged(float remain) => Remain = remain;
    }

    ///<summary>Handle event when round max time changed</summary>
    class OnMaxTimeSet : IDomainEvent
    {
        public float MaxTime { get; private set; } = 0f;
        public OnMaxTimeSet(float maxTime) => MaxTime = maxTime;
    }

    ///<summary>Raise event when enemy hp changed</summary>
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

    #endregion


}