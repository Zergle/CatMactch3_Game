using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 给元素添加可移动属性
/// </summary>
public class MovableBlock : MonoBehaviour
{
    #region 各种声明

    //block用来保存GameBlock的信息
    private GameBlock block;

    //逐帧填充
    private IEnumerator moveCoroutine;

    #endregion

    #region 方法们
    /// <summary>
    /// 游戏启动时获取组件信息，具有movable组件，就是可移动元素
    /// </summary>
    private void Awake()
    {
        block = GetComponent<GameBlock>();
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
    /// 移动动画
    /// </summary>
    /// <param name="newX">将要移动到的位置newX</param>
    /// <param name="newY">将要移动到的位置newY</param>
    /// <param name="time">动画时长</param>
    /// <returns></returns>
    public IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        //当前位置
        Vector3 startPos = transform.position;

        //目标位置
        Vector3 endPos = block.GridRef.GetWorldPosition(newX, newY);

        //在动画时长内进行交换动作
        for (float t = 0; t <= 1 * time; t += Time.deltaTime) 
        {
            block.X = newX;
            block.Y = newY;

            block.transform.position = Vector3.Lerp(startPos, endPos, t / time);

            yield return 0;
        }

        block.transform.position = endPos;
    }

    #endregion
}
