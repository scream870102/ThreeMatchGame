using UnityEngine;
using Eccentric;
using System.Threading.Tasks;
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
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(RectTransform.position - (Vector3)aspectOffset, VFXType.ELIMINATE));
                DomainEvents.Raise<OnPlayerAtkAnim>(new OnPlayerAtkAnim(RectTransform.position - (Vector3)aspectOffset, NodeType.CHARGE));
            }
            base.Eliminate();
        }
    }
}