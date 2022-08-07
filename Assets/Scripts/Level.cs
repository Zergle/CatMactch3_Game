using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 设置关卡功能
/// </summary>
public class Level : MonoBehaviour
{
    #region 各种声明

    //关卡类型
    public enum LevelType
    {
        Timer,
        Obstacle,
        Moves
    }

    //引用Grid信息
    public Grid _Grid;

    //得分星星
    public int Score1Star;
    public int Score2Star;
    public int Score3Star;

    protected LevelType type;
    public LevelType Type
    {
        get { return type; }
    }

    //每关得分
    protected int currentScore;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void GameWin()
    {
        Debug.Log("Win");
        _Grid.GameOver();
    }

    public virtual void GameLose()
    {
        Debug.Log("Lose");
        _Grid.GameOver();
    }

    /// <summary>
    /// 获取用户操作
    /// </summary>
    public virtual void OnMove()
    {
        Debug.Log("you moved");
    }

    /// <summary>
    /// 更新分数，消除指定类型之后添加相应的分数
    /// </summary>
    /// <param name="cat">参数是消除的元素类型</param>
    public virtual void OnCatCleared(GameCat cat)
    {
        currentScore += cat.Score;

        Debug.Log(currentScore);
    }
}
