using UnityEngine;
using Eccentric;
namespace TmUnity.Node
{
    class AttackNode : ANode
    {
        public int Atk { get; private set; } = 0;
        public void Init(int atk, Vector2Int point, NodeType type, NodeController controller)
        {
            base.Init(point, type, controller);
            Atk = atk;
        }

        public override void Eliminate(bool isFXPlay = true)
        {
            if (isFXPlay)
            {
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(GetCenterPos(), VFXType.ELIMINATE));
                DomainEvents.Raise<OnPlayerVFXPlay>(new OnPlayerVFXPlay(GetCenterPos(), NodeType.ATTACK));
            }
            base.Eliminate();
        }

    }
}