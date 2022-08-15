using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ClearableBlock的子类，给ColorClear添加清理花色功能
/// </summary>
public class ColorClear : ClearableBlock
{
    #region 各种声明

    //保存将要和ColorClear匹配的花色为属性
    private ColorBlock.ColorType bonusColor;
    public ColorBlock.ColorType BonusColor
    {
        get { return bonusColor; }
        set { bonusColor = value; }
    }

    #endregion

    #region 方法们

    /// <summary>
    /// 重写ClearableBlock.Clear()，将属性中的花色清除
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        
        block.GridRef.ClearColor(bonusColor);
    }

    #endregion
}
