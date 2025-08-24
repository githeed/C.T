using System.Collections.Generic;
using UnityEngine;

public class TapePlacementSystem : MonoBehaviour
{
    [Header("Tape Settings")]
    [SerializeField] private GameObject tapePrefab; // WarningTape 프리팹
    [SerializeField] private float tapeEndOffset = 1.5f; // 플레이어 앞 거리
    [SerializeField] private float tapeHeight = 1f; // 테이프 높이

    [Header("Visual Feedback")]
    [SerializeField] private Color placingTapeColor = new Color(1f, 1f, 0f, 0.7f); // 설치 중 테이프 색상
    [SerializeField] private Color completedTapeColor = Color.yellow; // 완료된 테이프 색상
    [SerializeField] private Color validPlacementColor = Color.green;
    [SerializeField] private Color invalidPlacementColor = Color.red;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip removeSound;
    [SerializeField] private AudioClip enterSound;
    private AudioSource audioSource;

    // 설치 상태
    private enum PlacementState
    {
        Idle,           // 대기 중
        PlacingTape     // 테이프 설치 중 (시작점에서 끝점 찾는 중)
    }

    private PlacementState currentState = PlacementState.Idle;
    private TapePlacePoint currentStartPoint; // 현재 시작점
    private TapePlacePoint currentEndPoint; // 현재 끝점
    private GameObject currentTape; // 현재 설치 중인 테이프
    private LineRenderer currentTapeRenderer; // 현재 테이프의 LineRenderer
    private List<GameObject> allTapes = new List<GameObject>();

    private bool canPlaceStart = false; // StartPoint에서 E키 사용 가능
    private bool canPlaceEnd = false; // EndPoint에서 E키 사용 가능

    void Start()
    {
        // 오디오 소스 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        HandleInput();
        UpdateTapeEndPosition();
    }

    void HandleInput()
    {
        // E키 처리
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentState == PlacementState.Idle && canPlaceStart && currentStartPoint != null)
            {
                // StartPoint에서 테이프 설치 시작
                StartTapePlacement();
            }
            else if (currentState == PlacementState.PlacingTape && canPlaceEnd && currentEndPoint != null)
            {
                // EndPoint에서 테이프 설치 완료
                CompleteTapePlacement();
            }
        }

        // R키: 모든 테이프 제거
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveAllTapes();
        }

        // ESC키: 설치 취소
        if (Input.GetKeyDown(KeyCode.Escape) && currentState == PlacementState.PlacingTape)
        {
            CancelPlacement();
        }
    }

    void StartTapePlacement()
    {
        currentState = PlacementState.PlacingTape;

        // 테이프 오브젝트 생성
        if (tapePrefab != null)
        {
            currentTape = Instantiate(tapePrefab);
        }
        else
        {
            // 기본 테이프 생성
            currentTape = new GameObject("WarningTape_Placing");
            currentTapeRenderer = currentTape.AddComponent<LineRenderer>();
            currentTapeRenderer.startWidth = 0.1f;
            currentTapeRenderer.endWidth = 0.1f;
            currentTapeRenderer.material = new Material(Shader.Find("Sprites/Default"));
            currentTapeRenderer.material.color = placingTapeColor;
            currentTapeRenderer.positionCount = 2;
        }

        // LineRenderer 가져오기
        if (currentTapeRenderer == null)
        {
            currentTapeRenderer = currentTape.GetComponent<LineRenderer>();
            if (currentTapeRenderer == null)
            {
                currentTapeRenderer = currentTape.AddComponent<LineRenderer>();
            }
        }

        // 테이프 색상을 설치 중 색상으로 설정
        if (currentTapeRenderer != null)
        {
            currentTapeRenderer.material.color = placingTapeColor;
        }

        // 시작점 설정
        Vector3 startPos = currentStartPoint.GetPosition();
        startPos.y = tapeHeight;

        // 초기 끝점 설정 (플레이어 앞)
        Vector3 endPos = transform.position + transform.forward * tapeEndOffset;
        endPos.y = tapeHeight;

        // LineRenderer 위치 설정
        currentTapeRenderer.SetPosition(0, startPos);
        currentTapeRenderer.SetPosition(1, endPos);

        // WarningTape 컴포넌트가 있으면 설정
        WarningTape warningTape = currentTape.GetComponent<WarningTape>();
        if (warningTape != null)
        {
            warningTape.SetPoints(startPos, endPos);
        }

        // 시작점 색상 변경
        currentStartPoint.SetSelected(true);

        PlaySound(placeSound);
        Debug.Log($"테이프 설치 시작: {currentStartPoint.name}에서");
    }

    void UpdateTapeEndPosition()
    {
        // 테이프 설치 중일 때만 업데이트
        if (currentState != PlacementState.PlacingTape || currentTapeRenderer == null) return;

        // 시작점 위치
        Vector3 startPos = currentStartPoint.GetPosition();
        startPos.y = tapeHeight;

        // 끝점 위치 계산
        Vector3 endPos;
        if (canPlaceEnd && currentEndPoint != null)
        {
            // EndPoint 안에 있으면 EndPoint 위치 사용
            endPos = currentEndPoint.GetPosition();
            endPos.y = tapeHeight;

            // 유효한 위치임을 표시
            currentTapeRenderer.material.color = validPlacementColor;
        }
        else
        {
            // 플레이어 앞 위치 사용
            endPos = transform.position + transform.forward * tapeEndOffset;
            endPos.y = tapeHeight;

            // 설치 중임을 표시
            currentTapeRenderer.material.color = placingTapeColor;
        }

        // LineRenderer 업데이트
        currentTapeRenderer.SetPosition(0, startPos);
        currentTapeRenderer.SetPosition(1, endPos);

        // WarningTape 컴포넌트 업데이트
        WarningTape warningTape = currentTape?.GetComponent<WarningTape>();
        if (warningTape != null)
        {
            warningTape.SetPoints(startPos, endPos);
        }
    }

    void CompleteTapePlacement()
    {
        if (currentTape == null || currentEndPoint == null) return;


        currentStartPoint = null;
        canPlaceStart = false;

        Debug.Log($"테이프 설치 완료 시작: currentEndPoint = {currentEndPoint.name}");

        // 테이프 최종 위치 설정
        Vector3 startPos = currentStartPoint.GetPosition();
        Vector3 endPos = currentEndPoint.GetPosition();
        

        // LineRenderer 최종 설정
        currentTapeRenderer.SetPosition(0, startPos);
        currentTapeRenderer.SetPosition(1, endPos);
        currentTapeRenderer.material.color = completedTapeColor;

        // WarningTape 컴포넌트 최종 설정
        WarningTape warningTape = currentTape.GetComponent<WarningTape>();
        if (warningTape != null)
        {
            warningTape.SetPoints(startPos, endPos);
        }

        // 테이프 이름 변경
        currentTape.name = $"WarningTape_{currentStartPoint.name}_to_{currentEndPoint.name}";

        // 완성된 테이프를 리스트에 추가
        allTapes.Add(currentTape);

        // 시작점 색상 복원
        currentStartPoint.SetHighlight(true);

        // 상태 초기화
        currentState = PlacementState.Idle;
        currentTape = null;
        currentTapeRenderer = null;

        PlaySound(placeSound);
        Debug.Log($"테이프 설치 완료: {currentStartPoint.name} → {currentEndPoint.name}");
    }

    void CancelPlacement()
    {
        if (currentTape != null)
        {
            Destroy(currentTape);
            currentTape = null;
            currentTapeRenderer = null;
        }

        if (currentStartPoint != null)
        {
            currentStartPoint.SetHighlight(true);
        }

        currentState = PlacementState.Idle;
        Debug.Log("테이프 설치가 취소되었습니다.");
    }

    void RemoveAllTapes()
    {
        if (allTapes.Count == 0)
        {
            Debug.Log("제거할 테이프가 없습니다.");
            return;
        }

        foreach (GameObject tape in allTapes)
        {
            if (tape != null) Destroy(tape);
        }

        allTapes.Clear();

        // 현재 설치 중인 테이프도 취소
        if (currentState == PlacementState.PlacingTape)
        {
            CancelPlacement();
        }

        PlaySound(removeSound);
        Debug.Log("모든 테이프가 제거되었습니다.");
    }

    // TapePlacePoint에서 호출하는 메서드들
    public void OnEnterStartPoint(TapePlacePoint point)
    {
        if (currentState == PlacementState.Idle)
        {
            currentStartPoint = point;
            canPlaceStart = true;
            PlaySound(enterSound);
        }
    }

    public void OnExitStartPoint(TapePlacePoint point)
    {
        if (currentStartPoint == point && currentState == PlacementState.Idle)
        {
            // Idle 상태일 때만 초기화 (테이프 설치 중에는 유지)
            currentStartPoint = null;
            canPlaceStart = false;
        }
    }

    public void OnEnterEndPoint(TapePlacePoint point)
    {
        // PlacingTape 상태이고 시작점과 다른 포인트일 때만
        if (currentState == PlacementState.PlacingTape && point != currentStartPoint)
        {
            currentEndPoint = point;
            canPlaceEnd = true;
            PlaySound(enterSound);
            Debug.Log($"EndPoint 진입: {point.name}, canPlaceEnd = {canPlaceEnd}");
        }
    }

    public void OnExitEndPoint(TapePlacePoint point)
    {
        if (currentEndPoint == point)
        {
            currentEndPoint = null;
            canPlaceEnd = false;
            Debug.Log($"EndPoint 나감: {point.name}");
        }
    }

    public bool IsPlacingTape()
    {
        return currentState == PlacementState.PlacingTape;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    //// GUI
    //void OnGUI()
    //{
    //    // 상태에 따른 안내 메시지
    //    if (currentState == PlacementState.Idle)
    //    {
    //        if (canPlaceStart && currentStartPoint != null)
    //        {
    //            GUI.Label(new Rect(Screen.width / 2 - 150, 30, 300, 30),
    //                $"<size=14><color=yellow>[E] 테이프 설치 시작 - {currentStartPoint.name}</color></size>");
    //        }
    //    }
    //    else if (currentState == PlacementState.PlacingTape)
    //    {
    //        GUI.Label(new Rect(Screen.width / 2 - 150, 30, 300, 30),
    //            $"<size=14><color=cyan>테이프 설치 중...</color></size>");

    //        if (canPlaceEnd && currentEndPoint != null)
    //        {
    //            GUI.Label(new Rect(Screen.width / 2 - 150, 60, 300, 30),
    //                $"<size=14><color=green>[E] 여기에 설치 - {currentEndPoint.name}</color></size>");
    //        }
    //        else
    //        {
    //            GUI.Label(new Rect(Screen.width / 2 - 150, 60, 300, 30),
    //                "<size=12><color=white>EndPoint로 이동하세요</color></size>");
    //        }

    //        GUI.Label(new Rect(Screen.width / 2 - 100, 90, 200, 30),
    //            "[ESC] 취소");
    //    }

    //    if (allTapes.Count > 0)
    //    {
    //        GUI.Label(new Rect(10, Screen.height - 30, 300, 30),
    //            $"테이프: {allTapes.Count}개 [R] 모두 제거");
    //    }
    //}

    //// 기즈모
    //void OnDrawGizmos()
    //{
    //    if (currentState == PlacementState.PlacingTape && currentStartPoint != null)
    //    {
    //        // 시작점
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(currentStartPoint.GetPosition(), 0.5f);

    //        // 현재 끝점
    //        Vector3 endPos = transform.position + transform.forward * tapeEndOffset;
    //        endPos.y = tapeHeight;

    //        if (canPlaceEnd && currentEndPoint != null)
    //        {
    //            endPos = currentEndPoint.GetPosition();
    //            Gizmos.color = Color.blue;
    //        }
    //        else
    //        {
    //            Gizmos.color = Color.yellow;
    //        }

    //        Gizmos.DrawWireSphere(endPos, 0.3f);
    //        Gizmos.DrawLine(currentStartPoint.GetPosition(), endPos);
    //    }
    //}
}