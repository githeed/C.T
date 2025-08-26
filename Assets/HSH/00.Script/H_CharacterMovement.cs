using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class H_CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    public float rotationSmoothTime = 0.1f;

    [Header("Camera Reference")]
    public Transform cameraTransform;
    private H_CamController cameraController;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask = 1;

    // ������Ʈ
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentRotationVelocity;

    // ���� �̵� ����
    private float currentSpeed;
    private Vector3 moveDirection;
    private bool isRunning;
    public bool canRun;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Animator�� ������ ���
        if (animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }

        // ī�޶� �ڵ� ã�� �� ����
        if (cameraTransform == null)
        {
            H_CamController camera = FindFirstObjectByType<H_CamController>(FindObjectsInactive.Exclude);
            if (camera != null)
            {
                cameraTransform = camera.transform;
                cameraController = camera;
                camera.SetTarget(this.transform);
            }
        }
        else
        {
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

        canRun = true;
    }

    void Update()
    {
        if(GameManager.Instance.isUIWorking)
        {
            animator.SetFloat("Move", 0);
        }

        if (GameManager.Instance.status == GameStatus.Ready || GameManager.Instance.status == GameStatus.Ending || GameManager.Instance.isUIWorking) return;
        HandleGroundCheck();
        HandleMovement();
        UpdateAnimator();
        HandleJump();
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

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

        // �Է� ���� ����ȭ
        Vector2 input = new Vector2(horizontal, vertical);
        if (input.magnitude > 1f)
        {
            input.Normalize();
        }

        // ī�޶� �������� �̵� ���� ���
        moveDirection = Vector3.zero;

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDirection = forward * input.y + right * input.x;
        }
        else
        {
            moveDirection = new Vector3(input.x, 0f, input.y);
        }

        // �޸��� üũ
        isRunning = Input.GetKey(KeyCode.LeftShift) && moveDirection.magnitude > 0.1f;

        // ���� �ӵ� ���
        if (moveDirection.magnitude >= 0.1f)
        {
            float targetSpeed = isRunning ? runSpeed : walkSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

            // �÷��̾� ȸ�� (�ε巴��)
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                ref currentRotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // �̵�
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 10f);
        }
    }

    void HandleJump()
    {
        //if (Input.GetButtonDown("Jump") && isGrounded)
        //{
        //    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        //    // ���� �ִϸ��̼� Ʈ���� (�ʿ��)
        //    // animator.SetTrigger("Jump");
        //}

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        // Blend Tree �Ķ���� ������Ʈ

        // ����ȭ�� �ӵ� ��� (0 = idle, 0.5 = walk, 1 = run)
        float normalizedSpeed = 0f;

        if (currentSpeed <= 0.1f)
        {
            // Idle
            normalizedSpeed = 0f;
        }
        else if (!isRunning)
        {
            // Walking
            normalizedSpeed = Mathf.InverseLerp(0f, walkSpeed, currentSpeed) * 0.5f;
        }
        else
        {
            // Running
            normalizedSpeed = Mathf.InverseLerp(walkSpeed, runSpeed, currentSpeed) * 0.5f + 0.5f;
        }

        animator.SetFloat("Move", normalizedSpeed);
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
        return currentSpeed > 0.1f;
    }

    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public bool IsRunning()
    {
        return isRunning;
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