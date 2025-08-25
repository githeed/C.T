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

    public Behaviour[] movementScriptsToDisable;
    public GameObject handShovel;
    public GameObject digShovel;
    public GameObject mud;
    public AudioClip shovelSound;
    public AudioSource shovelAudioSource;
    
    private bool hasShovel = false;
    private bool isShoveling = false;
    private int shovelStateHash;
    private float nextTriggerTime = 0f;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        shovelStateHash = Animator.StringToHash(shovelStateName);
        bool ok = false;
        foreach (var p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == shovelTrigger) ok = true;
        if (!ok) Debug.LogWarning($"[ShovelUser] Trigger '{shovelTrigger}' 없음");
        
        shovelAudioSource = gameObject.GetComponent<AudioSource>();
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
        handShovel.SetActive(false);
        digShovel.SetActive(true);
        mud.SetActive(false);
    }


    public void ShovelEnd()
    {
        SetMovementLock(false);
        handShovel.SetActive(true);
        digShovel.SetActive(false);
        mud.SetActive(false);
    }

    void SetMovementLock(bool locked)
    {
        isShoveling = locked;
        
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
        mud.SetActive(true);
        shovelAudioSource.PlayOneShot(shovelSound);
    }
}