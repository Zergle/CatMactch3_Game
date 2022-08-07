using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏基本元素的各种属性
/// </summary>
public class GameCat : MonoBehaviour
{
    #region 各种声明

    //添加x,y属性
    private int x;
    public int X
    {
        get { return x; }
        set
        {
            if (IsMovable())
            {
                x = value;
            }
        }
    }

    private int y;
    public int Y
    {
        get { return y; }
        set
        {
            if (IsMovable())
            {
                y = value;
            }
        }
    }

    //添加type属性
    private Grid.CatType type;
    public Grid.CatType Type
    {
        get { return type; }
    }

    //引用Grid信息
    private Grid grid;
    public Grid GridRef
    {
        get { return grid; }
    }

    //是否可移动
    private MovableCat movableComponent;
    public MovableCat MovableComponent
    {
        get { return movableComponent; }
    }

    //是否上色
    private ColorCat colorComponent;
    public ColorCat ColorComponent
    {
        get { return colorComponent; }
    }

    //是否可清除
    private ClearableCat clearableComponent;
    public ClearableCat ClearableComponent
    {
        get { return clearableComponent; }
    }

    //设置每个元素的得分
    public int Score;

    #endregion

    /// <summary>
    /// 游戏启动时获取其他组件信息到属性
    /// </summary>
    private void Awake()
    {
        //移动
        movableComponent = GetComponent<MovableCat>();
        //花色
        colorComponent = GetComponent<ColorCat>();
        //清除
        clearableComponent = GetComponent<ClearableCat>();
    }

    /// <summary>
    /// Init方法用来初始化来自Grid的变量，使用下划线进行区分
    /// </summary>
    /// <param name="_x">生成的cat坐标的x</param>
    /// <param name="_y">生成的cat坐标的y</param>
    /// <param name="_grid">棋盘信息</param>
    /// <param name="_type">cat类型</param>
    public void Init(int _x,int _y, Grid _grid, Grid.CatType _type)
    {
        x = _x;
        y = _y;
        grid = _grid;
        type = _type;
    }

    /// <summary>
    /// 判断能否移动
    /// </summary>
    /// <returns>有movable组件则返回真，表示元素可移动</returns>
    public bool IsMovable()
    {
        return movableComponent != null;
    }

    /// <summary>
    /// 判断是否上色
    /// </summary>
    /// <returns>有color组件则返回真，表示元素已上色</returns>
    public bool IsColored()
    {
        return colorComponent != null;
    }

    /// <summary>
    /// 判断元素是否具有可清除属性
    /// </summary>
    /// <returns>有clearable组件则返回真，表示可清除</returns>
    public bool IsClearable()
    {
        return clearableComponent != null;
    }

    #region 鼠标事件
    /// <summary>
    /// 鼠标点击、悬停时，将grid类中的PressedCat和EnterCat变为当时点击、悬停的对象
    /// 鼠标释放时，调用grid中的ReleaseCat()执行操作
    /// </summary>

    private void OnMouseDown()
    {
        grid.PressCat(this);
    }

    private void OnMouseEnter()
    {
        grid.EnterCat(this);
    }

    private void OnMouseUp()
    {
        grid.ReleaseCat(); 
    }

    //可以尝试用射线实现点击互换
    #endregion
}


