using UnityEngine;

public enum BehaviorType { DEFENDER, ATTACKER}
public class Enemy_AI : NewBase_AI
{
    public Transform m_goal;
    public Transform m_cover;

    public BehaviorType behaviorType = BehaviorType.ATTACKER;

    public AI_WeaponSystem weaponSystem;
    public float minDistanceToShoot = 9f;
    public Transform myForward = null;

    public bool playerInRoon = false;
    private float rotationRatio;

    public void SetEnemyCover(Transform cover)
    {
        m_cover = cover;
    }

    private void FixedUpdate()
    {
        if (m_IsDead)
            return;

        distance = Vector3.Distance(transform.position, m_goal.position);

        Vector3 toPlayer = m_target.position - transform.position;
        toPlayer.y = 0;


        rotationRatio = Vector3.Dot(toPlayer.normalized, transform.forward);

        if (rotationRatio > 0.985f)
            myForward.LookAt(m_target);

        anim.SetFloat("Distance", distance);
        anim.SetFloat("Speed", agent.velocity.magnitude);

        if (behaviorType == BehaviorType.ATTACKER)
        {
            if (distance < minDistanceToShoot)
            {
               weaponSystem.attack = true;
               anim.SetBool("PlayerIsTarget", true);
            }
            else
            {
                weaponSystem.attack = false;
                anim.SetBool("PlayerIsTarget", false);
            }
        }
        else
        {

            if (Vector3.Distance(transform.position, m_target.position) < 15f)
                SetBehaviorType(BehaviorType.ATTACKER);

            if (distance < 1.5f)
            {
                if(playerInRoon)
                {
                    weaponSystem.attack = true;
                    anim.SetBool("PlayerIsTarget", true);
                }
                else
                {
                    weaponSystem.attack = false;
                    anim.SetBool("PlayerIsTarget", false);
                }
            }
            else
            {
                weaponSystem.attack = false;
                anim.SetBool("PlayerIsTarget", false);
            }
        }
    }

    protected override void Ragdoll()
    {
        base.Ragdoll();
        weaponSystem.attack = false;
    }

    public void SetBehaviorType(BehaviorType behavior)
    {
        behaviorType = behavior;

        if (behavior == BehaviorType.ATTACKER)
            m_goal = playController.transform;
        else
            m_goal = m_cover;
    }
}
