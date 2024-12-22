using UnityEngine;

// ScriptableObject�� ����Ͽ� ���� �����͸� ����
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName; // ���� �̸�

    [Header("Bullet info")]
    public int bulletDamage; // �Ѿ� ������

    [Header("Magazine details")]
    public int bulletsInMagazine; // ���� źâ �� �Ѿ� ��
    public int magazineCapacity; // źâ �ִ� �뷮
    public int totalReserveAmmo; // ���� ź�� ��

    [Header("Regular shot")]
    public ShootType shootType; // �߻� Ÿ�� (��: ���� �߻�, ���� ��)
    public int bulletsPerShot = 1; // �� ���� �߻翡 �߻�Ǵ� �Ѿ� ��
    public float fireRate; // �߻� ���� (��)

    [Header("Burst shot")]
    public bool burstAvalible; // ���� ��� ���� ����
    public bool burstActive; // ���� ��� Ȱ��ȭ ����
    public int burstBulletsPerShot; // ���� ��忡�� �߻�Ǵ� �Ѿ� ��
    public float burstFireRate; // ���� ����� �߻� ����
    public float burstFireDelay = .1f; // ���� ���� ���� �ð�

    [Header("Weapon spread")]
    public float baseSpread; // �⺻ ź����
    public float maxSpread; // �ִ� ź����
    public float spreadIncreaseRate = .15f; // ź���� ���� �ӵ�

    [Header("Weapon generics")]
    public WeaponType weaponType; // ���� Ÿ�� (��: ����, ���� ��)
    [Range(1, 3)]
    public float reloadSpeed = 1; // ������ �ӵ� (1=�⺻ �ӵ�)
    [Range(1, 3)]
    public float equipmentSpeed = 1; // ���� ���� �ӵ� (1=�⺻ �ӵ�)
    [Range(4, 25)]
    public float gunDistance = 4; // �Ѿ� ���� �Ÿ�
    [Range(4, 10)]
    public float cameraDistance = 6; // ���� ���� �� ī�޶� �Ÿ�

    [Header("UI elements")]
    public Sprite weaponIcon; // ���� ������ (UI���� ���)
    public string weaponInfo; // ���� ���� �ؽ�Ʈ
}
