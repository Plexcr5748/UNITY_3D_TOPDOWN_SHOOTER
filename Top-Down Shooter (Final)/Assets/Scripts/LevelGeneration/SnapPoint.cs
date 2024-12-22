using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SnapPointType: SnapPoint의 타입을 정의하는 열거형
public enum SnapPointType { Enter, Exit }

// SnapPoint: 특정 지점을 나타내는 클래스, Enter와 Exit 타입으로 구분
public class SnapPoint : MonoBehaviour
{
    public SnapPointType pointType; // SnapPoint의 타입 (Enter 또는 Exit)

    private void Start()
    {
        // BoxCollider 및 MeshRenderer 컴포넌트를 가져옴
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        // BoxCollider가 존재하면 비활성화
        if (boxCollider != null)
            boxCollider.enabled = false;

        // MeshRenderer가 존재하면 비활성화
        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    // Unity Inspector에서 SnapPoint가 수정될 때 호출
    private void OnValidate()
    {
        // 오브젝트 이름을 SnapPoint 타입에 따라 설정
        gameObject.name = "SnapPoint - " + pointType.ToString();
    }
}
