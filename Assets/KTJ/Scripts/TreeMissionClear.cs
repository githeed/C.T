using UnityEngine;
using System.Collections;

public class TreeMissionClear : MonoBehaviour
{
    public Rigidbody treePart1;
    public Rigidbody treePart2;
    
    public float destroyDelay = 3f;

    public bool treesCleared= false;

    private bool waitStarted = false;

    void Update()
    {
        if (treesCleared || waitStarted) return;

        if (IsDropped(treePart1) && IsDropped(treePart2))
        {
            StartCoroutine(ClearAfterDelay());
        }
    }

    bool IsDropped(Rigidbody rb)
    {
        if (!rb) return false;
        return rb.useGravity && !rb.isKinematic;
    }

    IEnumerator ClearAfterDelay()
    {
        waitStarted = true;
        yield return new WaitForSeconds(destroyDelay);

        if (treePart1)
            treePart1.gameObject.SetActive(false);
        if (treePart2)
            treePart2.gameObject.SetActive(false);


        treesCleared = true;

    }
}