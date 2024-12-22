using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ÿ� ���� ��� ���¸� �����ϴ� Ŭ����
public class DeadState_Range : EnemyState
{
    private Enemy_Range enemy; // ���� ���°� �����ϴ� Enemy_Range ��ü
    private bool interactionDisabled; // ��ȣ�ۿ� ��Ȱ��ȭ ����

    public DeadState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy�� Enemy_Range�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        // ���� ����ź�� ���� ������ �ʾҴٸ� ����
        if (enemy.throwGrenadeState.finishedThrowingGrenade == false)
            enemy.ThrowGrenade();

        interactionDisabled = false; // ��ȣ�ۿ��� �ٽ� Ȱ��ȭ

        // ��� �� ���� �ð� ���� ���
        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();

        // ��� �� ��ȣ�ۿ��� ��Ȱ��ȭ�Ϸ��� �Ʒ� �Լ� Ȱ��ȭ
        // DisableInteractionIfShould();
    }

    private void DisableInteractionIfShould()
    {
        // ���� �ð��� ������ ��ȣ�ۿ� ��Ȱ��ȭ
        if (stateTimer < 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false); // ���׵� ��Ȱ��ȭ
            enemy.ragdoll.CollidersActive(false); // �浹ü ��Ȱ��ȭ
        }
    }
}
