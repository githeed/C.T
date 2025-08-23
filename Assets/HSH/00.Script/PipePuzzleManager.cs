using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

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
    public GameObject gamePanel;      // ���� �÷��� �г�
    public GameObject successPanel;   // ���� �� ǥ���� �г�
    public TextMeshProUGUI successMessage;
    public Button nextLevelButton;    // Success Panel ���� ���� ���� ��ư
    public Button retryButton;         // Success Panel ���� ��õ� ��ư

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
        // �ʼ� ������Ʈ üũ
        if (!ValidateComponents())
        {
            Debug.LogError("�ʼ� ������Ʈ�� �����ϴ�. Inspector�� Ȯ�����ּ���!");
            return;
        }

        InitializeGrid();
        GenerateLevel();

        // ��ư ������ ��� (null üũ ����)
        //if (checkButton != null) checkButton.onClick.AddListener(CheckSolution);
        //else Debug.LogWarning("Check Button�� �Ҵ���� �ʾҽ��ϴ�.");

        if (resetButton != null) resetButton.onClick.AddListener(ResetLevel);
        else Debug.LogWarning("Reset Button�� �Ҵ���� �ʾҽ��ϴ�.");

        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(NextLevel);
        else Debug.LogWarning("New Level Button�� �Ҵ���� �ʾҽ��ϴ�.");
    }

    bool ValidateComponents()
    {
        bool isValid = true;

        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab�� �Ҵ���� �ʾҽ��ϴ�!");
            isValid = false;
        }

        if (gridContainer == null)
        {
            Debug.LogError("Grid Container�� �Ҵ���� �ʾҽ��ϴ�!");
            isValid = false;
        }

        if (gridLayout == null)
        {
            // GridLayout�� ������ �ڵ����� ã��
            gridLayout = gridContainer?.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                Debug.LogError("Grid Layout Group�� ã�� �� �����ϴ�!");
                isValid = false;
            }
        }

        // ��������Ʈ üũ
        if (straightSprite == null) Debug.LogWarning("Straight Sprite�� �Ҵ���� �ʾҽ��ϴ�.");
        if (cornerSprite == null) Debug.LogWarning("Corner Sprite�� �Ҵ���� �ʾҽ��ϴ�.");
        if (startSprite == null) Debug.LogWarning("Start Sprite�� �Ҵ���� �ʾҽ��ϴ�.");
        if (endSprite == null) Debug.LogWarning("End Sprite�� �Ҵ���� �ʾҽ��ϴ�.");

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

        // Grid Layout ����
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridWidth;
        gridLayout.cellSize = new Vector2(80, 80);
        gridLayout.spacing = new Vector2(5, 5);
    }

    void GenerateLevel()
    {
        ClearGrid();

        // Prefab üũ
        if (tilePrefab == null)
        {
            Debug.LogError("TilePrefab�� �Ҵ���� �ʾҽ��ϴ�! Inspector���� �Ҵ����ּ���.");
            return;
        }

        if (gridContainer == null)
        {
            Debug.LogError("GridContainer�� �Ҵ���� �ʾҽ��ϴ�! Inspector���� �Ҵ����ּ���.");
            return;
        }

        Debug.Log($"���� ���� ���� - ũ��: {gridWidth}x{gridHeight}");

        // ���̵��� ���� ������ ��ġ
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

        // �������� ���� ����
        startPos = new Vector2Int(0, Random.Range(0, gridHeight));
        endPos = new Vector2Int(gridWidth - 1, Random.Range(0, gridHeight));

        // �ַ�� ��� ����
        GenerateSolutionPath();

        // �׸��� ����
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // Prefab �ν��Ͻ� ����
                GameObject tileObj = Instantiate(tilePrefab, gridContainer);
                tileObj.name = $"Tile_{x}_{y}";

                // ������Ʈ Ȯ�� �� �߰�
                PipeTileUI tileUI = tileObj.GetComponent<PipeTileUI>();
                if (tileUI == null)
                {
                    tileUI = tileObj.AddComponent<PipeTileUI>();
                    Debug.Log($"PipeTileUI ������Ʈ �߰�: Tile_{x}_{y}");
                }

                PipeType pipeType;
                int rotation = Random.Range(0, 4) * 90;

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
                    // �ַ�� ��ο� �ִ� Ÿ�� - �ùٸ� ������ Ÿ�԰� ȸ���� ����
                    pipeType = GetPipeTypeForSolution(new Vector2Int(x, y));

                    // ���� ȸ������ ���� ���
                    int correctRotation = GetCorrectRotationForSolution(new Vector2Int(x, y));

                    // ������ ���� �����ϰ� ȸ�� (���߿� �÷��̾ ����� ��)
                    rotation = Random.Range(0, 4) * 90;

                    // ����׿�: ���� ȸ���� ���� (�ʿ�� ���)
                    // grid[y, x].correctRotation = correctRotation;
                }
                else
                {
                    // �ַ�� ��ΰ� �ƴ� Ÿ��
                    float randomValue = Random.Range(0f, 1f);

                    if (randomValue < 0.4f)
                    {
                        // �� Ÿ�� (40%)
                        pipeType = PipeType.Empty;
                    }
                    else
                    {
                        // ���� ������ (60%) - ȥ���� �ֱ� ���� ���� ������
                        pipeType = availablePipes[Random.Range(0, availablePipes.Count)];
                    }
                }

                grid[y, x] = new PipeTile(pipeType, rotation);
                grid[y, x].tileObject = tileObj;

                tileUI.Initialize(this, new Vector2Int(x, y), grid[y, x]);
                UpdateTileVisual(new Vector2Int(x, y));
            }
        }

        UpdateUI();
    }

    void GenerateSolutionPath()
    {
        solutionPath = FindPath(startPos, endPos);

        // ��θ� ã�� ���� ��� (���� �Ͼ���� �ȵ�)
        if (solutionPath == null || solutionPath.Count == 0)
        {
            Debug.LogError("��θ� ������ �� �����ϴ�!");
            // ���� ��� ���� ����
            solutionPath = new List<Vector2Int>();
            solutionPath.Add(startPos);
            solutionPath.Add(endPos);
        }

        Debug.Log($"�ַ�� ��� ���� �Ϸ�: {solutionPath.Count}�� Ÿ��");
    }

    // A* ��� ã�� �˰���
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
            // fScore�� ���� ���� ��� ã��
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
                // ��� �籸��
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

            // �̿� ��� Ȯ��
            Vector2Int[] neighbors = {
                current + Vector2Int.up,
                current + Vector2Int.down,
                current + Vector2Int.left,
                current + Vector2Int.right
            };

            foreach (var neighbor in neighbors)
            {
                // �׸��� ���� üũ
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

        // ��θ� ã�� ���� ��� ���� ��� ����
        return CreateDirectPath(start, end);
    }

    // ���� ��� ���� (�����)
    List<Vector2Int> CreateDirectPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;
        path.Add(current);

        // ���� x������ �̵�
        while (current.x != end.x)
        {
            current.x += (end.x > current.x) ? 1 : -1;
            path.Add(current);
        }

        // �״��� y������ �̵�
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

        // ���� ���� ���
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

        // ������ Ÿ�� ����
        if ((connectLeft && connectRight) || (connectUp && connectDown))
        {
            return PipeType.Straight;
        }
        else if ((connectUp && connectRight) || (connectUp && connectLeft) ||
                 (connectDown && connectRight) || (connectDown && connectLeft))
        {
            return PipeType.Corner;
        }

        // ���� ó��
        return PipeType.Straight;
    }

    // �ַ�� ��ο� �´� �ùٸ� ȸ���� ���
    int GetCorrectRotationForSolution(Vector2Int pos)
    {
        int index = solutionPath.IndexOf(pos);
        if (index == -1) return Random.Range(0, 4) * 90;

        Vector2Int? prev = index > 0 ? solutionPath[index - 1] : (Vector2Int?)null;
        Vector2Int? next = index < solutionPath.Count - 1 ? solutionPath[index + 1] : (Vector2Int?)null;

        PipeType type = GetPipeTypeForSolution(pos);

        if (type == PipeType.Straight)
        {
            // ���� �Ǵ� ���� ����
            if ((prev.HasValue && prev.Value.x != pos.x) ||
                (next.HasValue && next.Value.x != pos.x))
            {
                return 0; // ����
            }
            else
            {
                return 90; // ����
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

            // �ڳ� ȸ�� ����
            if (connectUp && connectRight) return 0;    // ��
            if (connectRight && connectDown) return 90;  // ��
            if (connectDown && connectLeft) return 180;  // ��
            if (connectLeft && connectUp) return 270;    // ��
        }

        return Random.Range(0, 4) * 90;
    }

    public void RotateTile(Vector2Int pos)
    {
        if (grid[pos.y, pos.x].type == PipeType.Empty ||
            grid[pos.y, pos.x].type == PipeType.Start ||
            grid[pos.y, pos.x].type == PipeType.End)
            return;

        grid[pos.y, pos.x].Rotate();
        UpdateTileVisual(pos);
        moveCount++;
        UpdateUI();

        // ȸ�� �ִϸ��̼�
        StartCoroutine(RotateAnimation(grid[pos.y, pos.x].tileObject.transform));

        
    }

    IEnumerator RotateAnimation(Transform tile)
    {
        float duration = 0.2f;
        float elapsed = 0;
        Quaternion startRot = tile.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, -90);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            tile.rotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            yield return null;
        }

        tile.rotation = endRot;

        CheckSolution();
    }

    void UpdateTileVisual(Vector2Int pos)
    {
        PipeTile tile = grid[pos.y, pos.x];
        if (tile.tileObject == null) return;

        Image img = tile.tileObject.GetComponent<Image>();

        // ��������Ʈ ����
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

        // ȸ�� ����
        if (tile.type != PipeType.Start && tile.type != PipeType.End)
        {
            tile.tileObject.transform.rotation = Quaternion.Euler(0, 0, -tile.rotation);
            img.color = tile.isConnected ? connectedColor : normalColor;
        }
    }

    void CheckSolution()
    {
        // ��� Ÿ���� ���� ���� �ʱ�ȭ
        //for (int y = 0; y < gridHeight; y++)
        //{
        //    for (int x = 0; x < gridWidth; x++)
        //    {
        //        grid[y, x].isConnected = false;
        //    }
        //}

        // BFS�� ��� ã��
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);
        grid[startPos.y, startPos.x].isConnected = true;

        bool foundPath = false;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == endPos)
            {
                foundPath = true;
            }

            // 4���� üũ
            Vector2Int[] directions = {
                new Vector2Int(0, 1),   // ��
                new Vector2Int(1, 0),   // ��
                new Vector2Int(0, -1),  // ��
                new Vector2Int(-1, 0)   // ��
            };

            for (int i = 0; i < 4; i++)
            {
                Vector2Int next = current + directions[i];

                // ���� üũ
                if (next.x < 0 || next.x >= gridWidth || next.y < 0 || next.y >= gridHeight)
                    continue;

                // �̹� �湮�߰ų� �� Ÿ���̸� ��ŵ
                if (visited.Contains(next) || grid[next.y, next.x].type == PipeType.Empty)
                    continue;

                // ���� �������� üũ
                if (CanConnect(current, next, i))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                    grid[next.y, next.x].isConnected = true;
                }
            }
        }

        // ��� Ÿ�� ������Ʈ
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                UpdateTileVisual(new Vector2Int(x, y));
            }
        }

        if (foundPath)
        {
            OnLevelComplete();
        }
        else
        {
            ShowMessage("��ΰ� ������� �ʾҽ��ϴ�!", false);
        }
    }

    bool CanConnect(Vector2Int from, Vector2Int to, int direction)
    {
        PipeTile fromTile = grid[from.y, from.x];
        PipeTile toTile = grid[to.y, to.x];

        // from Ÿ�Ͽ��� direction �������� ���� �� �ִ���
        if (!fromTile.connections[direction])
            return false;

        // to Ÿ�Ͽ��� �ݴ� �������� ���� �� �ִ���
        int oppositeDir = (direction + 2) % 4;
        return toTile.connections[oppositeDir];
    }

    void OnLevelComplete()
    {
        isPlaying = false;
        ShowMessage($"���� {currentLevel} �Ϸ�!\n�̵� Ƚ��: {moveCount}\n�ð�: {FormatTime(gameTime)}", true);

        if (successPanel != null)
        {
            successPanel.SetActive(true);
            successMessage.text = $"�����մϴ�!\n���� {currentLevel} Ŭ����!";
        }
    }

    void ShowMessage(string message, bool isSuccess)
    {
        Debug.Log(message);
        // ���⿡ UI �޽��� ǥ�� ���� �߰�
    }

    void ResetLevel()
    {
        moveCount = 0;
        gameTime = 0;
        isPlaying = true;

        // ��� Ÿ�� �ʱ� ȸ�������� ����
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[y, x].type != PipeType.Empty &&
                    grid[y, x].type != PipeType.Start &&
                    grid[y, x].type != PipeType.End)
                {
                    grid[y, x].rotation = Random.Range(0, 4) * 90;
                    grid[y, x].UpdateConnections();
                    grid[y, x].isConnected = false;
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
