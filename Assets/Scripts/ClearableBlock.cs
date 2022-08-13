using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定义物体是否可被消除
/// </summary>
public class ClearableBlock : MonoBehaviour
{
    #region 各种声明
    public AnimationClip clearAnimation;

    //声明被消除属性
    private bool isBeingCleared = false;
    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }

    //调用GameCat信息
    protected GameBlock block;

    #endregion

    #region 方法们
    /// <summary>
    /// 游戏启动时获取GameCat信息保存到cat
    /// </summary>
    private void Awake()
    {
        block = GetComponent<GameBlock>();
    }

    /// <summary>
    /// 清除操作，清楚功能由ClearCoroutine()实现
    /// </summary>
    public virtual void Clear()
    {
        //清除时将信息反馈到OnBlockCleared()
        block.GridRef._Level.OnBlockCleared(block);

        isBeingCleared = true;

        StartCoroutine(ClearCoroutine());
    }

    /// <summary>
    /// 消除功能：播放动画，销毁GameObject
    /// </summary>
    /// <returns>返回缓冲动画</returns>
    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play(clearAnimation.name);

            yield return new WaitForSeconds(clearAnimation.length);

            Destroy(gameObject);
        }
    }
    #endregion
}
