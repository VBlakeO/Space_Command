using UnityEngine;

public class Target : MonoBehaviour
{
    public int life = 3;
    
    public Animator anim;
    private TargetManager m_TargetManager = null;

    void Start()
    {
        m_TargetManager = GetComponentInParent<TargetManager>();
    }

    public void RotateTarget()
    {
        if (life > 1)
        {
            anim.Play("Hit", 0, 0);
            life--;
        }
        else if (life == 1)
        {
            life--;
            anim.Play("Die", 0, 0);
            m_TargetManager.AddFallenTarget();
        }
        else
        {
            anim.Play("Die", 0, 0);
        }
    }
}
