using UnityEngine;

public class ShovelFinTrigger : MonoBehaviour
{
    bool isTriggerd = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!isTriggerd && GameManager.Instance.status == GameStatus.ShovelMission)
        {
            isTriggerd = true;

            GameManager.Instance.status = GameStatus.TapeMission;
            GameManager.Instance.SetCompleteUI();
            GameManager.Instance.OnMissionComplete();
        }
    }
}
