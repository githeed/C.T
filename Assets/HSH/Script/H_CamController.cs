using UnityEngine;

public class H_CamController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // �÷��̾� Transform

    [Header("Camera Settings")]
    public float distance = 5.0f; // ī�޶�� �÷��̾� ������ �Ÿ�
    public float height = 2.0f; // ī�޶� ���� ������

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100.0f;
    public float mouseYMinLimit = -60.0f;
    public float mouseYMaxLimit = 60.0f;

    [Header("Smoothing")]
    public float rotationDamping = 3.0f;
    public float positionDamping = 3.0f;

    [Header("Collision Detection")]
    public bool enableWallAvoidance = true;
    public LayerMask collisionLayers = -1;
    public float collisionOffset = 0.3f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float desiredDistance;

    void Start()
    {
        // Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // �ʱ� ���� ����
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        desiredDistance = distance;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        HandleMouseInput();
        UpdateCameraPosition();
    }

    void HandleMouseInput()
    {
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ȸ�� ���� ���
        currentX += mouseX;
        currentY -= mouseY;

        // ���� ȸ�� ����
        currentY = Mathf.Clamp(currentY, mouseYMinLimit, mouseYMaxLimit);
    }

    void UpdateCameraPosition()
    {
        // ��ǥ ��ġ�� ȸ�� ���
        Vector3 targetPosition = target.position + Vector3.up * height;
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
    }

    public void SetDistance(float newDistance)
    {
        distance = newDistance;
        desiredDistance = newDistance;
    }

    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    // ESC Ű�� Ŀ�� ���/����
    void Update()
    {
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
    }
}
