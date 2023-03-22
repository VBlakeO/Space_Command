using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Tooltip("Speed at which the aiming animatoin is played")]
    public float AimingAnimationSpeed = 10f;
    
    public WeaponSystem currentWeapon = null;
    public WeaponSystem[] allWeapons = null;
    [Space]

    public float changeDelay = 0.25f;

    private HudManager m_HudManager = null;
    private MeleeAttack m_MeleeAttack = null;
    private PlayerMovement m_PlayerMovement = null;

    private bool isAiming;

    public bool IsAiming { get => isAiming;}

    void Start()
    {
        m_HudManager = HudManager.m_Instance;
        m_MeleeAttack = MeleeAttack.m_Instance;
        m_PlayerMovement = PlayerMovement.m_Instance;
    }

    void Update()
    {
        if (currentWeapon)
        {
            isAiming = Input.GetKey(KeyCode.Mouse1) && !currentWeapon.IsReloading;
            m_HudManager.SetCrosshairState(!isAiming);

            if (Physics.Raycast(m_PlayerMovement.cameraFps.transform.position, m_PlayerMovement.cameraFps.transform.forward, out RaycastHit hit, 100, -1, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.GetComponentInParent<Health>() != null)
                    m_HudManager.SetCrosshairStyle(currentWeapon.TargetCrosshair);
                else
                    m_HudManager.SetCrosshairStyle(currentWeapon.DefaultCrosshair);
            }
            else
            {
                m_HudManager.SetCrosshairStyle(currentWeapon.DefaultCrosshair);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetKey(KeyCode.Alpha2))
            ChangeWeapon(0);

        if (Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetKey(KeyCode.Alpha1))
            ChangeWeapon(1);
    }

    public void MeleeAttackSkill()
    {
        if(m_MeleeAttack.Attack() && currentWeapon) 
          currentWeapon.InitiateMeleeAttack();
    }

    private void LateUpdate()
    {
        UpdateWeaponAiming();
    }

    public void ChangeWeapon(int selectedWeapon)
    {
        if (!allWeapons[selectedWeapon].canEquip)
            return;

        if (currentWeapon)
        {
            if (currentWeapon != allWeapons[selectedWeapon] && !currentWeapon.IsReloading)
            {
                WeaponSystem tempWeapon = currentWeapon;

                currentWeapon = allWeapons[selectedWeapon];

                currentWeapon.EquipWeapon();
                currentWeapon.gameObject.SetActive(true);

                tempWeapon.UnequipWeapon();
            }
        }
        else
        {
            currentWeapon = allWeapons[selectedWeapon];
            currentWeapon.EquipWeapon();

            currentWeapon.gameObject.SetActive(true);
        }
    }

    void UpdateWeaponAiming()
    {
        if (currentWeapon == null)
            return;

        if (isAiming)
        {
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, currentWeapon.AimOffset, AimingAnimationSpeed * Time.deltaTime);

            if (m_PlayerMovement.cameraFps.fieldOfView != currentWeapon.AimFOV)
                m_PlayerMovement.cameraFps.fieldOfView = Mathf.Lerp(m_PlayerMovement.cameraFps.fieldOfView, currentWeapon.AimFOV, AimingAnimationSpeed * Time.deltaTime);
        }
        else
        {
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, AimingAnimationSpeed * Time.deltaTime);

            if (m_PlayerMovement.cameraFps.fieldOfView != m_PlayerMovement.DefaltFOV)
                m_PlayerMovement.cameraFps.fieldOfView = Mathf.Lerp(m_PlayerMovement.cameraFps.fieldOfView, m_PlayerMovement.DefaltFOV, AimingAnimationSpeed * Time.deltaTime);
        }
    }
}