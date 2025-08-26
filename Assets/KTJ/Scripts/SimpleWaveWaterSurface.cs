using UnityEngine;

public class SimpleWaveWaterSurface : WaterSurfaceBase
{
    public float baseLevel = 0f;
    public float amplitude = 0.25f;
    public float wavelength = 6f;
    public float speed = 1.2f;

    public override float GetHeight(Vector3 worldPos)
    {
        float k = 2f * Mathf.PI / Mathf.Max(0.001f, wavelength);
        float t = Time.time * speed;
        // XZ 방향으로 간단한 합성파
        float h = baseLevel
                  + amplitude * Mathf.Sin(k * worldPos.x + t)
                  + 0.6f * amplitude * Mathf.Sin(k * 0.7f * worldPos.z - 1.3f * t);
        return h;
    }

    public override Vector3 GetNormal(Vector3 worldPos)
    {
        // 근사 노말: 미세한 기울기 추정
        float eps = 0.2f;
        float h = GetHeight(worldPos);
        float hx = GetHeight(worldPos + new Vector3(eps, 0, 0));
        float hz = GetHeight(worldPos + new Vector3(0, 0, eps));
        Vector3 n = new Vector3(-(hx - h) / eps, 1f, -(hz - h) / eps).normalized;
        return n;
    }

    public override Vector3 GetFlow(Vector3 worldPos)
    {
        // 물 흐름(수평 가속도) 넣고 싶을 때
        return new Vector3(0.5f, 0, 0.2f); // m/s^2 느낌의 가속도
    }
}