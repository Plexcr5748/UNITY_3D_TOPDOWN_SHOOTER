using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ZoneLimitation: 특정 영역에서의 제한 및 효과를 관리하는 클래스
public class ZoneLimitation : MonoBehaviour
{
    private ParticleSystem[] lines; // 자식 오브젝트에서 ParticleSystem 배열
    private BoxCollider zoneCollider; // 영역을 정의하는 BoxCollider

    private void Start()
    {
        // MeshRenderer 비활성화 (시각적으로 보이지 않게 설정)
        GetComponent<MeshRenderer>().enabled = false;

        // BoxCollider 및 자식 ParticleSystem 컴포넌트 가져오기
        zoneCollider = GetComponent<BoxCollider>();
        lines = GetComponentsInChildren<ParticleSystem>();

        // 초기 상태로 벽 비활성화
        ActivateWall(false);
    }

    private void ActivateWall(bool activate)
    {
        // 벽의 활성화/비활성화 처리
        foreach (var line in lines)
        {
            if (activate)
            {
                line.Play(); // ParticleSystem 활성화
            }
            else
            {
                line.Stop(); // ParticleSystem 비활성화
            }
        }

        // Collider를 트리거로 설정 (활성화 여부에 따라 변경)
        zoneCollider.isTrigger = !activate;
    }

    IEnumerator WallActivationCo()
    {
        // 벽을 활성화하고 1초 후 비활성화
        ActivateWall(true);

        yield return new WaitForSeconds(1);

        ActivateWall(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트리거에 다른 오브젝트가 진입하면 벽 활성화 코루틴 실행
        StartCoroutine(WallActivationCo());
        Debug.Log("My sensors are going crazy, I think it's dangerous!");
    }
}
