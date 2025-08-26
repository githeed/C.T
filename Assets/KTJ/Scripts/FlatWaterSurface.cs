using UnityEngine;

public class FlatWaterSurface : WaterSurfaceBase
{
        public float waterLevel = 0f;
        public override float GetHeight(Vector3 worldPos) => waterLevel;
}
