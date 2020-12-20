using UnityEngine.EventSystems;
using UnityEngine;
using TmUnity.Node;
namespace TmUnity.Skill
{
    class DefenseTransfer : ASkill
    {
        protected override void UseSkill() => controller.TransferNodeType(NodeType.ATTACK, NodeType.DEFENSE);
    }

}
