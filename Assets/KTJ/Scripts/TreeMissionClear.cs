using UnityEngine;
using System.Collections;

public class TreeMissionClear : MonoBehaviour
{
    public Rigidbody treePart1;
    public Rigidbody treePart2;
    public Rigidbody handAxe;
    public BoxCollider handAxeCol;
    public PlayerChopping playerChopping;

    public float destroyDelay = 3f;

    public bool treesCleared = false;

    bool waitStarted = false;
    bool finalized = false;

    void Awake()
    {
        if (handAxeCol) handAxeCol.enabled = false;
        if (handAxe)
        {
            handAxe.isKinematic = true;
            handAxe.useGravity  = false;
        }
    }

    void Update()
    {
        if (!waitStarted && IsDropped(treePart1) && IsDropped(treePart2))
            StartCoroutine(ClearAfterDelay());
    }

    bool IsDropped(Rigidbody rb)
    {
        return rb && rb.useGravity && !rb.isKinematic;
    }

    IEnumerator ClearAfterDelay()
    {
        waitStarted = true;

        yield return new WaitForSeconds(destroyDelay);

        if (treePart1) treePart1.gameObject.SetActive(false);
        if (treePart2) treePart2.gameObject.SetActive(false);

        treesCleared = true;

        // ▼ 여기서 바로 마무리 처리
        FinalizeMission();
        // 이 스크립트 더이상 필요 없으면 비활성화
        // enabled = false;
    }

    void FinalizeMission()
    {
        if (finalized) return;
        finalized = true;

        // 플레이어 채핑 종료 및 비활성
        if (playerChopping)
        {
            playerChopping.ChopEnd();   // 이동락/비주얼 복구
            playerChopping.enabled = false;
        }

        // 손도끼 드롭
        if (handAxe)
        {
            handAxe.transform.SetParent(null, true);
            handAxe.isKinematic = false;
            handAxe.useGravity  = true;
            handAxe.WakeUp();
        }
        if (handAxeCol) handAxeCol.enabled = true;
    }
}