using UnityEngine;

// 보스의 공격 상태를 처리하는 클래스
public class AttackState_Boss : EnemyState
{
    private Enemy_Boss enemy; // 보스 객체 참조
    public float lastTimeAttacked { get; private set; } // 마지막 공격 시간

    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy를 Enemy_Boss로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        // 무기 트레일 활성화
        enemy.bossVisuals.EnableWeaponTrail(true);

        // 랜덤 공격 애니메이션 설정 (0 또는 1)
        enemy.anim.SetFloat("AttackAnimIndex", Random.Range(0, 2));

        // 네비게이션 정지
        enemy.agent.isStopped = true;

        // 상태 지속 시간 초기화
        stateTimer = 1f;
    }

    public override void Update()
    {
        base.Update();

        // 상태 타이머가 남아 있는 동안 플레이어를 향해 회전
        if (stateTimer > 0)
            enemy.FaceTarget(enemy.player.position, 20);

        // 애니메이션 트리거 호출 시 상태 전환
        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.idleState); // 공격 범위 내에서 대기 상태로 전환
            else
                stateMachine.ChangeState(enemy.moveState); // 플레이어를 추적하는 이동 상태로 전환
        }
    }

    public override void Exit()
    {
        base.Exit();

        // 마지막 공격 시간 업데이트
        lastTimeAttacked = Time.time;

        // 무기 트레일 비활성화
        enemy.bossVisuals.EnableWeaponTrail(false);
    }
}
