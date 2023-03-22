using UnityEngine;
using UnityEngine.Events;

public class TargetManager : MonoBehaviour
{
    public int targets = 3;
    public int fallenTargets = 0;
    public WeaponManager m_WeaponManager = null;
    public WeaponSystem m_Rifle = null;
    public UnityEvent OnCompleteFirstEvent;
    public UnityEvent OnCompleteSecondEvent;

    public void AddFallenTarget()
    {
        fallenTargets++;

        if(fallenTargets == targets)
            OnCompleteFirstEvent?.Invoke();

        if(fallenTargets == 6)
            OnCompleteSecondEvent?.Invoke();
    }
}
