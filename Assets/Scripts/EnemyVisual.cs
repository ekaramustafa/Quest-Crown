using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private EnemyController enemy;
    private TintEffect damageTakenEffect;

    [Header("Tunable parameters")]
    [SerializeField] private float effectMagnitude;


    private void Awake()
    {
        damageTakenEffect = GetComponent<TintEffect>();
    }
    private void Start()
    {
        enemy = transform.parent.GetComponent<EnemyController>();
        
        enemy.OnDamageTaken += OnDamageTaken;
    }

    private void OnDamageTaken(object sender, System.EventArgs e)
    {
        damageTakenEffect.TriggerTintEffect(effectMagnitude);
    }
}
