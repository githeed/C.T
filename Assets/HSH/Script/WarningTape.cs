using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WarningTape : MonoBehaviour
{
    [Header("Tape Settings")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float tapeWidth = 0.1f;
    [SerializeField] private float sagAmount = 0.2f; // 테이프가 처지는 정도
    [SerializeField] private int segmentCount = 20; // 곡선의 부드러움 정도

    [Header("Visual Settings")]
    [SerializeField] private Material tapeMaterial;
    [SerializeField] private Color primaryColor = Color.red;
    [SerializeField] private Color secondaryColor = Color.white;
    [SerializeField] private float patternRepeat = 10f; // 패턴 반복 횟수

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

        // Line Renderer 기본 설정
        lineRenderer.positionCount = segmentCount;
        lineRenderer.startWidth = tapeWidth;
        lineRenderer.endWidth = tapeWidth;
        lineRenderer.useWorldSpace = true;

        // 양면 렌더링을 위한 설정
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        lineRenderer.alignment = LineAlignment.View;
    }

    void CreateTapeMaterial()
    {
        if (tapeMaterial != null)
        {
            // 머티리얼 인스턴스 생성
            instanceMaterial = new Material(tapeMaterial);
            lineRenderer.material = instanceMaterial;
        }
        else
        {
            // 기본 머티리얼 생성
            instanceMaterial = new Material(Shader.Find("Sprites/Default"));
            instanceMaterial.color = primaryColor;
            lineRenderer.material = instanceMaterial;
        }

        // 텍스처 타일링 설정 (줄무늬 패턴을 위해)
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

        // 테이프의 처짐을 표현하기 위한 카테나리 곡선 또는 간단한 포물선 사용
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // 중력에 의한 처짐 효과 (포물선)
            float sag = Mathf.Sin(t * Mathf.PI) * sagAmount;
            point.y -= sag;

            lineRenderer.SetPosition(i, point);
        }
    }

    // 런타임에서 시작점과 끝점을 설정할 수 있는 메서드
    public void SetPoints(Transform newStartPoint, Transform newEndPoint)
    {
        startPoint = newStartPoint;
        endPoint = newEndPoint;
        UpdateTapePosition();
    }

    public void SetPoints(Vector3 startPosition, Vector3 endPosition)
    {
        // GameObject가 없을 경우 위치만으로 설정
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

    // 테이프 색상 변경
    public void SetColors(Color primary, Color secondary)
    {
        primaryColor = primary;
        secondaryColor = secondary;

        if (instanceMaterial != null)
        {
            instanceMaterial.color = primaryColor;
        }
    }

    // 테이프 너비 변경
    public void SetTapeWidth(float width)
    {
        tapeWidth = width;
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = tapeWidth;
            lineRenderer.endWidth = tapeWidth;
        }
    }

    // 처짐 정도 변경
    public void SetSagAmount(float sag)
    {
        sagAmount = sag;
        UpdateTapePosition();
    }

    // 기즈모 그리기 (에디터에서 시각화)
    void OnDrawGizmos()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint.position, endPoint.position);

            // 시작점과 끝점 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPoint.position, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(endPoint.position, 0.1f);
        }
    }
}

// 여러 개의 테이프를 관리하는 매니저 클래스
public class WarningTapeManager : MonoBehaviour
{
    [Header("Tape Prefab")]
    [SerializeField] private GameObject tapePrefab;

    [Header("Tape Chain Settings")]
    [SerializeField] private Transform[] tapePoints; // 연결할 포인트들
    [SerializeField] private bool createLoop = false; // 마지막 점과 첫 점을 연결할지 여부

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

        // 연속된 점들 사이에 테이프 생성
        for (int i = 0; i < tapePoints.Length - 1; i++)
        {
            CreateTapeBetweenPoints(tapePoints[i], tapePoints[i + 1]);
        }

        // 루프 생성 (선택사항)
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

    // 모든 테이프 제거
    public void ClearAllTapes()
    {
        foreach (var tape in tapeInstances)
        {
            if (tape != null)
                DestroyImmediate(tape);
        }
        tapeInstances.Clear();
    }

    // 새로운 포인트 추가
    public void AddPoint(Transform newPoint)
    {
        if (tapePoints != null && tapePoints.Length > 0)
        {
            Transform lastPoint = tapePoints[tapePoints.Length - 1];
            CreateTapeBetweenPoints(lastPoint, newPoint);

            // 배열 확장
            System.Array.Resize(ref tapePoints, tapePoints.Length + 1);
            tapePoints[tapePoints.Length - 1] = newPoint;
        }
    }
}