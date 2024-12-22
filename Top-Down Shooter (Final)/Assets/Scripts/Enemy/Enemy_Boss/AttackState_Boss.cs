using UnityEngine;

// ������ ���� ���¸� ó���ϴ� Ŭ����
public class AttackState_Boss : EnemyState
{
    private Enemy_Boss enemy; // ���� ��ü ����
    public float lastTimeAttacked { get; private set; } // ������ ���� �ð�

    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy�� Enemy_Boss�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        // ���� Ʈ���� Ȱ��ȭ
        enemy.bossVisuals.EnableWeaponTrail(true);

        // ���� ���� �ִϸ��̼� ���� (0 �Ǵ� 1)
        enemy.anim.SetFloat("AttackAnimIndex", Random.Range(0, 2));

        // �׺���̼� ����
        enemy.agent.isStopped = true;

        // ���� ���� �ð� �ʱ�ȭ
        stateTimer = 1f;
    }

    public override void Update()
    {
        base.Update();

        // ���� Ÿ�̸Ӱ� ���� �ִ� ���� �÷��̾ ���� ȸ��
        if (stateTimer > 0)
            enemy.FaceTarget(enemy.player.position, 20);

        // �ִϸ��̼� Ʈ���� ȣ�� �� ���� ��ȯ
        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.idleState); // ���� ���� ������ ��� ���·� ��ȯ
            else
                stateMachine.ChangeState(enemy.moveState); // �÷��̾ �����ϴ� �̵� ���·� ��ȯ
        }
    }

    public override void Exit()
    {
        base.Exit();

        // ������ ���� �ð� ������Ʈ
        lastTimeAttacked = Time.time;

        // ���� Ʈ���� ��Ȱ��ȭ
        enemy.bossVisuals.EnableWeaponTrail(false);
    }
}
