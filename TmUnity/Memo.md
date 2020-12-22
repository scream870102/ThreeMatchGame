## Asset Use
- [Halloween Graphics](https://finalbossblues.itch.io/halloween-graphics)  
- [Pixel Dark forest](https://szadiart.itch.io/pixel-dark-forest)  
- [Pixel Effects Pack](https://codemanu.itch.io/pixelart-effect-pack)  
- [Free Pixel Art FX Spirit X Witch](https://ppeldo.itch.io/2d-pixel-art-game-spellmagic-fx)  
- [Fire fx](https://xyezawr.itch.io/free)  
- [Skeleton Seeker](https://eddies-workshop.itch.io/seeker)
- [Kyries's Free 16*16](https://kyrise.itch.io/kyrises-free-16x16-rpg-icon-pack)
- [Weiholmir](https://justfredrik.itch.io/weiholmir)
- [Silver](https://poppyworks.itch.io/silver)
- [Skill Icon Set](https://quintino-pixels.itch.io/free-pixel-art-skill-icons-pack)

## Memo
//TODO:

- 數值平衡
- 語言跟字體
- Combo加成
- 最高分系統
- 音效






//FIXME:
- Player Star Animation Pos Calc
- Node Eliminate Animation
- Node Attack Anim
- if add new node shape is square will keep the same result

//ATTEND:
- ScreenToPos Anchored is at top left.
- The event position get from drag event should add AspectOffset to make the anchor at the middle of picture.
- Ancored Postion is local base.Position is global based.
- Can't Transfer to a new state at state init method
- Chest Type now only have hp recover


//FIN:
- 敵人的新招式
- 頁面的基準(添滿寬度為主 高度不跟著變動)
- 定位調整
- 落下動畫
- 沒消珠的BUG
- 敵人的攻擊
- 動畫
    - Player
        - **Node Eliminate** MagicBarrier
        - **Hit** FireBall2
        - **Heal** HolyExplosion
        - **Dead** FireCast
    - Enemy
        - **Hit**
            - Poison Claw
            - Ice Cast
        - **Dead** Explosion 3
- 生成動畫
- UI美術調整
- 主選單
- 頁面跳轉
- 最高分系統


//TMP
ANODE
```csharp
async public Task MoveToPointAsync(Vector2Int newPoint)
{

    var controlPoints = new List<Vector3>();
    controlPoints.Add(controller.PointToAnchoredPos(Point));
    // find direction first 
    bool isHori = newPoint.y == Point.y;
    // if is hori move  
    if (isHori)
    {
        var yOffset = (newPoint.x - Point.x) * .3f;
        controlPoints.Add(controller.PointToAnchoredPos(new Vector2(Point.x, Point.y + yOffset)));
        controlPoints.Add(controller.PointToAnchoredPos(new Vector2(newPoint.x, newPoint.x + yOffset)));
    }
    // if is vert move
    else
    {
        var xOffset = (newPoint.y - Point.y) * .3f;
        controlPoints.Add(controller.PointToAnchoredPos(new Vector2(Point.x + xOffset, Point.y)));
        controlPoints.Add(controller.PointToAnchoredPos(new Vector2(newPoint.x + xOffset, newPoint.y)));
    }
    controlPoints.Add(controller.PointToAnchoredPos(newPoint));
    //Get Bezier
    Bezier b = new Bezier(controlPoints.ToArray(), .1f);
    var points = b.GetCurvesPoint();
    //Move in 0.1sec
    foreach (var p in points)
    {
        RectTransform.anchoredPosition = p;
        await Task.Delay(1);
    }
    Point = newPoint;
}
```

NODE CONTROLER
```csharp
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
        // }
```


