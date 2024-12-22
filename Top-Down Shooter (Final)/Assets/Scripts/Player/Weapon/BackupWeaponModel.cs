using UnityEngine;

// ���� �ɱ� ��� ����
public enum HangType
{
    LowBackHang, // �㸮 �Ʒ��� �ɱ�
    BackHang,    // � �ɱ�
    SideHang     // ���� �ɱ�
}

public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType weaponType; // ��� ������ Ÿ��
    [SerializeField] private HangType hangType; // ���⸦ �� ��ġ Ÿ��

    // ���⸦ Ȱ��ȭ�ϰų� ��Ȱ��ȭ
    public void Activate(bool activated) => gameObject.SetActive(activated);

    // ���� ������ �ɱ� Ÿ���� �־��� Ÿ�԰� ��ġ�ϴ��� Ȯ��
    public bool HangTypeIs(HangType hangType) => this.hangType == hangType;
}
