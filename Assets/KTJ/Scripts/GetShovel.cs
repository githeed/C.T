using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GetShovel : MonoBehaviour
{
    [Tooltip("아웃라인을 켤 대상(부모 오브젝트에 Outline이 있어야 함)")]
    public Outline outlineTarget;      // Shovel Object의 Outline
    public string playerTag = "Player";
    public TextMeshProUGUI text;

    [Header("References")]
    public GameObject shovelInPlayer;  // 플레이어 손(인벤토리)에 숨겨둔 삽
    public GameObject shovelInMap;     // 맵에 놓여있는 삽(월드)

    // 내부 상태
    private bool inRange = false;
    private bool hasShovel = false;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        if (outlineTarget == null)
            outlineTarget = GetComponentInParent<Outline>();
    }

    void Awake()
    {
        if (outlineTarget) outlineTarget.enabled = false;
        if (text) text.gameObject.SetActive(false);

        // 시작은 손의 삽을 꺼두는 걸 권장
        if (shovelInPlayer) shovelInPlayer.SetActive(false);
    }

    void Update()
    {
        // 범위 안이고 아직 안 주웠을 때만 G키 감지
        if (!inRange || hasShovel) return;

        if (Input.GetKeyDown(KeyCode.G))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        hasShovel = true;

        if (shovelInPlayer) shovelInPlayer.SetActive(true);
        if (shovelInMap)    shovelInMap.SetActive(false); // 필요하면 Destroy(shovelInMap);

        if (outlineTarget) outlineTarget.enabled = false;
        if (text)          text.gameObject.SetActive(false);

        // 다시 트리거 안 걸리게 콜라이더 끄거나 스크립트 제거
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        // Destroy(this); // 스크립트만 제거하고 싶으면 사용
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other) || hasShovel) return;

        inRange = true;
        if (outlineTarget) outlineTarget.enabled = true;
        if (text)          text.gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;

        inRange = false;
        if (outlineTarget) outlineTarget.enabled = false;
        if (text)          text.gameObject.SetActive(false);
    }

    bool IsPlayer(Collider c)
    {
        // 태그 또는 CharacterController 보정
        return c.CompareTag(playerTag) || c.GetComponent<CharacterController>() != null;
    }
}
