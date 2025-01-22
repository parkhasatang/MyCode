using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Todo : ������� �ٲ�� �� ��, ��������� �ҽ��� �����س��� ������ �ʿ�. ReturnAudioSource �� ����Ͽ� ���� ������� ������� �����ϰ� �ٽ� Ʋ���� �ʿ䰡 ����.
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

    // ���� ��� �޼���
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
                source.gameObject.SetActive(true); // ��Ȱ��ȭ�� ������Ʈ Ȱ��ȭ
                return source;
            }
        }

        // ��� ������ ����� �ҽ��� ���ٸ� ���� ����
        return CreateNewAudioSource();
    }

    // �߰��� ���带 ������ ��.
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
