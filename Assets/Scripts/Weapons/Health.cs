using UnityEngine.Events;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float MaxHealth = 100f;

    [Tooltip("Health ratio at which the critical health vignette starts appearing")]
    public float CriticalHealthRatio = 0.3f;

    public UnityAction<float, GameObject> OnDamaged;
    public UnityAction<float> OnHealed;
    public UnityAction ShieldDamage;
    public UnityAction OnDie;

    public float CurrentHealth;// { get; set; }
    public bool invincible;
    public bool CanPickup() => CurrentHealth < MaxHealth;

    public float GetRatio() => CurrentHealth / MaxHealth;
    public bool IsCritical() => GetRatio() <= CriticalHealthRatio;

    public bool m_IsDead;

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void Heal(float healAmount)
    {
        float healthBefore = CurrentHealth;
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        float trueHealAmount = CurrentHealth - healthBefore;
        if (trueHealAmount > 0f)
        {
            OnHealed?.Invoke(trueHealAmount);
        }
    }

    public virtual void TakeDamage(float damage, GameObject damageSource)
    {
        if (invincible)
            return;

        float healthBefore = CurrentHealth;
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        // call OnDamage action
        float trueDamageAmount = healthBefore - CurrentHealth;
        if (trueDamageAmount > 0f)
        {
            OnDamaged?.Invoke(trueDamageAmount, damageSource);
        }

        HandleDeath();
    }

    public void Kill()
    {
        CurrentHealth = 0f;

        OnDamaged?.Invoke(MaxHealth, null);

        HandleDeath();
    }

    protected virtual void HandleDeath()
    {
        if (m_IsDead)
            return;

        if (CurrentHealth <= 0f)
        {
            m_IsDead = true;
            OnDie?.Invoke();
        }
    }
}