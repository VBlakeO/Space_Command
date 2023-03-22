using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public Image equipRifleImage = null;
    public WeaponSystem rifle = null;
    [Space]

    public Transform Targets = null;
    public Transform TargetsFinalPosition = null;
    public float targetInterpolationSpeed = 3;
    [Space]

    public Transform portal = null;
    public Collider portalCollider = null;
    public float portalInterpolationSpeed = 4;

    public string nextSceneName;

    private bool disableRifleMessage = false;
    private bool moveTargets = false;
    private bool openPortal = false;

    private void Start()
    {
        portal.localScale = new Vector3(0f, 1f, 0f);
    }

    public void MoveTarget()
    {
        moveTargets = true;
    }

    public void ActiveRifleMessage()
    {
        equipRifleImage.enabled = true;
    }    
    
    public void OpenPortal()
    {
        openPortal = true;
    }

    public void OpenNewScene()
    {
        Invoke(nameof(LoadNewScene), 0.2f);
    }

    private void FixedUpdate()
    {
        if (rifle.onHands && !disableRifleMessage)
        {
            disableRifleMessage = true;
        }

        if (equipRifleImage.enabled)
        {
            if (disableRifleMessage)
                equipRifleImage.fillAmount -= Time.deltaTime;
            else
                equipRifleImage.fillAmount += Time.deltaTime;
        }

        if (moveTargets)
            Targets.position = Vector3.Lerp(Targets.position, TargetsFinalPosition.position, targetInterpolationSpeed * Time.deltaTime);

        if (openPortal)
        {
            if (!portalCollider.enabled)
                portalCollider.enabled = true;
           
            portal.localScale = Vector3.Lerp(portal.localScale, new Vector3(1, 1, 1), portalInterpolationSpeed * Time.deltaTime);
        }
    }

    private void LoadNewScene()
    {
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
}
