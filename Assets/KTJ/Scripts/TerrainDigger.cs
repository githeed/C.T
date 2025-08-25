using System.Collections.Generic;
using UnityEngine;

public class TerrainDigger : MonoBehaviour
{
    [Header("General")]
    public Camera cam;                 
    public LayerMask terrainMask = ~0;    
    public float rayDistance = 8f;       
    public bool hasShovel = false;       

    [Header("Brush")]
    public float brushRadiusMeters = 1.2f; 
    public float lowerAmountMeters = 0.12f; 
    public bool softFalloff = true;  

    private readonly Dictionary<Terrain, TerrainData> _original = new();
    private readonly Dictionary<Terrain, TerrainData> _runtime  = new();

    void Awake()
    {
        if (cam == null) cam = Camera.main;

        foreach (var t in Terrain.activeTerrains)
        {
            if (t == null || t.terrainData == null) continue;
            if (_original.ContainsKey(t)) continue;

            _original[t] = t.terrainData;
            var clone = Instantiate(t.terrainData);
            _runtime[t]  = clone;
            t.terrainData = clone;
        }
    }

    void OnDisable()
    {
        foreach (var kv in _original)
        {
            var t = kv.Key;
            if (t) t.terrainData = kv.Value;
        }
        _original.Clear();
        _runtime.Clear();
    }

    public void SetHasShovel(bool v) => hasShovel = v;


    public void DigOnce()
    {
        if (!hasShovel || cam == null) return;

        if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)),
                            out var hit, rayDistance, terrainMask, QueryTriggerInteraction.Ignore))
        {
            var terrain = hit.collider.GetComponent<Terrain>();
            if (!terrain) return;

            ApplyDig(terrain, hit.point);
        }
    }

    void ApplyDig(Terrain terrain, Vector3 worldPoint)
    {
        var td = terrain.terrainData;
        Vector3 tPos = terrain.transform.position;
        
        float normX = Mathf.InverseLerp(tPos.x, tPos.x + td.size.x, worldPoint.x);
        float normZ = Mathf.InverseLerp(tPos.z, tPos.z + td.size.z, worldPoint.z);
        int hmRes   = td.heightmapResolution;

        int cx = Mathf.RoundToInt(normX * (hmRes - 1));
        int cz = Mathf.RoundToInt(normZ * (hmRes - 1));

        int r = Mathf.RoundToInt((brushRadiusMeters / td.size.x) * hmRes);
        r = Mathf.Max(1, r);
        int sx = Mathf.Clamp(cx - r, 0, hmRes - 1);
        int sz = Mathf.Clamp(cz - r, 0, hmRes - 1);
        int ex = Mathf.Clamp(cx + r, 0, hmRes - 1);
        int ez = Mathf.Clamp(cz + r, 0, hmRes - 1);
        int w = ex - sx + 1;
        int h = ez - sz + 1;

        var heights = td.GetHeights(sx, sz, w, h);

        float lowerN = Mathf.Abs(lowerAmountMeters) / td.size.y;
        float rr = r;

        for (int z = 0; z < h; z++)
        {
            for (int x = 0; x < w; x++)
            {
                int hx = sx + x;
                int hz = sz + z;

                float dx = hx - cx;
                float dz = hz - cz;
                float dist = Mathf.Sqrt(dx * dx + dz * dz);

                if (dist > rr) continue;

                float falloff = 1f;
                if (softFalloff)
                {
                   
                    float t = Mathf.Clamp01(dist / rr);
                    falloff = 0.5f * (1f + Mathf.Cos(t * Mathf.PI)); // 1→0
                }

                heights[z, x] = Mathf.Clamp01(heights[z, x] - lowerN * falloff);
            }
        }
        
        td.SetHeightsDelayLOD(sx, sz, heights);
    }
}
