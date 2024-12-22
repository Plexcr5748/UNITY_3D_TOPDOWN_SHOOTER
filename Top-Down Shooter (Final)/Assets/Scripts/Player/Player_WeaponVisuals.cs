using UnityEngine;
using UnityEngine.Animations.Rigging;

// Player_WeaponVisuals: �÷��̾��� ���� �ð��� ȿ�� �� �ִϸ��̼��� �����ϴ� Ŭ����
public class Player_WeaponVisuals : MonoBehaviour
{
    private Player player; // �÷��̾� ��ü ����
    private Animator anim; // �÷��̾��� �ִϸ�����

    [SerializeField] private WeaponModel[] weaponModels; // ��� ���� ��
    [SerializeField] private BackupWeaponModel[] backupWeaponModels; // ��� ���� ��

    [Header("Rig ")]
    [SerializeField] private float rigWeightIncreaseRate; // ���� ���� ���� �ӵ�
    private bool shouldIncrease_RigWeight; // ���� ���Ը� ������ų�� ����
    private Rig rig; // �ִϸ��̼� ����

    [Header("Left hand IK")]
    [SerializeField] private float leftHandIkWeightIncreaseRate; // �޼� IK ���� ���� �ӵ�
    [SerializeField] private TwoBoneIKConstraint leftHandIK; // �޼� IK ����
    [SerializeField] private Transform leftHandIK_Target; // �޼� IK Ÿ��
    private bool shouldIncrease_LeftHandIKWieght; // �޼� IK ���Ը� ������ų�� ����

    private void Start()
    {
        // ������Ʈ �ʱ�ȭ
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();

        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWigth(); // ���� ���� ������Ʈ
        UpdateLeftHandIKWeight(); // �޼� IK ���� ������Ʈ
    }

    public void PlayFireAnimation() => anim.SetTrigger("Fire"); // �߻� �ִϸ��̼� ���

    public void PlayReloadAnimation()
    {
        // ������ �ִϸ��̼� ���
        float reloadSpeed = player.weapon.CurrentWeapon().reloadSpeed;
        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }

    public void PlayWeaponEquipAnimation()
    {
        // ���� ���� �ִϸ��̼� ���
        EquipType equipType = CurrentWeaponModel().equipAnimationType;
        float equipmentSpeed = player.weapon.CurrentWeapon().equipmentSpeed;

        leftHandIK.weight = 0;
        ReduceRigWeight();
        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipType", ((float)equipType));
        anim.SetFloat("EquipSpeed", equipmentSpeed);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        // ���� ���� �� Ȱ��ȭ
        int animationIndex = ((int)CurrentWeaponModel().holdType);

        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();

        if (player.weapon.HasOnlyOneWeapon() == false)
            SwitchOnBackupWeaponModel();

        SwitchAnimationLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        // ��� ���� �� ��Ȱ��ȭ
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void SwitchOffBackupWeaponModels()
    {
        // ��� ��� ���� �� ��Ȱ��ȭ
        foreach (BackupWeaponModel backupModel in backupWeaponModels)
        {
            backupModel.Activate(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        // ��� ���� �� Ȱ��ȭ
        SwitchOffBackupWeaponModels();

        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;

        foreach (BackupWeaponModel backupModel in backupWeaponModels)
        {
            if (backupModel.weaponType == player.weapon.CurrentWeapon().weaponType)
                continue;

            if (player.weapon.WeaponInSlots(backupModel.weaponType) != null)
            {
                if (backupModel.HangTypeIs(HangType.LowBackHang))
                    lowHangWeapon = backupModel;

                if (backupModel.HangTypeIs(HangType.BackHang))
                    backHangWeapon = backupModel;

                if (backupModel.HangTypeIs(HangType.SideHang))
                    sideHangWeapon = backupModel;
            }
        }

        lowHangWeapon?.Activate(true);
        backHangWeapon?.Activate(true);
        sideHangWeapon?.Activate(true);
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        // �ִϸ��̼� ���̾� Ȱ��ȭ
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public WeaponModel CurrentWeaponModel()
    {
        // ���� ���� �� ��ȯ
        WeaponModel weaponModel = null;
        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
                weaponModel = weaponModels[i];
        }

        return weaponModel;
    }

    #region Animation Rigging Methods

    private void AttachLeftHand()
    {
        // �޼� IK Ÿ�� ��ġ�� ȸ���� ���� ���� �𵨿� ����
        Transform targetTransform = CurrentWeaponModel().holdPoint;
        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    private void UpdateLeftHandIKWeight()
    {
        // �޼� IK ���� ���� ó��
        if (shouldIncrease_LeftHandIKWieght)
        {
            leftHandIK.weight += leftHandIkWeightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
                shouldIncrease_LeftHandIKWieght = false;
        }
    }

    private void UpdateRigWigth()
    {
        // ���� ���� ���� ó��
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1)
                shouldIncrease_RigWeight = false;
        }
    }

    private void ReduceRigWeight()
    {
        // ���� ���� ���� ó��
        rig.weight = .15f;
    }

    public void MaximizeRigWeight() => shouldIncrease_RigWeight = true; // ���� ���� �ִ�ȭ
    public void MaximizeLeftHandWeight() => shouldIncrease_LeftHandIKWieght = true; // �޼� IK ���� �ִ�ȭ

    #endregion
}
