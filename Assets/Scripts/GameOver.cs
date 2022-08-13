using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 设置游戏结束时的面板效果
/// </summary>
public class GameOver : MonoBehaviour
{
    #region 各种声明

    //设置获取各种UI信息
    public GameObject ScreenParent; //面板
    public GameObject ScoreParent; //分数
    public TMPro.TMP_Text LoseText; //失败文本
    public TMPro.TMP_Text ScoreText; //得分文本
    public UnityEngine.UI.Image[] Stars; //星星图组

    #endregion

    #region 方法们
    // Start is called before the first frame update
    void Start()
    {
        //在游戏开始时禁用面板
        ScreenParent.SetActive(false);

        //设置星星的显示情况
        for(int i = 0; i < Stars.Length; i++)
        {
            Stars[i].enabled = false;
        }
    }

    /// <summary>
    /// 游戏失败时的面板，不显示分数，显示失败文本
    /// </summary>
    public void ShowLose()
    {
        ScreenParent.SetActive(true);

        ScoreParent.SetActive(false);

        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play("GameOver");
        }
    }

    /// <summary>
    /// 游戏胜利时激活面板，不显示失败文本，显示分数
    /// </summary>
    /// <param name="score">得分，转换为字符串后输出为胜利文本</param>
    /// <param name="starCount">获得的星星数量</param>
    public void ShowWin(int score, int starCount)
    {
        ScreenParent.SetActive(true);

        LoseText.enabled = false;

        ScoreText.text = score.ToString();

        ScoreText.enabled = false;

        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play("GameOver");
        }

        StartCoroutine(ShowWinCoroutine(starCount));
    }

    /// <summary>
    /// 游戏胜利动画
    /// </summary>
    /// <param name="starCount">星星数量</param>
    /// <returns>在胜利面板上将星星逐个弹出</returns>
    private IEnumerator ShowWinCoroutine(int starCount)
    {
        yield return new WaitForSeconds(0.5f);

        if(starCount < Stars.Length)
        {
            for(int i = 0; i <= starCount; i++)
            {
                Stars[i].enabled = true;

                if (i > 0)
                {
                    Stars[i - 1].enabled = false;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        ScoreText.enabled = true;
    }

    /// <summary>
    /// 重新加载本场景
    /// </summary>
    public void ReplayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

    }

    /// <summary>
    /// 回到关卡选择界面
    /// </summary>
    /// <param name="levelName">需要加载的Scene</param>
    public void DoneButton(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    /// <summary>
    /// 加载下一关
    /// </summary>
    /// <param name="levelName">需要加载的Scene</param>
    public void NextButton(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    #endregion
}
