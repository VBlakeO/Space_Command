using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager m_Instance;

    public int medKits = 0;

    public WeaponManager m_WeaponManager = null;
    public PlayerShield m_PlayerShield = null;
    private HudManager m_HudManager = null;
    public Health m_PlayerLife = null;
   
    private bool canHealing = true;
    private float healingCurrentRate = 0f;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        m_HudManager = HudManager.m_Instance;

        AddMedKit(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            m_WeaponManager.MeleeAttackSkill();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            m_PlayerShield.ActivateShield();
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.G) && canHealing && medKits > 0)
        {
            if (m_HudManager.Healing(Time.deltaTime) >= 1)
            {
                m_PlayerLife.Heal(m_PlayerLife.MaxHealth * 0.8f);
                m_HudManager.healBar.fillAmount = 0f;
                healingCurrentRate = 0;
                canHealing = false;
                medKits--;
                NotifyHudManager();
            }
        }

        if(!Input.GetKey(KeyCode.G) && m_HudManager.healBar.fillAmount > 0f)
            m_HudManager.healBar.fillAmount = 0f;

        HealingCooldown();
    }

    private void HealingCooldown()
    {
        if (healingCurrentRate < 1)
            healingCurrentRate += Time.deltaTime;
        else if (!canHealing)
            canHealing = true;
    }

    public void AddMedKit(int kits)
    {
        medKits += kits;
        NotifyHudManager();
    }

    private void NotifyHudManager()
    {
        m_HudManager.ChangeMadKitValue(medKits);
    }

}
