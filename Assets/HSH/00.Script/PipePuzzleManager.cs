using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using static PipeTile;

public class PipePuzzleManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tilePrefab;
    public Transform gridContainer;
    public GridLayoutGroup gridLayout;

    [Header("Pipe Sprites")]
    public Sprite emptySprite;
    public Sprite straightSprite;
    public Sprite cornerSprite;
    public Sprite tShapeSprite;
    public Sprite crossSprite;
    public Sprite startSprite;
    public Sprite endSprite;

    [Header("Game Settings")]
    public int gridWidth = 7;
    public int gridHeight = 7;
    public Color normalColor = Color.white;
    public Color connectedColor = Color.green;
    public Color startColor = new Color(0.2f, 0.5f, 1f);
    public Color endColor = new Color(1f, 0.5f, 0.2f);

    [Header("UI Elements")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI timerText;
    public Button resetButton;
    public GameObject gamePanel;      // 게임 플레이 패널
    public GameObject successPanel;   // 성공 시 표시할 패널
    public TextMeshProUGUI successMessage;
    public Button nextLevelButton;    // Success Panel 내의 다음 레벨 버튼
    public Button retryButton;         // Success Panel 내의 재시도 버튼

    private PipeTile[,] grid;
    private Vector2Int startPos;
    private Vector2Int endPos;
    private int currentLevel = 1;
    private int moveCount = 0;
    private float gameTime = 0;
    private bool isPlaying = true;
    private List<Vector2Int> solutionPath;

    void Start()
    {
        // 필수 컴포넌트 체크
        if (!ValidateComponents())
        {
            Debug.LogError("필수 컴포넌트가 없습니다. Inspector를 확인해주세요!");
            return;
        }

        InitializeGrid();
        GenerateLevel();

        // 버튼 리스너 등록 (null 체크 포함)
        //if (checkButton != null) checkButton.onClick.AddListener(CheckSolution);
        //else Debug.LogWarning("Check Button이 할당되지 않았습니다.");

        if (resetButton != null) resetButton.onClick.AddListener(ResetLevel);
        else Debug.LogWarning("Reset Button이 할당되지 않았습니다.");

        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(NextLevel);
        else Debug.LogWarning("New Level Button이 할당되지 않았습니다.");
    }

    bool ValidateComponents()
    {
        bool isValid = true;

        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab이 할당되지 않았습니다!");
            isValid = false;
        }

        if (gridContainer == null)
        {
            Debug.LogError("Grid Container가 할당되지 않았습니다!");
            isValid = false;
        }

        if (gridLayout == null)
        {
            // GridLayout이 없으면 자동으로 찾기
            gridLayout = gridContainer?.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                Debug.LogError("Grid Layout Group을 찾을 수 없습니다!");
                isValid = false;
            }
        }

        // 스프라이트 체크
        if (straightSprite == null) Debug.LogWarning("Straight Sprite가 할당되지 않았습니다.");
        if (cornerSprite == null) Debug.LogWarning("Corner Sprite가 할당되지 않았습니다.");
        if (startSprite == null) Debug.LogWarning("Start Sprite가 할당되지 않았습니다.");
        if (endSprite == null) Debug.LogWarning("End Sprite가 할당되지 않았습니다.");

        return isValid;
    }

    void Update()
    {
        if (isPlaying)
        {
            gameTime += Time.deltaTime;
            UpdateTimer();
        }
    }

    void InitializeGrid()
    {
        grid = new PipeTile[gridHeight, gridWidth];

        // Grid Layout 설정
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridWidth;
        gridLayout.cellSize = new Vector2(80, 80);
        gridLayout.spacing = new Vector2(5, 5);
    }

    void GenerateLevel()
    {
        ClearGrid();

        if (tilePrefab == null || gridContainer == null)
        {
            Debug.LogError("필수 컴포넌트가 없습니다!");
            return;
        }

        Debug.Log($"레벨 생성 시작 - 크기: {gridWidth}x{gridHeight}");

        // 난이도에 따른 파이프 배치
        List<PipeType> availablePipes = new List<PipeType>();

        if (currentLevel <= 3)
        {
            availablePipes = new List<PipeType> { PipeType.Straight, PipeType.Corner };
        }
        else if (currentLevel <= 6)
        {
            availablePipes = new List<PipeType> { PipeType.Straight, PipeType.Corner, PipeType.TShape };
        }
        else
        {
            availablePipes = new List<PipeType> { PipeType.Straight, PipeType.Corner, PipeType.TShape, PipeType.Cross };
        }

        // 시작점과 끝점 설정
        startPos = new Vector2Int(0, Random.Range(0, gridHeight));
        endPos = new Vector2Int(gridWidth - 1, Random.Range(0, gridHeight));

        // 솔루션 경로 생성
        GenerateSolutionPath();

        // 그리드 생성
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject tileObj = Instantiate(tilePrefab, gridContainer);
                tileObj.name = $"Tile_{x}_{y}";

                PipeTileUI tileUI = tileObj.GetComponent<PipeTileUI>();
                if (tileUI == null)
                {
                    tileUI = tileObj.AddComponent<PipeTileUI>();
                }

                PipeType pipeType;
                int rotation = 0;

                if (x == startPos.x && y == startPos.y)
                {
                    pipeType = PipeType.Start;
                    rotation = 0;
                }
                else if (x == endPos.x && y == endPos.y)
                {
                    pipeType = PipeType.End;
                    rotation = 0;
                }
                else if (IsOnSolutionPath(new Vector2Int(x, y)))
                {
                    pipeType = GetPipeTypeForSolution(new Vector2Int(x, y));
                    rotation = Random.Range(0, 4) * 90;
                }
                else
                {
                    float randomValue = Random.Range(0f, 1f);
                    if (randomValue < 0.4f)
                    {
                        pipeType = PipeType.Empty;
                        rotation = 0;
                    }
                    else
                    {
                        pipeType = availablePipes[Random.Range(0, availablePipes.Count)];
                        rotation = Random.Range(0, 4) * 90;
                    }
                }

                grid[y, x] = new PipeTile(pipeType, rotation);
                grid[y, x].tileObject = tileObj;

                // 초기 회전값 설정
                tileObj.transform.rotation = Quaternion.Euler(0, 0, rotation);

                tileUI.Initialize(this, new Vector2Int(x, y), grid[y, x]);
                UpdateTileVisual(new Vector2Int(x, y));
            }
        }

        UpdateUI();
    }

    void GenerateSolutionPath()
    {
        solutionPath = FindPath(startPos, endPos);

        // 경로를 찾지 못한 경우 (절대 일어나서는 안됨)
        if (solutionPath == null || solutionPath.Count == 0)
        {
            Debug.LogError("경로를 생성할 수 없습니다!");
            // 직선 경로 강제 생성
            solutionPath = new List<Vector2Int>();
            solutionPath.Add(startPos);
            solutionPath.Add(endPos);
        }

        Debug.Log($"솔루션 경로 생성 완료: {solutionPath.Count}개 타일");
    }

    // A* 경로 찾기 알고리즘
    List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        Dictionary<Vector2Int, Vector2Int?> cameFrom = new Dictionary<Vector2Int, Vector2Int?>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();
        List<Vector2Int> openSet = new List<Vector2Int>();

        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Vector2Int.Distance(start, end);

        while (openSet.Count > 0)
        {
            // fScore가 가장 낮은 노드 찾기
            Vector2Int current = openSet[0];
            float lowestFScore = fScore[current];
            foreach (var node in openSet)
            {
                if (fScore.ContainsKey(node) && fScore[node] < lowestFScore)
                {
                    current = node;
                    lowestFScore = fScore[node];
                }
            }

            if (current == end)
            {
                // 경로 재구성
                List<Vector2Int> path = new List<Vector2Int>();
                Vector2Int? node = current;
                while (node.HasValue)
                {
                    path.Add(node.Value);
                    node = cameFrom.ContainsKey(node.Value) ? cameFrom[node.Value] : null;
                }
                path.Reverse();
                return path;
            }

            openSet.Remove(current);

            // 이웃 노드 확인
            Vector2Int[] neighbors = {
                current + Vector2Int.up,
                current + Vector2Int.down,
                current + Vector2Int.left,
                current + Vector2Int.right
            };

            foreach (var neighbor in neighbors)
            {
                // 그리드 범위 체크
                if (neighbor.x < 0 || neighbor.x >= gridWidth ||
                    neighbor.y < 0 || neighbor.y >= gridHeight)
                    continue;

                float tentativeGScore = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Vector2Int.Distance(neighbor, end);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // 경로를 찾지 못한 경우 직접 경로 생성
        return CreateDirectPath(start, end);
    }

    // 직접 경로 생성 (백업용)
    List<Vector2Int> CreateDirectPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;
        path.Add(current);

        // 먼저 x축으로 이동
        while (current.x != end.x)
        {
            current.x += (end.x > current.x) ? 1 : -1;
            path.Add(current);
        }

        // 그다음 y축으로 이동
        while (current.y != end.y)
        {
            current.y += (end.y > current.y) ? 1 : -1;
            path.Add(current);
        }

        return path;
    }

    bool IsOnSolutionPath(Vector2Int pos)
    {
        return solutionPath != null && solutionPath.Contains(pos);
    }

    PipeType GetPipeTypeForSolution(Vector2Int pos)
    {
        int index = solutionPath.IndexOf(pos);
        if (index == -1) return PipeType.Empty;

        Vector2Int? prev = index > 0 ? solutionPath[index - 1] : (Vector2Int?)null;
        Vector2Int? next = index < solutionPath.Count - 1 ? solutionPath[index + 1] : (Vector2Int?)null;

        // 연결 방향 계산
        bool connectUp = false, connectDown = false, connectLeft = false, connectRight = false;

        if (prev.HasValue)
        {
            Vector2Int diff = prev.Value - pos;
            if (diff == Vector2Int.up) connectUp = true;
            else if (diff == Vector2Int.down) connectDown = true;
            else if (diff == Vector2Int.left) connectLeft = true;
            else if (diff == Vector2Int.right) connectRight = true;
        }

        if (next.HasValue)
        {
            Vector2Int diff = next.Value - pos;
            if (diff == Vector2Int.up) connectUp = true;
            else if (diff == Vector2Int.down) connectDown = true;
            else if (diff == Vector2Int.left) connectLeft = true;
            else if (diff == Vector2Int.right) connectRight = true;
        }

        // 파이프 타입 결정
        if ((connectLeft && connectRight) || (connectUp && connectDown))
        {
            return PipeType.Straight;
        }
        else if ((connectUp && connectRight) || (connectUp && connectLeft) ||
                 (connectDown && connectRight) || (connectDown && connectLeft))
        {
            return PipeType.Corner;
        }

        // 예외 처리
        return PipeType.Straight;
    }

    // 솔루션 경로에 맞는 올바른 회전값 계산
    int GetCorrectRotationForSolution(Vector2Int pos)
    {
        int index = solutionPath.IndexOf(pos);
        if (index == -1) return Random.Range(0, 4) * 90;

        Vector2Int? prev = index > 0 ? solutionPath[index - 1] : (Vector2Int?)null;
        Vector2Int? next = index < solutionPath.Count - 1 ? solutionPath[index + 1] : (Vector2Int?)null;

        PipeType type = GetPipeTypeForSolution(pos);

        if (type == PipeType.Straight)
        {
            // 수평 또는 수직 결정
            if ((prev.HasValue && prev.Value.x != pos.x) ||
                (next.HasValue && next.Value.x != pos.x))
            {
                return 0; // 수평
            }
            else
            {
                return 90; // 수직
            }
        }
        else if (type == PipeType.Corner)
        {
            bool connectUp = false, connectDown = false, connectLeft = false, connectRight = false;

            if (prev.HasValue)
            {
                Vector2Int diff = prev.Value - pos;
                if (diff == Vector2Int.up) connectUp = true;
                else if (diff == Vector2Int.down) connectDown = true;
                else if (diff == Vector2Int.left) connectLeft = true;
                else if (diff == Vector2Int.right) connectRight = true;
            }

            if (next.HasValue)
            {
                Vector2Int diff = next.Value - pos;
                if (diff == Vector2Int.up) connectUp = true;
                else if (diff == Vector2Int.down) connectDown = true;
                else if (diff == Vector2Int.left) connectLeft = true;
                else if (diff == Vector2Int.right) connectRight = true;
            }

            // 코너 회전 결정
            if (connectUp && connectRight) return 0;    // ┗
            if (connectRight && connectDown) return 90;  // ┏
            if (connectDown && connectLeft) return 180;  // ┓
            if (connectLeft && connectUp) return 270;    // ┛
        }

        return Random.Range(0, 4) * 90;
    }

    public void RotateTile(Vector2Int pos)
    {
        if (grid[pos.y, pos.x].type == PipeType.Empty ||
            grid[pos.y, pos.x].type == PipeType.Start ||
            grid[pos.y, pos.x].type == PipeType.End)
            return;

        // 회전값 업데이트
        grid[pos.y, pos.x].Rotate();

        // 중요: 연결 정보 업데이트
        UpdatePipeTileConnections(grid[pos.y, pos.x]);

        moveCount++;
        UpdateUI();

        // 애니메이션 실행
        StartCoroutine(RotateAnimation(grid[pos.y, pos.x].tileObject.transform, pos));
    }

    // 수정된 RotateAnimation
    IEnumerator RotateAnimation(Transform tile, Vector2Int pos)
    {
        float duration = 0.2f;
        float elapsed = 0;

        // 현재 회전값과 목표 회전값
        float startRotation = tile.eulerAngles.z;
        float endRotation = grid[pos.y, pos.x].rotation;

        // 최단 경로로 회전하도록 조정
        float diff = Mathf.DeltaAngle(startRotation, endRotation);
        endRotation = startRotation + diff;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Smooth 보간
            t = t * t * (3f - 2f * t);

            float currentRotation = Mathf.Lerp(startRotation, endRotation, t);
            tile.rotation = Quaternion.Euler(0, 0, currentRotation);
            yield return null;
        }

        // 최종 회전값 설정
        tile.rotation = Quaternion.Euler(0, 0, grid[pos.y, pos.x].rotation);

        // 시각적 업데이트
        UpdateTileVisual(pos);

        // 솔루션 체크
        CheckSolution();
    }

    void UpdateTileVisual(Vector2Int pos)
    {
        PipeTile tile = grid[pos.y, pos.x];
        if (tile.tileObject == null) return;

        Image img = tile.tileObject.GetComponent<Image>();

        // 스프라이트 설정
        switch (tile.type)
        {
            case PipeType.Empty:
                img.sprite = emptySprite;
                break;
            case PipeType.Straight:
                img.sprite = straightSprite;
                break;
            case PipeType.Corner:
                img.sprite = cornerSprite;
                break;
            case PipeType.TShape:
                img.sprite = tShapeSprite;
                break;
            case PipeType.Cross:
                img.sprite = crossSprite;
                break;
            case PipeType.Start:
                img.sprite = startSprite;
                img.color = startColor;
                break;
            case PipeType.End:
                img.sprite = endSprite;
                img.color = endColor;
                break;
        }

        // 회전 적용 (Start와 End가 아닌 경우만)
        if (tile.type != PipeType.Start && tile.type != PipeType.End && tile.type != PipeType.Empty)
        {
            // 애니메이션 중이 아닐 때만 직접 회전 설정
            if (!IsAnimating(tile.tileObject))
            {
                tile.tileObject.transform.rotation = Quaternion.Euler(0, 0, tile.rotation);
            }
            img.color = tile.isConnected ? connectedColor : normalColor;
        }
    }

    // 애니메이션 중인지 확인하는 헬퍼 메서드
    private bool IsAnimating(GameObject tileObject)
    {
        // 태그나 컴포넌트로 애니메이션 상태를 체크
        // 간단한 방법: 모든 코루틴이 끝났는지 확인
        return false; // 실제로는 애니메이션 상태를 추적해야 함
    }
    void CheckSolution()
    {
        // 모든 타일의 연결 상태 초기화
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                grid[y, x].isConnected = false;
            }
        }

        // BFS로 경로 찾기
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);
        grid[startPos.y, startPos.x].isConnected = true;

        bool foundPath = false;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // 끝점에 도달했는지 확인
            if (current == endPos)
            {
                foundPath = true;
                // 계속 진행하여 모든 연결된 파이프를 표시
            }

            // 4방향 체크 (상, 우, 하, 좌)
            Vector2Int[] directions = {
            Vector2Int.up,    // 상 (0)
            Vector2Int.right, // 우 (1)
            Vector2Int.down,  // 하 (2)
            Vector2Int.left   // 좌 (3)
        };

            for (int i = 0; i < 4; i++)
            {
                Vector2Int next = current + directions[i];

                // 범위 체크
                if (next.x < 0 || next.x >= gridWidth || next.y < 0 || next.y >= gridHeight)
                    continue;

                // 이미 방문했거나 빈 타일이면 스킵
                if (visited.Contains(next) || grid[next.y, next.x].type == PipeType.Empty)
                    continue;

                // 연결 가능한지 체크 (양방향 연결 확인)
                if (CanConnect(current, next, i))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                    grid[next.y, next.x].isConnected = true;
                }
            }
        }

        // 모든 타일 시각적 업데이트
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                UpdateTileVisual(new Vector2Int(x, y));
            }
        }

        // 디버그 로그
        Debug.Log($"경로 탐색 완료 - 시작점: {startPos}, 끝점: {endPos}, 연결됨: {foundPath}");
        Debug.Log($"방문한 타일 수: {visited.Count}");

        if (foundPath)
        {
            OnLevelComplete();
        }
    }

    bool CanConnect(Vector2Int from, Vector2Int to, int direction)
    {
        PipeTile fromTile = grid[from.y, from.x];
        PipeTile toTile = grid[to.y, to.x];

        // 먼저 연결 배열이 초기화되었는지 확인
        if (fromTile.connections == null || toTile.connections == null)
        {
            Debug.LogError($"연결 배열이 null입니다! from: {from}, to: {to}");
            return false;
        }

        // 연결 배열이 올바른 크기인지 확인
        if (fromTile.connections.Length != 4 || toTile.connections.Length != 4)
        {
            Debug.LogError($"연결 배열 크기가 잘못되었습니다! from: {fromTile.connections.Length}, to: {toTile.connections.Length}");
            return false;
        }

        // direction 범위 확인
        if (direction < 0 || direction >= 4)
        {
            Debug.LogError($"잘못된 방향: {direction}");
            return false;
        }

        // from 타일에서 direction 방향으로 나갈 수 있는지 확인
        bool canExitFrom = fromTile.connections[direction];

        // to 타일에서 반대 방향으로 들어올 수 있는지 확인
        int oppositeDir = (direction + 2) % 4;
        bool canEnterTo = toTile.connections[oppositeDir];

        // 디버그 정보 (필요시 활성화)
        /*
        if (canExitFrom && canEnterTo)
        {
            Debug.Log($"연결 성공: {from} -> {to}, 방향: {direction}");
            Debug.Log($"  From tile ({fromTile.type}, rot: {fromTile.rotation}): {string.Join(",", fromTile.connections)}");
            Debug.Log($"  To tile ({toTile.type}, rot: {toTile.rotation}): {string.Join(",", toTile.connections)}");
        }
        */

        return canExitFrom && canEnterTo;
    }

    // PipeTile의 UpdateConnections 메서드도 수정이 필요합니다
    public void UpdatePipeTileConnections(PipeTile tile)
    {
        // 연결 배열 초기화
        if (tile.connections == null || tile.connections.Length != 4)
        {
            tile.connections = new bool[4];
        }

        // 모든 연결 초기화
        for (int i = 0; i < 4; i++)
            tile.connections[i] = false;

        // 타입별 기본 연결 설정 (회전 0도 기준)
        switch (tile.type)
        {
            case PipeType.Empty:
                // 연결 없음
                break;

            case PipeType.Straight:
                // 기본: 좌-우 연결
                if (tile.rotation % 180 == 0)
                {
                    // 0도 또는 180도: 좌-우 연결
                    tile.connections[1] = true; // 우
                    tile.connections[3] = true; // 좌
                }
                else
                {
                    // 90도 또는 270도: 상-하 연결
                    tile.connections[0] = true; // 상
                    tile.connections[2] = true; // 하
                }
                break;

            case PipeType.Corner:
                // 회전에 따른 연결 설정
                switch (tile.rotation)
                {
                    case 0:   // └ 모양 (상-우)
                        tile.connections[0] = true; // 상
                        tile.connections[1] = true; // 우
                        break;
                    case 90:  // ┌ 모양 (우-하)
                        tile.connections[1] = true; // 우
                        tile.connections[2] = true; // 하
                        break;
                    case 180: // ┐ 모양 (하-좌)
                        tile.connections[2] = true; // 하
                        tile.connections[3] = true; // 좌
                        break;
                    case 270: // ┘ 모양 (좌-상)
                        tile.connections[3] = true; // 좌
                        tile.connections[0] = true; // 상
                        break;
                }
                break;

            case PipeType.TShape:
                // T자 연결 (3방향)
                switch (tile.rotation)
                {
                    case 0:   // ┴ 모양 (상-좌-우)
                        tile.connections[0] = true; // 상
                        tile.connections[1] = true; // 우
                        tile.connections[3] = true; // 좌
                        break;
                    case 90:  // ├ 모양 (상-우-하)
                        tile.connections[0] = true; // 상
                        tile.connections[1] = true; // 우
                        tile.connections[2] = true; // 하
                        break;
                    case 180: // ┬ 모양 (하-좌-우)
                        tile.connections[1] = true; // 우
                        tile.connections[2] = true; // 하
                        tile.connections[3] = true; // 좌
                        break;
                    case 270: // ┤ 모양 (상-하-좌)
                        tile.connections[0] = true; // 상
                        tile.connections[2] = true; // 하
                        tile.connections[3] = true; // 좌
                        break;
                }
                break;

            case PipeType.Cross:
                // 십자 연결 (모든 방향)
                tile.connections[0] = true; // 상
                tile.connections[1] = true; // 우
                tile.connections[2] = true; // 하
                tile.connections[3] = true; // 좌
                break;

            case PipeType.Start:
                // 시작점 - 우측으로만 연결 (항상 좌측 가장자리에 있음)
                tile.connections[1] = true; // 우
                break;

            case PipeType.End:
                // 끝점 - 좌측으로만 연결 (항상 우측 가장자리에 있음)
                tile.connections[3] = true; // 좌
                break;
        }

        // 디버그 로그 (필요시 활성화)
        /*
        Debug.Log($"UpdateConnections - Type: {tile.type}, Rotation: {tile.rotation}, " +
                  $"Connections: [{string.Join(", ", tile.connections)}]");
        */
    }

    void OnLevelComplete()
    {
        isPlaying = false;
        ShowMessage($"레벨 {currentLevel} 완료!\n이동 횟수: {moveCount}\n시간: {FormatTime(gameTime)}", true);

        if (successPanel != null)
        {
            successPanel.SetActive(true);
            successMessage.text = $"축하합니다!\n레벨 {currentLevel} 클리어!";
        }
    }

    void ShowMessage(string message, bool isSuccess)
    {
        Debug.Log(message);
        // 여기에 UI 메시지 표시 로직 추가
    }

    void ResetLevel()
    {
        moveCount = 0;
        gameTime = 0;
        isPlaying = true;

        if (successPanel != null)
            successPanel.SetActive(false);

        // 모든 타일 초기 회전값으로 리셋
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[y, x].type != PipeType.Empty &&
                    grid[y, x].type != PipeType.Start &&
                    grid[y, x].type != PipeType.End)
                {
                    int newRotation = Random.Range(0, 4) * 90;
                    grid[y, x].rotation = newRotation;

                    // 중요: 연결 정보 업데이트
                    UpdatePipeTileConnections(grid[y, x]);

                    grid[y, x].isConnected = false;

                    // 즉시 회전 적용 (애니메이션 없이)
                    grid[y, x].tileObject.transform.rotation = Quaternion.Euler(0, 0, newRotation);
                    UpdateTileVisual(new Vector2Int(x, y));
                }
            }
        }

        UpdateUI();
    }

    void NextLevel()
    {
        currentLevel++;
        moveCount = 0;
        gameTime = 0;
        isPlaying = true;

        if (successPanel != null)
            successPanel.SetActive(false);

        GenerateLevel();
    }

    void ClearGrid()
    {
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateUI()
    {
        if (levelText != null)
            levelText.text = $"Level: {currentLevel}";

        if (movesText != null)
            movesText.text = $"Moves: {moveCount}";
    }

    void UpdateTimer()
    {
        if (timerText != null)
            timerText.text = $"Time: {FormatTime(gameTime)}";
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
