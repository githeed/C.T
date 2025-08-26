using UnityEngine;

public class PlayerChopping : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string chopTrigger = "TreeChop";
    public float triggerCooldown = 0.45f;

    [Header("장비 체크")]
    public bool requireAxeEquipped = true;
    public GameObject handAxe;
    public GameObject chopAxe;
    public BoxCollider axeCollider;

    [Header("스윙 중 비활성화할 스크립트")]
    public Behaviour[] movementScriptsToDisable;

    int chopTriggerHash;
    float nextTriggerTime = 0f;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        chopTriggerHash = Animator.StringToHash(chopTrigger);

        if (chopAxe) chopAxe.SetActive(false);
        axeCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryChop();
    }

    void TryChop()
    {
        if (Time.time < nextTriggerTime) return;
        if (requireAxeEquipped && handAxe && !handAxe.activeInHierarchy) return;

        animator.SetTrigger(chopTriggerHash);
        nextTriggerTime = Time.time + triggerCooldown;
    }

    // 애니메이션 이벤트에서 호출
    public void ChopBegin()
    {
        ToggleMovement(false);            // ↓ Begin에선 "끄기"
        if (handAxe) handAxe.SetActive(false);
        if (chopAxe) chopAxe.SetActive(true);
    }

    // 애니메이션 이벤트에서 호출
    public void ChopEnd()
    {
        ToggleMovement(true);             // ↑ End에선 "켜기"
        if (handAxe) handAxe.SetActive(true);
        if (chopAxe) chopAxe.SetActive(false);

    }

    void ToggleMovement(bool enable)
    {
        if (movementScriptsToDisable == null) return;
        foreach (var b in movementScriptsToDisable)
        {
            if (!b) continue;
            if (b == this) continue;          // 자신 끄지 않기
            if (b is Animator) continue;      // Animator 끄지 않기
            b.enabled = enable;
        }
    }

    public void AxeColOn()
    {
        axeCollider.enabled = true;
    }

    public void AxeColOff()
    {
        axeCollider.enabled = false;
    }
}