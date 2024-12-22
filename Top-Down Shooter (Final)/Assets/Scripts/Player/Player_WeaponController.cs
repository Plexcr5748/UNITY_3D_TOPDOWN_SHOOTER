using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_WeaponController: �÷��̾��� ���� ���۰� ���õ� ������ �����ϴ� Ŭ����
public class Player_WeaponController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsAlly; // �Ʊ� ���̾�
    private Player player; // �÷��̾� ��ü ����
    private const float REFERENCE_BULLET_SPEED = 20; // �⺻ �Ѿ� �ӵ� (���� ��� ����)

    [SerializeField] private List<Weapon_Data> defaultWeaponData; // �⺻ ���� ������ ����Ʈ
    [SerializeField] private Weapon currentWeapon; // ���� ���� ���� ����
    private bool weaponReady; // ���� �غ� ����
    private bool isShooting; // �߻� ����

    [Header("Bullet details")]
    [SerializeField] private float bulletImpactForce = 100; // �Ѿ� �浹 �� ��
    [SerializeField] private GameObject bulletPrefab; // �Ѿ� ������
    [SerializeField] private float bulletSpeed; // �Ѿ� �ӵ�

    [SerializeField] private Transform weaponHolder; // ���� Ȧ��

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2; // �ִ� ���� ���� ��
    [SerializeField] private List<Weapon> weaponSlots; // ���� ���� ����Ʈ
    [SerializeField] private GameObject weaponPickupPrefab; // ���� �Ⱦ� ������

    private void Start()
    {
        // �ʱ�ȭ
        player = GetComponent<Player>();
        AssignInputEvents(); // �Է� �̺�Ʈ ���
    }

    private void Update()
    {
        // �߻� ���� �� ���� ó��
        if (isShooting)
            Shoot();

        // ź���� ������ ���� UI ������Ʈ
        if (currentWeapon.bulletsInMagazine <= 0)
        {
            currentWeapon.bulletsInMagazine = 0;
            UpdateWeaponUI();
        }
    }

    #region Slots Management - Pickup/Equip/Drop/Ready Weapon

    public void SetDefaultWeapon(List<Weapon_Data> newWeaponData)
    {
        // �⺻ ���� ������ ���� �� �ʱ�ȭ
        defaultWeaponData = new List<Weapon_Data>(newWeaponData);
        weaponSlots.Clear();

        foreach (Weapon_Data weaponData in defaultWeaponData)
        {
            PickupWeapon(new Weapon(weaponData));
        }

        EquipWeapon(0); // ù ��° ���� ����
    }

    private void EquipWeapon(int i)
    {
        // Ư�� ������ ���⸦ ����
        if (i >= weaponSlots.Count)
            return;

        SetWeaponReady(false); // ���� �غ� ���� ����
        currentWeapon = weaponSlots[i]; // ���� ���� ����
        player.weaponVisuals.PlayWeaponEquipAnimation(); // ���� ���� �ִϸ��̼� ���

        UpdateWeaponUI(); // ���� UI ������Ʈ
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        // ���ο� ���� ȹ�� ó��
        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }

        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            player.weaponVisuals.SwitchOffWeaponModels(); // ���� ���� �� ��Ȱ��ȭ
            weaponSlots[weaponIndex] = newWeapon;

            CreateWeaponOnTheGround(); // ���⸦ �ٴڿ� ����
            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon); // ���ο� ���� ���Կ� �߰�
        player.weaponVisuals.SwitchOnBackupWeaponModel(); // ���� ���� Ȱ��ȭ
        UpdateWeaponUI(); // ���� UI ������Ʈ
    }

    private void DropWeapon()
    {
        // ���� ������ ó��
        if (HasOnlyOneWeapon())
            return;

        CreateWeaponOnTheGround(); // ���� ���� �ٴڿ� ����
        weaponSlots.Remove(currentWeapon); // ���Կ��� ����
        EquipWeapon(0); // ù ��° ���� ����
    }

    private void CreateWeaponOnTheGround()
    {
        // ���⸦ �ٴڿ� ����
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab, transform);
        droppedWeapon.GetComponent<Pickup_Weapon>()?.SetupPickupWeapon(currentWeapon, transform);
    }

    public void SetWeaponReady(bool ready)
    {
        // ���� �غ� ���� ����
        weaponReady = ready;

        if (ready)
            player.sound.weaponReady.Play(); // ���� �غ� ���� ���
    }

    public bool WeaponReady() => weaponReady; // ���� �غ� ���� ��ȯ

    #endregion

    public void UpdateWeaponUI()
    {
        // ���� UI ������Ʈ
        UI.instance.inGameUI.UpdateWeaponUI(weaponSlots, currentWeapon);
    }

    private IEnumerator BurstFire()
    {
        // ���� ��� ó��
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet(); // ���� �Ѿ� �߻�
            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletsPerShot)
                SetWeaponReady(true);
        }
    }

    private void Shoot()
    {
        // �߻� ó��
        if (!WeaponReady())
            return;

        if (currentWeapon.bulletsInMagazine <= 0)
        {
            if (currentWeapon.CanReload())
            {
                Reload(); // ������
            }
            else
            {
                Debug.Log("Out of ammo and no reserve bullets!"); // ź�� ���� ���
            }
            return;
        }

        if (!currentWeapon.CanShoot())
            return;

        player.weaponVisuals.PlayFireAnimation(); // �߻� �ִϸ��̼� ���

        if (currentWeapon.shootType == ShootType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated())
        {
            StartCoroutine(BurstFire()); // ���� ó��
            return;
        }

        FireSingleBullet(); // ���� �Ѿ� �߻�
        TriggerEnemyDodge(); // �� ȸ�� ó��
    }

    private void FireSingleBullet()
    {
        // ���� �Ѿ� �߻� ó��
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
        // ������ ó��
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
        player.weaponVisuals.CurrentWeaponModel().realodSfx.Play();
    }

    public Vector3 BulletDirection()
    {
        // �Ѿ� ���� ���
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (!player.aim.CanAimPrecisly())
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1; // ���� ���� ���� Ȯ��
    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        // Ư�� Ÿ���� ���� ��ȯ
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
                return weapon;
        }

        return null;
    }

    public Weapon CurrentWeapon() => currentWeapon; // ���� ���� ��ȯ
    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint; // �ѱ� ��ġ ��ȯ

    private void TriggerEnemyDodge()
    {
        // ���� ȸ�� ó��
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
        // �Է� �̺�Ʈ ���
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
