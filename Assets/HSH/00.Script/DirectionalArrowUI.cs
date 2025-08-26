using UnityEngine;

public class DirectionalArrowUI : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform targetLocation; // �̼� ��ǥ ��ġ
    [SerializeField] private Transform player; // �÷��̾� Transform

    [Header("Arrow Settings")]
    [SerializeField] private float heightOffset = 2.5f; // �÷��̾� �Ӹ� �� ����
    [SerializeField] private float rotationSpeed = 5f; // ȸ�� �ӵ� (�ε巯�� ȸ����)
    [SerializeField] private bool smoothRotation = true; // �ε巯�� ȸ�� ����

    [Header("Distance Display")]
    [SerializeField] private bool showDistance = true; // �Ÿ� ǥ�� ����
    [SerializeField] private TMPro.TextMeshProUGUI distanceText; // �Ÿ� ǥ�� �ؽ�Ʈ (���û���)

    private void Start()
    {
        // �÷��̾ �������� �ʾҴٸ� �±׷� ã��
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void LateUpdate()
    {
        if (player == null || targetLocation == null)
            return;

        // ȭ��ǥ ��ġ�� �÷��̾� �Ӹ� ���� ����
        UpdateArrowPosition();

        // ȭ��ǥ ȸ�� (Y�ุ)
        UpdateArrowRotation();

        // �Ÿ� ������Ʈ (���û���)
        if (showDistance && distanceText != null)
        {
            UpdateDistanceDisplay();
        }
    }

    private void UpdateArrowPosition()
    {
        // �÷��̾� ��ġ���� Y�����θ� ������ ����
        Vector3 newPosition = player.position + Vector3.up * heightOffset;
        transform.position = newPosition;
    }

    private void UpdateArrowRotation()
    {
        // ��ǥ������ ���� ��� (Y�� ����)
        Vector3 direction = targetLocation.position - transform.position;
        direction.y = 0; // Y�� ȸ���� �ϱ� ���� Y ���̴� ����

        // ������ 0�� �ƴ� ���� ȸ��
        if (direction != Vector3.zero)
        {
            // ��ǥ ȸ���� ���
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Y�� ȸ���� ���� (X�� Z ȸ���� 0����)
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            // ȸ�� ����
            if (smoothRotation)
            {
                // �ε巯�� ȸ��
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                                    rotationSpeed * Time.deltaTime);
            }
            else
            {
                // ��� ȸ��
                transform.rotation = targetRotation;
            }
        }
    }

    private void UpdateDistanceDisplay()
    {
        // ��ǥ������ �Ÿ� ��� (XZ ������ �Ÿ�)
        Vector3 flatPlayerPos = new Vector3(player.position.x, 0, player.position.z);
        Vector3 flatTargetPos = new Vector3(targetLocation.position.x, 0, targetLocation.position.z);
        float distance = Vector3.Distance(flatPlayerPos, flatTargetPos);

        // �Ÿ� �ؽ�Ʈ ������Ʈ
        distanceText.text = $"{distance:F1}m";
    }

    // ��ǥ ��ġ ���� ����
    public void SetTarget(Transform newTarget)
    {
        targetLocation = newTarget;
    }

    // ��ǥ ��ġ ���� ���� (Vector3)
    public void SetTarget(Vector3 newTargetPosition)
    {
        // �� GameObject�� �����Ͽ� ��ġ�� ���
        GameObject targetObj = new GameObject("Target_Position");
        targetObj.transform.position = newTargetPosition;
        targetLocation = targetObj.transform;
    }

    // ȭ��ǥ ǥ��/�����
    public void ShowArrow(bool show)
    {
        gameObject.SetActive(show);
    }

    // ��ǥ ���� üũ
    public bool IsNearTarget(float threshold = 2f)
    {
        if (player == null || targetLocation == null)
            return false;

        Vector3 flatPlayerPos = new Vector3(player.position.x, 0, player.position.z);
        Vector3 flatTargetPos = new Vector3(targetLocation.position.x, 0, targetLocation.position.z);
        return Vector3.Distance(flatPlayerPos, flatTargetPos) <= threshold;
    }
}