using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 적의 사망 상태를 관리하는 클래스
public class DeadState_Range : EnemyState
{
    private Enemy_Range enemy; // 현재 상태가 참조하는 Enemy_Range 객체
    private bool interactionDisabled; // 상호작용 비활성화 여부

    public DeadState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy를 Enemy_Range로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        // 만약 수류탄을 아직 던지지 않았다면 던짐
        if (enemy.throwGrenadeState.finishedThrowingGrenade == false)
            enemy.ThrowGrenade();

        interactionDisabled = false; // 상호작용을 다시 활성화

        // 사망 후 일정 시간 동안 대기
        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();

        // 사망 후 상호작용을 비활성화하려면 아래 함수 활성화
        // DisableInteractionIfShould();
    }

    private void DisableInteractionIfShould()
    {
        // 일정 시간이 지나면 상호작용 비활성화
        if (stateTimer < 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false); // 래그돌 비활성화
            enemy.ragdoll.CollidersActive(false); // 충돌체 비활성화
        }
    }
}
