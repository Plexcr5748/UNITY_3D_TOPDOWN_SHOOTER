using UnityEngine;

// 무기 타입 정의
public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

// 발사 타입 정의
public enum ShootType
{
    Single, // 단발
    Auto // 자동
}

// Weapon 클래스: 무기 관련 데이터를 관리하는 클래스
[System.Serializable] // Unity Inspector에서 볼 수 있도록 설정
public class Weapon
{
    public WeaponType weaponType; // 무기 타입
    public int bulletDamage; // 총알 데미지

    #region Regular mode variables
    public ShootType shootType; // 발사 방식
    public int bulletsPerShot { get; private set; } // 한 번의 발사로 나가는 총알 수
    private float defaultFireRate; // 기본 발사 속도
    public float fireRate = 1; // 초당 발사 가능한 총알 수
    private float lastShootTime; // 마지막 발사 시간
    #endregion

    #region Burst mode variables
    private bool burstAvalible; // 연사 모드 사용 가능 여부
    public bool burstActive; // 연사 모드 활성화 여부
    private int burstBulletsPerShot; // 연사 모드에서 발사되는 총알 수
    private float burstFireRate; // 연사 모드 발사 속도
    public float burstFireDelay { get; private set; } // 연사 간 발사 지연 시간
    #endregion

    [Header("Magazine details")]
    public int bulletsInMagazine; // 현재 탄창 내 총알 수
    public int magazineCapacity; // 탄창 최대 용량
    public int totalReserveAmmo; // 총 예비 탄약 수

    #region Weapon generic info variables
    public float reloadSpeed { get; private set; } // 재장전 속도
    public float equipmentSpeed { get; private set; } // 무기 장착 속도
    public float gunDistance { get; private set; } // 무기 유효 거리
    public float cameraDistance { get; private set; } // 무기 장착 시 카메라 거리
    #endregion

    #region Weapon spread variables
    [Header("Spread ")]
    private float baseSpread = 1; // 기본 탄퍼짐
    private float maximumSpread = 3; // 최대 탄퍼짐
    private float currentSpread = 2; // 현재 탄퍼짐
    private float spreadIncreaseRate = .15f; // 탄퍼짐 증가 속도
    private float lastSpreadUpdateTime; // 마지막 탄퍼짐 업데이트 시간
    private float spreadCooldown = 1; // 탄퍼짐 초기화 대기 시간
    #endregion

    public Weapon_Data weaponData { get; private set; } // 무기 데이터 참조

    // Weapon 클래스 생성자
    public Weapon(Weapon_Data weaponData)
    {
        bulletDamage = weaponData.bulletDamage;
        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        bulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;

        burstAvalible = weaponData.burstAvalible;
        burstActive = weaponData.burstActive;
        burstBulletsPerShot = weaponData.burstBulletsPerShot;
        burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;

        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        reloadSpeed = weaponData.reloadSpeed;
        equipmentSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        defaultFireRate = fireRate;
        this.weaponData = weaponData;
    }

    #region Spread methods
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        // 탄퍼짐을 적용한 방향 계산
        UpdateSpread();
        float randomizedValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue / 2, randomizedValue);
        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        // 탄퍼짐 업데이트
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
            currentSpread = baseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        // 탄퍼짐 증가
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }
    #endregion

    #region Burst methods
    public bool BurstActivated()
    {
        // 연사 모드 활성화 여부 확인
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        // 연사 모드 전환
        if (!burstAvalible)
            return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }
    #endregion

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire(); // 사격 가능 확인

    private bool ReadyToFire()
    {
        // 발사 준비 상태 확인
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }
        return false;
    }

    #region Reload methods
    public bool CanReload()
    {
        // 재장전 가능 여부 확인
        if (bulletsInMagazine == magazineCapacity)
            return false;

        return totalReserveAmmo > 0;
    }

    public void RefillBullets()
    {
        // 탄약 충전
        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
            bulletsToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;
    }

    private bool HaveEnoughBullets() => bulletsInMagazine > 0; // 충분한 탄약 여부 확인
    #endregion
}
