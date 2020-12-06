//TODO: CHECK DATA USE https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.PointerEventData.html
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TmUnity
{
    class ANode : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
        public void OnPointerUp(PointerEventData e) {
            Debug.Log($"Pointer UP Position {e.position} Raycast Result {e.pointerCurrentRaycast.gameObject.name} Gameobject {e.pointerPress.name}");
        }
        public void OnPointerDown(PointerEventData e) {
            Debug.Log($"Pointer DOWN Position {e.position} ");
            Debug.Log($"Raycast Result {e.pointerCurrentRaycast.gameObject.name}");
            Debug.Log($"Gameobject {e.rawPointerPress.name}");

        }

    }

}

