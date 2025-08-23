using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class PipeTileUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private PipePuzzleManager manager;
    private Vector2Int gridPosition;
    private PipeTile tileData;
    private Image image;
    private Button button;

    [Header("Visual Effects")]
    public float hoverScale = 1.1f;
    public float clickScale = 0.95f;

    void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
            Debug.Log($"Image ������Ʈ �߰�: {gameObject.name}");
        }

        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
            Debug.Log($"Button ������Ʈ �߰�: {gameObject.name}");
        }

        // ����ĳ��Ʈ Ÿ�� ����
        image.raycastTarget = true;
    }

    public void Initialize(PipePuzzleManager mgr, Vector2Int pos, PipeTile tile)
    {
        manager = mgr;
        gridPosition = pos;
        tileData = tile;

        Debug.Log($"Ÿ�� �ʱ�ȭ: ��ġ({pos.x}, {pos.y}), Ÿ��: {tile.type}");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager == null)
        {
            Debug.LogError("Manager�� �������� �ʾҽ��ϴ�!");
            return;
        }

        if (tileData != null &&
            tileData.type != PipeType.Empty &&
            tileData.type != PipeType.Start &&
            tileData.type != PipeType.End)
        {
            Debug.Log($"Ÿ�� Ŭ��: ({gridPosition.x}, {gridPosition.y})");
            manager.RotateTile(gridPosition);

            // Ŭ�� �ִϸ��̼� (LeanTween ����)
            StartCoroutine(ClickAnimation());
        }
    }

    IEnumerator ClickAnimation()
    {
        transform.localScale = Vector3.one * clickScale;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tileData != null && tileData.type != PipeType.Empty)
        {
            StartCoroutine(ScaleAnimation(Vector3.one * hoverScale, 0.2f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(ScaleAnimation(Vector3.one, 0.2f));
    }

    IEnumerator ScaleAnimation(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}