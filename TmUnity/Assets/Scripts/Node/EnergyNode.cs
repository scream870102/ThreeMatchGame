using UnityEngine;
using Eccentric;
namespace TmUnity.Node
{
    class EnergyNode : ANode
    {
        public float TimePlus { get; private set; } = 0f;
        public void Init(float timePlus, Vector2Int point, NodeType type, NodeController controller)
        {
            base.Init(point, type, controller);
            TimePlus = timePlus;
        }
        public override void Eliminate(bool isFXPlay = true)
        {
            if (isFXPlay)
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(GetCenterPos(), VFXType.HEAL));
            base.Eliminate();
        }
    }
}