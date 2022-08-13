using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 选择关卡的脚本
/// </summary>
public class LevelSelect : MonoBehaviour
{
    #region 各种声明
    
    //给每个关卡键选择对应得关卡得分展示
    [System.Serializable]
    public struct ButtonPlayerPrefs
    {
        public GameObject _GameObject; //对应关卡的button
        public string PlayerPrefKey; //对应关卡的名字
    }

    public ButtonPlayerPrefs[] Buttons;

    #endregion

    #region 方法们

    /// <summary>
    /// 游戏开始时设置每个按钮星星的隐藏和显示（满足条件）
    /// </summary>
    private void Start()
    {
        for(int i = 0; i < Buttons.Length; i++)
        {
            int score = PlayerPrefs.GetInt(Buttons[i].PlayerPrefKey, 0);

            for(int starID = 1;starID <= 3; starID++)
            {
                Transform star = Buttons[i]._GameObject.transform.Find("Star" + starID);

                //如果原有星级小于当前得分，则显示
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
