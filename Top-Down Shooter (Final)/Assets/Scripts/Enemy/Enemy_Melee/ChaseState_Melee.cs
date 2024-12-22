using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �÷��̾ �����ϴ� ���¸� �����ϴ� Ŭ����
public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy; // ���� ���°� �����ϴ� Enemy_Melee ��ü
    private float lastTimeUpdatedDistanation; // ���������� �������� ������ �ð�

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy�� Enemy_Melee�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.runSpeed; // ���� �ӵ� ����
        enemy.agent.isStopped = false; // ���� Ȱ��ȭ
    }

    public override void Exit()
    {
        base.Exit(); // �⺻ Exit ���� ȣ��
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange()) // �÷��̾ ���� ������ ������
            stateMachine.ChangeState(enemy.attackState); // ���� ���·� ��ȯ

        enemy.FaceTarget(GetNextPathPoint()); // ���� �÷��̾ �ٶ󺸵��� ����

        if (CanUpdateDestination()) // �������� ������ �� �ִ��� Ȯ��
        {
            enemy.agent.destination = enemy.player.transform.position; // �÷��̾��� ���� ��ġ�� ������ ����
        }
    }

    private bool CanUpdateDestination()
    {
        // ������ ���� �� 0.25�ʰ� �������� true ��ȯ
        if (Time.time > lastTimeUpdatedDistanation + .25f)
        {
            lastTimeUpdatedDistanation = Time.time; // ���� �ð� ������Ʈ
            return true;
        }

        return false; // ���� ������ �� ���� ���
    }
}
