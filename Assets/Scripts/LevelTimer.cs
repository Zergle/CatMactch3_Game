using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimer : Level
{
    #region 各种声明

    //剩余时间
    public int TimeLeft;

    //目标分数
    public int TargetScore;

    //已消耗时间
    private float timer;

    //判断时间结束
    private bool timeOut = false;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.Timer;

        ScoreStar(TargetScore);

        Debug.Log($"剩余时间：{TimeLeft}，目标分数：{TargetScore}");
    }

    /// <summary>
    /// 游戏运行过程中计时
    /// </summary>
    void Update()
    {
        if (!timeOut)
        {
            timer += Time.deltaTime;

            if (TimeLeft - timer <= 0)
            {
                if (currentScore >= TargetScore)
                { 
                    GameWin();
                }
                else
                {
                    GameLose();
                }
                timeOut = true;
            }
        }
    }
}
