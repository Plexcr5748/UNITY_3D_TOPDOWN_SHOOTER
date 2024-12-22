using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 적이 수류탄을 던지는 상태를 관리하는 클래스
public class ThrowGrenadeState_Range : EnemyState
{
    private Enemy_Range enemy; // 현재 상태가 참조하는 Enemy_Range 객체
    public bool finishedThrowingGrenade { get; private set; } = true; // 수류탄 던지기 완료 여부

    public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy를 Enemy_Range로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        finishedThrowingGrenade = false; // 수류탄 던지기 시작

        // 수류탄 던지기 준비
        enemy.visuals.EnableWeaponModel(false); // 무기 모델 비활성화
        enemy.visuals.EnableIK(false, false); // IK 비활성화
        enemy.visuals.EnableSeconoderyWeaponModel(true); // 보조 무기 모델 활성화
        enemy.visuals.EnableGrenadeModel(true); // 수류탄 모델 활성화
    }

    public override void Update()
    {
        base.Update();

        // 플레이어 위치로 조준
        Vector3 playerPos = enemy.player.position + Vector3.up;
        enemy.FaceTarget(playerPos); // 플레이어를 바라보도록 설정
        enemy.aim.position = playerPos; // 조준 위치 갱신

        if (triggerCalled) // 트리거가 호출되면 전투 상태로 전환
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        finishedThrowingGrenade = true; // 수류탄 던지기 완료
        enemy.ThrowGrenade(); // 수류탄 던지기 실행
    }
}
