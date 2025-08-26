using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIVideoPlayer : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public RenderTexture renderTexture;

    void Start()
    {
        // Render Texture ���� (�ڵ��)
        renderTexture = new RenderTexture(1920, 1080, 16);

        // ����
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture;

        // ���
        videoPlayer.Play();
    }
}
