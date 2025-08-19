using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WarningTape : MonoBehaviour
{
    [Header("Tape Settings")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float tapeWidth = 0.1f;
    [SerializeField] private float sagAmount = 0.2f; // �������� ó���� ����
    [SerializeField] private int segmentCount = 20; // ��� �ε巯�� ����

    [Header("Visual Settings")]
    [SerializeField] private Material tapeMaterial;
    [SerializeField] private Color primaryColor = Color.red;
    [SerializeField] private Color secondaryColor = Color.white;
    [SerializeField] private float patternRepeat = 10f; // ���� �ݺ� Ƚ��

    private LineRenderer lineRenderer;
    private Material instanceMaterial;

    void Start()
    {
        SetupLineRenderer();
        CreateTapeMaterial();
    }

    void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Line Renderer �⺻ ����
        lineRenderer.positionCount = segmentCount;
        lineRenderer.startWidth = tapeWidth;
        lineRenderer.endWidth = tapeWidth;
        lineRenderer.useWorldSpace = true;

        // ��� �������� ���� ����
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        lineRenderer.alignment = LineAlignment.View;
    }

    void CreateTapeMaterial()
    {
        if (tapeMaterial != null)
        {
            // ��Ƽ���� �ν��Ͻ� ����
            instanceMaterial = new Material(tapeMaterial);
            lineRenderer.material = instanceMaterial;
        }
        else
        {
            // �⺻ ��Ƽ���� ����
            instanceMaterial = new Material(Shader.Find("Sprites/Default"));
            instanceMaterial.color = primaryColor;
            lineRenderer.material = instanceMaterial;
        }

        // �ؽ�ó Ÿ�ϸ� ���� (�ٹ��� ������ ����)
        instanceMaterial.mainTextureScale = new Vector2(patternRepeat, 1f);
    }

    void Update()
    {
        if (startPoint != null && endPoint != null)
        {
            UpdateTapePosition();
        }
    }

    void UpdateTapePosition()
    {
        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        // �������� ó���� ǥ���ϱ� ���� ī�׳��� � �Ǵ� ������ ������ ���
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // �߷¿� ���� ó�� ȿ�� (������)
            float sag = Mathf.Sin(t * Mathf.PI) * sagAmount;
            point.y -= sag;

            lineRenderer.SetPosition(i, point);
        }
    }

    // ��Ÿ�ӿ��� �������� ������ ������ �� �ִ� �޼���
    public void SetPoints(Transform newStartPoint, Transform newEndPoint)
    {
        startPoint = newStartPoint;
        endPoint = newEndPoint;
        UpdateTapePosition();
    }

    public void SetPoints(Vector3 startPosition, Vector3 endPosition)
    {
        // GameObject�� ���� ��� ��ġ������ ����
        if (startPoint == null)
        {
            GameObject startObj = new GameObject("TapeStart");
            startObj.transform.position = startPosition;
            startObj.transform.parent = transform;
            startPoint = startObj.transform;
        }
        else
        {
            startPoint.position = startPosition;
        }

        if (endPoint == null)
        {
            GameObject endObj = new GameObject("TapeEnd");
            endObj.transform.position = endPosition;
            endObj.transform.parent = transform;
            endPoint = endObj.transform;
        }
        else
        {
            endPoint.position = endPosition;
        }

        UpdateTapePosition();
    }

    // ������ ���� ����
    public void SetColors(Color primary, Color secondary)
    {
        primaryColor = primary;
        secondaryColor = secondary;

        if (instanceMaterial != null)
        {
            instanceMaterial.color = primaryColor;
        }
    }

    // ������ �ʺ� ����
    public void SetTapeWidth(float width)
    {
        tapeWidth = width;
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = tapeWidth;
            lineRenderer.endWidth = tapeWidth;
        }
    }

    // ó�� ���� ����
    public void SetSagAmount(float sag)
    {
        sagAmount = sag;
        UpdateTapePosition();
    }

    // ����� �׸��� (�����Ϳ��� �ð�ȭ)
    void OnDrawGizmos()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint.position, endPoint.position);

            // �������� ���� ǥ��
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPoint.position, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(endPoint.position, 0.1f);
        }
    }
}

// ���� ���� �������� �����ϴ� �Ŵ��� Ŭ����
public class WarningTapeManager : MonoBehaviour
{
    [Header("Tape Prefab")]
    [SerializeField] private GameObject tapePrefab;

    [Header("Tape Chain Settings")]
    [SerializeField] private Transform[] tapePoints; // ������ ����Ʈ��
    [SerializeField] private bool createLoop = false; // ������ ���� ù ���� �������� ����

    private List<GameObject> tapeInstances = new List<GameObject>();

    void Start()
    {
        CreateTapeChain();
    }

    void CreateTapeChain()
    {
        if (tapePoints == null || tapePoints.Length < 2)
        {
            Debug.LogWarning("At least 2 points are required to create tape chain!");
            return;
        }

        // ���ӵ� ���� ���̿� ������ ����
        for (int i = 0; i < tapePoints.Length - 1; i++)
        {
            CreateTapeBetweenPoints(tapePoints[i], tapePoints[i + 1]);
        }

        // ���� ���� (���û���)
        if (createLoop && tapePoints.Length > 2)
        {
            CreateTapeBetweenPoints(tapePoints[tapePoints.Length - 1], tapePoints[0]);
        }
    }

    void CreateTapeBetweenPoints(Transform start, Transform end)
    {
        GameObject tapeObj;

        if (tapePrefab != null)
        {
            tapeObj = Instantiate(tapePrefab, transform);
        }
        else
        {
            tapeObj = new GameObject("WarningTape");
            tapeObj.transform.parent = transform;
            tapeObj.AddComponent<WarningTape>();
        }

        WarningTape tape = tapeObj.GetComponent<WarningTape>();
        if (tape != null)
        {
            tape.SetPoints(start, end);
        }

        tapeInstances.Add(tapeObj);
    }

    // ��� ������ ����
    public void ClearAllTapes()
    {
        foreach (var tape in tapeInstances)
        {
            if (tape != null)
                DestroyImmediate(tape);
        }
        tapeInstances.Clear();
    }

    // ���ο� ����Ʈ �߰�
    public void AddPoint(Transform newPoint)
    {
        if (tapePoints != null && tapePoints.Length > 0)
        {
            Transform lastPoint = tapePoints[tapePoints.Length - 1];
            CreateTapeBetweenPoints(lastPoint, newPoint);

            // �迭 Ȯ��
            System.Array.Resize(ref tapePoints, tapePoints.Length + 1);
            tapePoints[tapePoints.Length - 1] = newPoint;
        }
    }
}