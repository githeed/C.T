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

    [Header("Movement Lock")]
    public Behaviour[] movementScriptsToDisable;

    [Header("Shovel Visuals")]
    public GameObject handShovel;   // 평소 들고다니는 삽
    public GameObject digShovel;    // 파기 동작 시 보여줄 삽(애니용)

    [Header("Mud (Prefab flow)")]
    public GameObject mudPrefab;            // 생성할 흙 프리팹
    public Transform mudAttachParent;       // 파는 동안 붙일 부모(보통 digShovel의 끝부분 Transform)
    public Vector3 mudLocalPos;             // 붙였을 때 로컬 위치 보정
    public Vector3 mudLocalEuler;           // 붙였을 때 로컬 회전 보정
    public Vector3 mudLocalScale = Vector3.one;
    public bool addRbIfMissing = true;      // 프리팹에 RB 없으면 자동 추가
    public float releaseImpulse = 0f;       // MudOut 시 살짝 튀기고 싶으면 >0 (N·s)
    public Vector3 extraReleaseDir = Vector3.up * 1.0f;

    [Header("Audio")]
    public AudioClip shovelSound;
    public AudioSource shovelAudioSource;

    private bool hasShovel = false;
    private int shovelStateHash;
    private float nextTriggerTime = 0f;

    private GameObject mudInstance;
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

        if (!mudAttachParent && digShovel) mudAttachParent = digShovel.transform;
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

    void SetMovementLock(bool locked)
    {
        if (movementScriptsToDisable == null) return;
        foreach (var b in movementScriptsToDisable)
            if (b) b.enabled = !locked;
    }

    bool CanTrigger()
    {
        var s = animator.GetCurrentAnimatorStateInfo(0);
        return !(s.shortNameHash == shovelStateHash || animator.IsInTransition(0));
    }
    
    public void Anim_DigOnce()
    {
        if (digger) digger.DigOnce();
        if (shovelAudioSource && shovelSound) shovelAudioSource.PlayOneShot(shovelSound);

        if (mudInstance) { Destroy(mudInstance); mudInstance = null; mudRb = null; }

        if (!mudPrefab)
        {
            Debug.LogWarning("[ShovelUser] mudPrefab 미할당");
            return;
        }

        mudInstance = Instantiate(mudPrefab);
        mudInstance.name = mudPrefab.name + " (Inst)";

        Transform parent = mudAttachParent ? mudAttachParent : transform;
        mudInstance.transform.SetParent(parent, worldPositionStays: false);
        mudInstance.transform.localPosition = mudLocalPos;
        mudInstance.transform.localRotation = Quaternion.Euler(mudLocalEuler);
        mudInstance.transform.localScale    = mudLocalScale;

        mudRb = mudInstance.GetComponent<Rigidbody>();
        if (!mudRb && addRbIfMissing) mudRb = mudInstance.AddComponent<Rigidbody>();
        if (mudRb)
        {
            mudRb.isKinematic = true;
            mudRb.useGravity  = false;
            mudRb.linearVelocity    = Vector3.zero;
            mudRb.angularVelocity = Vector3.zero;
        }
    }
    
    public void MudOut()
    {
        if (!mudInstance)
        {
            Debug.LogWarning("[ShovelUser] MudOut 호출됐지만 mudInstance 없음");
            return;
        }
        mudInstance.transform.SetParent(null, true);

        if (!mudRb) mudRb = mudInstance.GetComponent<Rigidbody>();
        if (!mudRb && addRbIfMissing) mudRb = mudInstance.AddComponent<Rigidbody>();
        if (mudRb)
        {
            mudRb.isKinematic = false;
            mudRb.useGravity  = true;

            if (releaseImpulse > 0f)
            {
                Vector3 dir = (mudAttachParent ? mudAttachParent.forward : transform.forward) + extraReleaseDir;
                mudRb.AddForce(dir.normalized * releaseImpulse, ForceMode.Impulse);
            }
        }
    }
}
