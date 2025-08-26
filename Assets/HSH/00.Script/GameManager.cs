using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum GameStatus
{
    Ready,
    ShovelMission,
    TapeMission,
    TreeMission,
    PipeMission,
    Ending
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>(FindObjectsInactive.Exclude);

                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    public GameStatus status;

    [Header("Start UI References")]
    public GameObject Panel_Start;
    public GameObject Panel_OS;
    public Button btn_Next;
    public GameObject Panel_Guide;
    public Button btn_FinishTutorial;

    [Header("Update UI References")]
    public GameObject Panel_Update;
    public GameObject Panel_MisionAlarm;
    public TextMeshProUGUI TMP_MissionAlarmText;
    public GameObject Panel_Complete;
    public GameObject Panel_MissionPopUP;
    public TextMeshProUGUI TMP_MissionNum;
    public TextMeshProUGUI TMP_MissionText;

    private void Start()
    {
        status = GameStatus.Ready;

        if (btn_Next != null) btn_Next.onClick.AddListener(NextUI);
        if (btn_FinishTutorial != null) btn_FinishTutorial.onClick.AddListener(FinishTutorial);

        if (Panel_Start != null) Panel_Start.SetActive(true);
        if (Panel_OS != null) Panel_OS.SetActive(true);
    }

    void NextUI()
    {
        if(Panel_OS != null) Panel_OS.SetActive(false);
        if(Panel_Guide != null) Panel_Guide.SetActive(true);
    }
    
    void FinishTutorial()
    {
        // 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        status = GameStatus.ShovelMission;

        if (Panel_Start != null) Panel_Start.SetActive(false);
        if (Panel_Update != null) Panel_Update.SetActive(true);
    }

    public void UpdateGameState(GameStatus newStatus)
    {
        status = newStatus;

        switch (status)
        {
            case GameStatus.Ready:
                Debug.Log("게임 준비 상태");
                
                break;

            case GameStatus.ShovelMission:
                Debug.Log("삽 미션 시작");
                
                break;

            case GameStatus.TapeMission:
                Debug.Log("테이프 미션 시작");
                
                break;

            case GameStatus.TreeMission:
                Debug.Log("나무 미션 시작");
                
                break;

            case GameStatus.PipeMission:
                Debug.Log("파이프 미션 시작");
               
                break;

            case GameStatus.Ending:
                Debug.Log("엔딩");
              
                break;

            default:
                Debug.LogWarning($"처리되지 않은 상태: {status}");
                break;
        }
    }
}
