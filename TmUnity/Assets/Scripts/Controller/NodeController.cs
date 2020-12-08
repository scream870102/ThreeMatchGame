//TODO: Implemented Node Check 
//ATTEND: Implement Clamp position
//TODO: Check All Result
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace TmUnity.Node
{
    class NodeController : MonoBehaviour
    {
        [SerializeField] Vector2Int boardSize = new Vector2Int(6, 8);
        [SerializeField] GameObject nodeObject = null;
        [SerializeField] List<Sprite> nodeSprites = new List<Sprite>();
        RectTransform boardParent = null;
        Vector2 refRes = default(Vector2);
        Vector2 adjustedNodeSize = default(Vector2);
        public Vector2 AspectFactor { get; private set; } = default(Vector2);
        public Vector2 BoardMaxSize { get; private set; } = default(Vector2);
        public Vector2 AdjustedBoardMaxSize { get; private set; } = default(Vector2);
        public ANode[,] ActiveNodes { get; private set; } = null;
        void Awake()
        {
            //Get Ref
            boardParent = GameObject.Find("BoardParent").GetComponent<RectTransform>();
            refRes = boardParent.parent.GetComponent<CanvasScaler>().referenceResolution;
            //Set var
            AspectFactor = new Vector2(Screen.width / refRes.x, Screen.height / refRes.y);
            var tmpNode = Instantiate(nodeObject).GetComponent<ANode>();
            var nodeSize = tmpNode.Size;
            adjustedNodeSize = new Vector2(nodeSize.x * AspectFactor.x, nodeSize.y * AspectFactor.y);
            BoardMaxSize = new Vector2(boardSize.x * tmpNode.Size.x, boardSize.y * tmpNode.Size.y);
            AdjustedBoardMaxSize = BoardMaxSize * AspectFactor;
            //Init board
            ActiveNodes = new ANode[boardSize.x, boardSize.y];
            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                {
                    var node = Instantiate(nodeObject, boardParent).GetComponent<ANode>();
                    var type = Random.Range(0, System.Enum.GetNames(typeof(NodeType)).Length);
                    ActiveNodes[i, j] = node;
                    var point = new Vector2Int(i, j);
                    node.name = point.ToString();
                    node.Init(point, (NodeType)type, this, nodeSprites[type]);
                }
            }
            Destroy(tmpNode.gameObject);
        }

        void Update() {
            
        }

        public bool IsPointOutOfBoard(Vector2Int point) => (point.x >= boardSize.x) || (point.y >= boardSize.y) || (point.x < 0) || (point.y < 0);

        public Vector2Int ScreenPosToPoint(Vector2 pos)
        {
            var adjustedPos = pos;
            adjustedPos.y = adjustedPos.y - boardParent.anchoredPosition.y * AspectFactor.y;
            var y = boardSize.y - Mathf.RoundToInt(adjustedPos.y / adjustedNodeSize.y);
            y = y < 0 ? 0 : y;
            return new Vector2Int(Mathf.FloorToInt(adjustedPos.x / adjustedNodeSize.x), y);
        }


        public void Swap(Vector2Int movingNode, Vector2Int swapNode)
        {
            if (IsPointOutOfBoard(swapNode))
                return;
            ActiveNodes[swapNode.x, swapNode.y].MoveToPoint(movingNode);
            ActiveNodes[movingNode.x, movingNode.y].MoveToPoint(swapNode);

            var tmpNode = ActiveNodes[swapNode.x, swapNode.y];
            ActiveNodes[swapNode.x, swapNode.y] = ActiveNodes[movingNode.x, movingNode.y];
            ActiveNodes[movingNode.x, movingNode.y] = tmpNode;
        }
        public void CheckAllResult()
        {
            var unpairNode = new List<ANode>();
            foreach (var node in ActiveNodes)
            {
                Debug.Log(node.name);
            }
        }
    }
}
