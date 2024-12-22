using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ÿ� ���� �÷��̾�� �����ϴ� ���¸� �����ϴ� Ŭ����
public class AdvancePlayerState_Range : EnemyState
{
    private Enemy_Range enemy; // ���� ���°� �����ϴ� Enemy_Range ��ü
    private Vector3 playerPos; // �÷��̾��� ��ġ

    public float lastTimeAdvanced { get; private set; } // ���������� ������ �ð�

    public AdvancePlayerState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy�� Enemy_Range�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        // IK Ȱ��ȭ (���� �� ĳ���� ��ġ ����)
        enemy.visuals.EnableIK(true, true);

        // ������Ʈ �̵� ����
        enemy.agent.isStopped = false; // �̵� ���� ����
        enemy.agent.speed = enemy.advanceSpeed; // ���� �ӵ� ����

        // ���� �Ұ� ������ ��� �߰� ����
        if (enemy.IsUnstopppable())
        {
            enemy.visuals.EnableIK(true, false); // IK ���� ����
            stateTimer = enemy.advanceDuration; // ���� ���� �ð� ����
        }
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeAdvanced = Time.time; // ���� ���� �ð��� ���
    }

    public override void Update()
    {
        base.Update();

        // �÷��̾� ��ġ ������Ʈ
        playerPos = enemy.player.transform.position;
        enemy.UpdateAimPosition(); // ���� ��ġ ������Ʈ

        // �÷��̾ ���� �̵�
        enemy.agent.SetDestination(playerPos);
        enemy.FaceTarget(GetNextPathPoint()); // ��ǥ ������ ���ϵ��� �� ȸ��

        // ���� ���·� ��ȯ�� ������ �����Ǹ� ���� ����
        if (CanEnterBattleState() && enemy.IsSeeingPlayer())
            stateMachine.ChangeState(enemy.battleState);
    }

    private bool CanEnterBattleState()
    {
        // �÷��̾���� �Ÿ� Ȯ��
        bool closeEnoughToPlayer = Vector3.Distance(enemy.transform.position, playerPos) < enemy.advanceStoppingDistance;

        // ���� �Ұ� ���¶�� ������ �����ų�, �÷��̾�� ����� ��������� ���� ���·� ��ȯ
        if (enemy.IsUnstopppable())
            return closeEnoughToPlayer || stateTimer < 0;
        else
            return closeEnoughToPlayer; // ���� �Ұ� ���°� �ƴϸ� �÷��̾�� ��������� ���� ���� ���·� ��ȯ
    }
}
