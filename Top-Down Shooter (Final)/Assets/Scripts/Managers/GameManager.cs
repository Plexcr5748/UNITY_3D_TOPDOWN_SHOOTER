using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// GameManager: ���� ���¸� �����ϴ� Ŭ����
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // �̱��� �ν��Ͻ�
    public Player player; // �÷��̾� ��ü ����

    private Mission_Timer mission;

    [Header("Settings")]
    public bool friendlyFire; // �Ʊ� ���� ��� ����

    private void Awake()
    {
        instance = this; // �̱��� ����

        // �� ���� �÷��̾� ��ü�� ã��
        player = FindObjectOfType<Player>();

        if (mission != null)
        {
            mission.ResetMission(); // �̼� �ʱ�ȭ
        }
    }

    // ���� ���� �� ȣ��
    public void GameStart()
    {
        SetDefaultWeaponsForPlayer(); // �÷��̾��� �⺻ ���� ����

        // LevelGenerator���� ���� ���� �� �̼� ����
        // LevelGenerator.instance.InitializeGeneration();
        // �̼� ������ LevelGenerator���� ���� ���� �Ϸ� �� ó��
    }

    // ���� ���� �����
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���� �� �ٽ� �ε�

        if (mission != null)
        {
            mission.ResetMission(); // �̼� �ʱ�ȭ
        }
    }

    // ���� Ŭ���� ó��
    public void GameCompleted()
    {
        // �¸� ȭ�� UI ǥ��
        UI.instance.ShowVictoryScreenUI();

        // �÷��̾� ��Ʈ�� ��Ȱ��ȭ
        ControlsManager.instance.controls.Character.Disable();

        // �÷��̾� ü���� �ſ� ���� �����Ͽ� ������ ���� ��� ����
        player.health.currentHealth += 99999;
    }

    // ���� ���� ó��
    public void GameOver()
    {
        // ���ο� ��� ȿ�� ����
        TimeManager.instance.SlowMotionFor(1.5f);

        // ���� ���� ȭ�� UI ǥ��
        UI.instance.ShowGameOverUI();

        // ī�޶� �Ÿ� ����
        CameraManager.instance.ChangeCameraDistance(5);
    }

    // �÷��̾��� �⺻ ���� ����
    private void SetDefaultWeaponsForPlayer()
    {
        // UI���� ���õ� ���� �����͸� ������
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();

        // �÷��̾� ���� �ʱ�ȭ
        player.weapon.SetDefaultWeapon(newList);
    }
}
