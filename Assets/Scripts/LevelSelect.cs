using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 选择关卡的脚本
/// </summary>
public class LevelSelect : MonoBehaviour
{
    #region 各种声明
    [System.Serializable]
    public struct ButtonPlayerPrefs
    {
        public GameObject _GameObject;
        public string PlayerPrefKey;
    }

    public ButtonPlayerPrefs[] Buttons;

    #endregion

    #region 方法们

    /// <summary>
    /// 游戏开始时设置星星隐藏
    /// </summary>
    private void Start()
    {
        for(int i = 0; i < Buttons.Length; i++)
        {
            int score = PlayerPrefs.GetInt(Buttons[i].PlayerPrefKey, 0);

            for(int starID = 1;starID <= 3; starID++)
            {
                Transform star = Buttons[i]._GameObject.transform.Find("Star" + starID);

                if(starID <= score)
                {
                    star.gameObject.SetActive(true);
                }
                else
                {
                    star.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 选择关卡
    /// </summary>
    /// <param name="levelName">对应的关卡名称</param>
    public void OnButtonPress(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }
    #endregion
}
