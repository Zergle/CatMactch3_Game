using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏基本元素的各种属性
/// </summary>
public class GameBlock : MonoBehaviour
{
    #region 各种声明

    //x,y属性保存坐标，如果是可移动元素则产生允许接受移动坐标值
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

    //type属性保存元素类型
    private Grid.BlockType type;
    public Grid.BlockType Type
    {
        get { return type; }
    }

    //保存Grid信息
    private Grid grid;
    public Grid GridRef
    {
        get { return grid; }
    }

    //可移动属性
    private MovableBlock movableComponent;
    public MovableBlock MovableComponent
    {
        get { return movableComponent; }
    }

    //花色属性
    private ColorBlock colorComponent;
    public ColorBlock ColorComponent
    {
        get { return colorComponent; }
    }

    //可清除属性
    private ClearableBlock clearableComponent;
    public ClearableBlock ClearableComponent
    {
        get { return clearableComponent; }
    }

    //设置每个元素的得分
    public int Score;

    #endregion

    #region 方法们
    /// <summary>
    /// 游戏启动时获取其他组件信息到属性
    /// </summary>
    private void Awake()
    {
        //移动组件
        movableComponent = GetComponent<MovableBlock>();

        //花色组件
        colorComponent = GetComponent<ColorBlock>();

        //清除组件
        clearableComponent = GetComponent<ClearableBlock>();

    }

    /// <summary>
    /// Init方法用来初始化来自Grid的变量，然后返回到属性
    /// </summary>
    /// <param name="_x">生成的block坐标的x</param>
    /// <param name="_y">生成的block坐标的y</param>
    /// <param name="_grid">棋盘信息</param>
    /// <param name="_type">block类型</param>
    public void Init(int _x,int _y, Grid _grid, Grid.BlockType _type)
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
    /// 鼠标点击、悬停时，将grid类中的PressBlock和EnterBlock变为当时点击、悬停的对象
    /// 鼠标释放时，调用grid中的ReleaseBlock()执行操作
    /// </summary>

    private void OnMouseDown()
    {
        grid.PressBlock(this);
    }

    private void OnMouseEnter()
    {
        grid.EnterBlock(this);
    }

    private void OnMouseUp()
    {
        grid.ReleaseBlock(); 
    }

    //可以尝试用射线实现点击互换
    #endregion

    #endregion
}


