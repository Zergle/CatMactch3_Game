using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 设置HUD并更新内容
/// </summary>
public class HUD : MonoBehaviour
{
    #region 各种声明

    public Level _Level;
    
    public GameOver _GameOver;

    public TMPro.TMP_Text RemainingText;

    public TMPro.TMP_Text RemainingSubtext;

    public TMPro.TMP_Text TargetText;

    public TMPro.TMP_Text TargetSubText;

    public TMPro.TMP_Text ScoreText;

    //保存星星图片
    public UnityEngine.UI.Image[] Stars;

    //星级
    private int starID = 0;

    #endregion

    #region 方法们

    /// <summary>
    /// 游戏开始时给星星图片设置对应星级
    /// </summary>
    void Start()
    {
        for(int i = 0; i < Stars.Length; i++)
        {
            if(i == starID)
            {
                Stars[i].enabled = true;
            }
            else
            {
                Stars[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// 设置得分，将得分转换为字符串并设置不同分数时星星的显示情况
    /// </summary>
    /// <param name="score">获得分数</param>
    public void SetScore(int score)
    {
        ScoreText.text = score.ToString();

        //不同分数时现显示的星星
        int visiableStar = 0;

        if (score >= _Level.Score1Star && score < _Level.Score2Star)
        {
            visiableStar = 1;
        }
        else if (score >= _Level.Score2Star && score < _Level.Score3Star)
        {
            visiableStar = 2;
        }
        else if (score >= _Level.Score3Star) 
        {
            visiableStar = 3;
        }

        for(int i = 0; i < Stars.Length; i++)
        {
            if(i == visiableStar)
            {
                Stars[i].enabled = true;
            }
            else
            {
                Stars[i].enabled = false;
            }
        }

        starID = visiableStar;
    }

    /// <summary>
    /// 设置目标，将目标转换为字符串
    /// </summary>
    /// <param name="target">目标分数</param>
    public void SetTarget(int target)
    {
        TargetText.text = target.ToString();
    }

    /// <summary>
    /// 将剩余的Time/Obstacles/Moves转换为String并保存
    /// </summary>
    /// <param name="remaining">剩余的Time/Dogs/Moves</param>
    public void SetRemaining(int remaining)
    {
        RemainingText.text = remaining.ToString();
    }

    public void SetRemaining(string remaining)
    {
        RemainingText.text = remaining;
    }

    /// <summary>
    /// 设置关卡类型，按情况改变关卡提示
    /// </summary>
    /// <param name="type">当前的关卡类型</param>
    public void SetLevelType(Level.LevelType type)
    {
        if (type == Level.LevelType.Moves)
        {//如果为步数关，则提示剩余步数和目标分数

            RemainingSubtext.text = "Moves Remaining";

            TargetSubText.text = "Target Score";

        }else if(type == Level.LevelType.Timer)
        {//如果为时间关，则提示剩余时间和目标分数

            RemainingSubtext.text = "Time Remaining";

            TargetSubText.text = "Target Score";

        }
        else if(type == Level.LevelType.Obstacle)
        {//如果为障碍关，则提示剩余步数和剩余Dog

            RemainingSubtext.text = "Moves Remaining";

            TargetSubText.text = "Dogs Remaining";

        }
    }

    /// <summary>
    /// 游戏失败时的行为
    /// </summary>
    public void OnGameLose()
    {
        _GameOver.ShowLose();
    }

    /// <summary>
    /// 游戏胜利时的行为
    /// </summary>
    /// <param name="score">得分</param>
    public void OnGameWin(int score)
    {
        _GameOver.ShowWin(score, starID);

        //游戏胜利后保存星级，如果游戏未结束不保存，如果新分数高于历史分数则保存新分数星级
        if (starID > PlayerPrefs.GetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 0))
        {
            PlayerPrefs.SetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, starID);
        }
    }
    #endregion
}
