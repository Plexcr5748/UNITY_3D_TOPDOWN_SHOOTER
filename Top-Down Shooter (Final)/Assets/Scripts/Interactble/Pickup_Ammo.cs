using System.Collections.Generic;
using UnityEngine;

// 탄약 데이터를 정의하는 구조체
[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType; // 탄약이 호환되는 무기 타입
    [Range(10, 100)] public int minAmount; // 최소 탄약 양
    [Range(10, 100)] public int maxAmount; // 최대 탄약 양
}

// 탄약 상자의 유형
public enum AmmoBoxType { smallBox, bigBox }

// 탄약 상자를 나타내는 클래스
public class Pickup_Ammo : Interactable
{
    [SerializeField] private AmmoBoxType boxType; // 상자 유형 (작은 상자 또는 큰 상자)

    [SerializeField] private List<AmmoData> smallBoxAmmo; // 작은 상자에 포함된 탄약 데이터
    [SerializeField] private List<AmmoData> bigBoxAmmo; // 큰 상자에 포함된 탄약 데이터

    [SerializeField] private GameObject[] boxModel; // 상자의 3D 모델

    [SerializeField] private Weapon currentWeapon; // 현재 무기
    [SerializeField] private List<Weapon> weaponSlots; // 플레이어의 무기 슬롯

    // 초기 설정: 상자 모델 설정
    private void Start() => SetupBoxModel();

    // 상호작용 메서드: 탄약을 플레이어 무기에 추가
    public override void Interaction()
    {
        // 플레이어 오브젝트 가져오기
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // 현재 상자 유형에 따라 탄약 데이터 선택
        List<AmmoData> currentAmmoList = smallBoxAmmo;
        if (boxType == AmmoBoxType.bigBox)
            currentAmmoList = bigBoxAmmo;

        // 선택된 탄약 데이터를 기반으로 각 무기에 탄약 추가
        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType); // 슬롯에 있는 무기 확인
            AddBulletsToWeapon(weapon, GetBulletAmount(ammo)); // 무기에 탄약 추가
            player.GetComponent<Player_WeaponController>().UpdateWeaponUI(); // UI 업데이트
        }

        // 상자를 오브젝트 풀로 반환
        ObjectPool.instance.ReturnObject(gameObject);
    }

    // 탄약 양을 랜덤으로 계산
    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        float randomAmmoAmount = Random.Range(min, max); // 최소~최대 값 사이의 랜덤 값
        return Mathf.RoundToInt(randomAmmoAmount); // 정수로 변환
    }

    // 무기에 탄약 추가
    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null) // 무기가 없으면 반환
            return;

        weapon.totalReserveAmmo += amount; // 탄약 추가
    }

    // 상자 모델 설정: 상자 유형에 따라 적절한 모델 활성화
    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false); // 모든 모델 비활성화

            if (i == ((int)boxType)) // 현재 상자 유형과 일치하는 모델 활성화
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>()); // 메시와 재질 업데이트
            }
        }
    }
}
