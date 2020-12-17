using System.Collections.Generic;
using UnityEngine;
using Eccentric.Utils;
using Eccentric;
using UnityEngine.UI;
namespace TmUnity
{
    class Enemy : MonoBehaviour
    {
        [ReadOnly] [SerializeField] int currentHP = 0;
        [SerializeField] List<Attack> attacks = new List<Attack>();
        [SerializeField] int maxHP = 0;
        public Text SkillText { get; private set; } = null;
        public Text SkillDamageText { get; private set; } = null;
        Attack currentAttack = null;
        Animator anim = null;

        void Awake()
        {
            anim = GetComponent<Animator>();
            SkillText = transform.Find("SkillText").GetComponent<Text>();
            SkillDamageText = transform.Find("SkillDamageText").GetComponent<Text>();
        }

        void Start()
        {
            currentHP = maxHP;
            DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(currentHP, maxHP));
            anim.SetBool("IsDead", false);
            anim.SetTrigger("Spawn");
            attacks.ForEach(a => a.Init(this, anim));
        }

        void OnEnable() => DomainEvents.Register<OnEnemyBeAttacked>(HandleEnemyBeAttacked);

        void OnDisable() => DomainEvents.UnRegister<OnEnemyBeAttacked>(HandleEnemyBeAttacked);

        void HandleEnemyBeAttacked(OnEnemyBeAttacked e)
        {
            currentHP -= e.Atk;
            if (currentHP <= 0)
            {
                DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(0, maxHP));
                anim.SetBool("IsDead", true);
                return;
            }
            anim.SetTrigger("Damage");
            DomainEvents.Raise<OnEnemyHPChanged>(new OnEnemyHPChanged(currentHP, maxHP));
        }

        public AttackAttr InitAttack() => GetNextAttack();

        public void Attack()
        {
            if (currentAttack == null)
                GetNextAttack();
            currentAttack.PlayAttackAnim();
        }

        AttackAttr GetNextAttack()
        {
            var readyAttacks = attacks.FindAll(a => a.IsReady);
            currentAttack = readyAttacks[Random.Range(0, readyAttacks.Count)];
            DomainEvents.Raise<OnEnemyAtkAnimFin>(new OnEnemyAtkAnimFin(currentAttack.Attr));
            return currentAttack.Attr;
        }

        void AttackAnimFinAE()
        {
            currentAttack.AttackAnimFin();
            attacks.ForEach(a => a.RoundPass());
            GetNextAttack();
        }

        void DeadAnimFinAE() => DomainEvents.Raise<OnEnemyDead>(new OnEnemyDead());

    }

    [System.Serializable]
    class Attack
    {
        [SerializeField] protected AttackAttr attr = null;
        int remainCD = 0;
        Animator anim = null;
        Enemy parent = null;

        public bool IsReady => remainCD <= 0;
        public AttackAttr Attr => attr;
        public void RoundPass() => remainCD -= 1;

        public void Init(Enemy parent, Animator anim)
        {
            this.parent = parent;
            this.anim = anim;
        }

        public void PlayAttackAnim()
        {
            parent.SkillText.text = attr.AnimTrigger;
            parent.SkillDamageText.text = attr.Atk.ToString();
            anim.SetTrigger(attr.AnimTrigger);
        }

        public void AttackAnimFin()
        {
            remainCD = attr.CD + 1;
            DomainEvents.Raise<OnPlayerBeAttacked>(new OnPlayerBeAttacked(attr.Atk));
        }

    }
}
