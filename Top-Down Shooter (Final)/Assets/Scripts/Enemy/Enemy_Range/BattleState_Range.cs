using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ÿ� ���� ���� ���¸� �����ϴ� Ŭ����
public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy; // ���� ���°� �����ϴ� Enemy_Range ��ü

    private float lastTimeShot = -10; // ������ �Ѿ� �߻� �ð�
    private int bulletsShot = 0; // �߻�� �Ѿ� ��

    private int bulletsPerAttack; // �� �� ���ݿ� ����� �Ѿ� ��
    private float weaponCooldown; // ���� ��ٿ� �ð�

    private float coverCheckTimer; // ������ ���� Ȯ�� Ÿ�̸�
    private bool firstTimeAttack = true; // ù ��° ���� ����

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy�� Enemy_Range�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();
        SetupValuesForFirstAttack(); // ù ��° ������ ���� �� ����

        enemy.agent.isStopped = true; // �̵� ����
        enemy.agent.velocity = Vector3.zero; // �̵� �ӵ� �ʱ�ȭ

        enemy.visuals.EnableIK(true, true); // IK Ȱ��ȭ

        stateTimer = enemy.attackDelay; // ���� ���� �ð� ����
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsSeeingPlayer()) // �÷��̾ �� �� ������
            enemy.FaceTarget(enemy.aim.position); // �÷��̾ ���ϵ��� ����

        if (enemy.CanThrowGrenade()) // ����ź ������ �����ϸ�
            stateMachine.ChangeState(enemy.throwGrenadeState); // ����ź ������ ���·� ��ȯ

        if (MustAdvancePlayer()) // �÷��̾�� �����ؾ� �� ���
            stateMachine.ChangeState(enemy.advancePlayerState); // ���� ���·� ��ȯ

        ChangeCoverIfShould(); // ������ ���� �ʿ� ���� Ȯ��

        if (stateTimer > 0)
            return;

        if (WeaponOutOfBullets()) // �Ѿ��� �����ϸ�
        {
            if (enemy.IsUnstopppable() && UnstoppableWalkReady()) // ���� �Ұ� �����̰� ������ �غ� �Ǹ�
            {
                enemy.advanceDuration = weaponCooldown;
                stateMachine.ChangeState(enemy.advancePlayerState); // ���� ���·� ��ȯ
            }

            if (WeaponOnCooldown()) // ���Ⱑ ��ٿ� ���̸�
                AttemptToResetWeapon(); // ���� �ʱ�ȭ

            return;
        }

        if (CanShoot() && enemy.IsAimOnPlayer()) // �÷��̾ ������ �� ������
        {
            Shoot(); // �߻�
        }
    }

    private bool MustAdvancePlayer()
    {
        if (enemy.IsUnstopppable()) // ���� �Ұ� ������ ��� �������� ����
            return false;

        return enemy.IsPlayerInAgrresionRange() == false && ReadyToLeaveCover(); // ���� ���� �ۿ� �ְ� ���� ���� �غ� �Ǹ� ����
    }

    private bool UnstoppableWalkReady()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);
        bool outOfStoppingDistance = distanceToPlayer > enemy.advanceStoppingDistance;
        bool unstoppableWalkOnCooldown = Time.time < enemy.weaponData.maxWeaponCooldown + enemy.advancePlayerState.lastTimeAdvanced;

        return outOfStoppingDistance && unstoppableWalkOnCooldown == false; // ���� �غ� �Ϸ�
    }

    #region Cover system region

    private bool ReadyToLeaveCover()
    {
        return Time.time > enemy.minCoverTime + enemy.runToCoverState.lastTimeTookCover; // �ּ� ���� �ð� ����
    }

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover) // ���� ���� �ɷ��� ������
            return;

        coverCheckTimer -= Time.deltaTime; // Ÿ�̸� ����

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = .5f; // 0.5�ʸ��� ���� üũ

            if (ReadyToChangeCover() && ReadyToLeaveCover()) // ������ ���� �غ� �Ǹ�
            {
                if (enemy.CanGetCover()) // ���ο� �������� �̵��� �� ������
                    stateMachine.ChangeState(enemy.runToCoverState); // ���� ���·� ��ȯ
            }
        }
    }

    private bool ReadyToChangeCover()
    {
        bool inDanger = IsPlayerInClearSight() || IsPlayerClose(); // �÷��̾ ��Ȯ�� ���̰ų� ����� ���
        bool advanceTimeIsOver = Time.time > enemy.advancePlayerState.lastTimeAdvanced + enemy.advanceDuration; // ���� �ð��� ������

        return inDanger && advanceTimeIsOver; // �����ϰ� ���� �ð��� �����ٸ� ���� ���� ����
    }

    private bool IsPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.safeDistance; // �÷��̾ �ʹ� ������� Ȯ��
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position;

        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit)) // �÷��̾ �� �� ������
        {
            if (hit.transform.root == enemy.player.root)
                return true;
        }

        return false; // �÷��̾ ������ ������
    }

    #endregion

    #region Weapon region

    private void AttemptToResetWeapon()
    {
        bulletsShot = 0; // �߻�� �Ѿ� �� �ʱ�ȭ
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack(); // ���ݴ� �߻�Ǵ� �Ѿ� ��
        weaponCooldown = enemy.weaponData.GetWeaponCooldown(); // ���� ��ٿ� �ð�
    }

    private bool WeaponOnCooldown() => Time.time > lastTimeShot + weaponCooldown; // ���� ��ٿ� Ȯ��
    private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerAttack; // �Ѿ� ���� ���� Ȯ��
    private bool CanShoot() => Time.time > lastTimeShot + 1 / enemy.weaponData.fireRate; // �߻� ���� ���� Ȯ��

    private void Shoot()
    {
        enemy.FireSingleBullet(); // �Ѿ� �߻�
        lastTimeShot = Time.time; // ������ �߻� �ð� ����
        bulletsShot++; // �߻�� �Ѿ� �� ����
    }

    private void SetupValuesForFirstAttack()
    {
        if (firstTimeAttack)
        {
            // ���� ������ �� ���� ������ ���� �ߴ� �Ÿ� ����
            enemy.aggresionRange = enemy.advanceStoppingDistance + 2;

            firstTimeAttack = false; // ù ��° ���� �÷��� ����
            bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack(); // ���ݴ� �߻�Ǵ� �Ѿ� �� ����
            weaponCooldown = enemy.weaponData.GetWeaponCooldown(); // ���� ��ٿ� ����
        }
    }

    #endregion
}
