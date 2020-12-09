using UnityEngine;

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
    }
}