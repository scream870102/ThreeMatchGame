#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TmUnity.Node;
namespace TmUnity.ForEditor
{
    class NodeToolkit : MonoBehaviour
    {
        [HideInInspector] public ANode Node = null;
        [HideInInspector] public RectTransform RectTransform = null;
        public NodeType Type = NodeType.NORMAL;
        public Vector2Int Point = default(Vector2Int);
        void Awake()
        {
            Node = GetComponent<ANode>();
            RectTransform = GetComponent<RectTransform>();
        }
        void Update()
        {
            Type = Node.Type;
            Point = Node.Point;
        }
    }
    [CustomEditor(typeof(NodeToolkit))]
    class NodeToolkitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            NodeToolkit myScript = (NodeToolkit)target;
            if (GUILayout.Button("Get Anchored Position"))
                Debug.Log($"{myScript.Node.name}'s anchored position :{myScript.RectTransform.anchoredPosition}");
            if (GUILayout.Button("Get Point"))
                Debug.Log($"Anchroed Position : {myScript.Node.RectTransform.anchoredPosition} Point : {myScript.Node.Point} ");
            if (GUILayout.Button("Check Result"))
            {
                //myScript.Node.Controller.CheckResult(myScript.Node.Point);
                var founds = new List<ANode>();
                myScript.Node.CheckResult(ref founds);
            }

        }
    }
}

#endif