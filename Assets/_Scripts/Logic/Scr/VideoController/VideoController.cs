using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [Header("视频配置")]
    public VideoPlayer videoPlayer; // 拖入VideoPlayer组件
    public RawImage videoRawImage;  // 拖入显示视频的RawImage
    public VideoClip videoClips;  // 可配置多个视频片段（用于切换）

    [Header("控制参数")]
    public bool autoPlay = false;    // 是否自动播放
    public bool isLoop = false;      // 是否循环播放

    private void Awake()
    {
        // 初始化VideoPlayer（如果未赋值，自动获取）
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // 绑定循环播放
        videoPlayer.isLooping = isLoop;

        // 监听视频播放完成事件（非循环时触发）
        videoPlayer.loopPointReached += OnVideoPlayComplete;
    }
    public void StartVideo()
    {
        videoPlayer.clip = videoClips;
        videoPlayer.Play();
    }

    /// <summary>
    /// 停止视频（重置到开头）
    /// </summary>
    public void StopVideo()
    {
        videoPlayer.Stop();
        // 停止后隐藏RawImage（可选）
        videoRawImage.enabled = false;
    }

    /// <summary>
    /// 视频播放完成回调（非循环时触发）
    /// </summary>
    private void OnVideoPlayComplete(VideoPlayer vp)
    {
        if (!isLoop)
        {
            StopVideo();
            Debug.Log("视频播放完成！");
            // 可在此处添加逻辑：比如跳转到游戏场景、显示按钮等
            //SceneManager.LoadScene("LogicScene");
            StartCoroutine(LoadSceneAsync("LogicScene"));
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // 开始异步加载
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 禁止加载完成后自动切换（可选，可做加载进度条）
        asyncLoad.allowSceneActivation = false;

        // 等待加载完成
        while (!asyncLoad.isDone)
        {
            // 加载进度（0-1，注意最后0.1是激活场景的进度）
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("加载进度：" + progress * 100 + "%");

            // 加载完成后激活场景
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
