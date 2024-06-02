using System;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private PlayerController player;
    private TintEffect dieEffect;

    private void Start()
    {
        player = transform.parent.GetComponent<PlayerController>();
        dieEffect = GetComponent<TintEffect>();

        if (player != null)
        {
            player.OnDied += OnDiedShader;
        }
    }

    private void OnDiedShader(object sender, EventArgs e)
    {
        if (dieEffect != null)
        {
            dieEffect.TriggerTintEffect();
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnDied -= OnDiedShader;
        }
    }
}