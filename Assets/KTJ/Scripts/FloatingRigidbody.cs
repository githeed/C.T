using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingRigidbody : MonoBehaviour
{
    [Header("Water")]
    public WaterSurfaceBase water;            // 씬의 WaterSurfaceBase 할당
    public Transform[] floatPoints;           // 선체의 네 귀퉁이 등 여러 포인트 권장(2~4개)

    [Header("Buoyancy")]
    public float maxDepth = 1.0f;             // 이 깊이만큼 잠기면 최대 부력
    public float buoyancyMultiplier = 1.0f;   // 전체 부력 스케일(질량과 함께 튜닝)

    [Header("Drag (공기/물)")]
    public float airDrag = 0.05f;
    public float airAngularDrag = 0.05f;
    public float waterDrag = 3.0f;
    public float waterAngularDrag = 1.5f;

    [Header("Flow (물 흐름 영향)")]
    public float flowStrength = 1.0f;         // 물 흐름 가중치(0이면 끔)

    [Header("FX (선택)")]
    public GameObject splashVFX;
    public float splashSpeedThreshold = 3.0f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (floatPoints == null || floatPoints.Length == 0)
            floatPoints = new[] { this.transform }; // 최소 1점
    }

    void FixedUpdate()
    {
        if (!water) return;

        int submergedCount = 0;

        foreach (var p in floatPoints)
        {
            Vector3 pos = p.position;

            float waterHeight = water.GetHeight(pos);
            float depth = waterHeight - pos.y;        // +면 잠긴 것
            if (depth > 0f)
            {
                submergedCount++;

                // 잠긴 비율(0~1)
                float submergence = Mathf.Clamp01(depth / maxDepth);

                // 부력: 질량 * g * 잠김비율
                float buoyancy = Physics.gravity.magnitude * rb.mass * submergence * buoyancyMultiplier;

                // 물 표면 노말 방향으로 힘 가하기 (포인트 위치에)
                Vector3 normal = water.GetNormal(pos);
                rb.AddForceAtPosition(normal * buoyancy, pos, ForceMode.Force);

                // 물 흐름(수평 가속도) 반영(선택)
                if (flowStrength > 0f)
                {
                    Vector3 flowAccel = water.GetFlow(pos) * flowStrength;
                    rb.AddForceAtPosition(flowAccel * rb.mass, pos, ForceMode.Force);
                }
            }
        }

        // 드래그를 잠김 비율로 블렌딩
        float t = (float)submergedCount / floatPoints.Length;
        rb.linearDamping = Mathf.Lerp(airDrag, waterDrag, t);
        rb.angularDamping = Mathf.Lerp(airAngularDrag, waterAngularDrag, t);
    }

    // 물에 빠르게 진입 시 스플래시 (옵션)
    void OnCollisionEnter(Collision col)
    {
        if (!splashVFX || !water) return;

        // 충돌 지점이 물 높이 근처이고, 속도가 빠를 때
        foreach (var c in col.contacts)
        {
            float y = c.point.y;
            float wy = water.GetHeight(c.point);
            if (Mathf.Abs(y - wy) < 0.15f && rb.linearVelocity.magnitude > splashSpeedThreshold)
            {
                Instantiate(splashVFX, new Vector3(c.point.x, wy, c.point.z), Quaternion.identity);
                break;
            }
        }
    }
}
