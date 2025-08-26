using UnityEngine;

public class WarningTriggerCol : MonoBehaviour
{
    bool isTriggerd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.status == GameStatus.TreeMission)
            {
                Debug.LogWarning("�ɷ���!");
                isTriggerd = true;
                GameManager.Instance.SetAlarmText(GameManager.Instance.Panel_Warning);
            }
        }
    }
}
