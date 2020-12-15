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
        [ReadOnly] [SerializeField] int currentHP = 0;
        [SerializeField] List<Attack> attacks = new List<Attack>();
        [SerializeField] int maxHP = 0;
        Attack currentAttack = null;
        Animator anim = null;

        void Awake() => anim = GetComponent<Animator>();
        void Start()
        {
            currentHP = maxHP;
            DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(currentHP, maxHP));
            anim.SetBool("IsDead", false);
            anim.SetTrigger("Spawn");
            attacks.ForEach(a => a.Init(anim));
        }

        void OnEnable() => DomainEvents.Register<OnEnemyBeAttacked>(HandleEnemyBeAttacked);

        void OnDisable() => DomainEvents.UnRegister<OnEnemyBeAttacked>(HandleEnemyBeAttacked);

        void HandleEnemyBeAttacked(OnEnemyBeAttacked e)
        {
            anim.SetTrigger("Damage");
            currentHP -= e.Atk;
            if (currentHP <= 0)
            {
                DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(0, maxHP));
                DomainEvents.Raise<OnEnemyDead>(new OnEnemyDead());
                anim.SetBool("IsDead", true);
                return;
            }
            DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(currentHP, maxHP));
        }

        public void Attack()
        {
            if (currentAttack == null)
                GetNextAttack();
            currentAttack.PlayAttackAnim();


        }

        public float GetNextAttack()
        {
            var readyAttacks = attacks.FindAll(a => a.IsReady);
            currentAttack = readyAttacks[Random.Range(0, readyAttacks.Count)];
            return currentAttack.RoundDuration;
        }

        void AttackAnimFinAE()
        {
            currentAttack.AttackAnimFinAE();
            attacks.ForEach(a => a.RoundPass());
            DomainEvents.Raise<OnEnemyAtkAnimFin>(new OnEnemyAtkAnimFin());
        }

    }

    [System.Serializable]
    class Attack
    {
        [SerializeField] string animTrigger = "";
        [SerializeField] protected AttackAttr attrs = null;
        protected int remainCD = 0;
        protected Animator anim { get; private set; } = null;
        public bool IsReady => remainCD <= 0;
        public float RoundDuration => attrs.Time;
        public void RoundPass() => remainCD -= 1;
        public void Init(Animator anim) => this.anim = anim;
        public void PlayAttackAnim() => anim.SetTrigger(animTrigger);

        public void AttackAnimFinAE()
        {
            remainCD = attrs.CD + 1;
            DomainEvents.Raise<OnPlayerBeAttacked>(new OnPlayerBeAttacked(attrs.Atk));
        }

    }
}
