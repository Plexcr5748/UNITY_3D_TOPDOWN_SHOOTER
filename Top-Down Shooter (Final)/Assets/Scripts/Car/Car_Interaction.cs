// 자동차와의 상호작용을 처리하는 클래스

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Interaction : Interactable
{
    private Car_HealthController carHealthController; // 자동차 체력 관리
    private Car_Controller carController; // 자동차 컨트롤러 관리
    private Transform player; // 플레이어 캐릭터 참조

    private float defaultPlayerScale; // 플레이어의 기본 크기 저장

    [Header("Exit details")]
    [SerializeField] private float exitCheckRadius = .2f; // 출구 충돌 체크 반경
    [SerializeField] private Transform[] exitPoints; // 자동차에서 나갈 수 있는 지점들
    [SerializeField] private LayerMask whatToIngoreForExit; // 출구 확인 시 무시할 레이어

    private void Start()
    {
        carHealthController = GetComponent<Car_HealthController>(); // 자동차 체력 관리 컴포넌트 가져오기
        carController = GetComponent<Car_Controller>(); // 자동차 컨트롤러 컴포넌트 가져오기
        player = GameManager.instance.player.transform; // 게임 내 플레이어 참조 가져오기

        // 출구 지점의 렌더러와 충돌체를 비활성화
        foreach (var point in exitPoints)
        {
            point.GetComponent<MeshRenderer>().enabled = false;
            point.GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void Update()
    {
        // 자동차가 파괴되었을 경우 하이라이트를 기본 상태로 변경
        if (carController.carDead == true)
        {
            highlightMaterial = defaultMaterial;
        }
    }

    public override void Interaction()
    {
        base.Interaction(); // 상위 클래스의 Interaction 호출
        GetIntoTheCar(); // 자동차 탑승 처리
    }

    private void GetIntoTheCar()
    {
        if (carController.carDead == true)
            return; // 자동차가 파괴되었다면 동작하지 않음

        ControlsManager.instance.SwitchToCarControls(); // 자동차 조작으로 입력 변경
        carHealthController.UpdateCarHealthUI(); // 자동차 체력 UI 업데이트
        carController.ActivateCar(true); // 자동차 활성화

        defaultPlayerScale = player.localScale.x; // 플레이어의 기본 크기 저장

        // 플레이어를 자동차 내부로 축소 및 위치 이동
        player.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        player.transform.parent = transform;
        player.transform.localPosition = Vector3.up / 2;

        CameraManager.instance.ChangeCameraTarget(transform, 12, .5f); // 카메라 위치 변경
    }

    public void GetOutOfTheCar()
    {
        if (carController.carActive == false)
            return; // 자동차가 활성화되지 않았다면 동작하지 않음

        carController.ActivateCar(false); // 자동차 비활성화

        player.parent = null; // 플레이어 부모 해제
        player.position = GetExitPoint(); // 출구 지점으로 위치 이동
        player.transform.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale); // 플레이어 크기 복원

        ControlsManager.instance.SwitchToCharacterControls(); // 캐릭터 조작으로 입력 변경
        Player_AimController aim = GameManager.instance.player.aim; // 플레이어 에임 컨트롤러 가져오기

        CameraManager.instance.ChangeCameraTarget(aim.GetAimCameraTarget(), 8.5f); // 카메라 위치 복원
    }

    private Vector3 GetExitPoint()
    {
        // 출구 지점 중 가장 가까운 비어 있는 지점 반환
        for (int i = 0; i < exitPoints.Length; i++)
        {
            if (IsExitClear(exitPoints[i].position))
                return exitPoints[i].position;
        }

        return exitPoints[0].position; // 비어 있는 지점이 없으면 첫 번째 지점 반환
    }

    private bool IsExitClear(Vector3 point)
    {
        // 출구 지점이 비어 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(point, exitCheckRadius, ~whatToIngoreForExit);
        return colliders.Length == 0;
    }

    private void OnDrawGizmos()
    {
        // 출구 지점의 기즈모(디버깅용) 표시
        if (exitPoints.Length > 0)
        {
            foreach (var point in exitPoints)
            {
                Gizmos.DrawWireSphere(point.position, exitCheckRadius);
            }
        }
    }
}
