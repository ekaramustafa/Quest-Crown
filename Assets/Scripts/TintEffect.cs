using System;
using UnityEngine;

public class TintEffect : MonoBehaviour
{
    private const string TINT_SHADER = "_Tint";

    private Material material;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color materialTintColor;
    [SerializeField] private float tintFadeSpeed = 2f;

    private void Awake()
    {
        materialTintColor = new Color(1, 0, 0, 0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;

        if (!material.HasProperty(TINT_SHADER))
        {
            Debug.LogError($"Material on {gameObject.name} does not have a {TINT_SHADER} property.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (materialTintColor.a > 0f)
        {
            materialTintColor.a = Mathf.Clamp01(materialTintColor.a - tintFadeSpeed * Time.deltaTime);
            material.SetColor(TINT_SHADER, materialTintColor);
        }
    }

    public void TriggerTintEffect()
    {
        materialTintColor.a = 1f;

    }

    public void TriggerTintEffect(float magnitude)
    {
        materialTintColor.a = magnitude;
    }
}
