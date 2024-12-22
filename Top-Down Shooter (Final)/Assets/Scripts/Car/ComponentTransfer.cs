// �θ� GameObject�� ������Ʈ�� �ڽ� GameObject�� �̵���Ű�� Ŭ����

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentTransfer : MonoBehaviour
{
    [ContextMenu("Transfer Components to Child")] // ���ؽ�Ʈ �޴����� ���� ����
    public void TransferComponentsToChild()
    {
        GameObject selectedGameObject = transform.gameObject; // ���� GameObject ����

        if (selectedGameObject == null) return; // GameObject�� null�̸� ����

        // �ڽ� GameObject ����
        GameObject childGameObject = new GameObject("Car_Wheel_Mesh"); // �� �ڽ� GameObject ����
        childGameObject.transform.SetParent(selectedGameObject.transform); // �θ�-�ڽ� ���� ����
        childGameObject.transform.localPosition = Vector3.zero; // �ڽ� ��ġ �ʱ�ȭ
        childGameObject.transform.localRotation = Quaternion.identity; // �ڽ� ȸ�� �ʱ�ȭ

        // Mesh Filter ������Ʈ�� �ڽ����� �̵�
        MeshFilter meshFilter = selectedGameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            MeshFilter childMeshFilter = childGameObject.AddComponent<MeshFilter>(); // �ڽĿ� MeshFilter �߰�
            childMeshFilter.sharedMesh = meshFilter.sharedMesh; // Mesh ������ ����
            DestroyImmediate(meshFilter); // ���� MeshFilter ����
        }

        // Mesh Renderer ������Ʈ�� �ڽ����� �̵�
        MeshRenderer meshRenderer = selectedGameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            MeshRenderer childMeshRenderer = childGameObject.AddComponent<MeshRenderer>(); // �ڽĿ� MeshRenderer �߰�
            childMeshRenderer.sharedMaterials = meshRenderer.sharedMaterials; // Material ������ ����
            DestroyImmediate(meshRenderer); // ���� MeshRenderer ����
        }
    }
}
