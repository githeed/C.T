using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PipeGameTrigger : MonoBehaviour
{
    [Header("Visual")]
    public GameObject visualObject;  // 컴퓨터 모니터 등 시각적 오브젝트
    public Material normalMaterial;
    public Material highlightMaterial;

    private PipeGameInteraction gameInteraction;
    private MeshRenderer meshRenderer;

    void Start()
    {
        // Collider 설정
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // PipeGameInteraction 컴포넌트 추가
        gameInteraction = GetComponent<PipeGameInteraction>();
        if (gameInteraction == null)
        {
            gameInteraction = gameObject.AddComponent<PipeGameInteraction>();
        }

        // MeshRenderer 찾기
        if (visualObject != null)
        {
            meshRenderer = visualObject.GetComponent<MeshRenderer>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && meshRenderer != null && highlightMaterial != null)
        {
            meshRenderer.material = highlightMaterial;
            

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
        if (other.CompareTag("Player") && meshRenderer != null && normalMaterial != null)
        {
            meshRenderer.material = normalMaterial;

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