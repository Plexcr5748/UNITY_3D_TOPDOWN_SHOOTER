using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SnapPointType: SnapPoint�� Ÿ���� �����ϴ� ������
public enum SnapPointType { Enter, Exit }

// SnapPoint: Ư�� ������ ��Ÿ���� Ŭ����, Enter�� Exit Ÿ������ ����
public class SnapPoint : MonoBehaviour
{
    public SnapPointType pointType; // SnapPoint�� Ÿ�� (Enter �Ǵ� Exit)

    private void Start()
    {
        // BoxCollider �� MeshRenderer ������Ʈ�� ������
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        // BoxCollider�� �����ϸ� ��Ȱ��ȭ
        if (boxCollider != null)
            boxCollider.enabled = false;

        // MeshRenderer�� �����ϸ� ��Ȱ��ȭ
        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    // Unity Inspector���� SnapPoint�� ������ �� ȣ��
    private void OnValidate()
    {
        // ������Ʈ �̸��� SnapPoint Ÿ�Կ� ���� ����
        gameObject.name = "SnapPoint - " + pointType.ToString();
    }
}
