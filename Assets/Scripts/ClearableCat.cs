using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定义物体是否可被消除
/// </summary>
public class ClearableCat : MonoBehaviour
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
    protected GameCat cat;

    #endregion

    /// <summary>
    /// 游戏启动时获取GameCat信息保存到cat
    /// </summary>
    private void Awake()
    {
        cat = GetComponent<GameCat>();
    }

    /// <summary>
    /// 清除，isBeingCleared为真时调用ClearCoroutine()
    /// </summary>
    public virtual void Clear()
    {
        //清除时将信息反馈到OnCatCleared()
        cat.GridRef._Level.OnCatCleared(cat);

        isBeingCleared = true;

        StartCoroutine(ClearCoroutine());
    }

    /// <summary>
    /// 消除协程，播放动画并销毁GameObject
    /// </summary>
    /// <returns></returns>
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
}
