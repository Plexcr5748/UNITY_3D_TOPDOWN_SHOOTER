using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 적이 엄폐 지점으로 달려가는 상태를 관리하는 클래스
public class RunToCoverState_Range : EnemyState
{
    private Enemy_Range enemy; // 현재 상태가 참조하는 Enemy_Range 객체
    private Vector3 destination; // 목표 엄폐 지점

    public float lastTimeTookCover { get; private set; } // 마지막으로 엄폐 지점에 도달한 시간

    public RunToCoverState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy를 Enemy_Range로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        // 목표 엄폐 지점 설정
        destination = enemy.currentCover.transform.position;

        // IK 활성화하여 무기 및 캐릭터의 위치 조정
        enemy.visuals.EnableIK(true, false);

        // 이동 준비
        enemy.agent.isStopped = false; // 이동 중지 해제
        enemy.agent.speed = enemy.runSpeed; // 달리기 속도 설정
        enemy.agent.SetDestination(destination); // 목표 지점 설정
    }

    public override void Exit()
    {
        base.Exit();

        // 엄폐 지점에 도달한 시간을 기록
        lastTimeTookCover = Time.time;
    }

    public override void Update()
    {
        base.Update();

        // 목표 지점으로 얼굴을 향하도록 설정
        enemy.FaceTarget(GetNextPathPoint());

        // 목표 지점에 도달했으면 전투 상태로 전환
        if (Vector3.Distance(enemy.transform.position, destination) < .8f)
            stateMachine.ChangeState(enemy.battleState);
    }
}
