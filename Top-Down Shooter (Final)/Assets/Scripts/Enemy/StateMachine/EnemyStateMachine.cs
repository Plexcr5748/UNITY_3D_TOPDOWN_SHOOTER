// 적의 상태 기계(상태 관리)를 담당하는 클래스
public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; } // 현재 상태

    // 상태 기계 초기화: 시작 상태를 설정하고, 그 상태로 진입
    public void Initialize(EnemyState startState)
    {
        currentState = startState; // 시작 상태 설정
        currentState.Enter(); // 시작 상태로 진입
    }

    // 상태 변경: 현재 상태를 종료하고 새 상태로 변경
    public void ChangeState(EnemyState newState)
    {
        currentState.Exit(); // 현재 상태 종료
        currentState = newState; // 새 상태로 변경
        currentState.Enter(); // 새 상태로 진입
    }
}
