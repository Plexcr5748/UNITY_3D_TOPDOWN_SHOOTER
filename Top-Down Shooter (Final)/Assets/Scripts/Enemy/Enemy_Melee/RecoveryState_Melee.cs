using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy; // ���� ���°� �����ϴ� Enemy_Melee ��ü

    // ���� �ʱ�ȭ �� Enemy_Melee ���� ����
    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy�� Enemy_Melee�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true; // �� �̵� �ߴ�
    }

    public override void Exit()
    {
        base.Exit(); // �⺻ Exit ���� ȣ��
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position); // �÷��̾ �ٶ󺸵��� ����

        if (triggerCalled) // Ʈ���Ű� ȣ��� ���
        {
            if (enemy.CanThrowAxe()) // ������ ���� �� �ִٸ�
            {
                stateMachine.ChangeState(enemy.abilityState); // �ɷ� ���·� ��ȯ
            }
            else if (enemy.PlayerInAttackRange()) // �÷��̾ ���� ���� ���� �ִٸ�
            {
                stateMachine.ChangeState(enemy.attackState); // ���� ���·� ��ȯ
            }
            else
                stateMachine.ChangeState(enemy.chaseState); // �ƴϸ� ���� ���·� ��ȯ
        }
    }
}
