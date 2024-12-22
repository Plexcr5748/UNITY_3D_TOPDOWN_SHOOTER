using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ��Ʈ�ڽ��� ó���ϴ� Ŭ����
public class Enemy_HitBox : HitBox
{
    private Enemy enemy; // �� ĳ���Ϳ� ���� ����

    // �ʱ�ȭ
    protected override void Awake()
    {
        base.Awake(); // �θ� Ŭ������ Awake ȣ��
        enemy = GetComponentInParent<Enemy>(); // �θ� ������Ʈ���� Enemy ������Ʈ�� ������
    }

    // ���� ó��
    public override void TakeDamage(int damage)
    {
        // ���ط��� ��Ʈ�ڽ��� ������ ���� ����
        int newDamage = Mathf.RoundToInt(damage * damageMultiplier);

        // ������ ������ ���ط� ����
        enemy.GetHit(newDamage);
    }
}
