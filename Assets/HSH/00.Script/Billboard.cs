using UnityEngine;

public class Billboard : MonoBehaviour
{
    public enum BillboardType
    {
        LookAtCamera,      // ī�޶� ������ �ٶ�
        YAxisOnly          // Y�� ȸ���� (���� ������)
    }

    [Header("������ ����")]
    public BillboardType billboardType = BillboardType.LookAtCamera;

    [Header("ī�޶� ����")]
    public Camera targetCamera;
    public bool useMainCamera = true;

    [Header("�߰� �ɼ�")]
    public bool reverseFace = false;  // �޸��� �������� ����
    public Vector3 rotationOffset = Vector3.zero;  // �߰� ȸ�� ������

    private void Start()
    {
        // ī�޶� �������� �ʾҰ� ���� ī�޶� ����ϵ��� ������ ���
        if (targetCamera == null && useMainCamera)
        {
            targetCamera = Camera.main;

            if (targetCamera == null)
            {
                Debug.LogError("Billboard: ���� ī�޶� ã�� �� �����ϴ�!");
            }
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                LookAtCameraFull();
                break;
            case BillboardType.YAxisOnly:
                LookAtCameraYAxis();
                break;
        }
    }

    // ī�޶� ������ �ٶ󺸴� ������
    private void LookAtCameraFull()
    {
        Vector3 lookDirection = targetCamera.transform.position - transform.position;

        if (reverseFace)
            lookDirection = -lookDirection;

        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = rotation * Quaternion.Euler(rotationOffset);
    }

    // Y�� ȸ���� �ϴ� ������ (���� ������)
    private void LookAtCameraYAxis()
    {
        Vector3 lookDirection = targetCamera.transform.position - transform.position;
        lookDirection.y = 0;  // Y ������ 0���� ����� ���� ���⸸ ���

        if (lookDirection != Vector3.zero)
        {
            if (reverseFace)
                lookDirection = -lookDirection;

            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation * Quaternion.Euler(rotationOffset);
        }
    }

    //// �����Ϳ��� ����� ǥ�� (������)
    //private void OnDrawGizmosSelected()
    //{
    //    if (targetCamera != null)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawLine(transform.position, targetCamera.transform.position);
    //    }
    //}
}


