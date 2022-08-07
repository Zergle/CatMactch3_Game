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

    //定义游戏元素种类
    public enum CatType
    {
        Normal,
        Dog,
        Paw, //清除同花
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

    //定义随机狗的数量
    public int NumRandomDogs;

    //判断元素能否斜着走
    private bool inverse = false;

    //定义被点击的块和预交换的块
    private GameCat pressedCat;
    private GameCat enteredCat;

    //定义背景块
    public GameObject backgroundPrefab;

    //定义结构CatPrefab用来连接预制件和元素种类
    [System.Serializable]
    public struct CatPrefab
    {
        public CatType type;
        public GameObject prefab;
    }

    //定义字典catPrefabDict用来存储CatPrefab中预制件和元素种类的关系,CatType键，预制件是值
    private Dictionary<CatType, GameObject> catPrefabDict;

    //定义数组CatPrefabs存储结构
    public CatPrefab[] CatPrefabs;

    //定义二维数组cats承载元素，修改了权限，原本是private
    private GameCat[,] cats;

    //引用level
    public Level _Level;

    //游戏结束
    private bool gameOver = false;

    //获取每个元素的位置和类型
    [System.Serializable]
    public struct CatPosition
    {
        public CatType type;
        public int x;
        public int y;
    }

    //定义关卡开始时固定生成的元素类型和位置initialCats
    public CatPosition[] initialCats;

    #endregion

    /// <summary>
    /// 游戏启动时生成棋盘，给棋盘添加背景和空白元素，随后生成障碍物，然后填充其他游戏元素
    /// </summary>
    void Awake()
    {
        catPrefabDict = new Dictionary<CatType, GameObject>();
        //遍历CatPrefabs数组，添加到字典
        for (int i = 0; i< CatPrefabs.Length; i++)
        {
            if (!catPrefabDict.ContainsKey(CatPrefabs[i].type))
            {
                catPrefabDict.Add(CatPrefabs[i].type, CatPrefabs[i].prefab);
            }
        }

        //遍历棋盘,添加背景块
        for(int x = 0; x<xDim; x++)
        {
            for(int y = 0; y<yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);

                //设置为网格的子对象，这里和原工程有出入，多设置了一个子对象进行分类防止出问题，先注释一下
                /*background.transform.parent = GameObject.Find("BGs").transform;*/
                background.transform.parent = transform;
            }
        }

        //遍历棋盘，添加EmptyCat，生成Dog，然后Fill填充其他游戏元素
        cats = new GameCat[xDim, yDim];

        //在指定位置生成关卡指定的元素类型
        for(int i = 0; i < initialCats.Length; i++)
        {
            if (initialCats[i].x >= 0 && initialCats[i].x < xDim 
                && initialCats[i].y >= 0 && initialCats[i].y < yDim) 
            {
                SpawnNewCat(initialCats[i].x, initialCats[i].y, initialCats[i].type);
            }
        }

        //生成EmptyCat
        for(int x = 0; x<xDim; x++)
        {
            for(int y = 0; y<yDim; y++)
            {
                if (cats[x, y] == null)
                {
                    SpawnNewCat(x, y, CatType.Empty);
                }
            }
        }

        //如果NumRandomDogs大于0，调用随机dog生成
        if(NumRandomDogs > 0) { 
            for (int x = 0; x < NumRandomDogs; x++)
            {
                DogSpawner();
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
    /// 生成EmptyCat，设置为Grid子对象
    /// </summary>
    /// <param name="x">当前位置的x</param>
    /// <param name="y">当前位置的y</param>
    /// <param name="type">生成的Cat类型</param>
    /// <returns>在x,y位置返回EmptyCat</returns>
    public GameCat SpawnNewCat(int x, int y, CatType type)
    {
        GameObject newCat = (GameObject)Instantiate(catPrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);

        /*newCat.transform.parent = GameObject.Find("Cats").transform;*/
        newCat.transform.parent = transform;

        cats[x, y] = newCat.GetComponent<GameCat>();

        cats[x, y].Init(x, y, this, type);

        return cats[x, y];
    }

    /// <summary>
    /// 填充正常元素，在游戏启动时调用
    /// </summary>
    /// <returns>返回布尔值movedCat到Fill协程，为真则顺利填充完毕</returns>
    public bool FillStep()
    {
        bool movedCat = false;

        for(int y = yDim - 2; y >= 0; y--)
        {
            for(int loopX = 0; loopX<xDim; loopX++)
            {
                int x = loopX;

                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }

                GameCat cat = cats[x, y];

                if (cat.IsMovable())
                {
                    GameCat catBelow = cats[x, y + 1];

                    if (catBelow.Type == CatType.Empty)
                    {
                        Destroy(catBelow.gameObject);

                        cat.MovableComponent.Move(x, y + 1, FillTime);

                        cats[x, y + 1] = cat;

                        SpawnNewCat(x, y, CatType.Empty);
                        
                        movedCat = true;
                    }
                    else
                    {
                        //判断斜下落和执行斜下落的部分，diag是斜率，diag为-1则向左下方掉落，为1则向右下方
                        for(int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GameCat diagonalCat = cats[diagX, y + 1];

                                    if (diagonalCat.Type == CatType.Empty)
                                    {
                                        bool hasCatAbove = true;

                                        for(int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameCat catAbove = cats[diagX, aboveY];

                                            if (catAbove.IsMovable())
                                            {
                                                break;
                                            }else if (!catAbove.IsMovable() && catAbove.Type != CatType.Empty)
                                            {
                                                hasCatAbove = false;
                                                
                                                break;
                                            }
                                        }
                                        if (!hasCatAbove)
                                        {
                                            Destroy(diagonalCat.gameObject);

                                            cat.MovableComponent.Move(diagX, y + 1, FillTime);

                                            cats[diagX, y + 1] = cat;

                                            SpawnNewCat(x, y, CatType.Empty);

                                            movedCat = true;

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for(int x = 0; x < xDim; x++)
        {
            GameCat catBelow = cats[x, 0];

            if (catBelow.Type == CatType.Empty)
            {
                Destroy(catBelow.gameObject);

                GameObject newCat = (GameObject)Instantiate(catPrefabDict[CatType.Normal], GetWorldPosition(x, -1), Quaternion.identity);

                /*newCat.transform.parent = GameObject.Find("Cats").transform;*/
                newCat.transform.parent = transform;

                cats[x, 0] = newCat.GetComponent<GameCat>();

                cats[x, 0].Init(x, -1, this, CatType.Normal);

                cats[x, 0].MovableComponent.Move(x, 0, FillTime);

                cats[x, 0].ColorComponent.SetColor((ColorCat.ColorType)UnityEngine.Random.Range(0, cats[x, 0].ColorComponent.NumColors));

                movedCat = true;
            }    
        }

        return movedCat;
    }

    /// <summary>
    /// 填充动画
    /// </summary>
    /// <returns>调用时执行协程</returns>
    public IEnumerator Fill()
    {
        bool needsRefill = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(FillTime);

            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(FillTime);
            }
            needsRefill = ClearValidMatches();
        }
    }

    /// <summary>
    /// 随机dog生成器
    /// 从cats里随机抽取坐标生成Dog
    /// </summary>
    public void DogSpawner()
    {
        System.Random dogPos = new System.Random();

        int dogX = dogPos.Next(cats.GetLength(0));

        int dogY = dogPos.Next(cats.GetLength(1));

        Destroy(cats[dogX, dogY].gameObject);

        SpawnNewCat(dogX, dogY, CatType.Dog);
    }

    #region 三个鼠标状态事件
    /// <summary>
    /// PressCat 参数是被点击的cat
    /// EnterCat 参数是鼠标悬停的cat，
    /// ReleaseCat 鼠标释放的时候对pressedCat和enteredCat进行比对和交换
    /// </summary>
    /// <param name="cat"></param>

    public void PressCat(GameCat cat)
    {
        pressedCat = cat;
    }

    public void EnterCat(GameCat cat)
    {
        enteredCat = cat;
    }

    public void ReleaseCat()
    {
        if (IsAdjacent(pressedCat, enteredCat))
        {
            SwapCats(pressedCat, enteredCat);
        }

    }
    #endregion 三个鼠标状态事件

    /// <summary>
    /// 检查元素间是否相邻，判断两元素是否同轴且另一轴差值为1
    /// </summary>
    /// <param name="cat1">被选中的cat1</param>
    /// <param name="cat2">被选中的cat2</param>
    /// <returns>返回计算后的值，为真则相邻</returns>
    public bool IsAdjacent(GameCat cat1, GameCat cat2)
    {
        return (cat1.X == cat2.X && (int)Mathf.Abs(cat1.Y - cat2.Y) == 1)
            || (cat1.Y == cat2.Y && (int)Mathf.Abs(cat1.X - cat2.X) == 1);
    }

    /// <summary>
    /// 调换元素
    /// 两元素可移动且花纹匹配，则调换，然后调用ClearValidMatches()消除
    /// </summary>
    /// <param name="cat1">被选中的cat1</param>
    /// <param name="cat2">被选中的cat2</param>
    public void SwapCats(GameCat cat1, GameCat cat2)
    {
        if (gameOver)
        {
            return;
        }

        if (cat1.IsMovable() && cat2.IsMovable())
        {
            //给两个元素在数组里分配位置（交换后的）
            cats[cat1.X, cat1.Y] = cat2;
            cats[cat2.X, cat2.Y] = cat1;

            //调用GetMatch进行匹配，参数是元素和预计坐标，如果存在Paw则直接下一步
            if (GetMatch(cat1, cat2.X, cat2.Y) != null || GetMatch(cat2, cat1.X, cat1.Y) != null
                || cat1.Type == CatType.Paw || cat2.Type == CatType.Paw)  
            {
                //保存cat1的原坐标
                int cat1X = cat1.X;
                int cat1Y = cat1.Y;

                //交换
                cat1.MovableComponent.Move(cat2.X, cat2.Y, FillTime);
                cat2.MovableComponent.Move(cat1X, cat1Y, FillTime);

                #region Paw清除
                if (cat1.Type == CatType.Paw && cat1.IsClearable() && cat2.IsColored()) 
                {
                    //声明colorClear将cat2的颜色保存到Paw
                    ColorClear colorClear = cat1.GetComponent<ColorClear>();

                    if (colorClear)
                    {
                        colorClear.PawColor = cat2.ColorComponent.Color;
                    }

                    ClearCat(cat1.X, cat1.Y);
                }

                if (cat2.Type == CatType.Paw && cat2.IsClearable() && cat1.IsColored())
                {
                    //声明colorClear将cat2的颜色保存到Paw
                    ColorClear colorClear = cat2.GetComponent<ColorClear>();

                    if (colorClear)
                    {
                        colorClear.PawColor = cat1.ColorComponent.Color;
                    }

                    ClearCat(cat2.X, cat2.Y);
                }

                #endregion Paw清除结束

                ClearValidMatches();

                //当交换特殊元素时，涉及到的元素全部清除
                if (cat1.Type == CatType.RowClear || cat1.Type == CatType.ColClear)
                {
                    ClearCat(cat1.X, cat1.Y);
                }

                if (cat2.Type == CatType.RowClear || cat2.Type == CatType.ColClear)
                {
                    ClearCat(cat2.X, cat2.Y);
                }

                //滑动后，将选项归零
                pressedCat = null;
                enteredCat = null;

                StartCoroutine(Fill());

                //在每次移动时，计数
                _Level.OnMove();
            }
            else
            {
                cats[cat1.X, cat1.Y] = cat1;
                cats[cat2.X, cat2.Y] = cat2;
            }
        }
    }

    /// <summary>
    /// 匹配方法
    /// </summary>
    /// <param name="cat">需要匹配的元素cat</param>
    /// <param name="newX">预计转移的新坐标newX</param>
    /// <param name="newY">预计转移的新坐标newY</param>
    /// <returns>返回匹配的元素列表，不匹配返回空</returns>
    public List<GameCat> GetMatch(GameCat cat,int newX,int newY)
    {
        if (cat.IsColored())
        {
            //获取cat的颜色类型
            ColorCat.ColorType color = cat.ColorComponent.Color;
            
            //行、列和终极匹配遍历列表
            List<GameCat> horizontalCats = new List<GameCat>();
            List<GameCat> verticalCats = new List<GameCat>();
            List<GameCat> matchingCats = new List<GameCat>();

            #region 水平方向检测

            //水平方向遍历，将当前元素添加到列表作为基准点
            horizontalCats.Add(cat);

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
                    if (cats[x, newY].IsColored() && cats[x, newY].ColorComponent.Color == color)
                    {
                        horizontalCats.Add(cats[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //检查匹配的数量，达到条件的全部加到潜力股matchingCats列表中
            if(horizontalCats.Count >= 3)
            {
                for(int i = 0; i < horizontalCats.Count; i++)
                {
                    matchingCats.Add(horizontalCats[i]);
                }
            }

            //如果水平方向上符合条件的话，添加一个垂直检查，实现L,T型联合消除
            #region L和T型匹配

            if (horizontalCats.Count >= 3)
            {
                for(int i = 0; i < horizontalCats.Count; i++)
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
                            if (cats[horizontalCats[i].X, y].IsColored() && cats[horizontalCats[i].X, y].ColorComponent.Color == color) 
                            {
                                verticalCats.Add(cats[horizontalCats[i].X, y]);
                            }
                            else
                            {
                                break;
                            }

                        }
                    }

                    if(verticalCats.Count < 2)
                    {
                        verticalCats.Clear();
                    }
                    else
                    {
                        for(int j = 0; j < verticalCats.Count; j++)
                        {
                            matchingCats.Add(verticalCats[j]);
                        }

                        break;
                    }
                }
            }

            #endregion

            //如果在潜力股中达到条件，就将列表返回
            if (matchingCats.Count >= 3)
            {
                return matchingCats;
            }

            #endregion 水平方向检测结束

            //先清除上一轮检查结果
            horizontalCats.Clear();
            verticalCats.Clear();

            #region 垂直方向检测
            //垂直方向检测
            verticalCats.Add(cat);
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
                    if (cats[newX, y].IsColored() && cats[newX, y].ColorComponent.Color == color)
                    {
                        verticalCats.Add(cats[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //检查匹配的数量，达到条件的全部加到潜力股matchingCats列表中
            if (verticalCats.Count >= 3)
            {
                for (int i = 0; i < verticalCats.Count; i++)
                {
                    matchingCats.Add(verticalCats[i]);
                }
            }

            //如果垂直方向上符合条件的话，添加一个水平检查，实现L,T型联合消除
            #region L和T型检查

            if (verticalCats.Count >= 3)
            {
                for (int i = 0; i < verticalCats.Count; i++)
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
                            if (cats[x, verticalCats[i].Y].IsColored() && cats[x, verticalCats[i].Y].ColorComponent.Color == color) 
                            {
                                horizontalCats.Add(cats[x, verticalCats[i].Y]);//错误标记 GetMatch
                            }
                            else
                            {
                                break;
                            }

                        }
                    }

                    if (horizontalCats.Count < 2)
                    {
                        horizontalCats.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalCats.Count; j++)
                        {
                            matchingCats.Add(horizontalCats[j]);
                        }

                        break;
                    }
                }
            }

            #endregion L和T型检测结束

            //如果在潜力股中达到条件，就将列表返回
            if (matchingCats.Count >= 3)
            {
                return matchingCats;
            }

            #endregion 垂直方向检测结束

        }

        //都不符合，返回空
        return null;
    }

    /// <summary>
    /// 清除棋盘上达到3个的匹配元素，如果匹配数量达到4个及以上生成相应的特殊元素
    /// </summary>
    /// <returns>返回一个布尔值needsRefill，为真则匹配元素已消除，需要填充</returns>
    public bool ClearValidMatches()
    {
        bool needsRefill = false;

        for(int y = 0; y < yDim; y++)
        {
            for(int x = 0; x < xDim; x++)
            {
                //遍历棋盘，定义match列表，把所有cat送去匹配，参数是遍历到的cat和坐标
                if (cats[x, y].IsClearable())
                {
                    List<GameCat> match = GetMatch(cats[x, y], x, y);

                    //如果遍历的结果不为空，使用Count类型声明specialCatType进行统计
                    if(match != null)
                    {
                        CatType specialCatType = CatType.Count;

                        //从match列表里随机获取一个cat以及坐标，在条件符合的时候将它生成为特殊元素
                        GameCat randomCat = match[Random.Range(0, match.Count)];

                        int specialCatX = randomCat.X;
                        int specialCatY = randomCat.Y;

                        //匹配成功的数量为4，生成RowClear,ColClear
                        if(match.Count == 4)
                        {
                            //如果是随机消除的，随机生成
                            if (pressedCat == null || enteredCat == null)
                            {
                                specialCatType = (CatType)Random.Range((int)CatType.RowClear, (int)CatType.ColClear);
                            }else if (pressedCat.Y == enteredCat.Y)
                            {//同行互换生成整行清除
                                specialCatType = CatType.RowClear;
                            }
                            else
                            {//同列互换生成整列
                                specialCatType = CatType.ColClear;
                            }
                        }
                        //匹配数量大等于5生成Paw
                        else if(match.Count >= 5)
                        {
                            specialCatType = CatType.Paw;
                        }

                        //消除之后判断填充，这里没太明白，之后再看
                        for(int i = 0; i < match.Count; i++)
                        {
                            if (ClearCat(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                                if (match[i] == pressedCat || match[i] == enteredCat)
                                {
                                    specialCatX = match[i].X;
                                    specialCatY = match[i].Y;
                                }
                            }
                        }
                        if (specialCatType != CatType.Count)
                        {
                            Destroy(cats[specialCatX, specialCatY]);

                            GameCat newCat = SpawnNewCat(specialCatX, specialCatY, specialCatType);

                            if ((specialCatType == CatType.RowClear || specialCatType == CatType.ColClear 
                                && newCat.IsClearable() && match[0].IsColored()))
                            {
                                newCat.ColorComponent.SetColor(match[0].ColorComponent.Color);
                            }else if(specialCatType == CatType.Paw && newCat.IsColored())
                            {
                                newCat.ColorComponent.SetColor(ColorCat.ColorType.Any);
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
    public bool ClearCat(int x, int y)
    {
        if (cats[x, y].IsClearable() && !cats[x, y].ClearableComponent.IsBeingCleared) 
        {
            cats[x, y].ClearableComponent.Clear();
            
            SpawnNewCat(x, y, CatType.Empty);
            
            ClearDogs(x, y);
            
            return true;
        
        }
        
        return false;
        
    }

    /// <summary>
    /// 清除障碍物dog
    /// 如果参数坐标相邻位置存在dog，则清除dog生成emptyCat
    /// </summary>
    /// <param name="x">被清除的元素的坐标x</param>
    /// <param name="y">被清除的元素的坐标y</param>
    public void ClearDogs(int x,int y)
    {
        for(int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (cats[adjacentX, y].Type == CatType.Dog && cats[adjacentX, y].IsClearable())  
                {
                    cats[adjacentX, y].ClearableComponent.Clear();

                    SpawnNewCat(adjacentX, y, CatType.Empty);
                }
            }
        }

        for(int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if(adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if (cats[x, adjacentY].Type == CatType.Dog && cats[x, adjacentY].IsClearable()) 
                {
                    cats[x, adjacentY].ClearableComponent.Clear();

                    SpawnNewCat(x,adjacentY, CatType.Empty);
                }
            }
        }
    }

    /// <summary>
    /// 清除行，遍历x轴调用ClearCat()清除整行
    /// </summary>
    /// <param name="row">y值，定位行</param>
    public void ClearRow(int row)
    {
        for(int x= 0; x < xDim; x++)
        {
            ClearCat(x, row);
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
            ClearCat(col, y);
        }
    }

    /// <summary>
    /// 清除同花色
    /// 遍历棋盘，如果遍历到的元素颜色和pawColor相同则消除
    /// 选中的花色也是Paw则消除整个棋盘元素
    /// </summary>
    /// <param name="pawColor">与Paw发生交换的元素花色</param>
    public void ClearColor(ColorCat.ColorType pawColor)
    {
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0;y < yDim; y++)
            {
                if (cats[x, y].IsColored() && (cats[x, y].ColorComponent.Color == pawColor)
                    || pawColor == ColorCat.ColorType.Any) 
                {
                    ClearCat(x, y);
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
    public List<GameCat> GetNumOfTypes(CatType type)
    {
        List<GameCat> numOfType = new List<GameCat>();

        for(int x = 0; x < xDim; x++)
        {
            for(int y= 0;y<yDim; y++)
            {
                if (cats[x, y].Type == type)
                {
                    numOfType.Add(cats[x, y]);
                }
            }
        }

        return numOfType;
    }
}
