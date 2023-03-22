using UnityEngine;

public class DoubleDoorSystem : MonoBehaviour
{
    public bool open = false;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeDoorState()
    {
        open = !open;

        anim.speed = 1;

        if (open)
            anim.Play("OpeningDoor", 0, 0);
        else
            anim.Play("ClosingDoor", 0, 0);
    }

    public void StopAnimSpeed()
    {
        anim.speed = 0;
    }
}
