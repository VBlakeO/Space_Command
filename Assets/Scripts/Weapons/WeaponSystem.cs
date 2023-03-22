using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponType
{ Manual, Automatic }

public class WeaponSystem : MonoBehaviour
{
    [Header("Crosshair")]
    public CrosshairData DefaultCrosshair;
    public CrosshairData TargetCrosshair;

    [Header("Weapon Setting")]
    public float weaponRange = 0f;
    public float weaponDamage = 30f;
    public float delayBetweenShots = 0f;
    public WeaponType weaponType = WeaponType.Manual;
    public LayerMask attainableLayers;

    [Header("Reload")]
    public float ReloadTime = 0f;
    public bool AutomaticReload = true;

    [Header("Aim")]
    public float AimFOV = 55f;
    public Vector3 AimOffset;

    [Header("Ammo Parameters")]
    public int MaxAmmo = 15;

    [Header("Hud")]
    public TextMeshProUGUI AmmoText = null;

    [Header("VFX")]
    public Camera WeaponCamera = null;
    public Camera MainCamera = null;
    public Transform BulletPoint = null;
    public Animator WeaponAnimator = null;
    public ParticleSystem MuzzleEffect = null;
    public TrailRenderer TacerEffect = null;
    public Transform poitT = null;

    [Header("Recoil")]
    public Vector2 recoilMinMax;
    public float recoilY;

    [Header("SFX")]
    public AudioClip ShootSfx;
    public AudioClip ReloadSfx;
    public AudioClip ChangeWeaponSfx;
    private AudioSource ShootAudioSource;

    public UnityAction OnShoot;

    public bool IsReloading { get; private set; }
    public bool IsWeaponActive { get; private set; }

    public bool canEquip = true;

    private int shotCounter = 0;

    public PoolingManager m_PoolingManager = null;
    public PlayerMovement m_PlayerMovement = null;

    private int currentAmmo = 0;
    private float currentRate = 0f;

    private MeleeAttack m_MeleeAttack = null;
    private HudManager m_HudManager = null;
    
    private bool meleeAttack = false;

    [HideInInspector]
    public bool onHands = false;



    private bool CanShoot()
    {
        return currentAmmo > 0 && !IsReloading && currentRate >= delayBetweenShots && !meleeAttack && onHands;
    }

    public void SetCanEquip(bool state)
    {
        canEquip = state;
    }

    private void Awake()
    {
        ShootAudioSource = GetComponent<AudioSource>();
        currentAmmo = MaxAmmo;
        currentRate = delayBetweenShots;
        HandleHud();

        OnShoot += HandleHud;
    }

    private void Start()
    {
        m_HudManager = HudManager.m_Instance;
        m_MeleeAttack = MeleeAttack.m_Instance;
        m_MeleeAttack.OnPunchingEnd += FinishMeleeAttack;
    }

    private void OnEnable()
    {
        currentRate = delayBetweenShots;
        if (m_HudManager)
        {
            m_HudManager.SetCrosshairState(true);
            m_HudManager.SetCrosshairStyle(DefaultCrosshair);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentAmmo == 0)
            {
                if (CheckReload())
                    Reload();
            }
            if (weaponType == WeaponType.Manual)
            {
                TryShoot();
            }

        }

        if (Input.GetKey(KeyCode.Mouse0) && weaponType == WeaponType.Automatic)
        {
            TryShoot();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && weaponType == WeaponType.Automatic)
        {
            shotCounter = 0;
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < MaxAmmo)
        {
            if (CheckReload())
                Reload();
        }
    }

    private void FixedUpdate()
    {
        if (currentRate < delayBetweenShots)
            currentRate += Time.deltaTime;
    }

    private bool TryShoot()
    {
        if (!CanShoot())
            return false;

        var tracer = Instantiate(TacerEffect, BulletPoint.transform.position, Quaternion.identity);
        tracer.AddPosition(BulletPoint.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out hit, weaponRange, attainableLayers, QueryTriggerInteraction.Ignore))
        {
            Damageable damageable = hit.transform.GetComponentInParent<Damageable>();
            Health health = hit.transform.GetComponentInParent<Health>();
            if (damageable)
                damageable.InflictDamage(Random.Range(weaponDamage - 10, weaponDamage), false, m_PlayerMovement.gameObject);

            Target target = hit.transform.GetComponent<Target>();
            if (target)
            {
                target.RotateTarget();
            }

            tracer.transform.position = hit.point;

            if (health)
                m_PoolingManager.ActivateBlood(hit);
            else
                m_PoolingManager.ActivateDecal(hit);
        }
        else
        {
            tracer.transform.position = poitT.position;
        }

        currentRate = 0f;
        currentAmmo--;

        // VFX
        MuzzleEffect.Play(true);
        WeaponAnimator.Play("Fire", 0, 0);

        shotCounter++;

        if (weaponType == WeaponType.Automatic)
        {
            if (shotCounter > 3)
                RecoilMath();
        }
        else
        {
            RecoilMath();
        }

        // SFX
        ShootAudioSource.PlayOneShot(ShootSfx);

        // Event
        OnShoot?.Invoke();

        return true;
    }

    public void RecoilMath()
    {
        m_PlayerMovement.y_Recoil = recoilY;
        m_PlayerMovement.x_Recoil = Random.Range(recoilMinMax.x, recoilMinMax.y);
        Invoke(nameof(ResetRecoil), 0.1f);
    }

    void ResetRecoil()
    {
        m_PlayerMovement.y_Recoil = 0f;
        m_PlayerMovement.x_Recoil = 0f;
    }

    #region Reload
    public bool CheckReload() => !IsReloading;

    public void Reload()
    {
        if (meleeAttack)
            return;

        if (WeaponAnimator)
            WeaponAnimator.Play("Reload", 0, 0);

        if (ReloadSfx)
            ShootAudioSource.PlayOneShot(ReloadSfx);

        Invoke(nameof(ReloadDelay), ReloadTime);
        IsReloading = true;
    }

    public void ReloadDelay()
    {
        currentAmmo = MaxAmmo;
        IsReloading = false;
        HandleHud();
    }
    #endregion

    public void InitiateMeleeAttack()
    {
        WeaponAnimator.Play("MeleeAttack", 0, 0);
        meleeAttack = true;
    }

    public void FinishMeleeAttack()
    {
        meleeAttack = false;
    }

    public void HandleHud()
    {
        if (currentAmmo < 10)
            AmmoText.SetText($"0{currentAmmo}");
        else
            AmmoText.SetText($"{currentAmmo}");
    }

    public void EquipWeapon()
    {
        canEquip = true;
    }

    public void SetOnHands()
    {
        onHands = true;
    }

    public void UnequipWeapon()
    {
        onHands = false;
        WeaponAnimator.Play("UnequipWeapon", 0, 0);
    }

    public void DisableWeapon()
    {
        gameObject.SetActive(false);
    }
}

[System.Serializable]
public struct CrosshairData
{
    public Sprite CrosshairSprite;
    public float CrosshairSize;
    public Color CrosshairColor;
}

[System.Serializable]
public struct WeaponPosition
{
    public Transform defaltTransform;
    public Transform holsterTransform;
    public Transform rootObject;
    public float movementSpeed;
}