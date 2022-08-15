using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏暂停面板
/// </summary>
public class PauseMenu : MonoBehaviour
{
    #region 各种声明

    ///获取暂停面板
    public GameObject GamePauseParent;

    #endregion

    #region 方法们

    /// <summary>
    /// 继续按钮，按下时禁用面板，游戏继续
    /// </summary>
    public void ResumeBtn()
    {
        GamePauseParent.SetActive(false);

        Time.timeScale = 1;
    }

    /// <summary>
    /// 返回到场景
    /// </summary>
    /// <param name="levelName">需要跳转的场景</param>
    public void BackBtn(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);

        Time.timeScale = 1;
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        Time.timeScale = 1;
    }

    #endregion
}
