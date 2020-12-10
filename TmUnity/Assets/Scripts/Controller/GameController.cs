using UnityEngine;
using TmUnity.Node;
using Eccentric.Utils;
using Eccentric;
using GS = TmUnity.GameState;
using System.Threading.Tasks;
namespace TmUnity.Game
{
    class GameController : MonoBehaviour
    {
        [ReadOnly] [SerializeField] NodeController nodeController;
        [SerializeField] PlayerAttr attrs;
        [ReadOnly] [SerializeField] GameStats stats;
        [ReadOnly] [SerializeField] GS state = default(GS);
        WaitState waitState = null;
        ActionState actionState = null;
        AnimateState animateState = null;
        EnemyState enemyState = null;
        GameState currentState = null;
        bool isStateInit = false;
        void Awake() => nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();

        void Start()
        {
            nodeController.InitBoard();
            InitStats();
            waitState = new WaitState(this);
            actionState = new ActionState(this);
            animateState = new AnimateState(this);
            enemyState = new EnemyState(this);
            NewState(GS.WAIT);
        }

        void Update()
        {
            if (currentState != null && !isStateInit)
            {
                currentState.Init();
                isStateInit = true;
            }
            else if (currentState != null && isStateInit)
                currentState.Tick();
        }

        public async Task CalculateResultAsync() => await nodeController.CalculateResultAsync();

        public void NewState(GS nextState)
        {
            currentState?.End();
            isStateInit = false;
            switch (nextState)
            {
                case GS.WAIT:
                    currentState = waitState;
                    state = GS.WAIT;
                    DomainEvents.Raise(new OnGameStateChange(GS.WAIT));
                    break;
                case GS.ACTION:
                    currentState = actionState;
                    state = GS.ACTION;
                    DomainEvents.Raise(new OnGameStateChange(GS.ACTION));
                    break;
                case GS.ANIMATE:
                    currentState = animateState;
                    state = GS.ANIMATE;
                    DomainEvents.Raise(new OnGameStateChange(GS.ANIMATE));
                    break;
                case GS.ENEMY:
                    ResetStats();
                    currentState = enemyState;
                    state = GS.ENEMY;
                    DomainEvents.Raise(new OnGameStateChange(GS.ENEMY));
                    break;
            }
        }

        void OnEnable()
        {
            DomainEvents.Register<OnNormalNodeEliminate>(HandleNormalNodeEliminate);
            DomainEvents.Register<OnChargeNodeEliminate>(HandleChargeNodeEliminate);
            DomainEvents.Register<OnEnergyNodeEliminate>(HandleEnergyNodeEliminate);
            DomainEvents.Register<OnDefenseNodeEliminate>(HandleDefenseNodeEliminate);
            DomainEvents.Register<OnChestNodeEliminate>(HandleChestNodeEliminate);
            DomainEvents.Register<OnComboPlus>(HandleComboPlus);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnNormalNodeEliminate>(HandleNormalNodeEliminate);
            DomainEvents.UnRegister<OnChargeNodeEliminate>(HandleChargeNodeEliminate);
            DomainEvents.UnRegister<OnEnergyNodeEliminate>(HandleEnergyNodeEliminate);
            DomainEvents.UnRegister<OnDefenseNodeEliminate>(HandleDefenseNodeEliminate);
            DomainEvents.UnRegister<OnChestNodeEliminate>(HandleChestNodeEliminate);
            DomainEvents.UnRegister<OnComboPlus>(HandleComboPlus);
        }

        void InitStats()
        {
            DomainEvents.Raise<OnPlayerStatsInit>(new OnPlayerStatsInit(attrs.HP, attrs.MaxChargeNum));
            stats.CurrentChargeAtk = attrs.BasicChargeAtk;
            stats.CurrentChargeCount = 0;
            stats.CurrentHP = attrs.HP;
            stats.CurrentCombo = 0;
        }

        void ResetStats()
        {
            stats.CurrentCombo = 0;
        }


        void HandleNormalNodeEliminate(OnNormalNodeEliminate e)
        {
            stats.CurrentAtk = e.Atk + attrs.BasicNormalAtk;
        }

        void HandleChargeNodeEliminate(OnChargeNodeEliminate e)
        {
            stats.CurrentChargeAtk += e.Atk;
            stats.CurrentChargeCount += e.Num;
        }

        void HandleEnergyNodeEliminate(OnEnergyNodeEliminate e)
        {
            stats.NextRoundDuration = attrs.BasicEnergy + e.Time;
        }

        void HandleDefenseNodeEliminate(OnDefenseNodeEliminate e)
        {
            stats.CurrentDef = attrs.BasicDef + e.Def;
        }

        void HandleChestNodeEliminate(OnChestNodeEliminate e)
        {
            stats.CurrentAtk += e.Atk;
            stats.CurrentDef += e.Def;
            stats.NextRoundDuration += e.Energy;
            stats.CurrentHP += e.HPRecover;
            stats.CurrentChargeCount += e.ChargeNodeReduce;
        }

        void HandleComboPlus(OnComboPlus e) => stats.CurrentCombo += 1;

    }


    class WaitState : GameState
    {
        public WaitState(GameController controller) : base(controller) => DomainEvents.Register<OnNodeDragBegin>(HandleNodeDragBegin);
        ~WaitState() => DomainEvents.UnRegister<OnNodeDragBegin>(HandleNodeDragBegin);
        public override void Init() { }

        public override void Tick() { }

        public override void End() { }
        void HandleNodeDragBegin(OnNodeDragBegin e) => controller.NewState(GS.ACTION);

    }

    class ActionState : GameState
    {
        public ActionState(GameController controller) : base(controller) => DomainEvents.Register<OnNodeDragEnd>(HandleNodeDragEnd);
        ~ActionState() => DomainEvents.UnRegister<OnNodeDragEnd>(HandleNodeDragEnd);
        public override void Init() { }

        public override void Tick() { }

        public override void End() { }
        void HandleNodeDragEnd(OnNodeDragEnd e) => controller.NewState(GS.ANIMATE);
    }

    class AnimateState : GameState
    {
        public AnimateState(GameController controller) : base(controller) { }
        public async override void Init()
        {
            await controller.CalculateResultAsync();
            controller.NewState(GS.ENEMY);
        }

        public override void Tick() { }

        public override void End() { }
    }

    class EnemyState : GameState
    {
        public EnemyState(GameController controller) : base(controller) { }
        public override void Init() { }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.C))
                controller.NewState(GS.WAIT);
        }

        public override void End() { }
    }

    abstract class GameState : IState
    {
        protected GameController controller { get; private set; } = null;
        public GameState(GameController controller) => this.controller = controller;
        public abstract void Init();
        public abstract void Tick();
        public abstract void End();
    }
}