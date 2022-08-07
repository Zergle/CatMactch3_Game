using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ClearableCat的子类，给Paw添加清理交换的所有花色功能
/// </summary>
public class ColorClear : ClearableCat
{
    #region 各种声明
    //保存将要和Paw匹配的花色
    private ColorCat.ColorType pawColor;
    public ColorCat.ColorType PawColor
    {
        get { return pawColor; }
        set { pawColor = value; }
    }

    #endregion

    /// <summary>
    /// 重写ClearableCat.Clear()，将属性中的花色清除
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        
        cat.GridRef.ClearColor(pawColor);
    }
}
