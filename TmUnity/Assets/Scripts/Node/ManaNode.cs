using UnityEngine;
using Eccentric;
namespace TmUnity.Node
{
    class ManaNode : ANode
    {
        public int Mana { get; private set; } = 0;
        public void Init(int mana, Vector2Int point, NodeType type, NodeController controller)
        {
            base.Init(point, type, controller);
            Mana = mana;
        }

        public override void Eliminate(bool isFXPlay = true)
        {
            if (isFXPlay)
            {
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(GetCenterPos(), VFXType.ELIMINATE));
                DomainEvents.Raise<OnPlayerVFXPlay>(new OnPlayerVFXPlay(GetCenterPos(), NodeType.MANA));
            }
            base.Eliminate();
        }
    }
}