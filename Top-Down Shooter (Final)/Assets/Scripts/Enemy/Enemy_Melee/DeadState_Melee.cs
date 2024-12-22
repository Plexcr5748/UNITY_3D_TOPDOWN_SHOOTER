using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적이 사망했을 때의 상태를 관리하는 클래스
public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy; // 현재 상태가 참조하는 Enemy_Melee 객체
    private bool interactionDisabled; // 상호작용 비활성화 여부

    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy를 Enemy_Melee로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        interactionDisabled = false; // 초기화: 상호작용 활성 상태 유지
        stateTimer = 1.5f; // 상태 지속 시간 설정
    }

    public override void Exit()
    {
        base.Exit(); // 기본 Exit 로직 호출
    }

    public override void Update()
    {
        base.Update();

        // 필요한 경우 상호작용을 비활성화
        // DisableInteractionIfShould(); 
    }

    private void DisableInteractionIfShould()
    {
        // 사망 상태에서 상호작용 비활성화
        if (stateTimer < 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false); // 래그돌 비활성화
            enemy.ragdoll.CollidersActive(false); // 충돌체 비활성화
        }
    }
}
