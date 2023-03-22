using UnityEngine;

public class LookToTarget : StateMachineBehaviour
{
    private NewBase_AI m_base;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_base = animator.transform.GetComponent<NewBase_AI>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var targetRotation = Quaternion.LookRotation(m_base.m_target.position - m_base.transform.position);
        m_base.transform.rotation = Quaternion.Slerp(m_base.transform.rotation, targetRotation, m_base.rotationSpeed * Time.deltaTime);
    }
}
