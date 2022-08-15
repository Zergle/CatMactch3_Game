using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棋盘脚本
/// 设置游戏棋盘大小，生成棋盘，生成元素，消除元素
/// </summary>
public class Grid : MonoBehaviour
{
    #region 各种声明

    /*public AnimationClip NotMatchAnimation;*/

    ///游戏功能元素种类
    public enum BlockType
    {
        Normal,
        Obstacle,
        ColorClear, //清除同花
        RowClear, //整行清除
        ColClear, //整列清除
        Count, //用于计数
        Empty //空元素
    }

    //定义棋盘宽高
    public int xDim;
    public int yDim;

    //定义填充速度
    public float FillTime;

    //被点击的块pressedBlock和预交换的块enteredBlock
    private GameBlock pressedBlock;
    private GameBlock enteredBlock;

    //定义背景块
    public GameObject backgroundPrefab;

    //连接预制件和元素种类
    [System.Serializable]
    public struct BlockPrefab
    {
        public BlockType type;
        public GameObject prefab;
    }

    //字典blockPrefabDict存储BlockPrefab中预制件和元素种类的关系,元素类型是键，预制件是值
    private Dictionary<BlockType, GameObject> blockPrefabDict;

    //数组CatPrefabs存储BlockPrefab结构
    public BlockPrefab[] BlockPrefabs;

    //二维数组blocks承载元素
    private GameBlock[,] blocks;

    //引用Level，关卡信息
    public Level _Level;

    //判断游戏结束
    private bool gameOver = false;

    //判断游戏结束时是否继续填充
    private bool isFilling = false;
    public bool IsFilling
    {
        get { return isFilling; }
    }

    //获取每个元素的类型和位置
    [System.Serializable]
    public struct BlockPosition
    {
        public BlockType type; //类型
        public int x; //坐标x
        public int y; //坐标y
    }

    //定义关卡开始时固定生成的元素类型和位置InitialBlocks
    public BlockPosition[] InitialBlocks;

    //是否随机生成指定元素
    public bool RandomGenerate;

    //保存需要Find的对象
    private GameObject objBGs;
    private GameObject objBlocks;

    #endregion

    #region 方法们
    /// <summary>
    /// 游戏启动时生成棋盘，给棋盘添加背景和空白元素，随后生成障碍物，然后填充其他游戏元素
    /// </summary>
    void Awake()
    {
        //提前保存一些可能需要用到的元素
        objBGs = GameObject.Find("BGs");
        objBlocks = GameObject.Find("GameBlocks");

        blockPrefabDict = new Dictionary<BlockType, GameObject>();

        //遍历BlockPrefabs数组，将元素添加到字典
        for (int i = 0; i< BlockPrefabs.Length; i++)
        {
            if (!blockPrefabDict.ContainsKey(BlockPrefabs[i].type)) 
            {
                blockPrefabDict.Add(BlockPrefabs[i].type, BlockPrefabs[i].prefab);
            }
        }

        //遍历添加背景块生成游戏棋盘，分类到子元素BGs便于查看
        for(int x = 0; x<xDim; x++)
        {
            for(int y = 0; y<yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);

                background.transform.parent = objBGs.transform;

            }
        }

        //遍历棋盘，添加空元素EmptyBlock，生成关卡预设元素，然后Fill填充其他游戏元素
        blocks = new GameBlock[xDim, yDim];

        //在指定或随机位置生成关卡指定的元素类型
        for(int i = 0; i < InitialBlocks.Length; i++)
        {
            if (RandomGenerate)
            {
                System.Random initialPos = new System.Random();

                InitialBlocks[i].x = initialPos.Next(blocks.GetLength(0));

                InitialBlocks[i].y = initialPos.Next(blocks.GetLength(1));

                SpawnNewBlock(InitialBlocks[i].x, InitialBlocks[i].y, InitialBlocks[i].type);

            }else if (InitialBlocks[i].x >= 0 && InitialBlocks[i].x < xDim
                && InitialBlocks[i].y >= 0 && InitialBlocks[i].y < yDim)
            {
                SpawnNewBlock(InitialBlocks[i].x, InitialBlocks[i].y, InitialBlocks[i].type);

            }

        }

        //生成EmptyBlock
        for(int x = 0; x<xDim; x++)
        {
            for(int y = 0; y<yDim; y++)
            {
                if (blocks[x, y] == null)
                {
                    SpawnNewBlock(x, y, BlockType.Empty);
                }
            }
        }

        StartCoroutine(Fill());
    }

    /// <summary>
    /// 获取世界坐标
    /// x轴中心点-1/2宽+当前值，y轴换符号同理
    /// </summary>
    /// <param name="x">物体当前的x</param>
    /// <param name="y">物体当前的y</param>
    /// <returns>返回一个计算后的vector2世界坐标</returns>
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(

            transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y

            );
    }

    /// <summary>
    /// 生成EmptyBlock，设置为Grid子对象
    /// </summary>
    /// <param name="x">当前位置的x</param>
    /// <param name="y">当前位置的y</param>
    /// <param name="type">生成的Block类型</param>
    /// <returns>返回EmptyBlock</returns>
    public GameBlock SpawnNewBlock(int x, int y, BlockType type)
    {
        GameObject newBlock = (GameObject)Instantiate(blockPrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);

        newBlock.transform.parent = objBlocks.transform;

        blocks[x, y] = newBlock.GetComponent<GameBlock>();

        blocks[x, y].Init(x, y, this, type);

        return blocks[x, y];
    }

    /// <summary>
    /// 填充正常元素，在游戏启动时调用
    /// </summary>
    /// <returns>返回布尔值movedBlock，为真则已填充</returns>
    public bool FillStep()
    {
        bool movedBlock = false;

        //Y轴从倒数第二行开始遍历，最低行不需要向下移所以免去(y轴边界上没有元素，所以倒数第二行是-2)
        for(int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++) 
            {
                //保存当前遍历的位置
                int x = loopX;

                GameBlock block = blocks[x, y];

                if (block.IsMovable())
                {
                    //正下方的元素blockBelow
                    GameBlock blockBelow = blocks[x, y + 1];

                    #region 当正下方是EmptyBlock时垂直填充
                    if (blockBelow.Type == BlockType.Empty)
                    {
                        Destroy(blockBelow.gameObject);

                        block.MovableComponent.Move(x, y + 1, FillTime);

                        blocks[x, y + 1] = block;

                        SpawnNewBlock(x, y, BlockType.Empty);
                        
                        movedBlock = true;
                    }
                    #endregion

                    #region 如果不是则斜向填充
                    else
                    {
                        //判断斜下落和执行斜下落的部分，diag是斜率，diag为-1则向左下方掉落，为1则向右下方，为0则返回垂直下落部分
                        for(int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                //需要填补的空缺位diagX
                                int diagX = x + diag;

                                //确保斜下方在棋盘内
                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GameBlock diagonalBlock = blocks[diagX, y + 1];

                                    if (diagonalBlock.Type == BlockType.Empty)
                                    {
                                        //检查斜下方存在空缺的时候确认空缺上方是否可以下落
                                        bool hasBlockAbove = true;

                                        for(int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameBlock blockAbove = blocks[diagX, aboveY];

                                            if (blockAbove.IsMovable())
                                            {
                                                break;

                                            }else if (!blockAbove.IsMovable() && blockAbove.Type != BlockType.Empty)
                                            {
                                                hasBlockAbove = false;
                                                
                                                break;
                                            }
                                        }
                                        if (!hasBlockAbove)
                                        {
                                            Destroy(diagonalBlock.gameObject);

                                            block.MovableComponent.Move(diagX, y + 1, FillTime);

                                            blocks[diagX, y + 1] = block;

                                            SpawnNewBlock(x, y, BlockType.Empty);

                                            movedBlock = true;

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        for(int x = 0; x < xDim; x++)
        {
            GameBlock BlockBelow = blocks[x, 0];

            if (BlockBelow.Type == BlockType.Empty)
            {
                Destroy(BlockBelow.gameObject);

                GameObject newBlock = (GameObject)Instantiate(blockPrefabDict[BlockType.Normal], GetWorldPosition(x, -1), Quaternion.identity);

                newBlock.transform.parent = objBlocks.transform;

                blocks[x, 0] = newBlock.GetComponent<GameBlock>();

                blocks[x, 0].Init(x, -1, this, BlockType.Normal);

                blocks[x, 0].MovableComponent.Move(x, 0, FillTime);

                blocks[x, 0].ColorComponent.SetColor((ColorBlock.ColorType)UnityEngine.Random.Range(0, blocks[x, 0].ColorComponent.NumColors));

                movedBlock = true;
            }    
        }

        return movedBlock;
    }

    /// <summary>
    /// 填充动画
    /// </summary>
    /// <returns>调用时执行协程</returns>
    public IEnumerator Fill()
    {
        bool needsRefill = true;

        isFilling = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(FillTime);

            while (FillStep())
            {
                yield return new WaitForSeconds(FillTime);
            }
            needsRefill = ClearValidMatches();
        }

        isFilling = false;
    }

    #region 三个鼠标状态事件
    /// <summary>
    /// PressBlock 参数是被点击的block
    /// EnterBlock 参数是鼠标悬停的block，
    /// ReleaseBlock 鼠标释放的时候对pressedBlock和enteredBlock进行比对和交换
    /// </summary>
    /// <param name="block">鼠标点击/悬停/释放时对应的block</param>

    public void PressBlock(GameBlock block)
    {
        pressedBlock = block;
    }

    public void EnterBlock(GameBlock block)
    {
        enteredBlock = block;
    }

    public void ReleaseBlock()
    {
        if (IsAdjacent(pressedBlock, enteredBlock))
        {
            SwapBlocks(pressedBlock, enteredBlock);
        }

    }
    #endregion 

    /// <summary>
    /// 检查元素间是否相邻，判断两元素是否同轴且另一轴差值为1
    /// </summary>
    /// <param name="block1">被选中的block1</param>
    /// <param name="block2">被选中的block2</param>
    /// <returns>返回计算后的值，为真则相邻</returns>
    public bool IsAdjacent(GameBlock block1, GameBlock block2)
    {
        return (block1.X == block2.X && (int)Mathf.Abs(block1.Y - block2.Y) == 1)
            || (block1.Y == block2.Y && (int)Mathf.Abs(block1.X - block2.X) == 1);
    }

    /// <summary>
    /// 调换元素
    /// 两元素可移动且花纹匹配，则调换，然后调用ClearValidMatches()消除
    /// </summary>
    /// <param name="block1">被选中的block1</param>
    /// <param name="block2">被选中的block2</param>
    public void SwapBlocks(GameBlock block1, GameBlock block2)
    {
        if (gameOver)
        {
            return;
        }

        if (block1.IsMovable() && block2.IsMovable())
        {
            //给两个元素在数组里分配位置（交换后的）
            blocks[block1.X, block1.Y] = block2;
            blocks[block2.X, block2.Y] = block1;

            //调用GetMatch进行匹配，参数是元素和预计坐标，如果存在Bonus则直接下一步
            if (GetMatch(block1, block2.X, block2.Y) != null || GetMatch(block2, block1.X, block1.Y) != null
                || block1.Type == BlockType.ColorClear || block2.Type == BlockType.ColorClear)  
            {
                //保存cat1的原坐标
                int block1X = block1.X;
                int block1Y = block1.Y;

                //交换
                block1.MovableComponent.Move(block2.X, block2.Y, FillTime);
                block2.MovableComponent.Move(block1X, block1Y, FillTime);

                #region ColorClear的清除功能

                //如果两个block中存在一个特殊元素，并且另一个元素不为空，调用ClearBlock()
                if (block1.Type == BlockType.ColorClear && block1.IsClearable() && block2.IsColored()) 
                {
                    //colorClear保存block2的颜色
                    ColorClear colorClear = block1.GetComponent<ColorClear>();

                    if (colorClear)
                    {
                        colorClear.BonusColor = block2.ColorComponent.Color;
                    }

                    ClearBlock(block1.X, block1.Y);
                }

                if (block2.Type == BlockType.ColorClear && block2.IsClearable() && block1.IsColored())
                {
                    //colorClear保存block1的颜色
                    ColorClear colorClear = block2.GetComponent<ColorClear>();

                    if (colorClear)
                    {
                        colorClear.BonusColor = block1.ColorComponent.Color;
                    }

                    ClearBlock(block2.X, block2.Y);
                }

                #endregion 

                ClearValidMatches();

                #region Row/ColClear的清除功能

                if (block1.Type == BlockType.RowClear || block1.Type == BlockType.ColClear)
                {
                    ClearBlock(block1.X, block1.Y);
                }

                if (block2.Type == BlockType.RowClear || block2.Type == BlockType.ColClear)
                {
                    ClearBlock(block2.X, block2.Y);
                }

                #endregion

                //滑动后，将选项归零
                pressedBlock = null;
                enteredBlock = null;

                StartCoroutine(Fill());

                //在每次移动时，计数
                _Level.OnMove();
            }
            else
            {
                blocks[block1.X, block1.Y] = block1;
                blocks[block2.X, block2.Y] = block2;

                //调用动画
                block1.ClearableComponent.CantClear(block1);
                block2.ClearableComponent.CantClear(block2);
            }
        }
    }

    /// <summary>
    /// 匹配方法
    /// </summary>
    /// <param name="block">需要匹配的元素block</param>
    /// <param name="newX">预计转移的新坐标newX</param>
    /// <param name="newY">预计转移的新坐标newY</param>
    /// <returns>返回匹配的元素列表，不匹配返回空</returns>
    public List<GameBlock> GetMatch(GameBlock block,int newX,int newY)
    {
        if (block.IsColored())
        {
            //获取block的花色类型
            ColorBlock.ColorType color = block.ColorComponent.Color;
            
            //行、列和终极匹配遍历列表
            List<GameBlock> horizontalBlocks = new List<GameBlock>();
            List<GameBlock> verticalBlocks = new List<GameBlock>();
            List<GameBlock> matchingBlocks = new List<GameBlock>();

            #region 水平方向检测

            //水平方向遍历，将当前元素添加到列表作为基准点
            horizontalBlocks.Add(block);

            //dir是方向，为0时向左，为1时向右，左一遍右一遍，全部遍历完毕
            for(int dir = 0; dir <= 1; dir++)
            {
                //xOffeset是对基准坐标的偏离距离，依次向左或右，直到边界
                for(int xOffeset = 1; xOffeset < xDim; xOffeset++)
                {
                    //当前检查到的位置
                    int x;

                    //方向向左
                    if(dir == 0)
                    {
                        x = newX - xOffeset;
                    }
                    //向右
                    else
                    {
                        x = newX + xOffeset;
                    }

                    //超出边界停止遍历
                    if (x < 0 || x >= xDim)
                    {
                        break;
                    }

                    //最终检查结果x，行遍历中newY不变，此坐标元素颜色匹配则加入到列表中
                    if (blocks[x, newY].IsColored() && blocks[x, newY].ColorComponent.Color == color)
                    {
                        horizontalBlocks.Add(blocks[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //检查匹配的数量，达到条件的全部加到潜力股matchingCats列表中
            if(horizontalBlocks.Count >= 3)
            {
                for(int i = 0; i < horizontalBlocks.Count; i++)
                {
                    matchingBlocks.Add(horizontalBlocks[i]);
                }
            }

            //如果水平方向上符合条件的话，添加一个垂直检查，实现L,T型联合消除
            #region L和T型匹配

            if (horizontalBlocks.Count >= 3)
            {
                for(int i = 0; i < horizontalBlocks.Count; i++)
                {
                    for(int dir = 0; dir <= 1; dir++)
                    {
                        for(int yOffeset = 1; yOffeset < yDim; yOffeset++)
                        {
                            int y;

                            if (dir == 0)
                            {//向上
                                y = newY - yOffeset;
                            }
                            else
                            {//向下
                                y = newY + yOffeset;
                            }
                            if (y < 0 || y >= yDim)
                            {//超界
                                break;
                            }
                            //符合条件加到垂直匹配列表
                            if (blocks[horizontalBlocks[i].X, y].IsColored() && blocks[horizontalBlocks[i].X, y].ColorComponent.Color == color) 
                            {
                                verticalBlocks.Add(blocks[horizontalBlocks[i].X, y]);
                            }
                            else
                            {
                                break;
                            }

                        }
                    }

                    if(verticalBlocks.Count < 2)
                    {
                        verticalBlocks.Clear();
                    }
                    else
                    {
                        for(int j = 0; j < verticalBlocks.Count; j++)
                        {
                            matchingBlocks.Add(verticalBlocks[j]);
                        }

                        break;
                    }
                }
            }

            #endregion

            //如果在潜力股中达到条件，就将列表返回
            if (matchingBlocks.Count >= 3)
            {
                return matchingBlocks;
            }

            #endregion 水平方向检测结束

            //先清除上一轮检查结果
            horizontalBlocks.Clear();
            verticalBlocks.Clear();

            #region 垂直方向检测

            //垂直方向检测
            verticalBlocks.Add(block);
            //dir是方向，为0时向上，为1时向下，上一遍下一遍，全部遍历完毕
            for (int dir = 0; dir <= 1; dir++)
            {
                //yoffeset是相邻块到中心点的距离，从最近的开始直到边界
                for (int yOffeset = 1; yOffeset < yDim; yOffeset++)
                {
                    //当前检查到的位置
                    int y;

                    //方向向上
                    if (dir == 0)
                    {
                        y = newY - yOffeset;
                    }
                    //向下
                    else
                    {
                        y = newY + yOffeset;
                    }

                    //超出边界停止遍历
                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }

                    //最终检查结果
                    if (blocks[newX, y].IsColored() && blocks[newX, y].ColorComponent.Color == color)
                    {
                        verticalBlocks.Add(blocks[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //检查匹配的数量，达到条件的全部加到潜力股matchingBlockss列表中
            if (verticalBlocks.Count >= 3)
            {
                for (int i = 0; i < verticalBlocks.Count; i++)
                {
                    matchingBlocks.Add(verticalBlocks[i]);
                }
            }

            //如果垂直方向上符合条件的话，添加一个水平检查，实现L,T型联合消除
            #region L和T型检查

            if (verticalBlocks.Count >= 3)
            {
                for (int i = 0; i < verticalBlocks.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffeset = 1; xOffeset < xDim; xOffeset++)
                        {
                            int x;

                            if (dir == 0)
                            {//向左
                                x = newX - xOffeset;
                            }
                            else
                            {//向右
                                x = newX + xOffeset;
                            }
                            if (x < 0 || x >= xDim)
                            {//超界
                                break;
                            }
                            //符合条件加到垂直匹配列表
                            if (blocks[x, verticalBlocks[i].Y].IsColored() && blocks[x, verticalBlocks[i].Y].ColorComponent.Color == color) 
                            {
                                horizontalBlocks.Add(blocks[x, verticalBlocks[i].Y]);
                            }
                            else
                            {
                                break;
                            }

                        }
                    }

                    if (horizontalBlocks.Count < 2)
                    {
                        horizontalBlocks.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalBlocks.Count; j++)
                        {
                            matchingBlocks.Add(horizontalBlocks[j]);
                        }

                        break;
                    }
                }
            }

            #endregion L和T型检测结束

            //如果在潜力股中达到条件，就将列表返回
            if (matchingBlocks.Count >= 3)
            {
                return matchingBlocks;
            }

            #endregion 垂直方向检测结束

        }

        //都不符合，返回空
        return null;
    }

    /// <summary>
    /// 清除棋盘上达到3个的匹配元素，如果匹配数量达到4个及以上生成相应的特殊元素
    /// </summary>
    /// <returns>返回布尔值needsRefill，为真则匹配元素已消除，需要填充</returns>
    public bool ClearValidMatches()
    {
        bool needsRefill = false;

        for(int y = 0; y < yDim; y++)
        {
            for(int x = 0; x < xDim; x++)
            {
                //遍历棋盘，定义match列表，把所有block送去匹配，参数是遍历到的block和坐标
                if (blocks[x, y].IsClearable())
                {
                    List<GameBlock> match = GetMatch(blocks[x, y], x, y);

                    //存在匹配列表，使用Count类型定义一个specialBlockType进行统计
                    if(match != null)
                    {
                        BlockType specialBlockType = BlockType.Count;

                        //从match列表抽取坐标保存到specialBlockX,Y，在条件符合的时候将它生成为特殊元素
                        GameBlock randomBlock = match[Random.Range(0, match.Count)];

                        int specialBlockX = randomBlock.X;
                        int specialBlockY = randomBlock.Y;

                        //匹配数量为4，生成RowClear,ColClear
                        if(match.Count == 4)
                        {
                            //如果是随机消除的，specialBlockType为随机一种
                            if (pressedBlock == null || enteredBlock == null)
                            {
                                specialBlockType = (BlockType)Random.Range((int)BlockType.RowClear, (int)BlockType.ColClear);

                            }else if (pressedBlock.Y == enteredBlock.Y)
                            {//同行互换消除specialBlockType为RowClear
                                specialBlockType = BlockType.RowClear;
                            }
                            else
                            {//同列互换消除specialBlockType为ColClear
                                specialBlockType = BlockType.ColClear;
                            }
                        }
                        //匹配数量大等于5,specialBlockType为ColorClear
                        else if (match.Count >= 5)
                        {
                            specialBlockType = BlockType.ColorClear;
                        }

                        //match中的元素消除之后填充
                        for(int i = 0; i < match.Count; i++)
                        {
                            if (ClearBlock(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                                //如果match中存在被交换过的元素，坐标保存到specialBlock
                                if (match[i] == pressedBlock || match[i] == enteredBlock)
                                {
                                    specialBlockX = match[i].X;
                                    specialBlockY = match[i].Y;
                                }
                            }
                        }
                        //获取对应Type之后，生成对应类型的specialBlock并上色
                        if (specialBlockType != BlockType.Count)
                        {
                            Destroy(blocks[specialBlockX, specialBlockY]);

                            GameBlock newBlock = SpawnNewBlock(specialBlockX, specialBlockY, specialBlockType);

                            if ((specialBlockType == BlockType.RowClear || specialBlockType == BlockType.ColClear 
                                && newBlock.IsClearable() && match[0].IsColored()))
                            {
                                newBlock.ColorComponent.SetColor(match[0].ColorComponent.Color);

                            }else if(specialBlockType == BlockType.ColorClear && newBlock.IsColored())
                            {
                                newBlock.ColorComponent.SetColor(ColorBlock.ColorType.Any);
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }

    /// <summary>
    /// 清除指定元素，在原位置生成新的元素
    /// </summary>
    /// <param name="x">需要清除的元素的x</param>
    /// <param name="y">需要清除的元素的y</param>
    /// <returns>返回布尔值，为真则指定元素已清除</returns>
    public bool ClearBlock(int x, int y)
    {
        if (blocks[x, y].IsClearable() && !blocks[x, y].ClearableComponent.IsBeingCleared) 
        {
            blocks[x, y].ClearableComponent.Clear();
            
            SpawnNewBlock(x, y, BlockType.Empty);
            
            ClearObstacles(x, y);
            
            return true;
        
        }
        
        return false;
        
    }

    /// <summary>
    /// 清除障碍物Obstacle
    /// 如果参数坐标相邻位置存在Obstale，则清除Obstacle生成EmptyBlock
    /// </summary>
    /// <param name="x">被清除的元素的坐标x</param>
    /// <param name="y">被清除的元素的坐标y</param>
    public void ClearObstacles(int x,int y)
    {
        for(int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (blocks[adjacentX, y].Type == BlockType.Obstacle && blocks[adjacentX, y].IsClearable())  
                {
                    blocks[adjacentX, y].ClearableComponent.Clear();

                    SpawnNewBlock(adjacentX, y, BlockType.Empty);
                }
            }
        }

        for(int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if(adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if (blocks[x, adjacentY].Type == BlockType.Obstacle && blocks[x, adjacentY].IsClearable()) 
                {
                    blocks[x, adjacentY].ClearableComponent.Clear();

                    SpawnNewBlock(x,adjacentY, BlockType.Empty);
                }
            }
        }
    }

    /// <summary>
    /// 清除行，遍历x轴调用ClearBlock()清除整行
    /// </summary>
    /// <param name="row">y值，定位行</param>
    public void ClearRow(int row)
    {
        for(int x= 0; x < xDim; x++)
        {
            ClearBlock(x, row);
        }
    }

    /// <summary>
    /// 清除列，遍历y轴调用ClearCat()清除整列
    /// </summary>
    /// <param name="col">x值，定位该列</param>
    public void ClearCol(int col)
    {
        for(int y= 0; y < yDim; y++)
        {
            ClearBlock(col, y);
        }
    }

    /// <summary>
    /// 清除同花色
    /// 遍历棋盘，如果遍历到的元素颜色和bonusColor相同则消除
    /// 选中的花色也是ColorClear则遍历消除整个棋盘元素
    /// </summary>
    /// <param name="bonusColor">与ColorClear发生交换的元素花色</param>
    public void ClearColor(ColorBlock.ColorType bonusColor)
    {
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0;y < yDim; y++)
            {
                if (blocks[x, y].IsColored() && (blocks[x, y].ColorComponent.Color == bonusColor)
                    || bonusColor == ColorBlock.ColorType.Any) 
                {
                    ClearBlock(x, y);
                }
            }
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        gameOver = true;
    }

    /// <summary>
    /// 获取指定元素数量，逐个添加到列表
    /// 遍历棋盘，将每个元素的类型进行比对，符合条件添加到列表catsOfType
    /// </summary>
    /// <param name="type">需要比对的元素类型</param>
    /// <returns>返回比对后符合条件的元素列表</returns>
    public List<GameBlock> GetNumOfTypes(BlockType type)
    {
        List<GameBlock> numOfType = new List<GameBlock>();

        for(int x = 0; x < xDim; x++)
        {
            for(int y= 0;y<yDim; y++)
            {
                if (blocks[x, y].Type == type)
                {
                    numOfType.Add(blocks[x, y]);
                }
            }
        }

        return numOfType;
    }

    #endregion
}
