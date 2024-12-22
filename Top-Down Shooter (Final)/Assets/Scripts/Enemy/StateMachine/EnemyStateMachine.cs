// ���� ���� ���(���� ����)�� ����ϴ� Ŭ����
public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; } // ���� ����

    // ���� ��� �ʱ�ȭ: ���� ���¸� �����ϰ�, �� ���·� ����
    public void Initialize(EnemyState startState)
    {
        currentState = startState; // ���� ���� ����
        currentState.Enter(); // ���� ���·� ����
    }

    // ���� ����: ���� ���¸� �����ϰ� �� ���·� ����
    public void ChangeState(EnemyState newState)
    {
        currentState.Exit(); // ���� ���� ����
        currentState = newState; // �� ���·� ����
        currentState.Enter(); // �� ���·� ����
    }
}
