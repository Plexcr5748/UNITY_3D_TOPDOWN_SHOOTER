using UnityEngine;

// 보스의 능력 사용 상태를 처리하는 클래스
public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy; // 보스 객체 참조

    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy를 Enemy_Boss로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        // 상태 타이머를 화염 방사 지속 시간으로 설정
        stateTimer = enemy.flamethrowDuration;

        // 네비게이션 정지 및 초기화
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        // 무기 트레일 활성화
        enemy.bossVisuals.EnableWeaponTrail(true);
    }

    public override void Update()
    {
        base.Update();

        // 플레이어를 바라보도록 보스 회전
        enemy.FaceTarget(enemy.player.position);

        // 화염 방사기 비활성화 조건 확인
        if (ShouldDisableFlamethrower())
            DisableFlamethrower();

        // 애니메이션 트리거 호출 시 이동 상태로 전환
        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    // 화염 방사기를 비활성화할지 판단
    private bool ShouldDisableFlamethrower() => stateTimer < 0;

    // 화염 방사기 비활성화
    public void DisableFlamethrower()
    {
        if (enemy.bossWeaponType != BossWeaponType.Flamethrower)
            return;

        if (!enemy.flamethrowActive)
            return;

        enemy.ActivateFlamethrower(false);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        // 화염 방사기 능력 활성화
        if (enemy.bossWeaponType == BossWeaponType.Flamethrower)
        {
            enemy.ActivateFlamethrower(true);
            enemy.bossVisuals.DischargeBatteries(); // 배터리 방전
            enemy.bossVisuals.EnableWeaponTrail(false);
        }

        // 망치 능력 활성화
        if (enemy.bossWeaponType == BossWeaponType.Hummer)
        {
            enemy.ActivateHummer();
        }
    }

    public override void Exit()
    {
        base.Exit();

        // 능력 쿨다운 설정
        enemy.SetAbilityOnCooldown();

        // 배터리 초기화
        enemy.bossVisuals.ResetBatteries();
    }
}
