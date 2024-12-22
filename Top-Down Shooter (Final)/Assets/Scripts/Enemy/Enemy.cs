using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// 적 유형 정의
public enum EnemyType { Melee, Range, Boss, Random }

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType; // 적 유형
    public LayerMask whatIsAlly; // 아군 레이어
    public LayerMask whatIsPlayer; // 플레이어 레이어

    [Header("Idle data")]
    public float idleTime; // 대기 시간
    public float aggresionRange; // 적의 공격 범위

    [Header("Move data")]
    public float walkSpeed = 1.5f; // 걷는 속도
    public float runSpeed = 3; // 달리는 속도
    public float turnSpeed; // 회전 속도
    private bool manualMovement; // 수동 이동 여부
    private bool manualRotation; // 수동 회전 여부

    [SerializeField] private Transform[] patrolPoints; // 순찰 지점
    private Vector3[] patrolPointsPosition; // 순찰 지점의 위치
    private int currentPatrolIndex; // 현재 순찰 지점 인덱스

    public bool inBattleMode { get; private set; } // 전투 모드 여부
    protected bool isMeleeAttackReady; // 근접 공격 준비 여부

    public Transform player { get; private set; } // 플레이어 Transform
    public Animator anim { get; private set; } // Animator
    public NavMeshAgent agent { get; private set; } // NavMeshAgent
    public EnemyStateMachine stateMachine { get; private set; } // 상태 머신
    public Enemy_Visuals visuals { get; private set; } // 적 시각적 효과
    public Enemy_Health health { get; private set; } // 적 체력
    public Ragdoll ragdoll { get; private set; } // 래그돌 효과
    public Enemy_DropController dropController { get; private set; } // 아이템 드롭 컨트롤러
    public AudioManager audioManager { get; private set; } // 오디오 관리자

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        // 필요한 컴포넌트 초기화
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
        InitializePatrolPoints(); // 순찰 지점 초기화
        audioManager = AudioManager.instance; // 오디오 관리자 설정
    }

    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    protected virtual void InitializePerk() { } // 적 특성 초기화 (확장 가능)

    public virtual void MakeEnemyVIP()
    {
        // VIP 상태로 변경
        int additionalHealth = Mathf.RoundToInt(health.currentHealth * 1.5f);
        health.currentHealth += additionalHealth;
        transform.localScale *= 1.15f; // 크기 증가
    }

    protected bool ShouldEnterBattleMode()
    {
        if (IsPlayerInAgrresionRange() && !inBattleMode) //플레이어가 감지 거리 안에 들어왔다면
        {
            EnterBattleMode();
            return true;
        }

        return false;
    }

    public virtual void EnterBattleMode() //전투 모드 활성화
    {
        inBattleMode = true;
    }

    public virtual void GetHit(int damage)
    {
        EnterBattleMode(); // 전투 모드로 전환
        health.ReduceHealth(damage); // 체력 감소

        if (health.ShouldDie())
            Die();
    }

    public virtual void Die()
    {
        dropController.DropItems(); // 아이템 드롭

        anim.enabled = false; // 애니메이션 비활성화
        agent.isStopped = true;
        agent.enabled = false;

        ragdoll.RagdollActive(true); // 래그돌 활성화

        // 사냥 미션 대상이었는지 확인
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
                    damagable.TakeDamage(damage); // 데미지 적용
                    isMeleeAttackReady = false; // 공격 비활성화
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

    //순찰 로직
    public Vector3 GetPatrolDestination() 
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length; // 순찰 루프
        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false); // 순찰 지점 비활성화
        }
    }
    #endregion

    public bool IsPlayerInAgrresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange); // 공격 범위 표시
    }
}
