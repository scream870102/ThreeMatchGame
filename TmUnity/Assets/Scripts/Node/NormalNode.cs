using UnityEngine;
using Eccentric;
using System.Threading.Tasks;
namespace TmUnity.Node
{
    class NormalNode : ANode
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
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(RectTransform.position - (Vector3)aspectOffset, VFXType.ELIMINATE));
                DomainEvents.Raise<OnPlayerAtkAnim>(new OnPlayerAtkAnim(RectTransform.position - (Vector3)aspectOffset, NodeType.NORMAL));
            }
            base.Eliminate();
        }

    }
}