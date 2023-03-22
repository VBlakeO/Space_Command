using UnityEngine.AI;
using UnityEngine;

public class NewBase_AI : Health
{
    public Transform m_target;
    public float rotationSpeed = 10f;

    public float distance = 0f;

    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody[] rigs;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Rigidbody rootRigidbory;
    [HideInInspector] public PlayerMovement playController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rigs = GetComponentsInChildren<Rigidbody>();

        OnDie += Ragdoll;
        rootRigidbory = rigs[0];
        agent.updateRotation = true;

        for (int i = 0; i < rigs.Length; i++)
            rigs[i].isKinematic = true;
    }

    protected override void Start()
    {
        base.Start();
        playController = PlayerMovement.m_Instance;
        m_target = playController.middle.transform;
    }

    protected virtual void OnEnable()
    {
        playController = PlayerMovement.m_Instance;
        m_target = playController.middle.transform;
    }

    protected virtual void Ragdoll()
    {
        OnDie -= Ragdoll;

        Rigidbody[] rig = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rig.Length; i++)
            rig[i].isKinematic = false;

        anim.enabled = false;
        agent.enabled = false;
        rootRigidbory.AddForce(playController.cameraFps.transform.forward * 50, ForceMode.Impulse);

        Destroy(gameObject, 4f);
    }
}
