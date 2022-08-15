using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 主菜单
/// </summary>
public class Menu : MonoBehaviour
{
    #region 方法们
    /// <summary>
    /// 点击Play时跳转关卡选择
    /// </summary>
    /// <param name="levelName"></param>
    public void PlayBtn(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}
