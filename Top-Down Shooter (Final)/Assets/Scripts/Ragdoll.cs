using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ragdoll: ���׵� ���� ȿ���� �����ϴ� Ŭ����
public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;
    // ���׵��� �θ� Ʈ������ (�ʿ� �� Ư�� �θ�� ����)

    private Collider[] ragdollColliders; // ���׵��� ���Ե� ��� �ݶ��̴�
    private Rigidbody[] ragdollRigidbodies; // ���׵��� ���Ե� ��� ������ٵ�

    private void Awake()
    {
        // �ڽ� ��ü���� ��� �ݶ��̴��� ������ٵ� ������
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        // ���׵� ��Ȱ��ȭ ���·� �ʱ�ȭ
        RagdollActive(false);

        // ��� ������ٵ� Interpolation ���� (������ �������� �ε巴�� ����)
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    public void RagdollActive(bool active)
    {
        // ���׵��� ������ٵ� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active; // Ȱ��ȭ �� isKinematic�� ��Ȱ��ȭ
        }
    }

    public void CollidersActive(bool active)
    {
        // ���׵��� �ݶ��̴��� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        foreach (Collider cd in ragdollColliders)
        {
            cd.enabled = active;
        }
    }
}
