// 자동차 바퀴의 동작과 설정을 관리하는 클래스

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 축 종류를 나타내는 열거형 (전륜, 후륜)
public enum AxelType { Front, Back }

[RequireComponent(typeof(WheelCollider))] // WheelCollider를 필수로 요구
public class Car_Wheel : MonoBehaviour
{
    public AxelType axelType; // 바퀴의 축 타입
    public WheelCollider cd { get; private set; } // 바퀴의 물리 처리를 담당하는 WheelCollider
    public TrailRenderer trail { get; private set; } // 바퀴가 남기는 트레일 효과
    public GameObject model; // 바퀴의 시각적 모델

    private float defaultSideStiffnes; // 기본 측면 접지력

    private void Awake()
    {
        cd = GetComponent<WheelCollider>(); // WheelCollider 컴포넌트 가져오기
        trail = GetComponentInChildren<TrailRenderer>(); // TrailRenderer 컴포넌트 가져오기

        trail.emitting = false; // 초기에는 트레일 비활성화

        // 바퀴 모델이 설정되지 않았다면 기본 모델로 설정
        if (model == null)
            model = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    public void SetDefaultStiffnes(float newValue)
    {
        // 기본 측면 접지력을 설정하고 초기화
        defaultSideStiffnes = newValue;
        RestoreDefaultStiffnes();
    }

    public void RestoreDefaultStiffnes()
    {
        // 기본 측면 접지력을 WheelCollider에 적용
        WheelFrictionCurve sidewayFriction = cd.sidewaysFriction;
        sidewayFriction.stiffness = defaultSideStiffnes;
        cd.sidewaysFriction = sidewayFriction;
    }
}
