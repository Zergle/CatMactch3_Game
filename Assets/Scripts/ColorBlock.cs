using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储元素花色，赋予元素花色
/// </summary>
public class ColorBlock : MonoBehaviour
{
    #region 各种声明
    public enum ColorType
    {
        //前几种均为猫的花色，因为项目素材用的是猫
        Cow,
        Calico,
        White,
        Black,
        Blue,
        WandY,
        Orange,
        Any, //用来匹配任意花色
        Count //用来统计特定类型
    }

    //链接预制件和Sprite
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType Color;
        public Sprite Sprite;
    }

    //结构保存为数组
    public ColorSprite[] ColorSprites;

    //保存结构的字典
    private Dictionary<ColorType, Sprite> colorSpriteDict;

    //设置花色属性，上色后返回花色值
    private ColorType color;
    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }

    //保存SpriteRenderer用于替换
    private SpriteRenderer spriteRenderer;

    //将数组ColorSprites的长度保存为花色的数量NumColors
    public int NumColors
    {
        get { return ColorSprites.Length; }
    }

    #endregion

    #region 方法们
    /// <summary>
    /// 游戏启动时，将ColorSprites中的花色保存到字典
    /// </summary>
    private void Awake()
    {
        //找到贴图Cat的SpriteRenderer
        spriteRenderer = transform.Find("Cat").GetComponent<SpriteRenderer>();

        //将ColorSprites保存到字典，花色种类是键，Sprite是值
        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for(int i = 0; i < ColorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(ColorSprites[i].Color)) 
            {
                colorSpriteDict.Add(ColorSprites[i].Color, ColorSprites[i].Sprite);
            }
        }
    }

    /// <summary>
    /// 给元素上色，从字典中获取指定花色对应的Sprite替换到元素身上
    /// </summary>
    /// <param name="newColor">花色类型</param>
    public void SetColor(ColorType newColor)
    {
        color = newColor;

        if (colorSpriteDict.ContainsKey(newColor))
        {
            spriteRenderer.sprite = colorSpriteDict[newColor];
        }
    }

    #endregion
}
