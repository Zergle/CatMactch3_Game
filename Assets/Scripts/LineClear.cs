using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ClearableCat子类，给特殊元素清理整行整列的功能
/// </summary>
public class LineClear : ClearableBlock
{
    #region 各种声明

    //是否行消除
    public bool IsRow;

    #endregion

    /// <summary>
    /// 重写Clear()，消除整行或整列
    /// </summary>
    public override void Clear()
    {
        base.Clear();

        if (IsRow)
        {
            //清除行
            block.GridRef.ClearRow(block.Y);
        }
        else
        {
            //清除列
            block.GridRef.ClearCol(block.X);
        }
    }
}
