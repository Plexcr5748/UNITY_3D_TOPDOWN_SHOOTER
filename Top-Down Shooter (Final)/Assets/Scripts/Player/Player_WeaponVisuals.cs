using UnityEngine;
using UnityEngine.Animations.Rigging;

// Player_WeaponVisuals: 플레이어의 무기 시각적 효과 및 애니메이션을 관리하는 클래스
public class Player_WeaponVisuals : MonoBehaviour
{
    private Player player; // 플레이어 객체 참조
    private Animator anim; // 플레이어의 애니메이터

    [SerializeField] private WeaponModel[] weaponModels; // 모든 무기 모델
    [SerializeField] private BackupWeaponModel[] backupWeaponModels; // 백업 무기 모델

    [Header("Rig ")]
    [SerializeField] private float rigWeightIncreaseRate; // 리그 무게 증가 속도
    private bool shouldIncrease_RigWeight; // 리그 무게를 증가시킬지 여부
    private Rig rig; // 애니메이션 리그

    [Header("Left hand IK")]
    [SerializeField] private float leftHandIkWeightIncreaseRate; // 왼손 IK 무게 증가 속도
    [SerializeField] private TwoBoneIKConstraint leftHandIK; // 왼손 IK 제약
    [SerializeField] private Transform leftHandIK_Target; // 왼손 IK 타겟
    private bool shouldIncrease_LeftHandIKWieght; // 왼손 IK 무게를 증가시킬지 여부

    private void Start()
    {
        // 컴포넌트 초기화
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();

        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWigth(); // 리그 무게 업데이트
        UpdateLeftHandIKWeight(); // 왼손 IK 무게 업데이트
    }

    public void PlayFireAnimation() => anim.SetTrigger("Fire"); // 발사 애니메이션 재생

    public void PlayReloadAnimation()
    {
        // 재장전 애니메이션 재생
        float reloadSpeed = player.weapon.CurrentWeapon().reloadSpeed;
        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }

    public void PlayWeaponEquipAnimation()
    {
        // 무기 장착 애니메이션 재생
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
        // 현재 무기 모델 활성화
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
        // 모든 무기 모델 비활성화
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void SwitchOffBackupWeaponModels()
    {
        // 모든 백업 무기 모델 비활성화
        foreach (BackupWeaponModel backupModel in backupWeaponModels)
        {
            backupModel.Activate(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        // 백업 무기 모델 활성화
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
        // 애니메이션 레이어 활성화
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public WeaponModel CurrentWeaponModel()
    {
        // 현재 무기 모델 반환
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
        // 왼손 IK 타겟 위치와 회전을 현재 무기 모델에 맞춤
        Transform targetTransform = CurrentWeaponModel().holdPoint;
        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    private void UpdateLeftHandIKWeight()
    {
        // 왼손 IK 무게 증가 처리
        if (shouldIncrease_LeftHandIKWieght)
        {
            leftHandIK.weight += leftHandIkWeightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
                shouldIncrease_LeftHandIKWieght = false;
        }
    }

    private void UpdateRigWigth()
    {
        // 리그 무게 증가 처리
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1)
                shouldIncrease_RigWeight = false;
        }
    }

    private void ReduceRigWeight()
    {
        // 리그 무게 감소 처리
        rig.weight = .15f;
    }

    public void MaximizeRigWeight() => shouldIncrease_RigWeight = true; // 리그 무게 최대화
    public void MaximizeLeftHandWeight() => shouldIncrease_LeftHandIKWieght = true; // 왼손 IK 무게 최대화

    #endregion
}
