using UnityEngine;

public class H_CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;

    [Header("Camera Reference")]
    public Transform cameraTransform; // 카메라 Transform (카메라의 부모 오브젝트)
    private H_CamController cameraController;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask = 1;

    [Header("Animation")]
    public Animator animator; // 애니메이터 (선택사항)

    // 컴포넌트
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // 애니메이션 해시값 (성능 최적화)
    private int speedHash;
    private int isRunningHash;
    private int isGroundedHash;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 카메라 자동 찾기 및 연결
        if (cameraTransform == null)
        {
            H_CamController camera = FindObjectOfType<H_CamController>();
            if (camera != null)
            {
                cameraTransform = camera.transform;
                cameraController = camera;
                // 카메라에게 이 플레이어를 타겟으로 설정
                camera.SetTarget(this.transform);
            }
        }
        else
        {
            // 수동으로 할당된 카메라가 있다면 컨트롤러 참조 가져오기
            cameraController = cameraTransform.GetComponent<H_CamController>();
        }

        // Ground Check가 없으면 자동 생성
        if (groundCheck == null)
        {
            GameObject groundChecker = new GameObject("GroundCheck");
            groundChecker.transform.SetParent(transform);
            groundChecker.transform.localPosition = new Vector3(0, -1.0f, 0);
            groundCheck = groundChecker.transform;
        }

        // 애니메이션 해시값 캐싱
        if (animator != null)
        {
            speedHash = Animator.StringToHash("Speed");
            isRunningHash = Animator.StringToHash("IsRunning");
            isGroundedHash = Animator.StringToHash("IsGrounded");
        }
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleAnimation();
    }

    void HandleGroundCheck()
    {
        // 바닥 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 바닥에 닿아있고 떨어지고 있다면 속도 리셋
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        // 입력 받기
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 카메라 기준으로 이동 방향 계산
        Vector3 direction = Vector3.zero;

        if (cameraTransform != null)
        {
            // 카메라의 앞쪽과 오른쪽 벡터 가져오기 (Y축 제거)
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            // 이동 방향 계산
            direction = forward * vertical + right * horizontal;
        }
        else
        {
            // 카메라가 없으면 월드 좌표계 기준으로 이동
            direction = new Vector3(horizontal, 0f, vertical);
        }

        // 이동 속도 결정 (Shift 키로 달리기)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // 이동 적용
        if (direction.magnitude >= 0.1f)
        {
            // 플레이어 회전 (이동 방향을 향하도록)
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.AngleAxis(targetAngle, Vector3.up), Time.deltaTime * 10f);

            // 이동
            controller.Move(direction * currentSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        // 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAnimation()
    {
        if (animator == null) return;

        // 이동 속도 계산
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float speed = horizontalVelocity.magnitude;

        // 애니메이션 파라미터 설정
        animator.SetFloat(speedHash, speed);
        animator.SetBool(isRunningHash, Input.GetKey(KeyCode.LeftShift) && speed > 0.1f);
        animator.SetBool(isGroundedHash, isGrounded);
    }

    // 기즈모로 Ground Check 영역 표시
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    // 공개 메서드들
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsMoving()
    {
        return controller.velocity.magnitude > 0.1f;
    }

    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }

    public void SetCameraTransform(Transform camera)
    {
        cameraTransform = camera;
        cameraController = camera.GetComponent<H_CamController>();
    }

    public H_CamController GetCameraController()
    {
        return cameraController;
    }

    public void SetCameraDistance(float distance)
    {
        if (cameraController != null)
        {
            cameraController.SetDistance(distance);
        }
    }

    public void SetCameraSensitivity(float sensitivity)
    {
        if (cameraController != null)
        {
            cameraController.SetSensitivity(sensitivity);
        }
    }
}
