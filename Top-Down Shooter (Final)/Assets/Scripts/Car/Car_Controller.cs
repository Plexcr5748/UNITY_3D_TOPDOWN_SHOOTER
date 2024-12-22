// 자동차의 동작 및 물리적 처리를 관리하는 메인 컨트롤러 클래스

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// 자동차 구동 방식을 정의하는 열거형
public enum DriveType { FrontWheelDrive, RearWheelDrive, AllWheelDrive }

[RequireComponent(typeof(NavMeshObstacle))] // NavMesh와의 충돌 처리를 위한 컴포넌트
[RequireComponent(typeof(Car_HealthController))] // 자동차의 체력 관리 컴포넌트
[RequireComponent(typeof(Car_Interaction))] // 플레이어와 자동차 간의 상호작용 컴포넌트
[RequireComponent(typeof(BoxCollider))] // 충돌 처리를 위한 콜라이더
[RequireComponent(typeof(Rigidbody))] // 자동차의 물리적 동작을 처리하는 컴포넌트
public class Car_Controller : MonoBehaviour
{
    public Car_Sounds carSounds { get; private set; } // 자동차 사운드 관리
    public Rigidbody rb { get; private set; } // 자동차의 Rigidbody 참조
    public bool carActive { get; private set; } // 자동차 활성화 상태
    private PlayerControls controls; // 플레이어 입력 컨트롤 매핑
    private float moveInput; // 전진/후진 입력 값
    private float steerInput; // 조향 입력 값

    public bool carDead = false; // 자동차 파괴 여부

    [SerializeField] private LayerMask whatIsGround; // 바닥으로 인식할 레이어

    public float speed; // 자동차의 현재 속도

    [Range(30, 60)]
    [SerializeField] private float turnSensetivity = 30; // 조향 민감도

    [Header("Car Settings")]
    [SerializeField] private DriveType driveType; // 자동차의 구동 방식
    [SerializeField] private Transform centerOfMass; // 자동차의 질량 중심
    [Range(350, 1000)]
    [SerializeField] private float carMass = 400; // 자동차의 질량
    [Range(20, 80)]
    [SerializeField] private float wheelsMass = 30; // 바퀴의 질량
    [Range(.5f, 2f)]
    [SerializeField] private float frontWheelTraction = 1; // 전륜 접지력
    [Range(.5f, 2f)]
    [SerializeField] private float backWheelTraction = 1; // 후륜 접지력

    [Header("Engine Settings")]
    [SerializeField] private float currentSpeed; // 현재 속도
    [Range(7, 12)]
    [SerializeField] private float maxSpeed = 7; // 최대 속도
    [Range(.5f, 10)]
    [SerializeField] private float accleerationSpeed = 2; // 가속도
    [Range(1500, 5000)]
    [SerializeField] private float motorForce = 1500f; // 모터의 출력 토크

    [Header("Brakes Settings")]
    [Range(0, 10)]
    [SerializeField] private float frontBrakesSensetivity = 5; // 전륜 브레이크 민감도
    [Range(0, 10)]
    [SerializeField] private float backBrakesSensetivity = 5; // 후륜 브레이크 민감도
    [Range(4000, 6000)]
    [SerializeField] private float brakePower = 5000; // 브레이크 출력
    private bool isBraking; // 브레이크 상태

    [Header("Drift Settings")]
    [Range(0, 1)]
    [SerializeField] private float frontDriftFactor = .5f; // 전륜 드리프트 계수
    [Range(0, 1)]
    [SerializeField] private float backDriftFactor = .5f; // 후륜 드리프트 계수
    [SerializeField] private float driftDuration = 1f; // 드리프트 지속 시간
    private float driftTimer; // 드리프트 타이머
    private bool isDrifting; // 드리프트 상태
    private bool canEmitTrails = true; // 트레일 활성화 여부

    private Car_Wheel[] wheels; // 자동차 바퀴 관리
    private UI ui; // UI 관리 참조

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 초기화
        wheels = GetComponentsInChildren<Car_Wheel>(); // 모든 자식 바퀴 가져오기
        carSounds = GetComponent<Car_Sounds>(); // 자동차 사운드 컴포넌트 가져오기
        ui = UI.instance; // UI 인스턴스 참조

        controls = ControlsManager.instance.controls; // 플레이어 컨트롤 가져오기

        AssignInputEvents(); // 입력 이벤트 설정
        SetupDefaultValues(); // 기본값 초기화
        ActivateCar(false); // 자동차 비활성화 상태로 시작
    }

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition; // Rigidbody의 중심 설정
        rb.mass = carMass; // 자동차 질량 설정

        foreach (var wheel in wheels)
        {
            wheel.cd.mass = wheelsMass; // 각 바퀴의 질량 설정

            if (wheel.axelType == AxelType.Front)
                wheel.SetDefaultStiffnes(frontWheelTraction); // 전륜 기본 접지력 설정

            if (wheel.axelType == AxelType.Back)
                wheel.SetDefaultStiffnes(backWheelTraction); // 후륜 기본 접지력 설정
        }
    }

    private void Update()
    {
        if (carActive == false)
            return; // 자동차가 비활성화 상태면 동작하지 않음

        speed = rb.velocity.magnitude; // 현재 속도 계산
        ui.inGameUI.UpdateSpeedText(Mathf.RoundToInt(speed * 10) + "km/h"); // 속도 UI 업데이트

        driftTimer -= Time.deltaTime; // 드리프트 타이머 감소

        if (driftTimer < 0)
            isDrifting = false; // 드리프트 종료
    }

    private void FixedUpdate()
    {
        if (carActive == false)
            return; // 자동차가 비활성화 상태면 동작하지 않음

        ApplyTrailsOnTheGround(); // 트레일 효과 적용
        ApplyAnimationToWheels(); // 바퀴 애니메이션 적용
        ApplyDrive(); // 이동 동작 처리
        ApplySteering(); // 조향 처리
        ApplyBrakes(); // 브레이크 처리
        ApplySpeedLimit(); // 속도 제한 처리

        if (isDrifting)
            ApplyDrift(); // 드리프트 적용
        else
            StopDrift(); // 드리프트 종료
    }

    private void ApplyDrive()
    {
        currentSpeed = moveInput * accleerationSpeed * Time.deltaTime; // 입력값에 따른 속도 계산

        float motorTorqueValue = motorForce * currentSpeed; // 모터 토크 계산

        foreach (var wheel in wheels)
        {
            if (driveType == DriveType.FrontWheelDrive)
            {
                if (wheel.axelType == AxelType.Front)
                    wheel.cd.motorTorque = motorTorqueValue; // 전륜 구동 시 전륜에만 토크 적용
            }
            else if (driveType == DriveType.RearWheelDrive)
            {
                if (wheel.axelType == AxelType.Back)
                    wheel.cd.motorTorque = motorTorqueValue; // 후륜 구동 시 후륜에만 토크 적용
            }
            else
            {
                wheel.cd.motorTorque = motorTorqueValue; // 사륜 구동 시 모든 바퀴에 토크 적용
            }
        }
    }

    private void ApplySpeedLimit()
    {
        // 자동차 속도가 최대 속도를 초과하지 않도록 제한
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    private void ApplySteering()
    {
        // 조향 입력값에 따라 전륜 바퀴의 방향을 조정
        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                float targetSteerAngle = steerInput * turnSensetivity; // 목표 조향 각도 계산
                wheel.cd.steerAngle = Mathf.Lerp(wheel.cd.steerAngle, targetSteerAngle, .5f); // 부드럽게 각도 변경
            }
        }
    }

    private void ApplyBrakes()
    {
        // 브레이크 입력값에 따라 바퀴의 제동 토크를 적용
        foreach (var wheel in wheels)
        {
            bool frontBrakes = wheel.axelType == AxelType.Front; // 전륜 여부 확인
            float brakeSensetivity = frontBrakes ? frontBrakesSensetivity : backBrakesSensetivity; // 민감도 설정

            float newBrakeTorque = brakePower * brakeSensetivity * Time.deltaTime; // 제동 토크 계산
            float currentBrakeTorque = isBraking ? newBrakeTorque : 0; // 브레이크가 활성화된 경우만 적용

            wheel.cd.brakeTorque = currentBrakeTorque; // 제동 토크 적용
        }
    }

    private void ApplyDrift()
    {
        // 드리프트 상태일 때 바퀴의 측면 접지력을 감소
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.axelType == AxelType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : backDriftFactor; // 바퀴 위치에 따른 드리프트 계수

            WheelFrictionCurve sidewaysFriction = wheel.cd.sidewaysFriction;
            sidewaysFriction.stiffness *= (1 - driftFactor); // 접지력 감소
            wheel.cd.sidewaysFriction = sidewaysFriction; // 변경된 값 적용
        }
    }

    private void StopDrift()
    {
        // 드리프트 상태를 해제하고 기본 접지력 복원
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultStiffnes();
        }
    }

    private void ApplyAnimationToWheels()
    {
        // 바퀴의 애니메이션과 실제 물리적 위치를 동기화
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            wheel.cd.GetWorldPose(out position, out rotation); // 물리적으로 계산된 위치와 회전 값 가져오기

            if (wheel.model != null)
            {
                wheel.model.transform.position = position; // 모델의 위치 업데이트
                wheel.model.transform.rotation = rotation; // 모델의 회전 업데이트
            }
        }
    }

    private void ApplyTrailsOnTheGround()
    {
        // 바퀴가 지면에 트레일을 남길 수 있는 상태인지 확인 및 적용
        if (canEmitTrails == false)
            return;

        foreach (var wheel in wheels)
        {
            WheelHit hit;

            if (wheel.cd.GetGroundHit(out hit)) // 바퀴가 지면과 접촉 중인지 확인
            {
                if (whatIsGround == (whatIsGround | (1 << hit.collider.gameObject.layer)))
                {
                    wheel.trail.emitting = true; // 지정된 지면 레이어에 있을 때 트레일 활성화
                }
                else
                {
                    wheel.trail.emitting = false; // 지면이 아닌 경우 트레일 비활성화
                }
            }
            else
            {
                wheel.trail.emitting = false; // 지면 접촉이 없을 경우 트레일 비활성화
            }
        }
    }

    public void ActivateCar(bool activate)
    {
        carActive = activate; // 자동차 활성화 상태 설정

        if (carSounds != null)
            carSounds.ActivateCarSFX(activate); // 사운드 상태도 동기화
    }

    public void BrakeTheCar()
    {
        // 자동차를 정지 상태로 전환하고 필요한 설정 변경
        canEmitTrails = false; // 트레일 비활성화
        carDead = true; // 자동차 파괴 상태
        foreach (var wheel in wheels)
        {
            wheel.trail.emitting = false; // 모든 바퀴 트레일 비활성화
        }

        rb.drag = 1; // 드래그 적용
        motorForce = 0; // 모터 출력 비활성화
        isDrifting = true; // 드리프트 상태로 전환
        frontDriftFactor = .9f; // 전륜 드리프트 계수 증가
        backDriftFactor = .9f; // 후륜 드리프트 계수 증가
    }

    private void AssignInputEvents()
    {
        // 플레이어 입력 이벤트 설정
        controls.Car.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>(); // 입력 값 읽기

            moveInput = input.y; // 전진/후진 입력
            steerInput = input.x; // 조향 입력
        };

        controls.Car.Movement.canceled += ctx =>
        {
            moveInput = 0; // 입력이 중지되면 초기화
            steerInput = 0;
        };

        controls.Car.Brake.performed += ctx =>
        {
            isBraking = true; // 브레이크 활성화
            isDrifting = true; // 드리프트 시작
            driftTimer = driftDuration; // 드리프트 지속 시간 설정
        };
        controls.Car.Brake.canceled += ctx => isBraking = false; // 브레이크 해제

        controls.Car.CarExit.performed += ctx => GetComponent<Car_Interaction>().GetOutOfTheCar(); // 자동차에서 내리기
    }

    [ContextMenu("Focus camera and enable")]
    public void TestThisCar()
    {
        ActivateCar(true); // 자동차 활성화
        CameraManager.instance.ChangeCameraTarget(transform, 12); // 카메라를 자동차로 전환
    }
}
