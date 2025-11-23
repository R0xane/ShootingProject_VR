using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAction;
    public InputActionProperty gripAction;
    public Animator handAnimator;

    void Update()
    {
        float triggerValue = pinchAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);

        Debug.Log($"Trigger: {triggerValue}, Grip: {gripValue}");
    }
}
