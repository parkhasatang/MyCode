using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Todo : 배경음을 바꿔야 할 때, 배경음악의 소스를 저장해놓는 변수가 필요. ReturnAudioSource 를 사용하여 현재 재생중인 배경음을 제거하고 다시 틀어줄 필요가 있음.
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource audioSourcePrefab;
    private List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    // 사운드 재생 메서드
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.volume = volume;
        source.Play();
        StartCoroutine(DeactivateAfterPlaying(source));
    }

    private AudioSource CreateNewAudioSource()
    {
        AudioSource newSource = Instantiate(audioSourcePrefab, transform);
        audioSources.Add(newSource);
        return newSource;
    }

    public AudioSource GetAudioSource()
    {
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                source.gameObject.SetActive(true); // 비활성화된 오브젝트 활성화
                return source;
            }
        }

        // 사용 가능한 오디오 소스가 없다면 새로 생성
        return CreateNewAudioSource();
    }

    // 중간에 사운드를 꺼야할 때.
    public void ReturnAudioSource(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
    }

    private IEnumerator DeactivateAfterPlaying(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        source.gameObject.SetActive(false);
    }
}
