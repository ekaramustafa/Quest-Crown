using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TintEffect : MonoBehaviour
{
    private const string TINT_SHADER = "_Tint";

    private Material material;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color materialTintColor;
    [SerializeField] private float tintFadeSpeed = 2f;

    [System.Serializable]
    public class TintEvent : UnityEvent { }
    [SerializeField] private TintEvent onTintTrigger;

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

        onTintTrigger.AddListener(OnTintTriggered);
    }

    private void OnTintTriggered()
    {
        materialTintColor.a = 1f;
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
        onTintTrigger.Invoke();
    }
}