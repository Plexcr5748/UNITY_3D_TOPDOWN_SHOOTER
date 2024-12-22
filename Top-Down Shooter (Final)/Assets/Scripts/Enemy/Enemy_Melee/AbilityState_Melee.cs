using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 근접 적의 능력을 관리하는 클래스 (예: 도끼 던지기)
public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy; // 현재 상태가 참조하는 Enemy_Melee 객체
    private Vector3 movementDirection; // 이동 방향
    private const float MAX_MOVEMENT_DISTANCE = 20; // 최대 이동 거리
    private float moveSpeed; // 이동 속도

    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy를 Enemy_Melee로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableWeaponModel(true); // 무기 모델 활성화

        moveSpeed = enemy.walkSpeed; // 현재 이동 속도 저장
        movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE); // 이동 방향 설정
    }

    public override void Exit()
    {
        base.Exit();
        enemy.walkSpeed = moveSpeed; // 이동 속도 복원
        enemy.anim.SetFloat("RecoveryIndex", 0); // 회복 인덱스 초기화
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive()) // 수동 회전 활성화 상태라면
        {
            enemy.FaceTarget(enemy.player.position); // 적이 플레이어를 바라보도록 설정
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE); // 이동 방향 갱신
        }

        if (enemy.ManualMovementActive()) // 수동 이동 활성화 상태라면
        {
            enemy.transform.position =
                Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.walkSpeed * Time.deltaTime); // 이동
        }

        if (triggerCalled) // 트리거가 호출된 경우
            stateMachine.ChangeState(enemy.recoveryState); // 회복 상태로 전환
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        enemy.ThrowAxe(); // 능력 트리거 시 도끼 던지기 수행
    }
}
