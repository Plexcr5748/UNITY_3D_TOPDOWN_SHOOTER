using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HitBox: �������� ���� �� �ִ� ��Ʈ�ڽ� Ŭ����
public class HitBox : MonoBehaviour, IDamagable
{
    [SerializeField] protected float damageMultiplier = 1f;
    // ������ ���: ��Ʈ�ڽ��� ���� �������� �����ϴ� �� ���

    protected virtual void Awake()
    {
    }

    public virtual void TakeDamage(int damage)
    {
        // �������� ���� �� ����� ����
    }
}
