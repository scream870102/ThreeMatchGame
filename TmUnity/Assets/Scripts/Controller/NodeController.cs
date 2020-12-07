//TODO: Implemented Node Check 
//TODO: ImageChange
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Eccentric.Utils;

namespace TmUnity
{
    class NodeController : MonoBehaviour
    {
        [SerializeField] Vector2 BoardSize = new Vector2(6f, 8f);
        [SerializeField] GameObject NodeObject = null;
        [SerializeField] RectTransform BoardParent = null;
        public Vector2 MaxSize { get; set; } = default(Vector2);
        [SerializeField] List<ANode> ActiveNodes = new List<ANode>();
        [SerializeField] List<Sprite> nodeSprite = new List<Sprite>();
        void Awake()
        {
            ANode tmp = Instantiate(NodeObject).GetComponent<ANode>();
            MaxSize = new Vector2((BoardSize.x - 1f) * tmp.Size.x, (BoardSize.y - 1f) * tmp.Size.y);
            for (int i = 0; i < BoardSize.x; i++)
            {
                for (int j = 0; j < BoardSize.y; j++)
                {
                    ANode node = Instantiate(NodeObject, BoardParent).GetComponent<ANode>();
                    int type = Random.Range(0, System.Enum.GetNames(typeof(ENodeType)).Length);
                    ActiveNodes.Add(node);
                    node.name = new Vector2(i, j).ToString();
                    node.Init(new Vector2(i, j), (ENodeType)type, this, nodeSprite[type]);
                }
            }

        }
    }
}
