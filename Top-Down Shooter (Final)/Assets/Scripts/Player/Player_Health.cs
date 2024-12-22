using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_Health: �÷��̾��� ü�� ������ ��� ���� ó�� Ŭ����
public class Player_Health : HealthController
{
    private Player player; // �÷��̾� ��ü ����
    private MissionManager missionManager; // �̼� ���� �Ŵ���

    public bool isDead { get; private set; } // �÷��̾� ��� ����

    protected override void Awake()
    {
        // �θ� Ŭ������ Awake ȣ�� �� �÷��̾� ��ü �ʱ�ȭ
        base.Awake();
        player = GetComponent<Player>();
    }

    public override void ReduceHealth(int damage)
    {
        // �������� �޾� ü���� ����
        base.ReduceHealth(damage);

        // ü���� 0 ���ϰ� �Ǹ� ��� ó��
        if (ShouldDie())
            Die();

        // UI�� ������Ʈ�Ͽ� ���� ü���� ǥ��
        UI.instance.inGameUI.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void Die()
    {
        // �̹� ��� ���¶�� ó�� �ߴ�
        if (isDead)
            return;

        Debug.Log("Player was killed at " + Time.time); // ��� �ð� ����� ���
        isDead = true; // ��� ���� ����

        player.anim.enabled = false; // �ִϸ����� ��Ȱ��ȭ
        player.ragdoll.RagdollActive(true); // ���׵� Ȱ��ȭ

        GameManager.instance.GameOver(); // ���� ���� ó��
    }
}
