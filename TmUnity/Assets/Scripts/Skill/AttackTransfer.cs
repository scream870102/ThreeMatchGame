using UnityEngine.EventSystems;
using UnityEngine;
using TmUnity.Node;
namespace TmUnity.Skill
{
    class AttackTransfer : ASkill
    {
        protected override void UseSkill() => controller.TransferNodeType(NodeType.DEFENSE, NodeType.ATTACK);
    }

}
