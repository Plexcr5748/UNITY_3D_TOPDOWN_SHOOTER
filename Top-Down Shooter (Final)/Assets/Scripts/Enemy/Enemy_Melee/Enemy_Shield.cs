using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���и� �����ϴ� Ŭ����
public class Enemy_Shield : MonoBehaviour, IDamagable
{
    private Enemy_Melee enemy; // ���и� ���� ���� ����
    [SerializeField] private int durability; // ���� ������

    // �ʱ�ȭ
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>(); // �θ� ��ü���� �� ���� ��������
        durability = enemy.shieldDurability; // ���� ���� �������� ����
    }

    // ���� ������ ����
    public void ReduceDurability(int damage)
    {
        durability -= damage;

        // �������� 0 ���ϰ� �Ǹ� ���� ��Ȱ��ȭ
        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0); // �⺻ ���� �ִϸ��̼� Ȱ��ȭ
            gameObject.SetActive(false); // ���� ������Ʈ ��Ȱ��ȭ
        }
    }

    // ���ظ� �޾��� �� ������ ���� ó��
    public void TakeDamage(int damage)
    {
        ReduceDurability(damage);
    }
}
