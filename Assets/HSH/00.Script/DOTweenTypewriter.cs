using UnityEngine;
using DG.Tweening;
using TMPro;

public class DOTweenTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float typingDuration = 2f;
    [SerializeField] private Ease easeType = Ease.Linear;

    private string fullText;
    private Tween typingTween;

    private void OnEnable()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();

        fullText = textComponent.text;
        textComponent.maxVisibleCharacters = 0;

        // DOTween으로 maxVisibleCharacters 애니메이션
        typingTween = DOTween.To(
            () => textComponent.maxVisibleCharacters,
            x => textComponent.maxVisibleCharacters = x,
            fullText.Length,
            typingDuration
        ).SetEase(easeType);
    }

    private void OnDisable()
    {
        if (typingTween != null && typingTween.IsActive())
        {
            typingTween.Kill();
            textComponent.maxVisibleCharacters = fullText.Length;
        }
    }
}
