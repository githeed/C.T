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
    // public KeyCode useKey = KeyCode.G; // ← 더 이상 사용 안 함
    public float triggerCooldown = 0.4f;

    [Header("Lock & Visuals")]
    public Behaviour[] movementScriptsToDisable;
    public GameObject handShovel;
    public GameObject digShovel;

    [Header("Mud (Prefab & Socket)")]
    public GameObject mud;                
    public Transform mudSocket;           
    public Vector3 mudLocalPos;           
    public Vector3 mudLocalEuler;         
    public Vector3 mudLocalScale = Vector3.one;
    public bool addRbIfMissing = true;

    [Header("Release (날리기)")]
    public float releaseImpulse = 2.5f;   
    public Vector3 extraReleaseDir = new Vector3(0f, 1f, 0.2f);
    public float randomTorque = 1.0f;     

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
        if (!mudSocket && digShovel) mudSocket = digShovel.transform;
    }

    public void SetHasShovel(bool v)
    {
        hasShovel = v;
        if (digger) digger.SetHasShovel(v);
    }

    void Update()
    {
        if (!hasShovel) return;

        // ▼ G → 마우스 좌클릭으로 변경
        if (Input.GetMouseButtonDown(0) && Time.time >= nextTriggerTime && CanTrigger())
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
    }

    public void ShovelEnd()
    {
        SetMovementLock(false);
        if (handShovel) handShovel.SetActive(true);
        if (digShovel)  digShovel.SetActive(false);
    }

    public void Anim_DigOnce()
    {
        if (digger) digger.DigOnce();
        if (shovelAudioSource && shovelSound) shovelAudioSource.PlayOneShot(shovelSound);

        if (!mud) { Debug.LogWarning("[ShovelUser] mud 프리팹이 비어있음"); return; }

        var newMud = Instantiate(mud);
        newMud.name = mud.name + " (Inst)";

        Transform parent = mudSocket ? mudSocket : transform;
        newMud.transform.SetParent(parent, false);
        newMud.transform.localPosition = mudLocalPos;
        newMud.transform.localRotation = Quaternion.Euler(mudLocalEuler);
        newMud.transform.localScale    = mudLocalScale;

        var rb = newMud.GetComponent<Rigidbody>();
        if (!rb && addRbIfMissing) rb = newMud.AddComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity  = false;
            rb.linearVelocity = Vector3.zero;          // ← 고쳤음!
            rb.angularVelocity = Vector3.zero;
        }

        mudInst = newMud;
        mudRb   = rb;
    }

    public void MudOut()
    {
        if (!mudInst) { Debug.LogWarning("[ShovelUser] MudOut 호출됐지만 mudInst 없음"); return; }

        mudInst.transform.SetParent(null, true);

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
