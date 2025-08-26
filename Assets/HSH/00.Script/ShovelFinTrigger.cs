using UnityEngine;

public class ShovelFinTrigger : MonoBehaviour
{
    bool isTriggerd = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!isTriggerd)
        {
            isTriggerd = true;

            GameManager.Instance.status = GameStatus.TapeMission;
            GameManager.Instance.SetCompleteUI();
        }
    }

}
