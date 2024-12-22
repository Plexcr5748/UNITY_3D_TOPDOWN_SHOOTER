using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ControlsManager: ���� �� ��Ʈ�� ��带 �����ϴ� Ŭ����
public class ControlsManager : MonoBehaviour
{
    public static ControlsManager instance; // �̱��� �ν��Ͻ�
    public PlayerControls controls { get; private set; } // �Է� �ý��� Ŭ����
    private Player player; // �÷��̾� ��ü ����

    private void Awake()
    {
        instance = this; // �̱��� ����
        controls = new PlayerControls(); // PlayerControls �ʱ�ȭ
    }

    private void Start()
    {
        player = GameManager.instance.player; // GameManager���� �÷��̾� ��ü ��������

        // �ʱ� ��Ʈ���� ĳ���� ��Ʈ�ѷ� ����
        SwitchToCharacterControls();
    }

    // ĳ���� ��Ʈ�� Ȱ��ȭ
    public void SwitchToCharacterControls()
    {
        controls.Character.Enable(); // ĳ���� ��Ʈ�� Ȱ��ȭ

        controls.Car.Disable(); // ���� ��Ʈ�� ��Ȱ��ȭ
        controls.UI.Disable(); // UI ��Ʈ�� ��Ȱ��ȭ

        player.SetControlsEnabledTo(true); // �÷��̾� �Է� Ȱ��ȭ
        UI.instance.inGameUI.SwitchToCharcaterUI(); // UI�� ĳ���� ���� ��ȯ
    }

    // UI ��Ʈ�� Ȱ��ȭ
    public void SwitchToUIControls()
    {
        controls.UI.Enable(); // UI ��Ʈ�� Ȱ��ȭ

        controls.Car.Disable(); // ���� ��Ʈ�� ��Ȱ��ȭ
        controls.Character.Disable(); // ĳ���� ��Ʈ�� ��Ȱ��ȭ

        player.SetControlsEnabledTo(false); // �÷��̾� �Է� ��Ȱ��ȭ
    }

    // ���� ��Ʈ�� Ȱ��ȭ
    public void SwitchToCarControls()
    {
        controls.Car.Enable(); // ���� ��Ʈ�� Ȱ��ȭ

        controls.UI.Disable(); // UI ��Ʈ�� ��Ȱ��ȭ
        controls.Character.Disable(); // ĳ���� ��Ʈ�� ��Ȱ��ȭ

        player.SetControlsEnabledTo(false); // �÷��̾� �Է� ��Ȱ��ȭ
        UI.instance.inGameUI.SwitchToCarUI(); // UI�� ���� ���� ��ȯ
    }
}
