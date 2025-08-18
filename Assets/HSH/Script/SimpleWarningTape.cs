using System.Collections;
using UnityEngine;

public class URPWarningTape : MonoBehaviour
{
    [Header("������ ����")]
    public Material tapeMaterial;
    public float tapeHeight = 1.5f;
    public float tapeWidth = 0.05f;

    [Header("���� ����")]
    public bool useStripedPattern = true;
    public float stripeWidth = 0.5f;
    public Color color1 = Color.yellow;
    public Color color2 = Color.black;

    [Header("�ִϸ��̼�")]
    public bool animatePattern = true;
    public float animationSpeed = 1f;

    [Header("���� ����Ʈ")]
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
            Debug.LogError("�������� ������ �������ּ���!");
            return;
        }

        // Ŀ���� �޽÷� ������ ����
        tapeObject = new GameObject("WarningTape");
        tapeObject.transform.parent = transform;

        // �޽� ������Ʈ �߰�
        MeshFilter meshFilter = tapeObject.AddComponent<MeshFilter>();
        meshRenderer = tapeObject.AddComponent<MeshRenderer>();

        // Ŀ���� �޽� ����
        meshFilter.mesh = CreateTapeMesh();

        // ��Ƽ���� ����
        SetupMaterial();

        // ��ġ�� ũ�� ����
        PositionTape();
    }

    Mesh CreateTapeMesh()
    {
        Mesh mesh = new Mesh();

        // ������ ������ �簢�� �޽� ���� (X�� �������� ���)
        Vector3[] vertices = new Vector3[8]; // ����� ���� 8�� ���ؽ�

        // �ո� (Z = 0)
        vertices[0] = new Vector3(-0.5f, -0.5f, 0); // ���� �Ʒ�
        vertices[1] = new Vector3(-0.5f, 0.5f, 0); // ���� ��
        vertices[2] = new Vector3(0.5f, 0.5f, 0); // ������ ��
        vertices[3] = new Vector3(0.5f, -0.5f, 0); // ������ �Ʒ�

        // �޸� (Z = 0, ������ ��ġ)
        vertices[4] = new Vector3(-0.5f, -0.5f, 0); // ���� �Ʒ�
        vertices[5] = new Vector3(-0.5f, 0.5f, 0); // ���� ��
        vertices[6] = new Vector3(0.5f, 0.5f, 0); // ������ ��
        vertices[7] = new Vector3(0.5f, -0.5f, 0); // ������ �Ʒ�

        Vector2[] uvs = new Vector2[8];
        // �ո� UV
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(1, 0);
        // �޸� UV
        uvs[4] = new Vector2(0, 0);
        uvs[5] = new Vector2(0, 1);
        uvs[6] = new Vector2(1, 1);
        uvs[7] = new Vector2(1, 0);

        int[] triangles = new int[12];
        // �ո� (�ð����)
        triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
        triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;
        // �޸� (�ݽð����)
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
            // URP Unlit ���̴� ��� (���� ����ȭ)
            tapeMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            tapeMaterial.color = color1;
        }

        // �ν��Ͻ� ����
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
        texture.filterMode = FilterMode.Point; // ������ �ٹ��̸� ����

        materialInstance.mainTexture = texture;
    }

    void PositionTape()
    {
        if (startPoint == null || endPoint == null) return;

        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        // �Ÿ��� ���� ���
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        if (distance < 0.001f) return; // �ʹ� ������ ����

        // ������ ��ġ (�ٴڿ��� tapeHeight��ŭ ��)
        Vector3 center = (startPos + endPos) * 0.5f;
        center.y += tapeHeight * 0.5f; // ������ �߽��� �ٴڿ��� ���̸�ŭ �ö󰡵���
        tapeObject.transform.position = center;

        // ȸ�� ���� - �� ���� �մ� �������� ȸ��
        Vector3 normalizedDirection = direction.normalized;

        // Y���� ���� �����ϸ鼭 direction �������� ȸ��
        if (normalizedDirection.magnitude > 0.001f)
        {
            tapeObject.transform.rotation = Quaternion.LookRotation(normalizedDirection, Vector3.up);
        }

        // ������ ���� (X: ����, Y: ����, Z: �β�)
        tapeObject.transform.localScale = new Vector3(distance, tapeHeight, tapeWidth);

        // �ؽ�ó ������ ����
        if (materialInstance != null && useStripedPattern)
        {
            float textureScale = distance / stripeWidth;
            materialInstance.mainTextureScale = new Vector2(textureScale, 1);
        }

        Debug.Log($"������ - �߽�: {center}, ����: {distance:F2}, ����: {startPos}, ��: {endPos}");
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
        // ����Ʈ�� �����̸� �ǽð� ������Ʈ
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

    // �����Ϳ��� �ǽð� �̸�����
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

            // �������� ������ ��ġ�� ����
            Vector3 tapeStartPos = startPos + Vector3.up * tapeHeight * 0.5f;
            Vector3 tapeEndPos = endPos + Vector3.up * tapeHeight * 0.5f;

            // ���� ����Ʈ ��ġ (������)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPos, 0.1f);
            Gizmos.DrawWireSphere(endPos, 0.1f);

            // ������ ��ġ (�����)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(tapeStartPos, 0.05f);
            Gizmos.DrawWireSphere(tapeEndPos, 0.05f);

            // ������ ���� (���� �����)
            Gizmos.color = Color.yellow;
            Vector3 direction = (tapeEndPos - tapeStartPos).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized * tapeWidth * 0.5f;

            // ������ �β��� ǥ���ϴ� ���ε�
            Gizmos.DrawLine(tapeStartPos + perpendicular, tapeEndPos + perpendicular);
            Gizmos.DrawLine(tapeStartPos - perpendicular, tapeEndPos - perpendicular);
            Gizmos.DrawLine(tapeStartPos + perpendicular, tapeStartPos - perpendicular);
            Gizmos.DrawLine(tapeEndPos + perpendicular, tapeEndPos - perpendicular);

            // ���� ���ἱ (�ʷϻ�)
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPos, tapeStartPos);
            Gizmos.DrawLine(endPos, tapeEndPos);

            // �߽��� ǥ�� (�Ķ���)
            Vector3 center = (startPos + endPos) * 0.5f + Vector3.up * tapeHeight * 0.5f;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, 0.05f);
        }
    }
}