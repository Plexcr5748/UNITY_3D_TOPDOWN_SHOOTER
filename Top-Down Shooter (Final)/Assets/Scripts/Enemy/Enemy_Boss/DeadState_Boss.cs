using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스의 사망 상태를 처리하는 클래스
public class DeadState_Boss : EnemyState
{
    private Enemy_Boss enemy; // 보스 객체 참조
    private bool interactionDisabled; // 상호작용 비활성화 여부

    public DeadState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy를 Enemy_Boss로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        // 사망 상태 진입 시 화염 방사기 비활성화
        enemy.abilityState.DisableFlamethrower();

        interactionDisabled = false; // 상호작용 초기화
        stateTimer = 1.5f; // 상태 유지 시간 설정
    }

    public override void Update()
    {
        base.Update();
        // 주석 해제 시, 상호작용 비활성화 처리
        // DisableInteractionIfShould();
    }

    // 상호작용 비활성화 처리
    private void DisableInteractionIfShould()
    {
        if (stateTimer < 0 && !interactionDisabled)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false); // 래그돌 비활성화
            enemy.ragdoll.CollidersActive(false); // 충돌체 비활성화
        }
    }
}
