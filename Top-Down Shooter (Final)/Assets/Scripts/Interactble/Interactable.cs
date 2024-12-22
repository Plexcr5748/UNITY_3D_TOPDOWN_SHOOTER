using UnityEngine;

// Interactable: 상호작용 가능한 객체를 정의하는 클래스
public class Interactable : MonoBehaviour
{
    // 플레이어의 무기 컨트롤러 참조
    protected Player_WeaponController weaponController;

    // 객체의 시각적 강조를 위한 재질
    [SerializeField] protected MeshRenderer mesh; // 메시 렌더러
    [SerializeField] public Material highlightMaterial; // 강조 효과 재질
    [SerializeField] protected Material defaultMaterial; // 기본 재질

    private void Start()
    {
        // MeshRenderer를 찾고 기본 재질 초기화
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();

        defaultMaterial = mesh.sharedMaterial;
    }

    // 메시와 재질 업데이트
    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    // 기본 상호작용 메서드 (오버라이드 가능)
    public virtual void Interaction()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    // 강조 효과 활성화/비활성화
    public void HighlightActive(bool active)
    {
        if (active)
            mesh.material = highlightMaterial;
        else
            mesh.material = defaultMaterial;
    }

    // 트리거에 플레이어가 진입했을 때
    protected virtual void OnTriggerEnter(Collider other)
    {
        // 플레이어의 무기 컨트롤러를 캐싱
        if (weaponController == null)
            weaponController = other.GetComponent<Player_WeaponController>();

        // 플레이어 상호작용 시스템을 가져옴
        Player_Interaction playerInteraction = other.GetComponent<Player_Interaction>();

        // 상호작용 가능한 객체 목록에 추가
        if (playerInteraction == null)
            return;

        playerInteraction.GetInteracbles().Add(this);
        playerInteraction.UpdateClosestInteractble(); // 가장 가까운 상호작용 객체 업데이트
    }

    // 트리거에서 플레이어가 나갔을 때
    protected virtual void OnTriggerExit(Collider other)
    {
        // 플레이어 상호작용 시스템을 가져옴
        Player_Interaction playerInteraction = other.GetComponent<Player_Interaction>();

        // 상호작용 가능한 객체 목록에서 제거
        if (playerInteraction == null)
            return;

        playerInteraction.GetInteracbles().Remove(this);
        playerInteraction.UpdateClosestInteractble(); // 가장 가까운 상호작용 객체 업데이트
    }
}
