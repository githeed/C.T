using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TreeChopZone : MonoBehaviour
{
   [Header("맞으면 떨어질 파트")]
    public Rigidbody targetPart;

    [Header("필요 타격 수")]
    public int hitsToDrop = 2;

    [Header("히트 판정 옵션")]
    public bool requireExitForNextHit = true; // 다음 히트는 반드시 Exit 후
    public float minHitInterval = 0.15f;      // 동일 스윙 중복 방지(시간)
    public float minAxeSpeed = 0f;            // 너무 느린 접촉은 무시하고 싶으면 >0 로

    int   _hits = 0;
    bool  _axeInside = false;
    float _lastHitTime = -999f;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity  = false;
    }

    void Start()
    {
        if (targetPart)
        {
            targetPart.isKinematic = true;
            targetPart.useGravity  = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Axe")) return;

        _axeInside = true;
        TryCountHit(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Axe")) return;
        _axeInside = false;

        // Exit가 요구되면, 여기서 다음 히트 가능 상태가 됨
        // (별도 플래그 필요 없이 TryCountHit에서 시간을 체크하기 때문에 이것만으로 충분)
        Debug.Log($"[{name}] Axe EXIT");
    }

    void TryCountHit(Collider axeCol)
    {
        // 1) Exit 전에는 추가 히트 금지 옵션
        if (requireExitForNextHit && _hits > 0 && _axeInside)
        {
            Debug.Log($"[{name}] waiting for EXIT before next hit");
            return;
        }

        // 2) 동일 스윙 중복 방지(시간)
        if (Time.time - _lastHitTime < minHitInterval)
        {
            Debug.Log($"[{name}] hit ignored (interval)");
            return;
        }

        // 3) 속도 임계치 (Rigidbody 없으면 0으로 계산됨)
        float speed = axeCol.attachedRigidbody ? axeCol.attachedRigidbody.linearVelocity.magnitude : 0f;
        if (speed < minAxeSpeed)
        {
            Debug.Log($"[{name}] hit ignored (speed {speed:F2} < {minAxeSpeed})");
            return;
        }

        _lastHitTime = Time.time;
        _hits++;
        Debug.Log($"[{name}] HIT => {_hits}/{hitsToDrop}");

        if (_hits >= hitsToDrop)
        {
            DropNow();
        }
    }

    void DropNow()
    {
        if (!targetPart) return;
        targetPart.isKinematic = false;
        targetPart.useGravity  = true;
        Debug.Log($"[{name}] DROPPED!");
        GetComponent<Collider>().enabled = false; // 한 번만 작동
    }
}
