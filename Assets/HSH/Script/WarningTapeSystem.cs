using System;
using System.Collections;
using UnityEngine;

public class WarningTapeSystem : MonoBehaviour
{
    [Header("테이프 설정")]
    public Material warningTapeMaterial;
    public float tapeHeight = 1.5f;
    public float tapeWidth = 0.05f;
    public Color tapeColor1 = Color.yellow;
    public Color tapeColor2 = Color.black;

    [Header("애니메이션")]
    public float scrollSpeed = 1f;
    public bool enableScrolling = true;

    [Header("구역 설정")]
    public Transform[] corners; // 테이프를 연결할 코너 포인트들
    public bool closeLoop = true; // 마지막과 첫번째 포인트를 연결할지

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
            Debug.LogError("최소 2개의 코너 포인트가 필요합니다!");
            return;
        }

        // 테이프 머티리얼 생성
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
        // 머티리얼 복사본 생성 (각 테이프가 독립적으로 애니메이션되도록)
        tapeMaterials[index] = new Material(warningTapeMaterial);
        line.material = tapeMaterials[index];

        // LineRenderer 설정
        line.startWidth = tapeWidth;
        line.endWidth = tapeWidth;
        line.positionCount = 2;
        line.useWorldSpace = true;

        // 포지션 설정
        Transform startCorner = corners[index];
        Transform endCorner = closeLoop && index == corners.Length - 1
            ? corners[0]
            : corners[index + 1];

        Vector3 startPos = startCorner.position + Vector3.up * tapeHeight;
        Vector3 endPos = endCorner.position + Vector3.up * tapeHeight;

        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);

        // 텍스처 스케일 조정 (거리에 따라)
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

    // 런타임에서 코너 포인트 업데이트
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

            // 텍스처 스케일 재조정
            float distance = Vector3.Distance(startPos, endPos);
            tapeMaterials[i].mainTextureScale = new Vector2(distance * 2f, 1f);
        }
    }

    void OnDestroy()
    {
        // 머티리얼 정리
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
        // 에디터에서 코너 포인트와 테이프 라인을 시각화
        if (corners != null && corners.Length > 1)
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < corners.Length; i++)
            {
                if (corners[i] != null)
                {
                    Vector3 pos = corners[i].position + Vector3.up * tapeHeight;
                    Gizmos.DrawWireSphere(pos, 0.1f);

                    // 다음 코너로의 라인 그리기
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