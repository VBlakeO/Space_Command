using UnityEngine;

public class Zombie_AI : NewBase_AI
{
    protected override void OnEnable()
    {
        OnDamaged += TakeHit;
    }

    public void FollowPalyer(bool state)
    {
        anim.SetBool("FollowPlayer", state);
    }

    private void FixedUpdate()
    {
        if (m_IsDead)
            return;

        distance = Vector3.Distance(transform.position, m_target.position);

        anim.SetFloat("Distance", distance);
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void TakeHit(float t, GameObject obj)
    {
        anim.Play("ZombieHit", 0, 0);
    }
}
