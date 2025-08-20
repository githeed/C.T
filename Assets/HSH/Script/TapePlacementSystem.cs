using System.Collections.Generic;
using UnityEngine;

public class TapePlacementSystem : MonoBehaviour
{
    [Header("Tape Settings")]
    [SerializeField] private GameObject tapePrefab; // WarningTape ������
    [SerializeField] private GameObject placeMarkerPrefab; // ��ġ ��ġ ǥ�ÿ� ��Ŀ (�ɼ�)
    [SerializeField] private float placementDistance = 5f; // �÷��̾�κ��� ��ġ �Ÿ�
    [SerializeField] private float placementHeight = 1.5f; // ��ġ ����

    [Header("Visual Feedback")]
    [SerializeField] private Color previewColor = new Color(1f, 1f, 0f, 0.5f); // �̸����� ����
    [SerializeField] private Color validPlacementColor = Color.green;
    [SerializeField] private Color invalidPlacementColor = Color.red;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip removeSound;
    private AudioSource audioSource;

    // ��ġ ����
    private enum PlacementState
    {
        Idle,           // ��� ����
        PlacingEnd      // ���� ��ġ ��
    }

    private PlacementState currentState = PlacementState.Idle;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private GameObject currentPreviewMarker;
    private LineRenderer previewLine;
    private List<GameObject> allTapes = new List<GameObject>();
    private GameObject startMarker;
    private GameObject endMarker;

    // ī�޶� ����
    private Camera playerCamera;

    void Start()
    {
        // ī�޶� ã��
        if (Camera.main != null)
        {
            playerCamera = Camera.main;
        }
        else
        {
            // TPSCameraController�� ���� ī�޶� ã��
            H_CamController tpsCamera = FindFirstObjectByType<H_CamController>(FindObjectsInactive.Exclude);
            if (tpsCamera != null)
            {
                playerCamera = tpsCamera.GetComponent<Camera>();
            }
        }

        // ����� �ҽ� ����
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (placeSound != null || removeSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // �̸����� ���� ����
        CreatePreviewLine();
    }

    void CreatePreviewLine()
    {
        GameObject previewObj = new GameObject("TapePreview");
        previewObj.transform.parent = transform;
        previewLine = previewObj.AddComponent<LineRenderer>();

        // ���� ������ ����
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
        // EŰ: ������ ��ġ
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleTapePlacement();
        }

        // RŰ: ��� ������ ����
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveAllTapes();
        }

        // ESCŰ: ��ġ ���
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
                // ù ��° EŰ: ������ ��� �����ϰ� ���� ��� ����
                StartPlacement(placementPos);
                break;

            case PlacementState.PlacingEnd:
                // �� ��° EŰ: ���� �����ϰ� ������ ���� �� �Ϸ�
                CompletePlacement(placementPos);
                break;
        }
    }

    void StartPlacement(Vector3 position)
    {
        // ������ ��� Ȯ��
        currentState = PlacementState.PlacingEnd;
        startPoint = position;

        // ������ ��Ŀ ����
        if (placeMarkerPrefab != null)
        {
            startMarker = Instantiate(placeMarkerPrefab, startPoint, Quaternion.identity);
            startMarker.name = "StartMarker";
        }
        else
        {
            startMarker = CreateDefaultMarker(startPoint, validPlacementColor);
        }

        // �̸����� ���� Ȱ��ȭ
        previewLine.enabled = true;

        // ���� ���
        PlaySound(placeSound);

        Debug.Log("������ ������ ������");
    }

    void CompletePlacement(Vector3 position)
    {
        endPoint = position;

        // ���� ��Ŀ ����
        if (placeMarkerPrefab != null)
        {
            endMarker = Instantiate(placeMarkerPrefab, endPoint, Quaternion.identity);
            endMarker.name = "EndMarker";
        }
        else
        {
            endMarker = CreateDefaultMarker(endPoint, validPlacementColor);
        }

        // ������ ����
        CreateTape(startPoint, endPoint);

        // ���� �ʱ�ȭ (��ġ ��� ����)
        ResetPlacement();

        // ���� ���
        PlaySound(placeSound);

        Debug.Log("������ ��ġ �Ϸ�!.");
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
            // �������� ������ �⺻ ������ ����
            tapeObj = new GameObject("WarningTape");
            WarningTape tape = tapeObj.AddComponent<WarningTape>();

            // LineRenderer �߰� (WarningTape�� �ʿ�)
            LineRenderer lr = tapeObj.GetComponent<LineRenderer>();
            if (lr == null)
            {
                lr = tapeObj.AddComponent<LineRenderer>();
            }
        }

        // WarningTape ������Ʈ ����
        WarningTape warningTape = tapeObj.GetComponent<WarningTape>();
        if (warningTape != null)
        {
            warningTape.SetPoints(start, end);
        }

        // ����Ʈ�� �߰�
        allTapes.Add(tapeObj);

        // ��Ŀ�� ����Ʈ�� �߰� (RŰ�� �Բ� �����ϱ� ����)
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
            Debug.Log("������ �������� �����ϴ�.");
            return;
        }

        // ��� �������� ��Ŀ ����
        foreach (GameObject tape in allTapes)
        {
            if (tape != null)
            {
                Destroy(tape);
            }
        }

        allTapes.Clear();

        // ���� ��ġ ���� �͵� ���
        CancelPlacement();

        // ���� ���
        PlaySound(removeSound);

        Debug.Log("��� �������� ���ŵǾ����ϴ�.");
    }

    void CancelPlacement()
    {
        // �ӽ� ��Ŀ ����
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
        Debug.Log("������ ��ġ�� ��ҵǾ����ϴ�.");
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

        // �̸����� ���� ������Ʈ
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

        // ī�޶� ����ĳ��Ʈ�� �� ��Ȯ�� ��ġ ã�� (�ɼ�)
        if (playerCamera != null)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50f))
            {
                position = hit.point;
                position.y += 0.1f; // �ٴڿ��� �ణ ����
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

        // �ݶ��̴� ���� (�浹 ����)
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

    // ����׿� GUI
    void OnGUI()
    {
        if (currentState != PlacementState.Idle)
        {
            GUI.Label(new Rect(Screen.width / 2 - 100, 50, 200, 30),
                $"������ ��ġ ���: {currentState}");
            GUI.Label(new Rect(Screen.width / 2 - 100, 80, 200, 30),
                "ESC: ��� | E: Ȯ��");
        }

        if (allTapes.Count > 0)
        {
            GUI.Label(new Rect(10, Screen.height - 30, 200, 30),
                $"��ġ�� ������: {allTapes.Count / 3}�� (RŰ�� ��� ����)");
        }
    }

    // �����
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