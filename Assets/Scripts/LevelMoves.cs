using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{
    #region 各种声明

    public int numMoves;

    public int targetScore;

    private int movesUsed = 0;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.Moves;

        ScoreStar(targetScore);

        Debug.Log($"剩余步数{numMoves}，目标分数{targetScore}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnMove()
    {
        base.OnMove();

        movesUsed++;

        Debug.Log($"Moves remaining:{numMoves - movesUsed}");

        if(numMoves - movesUsed == 0)
        {
            if (currentScore >= targetScore)
            {
                GameWin();
            }
            else
            {
                GameLose();
            }
        }
    }

    /// <summary>
    /// 百分比设置星级，达到分数一星，1.5倍二星，2倍三星
    /// </summary>
    /// <param name="targetScore">目标分数</param>
    public void ScoreStar(int targetScore)
    {
        Score1Star = targetScore;
        Score2Star = targetScore * 3 / 2;
        Score3Star = targetScore * 2;
    }
}
