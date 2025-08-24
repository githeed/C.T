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
            if (currentState == PlacementState.Idle && canPlaceStart && currentStartPoint != null)
            {
                // StartPoint���� ������ ��ġ ����
                StartTapePlacement();
            }
            else if (currentState == PlacementState.PlacingTape && canPlaceEnd && currentEndPoint != null)
            {
                // EndPoint���� ������ ��ġ �Ϸ�
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
        Vector3 startPos = currentStartPoint.GetPosition();
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

        // ������ ���� ����
        currentStartPoint.SetSelected(true);

        PlaySound(placeSound);
        Debug.Log($"������ ��ġ ����: {currentStartPoint.name}����");
    }

    void UpdateTapeEndPosition()
    {
        // ������ ��ġ ���� ���� ������Ʈ
        if (currentState != PlacementState.PlacingTape || currentTapeRenderer == null) return;

        // ������ ��ġ
        Vector3 startPos = currentStartPoint.GetPosition();
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


        currentStartPoint = null;
        canPlaceStart = false;

        Debug.Log($"������ ��ġ �Ϸ� ����: currentEndPoint = {currentEndPoint.name}");

        // ������ ���� ��ġ ����
        Vector3 startPos = currentStartPoint.GetPosition();
        Vector3 endPos = currentEndPoint.GetPosition();
        

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
        currentTape.name = $"WarningTape_{currentStartPoint.name}_to_{currentEndPoint.name}";

        // �ϼ��� �������� ����Ʈ�� �߰�
        allTapes.Add(currentTape);

        // ������ ���� ����
        currentStartPoint.SetHighlight(true);

        // ���� �ʱ�ȭ
        currentState = PlacementState.Idle;
        currentTape = null;
        currentTapeRenderer = null;

        PlaySound(placeSound);
        Debug.Log($"������ ��ġ �Ϸ�: {currentStartPoint.name} �� {currentEndPoint.name}");
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

    //// GUI
    //void OnGUI()
    //{
    //    // ���¿� ���� �ȳ� �޽���
    //    if (currentState == PlacementState.Idle)
    //    {
    //        if (canPlaceStart && currentStartPoint != null)
    //        {
    //            GUI.Label(new Rect(Screen.width / 2 - 150, 30, 300, 30),
    //                $"<size=14><color=yellow>[E] ������ ��ġ ���� - {currentStartPoint.name}</color></size>");
    //        }
    //    }
    //    else if (currentState == PlacementState.PlacingTape)
    //    {
    //        GUI.Label(new Rect(Screen.width / 2 - 150, 30, 300, 30),
    //            $"<size=14><color=cyan>������ ��ġ ��...</color></size>");

    //        if (canPlaceEnd && currentEndPoint != null)
    //        {
    //            GUI.Label(new Rect(Screen.width / 2 - 150, 60, 300, 30),
    //                $"<size=14><color=green>[E] ���⿡ ��ġ - {currentEndPoint.name}</color></size>");
    //        }
    //        else
    //        {
    //            GUI.Label(new Rect(Screen.width / 2 - 150, 60, 300, 30),
    //                "<size=12><color=white>EndPoint�� �̵��ϼ���</color></size>");
    //        }

    //        GUI.Label(new Rect(Screen.width / 2 - 100, 90, 200, 30),
    //            "[ESC] ���");
    //    }

    //    if (allTapes.Count > 0)
    //    {
    //        GUI.Label(new Rect(10, Screen.height - 30, 300, 30),
    //            $"������: {allTapes.Count}�� [R] ��� ����");
    //    }
    //}

    //// �����
    //void OnDrawGizmos()
    //{
    //    if (currentState == PlacementState.PlacingTape && currentStartPoint != null)
    //    {
    //        // ������
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(currentStartPoint.GetPosition(), 0.5f);

    //        // ���� ����
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