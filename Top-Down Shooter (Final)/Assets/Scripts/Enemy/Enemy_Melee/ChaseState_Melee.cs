using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적이 플레이어를 추적하는 상태를 관리하는 클래스
public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy; // 현재 상태가 참조하는 Enemy_Melee 객체
    private float lastTimeUpdatedDistanation; // 마지막으로 목적지를 갱신한 시간

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy를 Enemy_Melee로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.runSpeed; // 추적 속도 설정
        enemy.agent.isStopped = false; // 추적 활성화
    }

    public override void Exit()
    {
        base.Exit(); // 기본 Exit 로직 호출
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange()) // 플레이어가 공격 범위에 들어오면
            stateMachine.ChangeState(enemy.attackState); // 공격 상태로 전환

        enemy.FaceTarget(GetNextPathPoint()); // 적이 플레이어를 바라보도록 설정

        if (CanUpdateDestination()) // 목적지를 갱신할 수 있는지 확인
        {
            enemy.agent.destination = enemy.player.transform.position; // 플레이어의 현재 위치로 목적지 갱신
        }
    }

    private bool CanUpdateDestination()
    {
        // 마지막 갱신 후 0.25초가 지났으면 true 반환
        if (Time.time > lastTimeUpdatedDistanation + .25f)
        {
            lastTimeUpdatedDistanation = Time.time; // 갱신 시간 업데이트
            return true;
        }

        return false; // 아직 갱신할 수 없는 경우
    }
}
