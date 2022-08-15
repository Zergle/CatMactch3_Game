using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBtn : MonoBehaviour
{
    #region 各种声明

    public GameObject _PauseMenu;

    #endregion

    #region 方法们

    /// <summary>
    /// 游戏开始时禁用暂停菜单
    /// </summary>
    void Start()
    {
        _PauseMenu.SetActive(false);
    }

    /// <summary>
    /// 暂停键，按下时激活面板，暂停游戏
    /// </summary>
    public void PauseButtonDown()
    {
        Time.timeScale = 0;

        _PauseMenu.SetActive(true);
    }

    #endregion
}
