using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public PlayerMovement playerMovement = null;
    public Transform nextFloor = null;

    public void ChangePlayerFloor()
    {
        playerMovement.m_Controller.enabled = false;
        playerMovement.transform.position = new Vector3(playerMovement.transform.position.x, nextFloor.position.y, playerMovement.transform.position.z);
        playerMovement.m_Controller.enabled = true;
        playerMovement.cantJump = false;
    }

}
