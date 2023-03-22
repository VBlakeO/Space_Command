using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    public float interactionTime = 0f;
    public float newInteractionDelay = 0f;
    public float interactionEventDelay = 0f;
    [Space]

    public bool canInteract = true;
    public bool quickInteraction = true;
    public bool uniqueInteraction = false;
    public bool continuousInteraction = false;
    [Space]

    public UnityEvent OnFinishInteractionEvent = null;
    public UnityAction OnFinishInteractionAction = null;

    public UnityEvent OnStopInteractionEvent = null;
    public UnityAction OnStopInteractionAction = null;

    public UnityEvent OnInteractingEvent = null;
    public UnityAction OnInteractingAction = null;

    public UnityEvent OnFinishInteractionEventWithDelay = null;
    public UnityAction OnFinishInteractionActionWithDelay = null;


    public float currentInteractionTime = 0f;

    private bool exhaustedInteractions = false;
    private bool interactionInterval = false;
    private bool interacted = false;

    public float GetInterectionTime()
    {
        return currentInteractionTime;
    }

    public float GetProgress()
    {
        return currentInteractionTime / interactionTime;
    }

    public void Interacting()
    {
        if (!canInteract || exhaustedInteractions || interactionInterval)
            return;

        OnInteractingEvent?.Invoke();
        OnInteractingAction?.Invoke();

        if (quickInteraction)
        {
            if (!interacted)
            {
                interacted = true;

                Invoke(nameof(OnInteract), 0);
            }
        }
        else
        {
            currentInteractionTime = GetProgress() < 1 ? currentInteractionTime += Time.deltaTime : currentInteractionTime = 1;

            if (GetProgress() >= 1 && !interacted)
            {
                interacted = true;

                Invoke(nameof(OnInteract), 0);
                currentInteractionTime = 0;
            }
        }
    }

    public void StopInteraction()
    {
        if (currentInteractionTime == 0)
            return;

        OnStopInteractionEvent?.Invoke();
        OnStopInteractionAction?.Invoke();

        if (!continuousInteraction)
            currentInteractionTime = 0f;
    }

    protected virtual void OnInteract()
    {
        OnFinishInteractionEvent?.Invoke();
        OnFinishInteractionAction?.Invoke();

        interacted = false;
        interactionInterval = true;
        currentInteractionTime = 0f;

        if (interactionEventDelay > 0)
            Invoke(nameof(OnInteractDelay), interactionEventDelay);

        if (newInteractionDelay > 0)
            Invoke(nameof(InteractionCooldown), newInteractionDelay);

        if (uniqueInteraction)
            exhaustedInteractions = true;
    }

    private void OnInteractDelay()
    {
        OnFinishInteractionEventWithDelay?.Invoke();
        OnFinishInteractionActionWithDelay?.Invoke();
    }


    public void ResetInteraction()
    {
        exhaustedInteractions = false;
    }

    public void SetCanInteract(bool state)
    {
        canInteract = state;
    }

    private void InteractionCooldown()
    {
        interactionInterval = false;
    }
}
