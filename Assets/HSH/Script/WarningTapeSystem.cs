using System;
using System.Collections;
using UnityEngine;

public class WarningTapeSystem : MonoBehaviour
{
    [Header("������ ����")]
    public Material warningTapeMaterial;
    public float tapeHeight = 1.5f;
    public float tapeWidth = 0.05f;
    public Color tapeColor1 = Color.yellow;
    public Color tapeColor2 = Color.black;

    [Header("�ִϸ��̼�")]
    public float scrollSpeed = 1f;
    public bool enableScrolling = true;

    [Header("���� ����")]
    public Transform[] corners; // �������� ������ �ڳ� ����Ʈ��
    public bool closeLoop = true; // �������� ù��° ����Ʈ�� ��������

    private LineRenderer[] tapeLines;
    private Material[] tapeMaterials;

    void Start()
    {
        CreateWarningTapes();
        if (enableScrolling)
            StartCoroutine(AnimateTapes());
    }

    void CreateWarningTapes()
    {
        if (corners == null || corners.Length < 2)
        {
            Debug.LogError("�ּ� 2���� �ڳ� ����Ʈ�� �ʿ��մϴ�!");
            return;
        }

        // ������ ��Ƽ���� ����
        CreateTapeMaterial();

        int lineCount = closeLoop ? corners.Length : corners.Length - 1;
        tapeLines = new LineRenderer[lineCount];
        tapeMaterials = new Material[lineCount];

        for (int i = 0; i < lineCount; i++)
        {
            GameObject tapeObj = new GameObject($"WarningTape_{i}");
            tapeObj.transform.parent = transform;

            LineRenderer line = tapeObj.AddComponent<LineRenderer>();
            SetupLineRenderer(line, i);

            tapeLines[i] = line;
        }
    }

    void CreateTapeMaterial()
    {
        if (warningTapeMaterial == null)
        {
            warningTapeMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            warningTapeMaterial.color = tapeColor1;
            warningTapeMaterial.enableInstancing = false;
        }
    }

    void SetupLineRenderer(LineRenderer line, int index)
    {
        // ��Ƽ���� ���纻 ���� (�� �������� ���������� �ִϸ��̼ǵǵ���)
        tapeMaterials[index] = new Material(warningTapeMaterial);
        line.material = tapeMaterials[index];

        // LineRenderer ����
        line.startWidth = tapeWidth;
        line.endWidth = tapeWidth;
        line.positionCount = 2;
        line.useWorldSpace = true;

        // ������ ����
        Transform startCorner = corners[index];
        Transform endCorner = closeLoop && index == corners.Length - 1
            ? corners[0]
            : corners[index + 1];

        Vector3 startPos = startCorner.position + Vector3.up * tapeHeight;
        Vector3 endPos = endCorner.position + Vector3.up * tapeHeight;

        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);

        // �ؽ�ó ������ ���� (�Ÿ��� ����)
        float distance = Vector3.Distance(startPos, endPos);
        tapeMaterials[index].mainTextureScale = new Vector2(distance * 2f, 1f);
    }

    IEnumerator AnimateTapes()
    {
        while (true)
        {
            float offset = Time.time * scrollSpeed;

            for (int i = 0; i < tapeMaterials.Length; i++)
            {
                if (tapeMaterials[i] != null)
                {
                    tapeMaterials[i].mainTextureOffset = new Vector2(offset, 0);
                }
            }

            yield return null;
        }
    }

    // ��Ÿ�ӿ��� �ڳ� ����Ʈ ������Ʈ
    public void UpdateTapePositions()
    {
        if (tapeLines == null) return;

        for (int i = 0; i < tapeLines.Length; i++)
        {
            Transform startCorner = corners[i];
            Transform endCorner = closeLoop && i == corners.Length - 1
            ? corners[0]
                : corners[i + 1];

            Vector3 startPos = startCorner.position + Vector3.up * tapeHeight;
            Vector3 endPos = endCorner.position + Vector3.up * tapeHeight;

            tapeLines[i].SetPosition(0, startPos);
            tapeLines[i].SetPosition(1, endPos);

            // �ؽ�ó ������ ������
            float distance = Vector3.Distance(startPos, endPos);
            tapeMaterials[i].mainTextureScale = new Vector2(distance * 2f, 1f);
        }
    }

    void OnDestroy()
    {
        // ��Ƽ���� ����
        if (tapeMaterials != null)
        {
            for (int i = 0; i < tapeMaterials.Length; i++)
            {
                if (tapeMaterials[i] != null)
                {
                    DestroyImmediate(tapeMaterials[i]);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // �����Ϳ��� �ڳ� ����Ʈ�� ������ ������ �ð�ȭ
        if (corners != null && corners.Length > 1)
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < corners.Length; i++)
            {
                if (corners[i] != null)
                {
                    Vector3 pos = corners[i].position + Vector3.up * tapeHeight;
                    Gizmos.DrawWireSphere(pos, 0.1f);

                    // ���� �ڳʷ��� ���� �׸���
                    int nextIndex = (closeLoop && i == corners.Length - 1) ? 0 : i + 1;
                    if (nextIndex < corners.Length && corners[nextIndex] != null)
                    {
                        Vector3 nextPos = corners[nextIndex].position + Vector3.up * tapeHeight;
                        Gizmos.DrawLine(pos, nextPos);
                    }
                }
            }
        }
    }
}