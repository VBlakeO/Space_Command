using UnityEngine;

public class AI_WeaponSystem : MonoBehaviour
{
    [Header("Weapon Setting")]
    public float weaponRange = 0f;
    public float weaponDamage = 30f;
    public float delayBetweenShots = 0f;

    public Transform fowardIA = null;

    public WeaponType weaponType = WeaponType.Manual;
    public LayerMask attainableLayers;

    [Header("Ammo Parameters")]
    public int MaxAmmo = 15;

    [Header("Reload")]
    public float ReloadTime = 0f;

    [Header("VFX")]
    public Transform BulletPoint = null;
    public ParticleSystem MuzzleEffect = null;
    public TrailRenderer TacerEffect = null;

    [Header("SFX")]
    public AudioClip ShootSfx;
    public AudioSource ShootAudioSource;

    public bool IsReloading { get; private set; }
    
    private int currentAmmo = 0;
    private float currentRate = 0f;

    public Animator ai_anim;
    public bool attack;
    private bool CanShoot()
    {
        return currentAmmo > 0 && !IsReloading && currentRate >= delayBetweenShots;
    }

    private void Awake()
    {
        ShootAudioSource = GetComponent<AudioSource>();
        currentRate = delayBetweenShots;
        currentAmmo = MaxAmmo;
    }

    void FixedUpdate()
    {
        if (!attack)
            return;

        if (currentRate < delayBetweenShots)
            currentRate += Time.deltaTime;
        else
            TryShoot();

        if (currentAmmo <= 0)
        {
            if (CheckReload())
                Reload();
        }
    }

    private bool TryShoot()
    {
        if (!CanShoot())
            return false;

        var tracer = Instantiate(TacerEffect, BulletPoint.transform.position, Quaternion.identity);
        tracer.AddPosition(BulletPoint.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(fowardIA.position, fowardIA.forward, out hit, weaponRange, attainableLayers, QueryTriggerInteraction.Ignore))
        {
            Damageable damageable = hit.transform.GetComponent<Damageable>();
            if (damageable && !hit.transform.GetComponentInParent<NewBase_AI>())
                damageable.InflictDamage(Random.Range(0, weaponDamage), false, null);

            tracer.transform.position = hit.point;
        }

        currentRate = 0f;
        currentAmmo--;

        // VFX
        MuzzleEffect.Play(true);
        ai_anim.Play("Firing", 0, 0);

        // SFX
        ShootAudioSource.PlayOneShot(ShootSfx);
        return true;
    }

    public bool CheckReload() => !IsReloading;

    public void Reload()
    {
        Invoke(nameof(ReloadDelay), ReloadTime);
        IsReloading = true;
    }

    public void ReloadDelay()
    {
        currentAmmo = MaxAmmo;
        IsReloading = false;
    }

}
