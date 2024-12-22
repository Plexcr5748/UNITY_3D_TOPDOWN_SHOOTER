using UnityEngine;
using UnityEngine.UIElements;

// Player_Movement: 플레이어의 이동 동작과 관련된 로직을 처리하는 클래스
public class Player_Movement : MonoBehaviour
{
    private Player player; // 플레이어 객체 참조
    private CharacterController characterController; // 캐릭터 컨트롤러 컴포넌트
    private PlayerControls controls; // 플레이어 입력 컨트롤
    private Animator animator; // 애니메이터

    [Header("Movement info")]
    [SerializeField] private float walkSpeed; // 걷기 속도
    [SerializeField] private float runSpeed; // 달리기 속도
    [SerializeField] private float turnSpeed; // 회전 속도
    private float speed; // 현재 이동 속도
    private float verticalVelocity; // 수직 속도 (중력 처리)

    public Vector2 moveInput { get; private set; } // 이동 입력 값
    private Vector3 movementDirection; // 이동 방향

    private bool isRunning; // 달리기 여부

    private AudioSource walkSFX; // 걷기 소리
    private AudioSource runSFX; // 달리기 소리
    private bool canPlayFootsteps; // 발소리 재생 가능 여부

    private void Start()
    {
        // 컴포넌트 초기화
        player = GetComponent<Player>();
        walkSFX = player.sound.walkSFX;
        runSFX = player.sound.runSFX;
        Invoke(nameof(AllowfootstepsSFX), 1f); // 1초 후 발소리 재생 허용

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed; // 초기 이동 속도 설정

        AssignInputEvents(); // 입력 이벤트 등록
    }

    private void Update()
    {
        // 플레이어가 죽었으면 동작 중단
        if (player.health.isDead)
            return;

        ApplyMovement(); // 이동 처리
        ApplyRotation(); // 회전 처리
        AnimatorControllers(); // 애니메이터 컨트롤러 값 업데이트
    }

    private void AnimatorControllers()
    {
        // 애니메이터에 이동 방향 값 전달
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        // 달리기 애니메이션 설정
        bool playRunAnimation = isRunning && movementDirection.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
    }

    private void ApplyRotation()
    {
        // 조준 방향을 기준으로 플레이어 회전
        Vector3 lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f; // 수직 축 제거
        lookingDirection.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        // 이동 방향 계산 및 중력 적용
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();

        if (movementDirection.magnitude > 0)
        {
            PlayFootstepsSFX(); // 발소리 재생
            characterController.Move(movementDirection * Time.deltaTime * speed); // 이동 처리
        }
    }

    private void PlayFootstepsSFX()
    {
        // 발소리 재생 여부 확인
        if (!canPlayFootsteps)
            return;

        if (isRunning)
        {
            if (!runSFX.isPlaying)
                runSFX.Play(); // 달리기 소리 재생
        }
        else
        {
            if (!walkSFX.isPlaying)
                walkSFX.Play(); // 걷기 소리 재생
        }
    }

    private void StopFootstepsSFX()
    {
        // 발소리 중지
        walkSFX.Stop();
        runSFX.Stop();
    }

    private void AllowfootstepsSFX() => canPlayFootsteps = true; // 발소리 재생 허용

    private void ApplyGravity()
    {
        // 중력 적용
        if (!characterController.isGrounded)
        {
            verticalVelocity -= 9.81f * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f; // 바닥에 있을 경우 수직 속도 초기화
        }
    }

    private void AssignInputEvents()
    {
        // 입력 이벤트 등록
        controls = player.controls;

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context =>
        {
            StopFootstepsSFX(); // 입력 중지 시 발소리 멈춤
            moveInput = Vector2.zero; // 이동 입력 초기화
        };

        controls.Character.Run.performed += context =>
        {
            speed = runSpeed; // 이동 속도를 달리기 속도로 변경
            isRunning = true; // 달리기 상태로 전환
        };

        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed; // 이동 속도를 걷기 속도로 변경
            isRunning = false; // 달리기 상태 종료
        };
    }
}
