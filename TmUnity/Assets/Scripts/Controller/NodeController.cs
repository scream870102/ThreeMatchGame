//ATTEND: Implement Clamp position
//TODO: Implement Anmation
//TODO: Player Stats Implement
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Eccentric.Utils;
using Lean.Pool;
namespace TmUnity.Node
{
    class NodeController : MonoBehaviour
    {
        [ReadOnly] [SerializeField] GameStats gameStats = null;

        [SerializeField] Vector2Int boardSize = new Vector2Int(6, 8);
        [SerializeField] GameObject[] nodeObjects = null;
        [SerializeField] PlayerAttr attr = null;

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
            var tmpNode = Instantiate(nodeObjects[0]).GetComponent<ANode>();
            var nodeSize = tmpNode.Size;
            adjustedNodeSize = new Vector2(nodeSize.x * AspectFactor.x, nodeSize.y * AspectFactor.y);
            BoardMaxSize = new Vector2(boardSize.x * tmpNode.Size.x, boardSize.y * tmpNode.Size.y);
            AdjustedBoardMaxSize = BoardMaxSize * AspectFactor;
            InitPlayerStats();
            //Init board
            ActiveNodes = new ANode[boardSize.x, boardSize.y];
            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                {
                    var type = Random.Range(0, System.Enum.GetNames(typeof(NodeType)).Length);
                    SpawnNode(i, j, (NodeType)type);
                }
            }
            Destroy(tmpNode.gameObject);
        }

        void InitPlayerStats()
        {
        }

        void SpawnNode(int x, int y, NodeType type)
        {

            var node = LeanPool.Spawn(nodeObjects[(int)type], boardParent).GetComponent<ANode>();
            ActiveNodes[x, y] = node;
            var point = new Vector2Int(x, y);
            node.name = point.ToString();
            switch (type)
            {
                case NodeType.NORMAL:
                    (node as NormalNode).Init(attr.NormalBasicAtk, point, (NodeType)type, this);
                    break;
                case NodeType.CHARGE:
                    (node as ChargeNode).Init(attr.ChargeBasicAtk, point, (NodeType)type, this);
                    break;
                case NodeType.ENERGY:
                    (node as EnergyNode).Init(attr.EnergyBasicTimePlus, point, (NodeType)type, this);
                    break;
                case NodeType.DEFENSE:
                    (node as DefenseNode).Init(attr.DefBasicUp, point, (NodeType)type, this);
                    break;
                case NodeType.CHEST:
                    var chestType = Random.Range(0, System.Enum.GetNames(typeof(ChestType)).Length);
                    (node as ChestNode).Init((ChestType)chestType, point, (NodeType)type, this);
                    break;
            }
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(CheckAllResult());
            if (Input.GetKeyDown(KeyCode.C))
                StartCoroutine(UpdateBoardPosition());
            if (Input.GetKeyDown(KeyCode.V))
                StartCoroutine(AddNewNode());
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

        //Check if there is another under current node if true move current node to under
        IEnumerator UpdateBoardPosition()
        {
            //form left bottom
            for (int j = boardSize.y - 2; j >= 0; j--)
            {
                for (int i = 0; i < boardSize.x; i++)
                {
                    var node = ActiveNodes[i, j];
                    // if node is disable do nothing
                    if (!node.IsActive)
                        continue;
                    ANode underNode = null;
                    int nextY = j;
                    //Try to find the next node
                    while (underNode == null)
                    {
                        nextY++;
                        // if next one is out of board break the loop
                        if (nextY >= boardSize.y)
                            break;
                        var tmpNextNode = ActiveNodes[i, nextY];
                        // if nextNode is the bottom row and is deactive add it
                        if (nextY == boardSize.y - 1 && !tmpNextNode.IsActive)
                            underNode = tmpNextNode;
                        //if next node is active and above the next node is deactive add it
                        if (tmpNextNode.IsActive && !ActiveNodes[i, nextY - 1].IsActive)
                            underNode = ActiveNodes[i, nextY - 1];
                    }
                    if (underNode != null)
                    {
                        Swap(node.Point, underNode.Point);
                        yield return new WaitForSeconds(.1f);
                    }
                }
            }
        }

        public IEnumerator CheckAllResult()
        {
            // Add all node into unpairnode
            var unpairNode = new List<ANode>();
            foreach (var node in ActiveNodes)
                unpairNode.Add(node);
            // check all node in unpair node if is exist in result node eliminate it
            for (int i = 0; i < unpairNode.Count; i++)
            {
                if (unpairNode[i] == null)
                    continue;
                var checkNode = unpairNode[i];
                var resultNode = new List<ANode>();
                checkNode.CheckResult(ref resultNode);
                if (resultNode.Count >= 3)
                {
                    foreach (var o in resultNode)
                    {
                        unpairNode[unpairNode.IndexOf(o)] = null;
                        o.Eliminate();
                    }
                    yield return new WaitForSeconds(.2f);
                }
            }
        }

        public IEnumerator AddNewNode()
        {
            var deactiveNodes = new List<ANode>();
            foreach (var node in ActiveNodes)
            {
                if (!node.IsActive)
                    deactiveNodes.Add(node);
            }

            var types = new NodeType[deactiveNodes.Count];
            var typeNum = System.Enum.GetNames(typeof(NodeType)).Length;
            for (int i = 0; i < deactiveNodes.Count / typeNum; i++)
            {
                for (int j = 0; j < typeNum; j++)
                    types[i * typeNum + j] = (NodeType)j;
            }
            for (int i = 0; i < types.Length % typeNum; i++)
                types[types.Length - i - 1] = (NodeType)Random.Range(0, typeNum);

            for (int i = 0; i < deactiveNodes.Count; i++)
            {
                Vector2Int point = deactiveNodes[i].Point;
                LeanPool.Despawn(deactiveNodes[i]);
                SpawnNode(point.x, point.y, types[i]);
                yield return new WaitForSeconds(.05f);
            }
        }
    }
}
