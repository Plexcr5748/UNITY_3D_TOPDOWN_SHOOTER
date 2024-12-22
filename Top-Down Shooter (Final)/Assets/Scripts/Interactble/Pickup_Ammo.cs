using System.Collections.Generic;
using UnityEngine;

// ź�� �����͸� �����ϴ� ����ü
[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType; // ź���� ȣȯ�Ǵ� ���� Ÿ��
    [Range(10, 100)] public int minAmount; // �ּ� ź�� ��
    [Range(10, 100)] public int maxAmount; // �ִ� ź�� ��
}

// ź�� ������ ����
public enum AmmoBoxType { smallBox, bigBox }

// ź�� ���ڸ� ��Ÿ���� Ŭ����
public class Pickup_Ammo : Interactable
{
    [SerializeField] private AmmoBoxType boxType; // ���� ���� (���� ���� �Ǵ� ū ����)

    [SerializeField] private List<AmmoData> smallBoxAmmo; // ���� ���ڿ� ���Ե� ź�� ������
    [SerializeField] private List<AmmoData> bigBoxAmmo; // ū ���ڿ� ���Ե� ź�� ������

    [SerializeField] private GameObject[] boxModel; // ������ 3D ��

    [SerializeField] private Weapon currentWeapon; // ���� ����
    [SerializeField] private List<Weapon> weaponSlots; // �÷��̾��� ���� ����

    // �ʱ� ����: ���� �� ����
    private void Start() => SetupBoxModel();

    // ��ȣ�ۿ� �޼���: ź���� �÷��̾� ���⿡ �߰�
    public override void Interaction()
    {
        // �÷��̾� ������Ʈ ��������
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // ���� ���� ������ ���� ź�� ������ ����
        List<AmmoData> currentAmmoList = smallBoxAmmo;
        if (boxType == AmmoBoxType.bigBox)
            currentAmmoList = bigBoxAmmo;

        // ���õ� ź�� �����͸� ������� �� ���⿡ ź�� �߰�
        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType); // ���Կ� �ִ� ���� Ȯ��
            AddBulletsToWeapon(weapon, GetBulletAmount(ammo)); // ���⿡ ź�� �߰�
            player.GetComponent<Player_WeaponController>().UpdateWeaponUI(); // UI ������Ʈ
        }

        // ���ڸ� ������Ʈ Ǯ�� ��ȯ
        ObjectPool.instance.ReturnObject(gameObject);
    }

    // ź�� ���� �������� ���
    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        float randomAmmoAmount = Random.Range(min, max); // �ּ�~�ִ� �� ������ ���� ��
        return Mathf.RoundToInt(randomAmmoAmount); // ������ ��ȯ
    }

    // ���⿡ ź�� �߰�
    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null) // ���Ⱑ ������ ��ȯ
            return;

        weapon.totalReserveAmmo += amount; // ź�� �߰�
    }

    // ���� �� ����: ���� ������ ���� ������ �� Ȱ��ȭ
    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false); // ��� �� ��Ȱ��ȭ

            if (i == ((int)boxType)) // ���� ���� ������ ��ġ�ϴ� �� Ȱ��ȭ
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>()); // �޽ÿ� ���� ������Ʈ
            }
        }
    }
}
