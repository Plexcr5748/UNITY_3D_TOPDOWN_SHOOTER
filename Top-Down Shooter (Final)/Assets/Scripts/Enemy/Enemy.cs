using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// �� ���� ����
public enum EnemyType { Melee, Range, Boss, Random }

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType; // �� ����
    public LayerMask whatIsAlly; // �Ʊ� ���̾�
    public LayerMask whatIsPlayer; // �÷��̾� ���̾�

    [Header("Idle data")]
    public float idleTime; // ��� �ð�
    public float aggresionRange; // ���� ���� ����

    [Header("Move data")]
    public float walkSpeed = 1.5f; // �ȴ� �ӵ�
    public float runSpeed = 3; // �޸��� �ӵ�
    public float turnSpeed; // ȸ�� �ӵ�
    private bool manualMovement; // ���� �̵� ����
    private bool manualRotation; // ���� ȸ�� ����

    [SerializeField] private Transform[] patrolPoints; // ���� ����
    private Vector3[] patrolPointsPosition; // ���� ������ ��ġ
    private int currentPatrolIndex; // ���� ���� ���� �ε���

    public bool inBattleMode { get; private set; } // ���� ��� ����
    protected bool isMeleeAttackReady; // ���� ���� �غ� ����

    public Transform player { get; private set; } // �÷��̾� Transform
    public Animator anim { get; private set; } // Animator
    public NavMeshAgent agent { get; private set; } // NavMeshAgent
    public EnemyStateMachine stateMachine { get; private set; } // ���� �ӽ�
    public Enemy_Visuals visuals { get; private set; } // �� �ð��� ȿ��
    public Enemy_Health health { get; private set; } // �� ü��
    public Ragdoll ragdoll { get; private set; } // ���׵� ȿ��
    public Enemy_DropController dropController { get; private set; } // ������ ��� ��Ʈ�ѷ�
    public AudioManager audioManager { get; private set; } // ����� ������

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        // �ʿ��� ������Ʈ �ʱ�ȭ
        health = GetComponent<Enemy_Health>();
        ragdoll = GetComponent<Ragdoll>();
        visuals = GetComponent<Enemy_Visuals>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dropController = GetComponent<Enemy_DropController>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints(); // ���� ���� �ʱ�ȭ
        audioManager = AudioManager.instance; // ����� ������ ����
    }

    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    protected virtual void InitializePerk() { } // �� Ư�� �ʱ�ȭ (Ȯ�� ����)

    public virtual void MakeEnemyVIP()
    {
        // VIP ���·� ����
        int additionalHealth = Mathf.RoundToInt(health.currentHealth * 1.5f);
        health.currentHealth += additionalHealth;
        transform.localScale *= 1.15f; // ũ�� ����
    }

    protected bool ShouldEnterBattleMode()
    {
        if (IsPlayerInAgrresionRange() && !inBattleMode) //�÷��̾ ���� �Ÿ� �ȿ� ���Դٸ�
        {
            EnterBattleMode();
            return true;
        }

        return false;
    }

    public virtual void EnterBattleMode() //���� ��� Ȱ��ȭ
    {
        inBattleMode = true;
    }

    public virtual void GetHit(int damage)
    {
        EnterBattleMode(); // ���� ���� ��ȯ
        health.ReduceHealth(damage); // ü�� ����

        if (health.ShouldDie())
            Die();
    }

    public virtual void Die()
    {
        dropController.DropItems(); // ������ ���

        anim.enabled = false; // �ִϸ��̼� ��Ȱ��ȭ
        agent.isStopped = true;
        agent.enabled = false;

        ragdoll.RagdollActive(true); // ���׵� Ȱ��ȭ

        // ��� �̼� ����̾����� Ȯ��
        MissionObject_HuntTarget huntTarget = GetComponent<MissionObject_HuntTarget>();
        huntTarget?.InvokeOnTargetKilled();
    }

    public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius, GameObject fx, int damage)
    {
        if (!isMeleeAttackReady)
            return;

        foreach (Transform attackPoint in damagePoints)
        {
            Collider[] detectedHits = Physics.OverlapSphere(attackPoint.position, attackCheckRadius, whatIsPlayer);

            for (int i = 0; i < detectedHits.Length; i++)
            {
                IDamagable damagable = detectedHits[i].GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.TakeDamage(damage); // ������ ����
                    isMeleeAttackReady = false; // ���� ��Ȱ��ȭ
                    GameObject newAttackFx = ObjectPool.instance.GetObject(fx, attackPoint);
                    ObjectPool.instance.ReturnObject(newAttackFx, 1);
                    return;
                }
            }
        }
    }

    public void EnableMeleeAttackCheck(bool enable) => isMeleeAttackReady = enable;

    public virtual void BulletImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        if (health.ShouldDie())
            StartCoroutine(DeathImpactCourutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCourutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void FaceTarget(Vector3 target, float turnSpeed = 0)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        if (turnSpeed == 0)
            turnSpeed = this.turnSpeed;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    #region Animation events
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }
    #endregion

    #region Patrol logic 

    //���� ����
    public Vector3 GetPatrolDestination() 
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length; // ���� ����
        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false); // ���� ���� ��Ȱ��ȭ
        }
    }
    #endregion

    public bool IsPlayerInAgrresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange); // ���� ���� ǥ��
    }
}
