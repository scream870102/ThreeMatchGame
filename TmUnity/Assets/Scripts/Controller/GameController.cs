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
        [ReadOnly] [SerializeField] NodeController nodeController = null;
        [SerializeField] PlayerAttr attrs = null;
        [SerializeField] Enemy enemy = null;
        [ReadOnly] [SerializeField] GameStats stats = null;
#if UNITY_EDITOR
        [ReadOnly] [SerializeField] GS state = default(GS);
#endif
        WaitState waitState = null;
        ActionState actionState = null;
        AnimateState animateState = null;
        EnemyState enemyState = null;
        GameState currentState = null;
        bool isStateInit = false;
        bool isChargeReady = false;
        public float NextRoundDuration => stats.NextRoundDuration;
        public int TotalDamage => stats.CurrentAtk + stats.CurrentChargeAtk;
        public float ExtraRoundDuration = 0f;
        void Awake() => nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();

        void Start()
        {
            nodeController.InitBoard();
            nodeController.CalculateResult();
            InitStats();
            waitState = new WaitState(this);
            actionState = new ActionState(this);
            animateState = new AnimateState(this);
            enemyState = new EnemyState(enemy, this);
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
                    currentState = enemyState;
                    state = GS.ENEMY;
                    DomainEvents.Raise(new OnGameStateChange(GS.ENEMY));
                    break;
            }
        }

        void OnEnable()
        {
            DomainEvents.Register<OnComboPlus>(HandleComboPlus);
            DomainEvents.Register<OnNodeEliminate>(HandleNodeEliminate);
            DomainEvents.Register<OnPlayerBeAttacked>(HandlePlayerBeAttacked);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnComboPlus>(HandleComboPlus);
            DomainEvents.UnRegister<OnNodeEliminate>(HandleNodeEliminate);
            DomainEvents.UnRegister<OnPlayerBeAttacked>(HandlePlayerBeAttacked);
        }

        void InitStats()
        {
            stats = new GameStats();
            DomainEvents.Raise<OnPlayerStatsInit>(new OnPlayerStatsInit(attrs.HP, attrs.MaxChargeNum));
            StartNewRound();
            stats.CurrentChargeAtk = attrs.BasicChargeAtk;
            stats.CurrentChargeCount = 0;
            stats.CurrentHP = attrs.HP;
            isChargeReady = false;
            ExtraRoundDuration = 0f;
        }


        void HandleComboPlus(OnComboPlus e) => stats.CurrentCombo += 1;

        void HandleNodeEliminate(OnNodeEliminate e)
        {
            stats.CurrentAtk += e.Info.NormalAtk;
            stats.CurrentChargeAtk += e.Info.ChargeAtk;
            stats.CurrentChargeCount += e.Info.ChargeNum;
            ExtraRoundDuration += e.Info.EnergyTime;
            //stats.NextRoundDuration += e.Info.EnergyTime;
            stats.CurrentDef += e.Info.Def;
            stats.CurrentHP += e.Info.HPRecover;
            if (stats.CurrentChargeCount > attrs.MaxChargeNum)
            {
                isChargeReady = true;
                stats.CurrentChargeAtk += attrs.MaxChargeAtk;
            }
        }

        void HandlePlayerBeAttacked(OnPlayerBeAttacked e)
        {
            Debug.Log($"Atk:{e.Atk} Def:{stats.CurrentDef} HP:{stats.CurrentHP}");
            var damage = e.Atk - stats.CurrentDef;
            damage = damage <= 0 ? 0 : damage;
            stats.CurrentHP -= damage;
        }

        public void StartNewRound()
        {
            if (isChargeReady)
            {
                isChargeReady = false;
                stats.CurrentChargeAtk = attrs.BasicChargeAtk;
                stats.CurrentChargeCount = 0;
            }
            stats.CurrentAtk = attrs.BasicNormalAtk;
            stats.CurrentDef = attrs.BasicDef;
            stats.CurrentCombo = 0;
            DomainEvents.Raise<OnComboChange>(new OnComboChange(stats.CurrentCombo, true));
        }

        public void CaculateNextRoundDuration()
        {
            stats.NextRoundDuration = attrs.BasicEnergy + ExtraRoundDuration;
            ExtraRoundDuration = 0f;
            DomainEvents.Raise<OnMaxTimeSet>(new OnMaxTimeSet(stats.NextRoundDuration));
            DomainEvents.Raise<OnRemainTimeChanged>(new OnRemainTimeChanged(stats.NextRoundDuration));
        }

    }


    class WaitState : GameState
    {
        public WaitState(GameController controller) : base(controller) => DomainEvents.Register<OnNodeDragBegin>(HandleNodeDragBegin);
        ~WaitState() => DomainEvents.UnRegister<OnNodeDragBegin>(HandleNodeDragBegin);
        public override void Init() => controller.CaculateNextRoundDuration();

        public override void Tick() { }

        public override void End() { }
        void HandleNodeDragBegin(OnNodeDragBegin e) => controller.NewState(GS.ACTION);

    }

    class ActionState : GameState
    {
        ScaledTimer timer = null;
        public ActionState(GameController controller) : base(controller)
        {
            timer = new ScaledTimer();
            DomainEvents.Register<OnNodeDragEnd>(HandleNodeDragEnd);
        }
        ~ActionState() => DomainEvents.UnRegister<OnNodeDragEnd>(HandleNodeDragEnd);
        public override void Init()
        {
            controller.StartNewRound();
            timer.Reset(controller.NextRoundDuration);
        }

        public override void Tick()
        {
            DomainEvents.Raise<OnRemainTimeChanged>(new OnRemainTimeChanged(timer.Remain));
            if (timer.IsFinished)
            {
                DomainEvents.Raise<OnForceEndDrag>(new OnForceEndDrag());
                controller.NewState(GS.ANIMATE);
            }
        }

        public override void End() { }
        void HandleNodeDragEnd(OnNodeDragEnd e) => controller.NewState(GS.ANIMATE);
    }

    class AnimateState : GameState
    {
        public AnimateState(GameController controller) : base(controller) { }
        public async override void Init()
        {
            await controller.CalculateResultAsync();
            DomainEvents.Raise<OnEnemyBeAttacked>(new OnEnemyBeAttacked(controller.TotalDamage));
            controller.NewState(GS.ENEMY);
        }

        public override void Tick() { }

        public override void End() { }
    }

    class EnemyState : GameState
    {
        Enemy enemy = null;
        public EnemyState(Enemy enemy, GameController controller) : base(controller) => this.enemy = enemy;
        public override void Init() { }

        async public override void Tick()
        {
            await enemy.AttackAsync();
            var extraTime = enemy.GetNextAttack();
            controller.ExtraRoundDuration += extraTime;
            controller.NewState(GS.WAIT);
            // if (Input.GetKeyDown(KeyCode.C))
            //     controller.NewState(GS.WAIT);
        }

        public override void End()
        {

        }
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