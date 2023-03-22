using UnityEngine;

public class MeleeAttackDetection : MonoBehaviour
{
    public bool detected = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponentInParent<Damageable>())
        {
            detected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.transform.GetComponentInParent<Damageable>()))
        {
            detected = false;
        }
    }
}
