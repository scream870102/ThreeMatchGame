//ATTEND: Implement Clamp position
//TODO: Implement Anmation
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using Lean.Pool;
using System.Threading.Tasks;
namespace TmUnity.Node
{
    class NodeController : MonoBehaviour
    {
        [SerializeField] Vector2Int boardSize = new Vector2Int(6, 8);
        [SerializeField] GameObject[] nodeObjects = null;
        [SerializeField] NodeAttr attr = null;
        RectTransform boardParent = null;
        Vector2 refRes = default(Vector2);
        Vector2 adjustedNodeSize = default(Vector2);
        ANode currentNode = null;
        public Vector2 AspectFactor { get; private set; } = default(Vector2);
        public Vector2 BoardMaxSize { get; private set; } = default(Vector2);
        public Vector2 AdjustedBoardMaxSize { get; private set; } = default(Vector2);
        public ANode[,] ActiveNodes { get; private set; } = null;
        public bool IsCanMove { get; private set; } = false;

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
            Destroy(tmpNode.gameObject);
        }

        void OnEnable()
        {
            DomainEvents.Register<OnGameStateChange>(HandleGameStateChange);
            DomainEvents.Register<OnNodeDragBegin>(HandleNodeDragBegin);
            DomainEvents.Register<OnNodeDragEnd>(HandleNodeDragEnd);
            DomainEvents.Register<OnForceEndDrag>(HandleForceEndDrag);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnGameStateChange>(HandleGameStateChange);
            DomainEvents.UnRegister<OnNodeDragBegin>(HandleNodeDragBegin);
            DomainEvents.UnRegister<OnNodeDragEnd>(HandleNodeDragEnd);
            DomainEvents.UnRegister<OnForceEndDrag>(HandleForceEndDrag);
        }

        void HandleGameStateChange(OnGameStateChange e) => IsCanMove = (e.NewState == GameState.WAIT || e.NewState == GameState.ACTION);

        void HandleNodeDragBegin(OnNodeDragBegin e) => currentNode = e.Node;

        void HandleNodeDragEnd(OnNodeDragEnd e) => currentNode = null;

        void HandleForceEndDrag(OnForceEndDrag e) => currentNode?.ForceEndDrag();

        public void InitBoard()
        {
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
                    (node as NormalNode).Init(attr.NormalAtk, point, (NodeType)type, this);
                    break;
                case NodeType.CHARGE:
                    (node as ChargeNode).Init(attr.ChargeAtk, point, (NodeType)type, this);
                    break;
                case NodeType.ENERGY:
                    (node as EnergyNode).Init(attr.Energy, point, (NodeType)type, this);
                    break;
                case NodeType.DEFENSE:
                    (node as DefenseNode).Init(attr.Def, point, (NodeType)type, this);
                    break;
                case NodeType.CHEST:
                    var chestType = Random.Range(0, System.Enum.GetNames(typeof(ChestType)).Length);
                    (node as ChestNode).Init(attr.ChestNodeAttr, (ChestType)chestType, point, (NodeType)type, this);
                    break;
            }
        }

        public async Task CalculateResultAsync()
        {
            await CheckAllResultAsync();
            await UpdateBoardPositionAsync();
            var isAnyNodeSpawn = await AddNewNodeAsync();
            if (isAnyNodeSpawn)
                await CalculateResultAsync();
        }

        public void CalculateResult()
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
                    CheckResult(resultNode, unpairNode);
            }

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
                        Swap(node.Point, underNode.Point);
                }
            }
            var isAnyNodeSpawn = false;
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
                isAnyNodeSpawn = true;
                Vector2Int point = deactiveNodes[i].Point;
                LeanPool.Despawn(deactiveNodes[i]);
                SpawnNode(point.x, point.y, types[i]);
            }
            if (isAnyNodeSpawn)
                CalculateResult();
        }

        public bool IsPointOutOfBoard(Vector2Int point) => (point.x >= boardSize.x) || (point.y >= boardSize.y) || (point.x < 0) || (point.y < 0);

        public Vector2Int ScreenPosToPoint(Vector2 pos)
        {
            var adjustedPos = pos;
            adjustedPos.y = adjustedPos.y - boardParent.anchoredPosition.y * AspectFactor.y;
            var y = boardSize.y - Mathf.RoundToInt(adjustedPos.y / adjustedNodeSize.y);
            y = y < 0 ? 0 : y;
            return new Vector2Int(Mathf.RoundToInt(adjustedPos.x / adjustedNodeSize.x), y);
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
        async Task UpdateBoardPositionAsync()
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
                        await Task.Delay(100);
                    }
                }
            }
        }

        EliminateInfo CheckResult(List<ANode> resultNode, List<ANode> unpairNode)
        {
            var eliminateInfo = new EliminateInfo();
            var type = resultNode[0].Type;
            DomainEvents.Raise<OnComboPlus>(new OnComboPlus());
            foreach (var o in resultNode)
            {
                unpairNode[unpairNode.IndexOf(o)] = null;
                o.Eliminate();
                switch (type)
                {
                    case NodeType.NORMAL:
                        eliminateInfo.NormalAtk += (o as NormalNode).Atk;
                        break;
                    case NodeType.CHARGE:
                        eliminateInfo.ChargeAtk += (o as ChargeNode).BasicAtk;
                        eliminateInfo.ChargeNum += 1;
                        break;
                    case NodeType.ENERGY:
                        eliminateInfo.EnergyTime += (o as EnergyNode).TimePlus;
                        break;
                    case NodeType.DEFENSE:
                        eliminateInfo.Def += (o as DefenseNode).Def;
                        break;
                    case NodeType.CHEST:
                        var node = (o as ChestNode);
                        switch (node.ChestType)
                        {

                            case ChestType.ATK_UP:
                                eliminateInfo.NormalAtk += node.Attr.AtkUp;
                                break;
                            case ChestType.CHARGE_COUNT_PLUS:
                                eliminateInfo.ChargeNum += node.Attr.ChargeCount;
                                break;
                            case ChestType.DEF_UP:
                                eliminateInfo.Def += node.Attr.DefUp;
                                break;
                            case ChestType.ENERGY_UP:
                                eliminateInfo.EnergyTime += node.Attr.EnergyUp;
                                break;
                            case ChestType.HP_RECOVER:
                                eliminateInfo.HPRecover += node.Attr.HPRecover;
                                break;
                        }
                        break;
                }
            }
            return eliminateInfo;
        }

        async Task CheckAllResultAsync()
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
                    var eliminateInfo = CheckResult(resultNode, unpairNode);
                    DomainEvents.Raise<OnNodeEliminate>(new OnNodeEliminate(eliminateInfo));
                    await Task.Delay(200);
                }
            }
        }

        async Task<bool> AddNewNodeAsync()
        {
            var isAnyNodeSpawn = false;
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
                isAnyNodeSpawn = true;
                Vector2Int point = deactiveNodes[i].Point;
                LeanPool.Despawn(deactiveNodes[i]);
                SpawnNode(point.x, point.y, types[i]);
                await Task.Delay(50);
            }
            return isAnyNodeSpawn;
        }
    }
}
