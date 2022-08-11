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

    //HUD信息
    public HUD _HUD;

    #endregion

    #region 方法们

    private void Start()
    {
        _HUD.SetScore(currentScore);
    }

    /// <summary>
    /// 游戏胜利
    /// </summary>
    public virtual void GameWin()
    {
        Debug.Log("Win");
        _HUD.OnGameWin(currentScore);
        _Grid.GameOver();
    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    public virtual void GameLose()
    {
        Debug.Log("Lose");
        _HUD.OnGameLose(currentScore);
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
        _HUD.SetScore(currentScore);
    }

    /// <summary>
    /// 百分比设置星级，达到分数一星，1.5倍二星，2倍三星
    /// </summary>
    /// <param name="targetScore">目标分数</param>
    public void ScoreStar(int target)
    {
            Score1Star = target * 3 / 2;
            Score2Star = target * 2;
            Score3Star = target * 5 / 2;
    }

    #endregion
}
