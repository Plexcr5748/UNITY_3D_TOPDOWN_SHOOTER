using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy; // 현재 상태가 참조하는 Enemy_Melee 객체

    // 상태 초기화 및 Enemy_Melee 참조 설정
    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy를 Enemy_Melee로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true; // 적 이동 중단
    }

    public override void Exit()
    {
        base.Exit(); // 기본 Exit 로직 호출
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position); // 플레이어를 바라보도록 설정

        if (triggerCalled) // 트리거가 호출된 경우
        {
            if (enemy.CanThrowAxe()) // 도끼를 던질 수 있다면
            {
                stateMachine.ChangeState(enemy.abilityState); // 능력 상태로 전환
            }
            else if (enemy.PlayerInAttackRange()) // 플레이어가 공격 범위 내에 있다면
            {
                stateMachine.ChangeState(enemy.attackState); // 공격 상태로 전환
            }
            else
                stateMachine.ChangeState(enemy.chaseState); // 아니면 추적 상태로 전환
        }
    }
}
