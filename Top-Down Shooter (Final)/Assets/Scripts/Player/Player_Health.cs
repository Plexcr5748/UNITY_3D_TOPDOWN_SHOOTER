using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_Health: 플레이어의 체력 관리와 사망 상태 처리 클래스
public class Player_Health : HealthController
{
    private Player player; // 플레이어 객체 참조
    private MissionManager missionManager; // 미션 관리 매니저

    public bool isDead { get; private set; } // 플레이어 사망 여부

    protected override void Awake()
    {
        // 부모 클래스의 Awake 호출 및 플레이어 객체 초기화
        base.Awake();
        player = GetComponent<Player>();
    }

    public override void ReduceHealth(int damage)
    {
        // 데미지를 받아 체력을 감소
        base.ReduceHealth(damage);

        // 체력이 0 이하가 되면 사망 처리
        if (ShouldDie())
            Die();

        // UI를 업데이트하여 현재 체력을 표시
        UI.instance.inGameUI.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void Die()
    {
        // 이미 사망 상태라면 처리 중단
        if (isDead)
            return;

        Debug.Log("Player was killed at " + Time.time); // 사망 시간 디버그 출력
        isDead = true; // 사망 상태 설정

        player.anim.enabled = false; // 애니메이터 비활성화
        player.ragdoll.RagdollActive(true); // 레그돌 활성화

        GameManager.instance.GameOver(); // 게임 종료 처리
    }
}
