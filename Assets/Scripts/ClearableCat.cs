using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearableCat : MonoBehaviour
{
    //调用消除动画
    public AnimationClip clearAnimation;

    private bool isBeingCleared = false;

    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }

    //调用gamecat信息
    protected GameCat cat;

    private void Awake()
    {
        cat = GetComponent<GameCat>();
    }

    //清除的方法
    public virtual void Clear()
    {
        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    //协程播放动画
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
