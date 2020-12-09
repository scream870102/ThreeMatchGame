using UnityEngine;

namespace TmUnity.Node
{
    class ChestNode : ANode
    {
        public ChestType ChestType { get; private set; } = default(ChestType);
        public void Init(ChestType chestType, Vector2Int point, NodeType type, NodeController controller)
        {
            base.Init(point, type, controller);
            ChestType = chestType;
        }
    }
}
