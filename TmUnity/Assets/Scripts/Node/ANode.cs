//NOTE: CHECK DATA USE https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.PointerEventData.html
//NOTE: IDragHandler ref "https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.IDragHandler.html"
//ATTEND: Controller should get refference from Init method not in awake method only 
//TODO: MoveTo implemented
//TODO: Shoudl Clamp Input
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Eccentric;
using System.Collections.Generic;
using Lean.Pool;

namespace TmUnity.Node
{
    abstract class ANode : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        Vector2Int point = default(Vector2Int);
        protected Vector2 oldPos { get; set; } = default(Vector2);
        protected Vector2 halfSize => Size / 2;
        protected Vector2 aspectOffset { get; private set; } = default(Vector2);
        protected bool isDraging { get; private set; } = false;
        protected bool isCanMove { get; private set; } = false;
        public RectTransform RectTransform { get; private set; } = null;
        public NodeController Controller { get; private set; } = null;
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
        public Vector2 Size { get; private set; } = default(Vector2);
        public bool IsActive { get; private set; } = true;

        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            Size = RectTransform.sizeDelta;
#if UNITY_EDITOR
            Controller = GameObject.FindObjectOfType<NodeController>();
#endif
        }

        protected virtual void Init(Vector2Int point, NodeType type, NodeController controller)
        {
            Point = point;
            Type = type;
            Controller = controller;
            aspectOffset = new Vector2(-halfSize.x * controller.AspectFactor.x, halfSize.y * controller.AspectFactor.y);
            IsActive = true;
            gameObject.SetActive(true);
        }

        public virtual void Eliminate()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }

        public virtual void OnPointerDown(PointerEventData e) => isCanMove = Controller.IsCanMove;

        public virtual void OnBeginDrag(PointerEventData e)
        {
            if (!isCanMove)
                return;
            isDraging = true;
            oldPos = RectTransform.anchoredPosition;
            DomainEvents.Raise<OnNodeDragBegin>(new OnNodeDragBegin(this));
        }

        public virtual void OnDrag(PointerEventData e)
        {
            if (!isCanMove || !isDraging)
                return;
            RectTransform.position = e.position + aspectOffset;
            var res = Controller.ScreenPosToPoint(RectTransform.position);
            if (Point != res)
                Controller.Swap(Point, res);
        }

        public virtual void OnPointerUp(PointerEventData e) => CorrectPos();

        public virtual void OnEndDrag(PointerEventData e)
        {
            if (isDraging && isCanMove)
            {
                isCanMove = false;
                isDraging = false;
                DomainEvents.Raise<OnNodeDragEnd>(new OnNodeDragEnd(this));
            }

        }

        public void ForceEndDrag()
        {
            var res = Controller.ScreenPosToPoint((Vector2)RectTransform.position);
            if (Point != res)
                Controller.Swap(Point, res);
            CorrectPos();
            isCanMove = false;
            isDraging = false;
            DomainEvents.Raise<OnNodeDragEnd>(new OnNodeDragEnd(this));
        }

        public void MoveToPoint(Vector2Int newPoint) => Point = newPoint;

        protected void CorrectPos() => RectTransform.anchoredPosition = ToAnchoredPosition();

        protected Vector2 ToAnchoredPosition() => new Vector2(point.x * Size.x, -point.y * Size.y);

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
            return;
        }

        void CheckNextNode(Vector2Int nextPoint, ref List<ANode> founds)
        {
            if (!Controller.IsPointOutOfBoard(nextPoint))
            {
                var nextNode = Controller.ActiveNodes[nextPoint.x, nextPoint.y];
                if (nextNode.IsActive && nextNode.Type == Type && !founds.Contains(nextNode))
                    nextNode.CheckResult(ref founds);
            }
        }

        public Vector3 GetCenterPosition() => RectTransform.position + (Vector3)aspectOffset;
    }

}