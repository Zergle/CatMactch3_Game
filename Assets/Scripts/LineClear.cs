using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineClear : ClearableCat
{
    public bool IsRow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Clear()
    {
        base.Clear();

        if (IsRow)
        {
            //清除行
            cat.GridRef.ClearRow(cat.Y);
        }
        else
        {
            //清除列
            cat.GridRef.ClearCol(cat.X);
        }
    }
}
