#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ANode = TmUnity.Node.ANode;
namespace TmUnity.ForEditor
{
    class NodeToolkit : MonoBehaviour
    {
        [HideInInspector] public ANode Node = null;
        [HideInInspector] public RectTransform RectTransform = null;
    }
    [CustomEditor(typeof(NodeToolkit))]
    class NodeToolkitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            NodeToolkit myScript = (NodeToolkit)target;
            myScript.Node = myScript.Node == null ? myScript.GetComponent<ANode>() : myScript.Node;
            myScript.RectTransform = myScript.RectTransform == null ? myScript.GetComponent<RectTransform>() : myScript.RectTransform;
            if (GUILayout.Button("Get Anchored Position"))
                Debug.Log($"{myScript.Node.name}'s anchored position :{myScript.RectTransform.anchoredPosition}");
            if (GUILayout.Button("Get Point"))
                Debug.Log($"Anchroed Position : {myScript.Node.RectTransform.anchoredPosition} Point : {myScript.Node.Point} ");
            if (GUILayout.Button("Check Result"))
                myScript.Node.CheckResult();

        }
    }
}

#endif