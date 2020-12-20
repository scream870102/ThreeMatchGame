using UnityEngine.EventSystems;
using UnityEngine;
using Eccentric;
using UnityEngine.UI;

namespace TmUnity.Skill
{
    class Heal : ASkill
    {
        [SerializeField] int healAmount = 0;
        protected override void UseSkill()
        {
            controller.Heal(healAmount);
            DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(transform.position, VFXType.HEAL));
        }

        protected override void SetExpositionText(Text expositionText, string skillName, int manaCost, string exposition) => expositionText.text = $"{skillName} ({manaCost})\n Heal {healAmount} instantly.";
    }

}
