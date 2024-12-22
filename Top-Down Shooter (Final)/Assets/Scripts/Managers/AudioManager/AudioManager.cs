using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// AudioManager: ���� �� ����(BGM �� SFX)�� �����ϴ� Ŭ����
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // �̱��� �ν��Ͻ�

    [SerializeField] private AudioSource[] bgm; // BGM ����� �ҽ� �迭
    [SerializeField] private bool playBgm; // BGM ��� ����
    [SerializeField] private int bgmIndex; // ���� ��� ���� BGM�� �ε���

    private void Awake()
    {
        instance = this; // �̱��� ����
    }

    private void Start()
    {
        PlayBGM(3); // ���� �� Ư�� BGM ���
    }

    private void Update()
    {
        // BGM ��Ȱ��ȭ ���¿��� ��� ���� BGM�� ������ ����
        if (playBgm == false && BgmIsPlaying())
            StopAllBGM();

        // BGM Ȱ��ȭ ���������� ���� BGM�� ��� ������ ������ ���� BGM ���
        if (playBgm && bgm[bgmIndex].isPlaying == false)
            PlayRandomBGM();
    }

    // SFX ��� �޼��� (�ɼ����� ���� ��ġ �߰�)
    public void PlaySFX(AudioSource sfx, bool randomPitch = false, float minPitch = .85f, float maxPitch = 1.1f)
    {
        if (sfx == null)
            return;

        // ���� ��ġ ����
        float pitch = Random.Range(minPitch, maxPitch);
        sfx.pitch = pitch;
        sfx.Play(); // SFX ���
    }

    // SFX ���� ��� �� ���̵� ��/�ƿ� ó��
    public void SFXDelayAndFade(AudioSource source, bool play, float taretVolume, float delay = 0, float fadeDuratuin = 1)
    {
        StartCoroutine(SFXDelayAndFadeCo(source, play, taretVolume, delay, fadeDuratuin));
    }

    // Ư�� �ε����� BGM ���
    public void PlayBGM(int index)
    {
        StopAllBGM(); // ���� BGM ����
        bgmIndex = index; // BGM �ε��� ������Ʈ
        bgm[index].Play(); // ���ο� BGM ���
    }

    // ��� BGM ����
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    // ���� BGM ���
    [ContextMenu("Play random music")]
    public void PlayRandomBGM()
    {
        StopAllBGM(); // ���� BGM ����
        bgmIndex = Random.Range(0, bgm.Length); // ���� �ε��� ����
        PlayBGM(bgmIndex); // ���õ� BGM ���
    }

    // ���� ��� ���� BGM�� �ִ��� Ȯ��
    private bool BgmIsPlaying()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
                return true;
        }
        return false; // ��� ���� BGM�� ������ false ��ȯ
    }

    // SFX ���� �� ���̵� ��/�ƿ� �ڷ�ƾ
    private IEnumerator SFXDelayAndFadeCo(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        yield return new WaitForSeconds(delay); // ����

        float startVolume = play ? 0 : source.volume; // ���̵� ���� ����
        float endVolume = play ? targetVolume : 0; // ���̵� ���� ����
        float elapsed = 0;

        if (play)
        {
            source.volume = 0; // �ʱ� ���� ����
            source.Play(); // SFX ���
        }

        // ���̵� ��/�ƿ� ó��
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / fadeDuration); // ���� ����
            yield return null;
        }

        source.volume = endVolume;

        // ���̵� �ƿ� �Ϸ� �� SFX ����
        if (play == false)
            source.Stop();
    }
}
