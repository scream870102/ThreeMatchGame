//NOTE: CHECK DATA USE https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.PointerEventData.html
//NOTE: IDragHandler ref "https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.IDragHandler.html"
//FIXME: Controller should get refference from Init method not in awake method
//TODO: MoveTo implemented
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Eccentric.Utils;

namespace TmUnity
{
    abstract class ANode : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public NodeController Controller { get; private set; }
        Vector2 point = default(Vector2);
        public Vector2 Point
        {
            get => point;
            protected set
            {
                point = value;
                RectTransform.anchoredPosition = ToAnchoredPosition();
            }
        }
        public ENodeType Type { get; private set; }
        public Vector2 Size { get; private set; }
        protected Vector2 HalfSize => Size / 2f;
        protected RectTransform RectTransform { get; private set; }
        protected Image Image { get; private set; }

        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            Size = RectTransform.sizeDelta;
            Controller = GameObject.FindObjectOfType<NodeController>();
            Image = GetComponent<Image>();
        }

        public virtual void Init(Vector2 point, ENodeType type, NodeController controller, Sprite sprite)
        {
            Point = point;
            Type = type;
            Controller = controller;
            Image.sprite = sprite;
        }

        public abstract void OnDrag(PointerEventData e);

        public abstract void OnBeginDrag(PointerEventData e);

        public abstract void OnEndDrag(PointerEventData e);

        protected void CorrectPos()
        {
            var tmpPos = RectTransform.anchoredPosition;
            Vector2 newPos = new Vector2(Mathf.Round(tmpPos.x / Size.x) * Size.x, Mathf.Round(tmpPos.y / Size.y) * Size.y);
            newPos.x = Mathf.Clamp(newPos.x, 0f, Controller.MaxSize.x);
            newPos.y = Mathf.Clamp(newPos.y, -Controller.MaxSize.y, 0f);
            RectTransform.anchoredPosition = newPos;
        }

        public Vector2 ToAnchoredPosition() => new Vector2(point.x * Size.x, -point.y * Size.y);

    }
    enum ENodeType
    {
        NORMAL,
        CHARGE,
        ENERGY,
        DEFENSE,
        CHEST,
    }
}