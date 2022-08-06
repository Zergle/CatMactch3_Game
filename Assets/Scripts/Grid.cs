using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于生成棋盘和棋盘中的元素

public class Grid : MonoBehaviour
{
    #region 定义

    //定义游戏元素种类
    public enum CatType
    {
        Normal,
        Dog,
        //整行清除
        RowClear,
        //整列清除
        ColClear,
        //用于计数
        Count,
        //空元素
        Empty
    }

    //定义棋盘宽高
    public int xDim;
    public int yDim;

    //定义填充速度
    public float FillTime;

    //定义狗的数量
    public int NumDogs;

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

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        catPrefabDict = new Dictionary<CatType, GameObject>();
        //遍历CatPrefabs数组，检查元素是否在字典中，没有则添加到字典
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

        //遍历棋盘，添加游戏基本块
        cats = new GameCat[xDim, yDim];

        for(int x = 0; x<xDim; x++)
        {
            for(int y = 0; y<yDim; y++)
            {
                SpawnNewCat(x, y, CatType.Empty);
            }
        }
        for (int x = 0; x < NumDogs; x++)
        {
            DogSpawner();
        }

        StartCoroutine(Fill());
    }

    //获取世界坐标
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(
            //x轴中心点-1/2宽+当前值，y轴换符号同理
            transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y
            );
    }

    //生成EmptyCat方法
    public GameCat SpawnNewCat(int x,int y,CatType type)
    {
        GameObject newCat = (GameObject)Instantiate(catPrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);

        /*newCat.transform.parent = GameObject.Find("Cats").transform;*/
        newCat.transform.parent = transform;

        cats[x, y] = newCat.GetComponent<GameCat>();

        cats[x, y].Init(x, y, this, type);

        return cats[x, y];
    }

    //填充正常方块的方法，返回值到Fill
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

    //执行动画
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

    //随机dog生成器
    public void DogSpawner()
    {
        System.Random dogPos = new System.Random();

        int dogX = dogPos.Next(cats.GetLength(0));

        int dogY = dogPos.Next(cats.GetLength(1));

        Destroy(cats[dogX, dogY].gameObject);

        SpawnNewCat(dogX, dogY, CatType.Dog);
    }

    /// <summary>
    /// 不随机dog生成器，占位
    /// 想法是传入二维数组，然后获取里面的坐标执行方法
    /// 检查拖动的元素是否相邻
    /// 两者同一轴且另一轴距离为1
    /// </summary>

    //鼠标状态事件方法们
    #region 
    public void PressCat(GameCat cat)
    {
        pressedCat = cat;
    }

    public void EnterCat(GameCat cat)
    {
        enteredCat = cat;
    }

    //鼠标释放事件，如果两者相邻，则执行SwapCats方法
    public void ReleaseCat()
    {
        if (IsAdjacent(pressedCat, enteredCat))
        {
            SwapCats(pressedCat, enteredCat);
        }

    }
    #endregion

    //检查相邻方法，两元素同轴且另一轴差值为1
    public bool IsAdjacent(GameCat cat1, GameCat cat2)
    {
        return (cat1.X == cat2.X && (int)Mathf.Abs(cat1.Y - cat2.Y) == 1)
            || (cat1.Y == cat2.Y && (int)Mathf.Abs(cat1.X - cat2.X) == 1);
    }

    //调换元素方法，参数为两个cat，两者可移动且花纹匹配，则调用移动方法进行移动，然后消除再填充，否则不动
    public void SwapCats(GameCat cat1, GameCat cat2)
    {
        if (cat1.IsMovable() && cat2.IsMovable())
        {
            //给两个元素在数组里分配位置（交换后的）
            cats[cat1.X, cat1.Y] = cat2;
            cats[cat2.X, cat2.Y] = cat1;

            if (GetMatch(cat1, cat2.X, cat2.Y) != null || GetMatch(cat2, cat1.X, cat1.Y) != null) //错误标记 SwapCat
            {
                //保存cat1的原坐标
                int cat1X = cat1.X;
                int cat1Y = cat1.Y;

                cat1.MovableComponent.Move(cat2.X, cat2.Y, FillTime);
                cat2.MovableComponent.Move(cat1X, cat1Y, FillTime);

                //清除随机生成的可消除元素
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
            }
            else
            {
                cats[cat1.X, cat1.Y] = cat1;
                cats[cat2.X, cat2.Y] = cat2;
            }
        }
    }

    //匹配方法，参数是要移动的元素和预计移动的位置，返回匹配的元素列表，如果不匹配则返回空
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

    //清除roll出来的可清除元素，同时创建特殊元素
    public bool ClearValidMatches()
    {
        bool needsRefill = false;

        for(int y = 0; y < yDim; y++)
        {
            for(int x = 0; x < xDim; x++)
            {
                //遍历棋盘，将所有猫猫送去匹配
                if (cats[x, y].IsClearable())
                {
                    List<GameCat> match = GetMatch(cats[x, y], x, y);

                    if(match != null)
                    {
                        //匹配成功，生成待统计的随机特别猫猫
                        CatType specialCatType = CatType.Count;

                        GameCat randomCat = match[Random.Range(0, match.Count)];

                        int specialCatX = randomCat.X;
                        int specialCatY = randomCat.Y;

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

                            if ((specialCatType == CatType.RowClear || specialCatType == CatType.ColClear && newCat.IsClearable() && match[0].IsColored()))
                            {
                                newCat.ColorComponent.SetColor(match[0].ColorComponent.Color);
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }

    //清除
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

    //清除障碍物dog
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
}