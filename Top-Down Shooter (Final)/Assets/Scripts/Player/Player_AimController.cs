using System;
#if UNITY_EDITOR
using UnityEditor.Rendering.LookDev;
#endif
using UnityEngine;

// Player_AimController: �÷��̾��� ���� ���� �� ī�޶� ��� �����ϴ� Ŭ����
public class Player_AimController : MonoBehaviour
{
    private CameraManager cameraManager; // ī�޶� �Ŵ��� ����
    private Player player; // �÷��̾� ����
    private PlayerControls controls; // �÷��̾� �Է� ��Ʈ��

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser; // ������ ������ ���� �ð� ȿ��

    [Header("Aim Control")]
    [SerializeField] private float camChangeRate = 2; // ī�޶� �Ÿ� ���� �ӵ�

    [Header("Aim Setup")]
    [SerializeField] private Transform aim; // ���� ����
    [SerializeField] private bool isAimingPrecisly; // ���� ���� ����
    [SerializeField] private float offsetChageRate = 6; // Y ������ ���� �ӵ�
    private float offsetY; // ������ Y ������

    [Header("Aim Layers")]
    [SerializeField] private LayerMask preciseAim; // ���� ���ؿ� ���� ���̾�
    [SerializeField] private LayerMask rergularAim; // �Ϲ� ���ؿ� ���� ���̾�

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget; // ī�޶� ���� ���
    [Range(.5f, 1)]
    [SerializeField] private float minCameraDistance = 1.5f; // �ּ� ī�޶� �Ÿ�
    [Range(1, 3f)]
    [SerializeField] private float maxCameraDistance = 4; // �ִ� ī�޶� �Ÿ�
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensetivity = 5f; // ī�޶� �ΰ���

    private Vector2 mouseInput; // ���콺 �Է� ��
    private RaycastHit lastKnownMouseHit; // ���������� ������ ���콺 ��Ʈ ����

    private void Start()
    {
        // �ʱ�ȭ
        cameraManager = CameraManager.instance;
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        // �÷��̾� ��� �Ǵ� �Է� ��Ȱ��ȭ ���¶�� ����
        if (player.health.isDead || !player.controlsEnabled)
            return;

        // ���� �� ī�޶� ������Ʈ
        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void EnablePreciseAim(bool enable)
    {
        // ���� ���� Ȱ��ȭ/��Ȱ��ȭ
        isAimingPrecisly = enable;
        Cursor.visible = false;

        if (enable)
        {
            cameraManager.ChangeCameraDistance(cameraManager.targetCameraDistance - 1.0f, camChangeRate);
            Time.timeScale = .9f; // ���� ���� �� �ð� ������
        }
        else
        {
            cameraManager.ChangeCameraDistance(cameraManager.targetCameraDistance + 1.0f, camChangeRate);
            Time.timeScale = 1f; // �ð� ����
        }
    }

    public Transform GetAimCameraTarget()
    {
        // ī�޶��� ���� ��� ��ȯ
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }

    public void EnableAimLaer(bool enable)
    {
        // ������ ���� Ȱ��ȭ/��Ȱ��ȭ
        aimLaser.enabled = enable;
    }

    private void UpdateAimVisuals()
    {
        // ���� �ð� ȿ�� ������Ʈ
        aim.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        aimLaser.enabled = player.weapon.WeaponReady();

        if (!aimLaser.enabled)
            return;

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();

        // ���⸦ ���� �������� ���ϵ��� ����
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
        // ���� ���� ������Ʈ
        aim.position = GetMouseHitInfo().point;

        Vector3 newAimPosition = isAimingPrecisly ? aim.position : transform.position;
        aim.position = new Vector3(aim.position.x, newAimPosition.y + AdjustedOffsetY(), aim.position.z);
    }

    private float AdjustedOffsetY()
    {
        // Y �������� �ε巴�� ��ȯ
        if (isAimingPrecisly)
            offsetY = Mathf.Lerp(offsetY, 0, Time.deltaTime * offsetChageRate * .5f);
        else
            offsetY = Mathf.Lerp(offsetY, 1, Time.deltaTime * offsetChageRate);

        return offsetY;
    }

    public Transform Aim() => aim; // ���� ���� ���� ��ȯ
    public bool CanAimPrecisly() => isAimingPrecisly; // ���� ���� ���� ���� ��ȯ

    public RaycastHit GetMouseHitInfo()
    {
        // ���콺 ��Ʈ ���� ��ȯ
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
        // ī�޶� ��ġ ������Ʈ
        bool canMoveCamera = Vector3.Distance(cameraTarget.position, DesieredCameraPosition()) > 1;
        if (!canMoveCamera)
            return;

        cameraTarget.position =
            Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private Vector3 DesieredCameraPosition()
    {
        // ī�޶��� ���ϴ� ��ġ ���
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
        // �Է� �̺�Ʈ ����
        controls = player.controls;

        controls.Character.PreciseAim.performed += context => EnablePreciseAim(true);
        controls.Character.PreciseAim.canceled += context => EnablePreciseAim(false);
        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }
}
