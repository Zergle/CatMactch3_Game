using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorCat : MonoBehaviour
{
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

    [Serializable]
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;

        if (colorSpriteDict.ContainsKey(newColor))
        {
            spriteRenderer.sprite = colorSpriteDict[newColor];
        }
    }
}
