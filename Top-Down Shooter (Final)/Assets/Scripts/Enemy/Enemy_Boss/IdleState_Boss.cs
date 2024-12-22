using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스의 대기 상태를 처리하는 클래스
public class IdleState_Boss : EnemyState
{
    private Enemy_Boss enemy; // 보스 객체 참조

    public IdleState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy를 Enemy_Boss로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime; // 대기 상태 지속 시간 설정
    }

    public override void Update()
    {
        base.Update();

        // 전투 모드에서 플레이어가 공격 범위 내에 있는 경우 공격 상태로 전환
        if (enemy.inBattleMode && enemy.PlayerInAttackRange())
            stateMachine.ChangeState(enemy.attackState);

        // 대기 시간이 끝나면 이동 상태로 전환
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}
