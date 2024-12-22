using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CameraManager: ī�޶� ���� �� ������ �����ϴ� Ŭ����
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance; // �̱��� �ν��Ͻ�

    private CinemachineVirtualCamera virtualCamera; // Cinemachine ���� ī�޶�
    private CinemachineFramingTransposer transposer; // ī�޶��� Framing Transposer ������Ʈ

    [Header("Camera distance")]
    [SerializeField] private bool canChangeCameraDistance; // ī�޶� �Ÿ� ���� ���� ����
    [SerializeField] private float distanceChangeRate; // ī�޶� �Ÿ� ���� �ӵ�
    public float targetCameraDistance; // ��ǥ ī�޶� �Ÿ�
    [SerializeField] private float minCameraDistance = 5f; // �ּ� ī�޶� �Ÿ�
    [SerializeField] private float maxCameraDistance = 20f; // �ִ� ī�޶� �Ÿ�
    [SerializeField] private float scrollSensitivity = 2f; // ���콺 �� �ΰ���

    private void Awake()
    {
        // �̱��� ����
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("You had more than one Camera Manager");
            Destroy(gameObject);
        }

        // ī�޶�� Transposer �ʱ�ȭ
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        // ���� ī�޶� �Ÿ��� ��ǥ �Ÿ��� �ʱ�ȭ
        targetCameraDistance = transposer.m_CameraDistance;
    }

    private void Update()
    {
        HandleMouseScroll(); // ���콺 ��ũ�� �Է� ó��
        UpdateCameraDistance(); // ī�޶� �Ÿ� ������Ʈ
    }

    // ���콺 ��ũ�� �Է� ó��
    private void HandleMouseScroll()
    {
        if (!canChangeCameraDistance)
            return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // ���콺 ��ũ�� �Է�
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            targetCameraDistance -= scrollInput * scrollSensitivity; // ��ǥ �Ÿ� ����
            targetCameraDistance = Mathf.Clamp(targetCameraDistance, minCameraDistance, maxCameraDistance); // �Ÿ� ����
        }
    }

    // ��ǥ �Ÿ��� ���� ī�޶� �Ÿ� ������Ʈ
    private void UpdateCameraDistance()
    {
        if (!canChangeCameraDistance)
            return;

        float currentDistance = transposer.m_CameraDistance; // ���� ī�޶� �Ÿ�

        if (Mathf.Abs(targetCameraDistance - currentDistance) < 0.01f)
            return; // ��ǥ �Ÿ��� ���� �����ϸ� ������Ʈ ����

        // ���� �������� ī�޶� �Ÿ� �ε巴�� ����
        transposer.m_CameraDistance =
            Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    // �ܺο��� ī�޶� �Ÿ��� ����
    public void ChangeCameraDistance(float distance, float newChangeRate = 0.25f)
    {
        distanceChangeRate = newChangeRate; // ���� �ӵ� ����
        targetCameraDistance = Mathf.Clamp(distance, minCameraDistance, maxCameraDistance); // �Ÿ� ���� �� ����
    }

    // ī�޶� Ÿ�� ����
    public void ChangeCameraTarget(Transform target, float cameraDistance = 7, float newLookAheadTime = 0)
    {
        virtualCamera.Follow = target; // ���ο� Ÿ�� ����
        transposer.m_LookaheadTime = newLookAheadTime; // Lookahead �ð� ����
        ChangeCameraDistance(cameraDistance); // ī�޶� �Ÿ� ����
    }
}
