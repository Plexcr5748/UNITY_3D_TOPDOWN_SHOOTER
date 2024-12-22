using UnityEngine;

// 무기 아이템을 줍는 Interactable 클래스
public class Pickup_Weapon : Interactable
{
    [SerializeField] private Weapon_Data weaponData; // 무기 데이터 (ScriptableObject)
    [SerializeField] private Weapon weapon; // 무기 인스턴스

    [SerializeField] private BackupWeaponModel[] models; // 무기 모델들

    private bool oldWeapon; // 기존 무기인지 여부를 나타내는 플래그

    private void Start()
    {
        // 기존 무기가 아닌 경우 Weapon_Data를 기반으로 새로운 무기 생성
        if (oldWeapon == false)
            weapon = new Weapon(weaponData);

        // 게임 오브젝트 초기화
        SetupGameObject();
    }

    // 무기를 설정하고 위치를 초기화
    public void SetupPickupWeapon(Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon; // 기존 무기 인스턴스를 저장
        weaponData = weapon.weaponData; // 무기 데이터 갱신
        this.transform.position = transform.position + new Vector3(0, .75f, 0); // 위치 조정
    }

    // Context Menu를 통해 아이템 모델을 업데이트할 수 있도록 설정
    [ContextMenu("Update Item Model")]
    public void SetupGameObject()
    {
        // 게임 오브젝트 이름을 무기 타입 기반으로 설정
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();

        // 무기 모델 초기화
        SetupWeaponModel();
    }

    // 무기 모델을 초기화하고 활성화
    private void SetupWeaponModel()
    {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.SetActive(false); // 모든 모델 비활성화

            if (model.weaponType == weaponData.weaponType) // 무기 타입이 일치하는 모델만 활성화
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>()); // 메시와 재질 업데이트
            }
        }
    }

    // 상호작용 구현: 무기를 플레이어가 줍도록 설정
    public override void Interaction()
    {
        // 플레이어의 무기 컨트롤러를 통해 무기를 줍도록 설정
        weaponController.PickupWeapon(weapon);

        // 아이템을 오브젝트 풀로 반환
        ObjectPool.instance.ReturnObject(gameObject);
    }
}
