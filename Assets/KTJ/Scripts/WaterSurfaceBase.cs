using UnityEngine;

/// 씬에 하나 배치해서 '물의 높이/노말/흐름'을 제공
public abstract class WaterSurfaceBase : MonoBehaviour
{
    public abstract float  GetHeight(Vector3 worldPos);
    public virtual  Vector3 GetNormal(Vector3 worldPos) => Vector3.up;
    public virtual  Vector3 GetFlow(Vector3 worldPos)   => Vector3.zero;
}
