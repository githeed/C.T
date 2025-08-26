using UnityEngine;

public class SavedPeople : MonoBehaviour
{
    // FSM ���� ����
    public enum State
    {
        WaitingForRescue,  // �յ�� ��ٸ��� ����
        Moving,            // �̵� ����
        Arrived            // ���� �Ϸ� ����
    }

    [Header("State Management")]
    [SerializeField] private State currentState = State.WaitingForRescue;

    [Header("Movement Settings")]
    [SerializeField] private Transform targetDestination; // �̵��� ������
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float arrivalDistance = 0.5f; // ���� ���� �Ÿ�

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string handUpAnimationTrigger = "HandUp";
    [SerializeField] private string walkAnimationBool = "IsWalking";
    [SerializeField] private string idleAnimationTrigger = "Idle";

    [Header("Components")]
    [SerializeField] private CharacterController controller; // CharacterController ���

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Mission Reference")]
    public TreeMissionClear tmc;

    // ���� ����
    private Vector3 startPosition;
    private Vector3 velocity;
    private bool isMoving = false;
    private bool isGrounded;

    void Start()
    {
        // �ʱ�ȭ
        startPosition = transform.position;

        // Animator ������Ʈ ��������
        if (animator == null)
            animator = GetComponent<Animator>();

        // CharacterController ����
        if (controller == null)
            controller = GetComponent<CharacterController>();

        // �ʱ� ���� ����
        ChangeState(State.WaitingForRescue);
    }

    void Update()
    {
        // �ٴ� üũ
        CheckGround();

        // FSM ������Ʈ
        UpdateStateMachine();

        // ���� ��ȯ üũ
        CheckStateTransitions();

        // �߷� ����
        ApplyGravity();
    }

    // �ٴ� üũ
    void CheckGround()
    {
        // groundCheck�� �����Ǿ� ������ ���, �ƴϸ� controller.isGrounded ���
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            isGrounded = controller.isGrounded;
        }

        // �ٴڿ� �ְ� �������� ���̸� �ӵ� ����
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // �ٴڿ� �پ��ְ� �ϱ� ���� ���� ������
        }
    }

    // �߷� ����
    void ApplyGravity()
    {
        // �߷� ���ӵ� ����
        velocity.y += gravity * Time.deltaTime;

        // Y�� �̵��� ���� (���� �̵��� UpdateMovingState���� ó��)
        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    // ���� �ӽ� ������Ʈ
    void UpdateStateMachine()
    {
        switch (currentState)
        {
            case State.WaitingForRescue:
                UpdateWaitingState();
                break;

            case State.Moving:
                UpdateMovingState();
                break;

            case State.Arrived:
                UpdateArrivedState();
                break;
        }
    }

    // ���� ��ȯ üũ
    void CheckStateTransitions()
    {
        switch (currentState)
        {
            case State.WaitingForRescue:
                // treesCleared�� true�� �Ǹ� �̵� ���·� ��ȯ
                if (tmc != null && tmc.treesCleared)
                {
                    ChangeState(State.Moving);
                }
                break;

            case State.Moving:
                // ������ ���� üũ
                if (HasArrivedAtDestination())
                {
                    ChangeState(State.Arrived);
                }
                break;
        }
    }

    // ========== �� ���º� ������Ʈ �Լ� ==========

    void UpdateWaitingState()
    {
        // �յ� �ִϸ��̼� ����
        // ����׿� - ���� �ð����� ���� ��û
        if (Time.frameCount % 180 == 0) // �� 3�ʸ��� (60fps ����)
        {
            Debug.Log($"{gameObject.name}: Help! Please clear the trees!");
        }
    }

    void UpdateMovingState()
    {
        if (targetDestination == null)
        {
            Debug.LogWarning("Target destination is not set!");
            return;
        }

        // ��ǥ ���� ���
        Vector3 direction = (targetDestination.position - transform.position).normalized;
        direction.y = 0; // Y�� �̵� ���� (���� �̵���)

        // CharacterController�� �̵�
        if (controller != null && direction.magnitude > 0.1f)
        {
            // ���� �̵�
            Vector3 moveVector = direction * moveSpeed * Time.deltaTime;
            controller.Move(moveVector);

            // ĳ���� ȸ��
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    void UpdateArrivedState()
    {
        // ���� �� ��� ����
        // �ʿ�� �߰� ���� (��: ���� �λ�, �ٸ� �ִϸ��̼� ��)
    }

    // ========== ���� ��ȯ �Լ� ==========

    void ChangeState(State newState)
    {
        // ���� ���� ���� ó��
        OnStateExit(currentState);

        // ���� ����
        State previousState = currentState;
        currentState = newState;

        // �� ���� ���� ó��
        OnStateEnter(currentState);

        Debug.Log($"{gameObject.name}: State changed from {previousState} to {newState}");
    }

    void OnStateEnter(State state)
    {
        switch (state)
        {
            case State.WaitingForRescue:
                // �յ� �ִϸ��̼� ����
                if (animator != null)
                {
                    animator.SetTrigger(handUpAnimationTrigger);
                }
                break;

            case State.Moving:
                // �ȱ� �ִϸ��̼� ����
                if (animator != null)
                {
                    animator.SetBool(walkAnimationBool, true);
                }
                isMoving = true;
                break;

            case State.Arrived:
                // ��� �ִϸ��̼�
                if (animator != null)
                {
                    animator.SetTrigger(idleAnimationTrigger);
                }
                break;
        }
    }

    void OnStateExit(State state)
    {
        switch (state)
        {
            case State.WaitingForRescue:
                // �յ� �ִϸ��̼� ����
                break;

            case State.Moving:
                // �ȱ� �ִϸ��̼� ����
                if (animator != null)
                {
                    animator.SetBool(walkAnimationBool, false);
                }
                isMoving = false;
                break;

            case State.Arrived:
                break;
        }
    }

    // ========== ��ƿ��Ƽ �Լ� ==========

    bool HasArrivedAtDestination()
    {
        if (targetDestination == null) return false;

        // Y�� ������ ���� �Ÿ��� üũ
        Vector3 flatPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatDestination = new Vector3(targetDestination.position.x, 0, targetDestination.position.z);

        float distance = Vector3.Distance(flatPosition, flatDestination);
        return distance <= arrivalDistance;
    }

    // ========== Public �Լ� (�ܺο��� ȣ�� ����) ==========

    // ������ ����
    public void SetDestination(Transform destination)
    {
        targetDestination = destination;
    }

    // ���� ���� ����
    public void ForceChangeState(State newState)
    {
        ChangeState(newState);
    }

    // ���� ���� ��ȯ
    public State GetCurrentState()
    {
        return currentState;
    }

    // ������ ���ŵǾ��� �� ȣ�� (�ܺο��� ��� ����)
    public void OnTreesCleared()
    {
        if (tmc != null)
        {
            tmc.treesCleared = true;
        }
    }

    // ========== ����׿� ==========

    void OnDrawGizmos()
    {
        // ������ ǥ��
        if (targetDestination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetDestination.position, 0.5f);

            // ���� ��ġ���� ���������� ��
            if (Application.isPlaying && currentState == State.Moving)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetDestination.position);
            }
        }

        // �ٴ� üũ ���� ǥ��
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

#if UNITY_EDITOR
        // ���� ���� ǥ�� (�����Ϳ�����)
        if (Application.isPlaying)
        {
            Vector3 labelPos = transform.position + Vector3.up * 2f;
            UnityEditor.Handles.Label(labelPos, $"State: {currentState}");
        }
#endif
    }
}