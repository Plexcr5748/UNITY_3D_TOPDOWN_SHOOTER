using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ��� ���¸� ó���ϴ� Ŭ����
public class IdleState_Boss : EnemyState
{
    private Enemy_Boss enemy; // ���� ��ü ����

    public IdleState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy�� Enemy_Boss�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime; // ��� ���� ���� �ð� ����
    }

    public override void Update()
    {
        base.Update();

        // ���� ��忡�� �÷��̾ ���� ���� ���� �ִ� ��� ���� ���·� ��ȯ
        if (enemy.inBattleMode && enemy.PlayerInAttackRange())
            stateMachine.ChangeState(enemy.attackState);

        // ��� �ð��� ������ �̵� ���·� ��ȯ
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}
