using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour
{
    public static HudManager m_Instance;
    
    [Header("Crosshair")]
    public Image CrosshairImage = null;
    public Image canPunchImage = null;
    [Space]

    [Header("Life Bar")]
    public Image lifeBar = null;
    public Image healBar = null;

    public Health m_PlayerLife = null;
    public TextMeshProUGUI medKitNum = null;
    [Space]

    [Header("Shield")]
    public Image shieldIcon = null;
    public Image helmetShield = null;
    public Image shieldBackground = null;
    public Color[] shieldIconColor;
    [Space]

    [Header("Interaction")]
    public Image interectionBar = null;
    public Animator interectionBase = null;
    [Space]

    [Header("Goal")]
    public Image goalImage = null;
    public Transform goal = null;
    [Space]

    public Animator HudHelmetAnim = null;
    public Transform mainCamera = null;
    public Transform head = null;
    [Space]

    private bool enableShield = false;
    private bool interactionControl =  false;

    private float goalHorizontalDirection = 0f;
    private float goalVerticalDirection = 0f;
    private float goalFowardDirection = 0f;

    private float goal_V = 0f;
    private float goal_H = 0f;

    private void Awake()
    {
        m_Instance = this;
        HudHelmetAnim.speed = 0;
    }

    public void SetCrosshairState(bool state)
    {
        CrosshairImage.enabled = state;
    }

    public void SetCrosshairStyle(CrosshairData Data)
    {
        CrosshairImage.sprite = Data.CrosshairSprite;
        CrosshairImage.color = Data.CrosshairColor;
        CrosshairImage.transform.localScale = new Vector3(Data.CrosshairSize, Data.CrosshairSize, Data.CrosshairSize);
    }

    public void SetActivatableShield()
    {
        shieldIcon.color = shieldIconColor[2];
    }

    public void ActivateHelmetShield(bool state)
    {
        enableShield = state;

        if (state)
            shieldIcon.color = shieldIconColor[1];
        else
            shieldIcon.color = shieldIconColor[0];
    }
   
    public void ShieldIconControl(float value)
    {
        shieldIcon.fillAmount = value;
    }

    public void SetInteractionBaseState(bool state)
    {
        if (state)
        {
            if (!interactionControl)
            {
                interectionBase.Play("OpenInteraction", 0, 0);
                interactionControl = true;
            }
        }
        else
        {
            if (interactionControl)
            {
                interectionBase.Play("CloseInteraction", 0, 0);
                interactionControl = false;
            }
        }
    }

    public void SetInterectionBarProgress(float progress)
    {
        interectionBar.fillAmount = progress;
    }

    private void FixedUpdate()
    {
        if (enableShield)
        {
            if (helmetShield.fillAmount < 1)
                helmetShield.fillAmount += Time.deltaTime;
            else if (!shieldBackground.enabled)
                shieldBackground.enabled = true;
        }
        else
        {
            if (helmetShield.fillAmount > 0)
                helmetShield.fillAmount -= Time.deltaTime;
        }

        if (goal && goal.gameObject.activeInHierarchy)
        {
            if (Vector3.Distance(head.position, goal.position) > 4)
            {
                FollowGoal();
            }
            else if (goalImage.enabled)
            {
                goalImage.enabled = false;
            }
        }

        lifeBar.fillAmount = m_PlayerLife.CurrentHealth * 0.01f;
    }

    private void FollowGoal()
    {
        goalHorizontalDirection = Vector3.Dot(head.right, -goal.forward);
        goalVerticalDirection = Vector3.Dot(mainCamera.forward, goal.up);
        goalFowardDirection = Vector3.Dot(head.forward, -goal.forward);

        if ((goalHorizontalDirection >= 0.73 || goalHorizontalDirection <= -0.73) || (goalVerticalDirection > 0.6 || goalVerticalDirection < -0.6))
        {
            goalImage.enabled = true;
        }
        else if (goalFowardDirection > 0 && !(goalVerticalDirection > 0.6 || goalVerticalDirection < -0.6))
        {
            goalImage.enabled = false;
        }

        if (goalHorizontalDirection > 0.56)
        {
            goal_H = 900f;
            goalImage.rectTransform.rotation = Quaternion.Euler(0, 0, 90);

        }
        else if (goalHorizontalDirection < -0.56)
        {
            goal_H = -900f;
            goalImage.rectTransform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (goalFowardDirection > 0)
        {
            goal_H = 0f;
        }


        if (goalVerticalDirection > 0.4)
        {
            if (goalHorizontalDirection >= -0.73 && goalHorizontalDirection <= 0.73)
                goalImage.rectTransform.rotation = Quaternion.Euler(0, 0, 0);

            goal_V = -490;
        }
        else if (goalVerticalDirection < -0.4)
        {
            if (goalHorizontalDirection >= -0.73 && goalHorizontalDirection <= 0.73)
                goalImage.rectTransform.rotation = Quaternion.Euler(0, 0, 180);

            goal_V = 490;
        }
        else
        {
            goal_V = 0;
        }

        goalImage.rectTransform.anchoredPosition = Vector3.Lerp(goalImage.rectTransform.anchoredPosition, new Vector3(goal_H, goal_V, 0), Time.deltaTime * 5);
    }

    public float Healing(float time)
    {
        return healBar.fillAmount += time;
    }

    public void ChangeMadKitValue(int value)
    {
        if (value < 10)
          medKitNum.text = "0" + value.ToString();
        else
          medKitNum.text = value.ToString();
    }
}
