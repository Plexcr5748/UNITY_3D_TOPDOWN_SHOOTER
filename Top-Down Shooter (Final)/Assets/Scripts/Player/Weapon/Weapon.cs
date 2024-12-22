using UnityEngine;

// ���� Ÿ�� ����
public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

// �߻� Ÿ�� ����
public enum ShootType
{
    Single, // �ܹ�
    Auto // �ڵ�
}

// Weapon Ŭ����: ���� ���� �����͸� �����ϴ� Ŭ����
[System.Serializable] // Unity Inspector���� �� �� �ֵ��� ����
public class Weapon
{
    public WeaponType weaponType; // ���� Ÿ��
    public int bulletDamage; // �Ѿ� ������

    #region Regular mode variables
    public ShootType shootType; // �߻� ���
    public int bulletsPerShot { get; private set; } // �� ���� �߻�� ������ �Ѿ� ��
    private float defaultFireRate; // �⺻ �߻� �ӵ�
    public float fireRate = 1; // �ʴ� �߻� ������ �Ѿ� ��
    private float lastShootTime; // ������ �߻� �ð�
    #endregion

    #region Burst mode variables
    private bool burstAvalible; // ���� ��� ��� ���� ����
    public bool burstActive; // ���� ��� Ȱ��ȭ ����
    private int burstBulletsPerShot; // ���� ��忡�� �߻�Ǵ� �Ѿ� ��
    private float burstFireRate; // ���� ��� �߻� �ӵ�
    public float burstFireDelay { get; private set; } // ���� �� �߻� ���� �ð�
    #endregion

    [Header("Magazine details")]
    public int bulletsInMagazine; // ���� źâ �� �Ѿ� ��
    public int magazineCapacity; // źâ �ִ� �뷮
    public int totalReserveAmmo; // �� ���� ź�� ��

    #region Weapon generic info variables
    public float reloadSpeed { get; private set; } // ������ �ӵ�
    public float equipmentSpeed { get; private set; } // ���� ���� �ӵ�
    public float gunDistance { get; private set; } // ���� ��ȿ �Ÿ�
    public float cameraDistance { get; private set; } // ���� ���� �� ī�޶� �Ÿ�
    #endregion

    #region Weapon spread variables
    [Header("Spread ")]
    private float baseSpread = 1; // �⺻ ź����
    private float maximumSpread = 3; // �ִ� ź����
    private float currentSpread = 2; // ���� ź����
    private float spreadIncreaseRate = .15f; // ź���� ���� �ӵ�
    private float lastSpreadUpdateTime; // ������ ź���� ������Ʈ �ð�
    private float spreadCooldown = 1; // ź���� �ʱ�ȭ ��� �ð�
    #endregion

    public Weapon_Data weaponData { get; private set; } // ���� ������ ����

    // Weapon Ŭ���� ������
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
        // ź������ ������ ���� ���
        UpdateSpread();
        float randomizedValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue / 2, randomizedValue);
        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        // ź���� ������Ʈ
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
            currentSpread = baseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        // ź���� ����
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }
    #endregion

    #region Burst methods
    public bool BurstActivated()
    {
        // ���� ��� Ȱ��ȭ ���� Ȯ��
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        // ���� ��� ��ȯ
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

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire(); // ��� ���� Ȯ��

    private bool ReadyToFire()
    {
        // �߻� �غ� ���� Ȯ��
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
        // ������ ���� ���� Ȯ��
        if (bulletsInMagazine == magazineCapacity)
            return false;

        return totalReserveAmmo > 0;
    }

    public void RefillBullets()
    {
        // ź�� ����
        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
            bulletsToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;
    }

    private bool HaveEnoughBullets() => bulletsInMagazine > 0; // ����� ź�� ���� Ȯ��
    #endregion
}
