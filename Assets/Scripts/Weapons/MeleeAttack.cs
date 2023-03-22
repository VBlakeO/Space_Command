using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeleeAttack : MonoBehaviour
{
    public static MeleeAttack m_Instance;

    public float damage = 0f;
    public float maxDistance = 4f;
    public LayerMask attainable = 0;

    [Tooltip("VFX prefab to spawn upon impact")]
    public Transform ImpactPoint;
    public float Radius = 0.01f;
    public float CheckDistance = 2;

    [Tooltip("Clip to play on impact")]
    public AudioClip ImpactSfxClip;

    public bool canPunch = true;

    public UnityAction OnPunching;
    public UnityAction OnPunchingEnd;

    private Color DebugColor;
    public Color NomalDebugColor;
    public Color HitDebugColor;

    private Damageable damageable = null;

    private bool punching;
    private Animator m_Anim = null;
    private List<Collider> m_IgnoredColliders;

    private HudManager m_HudManager = null;
    private PlayerMovement m_PlayerMovement = null;
    private MeleeAttackDetection m_MeleeAttackDetection = null;

    private void Awake()
    {
        m_Instance = this;
        m_Anim = GetComponent<Animator>();
    }

    void Start()
    {
        m_HudManager = HudManager.m_Instance;
        m_PlayerMovement = PlayerMovement.m_Instance;
        m_MeleeAttackDetection = GetComponentInChildren<MeleeAttackDetection>();
    }

    public bool Attack()
    {
        if (!canPunch || punching)
            return false;

        m_IgnoredColliders = new List<Collider>();

        // Ignore colliders of owner
        Collider[] ownerColliders = m_PlayerMovement.GetComponentsInChildren<Collider>();
        m_IgnoredColliders.AddRange(ownerColliders);

        OnPunching?.Invoke();
        m_Anim.Play("Attack", 0, 0);
        punching = true;

        return true;
    }

    private void FixedUpdate()
    {
        if (m_MeleeAttackDetection.detected)
            m_HudManager.canPunchImage.enabled = true;
        else
            m_HudManager.canPunchImage.enabled = false;
    }



    public void EndAttack()
    {
        punching = false;
        OnPunchingEnd?.Invoke();
    }

    public void ApllyDamage()
    {
        RaycastHit[] hits = Physics.SphereCastAll(ImpactPoint.position, Radius, ImpactPoint.transform.forward, CheckDistance, attainable, QueryTriggerInteraction.Ignore);
        {
            foreach (var _hit in hits)
            {
                if (_hit.transform.GetComponentInParent<Damageable>())
                {
                    float _distance = Vector3.Distance(m_PlayerMovement.cameraFps.transform.position, _hit.transform.position);
                    if (IsHitValid(_hit) && _distance <= maxDistance)
                    {
                        if (damageable != _hit.transform.GetComponentInParent<Damageable>())
                            OnHit(_hit.point, _hit.normal, _hit.collider);

                        damageable = _hit.transform.GetComponentInParent<Damageable>();
                    }
                }

            }
        }
        
        damageable = null;
    }

    bool IsHitValid(RaycastHit hit)
    {
        if (hit.collider.GetComponent<IgnoreHitDetection>())
        {
            return false;
        }

        if (m_IgnoredColliders != null && m_IgnoredColliders.Contains(hit.collider))
        {
            return false;
        }

        return true;
    }

    void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        Damageable damageable = collider.GetComponentInParent<Damageable>();
        if (damageable)
        {
            damageable.InflictDamage(damage, false, m_PlayerMovement.gameObject);
        }

        // impact sfx
        if (ImpactSfxClip)
        {
            // TODO: Impact SFX
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = DebugColor;
        Gizmos.DrawSphere(ImpactPoint.position + (ImpactPoint.forward * CheckDistance), Radius);
    }
}