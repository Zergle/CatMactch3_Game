using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{
    #region 各种声明

    //可用步数
    public int NumMoves;

    //目标分数
    public int TargetScore;

    //已用步数
    private int movesUsed = 0;

    #endregion

    #region 方法们

    /// <summary>
    /// 游戏开始时设置关卡类型，设置星级分数，为HUD设置值
    /// </summary>
    void Start()
    {
        type = LevelType.Moves;

        ScoreStar(TargetScore);

        _HUD.SetLevelType(type);

        _HUD.SetScore(currentScore);

        _HUD.SetTarget(TargetScore);

        _HUD.SetRemaining(NumMoves);
    }

    /// <summary>
    /// 设置用户每次操作后的行为，更新分数，判定输赢
    /// </summary>
    public override void OnMove()
    {
        movesUsed++;

        _HUD.SetRemaining(NumMoves - movesUsed);

        if(NumMoves - movesUsed == 0)
        {
            if (currentScore >= TargetScore)
            {
                GameWin();
            }
            else
            {
                GameLose();
            }
        }
    }

    #endregion
}
