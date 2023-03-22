using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public Transform[] m_poolingDecal;
    public Transform[] m_poolingBlood;    

    int activateDecal = 0;
    int activateBlood = 0;

    public void ActivateDecal(RaycastHit hit)
    {
        if (activateDecal < m_poolingDecal.Length)
        {
            m_poolingDecal[activateDecal].position = hit.point + hit.normal * 0.01f;
            m_poolingDecal[activateDecal].forward = hit.normal;
            m_poolingDecal[activateDecal].SetParent(hit.transform);
            m_poolingDecal[activateDecal].gameObject.SetActive(true);

            activateDecal++;
        }
        else
        {
            activateDecal = 0;
            m_poolingDecal[activateDecal].position = hit.point + hit.normal * 0.01f;
            m_poolingDecal[activateDecal].forward = hit.normal;
            m_poolingDecal[activateDecal].SetParent(hit.transform);
            m_poolingDecal[activateDecal].gameObject.SetActive(true);

            activateDecal++;
        }
    }

    public void ActivateBlood(RaycastHit hit)
    {
        if (activateBlood < m_poolingBlood.Length)
        {
            m_poolingBlood[activateBlood].position = hit.point + hit.normal * 0.01f;
            m_poolingBlood[activateBlood].rotation = Quaternion.LookRotation(hit.normal);
            m_poolingBlood[activateBlood].SetParent(null);
            m_poolingBlood[activateBlood].gameObject.SetActive(true);

            activateBlood++;
        }
        else
        {
            activateBlood = 0;
            m_poolingBlood[activateBlood].position = hit.point + hit.normal * 0.01f;
            m_poolingBlood[activateBlood].rotation = Quaternion.LookRotation(hit.normal);
            m_poolingBlood[activateBlood].SetParent(null);
            m_poolingBlood[activateBlood].gameObject.SetActive(true);

            activateBlood++;
        }
    }
}
