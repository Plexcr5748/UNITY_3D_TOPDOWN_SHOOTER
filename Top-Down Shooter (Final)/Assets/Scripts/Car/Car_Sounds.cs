// 자동차의 사운드 효과를 관리하는 클래스

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Sounds : MonoBehaviour
{
    private Car_Controller car; // 자동차 컨트롤러 참조

    [SerializeField] private float engineVolume = .07f; // 엔진 사운드 볼륨
    [SerializeField] private AudioSource engineStart; // 엔진 시작 사운드
    [SerializeField] private AudioSource workingEngine; // 작동 중인 엔진 사운드
    [SerializeField] private AudioSource engineOff; // 엔진 꺼짐 사운드

    private float maxSpeed = 10; // 자동차 최대 속도

    public float minPitch = .75f; // 엔진 사운드의 최소 피치
    public float maxPitch = 1.5f; // 엔진 사운드의 최대 피치

    private bool allowCarSounds; // 자동차 사운드 활성화 여부

    private void Start()
    {
        car = GetComponent<Car_Controller>(); // Car_Controller 컴포넌트 가져오기
        Invoke(nameof(AllowCarSounds), 1); // 1초 후 사운드 허용 설정
    }

    private void Update()
    {
        UpdateEngineSound(); // 매 프레임 엔진 사운드 업데이트
    }

    private void UpdateEngineSound()
    {
        // 자동차 속도에 따라 엔진 사운드 피치를 조정
        float currentSpeed = car.speed;
        float pitch = Mathf.Lerp(minPitch, maxPitch, currentSpeed / maxSpeed); // 속도에 비례한 피치 계산
        workingEngine.pitch = pitch; // 엔진 사운드에 피치 적용
    }

    public void ActivateCarSFX(bool activate)
    {
        // 자동차 사운드 활성화 또는 비활성화
        if (allowCarSounds == false)
            return; // 사운드가 허용되지 않은 경우 종료

        if (activate)
        {
            engineStart.Play(); // 엔진 시작 사운드 재생
            AudioManager.instance.SFXDelayAndFade(workingEngine, true, engineVolume, 1); // 엔진 작동 사운드 페이드 인
        }
        else
        {
            AudioManager.instance.SFXDelayAndFade(workingEngine, false, 0f, .25f); // 엔진 작동 사운드 페이드 아웃
            engineOff.Play(); // 엔진 꺼짐 사운드 재생
        }
    }

    private void AllowCarSounds() => allowCarSounds = true; // 사운드 허용 설정
}
