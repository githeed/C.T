using UnityEngine;

public class TerrainShovel : MonoBehaviour
{
    public Terrain terrain;
    public Camera sceneCamera;
    public Transform shovelTip;
    public bool useShovelTipRay = false;

    [Header("Raycast")]
    public float rayDistance = 100f;       // ← 5f → 100f
    public bool castDownward = true;       // ← shovelTip.forward 대신 아래로 쏘기
    public bool drawDebugRay = true;

    [Header("Brush")]
    public float brushRadiusMeters = 2.5f;
    public float strengthMetersPerSec = 0.5f;
    public AnimationCurve falloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public enum Mode { Lower, Raise, Flatten }
    public Mode mode = Mode.Flatten;

    [Header("Flatten")]
    public float targetHeightWorldY;
    public bool captureTargetOnClick = true;

    [Header("Input")]
    public bool requireMouseHold = false;  // ← 마우스 없이도 동작하도록 기본 false

    [Header("Mask")]
    public LayerMask hitLayers = ~0;

    void Update()
    {
        if (!terrain) return;

        bool pressed = !requireMouseHold || Input.GetMouseButton(0);

        if (pressed && TryGetHitPoint(out var hit))
        {
            if (mode == Mode.Flatten && captureTargetOnClick && Input.GetMouseButtonDown(0))
                targetHeightWorldY = hit.point.y;

            ApplyBrushAt(hit.point, Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) mode = Mode.Lower;
        if (Input.GetKeyDown(KeyCode.Alpha2)) mode = Mode.Raise;
        if (Input.GetKeyDown(KeyCode.Alpha3)) mode = Mode.Flatten;
        if (Input.GetKeyDown(KeyCode.F) && TryGetHitPoint(out var hit2))
            targetHeightWorldY = hit2.point.y;
    }

    bool TryGetHitPoint(out RaycastHit hit)
    {
        if (useShovelTipRay && shovelTip)
        {
            Vector3 dir = castDownward ? Vector3.down : shovelTip.forward;
            if (drawDebugRay) Debug.DrawRay(shovelTip.position, dir * rayDistance, Color.cyan);
            return Physics.Raycast(shovelTip.position, dir, out hit, rayDistance, hitLayers, QueryTriggerInteraction.Ignore);
        }
        else if (sceneCamera)
        {
            var ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
            if (drawDebugRay) Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);
            return Physics.Raycast(ray, out hit, rayDistance, hitLayers, QueryTriggerInteraction.Ignore);
        }
        hit = default;
        return false;
    }

    void ApplyBrushAt(Vector3 worldPoint, float dt)
    {
        var td = terrain.terrainData;
        Vector3 size = td.size;
        Vector3 tpos = terrain.GetPosition();

        Vector3 local = worldPoint - tpos;
        float u = Mathf.Clamp01(local.x / size.x);
        float v = Mathf.Clamp01(local.z / size.z);

        int hm = td.heightmapResolution;
        int cx = Mathf.RoundToInt(u * (hm - 1));
        int cz = Mathf.RoundToInt(v * (hm - 1));

        int rx = Mathf.CeilToInt(brushRadiusMeters * (hm - 1) / size.x);
        int rz = Mathf.CeilToInt(brushRadiusMeters * (hm - 1) / size.z);

        int xBase = Mathf.Clamp(cx - rx, 0, hm - 1);
        int zBase = Mathf.Clamp(cz - rz, 0, hm - 1);
        int width  = Mathf.Clamp(cx + rx, 0, hm - 1) - xBase + 1;
        int height = Mathf.Clamp(cz + rz, 0, hm - 1) - zBase + 1;

        float[,] heights = td.GetHeights(xBase, zBase, width, height);

        float targetH = Mathf.Clamp01((targetHeightWorldY - tpos.y) / size.y);
        float deltaNormPerSec = strengthMetersPerSec / size.y;

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int hx = xBase + i;
                int hz = zBase + j;

                float dx = (hx - cx) / (float)Mathf.Max(1, rx);
                float dz = (hz - cz) / (float)Mathf.Max(1, rz);
                float dist = Mathf.Sqrt(dx * dx + dz * dz);
                if (dist > 1f) continue;

                float w = falloff.Evaluate(dist);
                if (w <= 0f) continue;

                float h = heights[j, i];

                switch (mode)
                {
                    case Mode.Lower:
                        h -= deltaNormPerSec * dt * w;
                        break;
                    case Mode.Raise:
                        h += deltaNormPerSec * dt * w;
                        break;
                    case Mode.Flatten:
                        float t = deltaNormPerSec * dt * w;
                        h = Mathf.Lerp(h, targetH, t);
                        break;
                }

                heights[j, i] = Mathf.Clamp01(h);
            }
        }

        td.SetHeights(xBase, zBase, heights);
    }
}
