using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewScene : MonoBehaviour
{
    public string nextSceneName;

    public void LoadScene()
    {
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
}
