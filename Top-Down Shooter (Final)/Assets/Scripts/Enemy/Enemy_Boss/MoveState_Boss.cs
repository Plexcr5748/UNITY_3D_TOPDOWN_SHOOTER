using UnityEngine;

// 보스의 이동 상태를 처리하는 클래스
public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy; // 보스 적 참조
    private Vector3 destination; // 이동 목표 지점

    private float actionTimer; // 행동 타이머
    private float timeBeforeSpeedUp = 5; // 속도 증가 전 대기 시간

    private bool speedUpActivated; // 속도 증가 활성화 여부

    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy를 Enemy_Boss로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        SpeedReset(); // 이동 속도 초기화
        enemy.agent.isStopped = false; // 이동 시작

        destination = enemy.GetPatrolDestination(); // 순찰 목적지 설정
        enemy.agent.SetDestination(destination);

        actionTimer = enemy.actionCooldown; // 행동 타이머 초기화
    }

    // 이동 속도 및 애니메이션 초기화
    private void SpeedReset()
    {
        speedUpActivated = false;
        enemy.anim.SetFloat("MoveAnimSpeedMultiplier", 1);
        enemy.anim.SetFloat("MoveAnimIndex", 0);
        enemy.agent.speed = enemy.walkSpeed;
    }

    public override void Update()
    {
        base.Update();

        actionTimer -= Time.deltaTime; // 행동 타이머 감소
        enemy.FaceTarget(GetNextPathPoint()); // 이동 방향으로 회전

        if (enemy.inBattleMode)
        {
            if (ShouldSpeedUp())
                SpeedUp(); // 속도 증가 처리

            Vector3 playerPos = enemy.player.position;
            enemy.agent.SetDestination(playerPos); // 플레이어를 목표로 이동

            if (actionTimer < 0)
            {
                PerformRandomAction(); // 랜덤 행동 수행
            }
            else if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.attackState); // 공격 상태로 전환
        }
        else
        {
            if (Vector3.Distance(enemy.transform.position, destination) < .25f)
                stateMachine.ChangeState(enemy.idleState); // 도착 시 대기 상태로 전환
        }
    }

    // 속도 증가 처리
    private void SpeedUp()
    {
        enemy.agent.speed = enemy.runSpeed; // 속도 증가
        enemy.anim.SetFloat("MoveAnimIndex", 1); // 이동 애니메이션 변경
        enemy.anim.SetFloat("MoveAnimSpeedMultiplier", 1.5f); // 애니메이션 속도 증가
        speedUpActivated = true; // 속도 증가 활성화
    }

    // 랜덤 행동 수행
    private void PerformRandomAction()
    {
        actionTimer = enemy.actionCooldown; // 행동 타이머 초기화

        if (Random.Range(0, 2) == 0) // 0 또는 1의 랜덤 값 생성
        {
            TryAbility(); // 능력 사용 시도
        }
        else
        {
            if (enemy.CanDoJumpAttack())
                stateMachine.ChangeState(enemy.jumpAttackState); // 점프 공격 상태로 전환
            else if (enemy.bossWeaponType == BossWeaponType.Hummer)
                TryAbility(); // 망치 능력 사용 시도
        }
    }

    // 능력 사용 시도
    private void TryAbility()
    {
        if (enemy.CanDoAbility())
            stateMachine.ChangeState(enemy.abilityState); // 능력 상태로 전환
    }

    // 속도 증가 조건 확인
    private bool ShouldSpeedUp()
    {
        if (speedUpActivated)
            return false;

        if (Time.time > enemy.attackState.lastTimeAttacked + timeBeforeSpeedUp)
        {
            return true;
        }

        return false;
    }
}
