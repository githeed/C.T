using UnityEngine;

public class H_CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;

    [Header("Camera Reference")]
    public Transform cameraTransform; // ī�޶� Transform (ī�޶��� �θ� ������Ʈ)
    private H_CamController cameraController;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask = 1;

    [Header("Animation")]
    public Animator animator; // �ִϸ����� (���û���)

    // ������Ʈ
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // �ִϸ��̼� �ؽð� (���� ����ȭ)
    private int speedHash;
    private int isRunningHash;
    private int isGroundedHash;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // ī�޶� �ڵ� ã�� �� ����
        if (cameraTransform == null)
        {
            H_CamController camera = FindObjectOfType<H_CamController>();
            if (camera != null)
            {
                cameraTransform = camera.transform;
                cameraController = camera;
                // ī�޶󿡰� �� �÷��̾ Ÿ������ ����
                camera.SetTarget(this.transform);
            }
        }
        else
        {
            // �������� �Ҵ�� ī�޶� �ִٸ� ��Ʈ�ѷ� ���� ��������
            cameraController = cameraTransform.GetComponent<H_CamController>();
        }

        // Ground Check�� ������ �ڵ� ����
        if (groundCheck == null)
        {
            GameObject groundChecker = new GameObject("GroundCheck");
            groundChecker.transform.SetParent(transform);
            groundChecker.transform.localPosition = new Vector3(0, -1.0f, 0);
            groundCheck = groundChecker.transform;
        }

        // �ִϸ��̼� �ؽð� ĳ��
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
        // �ٴ� üũ
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // �ٴڿ� ����ְ� �������� �ִٸ� �ӵ� ����
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        // �Է� �ޱ�
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // ī�޶� �������� �̵� ���� ���
        Vector3 direction = Vector3.zero;

        if (cameraTransform != null)
        {
            // ī�޶��� ���ʰ� ������ ���� �������� (Y�� ����)
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            // �̵� ���� ���
            direction = forward * vertical + right * horizontal;
        }
        else
        {
            // ī�޶� ������ ���� ��ǥ�� �������� �̵�
            direction = new Vector3(horizontal, 0f, vertical);
        }

        // �̵� �ӵ� ���� (Shift Ű�� �޸���)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // �̵� ����
        if (direction.magnitude >= 0.1f)
        {
            // �÷��̾� ȸ�� (�̵� ������ ���ϵ���)
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.AngleAxis(targetAngle, Vector3.up), Time.deltaTime * 10f);

            // �̵�
            controller.Move(direction * currentSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        // ����
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // �߷� ����
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAnimation()
    {
        if (animator == null) return;

        // �̵� �ӵ� ���
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float speed = horizontalVelocity.magnitude;

        // �ִϸ��̼� �Ķ���� ����
        animator.SetFloat(speedHash, speed);
        animator.SetBool(isRunningHash, Input.GetKey(KeyCode.LeftShift) && speed > 0.1f);
        animator.SetBool(isGroundedHash, isGrounded);
    }

    // ������ Ground Check ���� ǥ��
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    // ���� �޼����
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
