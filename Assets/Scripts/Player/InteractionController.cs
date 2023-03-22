using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public static InteractionController m_Instance;

    public float range = 3f;
    public Transform head = null;
    public LayerMask attainableLayers;

    private InteractiveObject tempObject = null;
    private HudManager m_HudManager = null;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        m_HudManager = HudManager.m_Instance;
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(head.position, head.forward, out hit, range, attainableLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.transform.GetComponent<InteractiveObject>())
            {
                m_HudManager.SetInteractionBaseState(false);
                return;
            }

            if (!hit.transform.GetComponent<InteractiveObject>().canInteract)
            {
                m_HudManager.SetInteractionBaseState(false);
                return;
            }

            m_HudManager.SetInteractionBaseState(true);

            if (Input.GetKey(KeyCode.E))
            {
                tempObject = hit.transform.GetComponentInChildren<InteractiveObject>();
             
                if (!tempObject)
                    return;

                tempObject.OnFinishInteractionAction += ClearTempObject;
                tempObject.Interacting();

                m_HudManager.SetInterectionBarProgress(tempObject.GetProgress());
            }
            else
            {
                if (!tempObject)
                    return;

                tempObject.OnFinishInteractionAction -= ClearTempObject;

                tempObject.StopInteraction();
                m_HudManager.SetInterectionBarProgress(tempObject.GetProgress());
            }

        }
        else
        {
            m_HudManager.SetInteractionBaseState(false);
            ClearTempObject();
        }
    }
    private void ClearTempObject()
    {
        if (!tempObject)
            return;
        tempObject.OnFinishInteractionAction -= ClearTempObject;
        m_HudManager.SetInterectionBarProgress(0f);
        m_HudManager.SetInteractionBaseState(false);
        tempObject.StopInteraction();

        tempObject = null;
    }
}
