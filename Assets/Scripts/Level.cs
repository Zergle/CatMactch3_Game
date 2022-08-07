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

    //引用Grid信息，添加首字母区分
    public Grid L_Grid;

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

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void GameWin()
    {
        Debug.Log("Win");
    }

    public virtual void GameLose()
    {
        Debug.Log("Lose");
    }

    /// <summary>
    /// 获取用户操作
    /// </summary>
    public virtual void OnMove()
    {

    }

    /// <summary>
    /// 更新分数
    /// </summary>
    /// <param name="cat"></param>
    public virtual void OnCatCleared(GameCat cat)
    {

    }
}
