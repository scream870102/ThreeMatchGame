using UnityEngine;
using Eccentric;
namespace TmUnity.Skill
{
    class Stare : ASkill
    {
        protected override void UseSkill()
        {
            DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(Vector3.zero, VFXType.STARE));
            controller.IsEnemyStop = true;
        }
    }

}
