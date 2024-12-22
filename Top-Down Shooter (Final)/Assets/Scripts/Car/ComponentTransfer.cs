// 부모 GameObject의 컴포넌트를 자식 GameObject로 이동시키는 클래스

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentTransfer : MonoBehaviour
{
    [ContextMenu("Transfer Components to Child")] // 컨텍스트 메뉴에서 실행 가능
    public void TransferComponentsToChild()
    {
        GameObject selectedGameObject = transform.gameObject; // 현재 GameObject 참조

        if (selectedGameObject == null) return; // GameObject가 null이면 종료

        // 자식 GameObject 생성
        GameObject childGameObject = new GameObject("Car_Wheel_Mesh"); // 새 자식 GameObject 생성
        childGameObject.transform.SetParent(selectedGameObject.transform); // 부모-자식 관계 설정
        childGameObject.transform.localPosition = Vector3.zero; // 자식 위치 초기화
        childGameObject.transform.localRotation = Quaternion.identity; // 자식 회전 초기화

        // Mesh Filter 컴포넌트를 자식으로 이동
        MeshFilter meshFilter = selectedGameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            MeshFilter childMeshFilter = childGameObject.AddComponent<MeshFilter>(); // 자식에 MeshFilter 추가
            childMeshFilter.sharedMesh = meshFilter.sharedMesh; // Mesh 데이터 복사
            DestroyImmediate(meshFilter); // 기존 MeshFilter 제거
        }

        // Mesh Renderer 컴포넌트를 자식으로 이동
        MeshRenderer meshRenderer = selectedGameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            MeshRenderer childMeshRenderer = childGameObject.AddComponent<MeshRenderer>(); // 자식에 MeshRenderer 추가
            childMeshRenderer.sharedMaterials = meshRenderer.sharedMaterials; // Material 데이터 복사
            DestroyImmediate(meshRenderer); // 기존 MeshRenderer 제거
        }
    }
}
