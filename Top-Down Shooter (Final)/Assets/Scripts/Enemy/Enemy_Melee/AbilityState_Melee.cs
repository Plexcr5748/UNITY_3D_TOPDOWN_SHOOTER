using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� �ɷ��� �����ϴ� Ŭ���� (��: ���� ������)
public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy; // ���� ���°� �����ϴ� Enemy_Melee ��ü
    private Vector3 movementDirection; // �̵� ����
    private const float MAX_MOVEMENT_DISTANCE = 20; // �ִ� �̵� �Ÿ�
    private float moveSpeed; // �̵� �ӵ�

    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy�� Enemy_Melee�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableWeaponModel(true); // ���� �� Ȱ��ȭ

        moveSpeed = enemy.walkSpeed; // ���� �̵� �ӵ� ����
        movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE); // �̵� ���� ����
    }

    public override void Exit()
    {
        base.Exit();
        enemy.walkSpeed = moveSpeed; // �̵� �ӵ� ����
        enemy.anim.SetFloat("RecoveryIndex", 0); // ȸ�� �ε��� �ʱ�ȭ
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive()) // ���� ȸ�� Ȱ��ȭ ���¶��
        {
            enemy.FaceTarget(enemy.player.position); // ���� �÷��̾ �ٶ󺸵��� ����
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE); // �̵� ���� ����
        }

        if (enemy.ManualMovementActive()) // ���� �̵� Ȱ��ȭ ���¶��
        {
            enemy.transform.position =
                Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.walkSpeed * Time.deltaTime); // �̵�
        }

        if (triggerCalled) // Ʈ���Ű� ȣ��� ���
            stateMachine.ChangeState(enemy.recoveryState); // ȸ�� ���·� ��ȯ
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        enemy.ThrowAxe(); // �ɷ� Ʈ���� �� ���� ������ ����
    }
}
