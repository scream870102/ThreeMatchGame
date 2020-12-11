using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Eccentric.Utils;
using Eccentric;
using System.Linq;
namespace TmUnity
{
    class Enemy : MonoBehaviour
    {
        [SerializeField] int maxHP = 0;
        [ReadOnly] [SerializeField] int currentHP = 0;
        [SerializeField] List<AAttack> attacks = new List<AAttack>();
        AAttack currentAttack = null;

        void Start()
        {
            currentHP = maxHP;
            DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(currentHP, maxHP));
        }

        void OnEnable() => DomainEvents.Register<OnEnemyBeAttacked>(HandleEnemyBeAttacked);

        void OnDisable() => DomainEvents.UnRegister<OnEnemyBeAttacked>(HandleEnemyBeAttacked);

        void HandleEnemyBeAttacked(OnEnemyBeAttacked e)
        {
            currentHP -= e.Atk;
            if (currentHP <= 0)
            {
                DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(0, maxHP));
                DomainEvents.Raise<OnEnemyDead>(new OnEnemyDead());
                return;
            }
            DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(currentHP, maxHP));
        }

        public async Task AttackAsync()
        {
            if (currentAttack == null)
            {
                Debug.Log("No Attack Find");
                GetNextAttack();
                Debug.Log("HAHAHAHAHAHAHAHAHAHAHA");
            }
            Debug.Log($"{currentAttack.name} is attack");
            await currentAttack.AttackAsync();
            attacks.ForEach(a => a.RoundPass());

        }

        public float GetNextAttack()
        {
            attacks.ForEach(action => Debug.Log($"{action.name} {action.IsReady}"));
            var readyAttacks = attacks.FindAll(a => a.IsReady);
            currentAttack = readyAttacks[Random.Range(0, readyAttacks.Count)];
            Debug.Log($"Find {currentAttack.name}");
            return currentAttack.RoundDuration;
        }
#if UNITY_EDITOR
        void Update() => attacks.ForEach(a => a.bReady = a.IsReady);
#endif

    }

    [System.Serializable]
    class AAttack
    {
        [SerializeField] AttackAttr attrs;
#if UNITY_EDITOR
        public bool bReady;
        public string name;
#endif
        protected int remainCD = 0;
        public bool IsReady => remainCD <= 0;
        public float RoundDuration => attrs.Time;

        public void RoundPass() => remainCD -= 1;
        public async virtual Task AttackAsync()
        {
            remainCD = attrs.CD + 1;
            DomainEvents.Raise<OnPlayerBeAttacked>(new OnPlayerBeAttacked(attrs.Atk));
        }
    }
}
