using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCat : MonoBehaviour
{
    #region

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

    //添加一个type属性
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

    //检测是否可移动
    private MovableCat movableComponent;
    public MovableCat MovableComponent
    {
        get { return movableComponent; }
    }

    //检测是否上色
    private ColorCat colorComponent;
    public ColorCat ColorComponent
    {
        get { return colorComponent; }
    }

    #endregion

    private void Awake()
    {
        //获取这个组件，如果有，就返回到属性
        movableComponent = GetComponent<MovableCat>();

        colorComponent = GetComponent<ColorCat>();
    }

    //Init方法用来初始化变量，参数分别是新生成cat的坐标，网格信息和cat种类，使用下划线进行区分
    //初始值获取自Grid，然后返回到属性
    public void Init(int _x,int _y, Grid _grid, Grid.CatType _type)
    {
        x = _x;
        y = _y;
        grid = _grid;
        type = _type;
    }

    //判断能否移动的方法，返回到属性
    public bool IsMovable()
    {
        //只要有这个组件就为真，就是可移动，就返回到前面定义的属性
        return movableComponent != null;
    }

    public bool IsColored()
    {
        return colorComponent != null;
    }

    #region 鼠标事件
    //点击
    private void OnMouseDown()
    {
        //点击鼠标时，grid的PressCat变为这个
        grid.PressCat(this);
    }

    //悬停
    private void OnMouseEnter()
    {
        grid.EnterCat(this);
    }

    //松开
    private void OnMouseUp()
    {
        grid.ReleaseCat();
    }

    //可以尝试用射线实现点击互换
    #endregion
}


