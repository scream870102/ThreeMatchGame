#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace TmUnity.Editor
{
    class RectTransformToolkit : MonoBehaviour
    {
        [HideInInspector] public RectTransform RectTransform = null;
        void Awake() => RectTransform = GetComponent<RectTransform>();
    }

    [CustomEditor(typeof(RectTransformToolkit))]
    class RectTranformToolkitEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            RectTransformToolkit t = (RectTransformToolkit)target;
            if (GUILayout.Button(("Get Position")))
                Debug.Log($"{t.name}'s Position : {t.RectTransform.position} AnchoredPostion : {t.RectTransform.anchoredPosition} LocalPostion : {t.RectTransform.localPosition}");
        }
    }
}

#endif