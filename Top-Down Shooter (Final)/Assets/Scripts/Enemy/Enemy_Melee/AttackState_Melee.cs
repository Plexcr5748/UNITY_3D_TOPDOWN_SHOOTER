using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적의 근접 공격 상태를 관리하는 클래스
public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy; // 현재 상태가 참조하는 Enemy_Melee 객체
    private Vector3 attackDirection; // 공격 방향
    private float attackMoveSpeed; // 공격 이동 속도

    private const float MAX_ATTACK_DISTANCE = 50f; // 최대 공격 거리

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy를 Enemy_Melee로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();
        enemy.UpdateAttackData(); // 공격 데이터를 갱신
        enemy.visuals.EnableWeaponModel(true); // 무기 모델 활성화
        enemy.visuals.EnableWeaponTrail(true); // 무기 효과 활성화

        attackMoveSpeed = enemy.attackData.moveSpeed; // 공격 이동 속도 설정
        enemy.anim.SetFloat("AttackAnimationSpeed", enemy.attackData.animationSpeed); // 애니메이션 속도 설정
        enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex); // 공격 애니메이션 인덱스 설정
        enemy.anim.SetFloat("SlashAttackIndex", Random.Range(0, 6)); // 무작위 슬래시 공격 선택 (0~5)

        enemy.agent.isStopped = true; // 이동 중단
        enemy.agent.velocity = Vector3.zero; // 이동 속도 초기화

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE); // 공격 방향 설정
    }

    public override void Exit()
    {
        base.Exit();
        SetupNextAttack(); // 다음 공격 설정
        enemy.visuals.EnableWeaponTrail(false); // 무기 효과 비활성화
    }

    private void SetupNextAttack()
    {
        int recoveryIndex = PlayerClose() ? 1 : 0; // 플레이어가 가까우면 회복 인덱스를 1로 설정
        enemy.anim.SetFloat("RecoveryIndex", recoveryIndex);
        enemy.attackData = UpdatedAttackData(); // 새로운 공격 데이터 설정
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive()) // 수동 회전 활성화 상태라면
        {
            enemy.FaceTarget(enemy.player.position); // 적이 플레이어를 바라보도록 설정
            attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE); // 공격 방향 갱신
        }

        if (enemy.ManualMovementActive()) // 수동 이동 활성화 상태라면
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime); // 이동
        }

        if (triggerCalled) // 트리거가 호출된 경우
        {
            if (enemy.CanThrowAxe()) // 도끼를 던질 수 있으면 능력 상태로 전환
                stateMachine.ChangeState(enemy.abilityState);
            else if (enemy.PlayerInAttackRange()) // 공격 범위 내라면 회복 상태로 전환
                stateMachine.ChangeState(enemy.recoveryState);
            else // 아니면 추적 상태로 전환
                stateMachine.ChangeState(enemy.chaseState);
        }
    }

    private bool PlayerClose() => Vector3.Distance(enemy.transform.position, enemy.player.position) <= 1; // 플레이어가 가까운지 확인

    private AttackData_EnemyMelee UpdatedAttackData()
    {
        List<AttackData_EnemyMelee> validAttacks = new List<AttackData_EnemyMelee>(enemy.attackList); // 유효한 공격 데이터 리스트 생성

        if (PlayerClose()) // 플레이어가 가까우면 돌격 공격 제외
            validAttacks.RemoveAll(parameter => parameter.attackType == AttackType_Melee.Charge);

        int random = Random.Range(0, validAttacks.Count); // 유효한 공격 중 무작위로 선택
        return validAttacks[random];
    }
}
