using UnityEngine;

public class TapePlacePoint : MonoBehaviour
{
    public enum PointType
    {
        StartPoint,
        EndPoint,
        AnyPoint  // �������̳� ���� �� �� ����
    }
    public PointType pointType = PointType.AnyPoint;


    private GameObject player;
    private TapePlacementSystem ps;
    private Renderer sphereRenderer;
    private bool isPlayerInside = false;

    void Start()
    {
        // �÷��̾�� TapePlacementSystem ã��
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            ps = FindFirstObjectByType<TapePlacementSystem>();
            if (ps != null)
            {
                player = ps.gameObject;
            }
        }
        else
        {
            ps = player.GetComponent<TapePlacementSystem>();
        }

        // ������ ��������
        sphereRenderer = GetComponent<Renderer>();

        // �ݶ��̴��� Ʈ���ŷ� ����
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ���Դ��� Ȯ��
        if (other.gameObject == player)
        {
            isPlayerInside = true;

            // TapePlacementSystem�� �� ����Ʈ ���
            if (ps != null)
            {
                // �̸����� ����
                if (gameObject.name == "StartPoint" || pointType == PointType.StartPoint)
                {
                    ps.OnEnterStartPoint(this);
                    Debug.Log($"StartPoint Ʈ���� ����: {gameObject.name}");
                }
                else if (gameObject.name == "EndPoint" || pointType == PointType.EndPoint)
                {
                    ps.OnEnterEndPoint(this);
                    Debug.Log($"EndPoint Ʈ���� ����: {gameObject.name}");
                }
                else // AnyPoint�� ���
                {
                    // ���� ���¿� ���� ������ �Ǵ� �������� ó��
                    if (!ps.IsPlacingTape())
                    {
                        ps.OnEnterStartPoint(this);
                        Debug.Log($"AnyPoint�� StartPoint�� ó��: {gameObject.name}");
                    }
                    else
                    {
                        ps.OnEnterEndPoint(this);
                        Debug.Log($"AnyPoint�� EndPoint�� ó��: {gameObject.name}");
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ �������� Ȯ��
        if (other.gameObject == player)
        {
            isPlayerInside = false;

            // TapePlacementSystem���� �� ����Ʈ ����
            if (ps != null)
            {
                if (gameObject.name == "StartPoint" || pointType == PointType.StartPoint)
                {
                    ps.OnExitStartPoint(this);
                }
                else if (gameObject.name == "EndPoint" || pointType == PointType.EndPoint)
                {
                    ps.OnExitEndPoint(this);
                }
                else
                {
                    // AnyPoint�� ��� �� �޼��� ��� ȣ�� (�ý����� �Ǵ�)
                    ps.OnExitStartPoint(this);
                    ps.OnExitEndPoint(this);
                }
            }

            Debug.Log($"�÷��̾ {gameObject.name}���� ����");
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}