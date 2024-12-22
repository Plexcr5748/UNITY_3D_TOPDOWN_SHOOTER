using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 적이 플레이어에게 전진하는 상태를 관리하는 클래스
public class AdvancePlayerState_Range : EnemyState
{
    private Enemy_Range enemy; // 현재 상태가 참조하는 Enemy_Range 객체
    private Vector3 playerPos; // 플레이어의 위치

    public float lastTimeAdvanced { get; private set; } // 마지막으로 전진한 시간

    public AdvancePlayerState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy를 Enemy_Range로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        // IK 활성화 (무기 및 캐릭터 위치 조정)
        enemy.visuals.EnableIK(true, true);

        // 에이전트 이동 설정
        enemy.agent.isStopped = false; // 이동 중지 해제
        enemy.agent.speed = enemy.advanceSpeed; // 전진 속도 설정

        // 방해 불가 상태일 경우 추가 설정
        if (enemy.IsUnstopppable())
        {
            enemy.visuals.EnableIK(true, false); // IK 설정 변경
            stateTimer = enemy.advanceDuration; // 전진 지속 시간 설정
        }
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeAdvanced = Time.time; // 전진 종료 시간을 기록
    }

    public override void Update()
    {
        base.Update();

        // 플레이어 위치 업데이트
        playerPos = enemy.player.transform.position;
        enemy.UpdateAimPosition(); // 조준 위치 업데이트

        // 플레이어를 향해 이동
        enemy.agent.SetDestination(playerPos);
        enemy.FaceTarget(GetNextPathPoint()); // 목표 지점을 향하도록 얼굴 회전

        // 전투 상태로 전환할 조건이 충족되면 상태 변경
        if (CanEnterBattleState() && enemy.IsSeeingPlayer())
            stateMachine.ChangeState(enemy.battleState);
    }

    private bool CanEnterBattleState()
    {
        // 플레이어와의 거리 확인
        bool closeEnoughToPlayer = Vector3.Distance(enemy.transform.position, playerPos) < enemy.advanceStoppingDistance;

        // 방해 불가 상태라면 전진이 끝났거나, 플레이어와 충분히 가까워지면 전투 상태로 전환
        if (enemy.IsUnstopppable())
            return closeEnoughToPlayer || stateTimer < 0;
        else
            return closeEnoughToPlayer; // 방해 불가 상태가 아니면 플레이어와 가까워졌을 때만 전투 상태로 전환
    }
}
