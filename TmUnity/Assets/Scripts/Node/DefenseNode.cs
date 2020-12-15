using UnityEngine;
using Eccentric;
using System.Threading.Tasks;
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
        public override void Eliminate(bool isFXPlay = true)
        {
            if (isFXPlay)
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(RectTransform.position - (Vector3)aspectOffset, VFXType.HEAL));
            base.Eliminate();
        }
    }
}
