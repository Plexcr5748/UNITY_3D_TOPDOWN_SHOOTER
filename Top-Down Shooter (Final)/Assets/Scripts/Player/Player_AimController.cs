using System;
#if UNITY_EDITOR
using UnityEditor.Rendering.LookDev;
#endif
using UnityEngine;

// Player_AimController: 플레이어의 조준 동작 및 카메라 제어를 관리하는 클래스
public class Player_AimController : MonoBehaviour
{
    private CameraManager cameraManager; // 카메라 매니저 참조
    private Player player; // 플레이어 참조
    private PlayerControls controls; // 플레이어 입력 컨트롤

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser; // 무기의 레이저 조준 시각 효과

    [Header("Aim Control")]
    [SerializeField] private float camChangeRate = 2; // 카메라 거리 변경 속도

    [Header("Aim Setup")]
    [SerializeField] private Transform aim; // 조준 지점
    [SerializeField] private bool isAimingPrecisly; // 정밀 조준 여부
    [SerializeField] private float offsetChageRate = 6; // Y 오프셋 변경 속도
    private float offsetY; // 조준의 Y 오프셋

    [Header("Aim Layers")]
    [SerializeField] private LayerMask preciseAim; // 정밀 조준에 사용될 레이어
    [SerializeField] private LayerMask rergularAim; // 일반 조준에 사용될 레이어

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget; // 카메라가 따라갈 대상
    [Range(.5f, 1)]
    [SerializeField] private float minCameraDistance = 1.5f; // 최소 카메라 거리
    [Range(1, 3f)]
    [SerializeField] private float maxCameraDistance = 4; // 최대 카메라 거리
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensetivity = 5f; // 카메라 민감도

    private Vector2 mouseInput; // 마우스 입력 값
    private RaycastHit lastKnownMouseHit; // 마지막으로 감지된 마우스 히트 정보

    private void Start()
    {
        // 초기화
        cameraManager = CameraManager.instance;
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        // 플레이어 사망 또는 입력 비활성화 상태라면 종료
        if (player.health.isDead || !player.controlsEnabled)
            return;

        // 조준 및 카메라 업데이트
        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void EnablePreciseAim(bool enable)
    {
        // 정밀 조준 활성화/비활성화
        isAimingPrecisly = enable;
        Cursor.visible = false;

        if (enable)
        {
            cameraManager.ChangeCameraDistance(cameraManager.targetCameraDistance - 1.0f, camChangeRate);
            Time.timeScale = .9f; // 정밀 조준 중 시간 느리게
        }
        else
        {
            cameraManager.ChangeCameraDistance(cameraManager.targetCameraDistance + 1.0f, camChangeRate);
            Time.timeScale = 1f; // 시간 복원
        }
    }

    public Transform GetAimCameraTarget()
    {
        // 카메라의 조준 대상 반환
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }

    public void EnableAimLaer(bool enable)
    {
        // 레이저 조준 활성화/비활성화
        aimLaser.enabled = enable;
    }

    private void UpdateAimVisuals()
    {
        // 조준 시각 효과 업데이트
        aim.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        aimLaser.enabled = player.weapon.WeaponReady();

        if (!aimLaser.enabled)
            return;

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();

        // 무기를 조준 지점으로 향하도록 설정
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();

        float laserTipLength = .5f;
        float gunDistance = player.weapon.CurrentWeapon().gunDistance;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
    }

    private void UpdateAimPosition()
    {
        // 조준 지점 업데이트
        aim.position = GetMouseHitInfo().point;

        Vector3 newAimPosition = isAimingPrecisly ? aim.position : transform.position;
        aim.position = new Vector3(aim.position.x, newAimPosition.y + AdjustedOffsetY(), aim.position.z);
    }

    private float AdjustedOffsetY()
    {
        // Y 오프셋을 부드럽게 전환
        if (isAimingPrecisly)
            offsetY = Mathf.Lerp(offsetY, 0, Time.deltaTime * offsetChageRate * .5f);
        else
            offsetY = Mathf.Lerp(offsetY, 1, Time.deltaTime * offsetChageRate);

        return offsetY;
    }

    public Transform Aim() => aim; // 현재 조준 지점 반환
    public bool CanAimPrecisly() => isAimingPrecisly; // 정밀 조준 가능 여부 반환

    public RaycastHit GetMouseHitInfo()
    {
        // 마우스 히트 정보 반환
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, preciseAim))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;
    }

    #region Camera Region

    private void UpdateCameraPosition()
    {
        // 카메라 위치 업데이트
        bool canMoveCamera = Vector3.Distance(cameraTarget.position, DesieredCameraPosition()) > 1;
        if (!canMoveCamera)
            return;

        cameraTarget.position =
            Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private Vector3 DesieredCameraPosition()
    {
        // 카메라의 원하는 위치 계산
        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    #endregion

    private void AssignInputEvents()
    {
        // 입력 이벤트 연결
        controls = player.controls;

        controls.Character.PreciseAim.performed += context => EnablePreciseAim(true);
        controls.Character.PreciseAim.canceled += context => EnablePreciseAim(false);
        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }
}
