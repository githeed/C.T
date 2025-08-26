using UnityEngine;

public class ShovelFinTrigger : MonoBehaviour
{
    bool isTriggerd = false;
    public Rigidbody handShovelrb;
    public BoxCollider handShovelBoxCollider;
    public TerrainDigger terrainDigger;

    private void OnTriggerEnter(Collider other)
    {
        if(!isTriggerd)
        {
            isTriggerd = true;

            GameManager.Instance.status = GameStatus.TapeMission;
            GameManager.Instance.SetCompleteUI();
            GameManager.Instance.OnMissionComplete();

            terrainDigger.hasShovel = false;
            handShovelBoxCollider.enabled = true;
            handShovelrb.isKinematic = false;
            handShovelrb.useGravity = true;
            handShovelrb.gameObject.transform.SetParent(null);
        }
    }
}
