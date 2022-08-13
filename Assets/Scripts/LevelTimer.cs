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

    #region 方法们
    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.Timer;

        ScoreStar(TargetScore);

        //给HUD设置值
        _HUD.SetLevelType(type); //关卡类型
        _HUD.SetScore(currentScore); //当前得分
        _HUD.SetTarget(TargetScore); //目标分数
        _HUD.SetRemaining(string.Format("{0}:{1:00}", TimeLeft / 60, TimeLeft % 60)); //剩余时间
    }

    /// <summary>
    /// 游戏运行过程中计时
    /// </summary>
    void Update()
    {
        if (!timeOut)
        {
            timer += Time.deltaTime;

            _HUD.SetRemaining(string.Format("{0}:{1:00}", (int)Mathf.Max((TimeLeft - timer) / 60, 0), (int)Mathf.Max((TimeLeft - timer) % 60, 0)));

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

    #endregion
}
