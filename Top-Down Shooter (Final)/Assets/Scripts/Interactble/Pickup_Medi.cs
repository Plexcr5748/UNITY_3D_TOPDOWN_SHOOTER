using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 체력 회복 아이템 클래스
public class Pickup_Medi : Interactable
{
    // 회복량
    public int HealAmount = 50;

    // 상호작용 시 동작
    public override void Interaction()
    {
        // "Player" 태그를 가진 오브젝트 검색
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // 플레이어의 체력 증가
            Player_Health playerHealth = player.GetComponent<Player_Health>();
            playerHealth.currentHealth += HealAmount;

            // 최대 체력을 초과하지 않도록 제한
            if (playerHealth.currentHealth > playerHealth.maxHealth)
                playerHealth.currentHealth = playerHealth.maxHealth;

            // UI 업데이트
            UI.instance.inGameUI.UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
        }
        else
        {
            // 플레이어가 없는 경우 경고 메시지 출력
            Debug.LogWarning("No GameObject with the 'Player' tag found.");
        }

        // 아이템을 오브젝트 풀로 반환
        ObjectPool.instance.ReturnObject(gameObject);
    }
}
