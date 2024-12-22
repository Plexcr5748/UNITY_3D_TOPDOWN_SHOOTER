using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ���� ���� ���¸� ó���ϴ� Ŭ����
public class JumpAttackState_Boss : EnemyState
{
    private Enemy_Boss enemy; // ���� ��ü ����
    private Vector3 lastPlayerPos; // ���������� �÷��̾ �ִ� ��ġ

    private float jumpAttackMovementSpeed; // ���� ���� �̵� �ӵ�

    public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy�� Enemy_Boss�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        lastPlayerPos = enemy.player.position; // �÷��̾� ��ġ ����
        enemy.agent.isStopped = true; // �׺���̼� ����
        enemy.agent.velocity = Vector3.zero; // �̵� �ӵ� �ʱ�ȭ

        enemy.bossVisuals.PlaceLandindZone(lastPlayerPos); // ���� ��ġ ǥ��
        enemy.bossVisuals.EnableWeaponTrail(true); // ���� Ʈ���� Ȱ��ȭ

        float distanceToPlayer = Vector3.Distance(lastPlayerPos, enemy.transform.position);
        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget; // �̵� �ӵ� ���

        enemy.FaceTarget(lastPlayerPos, 1000); // �÷��̾� �������� ȸ��

        if (enemy.bossWeaponType == BossWeaponType.Hummer)
        {
            enemy.agent.isStopped = false; // �׺���̼� �����
            enemy.agent.speed = enemy.runSpeed; // �ٴ� �ӵ��� ����
            enemy.agent.SetDestination(lastPlayerPos); // �÷��̾� ��ġ�� ��ǥ�� ����
        }
    }

    public override void Update()
    {
        base.Update();

        Vector3 myPos = enemy.transform.position; // ���� ��ġ
        enemy.agent.enabled = !enemy.ManualMovementActive(); // ���� �̵� Ȱ�� ���¿� ���� �׺���̼� Ȱ��ȭ/��Ȱ��ȭ

        if (enemy.ManualMovementActive())
        {
            enemy.agent.velocity = Vector3.zero; // �ӵ� �ʱ�ȭ
            enemy.transform.position = Vector3.MoveTowards(myPos, lastPlayerPos, jumpAttackMovementSpeed * Time.deltaTime); // ���� �̵�
        }

        if (triggerCalled) // �ִϸ��̼� Ʈ���Ű� ȣ��Ǹ� ���� ��ȯ
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetJumpAttackOnCooldown(); // ���� ���� ��ٿ� ����
        enemy.bossVisuals.EnableWeaponTrail(false); // ���� Ʈ���� ��Ȱ��ȭ
    }
}
