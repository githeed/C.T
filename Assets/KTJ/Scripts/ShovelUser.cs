using UnityEngine;

public class ShovelUser : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;
    public TerrainDigger digger;

    [Header("Animator Params")]
    public string shovelTrigger = "Shovel";
    public string shovelStateName = "Survival_Build_Shoveling";

    [Header("Use")]
    public KeyCode useKey = KeyCode.G;
    public float triggerCooldown = 0.4f;

    [Header("Lock & Visuals")]
    public Behaviour[] movementScriptsToDisable;
    public GameObject handShovel;
    public GameObject digShovel;

    [Header("Mud (Prefab & Socket)")]
    public GameObject mud;                // ← 프리팹으로 사용
    public Transform mudSocket;           // ← 삽 끝(붙일 위치)
    public Vector3 mudLocalPos;           // 소켓 기준 위치 보정
    public Vector3 mudLocalEuler;         // 소켓 기준 회전 보정
    public Vector3 mudLocalScale = Vector3.one;
    public bool addRbIfMissing = true;

    [Header("Release (날리기)")]
    public float releaseImpulse = 2.5f;   // MudOut 시 임펄스
    public Vector3 extraReleaseDir = new Vector3(0f, 1f, 0.2f);
    public float randomTorque = 1.0f;     // 약간의 회전 임펄스

    [Header("Audio")]
    public AudioClip shovelSound;
    public AudioSource shovelAudioSource;

    private bool hasShovel = false;
    private int shovelStateHash;
    private float nextTriggerTime = 0f;

    // runtime mud
    private GameObject mudInst;
    private Rigidbody  mudRb;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        shovelStateHash = Animator.StringToHash(shovelStateName);

        bool ok = false;
        foreach (var p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == shovelTrigger) { ok = true; break; }
        if (!ok) Debug.LogWarning($"[ShovelUser] Trigger '{shovelTrigger}' 없음");

        if (!shovelAudioSource) shovelAudioSource = GetComponent<AudioSource>();
        if (!mudSocket && digShovel) mudSocket = digShovel.transform; // 기본값
    }

    public void SetHasShovel(bool v)
    {
        hasShovel = v;
        if (digger) digger.SetHasShovel(v);
    }

    void Update()
    {
        if (!hasShovel) return;

        if (Input.GetKeyDown(useKey) && Time.time >= nextTriggerTime && CanTrigger())
        {
            animator.ResetTrigger(shovelTrigger);
            animator.SetTrigger(shovelTrigger);
            nextTriggerTime = Time.time + triggerCooldown;
        }
    }

    // ===== 애니 이벤트 =====
    public void ShovelBegin()
    {
        SetMovementLock(true);
        if (handShovel) handShovel.SetActive(false);
        if (digShovel)  digShovel.SetActive(true);
        // mud 생성은 Anim_DigOnce에서
    }

    public void ShovelEnd()
    {
        SetMovementLock(false);
        if (handShovel) handShovel.SetActive(true);
        if (digShovel)  digShovel.SetActive(false);
    }

    // 삽이 흙을 퍼올린 타이밍
    public void Anim_DigOnce()
    {
        if (digger) digger.DigOnce();
        if (shovelAudioSource && shovelSound) shovelAudioSource.PlayOneShot(shovelSound);

        if (!mud) { Debug.LogWarning("[ShovelUser] mud 프리팹이 비어있음"); return; }

        // 1) 새 프리팹 생성 (이전 mudInst는 파괴/재활용하지 않음)
        var newMud = Instantiate(mud);
        newMud.name = mud.name + " (Inst)";

        // 2) 삽 끝에 부착
        Transform parent = mudSocket ? mudSocket : transform;
        newMud.transform.SetParent(parent, false);
        newMud.transform.localPosition = mudLocalPos;
        newMud.transform.localRotation = Quaternion.Euler(mudLocalEuler);
        newMud.transform.localScale    = mudLocalScale;

        // 3) 물리 잠금(들고있는 동안)
        var rb = newMud.GetComponent<Rigidbody>();
        if (!rb && addRbIfMissing) rb = newMud.AddComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity  = false;
            rb.linearVelocity = Vector3.zero;          // ← linearVelocity가 아니라 velocity
            rb.angularVelocity = Vector3.zero;
        }

        // 최신 사이클의 mud 포인터 갱신 (이전 것들은 씬에 그대로 둠)
        mudInst = newMud;
        mudRb   = rb;
    }

// 흙을 떨어뜨리는 타이밍 (최신 사이클의 mud만 분리/낙하)
    public void MudOut()
    {
        if (!mudInst) { Debug.LogWarning("[ShovelUser] MudOut 호출됐지만 mudInst 없음"); return; }

        // 1) 부모 분리(월드 좌표 유지)
        mudInst.transform.SetParent(null, true);

        // 2) 물리 전환
        if (!mudRb) mudRb = mudInst.GetComponent<Rigidbody>();
        if (!mudRb && addRbIfMissing) mudRb = mudInst.AddComponent<Rigidbody>();
        if (mudRb)
        {
            mudRb.isKinematic = false;
            mudRb.useGravity  = true;

            if (releaseImpulse > 0f)
            {
                Vector3 dir = (mudSocket ? mudSocket.forward : transform.forward) + extraReleaseDir;
                mudRb.AddForce(dir.normalized * releaseImpulse, ForceMode.Impulse);

                if (randomTorque > 0f)
                    mudRb.AddTorque(Random.onUnitSphere * randomTorque, ForceMode.Impulse);
            }
        }
    }

    void SetMovementLock(bool locked)
    {
        if (movementScriptsToDisable == null) return;
        foreach (var b in movementScriptsToDisable)
        {
            if (!b) continue;
            if (b == this || b == animator || b is Animator) continue;
            b.enabled = !locked;
        }
    }

    bool CanTrigger()
    {
        var s = animator.GetCurrentAnimatorStateInfo(0);
        return !(s.shortNameHash == shovelStateHash || animator.IsInTransition(0));
    }
}
