using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스의 점프 공격 상태를 처리하는 클래스
public class JumpAttackState_Boss : EnemyState
{
    private Enemy_Boss enemy; // 보스 객체 참조
    private Vector3 lastPlayerPos; // 마지막으로 플레이어가 있던 위치

    private float jumpAttackMovementSpeed; // 점프 공격 이동 속도

    public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy를 Enemy_Boss로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        lastPlayerPos = enemy.player.position; // 플레이어 위치 저장
        enemy.agent.isStopped = true; // 네비게이션 중지
        enemy.agent.velocity = Vector3.zero; // 이동 속도 초기화

        enemy.bossVisuals.PlaceLandindZone(lastPlayerPos); // 착지 위치 표시
        enemy.bossVisuals.EnableWeaponTrail(true); // 무기 트레일 활성화

        float distanceToPlayer = Vector3.Distance(lastPlayerPos, enemy.transform.position);
        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget; // 이동 속도 계산

        enemy.FaceTarget(lastPlayerPos, 1000); // 플레이어 방향으로 회전

        if (enemy.bossWeaponType == BossWeaponType.Hummer)
        {
            enemy.agent.isStopped = false; // 네비게이션 재시작
            enemy.agent.speed = enemy.runSpeed; // 뛰는 속도로 설정
            enemy.agent.SetDestination(lastPlayerPos); // 플레이어 위치를 목표로 설정
        }
    }

    public override void Update()
    {
        base.Update();

        Vector3 myPos = enemy.transform.position; // 현재 위치
        enemy.agent.enabled = !enemy.ManualMovementActive(); // 수동 이동 활성 상태에 따라 네비게이션 활성화/비활성화

        if (enemy.ManualMovementActive())
        {
            enemy.agent.velocity = Vector3.zero; // 속도 초기화
            enemy.transform.position = Vector3.MoveTowards(myPos, lastPlayerPos, jumpAttackMovementSpeed * Time.deltaTime); // 수동 이동
        }

        if (triggerCalled) // 애니메이션 트리거가 호출되면 상태 전환
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetJumpAttackOnCooldown(); // 점프 공격 쿨다운 설정
        enemy.bossVisuals.EnableWeaponTrail(false); // 무기 트레일 비활성화
    }
}
