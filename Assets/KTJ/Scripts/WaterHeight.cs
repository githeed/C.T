using UnityEngine;

public class WaterHeight : MonoBehaviour
{
    [Header("Target")]
    public Transform waterPlane;
    public Transform riverPlane;

    [Header("Heights")]
    public float targetY = 1f;          // waterPlane 목표 높이

    [Header("Speeds (units/sec) - Water")]
    public float slowRiseSpeed = 0.05f; // 평상시
    public float fastRiseSpeed = 0.2f;  // 트리거 후

    // --- River 전용 설정 ---
    [Header("River Settings")]
    public float riverTargetY = 0.6f;        // 강 수위 최대치
    public float riverSlowRiseSpeed = 0.1f; // 물보다 살짝 빠르게
    public float riverFastRiseSpeed = 0.24f; // 물보다 살짝 빠르게

    [Header("Trigger Source")]
    public WarningTriggerCol wtc;
    public bool latchTrigger = true;   // 한 번 트리거되면 계속 빠르게

    // --- Rain FX ---
    [Header("Rain Particle Systems")]
    public ParticleSystem[] rainSystems;   // 비 파티클들
    public float rainRateNormal  = 600f;   // 시작 값
    public float rainRateBoosted = 4000f;  // 트리거 후 값

    private bool goFast = false;
    private bool rainBoosted = false;

    void Awake()
    {
        if (!waterPlane) waterPlane = transform;
        SetRainRate(rainRateNormal);
    }

    void Update()
    {
        // 트리거 래치
        if (wtc && wtc.isTriggerd) goFast = true;

        bool fastNow = (goFast || (!latchTrigger && wtc && wtc.isTriggerd));

        // --- Water 상승 ---
        if (waterPlane)
        {
            float speed = fastNow ? fastRiseSpeed : slowRiseSpeed;
            Vector3 pos = waterPlane.position;
            float newY = Mathf.MoveTowards(pos.y, targetY, speed * Time.deltaTime);
            waterPlane.position = new Vector3(pos.x, newY, pos.z);
        }

        // --- River 상승(조금 더 빠름, 최대 0.6) ---
        if (riverPlane)
        {
            float rSpeed = fastNow ? riverFastRiseSpeed : riverSlowRiseSpeed;
            Vector3 rpos = riverPlane.position;
            float rNewY = Mathf.MoveTowards(rpos.y, riverTargetY, rSpeed * Time.deltaTime);
            riverPlane.position = new Vector3(rpos.x, rNewY, rpos.z);
        }

        // 비 세기 전환(한 번만 적용)
        if (!rainBoosted && fastNow)
        {
            SetRainRate(rainRateBoosted);
            rainBoosted = true;
        }
    }

    void SetRainRate(float rate)
    {
        if (rainSystems == null) return;
        foreach (var ps in rainSystems)
        {
            if (!ps) continue;
            var em = ps.emission;
            em.rateOverTime = new ParticleSystem.MinMaxCurve(rate);
        }
    }
}
