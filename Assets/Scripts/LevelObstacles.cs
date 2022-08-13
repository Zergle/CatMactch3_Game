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

    //障碍物保存为数组
    public Grid.BlockType[] ObstacleTypes;

    //剩余障碍物数量
    private int numObstaclesLeft;

    //步数奖励
    public int movesBonus;

    #endregion

    #region 方法们
    /// <summary>
    /// 游戏开始时，设置关卡类型
    /// </summary>
    void Start()
    {
        type = LevelType.Obstacle;

        //从地图上遍历找到障碍物数量
        for (int i = 0; i < ObstacleTypes.Length; i++)
        {
            numObstaclesLeft += _Grid.GetNumOfTypes(ObstacleTypes[i]).Count;
        }

        //设置HUD，类型、得分、剩余、目标
        _HUD.SetLevelType(type);
        _HUD.SetScore(currentScore);
        _HUD.SetRemaining(NumMoves);
        _HUD.SetTarget(numObstaclesLeft);
    }

    /// <summary>
    /// 玩家行动和失败判定
    /// </summary>
    public override void OnMove()
    {
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
    /// <param name="block">消除的元素类型</param>
    public override void OnBlockCleared(GameBlock block)
    {
        base.OnBlockCleared(block);
        
        //更新HUD信息和判定胜利
        for (int i = 0; i < ObstacleTypes.Length; i++)
        {
            if (ObstacleTypes[i] == block.Type)
            {
                numObstaclesLeft--;

                _HUD.SetTarget(numObstaclesLeft);
                
                if (numObstaclesLeft == 0)
                {
                    currentScore += movesBonus * (NumMoves - movesUsed);

                    _HUD.SetScore(currentScore);
                    
                    GameWin();
                
                }
            }
        }
    }

    #endregion
}
