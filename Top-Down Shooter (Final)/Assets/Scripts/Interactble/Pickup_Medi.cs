using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ü�� ȸ�� ������ Ŭ����
public class Pickup_Medi : Interactable
{
    // ȸ����
    public int HealAmount = 50;

    // ��ȣ�ۿ� �� ����
    public override void Interaction()
    {
        // "Player" �±׸� ���� ������Ʈ �˻�
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // �÷��̾��� ü�� ����
            Player_Health playerHealth = player.GetComponent<Player_Health>();
            playerHealth.currentHealth += HealAmount;

            // �ִ� ü���� �ʰ����� �ʵ��� ����
            if (playerHealth.currentHealth > playerHealth.maxHealth)
                playerHealth.currentHealth = playerHealth.maxHealth;

            // UI ������Ʈ
            UI.instance.inGameUI.UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
        }
        else
        {
            // �÷��̾ ���� ��� ��� �޽��� ���
            Debug.LogWarning("No GameObject with the 'Player' tag found.");
        }

        // �������� ������Ʈ Ǯ�� ��ȯ
        ObjectPool.instance.ReturnObject(gameObject);
    }
}
