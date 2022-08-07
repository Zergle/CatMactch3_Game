using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储元素花色，赋予元素花色
/// </summary>
public class ColorCat : MonoBehaviour
{
    #region 各种声明
    public enum ColorType
    {
        Cow,
        Calico,
        White,
        Black,
        Blue,
        WandY,
        Orange,
        //用于匹配Any和统计Count
        Any,
        Count
    }

    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType Color;
        public Sprite Sprite;
    }

    public ColorSprite[] ColorSprites;

    private Dictionary<ColorType, Sprite> colorSpriteDict;

    private ColorType color;
    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }

    private SpriteRenderer spriteRenderer;

    public int NumColors
    {
        get { return ColorSprites.Length; }
    }

    #endregion

    /// <summary>
    /// 游戏启动时，将ColorSprites中的花色保存到字典
    /// </summary>
    private void Awake()
    {
        spriteRenderer = transform.Find("Cat").GetComponent<SpriteRenderer>();

        //遍历ColorSprites然后保存到字典
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
    /// 给元素上色，从字典中获取指定的花色类型替换到元素身上
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
}
