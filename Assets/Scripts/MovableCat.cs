using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 给元素添加可移动属性
/// </summary>
public class MovableCat : MonoBehaviour
{
    #region 各种声明
    //cat用来保存GameCat的信息
    private GameCat cat;

    //逐帧填充
    private IEnumerator moveCoroutine;

    #endregion

    #region 方法们
    /// <summary>
    /// 游戏启动时获取组件信息，具有movable组件，就是可移动元素
    /// </summary>
    private void Awake()
    {
        cat = GetComponent<GameCat>();
    }

    /// <summary>
    /// 移动，调用协程进行动画
    /// </summary>
    /// <param name="newX">将要移动到的位置newX</param>
    /// <param name="newY">将要移动到的位置newY</param>
    /// <param name="time">动画时长</param>
    public void Move(int newX, int newY, float time)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);

        StartCoroutine(moveCoroutine);
    }

    /// <summary>
    /// 移动协程
    /// </summary>
    /// <param name="newX">将要移动到的位置newX</param>
    /// <param name="newY">将要移动到的位置newY</param>
    /// <param name="time">动画时长</param>
    /// <returns></returns>
    public IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = cat.GridRef.GetWorldPosition(newX, newY);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime) 
        {
            cat.X = newX;
            cat.Y = newY;

            cat.transform.position = Vector3.Lerp(startPos, endPos, t / time);

            yield return 0;
        }

        cat.transform.position = endPos;
    }

    #endregion
}
