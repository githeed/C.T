using UnityEngine;

public class H_CamController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // 플레이어 Transform

    [Header("Camera Settings")]
    public float distance = 5.0f; // 카메라와 플레이어 사이의 거리
    public float height = 2.0f; // 카메라 높이 오프셋

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
        // 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 초기 각도 설정
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
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 회전 각도 계산
        currentX += mouseX;
        currentY -= mouseY;

        // 수직 회전 제한
        currentY = Mathf.Clamp(currentY, mouseYMinLimit, mouseYMaxLimit);
    }

    void UpdateCameraPosition()
    {
        // 목표 위치와 회전 계산
        Vector3 targetPosition = target.position + Vector3.up * height;
        Quaternion targetRotation = Quaternion.Euler(currentY, currentX, 0);

        // 카메라가 위치할 지점 계산
        Vector3 direction = targetRotation * Vector3.back;
        Vector3 desiredPosition = targetPosition + direction * desiredDistance;

        // 벽 충돌 검사
        if (enableWallAvoidance)
        {
            desiredPosition = CheckWallCollision(targetPosition, desiredPosition);
        }

        // 부드러운 이동과 회전
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionDamping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
    }

    Vector3 CheckWallCollision(Vector3 targetPos, Vector3 desiredPos)
    {
        RaycastHit hit;
        Vector3 direction = (desiredPos - targetPos).normalized;
        float targetDistance = Vector3.Distance(targetPos, desiredPos);

        // 타겟에서 카메라 위치까지 레이캐스트
        if (Physics.Raycast(targetPos, direction, out hit, targetDistance, collisionLayers))
        {
            // 벽에 충돌하면 충돌 지점 앞으로 카메라 위치 조정
            return hit.point - direction * collisionOffset;
        }

        return desiredPos;
    }

    // 공개 메서드들
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

    // ESC 키로 커서 잠금/해제
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
