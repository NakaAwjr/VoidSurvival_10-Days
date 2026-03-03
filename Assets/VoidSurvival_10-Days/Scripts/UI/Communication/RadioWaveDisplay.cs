using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RadioWaveUI : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("設定")]
    public int points = 100;
    public float width = 500f; // UIの幅に合わせる
    public float speed = 15f;

    [Header("色設定")]
    public Color badColor = Color.red;    // 圏外
    public Color goodColor = Color.green; // 良好

    [Header("現在の状態")]
    [Range(0f, 1f)]
    public float signalStrength = 0f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points;

        // LineRendererをUIっぽく見せる設定
        lineRenderer.useWorldSpace = false; // 親（UI要素）からの相対座標にする
        lineRenderer.startWidth = 2f;
        lineRenderer.endWidth = 2f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // UI用シェーダー
        lineRenderer.widthCurve = AnimationCurve.Linear(0, 0.1f, 1, 0.1f); // 一定幅
    }

    void Update()
    {
        UpdateColor();
        DrawWave();
    }

    void UpdateColor()
    {
        // 信号強度(0-1)に合わせて、赤から緑へ補完
        Color currentColor = Color.Lerp(badColor, goodColor, signalStrength);

        // LineRendererの色を更新
        lineRenderer.startColor = currentColor;
        lineRenderer.endColor = currentColor;

        // 発光感を出したい場合は、マテリアルのEmissionをいじるのもアリ
    }

    void DrawWave()
    {
        for (int i = 0; i < points; i++)
        {
            float progress = (float)i / (points - 1);
            float x = (progress - 0.5f) * width;

            // 波形の合成
            float baseWave = Mathf.Sin(progress * 12f + Time.time * speed);
            float noise = (Mathf.PerlinNoise(progress * 8f, Time.time * speed * 0.5f) - 0.5f);

            // 強度が低いほどガタガタ、高いほど大きくクリアな波
            float y = (baseWave * signalStrength * 50f) + (noise * (1f - signalStrength) * 30f);

            // 端を絞る
            y *= Mathf.Sin(progress * Mathf.PI);

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}