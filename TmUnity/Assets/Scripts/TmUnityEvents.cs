using Eccentric;
namespace TmUnity.Node
{
    class OnNodeDragEnd : IDomainEvent
    {
        public ANode Node { get; private set; }
        public OnNodeDragEnd(ANode node) => Node = node;
    }
}