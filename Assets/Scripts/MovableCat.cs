using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于驱动元素动作

public class MovableCat : MonoBehaviour
{
    #region
    //cat用来保存GameCat的信息
    private GameCat cat;

    //逐帧填充
    private IEnumerator moveCoroutine;

    #endregion

    private void Awake()
    {
        //如果元素具备movable组件，那么就是可移动的元素
        cat = GetComponent<GameCat>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //移动方法，参数是将要移动到的坐标
    public void Move(int newX, int newY, float time)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

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


}
