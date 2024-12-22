using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_WeaponController: 플레이어의 무기 동작과 관련된 로직을 관리하는 클래스
public class Player_WeaponController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsAlly; // 아군 레이어
    private Player player; // 플레이어 객체 참조
    private const float REFERENCE_BULLET_SPEED = 20; // 기본 총알 속도 (질량 계산 기준)

    [SerializeField] private List<Weapon_Data> defaultWeaponData; // 기본 무기 데이터 리스트
    [SerializeField] private Weapon currentWeapon; // 현재 장착 중인 무기
    private bool weaponReady; // 무기 준비 상태
    private bool isShooting; // 발사 여부

    [Header("Bullet details")]
    [SerializeField] private float bulletImpactForce = 100; // 총알 충돌 시 힘
    [SerializeField] private GameObject bulletPrefab; // 총알 프리팹
    [SerializeField] private float bulletSpeed; // 총알 속도

    [SerializeField] private Transform weaponHolder; // 무기 홀더

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2; // 최대 무기 슬롯 수
    [SerializeField] private List<Weapon> weaponSlots; // 무기 슬롯 리스트
    [SerializeField] private GameObject weaponPickupPrefab; // 무기 픽업 프리팹

    private void Start()
    {
        // 초기화
        player = GetComponent<Player>();
        AssignInputEvents(); // 입력 이벤트 등록
    }

    private void Update()
    {
        // 발사 중일 때 슈팅 처리
        if (isShooting)
            Shoot();

        // 탄약이 없으면 무기 UI 업데이트
        if (currentWeapon.bulletsInMagazine <= 0)
        {
            currentWeapon.bulletsInMagazine = 0;
            UpdateWeaponUI();
        }
    }

    #region Slots Management - Pickup/Equip/Drop/Ready Weapon

    public void SetDefaultWeapon(List<Weapon_Data> newWeaponData)
    {
        // 기본 무기 데이터 설정 및 초기화
        defaultWeaponData = new List<Weapon_Data>(newWeaponData);
        weaponSlots.Clear();

        foreach (Weapon_Data weaponData in defaultWeaponData)
        {
            PickupWeapon(new Weapon(weaponData));
        }

        EquipWeapon(0); // 첫 번째 무기 장착
    }

    private void EquipWeapon(int i)
    {
        // 특정 슬롯의 무기를 장착
        if (i >= weaponSlots.Count)
            return;

        SetWeaponReady(false); // 무기 준비 상태 해제
        currentWeapon = weaponSlots[i]; // 현재 무기 설정
        player.weaponVisuals.PlayWeaponEquipAnimation(); // 무기 장착 애니메이션 재생

        UpdateWeaponUI(); // 무기 UI 업데이트
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        // 새로운 무기 획득 처리
        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }

        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            player.weaponVisuals.SwitchOffWeaponModels(); // 현재 무기 모델 비활성화
            weaponSlots[weaponIndex] = newWeapon;

            CreateWeaponOnTheGround(); // 무기를 바닥에 생성
            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon); // 새로운 무기 슬롯에 추가
        player.weaponVisuals.SwitchOnBackupWeaponModel(); // 예비 무기 활성화
        UpdateWeaponUI(); // 무기 UI 업데이트
    }

    private void DropWeapon()
    {
        // 무기 버리기 처리
        if (HasOnlyOneWeapon())
            return;

        CreateWeaponOnTheGround(); // 현재 무기 바닥에 생성
        weaponSlots.Remove(currentWeapon); // 슬롯에서 제거
        EquipWeapon(0); // 첫 번째 무기 장착
    }

    private void CreateWeaponOnTheGround()
    {
        // 무기를 바닥에 생성
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab, transform);
        droppedWeapon.GetComponent<Pickup_Weapon>()?.SetupPickupWeapon(currentWeapon, transform);
    }

    public void SetWeaponReady(bool ready)
    {
        // 무기 준비 상태 설정
        weaponReady = ready;

        if (ready)
            player.sound.weaponReady.Play(); // 무기 준비 사운드 재생
    }

    public bool WeaponReady() => weaponReady; // 무기 준비 상태 반환

    #endregion

    public void UpdateWeaponUI()
    {
        // 무기 UI 업데이트
        UI.instance.inGameUI.UpdateWeaponUI(weaponSlots, currentWeapon);
    }

    private IEnumerator BurstFire()
    {
        // 연사 모드 처리
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet(); // 단일 총알 발사
            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletsPerShot)
                SetWeaponReady(true);
        }
    }

    private void Shoot()
    {
        // 발사 처리
        if (!WeaponReady())
            return;

        if (currentWeapon.bulletsInMagazine <= 0)
        {
            if (currentWeapon.CanReload())
            {
                Reload(); // 재장전
            }
            else
            {
                Debug.Log("Out of ammo and no reserve bullets!"); // 탄약 부족 경고
            }
            return;
        }

        if (!currentWeapon.CanShoot())
            return;

        player.weaponVisuals.PlayFireAnimation(); // 발사 애니메이션 재생

        if (currentWeapon.shootType == ShootType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated())
        {
            StartCoroutine(BurstFire()); // 연사 처리
            return;
        }

        FireSingleBullet(); // 단일 총알 발사
        TriggerEnemyDodge(); // 적 회피 처리
    }

    private void FireSingleBullet()
    {
        // 단일 총알 발사 처리
        currentWeapon.bulletsInMagazine--;
        UpdateWeaponUI();

        player.weaponVisuals.CurrentWeaponModel().fireSFX.Play();

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab, GunPoint());
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        Bullet bulletScript = newBullet.GetComponent<Bullet>();

        bulletScript.BulletSetup(whatIsAlly, currentWeapon.bulletDamage, currentWeapon.gunDistance, bulletImpactForce);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());
        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;
    }

    private void Reload()
    {
        // 재장전 처리
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
        player.weaponVisuals.CurrentWeaponModel().realodSfx.Play();
    }

    public Vector3 BulletDirection()
    {
        // 총알 방향 계산
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (!player.aim.CanAimPrecisly())
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1; // 단일 무기 여부 확인
    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        // 특정 타입의 무기 반환
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
                return weapon;
        }

        return null;
    }

    public Weapon CurrentWeapon() => currentWeapon; // 현재 무기 반환
    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint; // 총구 위치 반환

    private void TriggerEnemyDodge()
    {
        // 적의 회피 처리
        Vector3 rayOrigin = GunPoint().position;
        Vector3 rayDirection = BulletDirection();

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
        {
            Enemy_Melee enemy_Melee = hit.collider.gameObject.GetComponentInParent<Enemy_Melee>();

            if (enemy_Melee != null)
                enemy_Melee.ActivateDodgeRoll();
        }
    }

    #region Input Events

    private void AssignInputEvents()
    {
        // 입력 이벤트 등록
        PlayerControls controls = player.controls;

        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;

        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += context => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controls.Character.ToogleWeaponMode.performed += context => currentWeapon.ToggleBurst();
    }

    #endregion
}
