using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TimeManager: 게임의 시간 조작을 관리하는 클래스
public class TimeManager : MonoBehaviour
{
    public static TimeManager instance; // 싱글턴 인스턴스

    [SerializeField] private float resumeRate = 3; // 시간 복귀 속도
    [SerializeField] private float pauseRate = 7; // 시간 일시정지 속도

    private float timeAdjustRate; // 시간 조정 속도
    private float targetTimeScale = 1; // 목표 시간 배율

    private void Awake()
    {
        instance = this; // 싱글턴 설정
    }

    private void Update()
    {
        // Q 키를 누르면 1초간 슬로우 모션 적용
        if (Input.GetKeyDown(KeyCode.Q))
            SlowMotionFor(1f);

        // 현재 시간 배율(Time.timeScale)이 목표 시간 배율과 다를 경우 조정
        if (Mathf.Abs(Time.timeScale - targetTimeScale) > .05f)
        {
            float adjustRate = Time.unscaledDeltaTime * timeAdjustRate; // 비정규화된 델타 타임 기반 조정 속도
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, adjustRate); // 선형 보간으로 시간 배율 조정
        }
        else
        {
            // 목표 시간 배율에 도달했을 경우 설정
            Time.timeScale = targetTimeScale;
        }
    }

    // 시간을 일시정지
    public void PauseTime()
    {
        timeAdjustRate = pauseRate; // 시간 조정 속도 설정
        targetTimeScale = 0; // 목표 시간 배율을 0으로 설정
    }

    // 시간을 재개
    public void ResumeTime()
    {
        timeAdjustRate = resumeRate; // 시간 조정 속도 설정
        targetTimeScale = 1; // 목표 시간 배율을 1로 설정
    }

    // 특정 시간 동안 슬로우 모션 적용
    public void SlowMotionFor(float seconds) => StartCoroutine(SlowTimeCo(seconds));

    // 슬로우 모션 코루틴
    private IEnumerator SlowTimeCo(float seconds)
    {
        targetTimeScale = .5f; // 목표 시간 배율을 0.5로 설정
        Time.timeScale = targetTimeScale; // 현재 시간 배율을 바로 적용
        yield return new WaitForSecondsRealtime(seconds); // 실시간으로 대기
        ResumeTime(); // 슬로우 모션 종료 후 시간 복귀
    }
}
