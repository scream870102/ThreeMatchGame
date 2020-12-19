//NOTE: CHECK DATA USE https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.PointerEventData.html
//NOTE: IDragHandler ref "https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.IDragHandler.html"
//ATTEND: Controller should get refference from Init method not in awake method only 
//TODO: MoveTo implemented
//TODO: Clamp Input
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Eccentric;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TmUnity.Node
{
    abstract class ANode : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Sprite normalSprite = null;
        [SerializeField] Sprite outlineSprite = null;
        Image image = null;
        Vector2Int point = default(Vector2Int);
        Vector2 aspectOffset = default(Vector2);
        NodeController controller = null;
        Vector2 size = default(Vector2);
        bool isDraging = false;
        bool isCanMove = false;
        public RectTransform RectTransform { get; private set; } = null;
        public Vector2Int Point
        {
            get => point;
            protected set
            {
                point = value;
                RectTransform.anchoredPosition = ToAnchoredPosition();
                gameObject.name = point.ToString();
            }
        }
        public NodeType Type { get; private set; } = default(NodeType);
        public bool IsActive { get; private set; } = true;

        void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }

        virtual protected void Init(Vector2Int point, NodeType type, NodeController controller)
        {
            image.sprite = normalSprite;
            size = RectTransform.sizeDelta;
            Point = point;
            Type = type;
            this.controller = controller;
            var halfSize = size * 0.5f;
            aspectOffset = new Vector2(-halfSize.x * controller.AspectFactor.x, halfSize.y * controller.AspectFactor.y);
            IsActive = true;
            gameObject.SetActive(true);
        }

        protected Vector3 GetCenterPos() => RectTransform.position - (Vector3)aspectOffset;

        public void OnPointerDown(PointerEventData e) => isCanMove = controller.IsCanMove;

        public void OnBeginDrag(PointerEventData e)
        {
            if (!isCanMove)
                return;
            isDraging = true;
            DomainEvents.Raise<OnNodeDragBegin>(new OnNodeDragBegin(this));
        }

        public void OnDrag(PointerEventData e)
        {
            if (!isCanMove || !isDraging)
                return;
            if (controller.IsNodeSwaping)
                return;
            RectTransform.position = e.position + aspectOffset;
            var res = controller.ScreenPosToPoint(RectTransform.position);
            var offset = res - Point;
            if (Point != res && (Mathf.Abs(offset.x) <= 1 && Mathf.Abs(offset.y) <= 1))
                controller.Swap(Point, res);
        }

        public void OnPointerUp(PointerEventData e) => CorrectPos();

        public void OnEndDrag(PointerEventData e)
        {
            if (isDraging && isCanMove)
            {
                isCanMove = false;
                isDraging = false;
                DomainEvents.Raise<OnNodeDragEnd>(new OnNodeDragEnd(this));
            }

        }

        public void ChangeSprite(bool changeToOutline = true) => image.sprite = changeToOutline ? outlineSprite : normalSprite;

        virtual public void Eliminate(bool isFXPlay = true)
        {
            IsActive = false;
            gameObject.SetActive(false);
        }

        public void ForceEndDrag()
        {
            var res = controller.ScreenPosToPoint((Vector2)RectTransform.position);
            if (Point != res)
                controller.Swap(Point, res);
            CorrectPos();
            isCanMove = false;
            isDraging = false;
            DomainEvents.Raise<OnNodeDragEnd>(new OnNodeDragEnd(this));
        }

        public void MoveToPoint(Vector2Int newPoint) => Point = newPoint;



        public void CheckResult(ref List<ANode> founds)
        {
            founds.Add(this);
            var nextPoint = default(Vector2Int);
            //left
            nextPoint.x = Point.x - 1;
            nextPoint.y = Point.y;
            CheckNextNode(nextPoint, ref founds);
            //up
            nextPoint.x = Point.x;
            nextPoint.y = Point.y - 1;
            CheckNextNode(nextPoint, ref founds);
            //right
            nextPoint.x = Point.x + 1;
            nextPoint.y = Point.y;
            CheckNextNode(nextPoint, ref founds);
            //down
            nextPoint.x = Point.x;
            nextPoint.y = Point.y + 1;
            CheckNextNode(nextPoint, ref founds);
        }

        void CorrectPos() => RectTransform.anchoredPosition = ToAnchoredPosition();

        Vector2 ToAnchoredPosition() => new Vector2(point.x * size.x, -point.y * size.y);

        void CheckNextNode(Vector2Int nextPoint, ref List<ANode> founds)
        {
            if (!controller.IsPointOutOfBoard(nextPoint))
            {
                var nextNode = controller.ActiveNodes[nextPoint.x, nextPoint.y];
                if (nextNode.IsActive && nextNode.Type == Type && !founds.Contains(nextNode))
                    nextNode.CheckResult(ref founds);
            }
        }

    }

}