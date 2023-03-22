﻿using UnityEngine;

public class Damageable : MonoBehaviour
{
    [Tooltip("Multiplier to apply to the received damage")]
    public float DamageMultiplier = 1f;

    [Range(0, 1)]
    [Tooltip("Multiplier to apply to self damage")]
    public float SensibilityToSelfdamage = 0.5f;

    public Health Health { get; private set; }

    void Awake()
    {
        if (Health == null)
            Health = GetComponentInParent<Health>();
    }

    public void InflictDamage(float damage, bool isExplosionDamage, GameObject damageSource)
    {
        if (Health != null)
        {
            var totalDamage = damage;

            if (!isExplosionDamage)
            {
                totalDamage *= DamageMultiplier;
            }

            if (Health.gameObject == damageSource)
            {
                totalDamage *= SensibilityToSelfdamage;
            }

            // apply the damages
            Health.TakeDamage(totalDamage, damageSource);
        }
    }
}