using UnityEngine;
using UnityEngine.UIElements;

// Player_Movement: �÷��̾��� �̵� ���۰� ���õ� ������ ó���ϴ� Ŭ����
public class Player_Movement : MonoBehaviour
{
    private Player player; // �÷��̾� ��ü ����
    private CharacterController characterController; // ĳ���� ��Ʈ�ѷ� ������Ʈ
    private PlayerControls controls; // �÷��̾� �Է� ��Ʈ��
    private Animator animator; // �ִϸ�����

    [Header("Movement info")]
    [SerializeField] private float walkSpeed; // �ȱ� �ӵ�
    [SerializeField] private float runSpeed; // �޸��� �ӵ�
    [SerializeField] private float turnSpeed; // ȸ�� �ӵ�
    private float speed; // ���� �̵� �ӵ�
    private float verticalVelocity; // ���� �ӵ� (�߷� ó��)

    public Vector2 moveInput { get; private set; } // �̵� �Է� ��
    private Vector3 movementDirection; // �̵� ����

    private bool isRunning; // �޸��� ����

    private AudioSource walkSFX; // �ȱ� �Ҹ�
    private AudioSource runSFX; // �޸��� �Ҹ�
    private bool canPlayFootsteps; // �߼Ҹ� ��� ���� ����

    private void Start()
    {
        // ������Ʈ �ʱ�ȭ
        player = GetComponent<Player>();
        walkSFX = player.sound.walkSFX;
        runSFX = player.sound.runSFX;
        Invoke(nameof(AllowfootstepsSFX), 1f); // 1�� �� �߼Ҹ� ��� ���

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed; // �ʱ� �̵� �ӵ� ����

        AssignInputEvents(); // �Է� �̺�Ʈ ���
    }

    private void Update()
    {
        // �÷��̾ �׾����� ���� �ߴ�
        if (player.health.isDead)
            return;

        ApplyMovement(); // �̵� ó��
        ApplyRotation(); // ȸ�� ó��
        AnimatorControllers(); // �ִϸ����� ��Ʈ�ѷ� �� ������Ʈ
    }

    private void AnimatorControllers()
    {
        // �ִϸ����Ϳ� �̵� ���� �� ����
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        // �޸��� �ִϸ��̼� ����
        bool playRunAnimation = isRunning && movementDirection.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
    }

    private void ApplyRotation()
    {
        // ���� ������ �������� �÷��̾� ȸ��
        Vector3 lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f; // ���� �� ����
        lookingDirection.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        // �̵� ���� ��� �� �߷� ����
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();

        if (movementDirection.magnitude > 0)
        {
            PlayFootstepsSFX(); // �߼Ҹ� ���
            characterController.Move(movementDirection * Time.deltaTime * speed); // �̵� ó��
        }
    }

    private void PlayFootstepsSFX()
    {
        // �߼Ҹ� ��� ���� Ȯ��
        if (!canPlayFootsteps)
            return;

        if (isRunning)
        {
            if (!runSFX.isPlaying)
                runSFX.Play(); // �޸��� �Ҹ� ���
        }
        else
        {
            if (!walkSFX.isPlaying)
                walkSFX.Play(); // �ȱ� �Ҹ� ���
        }
    }

    private void StopFootstepsSFX()
    {
        // �߼Ҹ� ����
        walkSFX.Stop();
        runSFX.Stop();
    }

    private void AllowfootstepsSFX() => canPlayFootsteps = true; // �߼Ҹ� ��� ���

    private void ApplyGravity()
    {
        // �߷� ����
        if (!characterController.isGrounded)
        {
            verticalVelocity -= 9.81f * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f; // �ٴڿ� ���� ��� ���� �ӵ� �ʱ�ȭ
        }
    }

    private void AssignInputEvents()
    {
        // �Է� �̺�Ʈ ���
        controls = player.controls;

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context =>
        {
            StopFootstepsSFX(); // �Է� ���� �� �߼Ҹ� ����
            moveInput = Vector2.zero; // �̵� �Է� �ʱ�ȭ
        };

        controls.Character.Run.performed += context =>
        {
            speed = runSpeed; // �̵� �ӵ��� �޸��� �ӵ��� ����
            isRunning = true; // �޸��� ���·� ��ȯ
        };

        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed; // �̵� �ӵ��� �ȱ� �ӵ��� ����
            isRunning = false; // �޸��� ���� ����
        };
    }
}
