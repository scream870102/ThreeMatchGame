using UnityEngine;
using UnityEngine.EventSystems;
namespace TmUnity
{
    class BasicNode : ANode
    {
        Vector2 oldPos = default(Vector2);

        public override void OnDrag(PointerEventData e)
        {
            RectTransform.position = e.position + new Vector2(-HalfSize.x, HalfSize.y);
        }
        public override void OnBeginDrag(PointerEventData e)
        {
            oldPos = RectTransform.anchoredPosition;
        }

        public override void OnEndDrag(PointerEventData e)
        {
            CorrectPos();
        }
    }
}