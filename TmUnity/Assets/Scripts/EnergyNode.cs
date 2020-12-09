using UnityEngine;

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
    }
}