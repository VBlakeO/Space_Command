using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public float damage = 30;
    Zombie_AI zombieAI;

    private void Start()
    {
        zombieAI = GetComponentInParent<Zombie_AI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (zombieAI.m_IsDead)
            return;

        if (other.GetComponentInParent<PlayerMovement>())
            other.GetComponentInParent<Health>().TakeDamage(damage, null);
    }
}
