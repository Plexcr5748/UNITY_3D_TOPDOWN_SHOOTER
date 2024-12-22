using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy; // 현재 상태가 참조하는 Enemy_Melee 객체
    private Vector3 destination; // 이동 목적지

    // 상태 초기화 및 Enemy_Melee 참조 설정
    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy를 Enemy_Melee로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.walkSpeed; // 적 이동 속도 설정

        destination = enemy.GetPatrolDestination(); // 순찰 목적지 설정
        enemy.agent.SetDestination(destination); // 네비게이션 목적지 설정
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint()); // 적이 다음 이동 지점을 바라보도록 설정

        // 목적지에 도착하면 상태를 Idle 상태로 변경
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .05f)
            stateMachine.ChangeState(enemy.idleState);
    }
}
