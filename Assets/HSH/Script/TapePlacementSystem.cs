using System.Collections.Generic;
using UnityEngine;

public class TapePlacementSystem : MonoBehaviour
{
    [Header("Tape Settings")]
    [SerializeField] private GameObject tapePrefab; // WarningTape 프리팹
    [SerializeField] private GameObject placeMarkerPrefab; // 설치 위치 표시용 마커 (옵션)
    [SerializeField] private float placementDistance = 5f; // 플레이어로부터 설치 거리
    [SerializeField] private float placementHeight = 1.5f; // 설치 높이

    [Header("Visual Feedback")]
    [SerializeField] private Color previewColor = new Color(1f, 1f, 0f, 0.5f); // 미리보기 색상
    [SerializeField] private Color validPlacementColor = Color.green;
    [SerializeField] private Color invalidPlacementColor = Color.red;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip removeSound;
    private AudioSource audioSource;

    // 설치 상태
    private enum PlacementState
    {
        Idle,           // 대기 상태
        PlacingEnd      // 끝점 설치 중
    }

    private PlacementState currentState = PlacementState.Idle;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private GameObject currentPreviewMarker;
    private LineRenderer previewLine;
    private List<GameObject> allTapes = new List<GameObject>();
    private GameObject startMarker;
    private GameObject endMarker;

    // 카메라 참조
    private Camera playerCamera;

    void Start()
    {
        // 카메라 찾기
        if (Camera.main != null)
        {
            playerCamera = Camera.main;
        }
        else
        {
            // TPSCameraController를 가진 카메라 찾기
            H_CamController tpsCamera = FindFirstObjectByType<H_CamController>(FindObjectsInactive.Exclude);
            if (tpsCamera != null)
            {
                playerCamera = tpsCamera.GetComponent<Camera>();
            }
        }

        // 오디오 소스 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (placeSound != null || removeSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 미리보기 라인 생성
        CreatePreviewLine();
    }

    void CreatePreviewLine()
    {
        GameObject previewObj = new GameObject("TapePreview");
        previewObj.transform.parent = transform;
        previewLine = previewObj.AddComponent<LineRenderer>();

        // 라인 렌더러 설정
        previewLine.startWidth = 0.05f;
        previewLine.endWidth = 0.05f;
        previewLine.material = new Material(Shader.Find("Sprites/Default"));
        previewLine.material.color = previewColor;
        previewLine.enabled = false;
        previewLine.positionCount = 2;
    }

    void Update()
    {
        HandleInput();
        UpdatePreview();
    }

    void HandleInput()
    {
        // E키: 테이프 설치
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleTapePlacement();
        }

        // R키: 모든 테이프 제거
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveAllTapes();
        }

        // ESC키: 설치 취소
        if (Input.GetKeyDown(KeyCode.Escape) && currentState != PlacementState.Idle)
        {
            CancelPlacement();
        }
    }

    void HandleTapePlacement()
    {
        Vector3 placementPos = GetPlacementPosition();

        switch (currentState)
        {
            case PlacementState.Idle:
                // 첫 번째 E키: 시작점 즉시 설정하고 끝점 대기 모드로
                StartPlacement(placementPos);
                break;

            case PlacementState.PlacingEnd:
                // 두 번째 E키: 끝점 설정하고 테이프 생성 후 완료
                CompletePlacement(placementPos);
                break;
        }
    }

    void StartPlacement(Vector3 position)
    {
        // 시작점 즉시 확정
        currentState = PlacementState.PlacingEnd;
        startPoint = position;

        // 시작점 마커 생성
        if (placeMarkerPrefab != null)
        {
            startMarker = Instantiate(placeMarkerPrefab, startPoint, Quaternion.identity);
            startMarker.name = "StartMarker";
        }
        else
        {
            startMarker = CreateDefaultMarker(startPoint, validPlacementColor);
        }

        // 미리보기 라인 활성화
        previewLine.enabled = true;

        // 사운드 재생
        PlaySound(placeSound);

        Debug.Log("테이프 시작점 설정됨");
    }

    void CompletePlacement(Vector3 position)
    {
        endPoint = position;

        // 끝점 마커 생성
        if (placeMarkerPrefab != null)
        {
            endMarker = Instantiate(placeMarkerPrefab, endPoint, Quaternion.identity);
            endMarker.name = "EndMarker";
        }
        else
        {
            endMarker = CreateDefaultMarker(endPoint, validPlacementColor);
        }

        // 테이프 생성
        CreateTape(startPoint, endPoint);

        // 상태 초기화 (설치 모드 종료)
        ResetPlacement();

        // 사운드 재생
        PlaySound(placeSound);

        Debug.Log("테이프 설치 완료!.");
    }

    void CreateTape(Vector3 start, Vector3 end)
    {
        GameObject tapeObj;

        if (tapePrefab != null)
        {
            tapeObj = Instantiate(tapePrefab);
        }
        else
        {
            // 프리팹이 없으면 기본 테이프 생성
            tapeObj = new GameObject("WarningTape");
            WarningTape tape = tapeObj.AddComponent<WarningTape>();

            // LineRenderer 추가 (WarningTape에 필요)
            LineRenderer lr = tapeObj.GetComponent<LineRenderer>();
            if (lr == null)
            {
                lr = tapeObj.AddComponent<LineRenderer>();
            }
        }

        // WarningTape 컴포넌트 설정
        WarningTape warningTape = tapeObj.GetComponent<WarningTape>();
        if (warningTape != null)
        {
            warningTape.SetPoints(start, end);
        }

        // 리스트에 추가
        allTapes.Add(tapeObj);

        // 마커도 리스트에 추가 (R키로 함께 제거하기 위해)
        if (startMarker != null)
        {
            allTapes.Add(startMarker);
            startMarker = null;
        }
        if (endMarker != null)
        {
            allTapes.Add(endMarker);
            endMarker = null;
        }
    }

    void RemoveAllTapes()
    {
        if (allTapes.Count == 0)
        {
            Debug.Log("제거할 테이프가 없습니다.");
            return;
        }

        // 모든 테이프와 마커 제거
        foreach (GameObject tape in allTapes)
        {
            if (tape != null)
            {
                Destroy(tape);
            }
        }

        allTapes.Clear();

        // 현재 설치 중인 것도 취소
        CancelPlacement();

        // 사운드 재생
        PlaySound(removeSound);

        Debug.Log("모든 테이프가 제거되었습니다.");
    }

    void CancelPlacement()
    {
        // 임시 마커 제거
        if (startMarker != null && !allTapes.Contains(startMarker))
        {
            Destroy(startMarker);
            startMarker = null;
        }
        if (endMarker != null && !allTapes.Contains(endMarker))
        {
            Destroy(endMarker);
            endMarker = null;
        }

        ResetPlacement();
        Debug.Log("테이프 설치가 취소되었습니다.");
    }

    void ResetPlacement()
    {
        currentState = PlacementState.Idle;
        previewLine.enabled = false;
        startMarker = null;
        endMarker = null;
    }

    void UpdatePreview()
    {
        if (currentState == PlacementState.Idle)
            return;

        Vector3 currentPos = GetPlacementPosition();

        // 미리보기 라인 업데이트
        if (currentState == PlacementState.PlacingEnd && previewLine != null)
        {
            previewLine.SetPosition(0, startPoint);
            previewLine.SetPosition(1, currentPos);
        }
    }

    Vector3 GetPlacementPosition()
    {
        Vector3 position = transform.position + transform.forward * placementDistance;
        position.y = placementHeight;

        // 카메라 레이캐스트로 더 정확한 위치 찾기 (옵션)
        if (playerCamera != null)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50f))
            {
                position = hit.point;
                position.y += 0.1f; // 바닥에서 약간 위로
            }
        }

        return position;
    }

    GameObject CreateDefaultMarker(Vector3 position, Color color)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.3f;

        Renderer renderer = marker.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            renderer.material.color = color;
        }

        // 콜라이더 제거 (충돌 방지)
        Collider collider = marker.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        return marker;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // 디버그용 GUI
    void OnGUI()
    {
        if (currentState != PlacementState.Idle)
        {
            GUI.Label(new Rect(Screen.width / 2 - 100, 50, 200, 30),
                $"테이프 설치 모드: {currentState}");
            GUI.Label(new Rect(Screen.width / 2 - 100, 80, 200, 30),
                "ESC: 취소 | E: 확정");
        }

        if (allTapes.Count > 0)
        {
            GUI.Label(new Rect(10, Screen.height - 30, 200, 30),
                $"설치된 테이프: {allTapes.Count / 3}개 (R키로 모두 제거)");
        }
    }

    // 기즈모
    void OnDrawGizmos()
    {
        if (currentState == PlacementState.PlacingEnd)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint, GetPlacementPosition());
            Gizmos.DrawWireSphere(startPoint, 0.3f);
            Gizmos.DrawWireSphere(GetPlacementPosition(), 0.3f);
        }
    }
}