//ATTEND: Can't Transfer to a new state at state init method
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TmUnity.Node;
using TmUnity.Skill;
using Eccentric.Utils;
using Eccentric;
using GS = TmUnity.GameState;
namespace TmUnity
{
    class GameController : MonoBehaviour
    {
#if UNITY_EDITOR
        [ReadOnly] [SerializeField] GS state = default(GS);
#endif
        [SerializeField] PlayerAttr attrs = null;
        [SerializeField] Enemy enemy = null;
        [SerializeField] List<ASkill> skills = null;
        [ReadOnly] [SerializeField] GameStats stats = null;
        [ReadOnly] [SerializeField] GameResultStats resultStats = null;
        NodeController nodeController = null;
        AGameState currentState = null;
        float elapsedTime = 0f;
        bool isStateInit = false;
        Dictionary<GS, AGameState> statesDic = null;
        public float NextRoundDuration => stats.NextRoundDuration;
        public int CurrentAtk => stats.CurrentAtk;
        public float ExtraRoundDuration { get; set; } = 0f;
        public bool IsEnemyStop { get; set; } = false;

        void Awake() => nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();

        void Start()
        {
            nodeController.InitBoard();
            nodeController.CalculateResultWithoutAnim();
            statesDic = new Dictionary<GS, AGameState>(){
                {GS.START, new StartState(this)},
                {GS.END, new EndState(this)},
                {GS.WAIT, new WaitState(this)},
                {GS.ACTION, new ActionState(this)},
                {GS.ANIMATE, new AnimateState(this)},
                {GS.ENEMY, new EnemyState(enemy,this)}
            };
            skills.ForEach(skill => skill.Init(this));
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
            DomainEvents.Raise<OnPlayerStatsInit>(new OnPlayerStatsInit(attrs.HP, attrs.BasicMana));
            stats.CurrentHP = attrs.HP;
            stats.CurrentMana = attrs.BasicMana;
            elapsedTime = 0f;
            StartNewRound();
            var attackAttr = enemy.InitAttack();
            ExtraRoundDuration = attackAttr.Time;
            CaculateNextRoundDuration();
        }

        public void StartNewRound()
        {
            stats.CurrentAtk = attrs.BasicAtk;
            stats.CurrentDef = attrs.BasicDef;
            stats.CurrentCombo = 0;
            //NOTE: This is for active the mana chage event
            stats.CurrentMana = stats.CurrentMana;
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
            if (CurrentAtk > resultStats.MaxDamage)
                resultStats.MaxDamage = CurrentAtk;
        }

        public void BeAttacked(int damage)
        {
            var newDamage = damage - stats.CurrentDef;
            newDamage = newDamage <= 0 ? 0 : newDamage;
            stats.CurrentHP -= newDamage;
        }

        public void UseSkill(string skillName, int manaCost)
        {
            stats.CurrentMana -= manaCost;
        }

        public void Heal(int healAmount) => stats.CurrentHP += healAmount;

        public void ForceEndDrag() => nodeController.ForceEndDrag();

        public void CheckAllResult() => nodeController.CheckAllResult();

        public void TransferNodeType(NodeType from, NodeType to) => nodeController.TransferNodeType(from, to);

        void HandleNodeEliminate(OnNodeEliminate e)
        {
            stats.CurrentAtk += e.Info.Atk;
            stats.CurrentMana += e.Info.Mana;
            ExtraRoundDuration += e.Info.EnergyTime;
            stats.CurrentDef += e.Info.Def;
            stats.CurrentHP += e.Info.HPRecover;
            stats.CurrentCombo += 1;
        }

        void HandlePlayerDead(OnPlayerDead e) => EndGame(false);

        void HandleEnemyDead(OnEnemyDead e) => EndGame(true);

        void OnEnable()
        {
            DomainEvents.Register<OnNodeEliminate>(HandleNodeEliminate);
            DomainEvents.Register<OnPlayerDead>(HandlePlayerDead);
            DomainEvents.Register<OnEnemyDead>(HandleEnemyDead);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnNodeEliminate>(HandleNodeEliminate);
            DomainEvents.UnRegister<OnPlayerDead>(HandlePlayerDead);
            DomainEvents.UnRegister<OnEnemyDead>(HandleEnemyDead);
        }

    }

    class StartState : AGameState
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

    class EndState : AGameState
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

    class WaitState : AGameState
    {
        public WaitState(GameController controller) : base(controller) => DomainEvents.Register<OnNodeDragBegin>(HandleNodeDragBegin);

        ~WaitState() => DomainEvents.UnRegister<OnNodeDragBegin>(HandleNodeDragBegin);

        public override void Init()
        {
            controller.CaculateNextRoundDuration();
            controller.StartNewRound();
        }

        public override void Tick() { }

        public override void End() { }

        void HandleNodeDragBegin(OnNodeDragBegin e) => controller.NewState(GS.ACTION);

    }

    class ActionState : AGameState
    {
        ScaledTimer timer = null;

        public ActionState(GameController controller) : base(controller)
        {
            timer = new ScaledTimer();
            DomainEvents.Register<OnNodeDragEnd>(HandleNodeDragEnd);
        }

        ~ActionState() => DomainEvents.UnRegister<OnNodeDragEnd>(HandleNodeDragEnd);

        public override void Init() => timer.Reset(controller.NextRoundDuration);

        public override void Tick()
        {
            DomainEvents.Raise<OnRemainTimeChanged>(new OnRemainTimeChanged(timer.Remain));
            controller.CheckAllResult();
            if (timer.IsFinished)
            {
                controller.ForceEndDrag();
                controller.NewState(GS.ANIMATE);
            }
        }

        public override void End() { }

        void HandleNodeDragEnd(OnNodeDragEnd e) => controller.NewState(GS.ANIMATE);
    }

    class AnimateState : AGameState
    {
        bool isFin = false;
        public AnimateState(GameController controller) : base(controller) { }

        async public override void Init()
        {
            isFin = false;
            controller.CheckAllResult();
            await controller.CalculateResultAsync();
            controller.UpdateMaxDamage();
            //NOTE: if enemy dead here it will play dead animation and game controller will receive the message and will enter END state
            DomainEvents.Raise<OnEnemyBeAttacked>(new OnEnemyBeAttacked(controller.CurrentAtk));
            isFin = true;
        }

        public override void Tick()
        {
            if (isFin)
                controller.NewState(GS.ENEMY);
        }

        public override void End() { }
    }

    class EnemyState : AGameState
    {
        Enemy enemy = null;
        float extraTime = 0f;
        int atk = 0;
        public EnemyState(Enemy enemy, GameController controller) : base(controller) => this.enemy = enemy;

        public override void Init()
        {
            DomainEvents.Register<OnEnemyGetNewAttack>(HandleEnemyGetNewAttack);
            DomainEvents.Register<OnDefAnimFin>(HandleDefAnimFin);
            DomainEvents.Register<OnPlayerBeAttacked>(HandlePlayerBeAttacked);
            //NOTE: enemy will play attack animation and will raise OnPlayerBeAttacked if player hp gets zero hp stats will raise OnPlayerDead
            // Controller will receive message and enter END state
            if (!controller.IsEnemyStop)
                enemy.Attack();

        }

        public override void Tick()
        {
            //NOTE: Should Invoke NewState in Init  otherwise it won't call end function 
            if (controller.IsEnemyStop)
            {
                controller.IsEnemyStop = false;
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(Vector3.zero, VFXType.STARE));
                controller.ExtraRoundDuration += extraTime;
                controller.NewState(GS.WAIT);
            }
        }

        public override void End()
        {
            DomainEvents.UnRegister<OnEnemyGetNewAttack>(HandleEnemyGetNewAttack);
            DomainEvents.UnRegister<OnDefAnimFin>(HandleDefAnimFin);
            DomainEvents.UnRegister<OnPlayerBeAttacked>(HandlePlayerBeAttacked);
        }

        void HandleEnemyGetNewAttack(OnEnemyGetNewAttack e) => extraTime = e.Attr.Time;

        void HandleDefAnimFin(OnDefAnimFin e)
        {
            controller.ExtraRoundDuration += extraTime;
            controller.BeAttacked(atk);
            controller.NewState(GS.WAIT);
        }

        void HandlePlayerBeAttacked(OnPlayerBeAttacked e) => atk = e.Atk;
    }

    abstract class AGameState : IState
    {
        protected GameController controller { get; private set; } = null;
        public AGameState(GameController controller) => this.controller = controller;
        public abstract void Init();
        public abstract void Tick();
        public abstract void End();
    }
}