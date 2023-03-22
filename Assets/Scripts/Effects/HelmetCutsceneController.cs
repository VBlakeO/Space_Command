using UnityEngine;

public class HelmetCutsceneController : MonoBehaviour
{
    public GameObject room = null;
    public GameObject Helmet = null;
    public GameObject m_light = null;

    public Transform PlayerSpawn = null;
    public PlayerMovement Player = null;
    public Animator HudHelmetAnim = null;

    private void Start()
    {
        GetComponent<Animator>().speed = 0;
    }

    public void ActivateCutscene()
    {
        GetComponent<Animator>().speed = 1;
        Helmet.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateCutscene();
        }
    }

    public void PutOnHelmet()
    {
        HudHelmetAnim.speed = 1;
    }

    public void DeactivateCutscene()
    {
        Player.transform.position = PlayerSpawn.transform.position;
        Player.head.transform.rotation = Quaternion.Euler(37, 0, 0);
        Player.headPitch = 37;

        Player.cantMove = false;
        Player.cantLook = false;
        Player.cantJump = false;

        m_light.SetActive(true);
        room.SetActive(false);
        gameObject.SetActive(false);
    }
}
