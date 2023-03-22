using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Health m_PlayerHealth = null;
    public LoadNewScene m_LoadNewScene = null;

    private void Start()
    {
        if (m_PlayerHealth)
            m_PlayerHealth.OnDie += ResetScene;
    }

    private void ResetScene()
    {
        if (m_LoadNewScene)
            m_LoadNewScene.LoadScene();
    }
}
