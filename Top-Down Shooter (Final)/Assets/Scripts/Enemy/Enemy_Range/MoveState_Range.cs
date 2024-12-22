using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 적이 이동하는 상태를 관리하는 클래스
public class MoveState_Range : EnemyState
{
    private Enemy_Range enemy; // 현재 상태가 참조하는 Enemy_Range 객체
    private Vector3 destination; // 목표 이동 지점

    public MoveState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy를 Enemy_Range로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        // 이동 속도 설정
        enemy.agent.speed = enemy.walkSpeed;

        // 목표 지점 설정
        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination); // 네비게이션 에이전트에 목표 지점 설정
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 목표 지점으로 얼굴을 향하도록 설정
        enemy.FaceTarget(GetNextPathPoint());

        // 목표 지점에 도달했으면 대기 상태로 전환
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .05f)
            stateMachine.ChangeState(enemy.idleState);
    }
}
