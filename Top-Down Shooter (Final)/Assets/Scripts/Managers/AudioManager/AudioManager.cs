using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// AudioManager: 게임 내 사운드(BGM 및 SFX)를 관리하는 클래스
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // 싱글턴 인스턴스

    [SerializeField] private AudioSource[] bgm; // BGM 오디오 소스 배열
    [SerializeField] private bool playBgm; // BGM 재생 여부
    [SerializeField] private int bgmIndex; // 현재 재생 중인 BGM의 인덱스

    private void Awake()
    {
        instance = this; // 싱글턴 설정
    }

    private void Start()
    {
        PlayBGM(3); // 시작 시 특정 BGM 재생
    }

    private void Update()
    {
        // BGM 비활성화 상태에서 재생 중인 BGM이 있으면 정지
        if (playBgm == false && BgmIsPlaying())
            StopAllBGM();

        // BGM 활성화 상태이지만 현재 BGM이 재생 중이지 않으면 랜덤 BGM 재생
        if (playBgm && bgm[bgmIndex].isPlaying == false)
            PlayRandomBGM();
    }

    // SFX 재생 메서드 (옵션으로 랜덤 피치 추가)
    public void PlaySFX(AudioSource sfx, bool randomPitch = false, float minPitch = .85f, float maxPitch = 1.1f)
    {
        if (sfx == null)
            return;

        // 랜덤 피치 설정
        float pitch = Random.Range(minPitch, maxPitch);
        sfx.pitch = pitch;
        sfx.Play(); // SFX 재생
    }

    // SFX 지연 재생 및 페이드 인/아웃 처리
    public void SFXDelayAndFade(AudioSource source, bool play, float taretVolume, float delay = 0, float fadeDuratuin = 1)
    {
        StartCoroutine(SFXDelayAndFadeCo(source, play, taretVolume, delay, fadeDuratuin));
    }

    // 특정 인덱스의 BGM 재생
    public void PlayBGM(int index)
    {
        StopAllBGM(); // 기존 BGM 정지
        bgmIndex = index; // BGM 인덱스 업데이트
        bgm[index].Play(); // 새로운 BGM 재생
    }

    // 모든 BGM 정지
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    // 랜덤 BGM 재생
    [ContextMenu("Play random music")]
    public void PlayRandomBGM()
    {
        StopAllBGM(); // 기존 BGM 정지
        bgmIndex = Random.Range(0, bgm.Length); // 랜덤 인덱스 선택
        PlayBGM(bgmIndex); // 선택된 BGM 재생
    }

    // 현재 재생 중인 BGM이 있는지 확인
    private bool BgmIsPlaying()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
                return true;
        }
        return false; // 재생 중인 BGM이 없으면 false 반환
    }

    // SFX 지연 및 페이드 인/아웃 코루틴
    private IEnumerator SFXDelayAndFadeCo(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        yield return new WaitForSeconds(delay); // 지연

        float startVolume = play ? 0 : source.volume; // 페이드 시작 볼륨
        float endVolume = play ? targetVolume : 0; // 페이드 종료 볼륨
        float elapsed = 0;

        if (play)
        {
            source.volume = 0; // 초기 볼륨 설정
            source.Play(); // SFX 재생
        }

        // 페이드 인/아웃 처리
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / fadeDuration); // 선형 보간
            yield return null;
        }

        source.volume = endVolume;

        // 페이드 아웃 완료 후 SFX 정지
        if (play == false)
            source.Stop();
    }
}
