using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� ���� ���¸� �����ϴ� Ŭ����
public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy; // ���� ���°� �����ϴ� Enemy_Melee ��ü
    private Vector3 attackDirection; // ���� ����
    private float attackMoveSpeed; // ���� �̵� �ӵ�

    private const float MAX_ATTACK_DISTANCE = 50f; // �ִ� ���� �Ÿ�

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy�� Enemy_Melee�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();
        enemy.UpdateAttackData(); // ���� �����͸� ����
        enemy.visuals.EnableWeaponModel(true); // ���� �� Ȱ��ȭ
        enemy.visuals.EnableWeaponTrail(true); // ���� ȿ�� Ȱ��ȭ

        attackMoveSpeed = enemy.attackData.moveSpeed; // ���� �̵� �ӵ� ����
        enemy.anim.SetFloat("AttackAnimationSpeed", enemy.attackData.animationSpeed); // �ִϸ��̼� �ӵ� ����
        enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex); // ���� �ִϸ��̼� �ε��� ����
        enemy.anim.SetFloat("SlashAttackIndex", Random.Range(0, 6)); // ������ ������ ���� ���� (0~5)

        enemy.agent.isStopped = true; // �̵� �ߴ�
        enemy.agent.velocity = Vector3.zero; // �̵� �ӵ� �ʱ�ȭ

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE); // ���� ���� ����
    }

    public override void Exit()
    {
        base.Exit();
        SetupNextAttack(); // ���� ���� ����
        enemy.visuals.EnableWeaponTrail(false); // ���� ȿ�� ��Ȱ��ȭ
    }

    private void SetupNextAttack()
    {
        int recoveryIndex = PlayerClose() ? 1 : 0; // �÷��̾ ������ ȸ�� �ε����� 1�� ����
        enemy.anim.SetFloat("RecoveryIndex", recoveryIndex);
        enemy.attackData = UpdatedAttackData(); // ���ο� ���� ������ ����
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive()) // ���� ȸ�� Ȱ��ȭ ���¶��
        {
            enemy.FaceTarget(enemy.player.position); // ���� �÷��̾ �ٶ󺸵��� ����
            attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE); // ���� ���� ����
        }

        if (enemy.ManualMovementActive()) // ���� �̵� Ȱ��ȭ ���¶��
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime); // �̵�
        }

        if (triggerCalled) // Ʈ���Ű� ȣ��� ���
        {
            if (enemy.CanThrowAxe()) // ������ ���� �� ������ �ɷ� ���·� ��ȯ
                stateMachine.ChangeState(enemy.abilityState);
            else if (enemy.PlayerInAttackRange()) // ���� ���� ����� ȸ�� ���·� ��ȯ
                stateMachine.ChangeState(enemy.recoveryState);
            else // �ƴϸ� ���� ���·� ��ȯ
                stateMachine.ChangeState(enemy.chaseState);
        }
    }

    private bool PlayerClose() => Vector3.Distance(enemy.transform.position, enemy.player.position) <= 1; // �÷��̾ ������� Ȯ��

    private AttackData_EnemyMelee UpdatedAttackData()
    {
        List<AttackData_EnemyMelee> validAttacks = new List<AttackData_EnemyMelee>(enemy.attackList); // ��ȿ�� ���� ������ ����Ʈ ����

        if (PlayerClose()) // �÷��̾ ������ ���� ���� ����
            validAttacks.RemoveAll(parameter => parameter.attackType == AttackType_Melee.Charge);

        int random = Random.Range(0, validAttacks.Count); // ��ȿ�� ���� �� �������� ����
        return validAttacks[random];
    }
}
