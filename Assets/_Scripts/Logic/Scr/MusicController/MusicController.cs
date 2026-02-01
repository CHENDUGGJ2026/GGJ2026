using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LunziSpace;
using MyFrame.EventSystem.Events;
using MyFrame.BrainBubbles.Bubbles.Manager;
public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [SerializeField] AudioClip DefaultBGM_music;
    [SerializeField] AudioClip FightBGM_music;

    private AudioSource audioSource_BGM;
    private AudioSource audioSource_SoundEffect;
    [SerializeField] List<SoundEffect> soundEffects = new List<SoundEffect>();

    private void Awake()
    {
        audioSource_BGM = GetComponent<AudioSource>();
        if (audioSource_BGM == null)
        {
            audioSource_BGM = gameObject.AddComponent<AudioSource>();
        }
        audioSource_BGM.loop = true; // BGM默认循环

        audioSource_SoundEffect = gameObject.AddComponent<AudioSource>();
        audioSource_SoundEffect.loop = false; // 音效默认不循环
    }
    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        // 游戏启动时自动播放默认BGM
        StartBGM();

        GameManager.Instance._eventBus.Subscribe<GameOverEvent>(DefaultBGM);
    }

    public void StartBGM()
    {
        PlayBGM(DefaultBGM_music);
    }
    /// <summary>
    /// 播放默认BGM（无卡顿切换）
    /// </summary>
    public void DefaultBGM(GameOverEvent gameOver)
    {
        PlayBGM(DefaultBGM_music);
    }

    /// <summary>
    /// 播放战斗BGM（无卡顿切换）
    /// </summary>
    public void FightBGM()
    {
        PlayBGM(FightBGM_music);
    }

    /// <summary>
    /// 通用BGM播放方法
    /// </summary>
    /// <param name="bgmClip">要播放的BGM音频片段</param>
    private void PlayBGM(AudioClip bgmClip)
    {
        // 校验：音频片段为空则提示并返回
        if (bgmClip == null)
        {
            Debug.LogWarning("要播放的BGM音频片段为空！请检查赋值", this);
            return;
        }

        // 如果当前正在播放目标BGM，无需重复播放
        if (audioSource_BGM.clip == bgmClip && audioSource_BGM.isPlaying)
        {
            return;
        }

        // 切换并播放BGM
        audioSource_BGM.clip = bgmClip;
        audioSource_BGM.Play(); // 开始播放（如果已在播放，会无缝切换）
    }
    //查找音效
    public AudioClip FindSoundEffect(string name)
    {
        foreach (var effect in soundEffects)
        {
            if(name == effect.soundName)
            {
                return effect.clip;
            }
        }
        Debug.Log("没有找到音效");
        return null;
    }

    /// <summary>
    /// 播放指定名称的音效
    /// </summary>
    public void PlaySoundEffect(string soundName)
    {
        AudioClip clip = FindSoundEffect(soundName);
        if (clip != null)
        {
            audioSource_SoundEffect.PlayOneShot(clip); // PlayOneShot不打断现有音效
        }
    }
}

[System.Serializable]
public class SoundEffect
{
    public string soundName;
    public AudioClip clip;
}
