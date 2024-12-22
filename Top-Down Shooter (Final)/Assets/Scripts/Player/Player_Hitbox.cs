using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_Hitbox: �÷��̾��� ��Ʈ�ڽ��� �����ϸ� �������� ó���ϴ� Ŭ����
public class Player_Hitbox : HitBox
{
    private Player player; // �÷��̾� ��ü ����

    protected override void Awake()
    {
        // �θ� Ŭ������ Awake ȣ��
        base.Awake();

        // �θ� ��ü���� Player ������Ʈ ��������
        player = GetComponentInParent<Player>();
    }

    public override void TakeDamage(int damage)
    {
        // �������� ������ ������ ����
        int newDamage = Mathf.RoundToInt(damage * damageMultiplier);

        // �÷��̾��� ü���� ���ҽ�Ŵ
        player.health.ReduceHealth(newDamage);
    }
}
