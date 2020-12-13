using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Eccentric.Utils;
using Eccentric;
namespace TmUnity
{
    class Enemy : MonoBehaviour
    {
        [SerializeField] int maxHP = 0;
        [ReadOnly] [SerializeField] int currentHP = 0;
        [SerializeField] List<AAttack> attacks = new List<AAttack>();
        [SerializeField] JackBomb jackBomb = null;
        AAttack currentAttack = null;

        void Start()
        {
            currentHP = maxHP;
            DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(currentHP, maxHP));
            attacks.Add(jackBomb);
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
                GetNextAttack();
            await currentAttack.AttackAsync();
            attacks.ForEach(a => a.RoundPass());

        }

        public float GetNextAttack()
        {
            var readyAttacks = attacks.FindAll(a => a.IsReady);
            currentAttack = readyAttacks[Random.Range(0, readyAttacks.Count)];
            return currentAttack.RoundDuration;
        }

    }

    [System.Serializable]
    class AAttack
    {
        [SerializeField] protected AttackAttr attrs = null;
        protected int remainCD = 0;
        public bool IsReady => remainCD <= 0;
        public float RoundDuration => attrs.Time;

        public void RoundPass() => remainCD -= 1;
        public async virtual Task AttackAsync()
        {
            await Task.CompletedTask;
            remainCD = attrs.CD + 1;
            DomainEvents.Raise<OnPlayerBeAttacked>(new OnPlayerBeAttacked(attrs.Atk));
        }
    }

    [System.Serializable]
    class JackBomb : AAttack
    {
        public async override Task AttackAsync()
        {
            //TODO: Play vfx here
            await base.AttackAsync();
        }
    }
}
