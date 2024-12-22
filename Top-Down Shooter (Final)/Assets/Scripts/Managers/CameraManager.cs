using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CameraManager: 카메라 동작 및 설정을 관리하는 클래스
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance; // 싱글턴 인스턴스

    private CinemachineVirtualCamera virtualCamera; // Cinemachine 가상 카메라
    private CinemachineFramingTransposer transposer; // 카메라의 Framing Transposer 컴포넌트

    [Header("Camera distance")]
    [SerializeField] private bool canChangeCameraDistance; // 카메라 거리 변경 가능 여부
    [SerializeField] private float distanceChangeRate; // 카메라 거리 변경 속도
    public float targetCameraDistance; // 목표 카메라 거리
    [SerializeField] private float minCameraDistance = 5f; // 최소 카메라 거리
    [SerializeField] private float maxCameraDistance = 20f; // 최대 카메라 거리
    [SerializeField] private float scrollSensitivity = 2f; // 마우스 휠 민감도

    private void Awake()
    {
        // 싱글턴 설정
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("You had more than one Camera Manager");
            Destroy(gameObject);
        }

        // 카메라와 Transposer 초기화
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        // 현재 카메라 거리를 목표 거리로 초기화
        targetCameraDistance = transposer.m_CameraDistance;
    }

    private void Update()
    {
        HandleMouseScroll(); // 마우스 스크롤 입력 처리
        UpdateCameraDistance(); // 카메라 거리 업데이트
    }

    // 마우스 스크롤 입력 처리
    private void HandleMouseScroll()
    {
        if (!canChangeCameraDistance)
            return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // 마우스 스크롤 입력
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            targetCameraDistance -= scrollInput * scrollSensitivity; // 목표 거리 조정
            targetCameraDistance = Mathf.Clamp(targetCameraDistance, minCameraDistance, maxCameraDistance); // 거리 제한
        }
    }

    // 목표 거리에 따라 카메라 거리 업데이트
    private void UpdateCameraDistance()
    {
        if (!canChangeCameraDistance)
            return;

        float currentDistance = transposer.m_CameraDistance; // 현재 카메라 거리

        if (Mathf.Abs(targetCameraDistance - currentDistance) < 0.01f)
            return; // 목표 거리와 거의 동일하면 업데이트 종료

        // 선형 보간으로 카메라 거리 부드럽게 변경
        transposer.m_CameraDistance =
            Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    // 외부에서 카메라 거리를 변경
    public void ChangeCameraDistance(float distance, float newChangeRate = 0.25f)
    {
        distanceChangeRate = newChangeRate; // 변경 속도 설정
        targetCameraDistance = Mathf.Clamp(distance, minCameraDistance, maxCameraDistance); // 거리 제한 후 설정
    }

    // 카메라 타겟 변경
    public void ChangeCameraTarget(Transform target, float cameraDistance = 7, float newLookAheadTime = 0)
    {
        virtualCamera.Follow = target; // 새로운 타겟 설정
        transposer.m_LookaheadTime = newLookAheadTime; // Lookahead 시간 설정
        ChangeCameraDistance(cameraDistance); // 카메라 거리 변경
    }
}
