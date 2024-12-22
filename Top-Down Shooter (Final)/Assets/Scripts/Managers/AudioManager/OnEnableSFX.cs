using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// OnEnableSFX: 오브젝트가 활성화될 때 사운드를 재생하는 스크립트
public class OnEnableSFX : MonoBehaviour
{
    [SerializeField] private AudioSource sfx; // 재생할 오디오 소스
    [SerializeField] private float volume = .3f; // 사운드 볼륨
    [SerializeField] private float minPitch = .85f; // 최소 피치
    [SerializeField] private float maxPitch = 1.1f; // 최대 피치

    // 오브젝트가 활성화될 때 호출
    private void OnEnable()
    {
        PlaySFX(); // 사운드 재생
    }

    // 사운드 재생 로직
    private void PlaySFX()
    {
        if (sfx == null) // 오디오 소스가 설정되지 않은 경우 반환
            return;

        float pitch = Random.Range(minPitch, maxPitch); // 랜덤 피치 설정
        sfx.pitch = pitch;
        sfx.volume = volume;
        sfx.Play(); // 사운드 재생
    }
}
