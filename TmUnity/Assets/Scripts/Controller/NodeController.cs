//ATTEND: Implement Clamp position
//ATTEND: Chest Type now only have hp recover
//FIXME: SwapAsync
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using Lean.Pool;
namespace TmUnity.Node
{
    class NodeController : MonoBehaviour
    {
        [SerializeField] float nodeFallenTime = .05f;
        [SerializeField] Vector2Int boardSize = new Vector2Int(6, 8);
        [SerializeField] GameObject[] nodePrefabs = null;
        [SerializeField] NodeAttr attr = null;
        [SerializeField] RectTransform boardParent = null;
        Vector2 refRes = default(Vector2);
        Vector2 adjustedNodeSize = default(Vector2);
        ANode currentNode = null;
        public Vector2 AspectFactor { get; private set; } = default(Vector2);
        public Vector2 BoardMaxSize { get; private set; } = default(Vector2);
        public ANode[,] ActiveNodes { get; private set; } = null;
        public bool IsCanMove { get; private set; } = false;

        public bool IsNodeSwaping { get; private set; } = false;
        void Awake()
        {
            refRes = boardParent.parent.parent.GetComponent<CanvasScaler>().referenceResolution;
            //NOTE: The aspect of board is constant so just calcluate the apsectFactor by width  cause screen height will change
            AspectFactor = new Vector2(Screen.width / refRes.x, Screen.width / refRes.x);
            adjustedNodeSize = new Vector2(Screen.width / boardSize.x, Screen.width / boardSize.x);
            BoardMaxSize = new Vector2(boardSize.x * adjustedNodeSize.x, boardSize.y * adjustedNodeSize.y);
        }

        void OnEnable()
        {
            DomainEvents.Register<OnGameStateChange>(HandleGameStateChange);
            DomainEvents.Register<OnNodeDragBegin>(HandleNodeDragBegin);
            DomainEvents.Register<OnNodeDragEnd>(HandleNodeDragEnd);
        }

        void OnDisable()
        {
            DomainEvents.UnRegister<OnGameStateChange>(HandleGameStateChange);
            DomainEvents.UnRegister<OnNodeDragBegin>(HandleNodeDragBegin);
            DomainEvents.UnRegister<OnNodeDragEnd>(HandleNodeDragEnd);
        }

        void HandleGameStateChange(OnGameStateChange e) => IsCanMove = (e.NewState == GameState.WAIT || e.NewState == GameState.ACTION);

        void HandleNodeDragBegin(OnNodeDragBegin e) => currentNode = e.Node;

        void HandleNodeDragEnd(OnNodeDragEnd e) => currentNode = null;

        public void ForceEndDrag() => currentNode?.ForceEndDrag();

        public void InitBoard()
        {
            ActiveNodes = new ANode[boardSize.x, boardSize.y];
            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                    SpawnNode(i, j, (NodeType)Random.Range(0, System.Enum.GetNames(typeof(NodeType)).Length));
            }
        }

        async public Task CalculateResultAsync()
        {
            //var resultsNodes = new List<ANode>();
            //var infos = await CheckAllResultAsync();
            //var infos = CheckAllResult(ref resultsNodes);
            var infos = CheckAllResult();
            await PlayEliminateAnimation(infos);
            await UpdateBoardPositionAsync();
            var isAnyNodeSpawn = await AddNewNodeAsync();
            if (isAnyNodeSpawn)
                await CalculateResultAsync();
        }

        public Dictionary<EliminateInfo, List<ANode>> CheckAllResult()
        {
            var infoAndNodes = new Dictionary<EliminateInfo, List<ANode>>();
            //var eliminateInfos = new List<EliminateInfo>();
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
                    resultNode.ForEach(node => node.ChangeSprite(true));
                    infoAndNodes.Add(GetEliminateResultNotEliminate(resultNode, unpairNode), resultNode);
                    // foreach (var node in resultNode)
                    // {
                    //     node.ChangeSprite(true);
                    //     resultNodes.Add(node);
                    // }
                    // //resultNode.ForEach(node => node.ChangeSprite(true));
                    // eliminateInfos.Add(GetEliminateResultNotEliminate(resultNode, unpairNode));
                    //eliminateInfos.Add(isEliminate ? GetEliminateResultAndEliminate(resultNode, unpairNode) : GetEliminateResultNotEliminate(resultNode, unpairNode));
                }
                else
                    checkNode.ChangeSprite(false);
            }
            //return eliminateInfos;
            return infoAndNodes;
        }

        public void CalculateResultWithoutAnim()
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
                    GetEliminateResultAndEliminate(resultNode, unpairNode, false);
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
                CalculateResultWithoutAnim();
        }

        public bool IsPointOutOfBoard(Vector2Int point) => (point.x >= boardSize.x) || (point.y >= boardSize.y) || (point.x < 0) || (point.y < 0);

        public Vector2Int ScreenPosToPoint(Vector2 pos)
        {
            var adjustedPos = pos;
            adjustedPos.y = adjustedPos.y - boardParent.position.y;
            var y = boardSize.y - Mathf.RoundToInt(adjustedPos.y / adjustedNodeSize.y);
            y = y < 0 ? 0 : y;
            return new Vector2Int(Mathf.RoundToInt(adjustedPos.x / adjustedNodeSize.x), y);
        }

        public Vector2 PointToAnchoredPos(Vector2 point) => new Vector2(point.x * adjustedNodeSize.x, -point.y * adjustedNodeSize.y);

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

        async public Task SwapAsync(Vector2Int movingNode, Vector2Int swapNode)
        {
            if (IsPointOutOfBoard(swapNode))
                return;

            IsNodeSwaping = true;
            ActiveNodes[movingNode.x, movingNode.y].MoveToPoint(swapNode);
            await ActiveNodes[swapNode.x, swapNode.y].MoveToPointAsync(movingNode);
            var tmpNode = ActiveNodes[swapNode.x, swapNode.y];
            ActiveNodes[swapNode.x, swapNode.y] = ActiveNodes[movingNode.x, movingNode.y];
            ActiveNodes[movingNode.x, movingNode.y] = tmpNode;
            IsNodeSwaping = false;
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
                        //Caculate the distance
                        var vel = (node.RectTransform.position.y - underNode.RectTransform.position.y) / nodeFallenTime;
                        while (node.RectTransform.position.y > underNode.RectTransform.position.y)
                        {
                            var newPos = node.RectTransform.position - new Vector3(0f, vel * Time.deltaTime, 0f);
                            node.RectTransform.position = newPos;
                            await Task.Delay((int)(Time.deltaTime * 1000f));
                        }
                        Swap(node.Point, underNode.Point);
                    }
                }
            }
        }

        // async Task<List<EliminateInfo>> CheckAllResultAsync()
        // {
        //     var eliminateInfos = new List<EliminateInfo>();
        //     // Add all node into unpairnode
        //     var unpairNode = new List<ANode>();
        //     // NOTE: Check node from left top to right bottom
        //     for (int j = 0; j < boardSize.y; j++)
        //     {
        //         for (int i = 0; i < boardSize.x; i++)
        //             unpairNode.Add(ActiveNodes[i, j]);
        //     }
        //     // check all node in unpair node if is exist in result node eliminate it
        //     for (int i = 0; i < unpairNode.Count; i++)
        //     {
        //         if (unpairNode[i] == null)
        //             continue;
        //         var checkNode = unpairNode[i];
        //         var resultNode = new List<ANode>();
        //         checkNode.CheckResult(ref resultNode);
        //         if (resultNode.Count >= 3)
        //         {
        //             resultNode.ForEach(n => n.ChangeSprite());
        //             await Task.Delay(400);
        //             eliminateInfos.Add(GetEliminateResultAndEliminate(resultNode, unpairNode));
        //         }
        //     }
        //     return eliminateInfos;
        // }

        async Task PlayEliminateAnimation(Dictionary<EliminateInfo, List<ANode>> infos)
        {
            foreach (var o in infos)
            {
                DomainEvents.Raise<OnNodeEliminate>(new OnNodeEliminate(o.Key));
                o.Value.ForEach(node => node.Eliminate(true));
                await Task.Delay(300);
            }
            // foreach (var info,nodes in infos)
            // {
            //     DomainEvents.Raise<OnNodeEliminate>(new OnNodeEliminate(info));
            //     await Task.Delay(1000);
            // }
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
            var shuffleTypes = types.ToList().Shuffle();
            for (int i = 0; i < deactiveNodes.Count; i++)
            {
                isAnyNodeSpawn = true;
                Vector2Int point = deactiveNodes[i].Point;
                LeanPool.Despawn(deactiveNodes[i]);
                SpawnNode(point.x, point.y, shuffleTypes[i]);
                await Task.Delay(25);
            }
            return isAnyNodeSpawn;
        }

        EliminateInfo GetEliminateResultAndEliminate(List<ANode> resultNode, List<ANode> unpairNode, bool isFXPlay = true)
        {
            var eliminateInfo = new EliminateInfo();
            var type = resultNode[0].Type;
            foreach (var o in resultNode)
            {
                unpairNode[unpairNode.IndexOf(o)] = null;
                o.Eliminate(isFXPlay);
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


        EliminateInfo GetEliminateResultNotEliminate(List<ANode> resultNode, List<ANode> unpairNode, bool isFXPlay = true)
        {
            var eliminateInfo = new EliminateInfo();
            var type = resultNode[0].Type;
            foreach (var o in resultNode)
            {
                unpairNode[unpairNode.IndexOf(o)] = null;
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

        void SpawnNode(int x, int y, NodeType type)
        {

            var node = LeanPool.Spawn(nodePrefabs[(int)type], boardParent).GetComponent<ANode>();
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
                    //var chestType = Random.Range(0, System.Enum.GetNames(typeof(ChestType)).Length);
                    var chestType = ChestType.HP_RECOVER;
                    (node as ChestNode).Init(attr.ChestNodeAttr, (ChestType)chestType, point, (NodeType)type, this);
                    break;
            }
        }

    }
}
