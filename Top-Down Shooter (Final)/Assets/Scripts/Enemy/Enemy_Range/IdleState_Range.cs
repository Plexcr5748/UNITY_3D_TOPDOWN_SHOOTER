using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 적이 대기하는 상태를 관리하는 클래스
public class IdleState_Range : EnemyState
{
    private Enemy_Range enemy; // 현재 상태가 참조하는 Enemy_Range 객체

    public IdleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy를 Enemy_Range로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        // 대기 애니메이션 인덱스 설정 (0~3까지 랜덤 선택)
        enemy.anim.SetFloat("IdleAnimIndex", Random.Range(0, 3));

        // IK 활성화 (보조 무기 및 무기 위치 조정)
        enemy.visuals.EnableIK(true, false);

        // 무기가 권총인 경우 IK 비활성화
        if (enemy.weaponType == Enemy_RangeWeaponType.Pistol)
            enemy.visuals.EnableIK(false, false);

        // 대기 상태 타이머 설정
        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        // 대기 시간이 끝나면 이동 상태로 전환
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}
