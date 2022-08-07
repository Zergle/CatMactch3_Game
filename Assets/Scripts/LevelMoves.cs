using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{
    #region 各种声明

    public int NumMoves;

    public int TargetScore;

    private int movesUsed = 0;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.Moves;

        ScoreStar(TargetScore);

        Debug.Log($"剩余步数{NumMoves}，目标分数{TargetScore}");
    }

    public override void OnMove()
    {
        base.OnMove();

        movesUsed++;

        Debug.Log($"Moves remaining:{NumMoves - movesUsed}");

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
}
