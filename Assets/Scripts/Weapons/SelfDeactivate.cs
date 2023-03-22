using UnityEngine;

public class SelfDeactivate : MonoBehaviour
{
    public float deactivateTime;
    public Transform parent;
    public ParticleSystem particles = null;

    private void OnEnable()
    {
        if (particles)
            particles.Emit(1);

        Invoke("Deactivate", deactivateTime);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
        transform.parent = parent;
        transform.position = parent.position;
    }
}
