using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using MyFrame.BrainBubbles.Bubbles.Manager;
using MyFrame.EventSystem.Events;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GameOver_Result : MonoBehaviour
{
    [SerializeField] Slider slider_Win;
    private Image slider_Win_Background;
    private Image slider_Win_Fill;

    [SerializeField] Slider slider_Lose;
    private Image slider_Lose_Background;
    private Image slider_Lose_Fill;

    private RawImage rawImage;

    private const int Win_target = 3;
    private const int Lose_target = 10;

    private int currentWin_num;
    private int currentLose_num;

    private const float aliveTime = 4f;      // 进度条显示时长
    private const float fadeDuration = 1f;   // 淡出持续时间

    private void Start()
    {
        slider_Win.maxValue = Win_target;
        slider_Lose.maxValue = Lose_target;
        currentWin_num = 0;
        currentLose_num = 0;

        // 初始化胜利进度条元素（只取 Background 和 Fill）
        slider_Win_Background = slider_Win.transform.Find("Background")?.GetComponent<Image>();
        slider_Win_Fill = slider_Win.transform.Find("Fill Area/Fill")?.GetComponent<Image>();

        // 初始化失败进度条元素（只取 Background 和 Fill）
        slider_Lose_Background = slider_Lose.transform.Find("Background")?.GetComponent<Image>();
        slider_Lose_Fill = slider_Lose.transform.Find("Fill Area/Fill")?.GetComponent<Image>();

        // 初始隐藏
        SetSliderAlpha(slider_Win, 0f);
        SetSliderAlpha(slider_Lose, 0f);
        slider_Win.enabled = false;
        slider_Lose.enabled = false;

        GameManager.Instance._eventBus.Subscribe<GameOverEvent>(ChangeInfo);
    }

    private void ChangeInfo(GameOverEvent overEvent)
    {
        if (overEvent.Res)
        {
            currentWin_num++;
            slider_Win.value = currentWin_num;

            slider_Win.enabled = true;
            SetSliderAlpha(slider_Win, 1f);
            StopCoroutine(StartSliderFadeOut(slider_Win));
            StartCoroutine(StartSliderFadeOut(slider_Win));

            EndJudgment();
        }
        else
        {
            currentLose_num++;
            slider_Lose.value = currentLose_num;

            slider_Lose.enabled = true;
            SetSliderAlpha(slider_Lose, 1f);
            StopCoroutine(StartSliderFadeOut(slider_Lose));
            StartCoroutine(StartSliderFadeOut(slider_Lose));
        }
    }

    /// <summary>
    /// 进度条淡出协程：先显示 aliveTime 秒，再 fadeDuration 秒淡出到透明
    /// </summary>
    IEnumerator StartSliderFadeOut(Slider targetSlider)
    {
        // 等待显示时间
        float showTimer = aliveTime;
        while (showTimer > 0)
        {
            showTimer -= Time.deltaTime;
            yield return null;
        }

        // 开始淡出
        float fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float progress = fadeTimer / fadeDuration;
            float currentAlpha = Mathf.Lerp(1f, 0f, progress);

            SetSliderAlpha(targetSlider, currentAlpha);
            yield return null;
        }

        // 最终强制透明并禁用
        SetSliderAlpha(targetSlider, 0f);
        targetSlider.enabled = false;
    }

    /// <summary>
    /// 统一设置指定进度条的透明度（只控制 Background 和 Fill）
    /// </summary>
    private void SetSliderAlpha(Slider slider, float alpha)
    {
        alpha = Mathf.Clamp01(alpha);

        if (slider == slider_Win)
        {
            if (slider_Win_Background != null) ChangeAlpha(slider_Win_Background, alpha);
            if (slider_Win_Fill != null) ChangeAlpha(slider_Win_Fill, alpha);
        }
        else if (slider == slider_Lose)
        {
            if (slider_Lose_Background != null) ChangeAlpha(slider_Lose_Background, alpha);
            if (slider_Lose_Fill != null) ChangeAlpha(slider_Lose_Fill, alpha);
        }
    }

    /// <summary>
    /// 修改单个 Image 的透明度
    /// </summary>
    private void ChangeAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private void EndJudgment()
    {
        if (slider_Win.value >= slider_Win.maxValue)
        {
            StartCoroutine(WaitTime());
        }
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("EndScene");
    }
}