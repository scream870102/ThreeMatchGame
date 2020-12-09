using UnityEngine;

namespace TmUnity.Node
{
    class DefenseNode : ANode
    {
        public int Def { get; private set; } = 0;
        public void Init(int def, Vector2Int point, NodeType type, NodeController controller)
        {
            base.Init(point, type, controller);
            Def = def;
        }
    }
}
