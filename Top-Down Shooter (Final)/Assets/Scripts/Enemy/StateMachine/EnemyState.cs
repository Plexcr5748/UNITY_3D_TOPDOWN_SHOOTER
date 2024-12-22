using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 적의 상태를 관리하는 기본 클래스
public class EnemyState
{
    protected Enemy enemyBase; // 적 객체
    protected EnemyStateMachine stateMachine; // 상태 기계 객체

    protected string animBoolName; // 애니메이션을 위한 bool 변수 이름
    protected float stateTimer; // 상태의 타이머 (시간 관련 제어)

    protected bool triggerCalled; // 애니메이션 트리거 호출 여부

    // 생성자: 상태에 필요한 적 객체, 상태 기계, 애니메이션 bool 이름 초기화
    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // 상태 진입 시 호출되는 메서드
    public virtual void Enter()
    {
        enemyBase.anim.SetBool(animBoolName, true); // 애니메이션의 bool 값을 true로 설정하여 상태 애니메이션 시작
        triggerCalled = false; // 트리거 초기화
    }

    // 상태 업데이트 시 호출되는 메서드
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime; // 상태 타이머 감소
    }

    // 상태 종료 시 호출되는 메서드
    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false); // 애니메이션의 bool 값을 false로 설정하여 상태 애니메이션 종료
    }

    // 애니메이션 트리거를 호출하는 메서드
    public void AnimationTrigger() => triggerCalled = true;

    // 능력 발동 시 호출되는 메서드 (기본적으로 비워둠, 필요 시 오버라이드 가능)
    public virtual void AbilityTrigger()
    {

    }

    // 경로의 다음 지점을 얻는 메서드
    protected Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemyBase.agent; // 네비게이션 에이전트
        NavMeshPath path = agent.path; // 현재 경로

        // 경로의 코너가 2개 미만이면 목적지 반환
        if (path.corners.Length < 2)
            return agent.destination;

        // 경로를 따라가며 각 지점에 도달할 때마다 다음 지점으로 이동
        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1) // 현재 지점에 도달한 경우
                return path.corners[i + 1]; // 다음 지점 반환
        }

        return agent.destination; // 목적지 반환
    }
}
