using UnityEngine;

public class HeadBobber : MonoBehaviour
{
    public float walkingBobbingSpeed = 14f;
    public float runningBobbingSpeed = 14f;
    public float idleSpeed = 3f;
    public float bobbingAmount = 0.05f;
    public bool IsWeapon;
    public WeaponManager m_WeaponManager;
    public PlayerMovement m_PlayerMovement;

    float defaultPosY = 0;
    float timer = 0;

    void Start()
    {
        defaultPosY = transform.localPosition.y;
    }

    void LateUpdate()
    {
        if (m_PlayerMovement.cantMove)
            return;

        if (IsWeapon)
            if (m_WeaponManager.IsAiming)
                return;


        if (Mathf.Abs(m_PlayerMovement.CurrentDir.x) > 0.2f || Mathf.Abs(m_PlayerMovement.CurrentDir.y) > 0.2f)
        {
            //Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.MoveTowards(transform.localPosition.y, defaultPosY, Time.deltaTime * idleSpeed), transform.localPosition.z);
        }  
    } 
}