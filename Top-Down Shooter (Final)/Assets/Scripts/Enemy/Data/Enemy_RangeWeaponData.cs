using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ÿ� ���� �����͸� �����ϴ� ScriptableObject
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Range weapon Data")]
public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon details")]
    public Enemy_RangeWeaponType weaponType; // ���� ���� (�ǽ���, ������ ��)
    public float fireRate = 1f; // �߻� �ӵ� (�ʴ� �Ѿ� ��)

    public int minBulletsPerAttack = 1; // ���ݴ� �ּ� �߻� �Ѿ� ��
    public int maxBulletsPerAttack = 1; // ���ݴ� �ִ� �߻� �Ѿ� ��

    public float minWeaponCooldown = 2; // �ּ� ���� ���� ��� �ð�
    public float maxWeaponCooldown = 3; // �ִ� ���� ���� ��� �ð�

    [Header("Bullet details")]
    public int bulletDamage; // �Ѿ� ������
    [Space]
    public float bulletSpeed = 20; // �Ѿ� �ӵ�
    public float weaponSpread = .1f; // ������ ��ź ����

    // ���ݴ� �߻��� �Ѿ� ���� �������� ����
    public int GetBulletsPerAttack() => Random.Range(minBulletsPerAttack, maxBulletsPerAttack + 1);

    // ������ ���� ��� �ð��� �������� ����
    public float GetWeaponCooldown() => Random.Range(minWeaponCooldown, maxWeaponCooldown);

    // ������ ��ź ȿ���� �����Ͽ� ���� ���͸� ����
    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        float randomizedValue = Random.Range(-weaponSpread, weaponSpread); // ��ź ���� ������ ���� �� ����
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue / 2, randomizedValue); // ��ź ȸ���� ����

        return spreadRotation * originalDirection; // ���� ���⿡ ��ź ȿ�� ����
    }
}
