using UnityEngine;

// ���� �������� �ݴ� Interactable Ŭ����
public class Pickup_Weapon : Interactable
{
    [SerializeField] private Weapon_Data weaponData; // ���� ������ (ScriptableObject)
    [SerializeField] private Weapon weapon; // ���� �ν��Ͻ�

    [SerializeField] private BackupWeaponModel[] models; // ���� �𵨵�

    private bool oldWeapon; // ���� �������� ���θ� ��Ÿ���� �÷���

    private void Start()
    {
        // ���� ���Ⱑ �ƴ� ��� Weapon_Data�� ������� ���ο� ���� ����
        if (oldWeapon == false)
            weapon = new Weapon(weaponData);

        // ���� ������Ʈ �ʱ�ȭ
        SetupGameObject();
    }

    // ���⸦ �����ϰ� ��ġ�� �ʱ�ȭ
    public void SetupPickupWeapon(Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon; // ���� ���� �ν��Ͻ��� ����
        weaponData = weapon.weaponData; // ���� ������ ����
        this.transform.position = transform.position + new Vector3(0, .75f, 0); // ��ġ ����
    }

    // Context Menu�� ���� ������ ���� ������Ʈ�� �� �ֵ��� ����
    [ContextMenu("Update Item Model")]
    public void SetupGameObject()
    {
        // ���� ������Ʈ �̸��� ���� Ÿ�� ������� ����
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();

        // ���� �� �ʱ�ȭ
        SetupWeaponModel();
    }

    // ���� ���� �ʱ�ȭ�ϰ� Ȱ��ȭ
    private void SetupWeaponModel()
    {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.SetActive(false); // ��� �� ��Ȱ��ȭ

            if (model.weaponType == weaponData.weaponType) // ���� Ÿ���� ��ġ�ϴ� �𵨸� Ȱ��ȭ
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>()); // �޽ÿ� ���� ������Ʈ
            }
        }
    }

    // ��ȣ�ۿ� ����: ���⸦ �÷��̾ �ݵ��� ����
    public override void Interaction()
    {
        // �÷��̾��� ���� ��Ʈ�ѷ��� ���� ���⸦ �ݵ��� ����
        weaponController.PickupWeapon(weapon);

        // �������� ������Ʈ Ǯ�� ��ȯ
        ObjectPool.instance.ReturnObject(gameObject);
    }
}
