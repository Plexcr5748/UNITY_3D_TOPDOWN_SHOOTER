using System.Collections.Generic;
using UnityEngine;

// Player_Interaction: 플레이어와 상호작용 가능한 객체를 관리하는 클래스
public class Player_Interaction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>(); // 상호작용 가능한 객체 목록
    private Interactable closestInteractable; // 가장 가까운 상호작용 가능한 객체

    private void Start()
    {
        // 플레이어의 입력 컨트롤에서 상호작용 이벤트 등록
        Player player = GetComponent<Player>();
        player.controls.Character.Interaction.performed += context => InteractWithClosest();
    }

    private void InteractWithClosest()
    {
        // 가장 가까운 객체와 상호작용
        closestInteractable?.Interaction();

        // 상호작용 후 해당 객체를 리스트에서 제거
        interactables.Remove(closestInteractable);

        // 가장 가까운 상호작용 가능한 객체 업데이트
        UpdateClosestInteractble();
    }

    public void UpdateClosestInteractble()
    {
        // 기존 가장 가까운 객체의 하이라이트 비활성화
        closestInteractable?.HighlightActive(false);
        closestInteractable = null;

        float closestDistance = float.MaxValue; // 초기화된 가장 가까운 거리

        // 상호작용 가능한 객체들 중 가장 가까운 객체를 찾음
        foreach (Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        // 가장 가까운 객체의 하이라이트 활성화
        closestInteractable?.HighlightActive(true);
    }

    public List<Interactable> GetInteracbles() => interactables;
    // 현재 상호작용 가능한 객체 목록 반환
}
