using UnityEngine;

public class H_CamController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // �÷��̾� Transform

    [Header("Camera Settings")]
    public float distance = 5.0f; // ī�޶�� �÷��̾� ������ �Ÿ�
    public float height = 2.0f; // ī�޶� ���� ������

    [Header("Distance Control")]
    public float minDistance = 1.0f; // �ּ� �Ÿ�
    public float maxDistance = 10.0f; // �ִ� �Ÿ�
    public float distanceStep = 4f; // NŰ ���� ������ �پ��� �Ÿ�
    public float distanceChangeSpeed = 5.0f; // �Ÿ� ���� �ӵ� (�ε巯�� ��ȯ��)

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100.0f;

    [Header("Vertical Rotation Limits")]
    [Range(-89, 0)]
    public float minVerticalAngle = -60.0f; // �Ʒ��� ���� ���� (����)
    [Range(0, 89)]
    public float maxVerticalAngle = 60.0f; // ���� ���� ���� (���)

    [Header("Smoothing")]
    public float rotationDamping = 3.0f;
    public float positionDamping = 3.0f;

    [Header("Collision Detection")]
    public bool enableWallAvoidance = true;
    public LayerMask collisionLayers = -1;
    public float collisionOffset = 0.3f;

    private float currentX = 0.0f; // ���� ȸ�� (Y�� ȸ��)
    private float currentY = 0.0f; // ���� ȸ�� (X�� ȸ��)
    private float desiredDistance;
    private float targetDistance; // ��ǥ �Ÿ� (�ε巯�� ��ȯ��)

    // ����׿� ����
    [Header("Debug Info")]
    [SerializeField] private float currentVerticalAngle; // ���� ���� ���� ǥ��
    [SerializeField] private float currentDistance; // ���� �Ÿ� ǥ��

    void Start()
    {
        // Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // �ʱ� ���� ����
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;

        // �ʱ� ���� ���� ���� (0-360���� -180~180���� ��ȯ)
        currentY = angles.x;
        if (currentY > 180)
            currentY -= 360;

        // �ʱ� ������ ���� ���� ���� Ŭ����
        currentY = ClampVerticalAngle(currentY);

        desiredDistance = distance;
        targetDistance = distance;

        // �÷��̾� �ڵ� ã�� �� Ÿ�� ����
        if (target == null)
        {

            H_CharacterMovement player = FindFirstObjectByType<H_CharacterMovement>(FindObjectsInactive.Exclude);

            if (player != null)
            {
                target = player.transform;
                // �÷��̾�Ե� ī�޶� ���� ����
                player.SetCameraTransform(this.transform);
            }
        }

        // ���� �� ��ȿ�� �˻�
        ValidateLimits();
    }

    void ValidateLimits()
    {
        // ���� ���� �ùٸ� ������ �ִ��� Ȯ��
        minVerticalAngle = Mathf.Clamp(minVerticalAngle, -89f, 0f);
        maxVerticalAngle = Mathf.Clamp(maxVerticalAngle, 0f, 89f);

        // �ּҰ��� �ִ밪���� ũ�� �ʵ���
        if (minVerticalAngle > maxVerticalAngle)
        {
            Debug.LogWarning("minVerticalAngle�� maxVerticalAngle���� Ů�ϴ�. ���� ��ȯ�մϴ�.");
            float temp = minVerticalAngle;
            minVerticalAngle = maxVerticalAngle;
            maxVerticalAngle = temp;
        }

        // �Ÿ� ���� ����
        minDistance = Mathf.Max(0.1f, minDistance);
        maxDistance = Mathf.Max(minDistance + 0.1f, maxDistance);
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        HandleMouseInput();
        UpdateCameraDistance();
        UpdateCameraPosition();

        // ����׿� ���� �� ������Ʈ
        currentVerticalAngle = currentY;
        currentDistance = desiredDistance;
    }

    void HandleMouseInput()
    {
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ���� ȸ�� (Y��, ���� ����)
        currentX += mouseX;

        // ���� ȸ�� (X��, ���� ����)
        currentY -= mouseY;
        currentY = ClampVerticalAngle(currentY);
    }

    float ClampVerticalAngle(float angle)
    {
        // ������ -180 ~ 180 ������ ����ȭ
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;

        // ���� ���� ���� ����
        return Mathf.Clamp(angle, minVerticalAngle, maxVerticalAngle);
    }

    void UpdateCameraDistance()
    {
        // �ε巯�� �Ÿ� ��ȯ
        desiredDistance = Mathf.Lerp(desiredDistance, targetDistance, distanceChangeSpeed * Time.deltaTime);
    }

    void UpdateCameraPosition()
    {
        // ��ǥ ��ġ�� ȸ�� ���
        Vector3 targetPosition = target.position + Vector3.up * height;

        // ���ѵ� ������ ȸ�� ����
        Quaternion targetRotation = Quaternion.Euler(currentY, currentX, 0);

        // ī�޶� ��ġ�� ���� ���
        Vector3 direction = targetRotation * Vector3.back;
        Vector3 desiredPosition = targetPosition + direction * desiredDistance;

        // �� �浹 �˻�
        if (enableWallAvoidance)
        {
            desiredPosition = CheckWallCollision(targetPosition, desiredPosition);
        }

        // �ε巯�� �̵��� ȸ��
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionDamping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
    }

    Vector3 CheckWallCollision(Vector3 targetPos, Vector3 desiredPos)
    {
        RaycastHit hit;
        Vector3 direction = (desiredPos - targetPos).normalized;
        float targetDistance = Vector3.Distance(targetPos, desiredPos);

        // Ÿ�ٿ��� ī�޶� ��ġ���� ����ĳ��Ʈ
        if (Physics.Raycast(targetPos, direction, out hit, targetDistance, collisionLayers))
        {
            // ���� �浹�ϸ� �浹 ���� ������ ī�޶� ��ġ ����
            return hit.point - direction * collisionOffset;
        }

        return desiredPos;
    }

    // ���� �޼����
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        // Ÿ���� �÷��̾��� ī�޶� ������ ����
        H_CharacterMovement player = target.GetComponent<H_CharacterMovement>();
        if (player != null)
        {
            player.SetCameraTransform(this.transform);
        }
    }

    public void SetDistance(float newDistance)
    {
        distance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        targetDistance = distance;
        desiredDistance = distance;
    }

    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    // �Ÿ��� ���̴� �޼���
    public void DecreaseDistance()
    {
        targetDistance = Mathf.Max(minDistance, targetDistance - distanceStep);
        distance = targetDistance;
    }

    // �Ÿ��� �ø��� �޼���
    public void IncreaseDistance()
    {
        targetDistance = Mathf.Min(maxDistance, targetDistance + distanceStep);
        distance = targetDistance;
    }

    // �Ÿ��� Ư�� ������ ����
    public void SetDistanceImmediate(float newDistance)
    {
        distance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        targetDistance = distance;
        desiredDistance = distance;
    }

    // ���� ���� ���� ����
    public void SetVerticalLimits(float min, float max)
    {
        minVerticalAngle = Mathf.Clamp(min, -89f, 0f);
        maxVerticalAngle = Mathf.Clamp(max, 0f, 89f);
        ValidateLimits();

        // ���� ������ ���ο� ���ѿ� �°� ����
        currentY = ClampVerticalAngle(currentY);
    }

    // ���� ���� ���� ��������
    public float GetCurrentVerticalAngle()
    {
        return currentY;
    }

    // ���� �Ÿ� ��������
    public float GetCurrentDistance()
    {
        return desiredDistance;
    }

    // ���� ���� ����
    public void ResetVerticalLimits()
    {
        minVerticalAngle = -60f;
        maxVerticalAngle = 60f;
    }

    // �Ÿ� ����
    public void ResetDistance()
    {
        distance = 5.0f;
        targetDistance = distance;
        desiredDistance = distance;
    }

    // ESC Ű�� Ŀ�� ���/����
    void Update()
    {
        // ESC Ű - Ŀ�� ���/����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // N Ű - ī�޶� �÷��̾�� ������
        if (Input.GetKeyDown(KeyCode.N))
        {
            DecreaseDistance();
            Debug.Log($"ī�޶� �Ÿ�: {targetDistance:F2}");
        }

        // M Ű - ī�޶� �÷��̾�Լ� �ָ� (�߰� ���)
        if (Input.GetKeyDown(KeyCode.M))
        {
            IncreaseDistance();
            Debug.Log($"ī�޶� �Ÿ�: {targetDistance:F2}");
        }

        // ���콺 �ٷ� �Ÿ� ���� (�߰� ���)
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {
            targetDistance = Mathf.Clamp(targetDistance - scrollWheel * distanceStep * 10, minDistance, maxDistance);
            distance = targetDistance;
        }

        // ����׿� - ���� ���� �ǽð� ���� (���� �߿��� ���)
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetVerticalLimits();
                Debug.Log($"���� ���� ���� ����: [{minVerticalAngle}, {maxVerticalAngle}]");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                ResetDistance();
                Debug.Log($"�Ÿ� ����: {distance}");
            }
        }
#endif
    }

    // ������ ���� ���� �ð�ȭ (�����Ϳ�����)
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + Vector3.up * height;

        // ���� ī�޶� ����
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(targetPos, transform.position);

        // �ִ� ���� ����
        Gizmos.color = Color.red;
        Vector3 maxDir = Quaternion.Euler(maxVerticalAngle, currentX, 0) * Vector3.back * desiredDistance;
        Gizmos.DrawLine(targetPos, targetPos + maxDir);

        // �ִ� �Ʒ��� ����
        Gizmos.color = Color.blue;
        Vector3 minDir = Quaternion.Euler(minVerticalAngle, currentX, 0) * Vector3.back * desiredDistance;
        Gizmos.DrawLine(targetPos, targetPos + minDir);

        // ���� ���� ȣ �׸���
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        int segments = 20;
        float angleRange = maxVerticalAngle - minVerticalAngle;
        Vector3 prevPoint = targetPos + minDir;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = minVerticalAngle + angleRange * t;
            Vector3 dir = Quaternion.Euler(angle, currentX, 0) * Vector3.back * desiredDistance;
            Vector3 point = targetPos + dir;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }

        // �Ÿ� ���� �ð�ȭ
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(targetPos, minDistance);
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawWireSphere(targetPos, maxDistance);
    }
#endif
}