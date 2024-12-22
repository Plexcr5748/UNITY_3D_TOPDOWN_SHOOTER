using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Melee : EnemyState
{
    private Enemy_Melee enemy; // 현재 상태가 참조하는 Enemy_Melee 객체

    // 상태 초기화 및 Enemy_Melee 참조 설정
    public IdleState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy를 Enemy_Melee로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime; // 대기 상태 지속 시간 설정
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0) // 대기 시간이 종료되면
            stateMachine.ChangeState(enemy.moveState); // Move 상태로 전환
    }
}
