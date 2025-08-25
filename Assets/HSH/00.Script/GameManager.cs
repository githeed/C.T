using UnityEngine;
using UnityEngine.UI;


public enum GameStatus
{
    Ready,
    Playing,
    Ending
}

public class GameManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static GameManager _instance;

    // �ܺο��� ���� ������ ������Ƽ
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


    public GameObject Panel_Start;
    public GameObject Panel_OS;
    public GameObject Panel_Guide;
    public GameObject Panel_Update;
    public Button btn_Next;
    public Button btn_FinishTutorial;


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
        // Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        status = GameStatus.Playing;

        if (Panel_Start != null) Panel_Start.SetActive(false);
        if (Panel_Update != null) Panel_Update.SetActive(true);
    }
}
