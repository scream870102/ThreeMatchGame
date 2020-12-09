﻿using UnityEngine;
namespace TmUnity.Node
{
    class NormalNode : ANode
    {
        public int Atk { get; private set; } = 0;
        public void Init(int atk, Vector2Int point, NodeType type, NodeController controller)
        {
            Atk = atk;
            base.Init(point, type, controller);
        }

    }
}