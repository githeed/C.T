using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PipeGameInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pipeGameCanvas;  // 파이프 게임 전체 캔버스
    public GameObject interactionPrompt; // "Q키를 눌러 시작" UI
    public TextMeshProUGUI promptText;

    [Header("Game References")]
    public PipePuzzleManager pipePuzzleManager;
    public H_CharacterMovement playerController;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.Q;

    public bool isInRange = false;
    public bool isGameActive = false;
    public GameObject currentTrigger;

    void Start()
    {
        // 초기 상태 설정
        if (pipeGameCanvas != null)
            pipeGameCanvas.SetActive(false);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // 플레이어 자동 찾기
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<H_CharacterMovement>(FindObjectsInactive.Exclude);
        }

        // PipePuzzleManager 자동 찾기
        if (pipePuzzleManager == null && pipeGameCanvas != null)
        {
            pipePuzzleManager = pipeGameCanvas.GetComponentInChildren<PipePuzzleManager>();
        }

        if (promptText != null)
        {
            promptText.text = $"{interactionKey} 키를 눌러 파이프 퍼즐 시작";
        }
    }

    void Update()
    {
        // 범위 내에 있고 Q키를 누르면
        if (isInRange && !isGameActive && Input.GetKeyDown(interactionKey))
        {
            StartPipeGame();
        }

        // 게임 중 ESC로 나가기
        if (isGameActive && Input.GetKeyDown(KeyCode.K))
        {
            ExitPipeGame();
        }
    }

    public void ShowPrompt()
    {
        if (interactionPrompt != null && !isGameActive)
        {
            interactionPrompt.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    public void StartPipeGame()
    {
        isGameActive = true;

        // UI 활성화
        if (pipeGameCanvas != null)
            pipeGameCanvas.SetActive(true);
        if (pipePuzzleManager != null)
            pipePuzzleManager.gameObject.SetActive(true);


        HidePrompt();

        // 플레이어 움직임 비활성화
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // 마우스 커서 표시
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("파이프 게임 시작!");
    }

    public void ExitPipeGame()
    {
        isGameActive = false;

        // UI 비활성화
        if (pipeGameCanvas != null)
            pipeGameCanvas.SetActive(false);
        if (pipePuzzleManager != null)
            pipePuzzleManager.gameObject.SetActive(false);


        // 플레이어 움직임 활성화
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // 마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 범위 내에 있으면 프롬프트 다시 표시
        if (isInRange)
        {
            ShowPrompt();
        }

        Debug.Log("파이프 게임 종료!");
    }

    // PipePuzzleManager에서 호출할 수 있는 메서드
    public void OnPuzzleComplete()
    {
        // 퍼즐 완료 시 처리
        Debug.Log("퍼즐 완료!");

        // 보상 지급 등 추가 로직
        // ...
        isGameActive = false;

        // UI 비활성화
        if (pipeGameCanvas != null)
            pipeGameCanvas.SetActive(false);
        if (pipePuzzleManager != null)
            pipePuzzleManager.gameObject.SetActive(false);


        // 플레이어 움직임 활성화
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // 마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        // 잠시 후 자동으로 게임 종료
        Invoke("ExitPipeGame", 2f);
    }

    //void OnDrawGizmosSelected()
    //{
    //    // 상호작용 범위 표시
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, interactionDistance);
    //}
}
