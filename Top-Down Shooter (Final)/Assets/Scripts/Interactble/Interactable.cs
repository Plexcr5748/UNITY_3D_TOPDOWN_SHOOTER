using UnityEngine;

// Interactable: ��ȣ�ۿ� ������ ��ü�� �����ϴ� Ŭ����
public class Interactable : MonoBehaviour
{
    // �÷��̾��� ���� ��Ʈ�ѷ� ����
    protected Player_WeaponController weaponController;

    // ��ü�� �ð��� ������ ���� ����
    [SerializeField] protected MeshRenderer mesh; // �޽� ������
    [SerializeField] public Material highlightMaterial; // ���� ȿ�� ����
    [SerializeField] protected Material defaultMaterial; // �⺻ ����

    private void Start()
    {
        // MeshRenderer�� ã�� �⺻ ���� �ʱ�ȭ
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();

        defaultMaterial = mesh.sharedMaterial;
    }

    // �޽ÿ� ���� ������Ʈ
    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    // �⺻ ��ȣ�ۿ� �޼��� (�������̵� ����)
    public virtual void Interaction()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    // ���� ȿ�� Ȱ��ȭ/��Ȱ��ȭ
    public void HighlightActive(bool active)
    {
        if (active)
            mesh.material = highlightMaterial;
        else
            mesh.material = defaultMaterial;
    }

    // Ʈ���ſ� �÷��̾ �������� ��
    protected virtual void OnTriggerEnter(Collider other)
    {
        // �÷��̾��� ���� ��Ʈ�ѷ��� ĳ��
        if (weaponController == null)
            weaponController = other.GetComponent<Player_WeaponController>();

        // �÷��̾� ��ȣ�ۿ� �ý����� ������
        Player_Interaction playerInteraction = other.GetComponent<Player_Interaction>();

        // ��ȣ�ۿ� ������ ��ü ��Ͽ� �߰�
        if (playerInteraction == null)
            return;

        playerInteraction.GetInteracbles().Add(this);
        playerInteraction.UpdateClosestInteractble(); // ���� ����� ��ȣ�ۿ� ��ü ������Ʈ
    }

    // Ʈ���ſ��� �÷��̾ ������ ��
    protected virtual void OnTriggerExit(Collider other)
    {
        // �÷��̾� ��ȣ�ۿ� �ý����� ������
        Player_Interaction playerInteraction = other.GetComponent<Player_Interaction>();

        // ��ȣ�ۿ� ������ ��ü ��Ͽ��� ����
        if (playerInteraction == null)
            return;

        playerInteraction.GetInteracbles().Remove(this);
        playerInteraction.UpdateClosestInteractble(); // ���� ����� ��ȣ�ۿ� ��ü ������Ʈ
    }
}
