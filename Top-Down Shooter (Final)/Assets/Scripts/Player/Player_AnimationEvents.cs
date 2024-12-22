using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_AnimationEvents: �÷��̾��� �ִϸ��̼� �̺�Ʈ�� ó���ϴ� Ŭ����
public class Player_AnimationEvents : MonoBehaviour
{
    private Player_WeaponVisuals visualController; // ���� �ð� ȿ�� ����
    private Player_WeaponController weaponController; // ���� ��Ʈ�ѷ�

    private void Start()
    {
        // �θ� ��ü���� ���� ��Ʈ�ѷ� ������Ʈ ��������
        visualController = GetComponentInParent<Player_WeaponVisuals>();
        weaponController = GetComponentInParent<Player_WeaponController>();
    }

    public void ReloadIsOver()
    {
        // ������ �ִϸ��̼��� ������ �� ȣ��
        visualController.MaximizeRigWeight(); // ĳ���� ����(Rig)�� ���� �ʱ�ȭ
        visualController.CurrentWeaponModel().realodSfx.Stop(); // ������ ���� ȿ�� ����

        weaponController.CurrentWeapon().RefillBullets(); // ���� ������ ź���� �ִ�ġ�� ����
        weaponController.SetWeaponReady(true); // ���� �غ� ���·� ����
        weaponController.UpdateWeaponUI(); // UI ������Ʈ
    }

    public void ReturnRig()
    {
        // ����(Rig) ���¸� �⺻������ ����
        visualController.MaximizeRigWeight(); // ĳ���� ���� ���� ����
        visualController.MaximizeLeftHandWeight(); // �޼� ���� ���� ����
    }

    public void WeaponEquipingIsOver()
    {
        // ���� ���� �ִϸ��̼��� ������ �� ȣ��
        weaponController.SetWeaponReady(true); // ���� �غ� ���·� ����
    }

    public void SwitchOnWeaponModel()
    {
        // ���� ���� Ȱ��ȭ
        visualController.SwitchOnCurrentWeaponModel();
    }
}
