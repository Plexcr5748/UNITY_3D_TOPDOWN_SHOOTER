using UnityEngine;

// Player: �÷��̾�� ���õ� ��� �ֿ� ������Ʈ�� �ʱ�ȭ�ϰ� �����ϴ� Ŭ����
public class Player : MonoBehaviour
{
    public Transform playerBody; // �÷��̾��� ��ü Ʈ������

    // �÷��̾�� ���õ� �ֿ� ������Ʈ�� ��Ʈ�� ����
    public PlayerControls controls { get; private set; } // �÷��̾� �Է� ��Ʈ��
    public Player_AimController aim { get; private set; } // ���� ��Ʈ�ѷ�
    public Player_Movement movement { get; private set; } // �̵� ��Ʈ�ѷ�
    public Player_WeaponController weapon { get; private set; } // ���� ��Ʈ�ѷ�
    public Player_WeaponVisuals weaponVisuals { get; private set; } // ���� �ð� ȿ��
    public Player_Interaction interaction { get; private set; } // ��ȣ�ۿ� ��Ʈ�ѷ�
    public Player_Health health { get; private set; } // �÷��̾� ü�� ����
    public Ragdoll ragdoll { get; private set; } // ���׵� ó��

    public Animator anim { get; private set; } // �ִϸ�����
    public Player_SoundFX sound { get; private set; } // ���� ȿ�� ����

    public bool controlsEnabled { get; private set; } // �Է� ��Ʈ�� Ȱ��ȭ ����

    private void Awake()
    {
        // �÷��̾� ���� ��� ������Ʈ �ʱ�ȭ
        anim = GetComponentInChildren<Animator>(); // �ִϸ�����
        ragdoll = GetComponent<Ragdoll>(); // ���׵� ������Ʈ
        health = GetComponent<Player_Health>(); // ü�� ����
        aim = GetComponent<Player_AimController>(); // ���� ����
        movement = GetComponent<Player_Movement>(); // �̵� ����
        weapon = GetComponent<Player_WeaponController>(); // ���� ����
        weaponVisuals = GetComponent<Player_WeaponVisuals>(); // ���� �ð� ȿ��
        interaction = GetComponent<Player_Interaction>(); // ��ȣ�ۿ� ����
        sound = GetComponent<Player_SoundFX>(); // ���� ȿ��
        controls = ControlsManager.instance.controls; // �Է� ��Ʈ�� �ʱ�ȭ
    }

    private void OnEnable()
    {
        // �Է� ��Ʈ�� Ȱ��ȭ �� �̺�Ʈ ���
        controls.Enable();
        controls.Character.UIMissionToolTipSwitch.performed += ctx => UI.instance.inGameUI.SwitchMissionTooltip(); // �̼� ���� ��ȯ
        controls.Character.UIPause.performed += ctx => UI.instance.PauseSwitch(); // ���� �Ͻ����� ��ȯ
    }

    private void OnDisable()
    {
        // �Է� ��Ʈ�� ��Ȱ��ȭ
        controls.Disable();
    }

    public void SetControlsEnabledTo(bool enabled)
    {
        // �÷��̾� �Է� ��Ʈ�� Ȱ��ȭ/��Ȱ��ȭ
        controlsEnabled = enabled;

        // ���׵� �� ���� ������ Ȱ��ȭ ���� ����
        ragdoll.CollidersActive(enabled);
        aim.EnableAimLaer(enabled);
    }
}
