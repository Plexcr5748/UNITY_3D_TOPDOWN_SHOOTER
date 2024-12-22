using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ȭ�������� ���� ������ �����ϴ� Ŭ����
public class Flamethrow_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy; // ���� ��ü ����

    private float damageCooldown; // ���� ���� ����
    private float lastTimeDamaged; // ���������� ���ظ� ������ �ð�
    private int flameDamage; // ȭ�� ���ط�

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>(); // �θ� ��ü���� ���� ������Ʈ ��������
        damageCooldown = enemy.flameDamageCooldown; // ���� ���� �ʱ�ȭ
        flameDamage = enemy.flameDamage; // ȭ�� ���ط� �ʱ�ȭ
    }

    private void OnTriggerStay(Collider other)
    {
        // ȭ�� ���Ⱑ Ȱ��ȭ���� ���� ��� ����
        if (!enemy.flamethrowActive)
            return;

        // ���� ���� �������� ���ظ� �������� ����
        if (Time.time - lastTimeDamaged < damageCooldown)
            return;

        // ���ظ� ���� �� �ִ� ������� Ȯ��
        IDamagable damagable = other.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.TakeDamage(flameDamage); // ��󿡰� ���� ����
            lastTimeDamaged = Time.time; // ������ ���� �ð� ����
            damageCooldown = enemy.flameDamageCooldown; // ���� ���� ���� (�׽�Ʈ ����)
        }
    }
}
