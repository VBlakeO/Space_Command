using UnityEngine.AI;
using UnityEngine;

public class Idle_AI : StateMachineBehaviour
{
    private NavMeshAgent agent;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.transform.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
    }
}
