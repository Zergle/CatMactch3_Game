using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level的子类，障碍赛
/// 目前只能手动摆放障碍，想办法解决下随机摆放
/// </summary>
public class LevelObstacles : Level
{
    #region 各种声明

    //步数
    public int NumMoves;

    //已走步数
    private int movesUsed = 0;

    //设置障碍物数组，障碍物为Dog
    public Grid.CatType[] ObstacleTypes;

    //剩下的障碍物数量
    private int numObstaclesLeft;

    #endregion

    #region 方法们
    /// <summary>
    /// 游戏开始时，设置关卡类型
    /// </summary>
    void Start()
    {
        type = LevelType.Obstacle;

        //从地图上遍历找到障碍物数量
        if(ObstacleTypes != null)
        {
            for (int i = 0; i < ObstacleTypes.Length; i++)
            {
                numObstaclesLeft += _Grid.GetNumOfTypes(ObstacleTypes[i]).Count;
            }
        }

        //设置HUD，类型、得分、剩余、目标
        _HUD.SetLevelType(type);
        _HUD.SetScore(currentScore);
        _HUD.SetRemaining(numObstaclesLeft);
        _HUD.SetTarget(NumMoves);
    }

    /// <summary>
    /// 玩家行动
    /// </summary>
    public override void OnMove()
    {
        base.OnMove();

        movesUsed++;

        _HUD.SetRemaining(NumMoves - movesUsed);

        if (NumMoves - movesUsed == 0 && numObstaclesLeft > 0) 
        {
            GameLose();
        }
    }

    /// <summary>
    /// 剩余障碍物，如果原障碍物位置上的元素变成了可消除的类型，则障碍物-1
    /// </summary>
    /// <param name="cat">消除的元素类型</param>
    public override void OnCatCleared(GameCat cat)
    {
        base.OnCatCleared(cat);
        
        for (int i = 0; i < ObstacleTypes.Length; i++)
        {
            if (ObstacleTypes[i] == cat.Type)
            {
                numObstaclesLeft--;

                _HUD.SetTarget(numObstaclesLeft);
                
                if (numObstaclesLeft == 0)
                {
                    currentScore += 10000 * (NumMoves - movesUsed);

                    _HUD.SetScore(currentScore);
                    
                    GameWin();
                
                }
            }
        }
    }

    #endregion
}
