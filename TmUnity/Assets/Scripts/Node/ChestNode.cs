﻿using UnityEngine;
using Eccentric;
namespace TmUnity.Node
{
    class ChestNode : ANode
    {
        public ChestNodeAttr Attr { get; private set; } = null;
        public ChestType ChestType { get; private set; } = default(ChestType);
        public void Init(ChestNodeAttr attr, ChestType chestType, Vector2Int point, NodeType type, NodeController controller)
        {
            base.Init(point, type, controller);
            ChestType = chestType;
            Attr = attr;
        }
        public override void Eliminate(bool isFXPlay = true)
        {
            if (isFXPlay)
                DomainEvents.Raise<OnVFXPlay>(new OnVFXPlay(GetCenterPos(), VFXType.HEAL));
            base.Eliminate();
        }
    }
}
