using UnityEngine;
using Eccentric;
namespace TmUnity.Node
{
    class ChargeNode : ANode
    {
        public int BasicAtk { get; private set; } = 0;
        public void Init(int basicAtk, Vector2Int point, NodeType type, NodeController controller)
        {
            base.Init(point, type, controller);
            BasicAtk = basicAtk;
        }

        public override void Eliminate(bool isFXPlay = true)
        {
            if (isFXPlay)
            {
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(GetCenterPos(), VFXType.ELIMINATE));
                DomainEvents.Raise<OnPlayerAtkAnim>(new OnPlayerAtkAnim(GetCenterPos(), NodeType.CHARGE));
            }
            base.Eliminate();
        }
    }
}