using System.Collections.Generic;
using UnityEngine;

public class TapePlacementSystem : MonoBehaviour
{
    [Header("Tape Settings")]
    [SerializeField] private GameObject tapePrefab; // WarningTape ������
    [SerializeField] private float tapeEndOffset = 1.5f; // �÷��̾� �� �Ÿ�
    [SerializeField] private float tapeHeight = 1f; // ������ ����

    [Header("Visual Feedback")]
    [SerializeField] private Color placingTapeColor = new Color(1f, 1f, 0f, 0.7f); // ��ġ �� ������ ����
    [SerializeField] private Color completedTapeColor = Color.yellow; // �Ϸ�� ������ ����
    [SerializeField] private Color validPlacementColor = Color.green;
    [SerializeField] private Color invalidPlacementColor = Color.red;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip removeSound;
    [SerializeField] private AudioClip enterSound;
    private AudioSource audioSource;

    // ��ġ ����
    private enum PlacementState
    {
        Idle,           // ��� ��
        PlacingTape     // ������ ��ġ �� (���������� ���� ã�� ��)
    }

    private PlacementState currentState = PlacementState.Idle;
    private TapePlacePoint currentStartPoint; // ���� ������
    private TapePlacePoint currentEndPoint; // ���� ����
    private GameObject currentTape; // ���� ��ġ ���� ������
    private LineRenderer currentTapeRenderer; // ���� �������� LineRenderer
    private List<GameObject> allTapes = new List<GameObject>();

    // ������ ���� ���� (StartPoint�� ������ �����ϱ� ����)
    private string startPointName;
    private Vector3 startPointPosition;

    private bool canPlaceStart = false; // StartPoint���� EŰ ��� ����
    private bool canPlaceEnd = false; // EndPoint���� EŰ ��� ����

    void Start()
    {
        // ����� �ҽ� ����
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
        // EŰ ó��
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"EŰ �Է� - State: {currentState}, canPlaceStart: {canPlaceStart}, canPlaceEnd: {canPlaceEnd}");

            if (currentState == PlacementState.Idle && canPlaceStart && currentStartPoint != null)
            {
                // StartPoint���� ������ ��ġ ����
                StartTapePlacement();
            }
            else if (currentState == PlacementState.PlacingTape && canPlaceEnd && currentEndPoint != null)
            {
                // EndPoint���� ������ ��ġ �Ϸ�
                Debug.Log("EndPoint���� ��ġ �Ϸ� �õ�");
                CompleteTapePlacement();
            }
        }

        // RŰ: ��� ������ ����
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveAllTapes();
        }

        // ESCŰ: ��ġ ���
        if (Input.GetKeyDown(KeyCode.Escape) && currentState == PlacementState.PlacingTape)
        {
            CancelPlacement();
        }
    }

    void StartTapePlacement()
    {
        currentState = PlacementState.PlacingTape;

        // ������ �̸� ���� (���߿� ����ϱ� ����)
        startPointName = currentStartPoint.name;
        startPointPosition = currentStartPoint.GetPosition();

        // ������ ������Ʈ ����
        if (tapePrefab != null)
        {
            currentTape = Instantiate(tapePrefab);
        }
        else
        {
            // �⺻ ������ ����
            currentTape = new GameObject("WarningTape_Placing");
            currentTapeRenderer = currentTape.AddComponent<LineRenderer>();
            currentTapeRenderer.startWidth = 0.1f;
            currentTapeRenderer.endWidth = 0.1f;
            currentTapeRenderer.material = new Material(Shader.Find("Sprites/Default"));
            currentTapeRenderer.material.color = placingTapeColor;
            currentTapeRenderer.positionCount = 2;
        }

        // LineRenderer ��������
        if (currentTapeRenderer == null)
        {
            currentTapeRenderer = currentTape.GetComponent<LineRenderer>();
            if (currentTapeRenderer == null)
            {
                currentTapeRenderer = currentTape.AddComponent<LineRenderer>();
            }
        }

        // ������ ������ ��ġ �� �������� ����
        if (currentTapeRenderer != null)
        {
            currentTapeRenderer.material.color = placingTapeColor;
        }

        // ������ ����
        Vector3 startPos = startPointPosition;
        startPos.y = tapeHeight;

        // �ʱ� ���� ���� (�÷��̾� ��)
        Vector3 endPos = transform.position + transform.forward * tapeEndOffset;
        endPos.y = tapeHeight;

        // LineRenderer ��ġ ����
        currentTapeRenderer.SetPosition(0, startPos);
        currentTapeRenderer.SetPosition(1, endPos);

        // WarningTape ������Ʈ�� ������ ����
        WarningTape warningTape = currentTape.GetComponent<WarningTape>();
        if (warningTape != null)
        {
            warningTape.SetPoints(startPos, endPos);
        }

        PlaySound(placeSound);
        Debug.Log($"������ ��ġ ����: {startPointName}����");
    }

    void UpdateTapeEndPosition()
    {
        // ������ ��ġ ���� ���� ������Ʈ
        if (currentState != PlacementState.PlacingTape || currentTapeRenderer == null) return;

        // ������ ��ġ (����� ��ġ ���)
        Vector3 startPos = startPointPosition;
        startPos.y = tapeHeight;

        // ���� ��ġ ���
        Vector3 endPos;
        if (canPlaceEnd && currentEndPoint != null)
        {
            // EndPoint �ȿ� ������ EndPoint ��ġ ���
            endPos = currentEndPoint.GetPosition();
            endPos.y = tapeHeight;

            // ��ȿ�� ��ġ���� ǥ��
            currentTapeRenderer.material.color = validPlacementColor;
        }
        else
        {
            // �÷��̾� �� ��ġ ���
            endPos = transform.position + transform.forward * tapeEndOffset;
            endPos.y = tapeHeight;

            // ��ġ ������ ǥ��
            currentTapeRenderer.material.color = placingTapeColor;
        }

        // LineRenderer ������Ʈ
        currentTapeRenderer.SetPosition(0, startPos);
        currentTapeRenderer.SetPosition(1, endPos);

        // WarningTape ������Ʈ ������Ʈ
        WarningTape warningTape = currentTape?.GetComponent<WarningTape>();
        if (warningTape != null)
        {
            warningTape.SetPoints(startPos, endPos);
        }
    }

    void CompleteTapePlacement()
    {
        if (currentTape == null || currentEndPoint == null) return;

        Debug.Log($"������ ��ġ �Ϸ� ����: currentEndPoint = {currentEndPoint.name}");

        // ������ ���� ��ġ ����
        Vector3 startPos = startPointPosition;
        startPos.y = tapeHeight;
        Vector3 endPos = currentEndPoint.GetPosition();
        endPos.y = tapeHeight;

        // LineRenderer ���� ����
        currentTapeRenderer.SetPosition(0, startPos);
        currentTapeRenderer.SetPosition(1, endPos);
        currentTapeRenderer.material.color = completedTapeColor;

        // WarningTape ������Ʈ ���� ����
        WarningTape warningTape = currentTape.GetComponent<WarningTape>();
        if (warningTape != null)
        {
            warningTape.SetPoints(startPos, endPos);
        }

        // ������ �̸� ����
        currentTape.name = $"WarningTape_{startPointName}_to_{currentEndPoint.name}";

        // �ϼ��� �������� ����Ʈ�� �߰�
        allTapes.Add(currentTape);

        // ���� �ʱ�ȭ
        currentState = PlacementState.Idle;
        currentTape = null;
        currentTapeRenderer = null;
        currentStartPoint = null;
        canPlaceStart = false;

        PlaySound(placeSound);
        Debug.Log($"������ ��ġ �Ϸ�: {startPointName} �� {currentEndPoint.name}");

        GameManager.Instance.status = GameStatus.PipeMission;
        GameManager.Instance.SetCompleteUI();

    }

    void CancelPlacement()
    {
        if (currentTape != null)
        {
            Destroy(currentTape);
            currentTape = null;
            currentTapeRenderer = null;
        }
        currentState = PlacementState.Idle;
        Debug.Log("������ ��ġ�� ��ҵǾ����ϴ�.");
    }

    void RemoveAllTapes()
    {
        if (allTapes.Count == 0)
        {
            Debug.Log("������ �������� �����ϴ�.");
            return;
        }

        foreach (GameObject tape in allTapes)
        {
            if (tape != null) Destroy(tape);
        }

        allTapes.Clear();

        // ���� ��ġ ���� �������� ���
        if (currentState == PlacementState.PlacingTape)
        {
            CancelPlacement();
        }

        PlaySound(removeSound);
        Debug.Log("��� �������� ���ŵǾ����ϴ�.");
    }

    // TapePlacePoint���� ȣ���ϴ� �޼����
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
            // Idle ������ ���� �ʱ�ȭ (������ ��ġ �߿��� ����)
            currentStartPoint = null;
            canPlaceStart = false;
        }
    }

    public void OnEnterEndPoint(TapePlacePoint point)
    {
        // PlacingTape �����̰� �������� �ٸ� ����Ʈ�� ����
        if (currentState == PlacementState.PlacingTape && point != currentStartPoint)
        {
            currentEndPoint = point;
            canPlaceEnd = true;
            PlaySound(enterSound);
            Debug.Log($"EndPoint ����: {point.name}, canPlaceEnd = {canPlaceEnd}");
        }
    }

    public void OnExitEndPoint(TapePlacePoint point)
    {
        if (currentEndPoint == point)
        {
            currentEndPoint = null;
            canPlaceEnd = false;
            Debug.Log($"EndPoint ����: {point.name}");
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
}
