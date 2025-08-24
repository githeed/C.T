using UnityEngine;

public class PipeGameTrigger : MonoBehaviour
{
    [Header("Visual")]
    public GameObject visualObject;  // ��ǻ�� ����� �� �ð��� ������Ʈ

    private PipeGameInteraction gameInteraction;
    private MeshRenderer meshRenderer;
    public Outline outline;

    void Start()
    {
        // Collider ����
        Collider col = GetComponent<SphereCollider>();
        col.isTrigger = true;

        //// Quick Outline ������Ʈ �߰�
        //outline = visualObject.GetComponent<Outline>();
        //if (outline == null)
        //    outline = visualObject.AddComponent<Outline>();

        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f;
        outline.enabled = false; // ������ ���� ��Ȱ��ȭ

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            outline.enabled = true;

            PipeGameInteraction pi = other.GetComponent<PipeGameInteraction>();
            if(pi)
            {
                pi.isInRange = true;
                pi.currentTrigger = gameObject;
                pi.ShowPrompt();
            }
                
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            outline.enabled = false;

            PipeGameInteraction pi = other.GetComponent<PipeGameInteraction>();
            if (pi)
            {
                pi.isInRange = false;
                pi.currentTrigger = null;
                pi.HidePrompt();
            }
        }
    }
}