//ATTEND: Can't Transfer to a new state at state init method
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TmUnity.Node;
using Eccentric.Utils;
using Eccentric;
using GS = TmUnity.GameState;
namespace TmUnity.Game
{
    class GameController : MonoBehaviour
    {
#if UNITY_EDITOR
        [ReadOnly] [SerializeField] GS state = default(GS);
#endif
        [SerializeField] PlayerAttr attrs = null;
        [SerializeField] Enemy enemy = null;
        [ReadOnly] [SerializeField] GameStats stats = null;
        [ReadOnly] [SerializeField] GameResultStats resultStats = null;
        NodeController nodeController = null;
        GameState currentState = null;
        float elapsedTime = 0f;
        bool isStateInit = false;
        bool isChargeReady = false;
        Dictionary<GS, GameState> statesDic = null;
        public float NextRoundDuration => stats.NextRoundDuration;
        public int TotalDamage => stats.CurrentAtk + stats.CurrentChargeAtk;
        public float ExtraRoundDuration { get; set; } = 0f;

        void Awake() => nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();

        void Start()
        {
            nodeController.InitBoard();
            nodeController.CalculateResultWithoutAnim();
            statesDic = new Dictionary<GS, GameState>(){
                {GS.START, new StartState(this)},
                {GS.END, new EndState(this)},
                {GS.WAIT, new WaitState(this)},
                {GS.ACTION, new ActionState(this)},
                {GS.ANIMATE, new AnimateState(this)},
                {GS.ENEMY, new EnemyState(enemy,this)}
            };
            NewState(GS.START);
        }

        void Update()
        {
            elapsedTime += Time.deltaTime;
            if (currentState != null && !isStateInit)
            {
                currentState.Init();
                isStateInit = true;
            }
            else if (isStateInit)
                currentState?.Tick();
        }

        void EndGame(bool isWin)
        {
            NewState(GS.END);
            resultStats.ElapsedTime = elapsedTime;
            DomainEvents.Raise<OnGameEnd>(new OnGameEnd(resultStats, isWin));
        }

        async public Task CalculateResultAsync() => await nodeController.CalculateResultAsync();

        public void NewState(GS nextState)
        {
            currentState?.End();
            isStateInit = false;
            currentState = statesDic[nextState];
            state = nextState;
            DomainEvents.Raise<OnGameStateChange>(new OnGameStateChange(nextState));
        }

        public void InitStats()
        {
            stats = new GameStats();
            resultStats = new GameResultStats();
            DomainEvents.Raise<OnPlayerStatsInit>(new OnPlayerStatsInit(attrs.HP, attrs.MaxChargeNum));
            isChargeReady = true;
            stats.CurrentHP = attrs.HP;
            elapsedTime = 0f;
            StartNewRound();
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
            ExtraRoundDuration = 0f;
        }

        public void CaculateNextRoundDuration()
        {
            stats.NextRoundDuration = attrs.BasicEnergy + ExtraRoundDuration;
            DomainEvents.Raise<OnMaxTimeSet>(new OnMaxTimeSet(stats.NextRoundDuration));
            DomainEvents.Raise<OnRemainTimeChanged>(new OnRemainTimeChanged(stats.NextRoundDuration));
        }

        public void UpdateMaxDamage()
        {
            if (TotalDamage > resultStats.MaxDamage)
                resultStats.MaxDamage = TotalDamage;
        }

        public void ForceEndDrag() => nodeController.ForceEndDrag();

        void HandleNodeEliminate(OnNodeEliminate e)
        {
            stats.CurrentAtk += e.Info.NormalAtk;
            stats.CurrentChargeAtk += e.Info.ChargeAtk;
            stats.CurrentChargeCount += e.Info.ChargeNum;
            ExtraRoundDuration += e.Info.EnergyTime;
            stats.CurrentDef += e.Info.Def;
            stats.CurrentHP += e.Info.HPRecover;
            stats.CurrentCombo += 1;
            if (stats.CurrentChargeCount >= attrs.MaxChargeNum && !isChargeReady)
            {
                isChargeReady = true;
                stats.CurrentChargeAtk += attrs.MaxChargeAtk;
            }
        }

        void HandlePlayerBeAttacked(OnPlayerBeAttacked e)
        {
            var damage = e.Atk - stats.CurrentDef;
            damage = damage <= 0 ? 0 : damage;
            stats.CurrentHP -= damage;
        }

        void HandlePlayerDead(OnPlayerDead e) => EndGame(false);

        void HandleEnemyDead(OnEnemyDead e) => EndGame(true);

        void OnEnable()
        {
            DomainEvents.Register<OnNodeEliminate>(HandleNodeEliminate);
            DomainEvents.Register<OnPlayerBeAttacked>(HandlePlayerBeAttacked);
            DomainEvents.Register<OnPlayerDead>(HandlePlayerDead);
            DomainEvents.Register<OnEnemyDead>(HandleEnemyDead);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnNodeEliminate>(HandleNodeEliminate);
            DomainEvents.UnRegister<OnPlayerBeAttacked>(HandlePlayerBeAttacked);
            DomainEvents.UnRegister<OnPlayerDead>(HandlePlayerDead);
            DomainEvents.UnRegister<OnEnemyDead>(HandleEnemyDead);
        }

    }

    class StartState : GameState
    {
        public StartState(GameController controller) : base(controller) { }

        public override void Init() => DomainEvents.Raise<OnGameStart>(new OnGameStart());

        public override void Tick()
        {
            if (Input.anyKeyDown)
                controller.NewState(GS.WAIT);
        }

        public override void End() => controller.InitStats();

    }

    class EndState : GameState
    {
        public EndState(GameController controller) : base(controller) { }

        public override void Init() { }
        public override void Tick()
        {
            if (Input.anyKeyDown)
                SceneManager.LoadScene(0);
        }

        public override void End() { }
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
                controller.ForceEndDrag();
                controller.NewState(GS.ANIMATE);
            }
        }

        public override void End() { }

        void HandleNodeDragEnd(OnNodeDragEnd e) => controller.NewState(GS.ANIMATE);
    }

    class AnimateState : GameState
    {
        bool isFin = false;
        public AnimateState(GameController controller) : base(controller) { }

        async public  override void Init()
        {
            isFin = false;
            await controller.CalculateResultAsync();
            controller.UpdateMaxDamage();
            //NOTE: if enemy dead here it will play dead animation and game controller will receive the message and will enter END state
            DomainEvents.Raise<OnEnemyBeAttacked>(new OnEnemyBeAttacked(controller.TotalDamage));
            isFin = true;
        }

        public override void Tick()
        {
            if (isFin)
                controller.NewState(GS.ENEMY);
        }

        public override void End() { }
    }

    class EnemyState : GameState
    {
        Enemy enemy = null;

        public EnemyState(Enemy enemy, GameController controller) : base(controller) => this.enemy = enemy;

        public override void Init()
        {
            DomainEvents.Register<OnEnemyAtkAnimFin>(HandleEnemyAtkAnimFin);
            //NOTE: enemy will play attack animation and will raise OnPlayerBeAttacked if player hp gets zero hp stats will raise OnPlayerDead
            // Controller will receive message and enter END state
            enemy.Attack();
        }

        public override void Tick() { }

        public override void End() => DomainEvents.UnRegister<OnEnemyAtkAnimFin>(HandleEnemyAtkAnimFin);

        void HandleEnemyAtkAnimFin(OnEnemyAtkAnimFin e)
        {
            var extraTime = enemy.GetNextAttack();
            controller.ExtraRoundDuration += extraTime;
            controller.NewState(GS.WAIT);
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