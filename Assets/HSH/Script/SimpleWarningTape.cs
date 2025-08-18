using System.Collections;
using UnityEngine;

public class URPWarningTape : MonoBehaviour
{
    [Header("테이프 설정")]
    public Material tapeMaterial;
    public float tapeHeight = 1.5f;
    public float tapeWidth = 0.05f;

    [Header("패턴 설정")]
    public bool useStripedPattern = true;
    public float stripeWidth = 0.5f;
    public Color color1 = Color.yellow;
    public Color color2 = Color.black;

    [Header("애니메이션")]
    public bool animatePattern = true;
    public float animationSpeed = 1f;

    [Header("연결 포인트")]
    public Transform startPoint;
    public Transform endPoint;

    private GameObject tapeObject;
    private Material materialInstance;
    private MeshRenderer meshRenderer;

    void Start()
    {
        CreateTape();
        if (animatePattern)
            StartCoroutine(AnimateTape());
    }

    void CreateTape()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("시작점과 끝점을 설정해주세요!");
            return;
        }

        // 커스텀 메시로 테이프 생성
        tapeObject = new GameObject("WarningTape");
        tapeObject.transform.parent = transform;

        // 메시 컴포넌트 추가
        MeshFilter meshFilter = tapeObject.AddComponent<MeshFilter>();
        meshRenderer = tapeObject.AddComponent<MeshRenderer>();

        // 커스텀 메시 생성
        meshFilter.mesh = CreateTapeMesh();

        // 머티리얼 설정
        SetupMaterial();

        // 위치와 크기 조정
        PositionTape();
    }

    Mesh CreateTapeMesh()
    {
        Mesh mesh = new Mesh();

        // 테이프 형태의 사각형 메시 생성 (X축 방향으로 길게)
        Vector3[] vertices = new Vector3[8]; // 양면을 위해 8개 버텍스

        // 앞면 (Z = 0)
        vertices[0] = new Vector3(-0.5f, -0.5f, 0); // 왼쪽 아래
        vertices[1] = new Vector3(-0.5f, 0.5f, 0); // 왼쪽 위
        vertices[2] = new Vector3(0.5f, 0.5f, 0); // 오른쪽 위
        vertices[3] = new Vector3(0.5f, -0.5f, 0); // 오른쪽 아래

        // 뒷면 (Z = 0, 동일한 위치)
        vertices[4] = new Vector3(-0.5f, -0.5f, 0); // 왼쪽 아래
        vertices[5] = new Vector3(-0.5f, 0.5f, 0); // 왼쪽 위
        vertices[6] = new Vector3(0.5f, 0.5f, 0); // 오른쪽 위
        vertices[7] = new Vector3(0.5f, -0.5f, 0); // 오른쪽 아래

        Vector2[] uvs = new Vector2[8];
        // 앞면 UV
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(1, 0);
        // 뒷면 UV
        uvs[4] = new Vector2(0, 0);
        uvs[5] = new Vector2(0, 1);
        uvs[6] = new Vector2(1, 1);
        uvs[7] = new Vector2(1, 0);

        int[] triangles = new int[12];
        // 앞면 (시계방향)
        triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
        triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;
        // 뒷면 (반시계방향)
        triangles[6] = 6; triangles[7] = 5; triangles[8] = 4;
        triangles[9] = 7; triangles[10] = 6; triangles[11] = 4;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    void SetupMaterial()
    {
        if (tapeMaterial == null)
        {
            // URP Unlit 셰이더 사용 (성능 최적화)
            tapeMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            tapeMaterial.color = color1;
        }

        // 인스턴스 생성
        materialInstance = new Material(tapeMaterial);

        if (useStripedPattern)
        {
            CreateStripedTexture();
        }

        meshRenderer.material = materialInstance;
    }

    void CreateStripedTexture()
    {
        int textureWidth = 128;
        int textureHeight = 16;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        int stripePixelWidth = Mathf.Max(1, Mathf.RoundToInt(textureWidth * stripeWidth / 4f));

        for (int x = 0; x < textureWidth; x++)
        {
            bool isYellow = (x / stripePixelWidth) % 2 == 0;
            Color color = isYellow ? color1 : color2;

            for (int y = 0; y < textureHeight; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Point; // 선명한 줄무늬를 위해

        materialInstance.mainTexture = texture;
    }

    void PositionTape()
    {
        if (startPoint == null || endPoint == null) return;

        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        // 거리와 방향 계산
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        if (distance < 0.001f) return; // 너무 가까우면 무시

        // 중점에 위치 (바닥에서 tapeHeight만큼 위)
        Vector3 center = (startPos + endPos) * 0.5f;
        center.y += tapeHeight * 0.5f; // 테이프 중심이 바닥에서 높이만큼 올라가도록
        tapeObject.transform.position = center;

        // 회전 설정 - 두 점을 잇는 방향으로 회전
        Vector3 normalizedDirection = direction.normalized;

        // Y축을 위로 유지하면서 direction 방향으로 회전
        if (normalizedDirection.magnitude > 0.001f)
        {
            tapeObject.transform.rotation = Quaternion.LookRotation(normalizedDirection, Vector3.up);
        }

        // 스케일 조정 (X: 길이, Y: 높이, Z: 두께)
        tapeObject.transform.localScale = new Vector3(distance, tapeHeight, tapeWidth);

        // 텍스처 스케일 조정
        if (materialInstance != null && useStripedPattern)
        {
            float textureScale = distance / stripeWidth;
            materialInstance.mainTextureScale = new Vector2(textureScale, 1);
        }

        Debug.Log($"테이프 - 중심: {center}, 길이: {distance:F2}, 시작: {startPos}, 끝: {endPos}");
    }

    IEnumerator AnimateTape()
    {
        while (tapeObject != null && materialInstance != null)
        {
            float offset = Time.time * animationSpeed;
            materialInstance.mainTextureOffset = new Vector2(offset, 0);
            yield return null;
        }
    }

    void Update()
    {
        // 포인트가 움직이면 실시간 업데이트
        if (tapeObject != null && startPoint != null && endPoint != null)
        {
            if (startPoint.hasChanged || endPoint.hasChanged)
            {
                PositionTape();
                startPoint.hasChanged = false;
                endPoint.hasChanged = false;
            }
        }
    }

    // 에디터에서 실시간 미리보기
    void OnValidate()
    {
        if (Application.isPlaying && tapeObject != null)
        {
            PositionTape();
        }
    }

    void OnDestroy()
    {
        if (materialInstance != null)
        {
            DestroyImmediate(materialInstance);
        }
    }

    void OnDrawGizmos()
    {
        if (startPoint != null && endPoint != null)
        {
            Vector3 startPos = startPoint.position;
            Vector3 endPos = endPoint.position;

            // 테이프가 실제로 위치할 높이
            Vector3 tapeStartPos = startPos + Vector3.up * tapeHeight * 0.5f;
            Vector3 tapeEndPos = endPos + Vector3.up * tapeHeight * 0.5f;

            // 실제 포인트 위치 (빨간색)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPos, 0.1f);
            Gizmos.DrawWireSphere(endPos, 0.1f);

            // 테이프 위치 (노란색)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(tapeStartPos, 0.05f);
            Gizmos.DrawWireSphere(tapeEndPos, 0.05f);

            // 테이프 라인 (굵은 노란색)
            Gizmos.color = Color.yellow;
            Vector3 direction = (tapeEndPos - tapeStartPos).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized * tapeWidth * 0.5f;

            // 테이프 두께를 표현하는 라인들
            Gizmos.DrawLine(tapeStartPos + perpendicular, tapeEndPos + perpendicular);
            Gizmos.DrawLine(tapeStartPos - perpendicular, tapeEndPos - perpendicular);
            Gizmos.DrawLine(tapeStartPos + perpendicular, tapeStartPos - perpendicular);
            Gizmos.DrawLine(tapeEndPos + perpendicular, tapeEndPos - perpendicular);

            // 높이 연결선 (초록색)
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPos, tapeStartPos);
            Gizmos.DrawLine(endPos, tapeEndPos);

            // 중심점 표시 (파란색)
            Vector3 center = (startPos + endPos) * 0.5f + Vector3.up * tapeHeight * 0.5f;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, 0.05f);
        }
    }
}