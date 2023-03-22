using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public float currentTime = 100f;
    public float loadSpeed = 10f;
    public float disloadSpeed = 10f;

    public bool shieldLoaded = false;
    public bool usingShield = false;

    private HudManager m_HudManager = null;
    private Health playerLife = null;

    private void Start()
    {
        m_HudManager = HudManager.m_Instance;
        playerLife = GetComponent<Health>();
    }

    public bool ActivateShield()
    {
        if (shieldLoaded)
        {
            usingShield = true;
            m_HudManager.ActivateHelmetShield(true);

            playerLife.invincible = true;
        }

        return shieldLoaded;
    }

    private void Update()
    {
        if (usingShield)
        {
            if (currentTime > 0)
                currentTime -= Time.deltaTime * disloadSpeed;
            else
            {
                usingShield = false;
                m_HudManager.ActivateHelmetShield(false);

                playerLife.invincible = false;
            }
        }
        else
        {
            if (currentTime < 100)
                currentTime += Time.deltaTime * loadSpeed;
            else
                m_HudManager.SetActivatableShield();
        }

        shieldLoaded = currentTime >= 100;
        m_HudManager.ShieldIconControl(currentTime * 0.01f);
    }
}
