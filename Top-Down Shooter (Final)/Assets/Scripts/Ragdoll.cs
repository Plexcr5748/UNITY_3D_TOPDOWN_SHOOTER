using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ragdoll: 레그돌 물리 효과를 관리하는 클래스
public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;
    // 레그돌의 부모 트랜스폼 (필요 시 특정 부모로 설정)

    private Collider[] ragdollColliders; // 레그돌에 포함된 모든 콜라이더
    private Rigidbody[] ragdollRigidbodies; // 레그돌에 포함된 모든 리지드바디

    private void Awake()
    {
        // 자식 객체에서 모든 콜라이더와 리지드바디를 가져옴
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        // 레그돌 비활성화 상태로 초기화
        RagdollActive(false);

        // 모든 리지드바디에 Interpolation 설정 (물리적 움직임을 부드럽게 만듦)
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    public void RagdollActive(bool active)
    {
        // 레그돌의 리지드바디를 활성화 또는 비활성화
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active; // 활성화 시 isKinematic을 비활성화
        }
    }

    public void CollidersActive(bool active)
    {
        // 레그돌의 콜라이더를 활성화 또는 비활성화
        foreach (Collider cd in ragdollColliders)
        {
            cd.enabled = active;
        }
    }
}
