using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorClear : ClearableCat
{
    //保存将要和Paw匹配的花色
    private ColorCat.ColorType pawColor;
    public ColorCat.ColorType PawColor
    {
        get { return pawColor; }
        set { pawColor = value; }
    }

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

        //清除所有花色
        cat.GridRef.ClearColor(pawColor);
    }
}
