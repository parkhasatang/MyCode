using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonDontDestroy<SoundManager>, ISavable
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Background Music")]
    public GameObject bgmSound;
    private AudioSource backgroundMusicSource;

    public bool isBgmOn = true;
    public bool isSfxOn = true;

    private Dictionary<string, Sound> loopingSounds = new();

    protected override void Awake()
    {
        base.Awake();
        Register(this);
        InitializeBackgroundMusicSource();
    }

    private void Start()
    {
        // 초기 설정 적용
        SetBgmOn(isBgmOn);
        SetSfxOn(isSfxOn);
    }

    public void PlaySoundEffect(string clipName, float soundValue = 1f, float pitch = 1f, bool loop = false)
    {
        if (!isSfxOn) return; // 효과음이 꺼져있으면 재생하지 않음

        AudioClip clip = ResourceManager.Instance.GetResource<AudioClip>(clipName);
        if (clip != null)
        {
            Sound effectSound = GameManager.Instance.Pool.SpawnFromPool<Sound>(E_PoolObjectType.Sound);
            effectSound.InitSound(audioMixer, clip, soundValue, pitch, loop);

            if (loop)
            {
                loopingSounds[clipName] = effectSound;
            }
        }
    }

    // 버튼 컴포넌트용
    public void PlaySoundEffect(string clipName)
    {
        if (!isSfxOn) return; // 효과음이 꺼져있으면 재생하지 않음

        AudioClip clip = ResourceManager.Instance.GetResource<AudioClip>(clipName);
        if (clip != null)
        {
            Sound effectSound = GameManager.Instance.Pool.SpawnFromPool<Sound>(E_PoolObjectType.Sound);
            effectSound.InitSound(audioMixer, clip, 1f, 1f, false);
        }
    }

    public void StopSoundEffect(string clipName)
    {
        if (loopingSounds.TryGetValue(clipName, out Sound effectSound))
        {
            effectSound.Stop();
            loopingSounds.Remove(clipName);
        }
    }

    void InitializeBackgroundMusicSource()
    {
        backgroundMusicSource = bgmSound.GetComponent<AudioSource>();
        backgroundMusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
        LoadBackgroundMusic("idleBGM", 0.5f);
    }

    public void LoadBackgroundMusic(string clipName, float size = 1.0f)
    {
        AudioClip bgmClip = ResourceManager.Instance.GetResource<AudioClip>(clipName);
        if (bgmClip != null)
        {
            backgroundMusicSource.clip = bgmClip;
            backgroundMusicSource.volume = size;
            PlayBackgroundMusic();
        }
        else
        {
            Debug.LogError("해당 클립을 찾을 수 없습니다.");
        }
    }

    public void PlayBackgroundMusic()
    {
        if (!isBgmOn) return; // 배경음악이 꺼져있으면 재생하지 않음
        if (backgroundMusicSource.clip != null)
        {
            backgroundMusicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }

    public void SetBgmOn(bool isOn)
    {
        isBgmOn = isOn;
        if (isBgmOn)
        {
            PlayBackgroundMusic();
        }
        else
        {
            StopBackgroundMusic();
        }
    }

    public void SetSfxOn(bool isOn)
    {
        isSfxOn = isOn;
        // 효과음은 재생 시점에서 체크하므로 추가 로직 불필요
    }

    // 저장 및 로드 함수
    public void Save()
    {
        if (SaveManager.Instance.saveData != null)
        {
            SaveManager.Instance.saveData.isBgmOn = isBgmOn;
            SaveManager.Instance.saveData.isSfxOn = isSfxOn;
            SaveManager.Instance.saveData.isVibrationOn = Vibration.IsVibrationEnabled;
        }
    }

    public void Load()
    {
        // Using the null-conditional operator and null-coalescing operator to assign values
        isBgmOn = SaveManager.Instance.saveData?.isBgmOn ?? true;
        isSfxOn = SaveManager.Instance.saveData?.isSfxOn ?? true;
        Vibration.IsVibrationEnabled = SaveManager.Instance.saveData?.isVibrationOn ?? true;

        SetBgmOn(isBgmOn);
        SetSfxOn(isSfxOn);
    }

    public void Register(ISavable savable)
    {
        SaveManager.Instance.RegisterSavable(savable);
    }

    public void Unregister(ISavable savable)
    {
        SaveManager.Instance.UnregisterSavable(savable);
    }

    private void OnDestroy()
    {
        Unregister(this);
    }
}
