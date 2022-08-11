using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    #region 各种声明

    public Level level;

    public GameOver _GameOver;

    public TMPro.TMP_Text RemainingText;
    public TMPro.TMP_Text RemainingSubtext;
    public TMPro.TMP_Text TargetText;
    public TMPro.TMP_Text TargetSubText;
    public TMPro.TMP_Text ScoreText;
    public UnityEngine.UI.Image[] Stars;

    private int starID = 0;

    #endregion

    #region 方法们

    /// <summary>
    /// 游戏开始时设置星星
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

        //设置不同分数时现显示的星星
        int visiableStar = 0;

        if (score >= level.Score1Star && score < level.Score2Star)
        {
            visiableStar = 1;
        }
        else if (score >= level.Score2Star && score < level.Score3Star)
        {
            visiableStar = 2;
        }
        else if (score >= level.Score3Star) 
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
    /// 将剩余的Time/Dogs/Moves转换为String并保存
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

    public void OnGameLose()
    {
        _GameOver.ShowLose();

        if (starID > PlayerPrefs.GetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 0))
        {
            PlayerPrefs.SetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, starID);
        }
    }

    public void OnGameWin(int score)
    {
        _GameOver.ShowWin(score, starID);
    }

    #endregion
}
