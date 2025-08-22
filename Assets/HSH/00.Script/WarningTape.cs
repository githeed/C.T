using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class WarningTape : MonoBehaviour
{
    [Header("Tape Physical Properties")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float tapeWidth = 0.08f; // ���� ������ �ʺ� (8cm)
    [SerializeField] private float sagAmount = 0.3f; // �߷¿� ���� ó��
    [SerializeField] private int segmentCount = 30; // � �ε巯��
    [SerializeField] private float windStrength = 0.02f; // �ٶ� ȿ��
    [SerializeField] private float windSpeed = 2f; // �ٶ� �ӵ�

    [Header("Visual Style")]
    [SerializeField] private bool useStripePattern = true; // �밢�� �ٹ��� ����
    [SerializeField] private Color primaryColor = new Color(1f, 0.9f, 0f, 1f); // �����
    [SerializeField] private Color secondaryColor = Color.black; // ������
    [SerializeField] private float stripeWidth = 0.5f; // �ٹ��� �ʺ�
    [SerializeField] private float stripeAngle = 45f; // �ٹ��� ����

    [Header("Material Settings")]
    [SerializeField] private Material tapeMaterial; // Ŀ���� ��Ƽ����
    [SerializeField] private float materialGlossiness = 0.3f; // ����
    [SerializeField] private bool doubleSided = true; // ��� ������

    [Header("Animation")]
    [SerializeField] private bool enableWaving = true; // �ٶ��� ��鸲
    [SerializeField] private AnimationCurve sagCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // ó�� Ŀ��

    private LineRenderer lineRenderer;
    private Material instanceMaterial;
    private Vector3[] basePositions;
    private float windPhase = 0f;

    void Start()
    {
        SetupLineRenderer();
        CreateTapeMaterial();
        InitializeBasePositions();
    }

    void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Line Renderer ����
        lineRenderer.positionCount = segmentCount;
        lineRenderer.startWidth = tapeWidth;
        lineRenderer.endWidth = tapeWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.textureMode = LineTextureMode.Tile; // �ؽ�ó Ÿ�ϸ�
        lineRenderer.alignment = LineAlignment.View; // �׻� ī�޶� ����

        // �� �κ��� �簢������ (�ձ� ĸ ����)
        lineRenderer.numCapVertices = 0; // 0���� �����ϸ� �簢�� ��
        lineRenderer.numCornerVertices = 0; // �𼭸��� ������

        // �׸��� ����
        lineRenderer.shadowCastingMode = doubleSided ?
            UnityEngine.Rendering.ShadowCastingMode.TwoSided :
            UnityEngine.Rendering.ShadowCastingMode.On;
        lineRenderer.receiveShadows = true;
    }

    void CreateTapeMaterial()
    {
        if (tapeMaterial != null)
        {
            instanceMaterial = new Material(tapeMaterial);
        }
        else
        {
            // �⺻ ��Ƽ���� ���� (URP Shader ���)
            instanceMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (instanceMaterial == null) // URP�� ���� ��� ����
            {
                instanceMaterial = new Material(Shader.Find("Sprites/Default"));
            }
            instanceMaterial.name = "WarningTapeMaterial";
        }

        // ��Ƽ���� �Ӽ� ����
        ConfigureMaterial();

        lineRenderer.material = instanceMaterial;
    }

    void ConfigureMaterial()
    {
        if (instanceMaterial == null) return;

        // �⺻ ���� ����
        instanceMaterial.color = primaryColor;

        // URP Lit ���̴��� �Ӽ� ����
        if (instanceMaterial.shader.name.Contains("Universal Render Pipeline"))
        {
            // URP Metallic/Smoothness ����
            instanceMaterial.SetFloat("_Smoothness", materialGlossiness);
            instanceMaterial.SetFloat("_Metallic", 0f);
        }
        else
        {
            // Legacy Standard ���̴��� (����)
            instanceMaterial.SetFloat("_Glossiness", materialGlossiness);
            instanceMaterial.SetFloat("_Metallic", 0f);
        }

        // ��� ������
        if (doubleSided)
        {
            instanceMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }

        // �ٹ��� ���� �ؽ�ó ����
        if (useStripePattern)
        {
            Texture2D stripeTexture = CreateStripeTexture();
            instanceMaterial.mainTexture = stripeTexture;
            instanceMaterial.SetTextureScale("_MainTex", new Vector2(10f, 1f));
        }

        // �ణ�� ���� (�ɼ�)
        if (primaryColor.a < 1f || secondaryColor.a < 1f)
        {
            // URP Transparent ����
            if (instanceMaterial.shader.name.Contains("Universal Render Pipeline"))
            {
                // Surface Type�� Transparent�� ����
                instanceMaterial.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
                instanceMaterial.SetFloat("_Blend", 0); // 0 = Alpha, 1 = Premultiply, 2 = Additive, 3 = Multiply

                // Render Face ����
                instanceMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

                // ���� ��� ����
                instanceMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                instanceMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                instanceMaterial.SetInt("_ZWrite", 0);

                // ���� ť ����
                instanceMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                // Ű���� Ȱ��ȭ
                instanceMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                instanceMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            else
            {
                // Legacy Transparent ������ ��� ����
                instanceMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                instanceMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                instanceMaterial.SetInt("_ZWrite", 0);
                instanceMaterial.DisableKeyword("_ALPHATEST_ON");
                instanceMaterial.EnableKeyword("_ALPHABLEND_ON");
                instanceMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                instanceMaterial.renderQueue = 3000;
            }
        }
    }

    Texture2D CreateStripeTexture()
    {
        int textureSize = 256;
        Texture2D texture = new Texture2D(textureSize, textureSize);

        // �밢�� �ٹ��� ���� ����
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                // �밢�� ���
                float diagonal = (x + y * Mathf.Tan(stripeAngle * Mathf.Deg2Rad)) % (textureSize * stripeWidth);
                bool isStripe = diagonal < (textureSize * stripeWidth * 0.5f);

                Color pixelColor = isStripe ? primaryColor : secondaryColor;
                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Bilinear;

        return texture;
    }

    void InitializeBasePositions()
    {
        basePositions = new Vector3[segmentCount];
        UpdateBasePositions();
    }

    void UpdateBasePositions()
    {
        if (startPoint == null || endPoint == null) return;

        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // ī�׳��� � (���� �þ��� ������ ���)
            float sag = CalculateCatenarySag(t);
            point.y -= sag * sagAmount;

            basePositions[i] = point;
        }
    }

    float CalculateCatenarySag(float t)
    {
        // ī�׳��� � �ٻ� (�ְ��ڻ��� �Լ�)
        // �߾��� ���� ���� ó������
        float x = (t - 0.5f) * 2f; // -1 to 1 ������ ��ȯ
        float cosh = (Mathf.Exp(x) + Mathf.Exp(-x)) / 2f;
        float catenary = cosh - 1f;

        // AnimationCurve�� �߰� ����
        if (sagCurve != null && sagCurve.length > 0)
        {
            catenary *= sagCurve.Evaluate(t);
        }
        else
        {
            // �⺻ ������ Ŀ��
            catenary *= Mathf.Sin(t * Mathf.PI);
        }

        return catenary;
    }

    void Update()
    {
        if (startPoint != null && endPoint != null)
        {
            UpdateTapePosition();

            if (enableWaving)
            {
                ApplyWindEffect();
            }
        }
    }

    void UpdateTapePosition()
    {
        UpdateBasePositions();
    }

    void ApplyWindEffect()
    {
        windPhase += Time.deltaTime * windSpeed;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 position = basePositions[i];

            // �ٶ��� ���� �¿� ��鸲
            float t = i / (float)(segmentCount - 1);
            float windEffect = Mathf.Sin(windPhase + t * Mathf.PI * 2f) * windStrength;

            // �߾� �κ��� �� ���� ��鸮����
            float centerWeight = Mathf.Sin(t * Mathf.PI);
            windEffect *= centerWeight;

            // ���� �������θ� ��鸲
            Vector3 windDirection = Vector3.Cross(Vector3.up,
                (endPoint.position - startPoint.position).normalized);
            position += windDirection * windEffect;

            // �ణ�� ���� �����ӵ� �߰�
            position.y += Mathf.Sin(windPhase * 1.5f + t * Mathf.PI) * windStrength * 0.3f * centerWeight;

            lineRenderer.SetPosition(i, position);
        }
    }

    // ���� �޼����
    public void SetPoints(Transform newStartPoint, Transform newEndPoint)
    {
        startPoint = newStartPoint;
        endPoint = newEndPoint;
        InitializeBasePositions();
    }

    public void SetPoints(Vector3 startPosition, Vector3 endPosition)
    {
        if (startPoint == null)
        {
            GameObject startObj = new GameObject("TapeStart");
            startObj.transform.position = startPosition;
            startObj.transform.parent = transform;
            startPoint = startObj.transform;
        }
        else
        {
            startPoint.position = startPosition;
        }

        if (endPoint == null)
        {
            GameObject endObj = new GameObject("TapeEnd");
            endObj.transform.position = endPosition;
            endObj.transform.parent = transform;
            endPoint = endObj.transform;
        }
        else
        {
            endPoint.position = endPosition;
        }

        InitializeBasePositions();
    }

    public void SetTapeStyle(Color primary, Color secondary, float stripeWidthValue)
    {
        primaryColor = primary;
        secondaryColor = secondary;
        stripeWidth = stripeWidthValue;

        if (instanceMaterial != null)
        {
            ConfigureMaterial();
        }
    }

    public void SetPhysicalProperties(float width, float sag, float wind)
    {
        tapeWidth = width;
        sagAmount = sag;
        windStrength = wind;

        if (lineRenderer != null)
        {
            lineRenderer.startWidth = tapeWidth;
            lineRenderer.endWidth = tapeWidth;
        }
    }

    // ������ ��Ÿ�� ������
    public enum TapeStyle
    {
        YellowBlack,    // �����-������ (�Ϲ� ���)
        RedWhite,       // ������-��� (����)
        BlueWhite,      // �Ķ���-��� (����)
        OrangeWhite,    // ��Ȳ��-��� (����)
        GreenWhite      // ���-��� (����)
    }

    public void ApplyPresetStyle(TapeStyle style)
    {
        switch (style)
        {
            case TapeStyle.YellowBlack:
                SetTapeStyle(new Color(1f, 0.9f, 0f), Color.black, 0.5f);
                break;
            case TapeStyle.RedWhite:
                SetTapeStyle(Color.red, Color.white, 0.5f);
                break;
            case TapeStyle.BlueWhite:
                SetTapeStyle(Color.blue, Color.white, 0.5f);
                break;
            case TapeStyle.OrangeWhite:
                SetTapeStyle(new Color(1f, 0.5f, 0f), Color.white, 0.5f);
                break;
            case TapeStyle.GreenWhite:
                SetTapeStyle(Color.green, Color.white, 0.5f);
                break;
        }
    }

    // ����� (�����Ϳ��� �ð�ȭ)
    void OnDrawGizmos()
    {
        if (startPoint != null && endPoint != null)
        {
            // ���� ���ἱ
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawLine(startPoint.position, endPoint.position);

            // �������� ����
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPoint.position, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(endPoint.position, 0.1f);

            // ó�� ǥ��
            if (Application.isPlaying && basePositions != null && basePositions.Length > 0)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
                for (int i = 0; i < basePositions.Length - 1; i++)
                {
                    Gizmos.DrawLine(basePositions[i], basePositions[i + 1]);
                }
            }
        }
    }
}