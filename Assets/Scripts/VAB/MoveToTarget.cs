using UnityEngine.AI;
using UnityEngine;

public class MoveToTarget : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Zombie_AI zombieBase;
    private Enemy_AI enemyBase;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        zombieBase  = animator.transform.GetComponent<Zombie_AI>();
        enemyBase  = animator.transform.GetComponent<Enemy_AI>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (zombieBase)
        {
            if (zombieBase.m_IsDead)
                return;

            if (zombieBase.m_target)
                agent.SetDestination(zombieBase.m_target.position);
        }

        if (enemyBase)
        {
            if (enemyBase.m_IsDead)
                return;

            if (enemyBase.m_goal)
                agent.SetDestination(enemyBase.m_goal.position);
        }
    }
}
